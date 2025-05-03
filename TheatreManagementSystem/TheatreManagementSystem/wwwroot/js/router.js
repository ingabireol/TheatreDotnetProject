namespace TheatreManagementSystem.wwwroot.js
// File: wwwroot/js/router.js
document.addEventListener('DOMContentLoaded', function () {
    const router = {
        routes: [
            { path: '/', title: 'Home', htmlPath: '/pages/home.html', jsPath: '/js/pages/home.js' },
            { path: '/movies', title: 'Movies', htmlPath: '/pages/movies/list.html', jsPath: '/js/pages/movies/list.js' },
            { path: '/movies/:id', title: 'Movie Details', htmlPath: '/pages/movies/detail.html', jsPath: '/js/pages/movies/detail.js' },
            { path: '/bookings', title: 'My Bookings', htmlPath: '/pages/booking/bookings.html', jsPath: '/js/pages/booking/bookings.js', requireAuth: true },
            { path: '/bookings/:id', title: 'Booking Details', htmlPath: '/pages/booking/view.html', jsPath: '/js/pages/booking/view.js', requireAuth: true },
            { path: '/bookings/screening/:id', title: 'Select Seats', htmlPath: '/pages/booking/seat-selection.html', jsPath: '/js/pages/booking/seat-selection.js', requireAuth: true },
            { path: '/auth/login', title: 'Login', htmlPath: '/pages/auth/login.html', jsPath: '/js/pages/auth/login.js' },
            { path: '/auth/register', title: 'Register', htmlPath: '/pages/auth/register.html', jsPath: '/js/pages/auth/register.js' },
            { path: '/about', title: 'About Us', htmlPath: '/pages/about.html', jsPath: '/js/pages/about.js' },
            { path: '/contact', title: 'Contact Us', htmlPath: '/pages/contact.html', jsPath: '/js/pages/contact.js' },

            // Admin routes
            { path: '/admin/dashboard', title: 'Admin Dashboard', htmlPath: '/pages/admin/dashboard.html', jsPath: '/js/pages/admin/dashboard.js', requireAuth: true, requireRole: 'ROLE_ADMIN' },
            { path: '/admin/movies', title: 'Manage Movies', htmlPath: '/pages/admin/movies.html', jsPath: '/js/pages/admin/movies.js', requireAuth: true, requireRole: ['ROLE_ADMIN', 'ROLE_MANAGER'] },
            { path: '/admin/theatres', title: 'Manage Theatres', htmlPath: '/pages/admin/theatres.html', jsPath: '/js/pages/admin/theatres.js', requireAuth: true, requireRole: ['ROLE_ADMIN', 'ROLE_MANAGER'] },
            { path: '/admin/screenings', title: 'Manage Screenings', htmlPath: '/pages/admin/screenings.html', jsPath: '/js/pages/admin/screenings.js', requireAuth: true, requireRole: ['ROLE_ADMIN', 'ROLE_MANAGER'] },
            { path: '/admin/users', title: 'Manage Users', htmlPath: '/pages/admin/users.html', jsPath: '/js/pages/admin/users.js', requireAuth: true, requireRole: 'ROLE_ADMIN' },

            // Error routes
            { path: '/error/404', title: 'Page Not Found', htmlPath: '/pages/error/404.html' },
            { path: '/error/403', title: 'Access Denied', htmlPath: '/pages/error/403.html' },
            { path: '/error/500', title: 'Server Error', htmlPath: '/pages/error/500.html' }
        ],

        init: function () {
            // Handle initial page load
            this.navigate(window.location.pathname + window.location.search);

            // Handle back/forward navigation
            window.addEventListener('popstate', (event) => {
                this.navigate(window.location.pathname + window.location.search, true);
            });

            // Intercept link clicks
            document.addEventListener('click', (event) => {
                // Find closest anchor tag
                const anchor = event.target.closest('a');

                // Only handle internal links
                if (anchor && anchor.href.startsWith(window.location.origin) &&
                    !anchor.hasAttribute('target') && !anchor.hasAttribute('download')) {
                    event.preventDefault();

                    const path = anchor.pathname + anchor.search;
                    this.navigate(path);
                }
            });
        },

        findRoute: function (path) {
            // Extract path without query parameters
            const pathWithoutQuery = path.split('?')[0];

            // First look for exact matches
            let route = this.routes.find(r => r.path === pathWithoutQuery);
            if (route) return route;

            // Then look for parameterized routes
            const segments = pathWithoutQuery.split('/').filter(Boolean);

            for (const r of this.routes) {
                const routeSegments = r.path.split('/').filter(Boolean);

                if (segments.length !== routeSegments.length) continue;

                let isMatch = true;
                const params = {};

                for (let i = 0; i < routeSegments.length; i++) {
                    if (routeSegments[i].startsWith(':')) {
                        // This is a parameter
                        const paramName = routeSegments[i].substring(1);
                        params[paramName] = segments[i];
                    } else if (routeSegments[i] !== segments[i]) {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch) {
                    // Create a copy of the route with extracted params
                    const matchedRoute = { ...r, params };
                    return matchedRoute;
                }
            }

            // If no route found, return the 404 route
            return this.routes.find(r => r.path === '/error/404');
        },

        navigate: function (path, isPopState = false) {
            const route = this.findRoute(path);

            // Check authentication if required
            if (route.requireAuth && !authService.isAuthenticated()) {
                // Redirect to login
                if (!isPopState) {
                    window.history.pushState({}, '', '/auth/login?returnUrl=' + encodeURIComponent(path));
                }
                this.loadRoute(this.routes.find(r => r.path === '/auth/login'));
                return;
            }

            // Check role if required
            if (route.requireRole) {
                const userRole = authService.getUserRole();
                const requiredRoles = Array.isArray(route.requireRole) ? route.requireRole : [route.requireRole];

                if (!requiredRoles.includes(userRole)) {
                    // Redirect to 403 page
                    if (!isPopState) {
                        window.history.pushState({}, '', '/error/403');
                    }
                    this.loadRoute(this.routes.find(r => r.path === '/error/403'));
                    return;
                }
            }

            // Update browser history (if not triggered by popstate)
            if (!isPopState) {
                window.history.pushState({}, '', path);
            }

            // Load the route
            this.loadRoute(route);
        },

        loadRoute: function (route) {
            // Set page title
            document.title = route.title ? `${route.title} - Theatre Management System` : 'Theatre Management System';

            // Get content element
            const contentElement = document.getElementById('content');
            if (!contentElement) return;

            // Load page content
            fetch(route.htmlPath)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Error loading content: ${response.status}`);
                    }
                    return response.text();
                })
                .then(html => {
                    // Set content
                    contentElement.innerHTML = html;

                    // Load page script if provided
                    if (route.jsPath) {
                        const scriptElement = document.getElementById('page-script');
                        if (scriptElement) {
                            scriptElement.src = route.jsPath;

                            // Make route params available to the script
                            if (route.params) {
                                window.routeParams = route.params;
                            }
                        }
                    }
                })
                .catch(error => {
                    console.error('Error loading content:', error);
                    contentElement.innerHTML = `
                        <div class="container py-5 text-center">
                            <h2>Error Loading Content</h2>
                            <p class="lead text-muted">Sorry, there was a problem loading the page content.</p>
                            <a href="/" class="btn btn-primary">Go Home</a>
                        </div>
                    `;
                });
        }
    };

    // Initialize the router
    router.init();
});
