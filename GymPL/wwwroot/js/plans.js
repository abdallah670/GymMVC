// Plans Logic
function toggleRemainingDays() {
  var container = document.getElementById("remaining-days-container");
  var text = document.getElementById("toggle-text");
  var icon = document.getElementById("toggle-icon");

  if (container && container.style.display === "none") {
    container.style.display = "block";
    if (text) text.textContent = "Hide Remaining Days";
    if (icon) icon.className = "fas fa-chevron-up";
  } else if (container) {
    container.style.display = "none";
    if (text) text.textContent = "Show Remaining Days";
    if (icon) icon.className = "fas fa-chevron-down";
  }
}

document.addEventListener("DOMContentLoaded", function () {
  // Shared plan logic if needed
});
