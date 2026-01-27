// Member Index Logic
const loadedMembers = new Set();

document.addEventListener("DOMContentLoaded", function () {
  const memberIdsElement = document.getElementById("memberIdsData");
  if (!memberIdsElement) return;

  const data = JSON.parse(memberIdsElement.textContent);
  const memberIds = data.memberIds;
  const viewType = data.viewType;

  console.log(
    "Loading status for",
    memberIds.length,
    "members in",
    viewType,
    "view"
  );

  // Load status for each member with a small delay to prevent overwhelming
  memberIds.forEach((memberId, index) => {
    setTimeout(() => {
      if (!loadedMembers.has(memberId)) {
        loadMemberPlanStatus(memberId, viewType);
        loadedMembers.add(memberId);
      }
    }, index * 50);
  });
});

async function loadMemberPlanStatus(memberId, viewType) {
  try {
    const response = await fetch(
      "/Member/GetMemberPlanStatus?memberId=" + memberId
    );
    if (!response.ok) throw new Error("Network response was not ok");

    const data = await response.json();
    let hasWorkout = data.hasWorkoutPlan || false;
    let workoutAssignmentId = data.workoutAssignmentId;
    let hasDiet = data.hasDietPlan || false;
    let dietPlanAssignmentId = data.dietPlanAssignmentId;
    let isSuccess = data.success !== undefined ? data.success : true;

    updatePlanStatusUI(
      memberId,
      hasWorkout,
      workoutAssignmentId,
      hasDiet,
      dietPlanAssignmentId,
      viewType,
      !isSuccess
    );
  } catch (error) {
    console.error("Error loading plan status for member", memberId, error);
    updatePlanStatusUI(memberId, false, null, false, null, viewType, true);
  }
}

function updatePlanStatusUI(
  memberId,
  hasWorkout,
  workoutAssignmentId,
  hasDiet,
  dietPlanAssignmentId,
  viewType,
  isError
) {
  if (isError) {
    showErrorState(memberId, viewType);
    return;
  }

  if (viewType === "table") {
    updateTableView(
      memberId,
      hasWorkout,
      workoutAssignmentId,
      hasDiet,
      dietPlanAssignmentId
    );
  } else {
    updateGridView(
      memberId,
      hasWorkout,
      workoutAssignmentId,
      hasDiet,
      dietPlanAssignmentId
    );
  }
}

function updateTableView(
  memberId,
  hasWorkout,
  workoutAssignmentId,
  hasDiet,
  dietPlanAssignmentId
) {
  const workoutElement = document.getElementById("workout-status-" + memberId);
  const dietElement = document.getElementById("diet-status-" + memberId);
  const returnUrl = encodeURIComponent(
    window.location.pathname + window.location.search
  );

  if (workoutElement) {
    workoutElement.innerHTML = getPlanStatusContent(
      hasWorkout,
      workoutAssignmentId,
      "WorkoutPlanAssignment",
      returnUrl
    );
  }

  if (dietElement) {
    dietElement.innerHTML = getPlanStatusContent(
      hasDiet,
      dietPlanAssignmentId,
      "DietPlanAssignment",
      returnUrl
    );
  }
}

function updateGridView(
  memberId,
  hasWorkout,
  workoutAssignmentId,
  hasDiet,
  dietPlanAssignmentId
) {
  const workoutContainer = document.getElementById(
    "grid-workout-container-" + memberId
  );
  const dietContainer = document.getElementById(
    "grid-diet-container-" + memberId
  );
  const returnUrl = encodeURIComponent(
    window.location.pathname + window.location.search
  );

  if (workoutContainer) {
    workoutContainer.innerHTML = getPlanStatusContent(
      hasWorkout,
      workoutAssignmentId,
      "WorkoutPlanAssignment",
      returnUrl
    );
  }

  if (dietContainer) {
    dietContainer.innerHTML = getPlanStatusContent(
      hasDiet,
      dietPlanAssignmentId,
      "DietPlanAssignment",
      returnUrl
    );
  }
}

function getPlanStatusContent(hasPlan, assignmentId, controller, returnUrl) {
    if (hasPlan && assignmentId) {
        return `
            <div class="d-flex flex-column align-items-center">
                <i class="fas fa-check-circle mb-2" style="color: var(--success); font-size: 1.25rem;"></i>
                <a href="/${controller}/Details?id=${assignmentId}&returnUrl=${returnUrl}" 
                   style="padding: 2px 8px; font-size: 0.65rem; background: color-mix(in srgb, var(--success) 20%, transparent); color: var(--success); border: 1px solid var(--success); border-radius: 4px; text-decoration: none; display: flex; align-items: center; gap: 4px;">
                    <i class="fas fa-eye"></i> Details
                </a>
            </div>
        `;
    } else if (hasPlan) {
        return '<i class="fas fa-check-circle" style="color: var(--success); font-size: 1.25rem;"></i>';
    } else {
        return '<i class="fas fa-times-circle" style="color: var(--error); font-size: 1.25rem;"></i>';
    }
}

function showErrorState(memberId, viewType) {
  const containers = [
    document.getElementById("workout-status-" + memberId),
    document.getElementById("diet-status-" + memberId),
    document.getElementById("grid-workout-container-" + memberId),
    document.getElementById("grid-diet-container-" + memberId),
  ];

  containers.forEach((container) => {
    if (container) {
      container.innerHTML = `
                <span class="badge badge-danger">
                    <i class="fas fa-exclamation-triangle mr-1"></i> Error
                </span>
            `;
    }
  });

  const icons = [
    document.getElementById("grid-workout-icon-" + memberId),
    document.getElementById("grid-diet-icon-" + memberId),
  ];

  icons.forEach((icon) => {
    if (icon) icon.classList.add("text-danger");
  });
}
