namespace TheatreManagementSystem.wwwroot.js.pages.movies
// File: wwwroot/js/pages/movies/list.js
document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const moviesContainer = document.getElementById('moviesContainer');
    const emptyState = document.getElementById('emptyState');
    const loadingIndicator = document.getElementById('loadingIndicator');
    const successAlert = document.getElementById('successAlert');
    const errorAlert = document.getElementById('errorAlert');
    const successMessage = document.getElementById('successMessage');
    const errorMessage = document.getElementById('errorMessage');
    const filterForm = document.getElementById('movieFilterForm');
    const genreSelect = document.getElementById('genre');

    // Parse URL search params for initial filters
    const urlParams = new URLSearchParams(window.location.search);
    const initialQuery = urlParams.get('query') || '';
    const initialGenre = urlParams.get('genre') || '';
    const initialDate = urlParams.get('date') || '';

    // Set initial form values
    document.getElementById('query').value = initialQuery;
    document.getElementById('date').value = initialDate;

    // Show success message if passed in URL
    const msgSuccess = urlParams.get('success');
    if (msgSuccess) {
        showAlert(successAlert, successMessage, decodeURIComponent(msgSuccess));
    }

    // Show error message if passed in URL
    const msgError = urlParams.get('error');
    if (msgError) {
        showAlert(errorAlert, errorMessage, decodeURIComponent(msgError));
    }

    // Load genres for dropdown
    loadGenres();

    // Load initial movies
    loadMovies(initialQuery, initialGenre, initialDate);

    // Handle filter form submission
    filterForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const query = document.getElementById('query').value.trim();
        const genre = document.getElementById('genre').value;
        const date = document.getElementById('date').value;

        // Update URL with query parameters
        const params = new URLSearchParams();
        if (query) params.set('query', query);
        if (genre) params.set('genre', genre);
        if (date) params.set('date', date);

        const newUrl = `${window.location.pathname}${params.toString() ? '?' + params.toString() : ''}`;
        window.history.pushState({}, '', newUrl);

        // Load filtered movies
        loadMovies(query, genre, date);
    });

    // Load genres from API
    function loadGenres() {
        apiService.get('/movies/genres')
            .then(genres => {
                // Clear existing options except the first one
                genreSelect.innerHTML = '<option value="">All Genres</option>';

                // Add genres to select
                genres.forEach(genre => {
                    const option = document.createElement('option');
                    option.value = genre;
                    option.textContent = genre;
                    if (genre === initialGenre) {
                        option.selected = true;
                    }
                    genreSelect.appendChild(option);
                });
            })
            .catch(error => {
                console.error('Error loading genres:', error);
            });
    }

    // Load movies from API
    function loadMovies(query = '', genre = '', date = '') {
        // Show loading indicator
        loadingIndicator.classList.remove('d-none');
        moviesContainer.classList.add('d-none');
        emptyState.classList.add('d-none');

        // Build API endpoint with query parameters
        let endpoint = '/movies';
        const params = new URLSearchParams();
        if (query) params.set('title', query);
        if (genre) params.set('genre', genre);
        if (date) params.set('date', date);
        if (params.toString()) {
            endpoint += '?' + params.toString();
        }

        // Fetch movies from API
        apiService.get(endpoint)
            .then(response => {
                // Hide loading indicator
                loadingIndicator.classList.add('d-none');

                if (response.length === 0) {
                    // Show empty state if no movies found
                    emptyState.classList.remove('d-none');
                    return;
                }

                // Show movies container
                moviesContainer.classList.remove('d-none');

                // Build movie cards
                renderMovies(response);
            })
            .catch(error => {
                // Hide loading indicator
                loadingIndicator.classList.add('d-none');

                // Show error message
                showAlert(errorAlert, errorMessage, 'Error loading movies: ' + (error.message || 'Unknown error'));

                // Show empty state
                emptyState.classList.remove('d-none');

                console.error('Error loading movies:', error);
            });
    }

    // Render movies to the container
    function renderMovies(movies) {
        // Clear existing content
        moviesContainer.innerHTML = '';

        // For each movie, create a card and append to container
        movies.forEach(movie => {
            // Create movie card
            const movieCard = document.createElement('div');
            movieCard.className = 'col-sm-6 col-lg-4 mb-4';

            // Format movie duration
            const duration = movie.durationMinutes ? `${movie.durationMinutes} min` : 'N/A';

            // Get poster URL or use placeholder
            const posterUrl = movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster';

            // Truncate description if too long
            const shortDescription = movie.description ?
                (movie.description.length > 100 ? movie.description.substring(0, 97) + '...' : movie.description) :
                'No description available.';

            // Create HTML for the movie card
            movieCard.innerHTML = `
                <div class="card h-100 shadow-sm movie-card">
                    <div class="poster-wrapper position-relative overflow-hidden">
                        <img src="${posterUrl}" class="card-img-top movie-poster" alt="${movie.title} Poster">
                        <div class="movie-overlay">
                            <div class="d-flex justify-content-end w-100 p-2">
                                <span class="badge bg-secondary">${movie.rating || 'N/A'}</span>
                            </div>
                            <div class="movie-actions p-3">
                                <a href="/movies/${movie.id}" class="btn btn-primary">
                                    <i class="fas fa-ticket-alt me-2"></i>Book Tickets
                                </a>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title movie-title">${movie.title}</h5>
                        <div class="d-flex align-items-center mb-2">
                            <span class="badge bg-primary me-2">${movie.genre || 'N/A'}</span>
                            <span class="text-muted"><i class="fas fa-clock me-1"></i>${duration}</span>
                        </div>
                        <p class="card-text movie-description text-muted mb-3">${shortDescription}</p>
                        
                        <!-- Available Screenings Section -->
                        <h6 class="border-top pt-2 mb-2">Available Screenings:</h6>
                        <div id="screenings-${movie.id}" class="screening-times">
                            <div class="text-center">
                                <div class="spinner-border spinner-border-sm text-primary" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                </div>
                                <span class="ms-2">Loading screenings...</span>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer">
                        <a href="/movies/${movie.id}" class="btn btn-link text-decoration-none text-primary w-100">
                            View Details
                        </a>
                    </div>
                </div>
            `;

            // Append card to container
            moviesContainer.appendChild(movieCard);

            // Load screenings for this movie
            loadScreeningsForMovie(movie.id);
        });
    }

    // Load screenings for a specific movie
    function loadScreeningsForMovie(movieId) {
        const screeningsContainer = document.getElementById(`screenings-${movieId}`);

        apiService.get(`/screenings?movieId=${movieId}&upcoming=true&limit=3`)
            .then(screenings => {
                if (screenings.length === 0) {
                    screeningsContainer.innerHTML = `<div class="text-muted text-center py-2">No screenings available</div>`;
                    return;
                }

                let screeningsHtml = '';

                // Add up to 3 screenings
                screenings.slice(0, 3).forEach(screening => {
                    const screeningTime = new Date(screening.startTime);
                    const timeFormatted = screeningTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                    screeningsHtml += `
                        <div class="mb-2">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <span>${screening.theatreName}</span>
                                    <span class="badge bg-info ms-1">${screening.format}</span>
                                </div>
                                <a href="/bookings/screening/${screening.id}" class="btn btn-sm btn-outline-primary">
                                    ${timeFormatted}
                                </a>
                            </div>
                        </div>
                    `;
                });

                // Add "See all" link if there are more than 3 screenings
                if (screenings.length > 3) {
                    screeningsHtml += `
                        <div class="text-center mt-2">
                            <a href="/movies/${movieId}" class="btn btn-sm btn-link">
                                See all screenings
                            </a>
                        </div>
                    `;
                }

                screeningsContainer.innerHTML = screeningsHtml;
            })
            .catch(error => {
                screeningsContainer.innerHTML = `<div class="text-danger text-center py-2">Error loading screenings</div>`;
                console.error(`Error loading screenings for movie ${movieId}:`, error);
            });
    }

    // Helper function to show alerts
    function showAlert(alertElement, messageElement, message) {
        messageElement.textContent = message;
        alertElement.classList.remove('d-none');

        // Auto-hide after 5 seconds
        setTimeout(() => {
            alertElement.classList.add('d-none');
        }, 5000);
    }
});