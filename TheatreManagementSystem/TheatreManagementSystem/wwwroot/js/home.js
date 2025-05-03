/**
 * Home page JavaScript
 * This script handles loading data for the home page
 */

document.addEventListener('DOMContentLoaded', function () {
    // Load header and footer
    loadHeaderAndFooter();

    // Load now playing movies
    loadNowPlayingMovies();

    // Load upcoming movies
    loadUpcomingMovies();

    // Setup auth state listener
    setupAuthStateListener();
});

/**
 * Load header and footer components
 */
function loadHeaderAndFooter() {
    // Load header
    fetch('/pages/fragments/header-static.html')
        .then(response => response.text())
        .then(data => {
            document.getElementById('header').innerHTML = data;
            updateHeaderUI();
        })
        .catch(error => console.error('Error loading header:', error));

    // Load footer
    fetch('/pages/fragments/footer-static.html')
        .then(response => response.text())
        .then(data => {
            document.getElementById('footer').innerHTML = data;
        })
        .catch(error => console.error('Error loading footer:', error));
}

/**
 * Update header UI based on authentication status
 */
function updateHeaderUI() {
    const token = localStorage.getItem('jwtToken');
    const username = localStorage.getItem('username');
    const userRole = localStorage.getItem('userRole');

    const navbarNav = document.getElementById('navbarNav');
    if (!navbarNav) return;

    // Find user dropdown
    const userDropdown = document.querySelector('.dropdown-toggle');
    const loginLink = document.querySelector('a[href="/auth/login"]');
    const registerLink = document.querySelector('a[href="/auth/register"]');

    if (token && username) {
        // User is logged in
        if (userDropdown) {
            userDropdown.innerHTML = `<i class="fas fa-user me-1"></i> ${username}`;
            userDropdown.parentElement.classList.remove('d-none');
        }

        if (loginLink) loginLink.parentElement.classList.add('d-none');
        if (registerLink) registerLink.parentElement.classList.add('d-none');

        // Show appropriate nav items based on role
        const adminItems = document.querySelectorAll('[data-role="admin"]');
        const managerItems = document.querySelectorAll('[data-role="manager"]');
        const userItems = document.querySelectorAll('[data-role="user"]');

        if (userRole === 'ROLE_ADMIN') {
            adminItems.forEach(item => item.classList.remove('d-none'));
            managerItems.forEach(item => item.classList.remove('d-none'));
            userItems.forEach(item => item.classList.add('d-none'));
        } else if (userRole === 'ROLE_MANAGER') {
            adminItems.forEach(item => item.classList.add('d-none'));
            managerItems.forEach(item => item.classList.remove('d-none'));
            userItems.forEach(item => item.classList.add('d-none'));
        } else {
            adminItems.forEach(item => item.classList.add('d-none'));
            managerItems.forEach(item => item.classList.add('d-none'));
            userItems.forEach(item => item.classList.remove('d-none'));
        }
    } else {
        // User is not logged in
        if (userDropdown) userDropdown.parentElement.classList.add('d-none');
        if (loginLink) loginLink.parentElement.classList.remove('d-none');
        if (registerLink) registerLink.parentElement.classList.remove('d-none');

        // Hide all role-specific items
        document.querySelectorAll('[data-role]').forEach(item => {
            item.classList.add('d-none');
        });
    }
}

/**
 * Load now playing movies
 */
function loadNowPlayingMovies() {
    const nowPlayingContainer = document.getElementById('nowPlayingMovies');

    // Show loading spinner
    nowPlayingContainer.innerHTML = `
        <div class="text-center py-5 w-100">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    `;

    // API call to get now playing movies
    fetch('/api/now-playing')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            displayMovies(nowPlayingContainer, data);

            // Also fetch screenings for each movie
            fetchScreeningsForMovies(data);
        })
        .catch(error => {
            console.error('Error fetching now playing movies:', error);
            nowPlayingContainer.innerHTML = `
                <div class="col-12 text-center py-5">
                    <div class="alert alert-danger">
                        Failed to load movies. Please try again later.
                    </div>
                </div>
            `;
        });
}

/**
 * Load upcoming movies
 */
function loadUpcomingMovies() {
    const upcomingContainer = document.getElementById('upcomingMovies');

    // Show loading spinner
    upcomingContainer.innerHTML = `
        <div class="text-center py-5 w-100">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    `;

    // API call to get upcoming movies
    fetch('/api/upcoming-movies')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            displayUpcomingMovies(upcomingContainer, data);
        })
        .catch(error => {
            console.error('Error fetching upcoming movies:', error);
            upcomingContainer.innerHTML = `
                <div class="col-12 text-center py-5">
                    <div class="alert alert-danger">
                        Failed to load upcoming movies. Please try again later.
                    </div>
                </div>
            `;
        });
}

/**
 * Display movies in the container
 * @param {HTMLElement} container - The container element
 * @param {Array} movies - Array of movie objects
 */
