namespace TheatreManagementSystem.wwwroot.js.pages.auth
// File: wwwroot/js/pages/auth/login.js
document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const loginForm = document.getElementById('loginForm');
    const loginButton = document.getElementById('loginButton');
    const errorAlert = document.getElementById('errorAlert');
    const errorMessage = document.getElementById('errorMessage');
    const successAlert = document.getElementById('successAlert');
    const successMessage = document.getElementById('successMessage');

    // Check if user is already logged in
    if (authService.isAuthenticated()) {
        window.location.href = '/';
        return;
    }

    // Check for success message in URL (e.g., after registration)
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('success')) {
        successMessage.textContent = decodeURIComponent(urlParams.get('success'));
        successAlert.classList.remove('d-none');
    }

    // Check for error message in URL
    if (urlParams.has('error')) {
        errorMessage.textContent = decodeURIComponent(urlParams.get('error'));
        errorAlert.classList.remove('d-none');
    }

    // Check for returnUrl
    const returnUrl = urlParams.get('returnUrl') || '/';

    // Handle form submission
    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Get form data
        const username = document.getElementById('username').value.trim();
        const password = document.getElementById('password').value;
        const rememberMe = document.getElementById('remember-me').checked;

        // Disable button and show loading state
        loginButton.disabled = true;
        loginButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Logging in...';

        // Hide any existing alerts
        errorAlert.classList.add('d-none');
        successAlert.classList.add('d-none');

        // Attempt to login
        authService.login({ username, password, rememberMe })
            .then(response => {
                // Show success message
                successMessage.textContent = 'Login successful. Redirecting...';
                successAlert.classList.remove('d-none');

                // Redirect to return URL or home page
                setTimeout(() => {
                    window.location.href = returnUrl;
                }, 1000);
            })
            .catch(error => {
                // Show error message
                errorMessage.textContent = error.message || 'Invalid username or password';
                errorAlert.classList.remove('d-none');

                // Reset button state
                loginButton.disabled = false;
                loginButton.textContent = 'Login';
            });
    });
});