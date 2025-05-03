namespace TheatreManagementSystem.wwwroot.js.services
<<<<<<< HEAD
// apiService.js - A comprehensive service to interact with your ASP.NET backend

const API_URL = '/api'; // Base API URL

// Generic API service for communicating with the backend
const apiService = {
    // Authentication API calls
    auth: {
        login: async (credentials) => {
            return await apiService.post('/auth/login', credentials);
        },
        register: async (userData) => {
            return await apiService.post('/auth/register', userData);
        },
        logout: async () => {
            return await apiService.post('/auth/logout');
        },
        validateToken: async () => {
            return await apiService.get('/auth/validate');
        }
    },

    // Movies API calls
    movies: {
        getAll: async (filters = {}) => {
            const queryParams = new URLSearchParams();
            if (filters.title) queryParams.set('title', filters.title);
            if (filters.genre) queryParams.set('genre', filters.genre);

            return await apiService.get(`/movies?${queryParams.toString()}`);
        },
        getById: async (id) => {
            return await apiService.get(`/movies/${id}`);
        },
        getNowPlaying: async () => {
            return await apiService.get('/movies/current-playing');
        },
        getUpcoming: async () => {
            return await apiService.get('/movies/upcoming');
        },
        getGenres: async () => {
            const movies = await apiService.get('/movies');
            // Extract unique genres from movies
            const genres = [...new Set(movies.map(movie => movie.genre))];
            return genres;
        }
    },

    // Screening API calls
    screenings: {
        getAll: async (filters = {}) => {
            const queryParams = new URLSearchParams();
            if (filters.movieId) queryParams.set('movieId', filters.movieId);
            if (filters.theatreId) queryParams.set('theatreId', filters.theatreId);
            if (filters.date) queryParams.set('date', filters.date);

            return await apiService.get(`/screenings?${queryParams.toString()}`);
        },
        getById: async (id) => {
            return await apiService.get(`/screenings/${id}`);
        },
        getByMovie: async (movieId) => {
            return await apiService.get(`/screenings/by-movie/${movieId}`);
        },
        getBookedSeats: async (screeningId) => {
            return await apiService.get(`/screenings/${screeningId}/booked-seats`);
        }
    },

    // Booking API calls
    bookings: {
        getAll: async () => {
            return await apiService.get('/bookings/by-user');
        },
        getById: async (id) => {
            return await apiService.get(`/bookings/${id}`);
        },
        create: async (screeningId, selectedSeats, paymentMethod) => {
            return await apiService.post(`/bookings?screeningId=${screeningId}`, {
                selectedSeats,
                paymentMethod
            });
        },
        cancel: async (id) => {
            return await apiService.post(`/bookings/${id}/cancel`);
        },
        calculatePrice: async (screeningId, selectedSeats) => {
            const queryParams = new URLSearchParams();
            queryParams.set('screeningId', screeningId);
            selectedSeats.forEach(seat => queryParams.append('selectedSeats', seat));

            return await apiService.get(`/bookings/calculate-price?${queryParams.toString()}`);
        }
    },

    // Generic HTTP methods
    get: async (endpoint) => {
        try {
            const response = await fetch(`${API_URL}${endpoint}`, {
                method: 'GET',
                headers: apiService.getHeaders()
            });
            return await apiService.handleResponse(response);
        } catch (error) {
            console.error(`GET ${endpoint} error:`, error);
            throw error;
        }
    },

    post: async (endpoint, data) => {
        try {
            const response = await fetch(`${API_URL}${endpoint}`, {
                method: 'POST',
                headers: apiService.getHeaders(),
                body: JSON.stringify(data)
            });
            return await apiService.handleResponse(response);
        } catch (error) {
            console.error(`POST ${endpoint} error:`, error);
            throw error;
        }
    },

    put: async (endpoint, data) => {
        try {
            const response = await fetch(`${API_URL}${endpoint}`, {
                method: 'PUT',
                headers: apiService.getHeaders(),
                body: JSON.stringify(data)
            });
            return await apiService.handleResponse(response);
        } catch (error) {
            console.error(`PUT ${endpoint} error:`, error);
            throw error;
        }
    },

    delete: async (endpoint) => {
        try {
            const response = await fetch(`${API_URL}${endpoint}`, {
                method: 'DELETE',
                headers: apiService.getHeaders()
            });
            return await apiService.handleResponse(response);
        } catch (error) {
            console.error(`DELETE ${endpoint} error:`, error);
            throw error;
        }
    },

    // Helper functions
    getHeaders: () => {
        const headers = {
            'Content-Type': 'application/json'
        };

        // Add authorization header if user is logged in
        const token = localStorage.getItem('authToken');
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
=======
// File: wwwroot/js/services/apiService.js
const apiService = {
    // Base API URL
    baseUrl: '/api',

    // Get headers for API requests
    getHeaders: function (additionalHeaders = {}) {
        const headers = {
            'Content-Type': 'application/json',
            ...additionalHeaders
        };

        // Add auth token if available
        if (authService.isAuthenticated()) {
            headers['Authorization'] = `Bearer ${authService.getToken()}`;
>>>>>>> 9d3bc78ce5d6b2acdee2a57adbab601df2ef35b5
        }

        return headers;
    },

<<<<<<< HEAD
    handleResponse: async (response) => {
        const contentType = response.headers.get('content-type');
        const isJson = contentType && contentType.includes('application/json');
        const data = isJson ? await response.json() : await response.text();

        if (!response.ok) {
            // Handle different error status codes
            const error = new Error(isJson ? data.message || response.statusText : response.statusText);
            error.status = response.status;
            error.data = data;
            throw error;
        }

        return data;
=======
    // Generic request method
    request: async function (endpoint, method = 'GET', data = null, additionalHeaders = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const options = {
            method,
            headers: this.getHeaders(additionalHeaders)
        };

        if (data && (method === 'POST' || method === 'PUT' || method === 'PATCH')) {
            options.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(url, options);

            // Parse JSON response if it exists
            const contentType = response.headers.get('content-type');
            let responseData;

            if (contentType && contentType.includes('application/json')) {
                responseData = await response.json();
            } else {
                responseData = await response.text();
            }

            // Handle error responses
            if (!response.ok) {
                const error = new Error(responseData.message || 'API request failed');
                error.status = response.status;
                error.data = responseData;
                throw error;
            }

            return responseData;
        } catch (error) {
            console.error(`API error (${endpoint}):`, error);
            throw error;
        }
    },

    // Shorthand methods for common HTTP verbs
    get: function (endpoint, additionalHeaders = {}) {
        return this.request(endpoint, 'GET', null, additionalHeaders);
    },

    post: function (endpoint, data, additionalHeaders = {}) {
        return this.request(endpoint, 'POST', data, additionalHeaders);
    },

    put: function (endpoint, data, additionalHeaders = {}) {
        return this.request(endpoint, 'PUT', data, additionalHeaders);
    },

    patch: function (endpoint, data, additionalHeaders = {}) {
        return this.request(endpoint, 'PATCH', data, additionalHeaders);
    },

    delete: function (endpoint, additionalHeaders = {}) {
        return this.request(endpoint, 'DELETE', null, additionalHeaders);
>>>>>>> 9d3bc78ce5d6b2acdee2a57adbab601df2ef35b5
    }
};