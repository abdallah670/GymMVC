// Trainer Profile Preview Function
function previewProfilePicture(input) {
  if (input.files && input.files[0]) {
    const reader = new FileReader();
    reader.onload = function (e) {
      const container = document.getElementById("profilePictureContainer");
      if (container) {
        container.innerHTML = "";
        const img = document.createElement("img");
        img.src = e.target.result;
        img.className = "profile-picture rounded-circle";
        img.alt = "New Profile Picture";
        container.appendChild(img);
      }

      const message = document.querySelector(
        ".profile-picture-wrapper + .mt-2 small"
      );
      if (message) {
        message.innerHTML =
          '<span class="text-success">✓ New picture selected. Click Save Changes to update.</span>';
      }
    };
    reader.readAsDataURL(input.files[0]);
  }
}

// Form Submission Spinner
document.addEventListener("DOMContentLoaded", function () {
  const profileForm = document.getElementById("profileForm");
  if (profileForm) {
    profileForm.addEventListener("submit", function () {
      const submitBtn = this.querySelector('button[type="submit"]');
      if (submitBtn) {
        submitBtn.innerHTML =
          '<i class="fas fa-spinner fa-spin me-2"></i>Saving...';
        submitBtn.disabled = true;
      }
    });
  }
});
