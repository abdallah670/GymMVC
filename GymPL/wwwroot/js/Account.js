
        // Toggle password visibility
    function togglePassword(inputId) {
            const input = document.getElementById(inputId);
    const button = input.nextElementSibling.querySelector('i');

    if (input.type === 'password') {
        input.type = 'text';
    button.classList.remove('fa-eye');
    button.classList.add('fa-eye-slash');
    button.title = "Hide password";
            } else {
        input.type = 'password';
    button.classList.remove('fa-eye-slash');
    button.classList.add('fa-eye');
    button.title = "Show password";
            }
        }


     
        function checkPasswordMatch() {
            const newPassword = document.getElementById('newPassword').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const matchText = document.getElementById('passwordMatchText');

            if (!newPassword || !confirmPassword) {
                matchText.innerHTML = '<i class="fas fa-info-circle me-2"></i><span>Passwords must match</span>';
                matchText.className = 'text-muted d-flex align-items-center';
                return false;
            }

            if (newPassword === confirmPassword) {
                matchText.innerHTML = '<i class="fas fa-check-circle me-2 text-success"></i><span class="text-success">✓ Passwords match perfectly!</span>';
                matchText.className = 'd-flex align-items-center';
                return true;
            } else {
                matchText.innerHTML = '<i class="fas fa-times-circle me-2 text-danger"></i><span class="text-danger">✗ Passwords do not match</span>';
                matchText.className = 'd-flex align-items-center';
                return false;
            }
        }

        // Form submission handler
        document.getElementById('changePasswordForm').addEventListener('submit', function(e) {
            const submitBtn = document.getElementById('submitBtn');

    // Validate passwords match
    if (!checkPasswordMatch()) {
        e.preventDefault();
    alert('Passwords do not match. Please check and try again.');
    return false;
            }

    // Validate password strength
    const password = document.getElementById('newPassword').value;
    const strength = checkPasswordStrength(password);

    if (strength < 50) {
                const proceed = confirm('Your password strength is weak. Are you sure you want to continue?');
    if (!proceed) {
        e.preventDefault();
    return false;
                }
            }

    // Show loading state
    if (submitBtn) {
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Changing Password...';
    submitBtn.disabled = true;

                // Add slight delay for better UX
                setTimeout(() => {
        submitBtn.innerHTML = '<i class="fas fa-save me-2"></i>Change Password';
    submitBtn.disabled = false;
                }, 3000);
            }

    return true;
        });

    // Real-time validation
    document.getElementById('newPassword').addEventListener('input', function() {
        // checkPasswordStrength(this.value);
        checkPasswordMatch();
        });

    document.getElementById('confirmPassword').addEventListener('input', checkPasswordMatch);

    // Initialize with current password focus
    document.addEventListener('DOMContentLoaded', function() {
            const currentPasswordField = document.getElementById('currentPassword');
    if (currentPasswordField) {
        currentPasswordField.focus();

                // Add animation on focus
                setTimeout(() => {
        currentPasswordField.classList.add('highlight');
                    setTimeout(() => {
        currentPasswordField.classList.remove('highlight');
                    }, 1000);
                }, 500);
            }

            // Add input validation styling
            document.querySelectorAll('input').forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value.trim() === '') {
                this.classList.add('is-invalid');
            } else {
                this.classList.remove('is-invalid');
            }
        });
            });
        });
    // Initialize with current password focus
    document.addEventListener('DOMContentLoaded', function() {
            const currentPasswordField = document.getElementById('currentPassword');
    if (currentPasswordField) {
        currentPasswordField.focus();

                // Add animation on focus
                setTimeout(() => {
        currentPasswordField.classList.add('highlight');
                    setTimeout(() => {
        currentPasswordField.classList.remove('highlight');
                    }, 1000);
                }, 500);
            }

            // Add input validation styling
            document.querySelectorAll('input').forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value.trim() === '') {
                this.classList.add('is-invalid');
            } else {
                this.classList.remove('is-invalid');
            }
        });

    // Remove invalid class when user starts typing
    input.addEventListener('input', function() {
                    if (this.value.trim() !== '') {
        this.classList.remove('is-invalid');
                    }
                });
            });

            // Add focus effects
            document.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

    input.addEventListener('blur', function() {
        this.parentElement.classList.remove('focused');
                });
            });
        });


    document.getElementById('forgotPasswordForm').addEventListener('submit', function(e) {
            const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
                const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<div class="loading-spinner"></div>Sending...';
    submitBtn.disabled = true;

                // Re-enable after 5 seconds in case of error
                setTimeout(() => {
        submitBtn.innerHTML = originalText;
    submitBtn.disabled = false;
                }, 5000);
            }
        });

    // Focus on email field
    document.addEventListener('DOMContentLoaded', function() {
        document.getElementById('Email')?.focus();
        });