function displayMovies(container, movies) {
    if (!movies || movies.length === 0) {
        container.innerHTML = `
            <div class="col-12">
                <div class="empty-state text-center py-5">
                    <div class="empty-icon mb-4">
                        <i class="bi bi-film text-muted" style="font-size: 4rem;"></i>
                    </div>
                    <h4 class="mb-3">No Movies Currently Playing</h4>
                    <p class="lead text-muted mb-4">Check back soon for new releases and showtimes.</p>
                    <a href="/movies/upcoming" class="btn btn-primary rounded-pill px-4 py-2">View Upcoming Movies</a>
                </div>
            </div>
        `;
        return;
    }

    // Clear container
    container.innerHTML = '';

    // Add each movie card
    movies.forEach(movie => {
        const movieCard = document.createElement('div');
        movieCard.className = 'col-sm-6 col-md-4 col-lg-3 mb-4';
        movieCard.innerHTML = `
            <div class="movie-card card h-100 border-0 shadow-lg rounded-4 overflow-hidden">
                <div class="poster-wrapper position-relative overflow-hidden">
                    <img src="${movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster'}"
                         class="card-img-top movie-poster" alt="Movie Poster">

                    <div class="movie-overlay">
                        <span class="movie-rating bg-warning text-dark rounded-circle">
                            <span>${movie.rating || 'N/A'}</span>
                        </span>
                        <div class="movie-actions">
                            <a href="/movies/${movie.id}" class="btn btn-light rounded-circle">
                                <i class="bi bi-play-fill"></i>
                            </a>
                            <button class="btn btn-light rounded-circle ms-2 favorite-btn">
                                <i class="bi bi-heart"></i>
                            </button>
                        </div>
                    </div>
                </div>

                <div class="card-body py-4">
                    <h5 class="card-title movie-title mb-2">${movie.title}</h5>

                    <div class="movie-meta d-flex align-items-center mb-3">
                        <span class="badge rounded-pill ${getGenreClass(movie.genre)}">${movie.genre}</span>
                        <span class="mx-2">•</span>
                        <span class="text-muted duration">
                            <i class="bi bi-clock me-1"></i>
                            <span>${movie.durationMinutes} min</span>
                        </span>
                    </div>

                    <p class="card-text description">${truncateText(movie.description || 'No description available.', 85)}</p>

                    <!-- Available Screenings - will be populated later -->
                    <div class="mt-3">
                        <h6 class="text-muted mb-2">Available Screenings:</h6>
                        <div class="d-flex flex-wrap gap-2 screenings-container" id="screenings-${movie.id}">
                            <div class="loading-spinner">
                                <div class="spinner-border spinner-border-sm text-primary" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card-footer bg-white border-top-0 pb-4 pt-0">
                    <div class="d-grid gap-2">
                        <a href="/movies/${movie.id}" class="btn btn-primary btn-lg rounded-pill">
                            Book Tickets
                        </a>
                        <a href="/movies/${movie.id}" class="btn btn-outline-secondary rounded-pill">
                            View Details
                        </a>
                    </div>
                </div>
            </div>
        `;

        container.appendChild(movieCard);
    });
}

/**
 * Display upcoming movies in the container
 * @param {HTMLElement} container - The container element
 * @param {Array} movies - Array of movie objects
 */
function displayUpcomingMovies(container, movies) {
    if (!movies || movies.length === 0) {
        container.innerHTML = `
            <div class="col-12">
                <div class="empty-state text-center py-5">
                    <div class="empty-icon mb-4">
                        <i class="bi bi-calendar text-muted" style="font-size: 4rem;"></i>
                    </div>
                    <h4 class="mb-3">No Upcoming Movies</h4>
                    <p class="lead text-muted mb-4">Check back soon for future releases.</p>
                    <a href="/movies" class="btn btn-primary rounded-pill px-4 py-2">Browse Current Movies</a>
                </div>
            </div>
        `;
        return;
    }

    // Clear container
    container.innerHTML = '';

    // Add each movie card
    movies.forEach(movie => {
        const movieCard = document.createElement('div');
        movieCard.className = 'col-sm-6 col-md-4 col-lg-3 mb-4';
        movieCard.innerHTML = `
            <div class="movie-card card h-100 border-0 shadow-lg rounded-4 overflow-hidden">
                <div class="poster-wrapper position-relative overflow-hidden">
                    <img src="${movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster'}"
                         class="card-img-top movie-poster" alt="Movie Poster">

                    <div class="position-absolute top-0 end-0 m-3">
                        <span class="badge bg-warning text-dark px-3 py-2 rounded-pill">Coming Soon</span>
                    </div>

                    <div class="movie-overlay">
                        <div class="movie-actions">
                            <a href="/movies/${movie.id}" class="btn btn-light rounded-circle">
                                <i class="bi bi-play-fill"></i>
                            </a>
                            <button class="btn btn-light rounded-circle ms-2 favorite-btn">
                                <i class="bi bi-heart"></i>
                            </button>
                        </div>
                    </div>
                </div>

                <div class="card-body py-4">
                    <h5 class="card-title movie-title mb-2">${movie.title}</h5>

                    <div class="movie-meta d-flex align-items-center mb-3">
                        <span class="badge rounded-pill ${getGenreClass(movie.genre)}">${movie.genre}</span>
                        <span class="mx-2">•</span>
                        <span class="text-muted">
                            <i class="bi bi-calendar me-1"></i>
                            <span>${formatDate(movie.releaseDate)}</span>
                        </span>
                    </div>

                    <p class="card-text description">${truncateText(movie.description || 'No description available.', 85)}</p>
                </div>

                <div class="card-footer bg-white border-top-0 pb-4 pt-0">
                    <div class="d-grid">
                        <a href="/movies/${movie.id}" class="btn btn-outline-primary rounded-pill">
                            <i class="bi bi-info-circle me-2"></i>View Details
                        </a>
                    </div>
                </div>
            </div>
        `;

        container.appendChild(movieCard);
    });
}

