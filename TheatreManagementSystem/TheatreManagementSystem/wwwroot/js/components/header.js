namespace TheatreManagementSystem.wwwroot.js.components
// File: wwwroot/js/components/header.js
document.addEventListener('DOMContentLoaded', function () {
    const headerElement = document.getElementById('header');
    if (!headerElement) return;

    // Determine active link
    const currentPath = window.location.pathname;

    // Get authentication status (will be implemented with your auth logic)
    const isAuthenticated = localStorage.getItem('authToken') !== null;
    const userRole = localStorage.getItem('userRole') || 'USER';
    const username = localStorage.getItem('username') || 'User';

    // Build the header HTML
    let headerHtml = `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand" href="/">
                <i class="fas fa-film me-2"></i>Theatre Management
            </a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link ${currentPath === '/' ? 'active' : ''}" href="/">Home</a>
                    </li>`;

    // Navigation for regular users
    if (!['ROLE_ADMIN', 'ROLE_MANAGER'].includes(userRole)) {
        headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/movies') ? 'active' : ''}" href="/movies">Movies</a>
                    </li>`;

        if (isAuthenticated) {
            headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/bookings') ? 'active' : ''}" href="/bookings">My Bookings</a>
                    </li>`;
        }

        headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath === '/about' ? 'active' : ''}" href="/about">About</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${currentPath === '/contact' ? 'active' : ''}" href="/contact">Contact Us</a>
                    </li>`;
    }

    // Navigation for admin and manager
    if (['ROLE_ADMIN', 'ROLE_MANAGER'].includes(userRole)) {
        headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/admin/users') ? 'active' : ''}" href="/admin/users">Users</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/admin/screenings') ? 'active' : ''}" href="/admin/screenings">Screenings</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/admin/movies') ? 'active' : ''}" href="/admin/movies">Movies</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/admin/theatres') ? 'active' : ''}" href="/admin/theatres">Theatres</a>
                    </li>`;

        // Admin Dashboard
        if (userRole === 'ROLE_ADMIN') {
            headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/admin/dashboard') ? 'active' : ''}" href="/admin/dashboard">Dashboard</a>
                    </li>`;
        }

        // Manager Dashboard
        if (userRole === 'ROLE_MANAGER' && userRole !== 'ROLE_ADMIN') {
            headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath.startsWith('/manager/dashboard') ? 'active' : ''}" href="/manager/dashboard">Dashboard</a>
                    </li>`;
        }
    }

    headerHtml += `
                </ul>
                <ul class="navbar-nav">`;

    // Authentication links
    if (!isAuthenticated) {
        headerHtml += `
                    <li class="nav-item">
                        <a class="nav-link ${currentPath === '/auth/login' ? 'active' : ''}" href="/auth/login">Login</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link ${currentPath === '/auth/register' ? 'active' : ''}" href="/auth/register">Register</a>
                    </li>`;
    } else {
        headerHtml += `
                    <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                            <i class="fas fa-user me-1"></i>
                            <span>${username}</span>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li><a class="dropdown-item" href="/user/profile">My Profile</a></li>
                            <li ${['ROLE_ADMIN', 'ROLE_MANAGER'].includes(userRole) ? 'style="display:none"' : ''}><a class="dropdown-item" href="/bookings">My Bookings</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li>
                                <a class="dropdown-item" href="#" id="logoutLink">Logout</a>
                            </li>
                        </ul>
                    </li>`;
    }

    headerHtml += `
                </ul>
            </div>
        </div>
    </nav>`;

    // Set the header HTML
    headerElement.innerHTML = headerHtml;

    // Add event listener for logout
    const logoutLink = document.getElementById('logoutLink');
    if (logoutLink) {
        logoutLink.addEventListener('click', function (e) {
            e.preventDefault();
            // Implement logout logic here
            fetch('/api/auth/logout', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            })
                .then(response => {
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('userRole');
                    localStorage.removeItem('username');
                    window.location.href = '/';
                })
                .catch(error => console.error('Logout failed:', error));
        });
    }
});
