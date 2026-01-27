function highlightJoinNow() {
  // Prevent default link behavior
  if (event) event.preventDefault();

  // Find the Join Now button in the header
  const joinButton = document.querySelector('a[href="/Member/Create"]');
  const joinInstruction = document.getElementById("joinInstruction");

  if (joinButton) {
    // Add highlight animation
    joinButton.classList.add("join-now-highlight");

    // Show instruction message
    if (joinInstruction) {
      joinInstruction.style.display = "block";
      joinInstruction.classList.add("join-now-flash");
    }

    // Scroll to the Join Now button smoothly
    joinButton.scrollIntoView({
      behavior: "smooth",
      block: "center",
    });

    // Remove animation after 8 seconds
    setTimeout(() => {
      joinButton.classList.remove("join-now-highlight");
      if (joinInstruction) {
        joinInstruction.style.display = "none";
      }
    }, 8000);

    // Optional: Add click event to remove animation when Join Now is clicked
    joinButton.addEventListener(
      "click",
      function () {
        this.classList.remove("join-now-highlight");
        if (joinInstruction) {
          joinInstruction.style.display = "none";
        }
      },
      { once: true }
    ); // Remove after one click
  }
}

document.addEventListener("DOMContentLoaded", function () {
  const getStartedButtons = document.querySelectorAll(".get-started-btn");

  getStartedButtons.forEach((button) => {
    button.addEventListener("click", function (e) {
      e.preventDefault();
      highlightJoinNow();
    });
  });
});
