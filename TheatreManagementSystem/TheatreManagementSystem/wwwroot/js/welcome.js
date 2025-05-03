namespace TheatreManagementSystem.wwwroot.js

document.addEventListener('DOMContentLoaded', function () {
    // Initialize AOS animation library
    AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true,
        mirror: false
    });

    // Load header and footer
    loadHeaderFooter();

    // Load now playing movies
    fetchNowPlayingMovies();

    // Check authentication status and update UI accordingly
    checkAuthStatus();
});

/**
 * Load the header and footer components
 */
function loadHeaderFooter() {
    // Load header
    fetch('/components/header.html')
        .then(response => response.text())
        .then(html => {
            document.getElementById('header').innerHTML = html;
            // After loading header, check if we need to update auth state in the UI
            updateHeaderAuthState();
        })
        .catch(error => console.error('Error loading header:', error));

    // Load footer
    fetch('/components/footer.html')
        .then(response => response.text())
        .then(html => {
            document.getElementById('footer').innerHTML = html;
        })
        .catch(error => console.error('Error loading footer:', error));
}

/**
 * Fetch now playing movies from the API
 */
function fetchNowPlayingMovies() {
    // API endpoint for now playing movies
    fetch('/api/movies/current-playing')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(movies => {
            displayMovies(movies);
        })
        .catch(error => {
            console.error('Error fetching movies:', error);
            displayMovieError();
        });
}

/**
 * Display movies in the movie grid
 * @param {Array} movies - The array of movie objects
 */
function displayMovies(movies) {
    const movieGrid = document.getElementById('now-playing-movies');

    // Clear any existing content
    movieGrid.innerHTML = '';

    // If no movies, display a message
    if (!movies || movies.length === 0) {
        movieGrid.innerHTML = `
            <div class="empty-state text-center py-5">
                <div class="empty-icon mb-4">
                    <i class="bi bi-film text-muted" style="font-size: 4rem;"></i>
                </div>
                <h4 class="mb-3">No Movies Currently Playing</h4>
                <p class="lead text-muted mb-4">Check back soon for new releases and showtimes.</p>
                <a href="/movies/upcoming" class="btn btn-primary rounded-pill px-4 py-2">View Upcoming Movies</a>
            </div>
        `;
        return;
    }

    // Limit to 4 movies for the welcome page
    const displayMovies = movies.slice(0, 4);

    // Create and append movie cards
    displayMovies.forEach((movie, index) => {
        const movieCard = document.createElement('div');
        movieCard.className = 'movie-card';
        movieCard.setAttribute('data-aos', 'fade-up');
        movieCard.setAttribute('data-aos-delay', (index + 1) * 100);

        // Use placeholder image if no poster is available
        const posterUrl = movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster';

        movieCard.innerHTML = `
            <div class="movie-poster">
                <img src="${posterUrl}" alt="${movie.title} Poster">
                <div class="movie-overlay">
                    <a href="/movies/${movie.id}" class="btn btn-primary">Book Now</a>
                </div>
            </div>
            <div class="movie-info">
                <h3 class="movie-title">${movie.title}</h3>
                <div class="movie-meta">
                    <span>${movie.genre || 'N/A'}</span>
                    <span>${movie.durationMinutes || '0'} min</span>
                </div>
            </div>
        `;

        movieGrid.appendChild(movieCard);
    });
}

/**
 * Display an error message when movies cannot be loaded
 */
function displayMovieError() {
    const movieGrid = document.getElementById('now-playing-movies');
    movieGrid.innerHTML = `
        <div class="alert alert-danger text-center w-100" role="alert">
            <i class="fas fa-exclamation-circle me-2"></i>
            Unable to load movies. Please try again later.
        </div>
    `;
}

/**
 * Check authentication status
 */
function checkAuthStatus() {
    // Get the authentication token from local storage
    const token = localStorage.getItem('authToken');

    // If token exists, validate it with the server
    if (token) {
        // Make a request to a validate endpoint
        fetch('/api/auth/validate', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    // If token is invalid, remove it
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('user');
                    updateHeaderAuthState(false);
                } else {
                    // Token is valid
                    updateHeaderAuthState(true);
                }
            })
            .catch(error => {
                console.error('Error validating token:', error);
                // On error, assume token is invalid
                localStorage.removeItem('authToken');
                localStorage.removeItem('user');
                updateHeaderAuthState(false);
            });
    } else {
        // No token, user is not authenticated
        updateHeaderAuthState(false);
    }
}

/**
 * Update the header UI based on authentication state
 * @param {boolean} isAuthenticated - Whether the user is authenticated
 */
function updateHeaderAuthState(isAuthenticated) {
    // Check if header is loaded and contains auth elements
    const authNav = document.querySelector('.navbar-nav:last-child');
    if (!authNav) return;

    const loginItem = authNav.querySelector('a[href="/auth/login"]')?.parentElement;
    const registerItem = authNav.querySelector('a[href="/auth/register"]')?.parentElement;
    const userDropdown = authNav.querySelector('.dropdown');

    if (isAuthenticated) {
        // User is authenticated, show dropdown and hide login/register
        if (loginItem) loginItem.style.display = 'none';
        if (registerItem) registerItem.style.display = 'none';
        if (userDropdown) userDropdown.style.display = 'block';

        // Update username in dropdown if available
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        const userNameElement = userDropdown?.querySelector('.dropdown-toggle span');
        if (userNameElement && user.username) {
            userNameElement.textContent = user.username;
        }
    } else {
        // User is not authenticated, hide dropdown and show login/register
        if (loginItem) loginItem.style.display = 'block';
        if (registerItem) registerItem.style.display = 'block';
        if (userDropdown) userDropdown.style.display = 'none';
    }
}