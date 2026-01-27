// Help & Support Scripts
document.addEventListener("DOMContentLoaded", function () {
  // Support form submission
  const supportForm = document.getElementById("supportForm");
  if (supportForm) {
    supportForm.addEventListener("submit", function (e) {
      e.preventDefault();

      const formData = new FormData();
      formData.append(
        "subject",
        document.getElementById("supportSubject").value
      );
      formData.append(
        "message",
        document.getElementById("supportMessage").value
      );
      formData.append(
        "urgent",
        document.getElementById("urgentSupport").checked
      );

      const attachments = document.getElementById("supportAttachments").files;
      for (let i = 0; i < attachments.length; i++) {
        formData.append("attachments", attachments[i]);
      }

      // Show loading
      const submitBtn = this.querySelector('button[type="submit"]');
      const originalText = submitBtn.innerHTML;
      submitBtn.innerHTML =
        '<i class="fas fa-spinner fa-spin me-2"></i>Sending...';
      submitBtn.disabled = true;

      // Submit to server
      fetch("/Home/SubmitSupportRequest", {
        method: "POST",
        body: formData,
      })
        .then((response) => response.json())
        .then((data) => {
          if (data.success) {
            alert(
              "Support request sent! We'll get back to you within 24 hours."
            );
            this.reset();
          } else {
            alert(data.message || "Failed to send support request");
          }
        })
        .catch((error) => {
          alert("Error sending support request");
          console.error(error);
        })
        .finally(() => {
          submitBtn.innerHTML = originalText;
          submitBtn.disabled = false;
        });
    });
  }

  // Initialize FAQ accordion
  // Auto-expand first FAQ item
  const firstFaqElement = document.querySelector(
    "#faqAccordion .accordion-collapse"
  );
  if (firstFaqElement && typeof bootstrap !== "undefined") {
    new bootstrap.Collapse(firstFaqElement, { toggle: true });
  }
});

// Live chat simulation
function openLiveChat() {
  alert(
    "Live chat would open here in a real application. For now, please use email or phone support."
  );
}
