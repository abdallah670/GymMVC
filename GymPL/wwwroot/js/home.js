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
      { once: true },
    ); // Remove after one click
  }
}

// Number Ticker Animation
function animateCounter(el) {
  const target = +el.getAttribute("data-target");
  const duration = 2000; // 2 seconds
  const stepTime = 20;
  const steps = duration / stepTime;
  const increment = target / steps;

  let current = 0;
  const timer = setInterval(() => {
    current += increment;
    if (current >= target) {
      el.innerText = target + "+";
      clearInterval(timer);
    } else {
      el.innerText = Math.ceil(current);
    }
  }, stepTime);
}

document.addEventListener("DOMContentLoaded", function () {
  // Reveal on Scroll
  const observerOptions = {
    threshold: 0.15,
    rootMargin: "0px 0px -50px 0px",
  };

  const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add("revealed");

        // Trigger counter if it's inside the hero
        if (entry.target.classList.contains("hero-content")) {
          document.querySelectorAll(".counter").forEach((counter) => {
            animateCounter(counter);
          });
        }

        observer.unobserve(entry.target);
      }
    });
  }, observerOptions);

  document.querySelectorAll(".reveal-on-scroll").forEach((el) => {
    observer.observe(el);
  });

  const getStartedButtons = document.querySelectorAll(".get-started-btn");
  if (getStartedButtons) {
    getStartedButtons.forEach((button) => {
      button.addEventListener("click", function (e) {
        e.preventDefault();
        highlightJoinNow();
      });
    });
  }
});
