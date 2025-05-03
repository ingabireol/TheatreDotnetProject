namespace TheatreManagementSystem.wwwroot.js.services
// File: wwwroot/js/services/authService.js
const authService = {
    // Base API URL
    apiUrl: '/api/auth',

    // Initialize authentication from localStorage
    init: function () {
        this.token = localStorage.getItem('authToken');
        this.username = localStorage.getItem('username');
        this.userRole = localStorage.getItem('userRole');

        return this.isAuthenticated();
    },

    // Check if user is authenticated
    isAuthenticated: function () {
        return this.token !== null && this.token !== undefined;
    },

    // Get the current user's role
    getUserRole: function () {
        return this.userRole;
    },

    // Get the current username
    getUsername: function () {
        return this.username;
    },

    // Get the authentication token
    getToken: function () {
        return this.token;
    },

    // Set authentication data after successful login
    setAuth: function (token, username, role) {
        this.token = token;
        this.username = username;
        this.userRole = role;

        // Save to localStorage
        localStorage.setItem('authToken', token);
        localStorage.setItem('username', username);
        localStorage.setItem('userRole', role);
    },

    // Clear authentication data on logout
    clearAuth: function () {
        this.token = null;
        this.username = null;
        this.userRole = null;

        // Remove from localStorage
        localStorage.removeItem('authToken');
        localStorage.removeItem('username');
        localStorage.removeItem('userRole');
    },

    // Register a new user
    register: async function (userData) {
        try {
            const response = await fetch(`${this.apiUrl}/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Registration failed');
            }

            return data;
        } catch (error) {
            console.error('Registration error:', error);
            throw error;
        }
    },

    // Login a user
    login: async function (credentials) {
        try {
            const response = await fetch(`${this.apiUrl}/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(credentials)
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Login failed');
            }

            // Set authentication data
            this.setAuth(data.token, credentials.username, data.role || 'ROLE_USER');

            return data;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    },

    // Logout the current user
    logout: async function () {
        try {
            if (this.isAuthenticated()) {
                const response = await fetch(`${this.apiUrl}/logout`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${this.token}`
                    }
                });

                // Clear authentication data regardless of response
                this.clearAuth();

                // If response is not successful, just log the error
                if (!response.ok) {
                    console.warn('Logout response not OK, but auth was still cleared');
                }
            }

            return true;
        } catch (error) {
            console.error('Logout error:', error);
            // Still clear auth even if API call fails
            this.clearAuth();
            return true;
        }
    }
};

// Initialize auth state when the script loads
authService.init();