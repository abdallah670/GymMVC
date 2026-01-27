// Registration & Profile Setup Scripts
document.addEventListener("DOMContentLoaded", function () {
  // 1. Membership Plan Selection (Create Page)
  const membershipSelect = document.getElementById("MembershipId");
  const planDetails = document.getElementById("planDetails");
  if (membershipSelect && planDetails) {
    membershipSelect.addEventListener("change", function () {
      const selectedOption = this.options[this.selectedIndex];

      if (selectedOption.value) {
        const planName = selectedOption.getAttribute("data-name");
        const planPrice = selectedOption.getAttribute("data-price");
        const planFeatures = selectedOption
          .getAttribute("data-features")
          .split("|");
        planDetails.innerHTML = `
                    <div class="plan-summary plan-summary-active">
                        <h4 style="color: var(--main-color); margin-bottom: 0.5rem; font-size: 1.5rem;">${planName} Plan</h4>
                        <div class="plan-price">
                            $${planPrice}<span style="font-size: 1rem; color: var(--muted);">/month</span>
                        </div>
                        <div style="margin-bottom: 1rem;">
                            <h5 style="color: var(--ink); margin-bottom: 0.5rem; font-weight: 600;">Plan Features:</h5>
                            <ul class="plan-features-list">
                                ${planFeatures
                                  .map(
                                    (feature) => `
                                    <li class="plan-feature-item">
                                        <i class="fas fa-check" style="color: var(--success); margin-top: 0.25rem; flex-shrink: 0;"></i>
                                        <span>${feature}</span>
                                    </li>
                                `
                                  )
                                  .join("")}
                            </ul>
                        </div>
                    </div>
                `;
      } else {
        planDetails.innerHTML = `
                    <div class="plan-summary">
                        <h4 style="color: var(--main-color); margin-bottom: 0.5rem;">Selected Plan</h4>
                        <p style="color: var(--muted);">Please select a plan to see details</p>
                    </div>
                `;
      }
    });

    // Trigger on load if pre-selected
    if (membershipSelect.value) {
      membershipSelect.dispatchEvent(new Event("change"));
    }
  }

  // 2. Form Validation (Create Page)
  const registrationForm = document.getElementById("Create");
  if (registrationForm) {
    registrationForm.addEventListener("submit", function (e) {
      const password = document.getElementById("Password").value;
      const confirmPassword = document.getElementById("ConfirmPassword").value;

      if (password !== confirmPassword) {
        e.preventDefault();
        alert("Passwords do not match!");
        return false;
      }
    });
  }

  // 3. Profile Setup Logic (Complete Page)
  const fileInput = document.getElementById("ProfileImageFile");
  const fileNameSpan = document.getElementById("fileName");
  const imagePreview = document.getElementById("imagePreview");
  const defaultPreview = document.getElementById("defaultPreview");
  const previewImage = document.getElementById("previewImage");
  const setupForm = document.querySelector(".setup-form");

  if (fileInput) {
    fileInput.addEventListener("change", function () {
      if (this.files && this.files[0]) {
        const file = this.files[0];

        // Update file name
        if (fileNameSpan) fileNameSpan.textContent = file.name;

        // Validate file size (5MB)
        if (file.size > 5 * 1024 * 1024) {
          alert("File size cannot exceed 5MB");
          this.value = "";
          if (fileNameSpan) fileNameSpan.textContent = "No file chosen";
          if (imagePreview) imagePreview.style.display = "none";
          return;
        }

        // Validate extension
        const allowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
        const ext = file.name
          .toLowerCase()
          .substring(file.name.lastIndexOf("."));
        if (!allowedExtensions.includes(ext)) {
          alert("Only JPG, JPEG, PNG, and GIF files are allowed");
          this.value = "";
          if (fileNameSpan) fileNameSpan.textContent = "No file chosen";
          return;
        }

        // Preview
        const reader = new FileReader();
        reader.onload = function (e) {
          if (previewImage) {
            previewImage.src = e.target.result;
            if (imagePreview) imagePreview.style.display = "block";
            if (defaultPreview) defaultPreview.style.display = "none";
          }
        };
        reader.readAsDataURL(file);
      }
    });
  }

  // 4. Form Submission Loading (Setup)
  if (setupForm) {
    setupForm.addEventListener("submit", function () {
      const submitBtn = this.querySelector('button[type="submit"]');
      if (submitBtn) {
        submitBtn.innerHTML =
          '<i class="fas fa-spinner fa-spin me-2"></i>Processing...';
        submitBtn.disabled = true;
      }
    });
  }
});
