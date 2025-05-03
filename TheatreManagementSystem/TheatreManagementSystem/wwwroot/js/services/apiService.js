namespace TheatreManagementSystem.wwwroot.js.services
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
        }

        return headers;
    },

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
    }
};