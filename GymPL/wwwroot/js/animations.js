// Advanced Scroll Animations
document.addEventListener("DOMContentLoaded", function () {
  const observerOptions = {
    threshold: 0.1,
    rootMargin: "0px 0px -50px 0px",
  };

  const observer = new IntersectionObserver(function (entries) {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add("revealed");
        observer.unobserve(entry.target); // Only animate once
      }
    });
  }, observerOptions);

  // Elements to animate
  const animClasses =
    ".feature-card, .service-card, .testimonial-card, .pricing-card, .section-header, .details-card, .stat-item, .reveal-on-scroll";
  const elementsToAnimate = document.querySelectorAll(animClasses);

  elementsToAnimate.forEach((el, index) => {
    el.classList.add("reveal-on-scroll");

    // Check for staggered parent
    const parent = el.closest(
      ".features-grid, .services-grid, .testimonials-grid, .pricing-cards, .stats-glass"
    );
    if (parent) {
      const children = Array.from(parent.querySelectorAll(animClasses));
      const childIndex = children.indexOf(el);
      el.style.transitionDelay = `${childIndex * 0.1}s`;
    }

    observer.observe(el);
  });
});
