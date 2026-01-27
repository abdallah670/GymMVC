/* ===== Form Interaction Scripts ===== */

document.addEventListener("DOMContentLoaded", function () {
  // Checkbox Interaction (e.g. Remember Me)
  const checkbox = document.getElementById("rememberMe");
  const label = document.querySelector('label[for="rememberMe"]');

  if (label && checkbox) {
    label.addEventListener("click", function (e) {
      // Prevent default if label handles click natively, but here we want to ensure style/logic sync
      // e.preventDefault(); // Sometimes needed depending on structure
      // checkbox.checked = !checkbox.checked;
      // checkbox.dispatchEvent(new Event('change', { bubbles: true }));
    });

    checkbox.addEventListener("change", function () {
      // console.log('Checkbox checked:', this.checked);
    });
  }

  // Auto-focus on first password field if present
  const firstPwd = document.getElementById("Password");
  if (firstPwd) firstPwd.focus();

  // Initialize Password Logic if form exists
  const resetForm = document.getElementById("resetPasswordForm");
  const changePwdForm =
    document.getElementById("changePasswordForm"); /* Assuming ID */

  if (resetForm || changePwdForm) {
    const pwdInput = document.getElementById("Password");
    const confirmInput = document.getElementById("ConfirmPassword");

    if (pwdInput) {
      pwdInput.addEventListener("input", function () {
        checkPasswordStrength(this.value);
        if (confirmInput) checkPasswordMatch();
      });
    }

    if (confirmInput) {
      confirmInput.addEventListener("input", checkPasswordMatch);
    }
  }
});

// Toggle Password Visibility
function togglePassword(fieldId) {
  const input = document.getElementById(fieldId);
  if (!input) return;

  // Find icon - assumes icon is in a sibling button or next element
  // Structure in View: input + button > icon
  const btn = input.nextElementSibling;
  let icon = null;

  if (btn && btn.tagName === "BUTTON") {
    icon = btn.querySelector("i");
  } else {
    // Fallback look
    const group = input.parentElement;
    icon = group.querySelector(".fa-eye, .fa-eye-slash");
  }

  if (input.type === "password") {
    input.type = "text";
    if (icon) {
      icon.classList.remove("fa-eye");
      icon.classList.add("fa-eye-slash");
    }
  } else {
    input.type = "password";
    if (icon) {
      icon.classList.remove("fa-eye-slash");
      icon.classList.add("fa-eye");
    }
  }
}

// Password Strength Checker
function checkPasswordStrength(password) {
  let strength = 0;
  if (password.length >= 8) strength += 25;
  if (password.length >= 12) strength += 10;
  if (/[a-z]/.test(password)) strength += 15;
  if (/[A-Z]/.test(password)) strength += 15;
  if (/[0-9]/.test(password)) strength += 15;
  if (/[^A-Za-z0-9]/.test(password)) strength += 20;

  const bar = document.getElementById("passwordStrengthBar");
  const text = document.getElementById("passwordStrengthText");

  if (!bar || !text) return;

  bar.style.width = strength + "%";

  if (strength < 50) {
    bar.className = "progress-bar bg-danger";
    text.textContent = "Weak password";
    text.className = "text-danger";
  } else if (strength < 75) {
    bar.className = "progress-bar bg-warning";
    text.textContent = "Medium strength";
    text.className = "text-warning";
  } else {
    bar.className = "progress-bar bg-success";
    text.textContent = "Strong password";
    text.className = "text-success";
  }
}

// Password Match Checker
function checkPasswordMatch() {
  const password = document.getElementById("Password").value;
  const confirm = document.getElementById("ConfirmPassword").value;
  const matchText = document.getElementById("passwordMatchText");

  if (!matchText) return;

  if (!password || !confirm) {
    matchText.textContent = "";
    return false;
  }

  if (password === confirm) {
    matchText.textContent = "✓ Passwords match";
    matchText.className = "text-success";
    return true;
  } else {
    matchText.textContent = "✗ Passwords do not match";
    matchText.className = "text-danger";
    return false;
  }
}

// Theme Toggle Logic
document.addEventListener("DOMContentLoaded", function () {
  const currentTheme = localStorage.getItem("theme") || "dark";

  function updateIcons(isLight) {
    const buttons = document.querySelectorAll(".theme-toggle-btn");
    buttons.forEach((btn) => {
      btn.innerHTML = isLight
        ? '<i class="fas fa-sun"></i>'
        : '<i class="fas fa-moon"></i>';
    });
  }

  // Initial Sync
  if (currentTheme === "light") {
    document.documentElement.setAttribute("data-theme", "light");
    updateIcons(true);
  } else {
    document.documentElement.removeAttribute("data-theme");
    updateIcons(false);
  }

  // Event Delegation for Theme Button
  document.addEventListener("click", function (e) {
    const btn = e.target.closest(".theme-toggle-btn");
    if (btn) {
      const isLight =
        document.documentElement.getAttribute("data-theme") === "light";
      if (isLight) {
        document.documentElement.removeAttribute("data-theme");
        localStorage.setItem("theme", "dark");
        updateIcons(false);
      } else {
        document.documentElement.setAttribute("data-theme", "light");
        localStorage.setItem("theme", "light");
        updateIcons(true);
      }
    }
  });
});