/**
 * Fetch screenings for the given movies
 * @param {Array} movies - Array of movie objects
 */
function fetchScreeningsForMovies(movies) {
    if (!movies || movies.length === 0) return;

    movies.forEach(movie => {
        fetch(`/api/screenings/by-movie/${movie.id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(screenings => {
                displayScreenings(movie.id, screenings);
            })
            .catch(error => {
                console.error(`Error fetching screenings for movie ${movie.id}:`, error);
                const screeningsContainer = document.getElementById(`screenings-${movie.id}`);
                if (screeningsContainer) {
                    screeningsContainer.innerHTML = `<span class="text-muted">No screenings available</span>`;
                }
            });
    });
}

/**
 * Display screenings for a movie
 * @param {number} movieId - The movie ID
 * @param {Array} screenings - Array of screening objects
 */
function displayScreenings(movieId, screenings) {
    const screeningsContainer = document.getElementById(`screenings-${movieId}`);
    if (!screeningsContainer) return;

    if (!screenings || screenings.length === 0) {
        screeningsContainer.innerHTML = `<span class="text-muted">No screenings available</span>`;
        return;
    }

    // Clear container
    screeningsContainer.innerHTML = '';

    // Display up to 5 screenings
    const limitedScreenings = screenings.slice(0, 5);

    limitedScreenings.forEach(screening => {
        const time = new Date(screening.startTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

        const screeningBadge = document.createElement('a');
        screeningBadge.href = `/bookings/screening/${screening.id}`;
        screeningBadge.className = 'screening-time-badge text-decoration-none';
        screeningBadge.innerHTML = `
            <span class="badge bg-light text-dark border p-2">${time}</span>
        `;

        screeningsContainer.appendChild(screeningBadge);
    });

    // Add "more" link if there are more screenings
    if (screenings.length > 5) {
        const moreLink = document.createElement('a');
        moreLink.href = `/movies/${movieId}`;
        moreLink.className = 'text-primary small';
        moreLink.textContent = '+ more times';

        screeningsContainer.appendChild(moreLink);
    }
}

/**
 * Setup auth state listener
 */
function setupAuthStateListener() {
    // Check for token expiration
    const token = localStorage.getItem('jwtToken');
    if (token) {
        // Get expiration time
        const expiry = localStorage.getItem('tokenExpiry');
        if (expiry && new Date().getTime() > parseInt(expiry)) {
            // Token has expired, clear it
            localStorage.removeItem('jwtToken');
            localStorage.removeItem('username');
            localStorage.removeItem('userRole');
            localStorage.removeItem('tokenExpiry');

            // Update UI
            updateHeaderUI();
        }
    }
}

/**
 * Helper function to get badge class based on genre
 * @param {string} genre - The movie genre
 * @returns {string} The badge class
 */
function getGenreClass(genre) {
    switch (genre) {
        case 'ACTION':
            return 'bg-danger';
        case 'DRAMA':
            return 'bg-primary';
        case 'COMEDY':
            return 'bg-success';
        case 'HORROR':
            return 'bg-dark';
        case 'SCI_FI':
            return 'bg-info text-dark';
        case 'ROMANCE':
            return 'bg-danger';
        case 'ANIMATION':
            return 'bg-success';
        case 'THRILLER':
            return 'bg-dark';
        case 'FANTASY':
            return 'bg-primary';
        case 'ADVENTURE':
            return 'bg-warning text-dark';
        default:
            return 'bg-secondary';
    }
}

/**
 * Helper function to truncate text
 * @param {string} text - The text to truncate
 * @param {number} maxLength - Maximum length
 * @returns {string} The truncated text
 */
function truncateText(text, maxLength) {
    if (!text) return '';
    if (text.length <= maxLength) return text;

    return text.substr(0, maxLength) + '...';
}

/**
 * Helper function to format date
 * @param {string} dateString - The date string
 * @returns {string} The formatted date
 */
function formatDate(dateString) {
    if (!dateString) return 'TBA';

    const date =