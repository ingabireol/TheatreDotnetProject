namespace TheatreManagementSystem.wwwroot.js.utils
// File: wwwroot/js/utils/pageLoader.js
const pageLoader = {
    // Load content into the main content area
    loadContent: function (htmlPath, callback) {
        const contentElement = document.getElementById('content');
        if (!contentElement) return;

        fetch(htmlPath)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Error loading content: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                contentElement.innerHTML = html;
                if (typeof callback === 'function') {
                    callback();
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
    },

    // Set page title
    setTitle: function (title) {
        document.title = title ? `${title} - Theatre Management System` : 'Theatre Management System';
    },

    // Load CSS for a specific page
    loadCss: function (cssPath) {
        const pageCssLink = document.getElementById('page-css');
        if (pageCssLink) {
            pageCssLink.href = cssPath || '';
        }
    },

    // Initialize a page with content, title, and CSS
    initPage: function (options) {
        const { htmlPath, title, cssPath, callback } = options;

        // Set the page title
        this.setTitle(title);

        // Load page-specific CSS if provided
        this.loadCss(cssPath);

        // Load the page content
        this.loadContent(htmlPath, callback);
    }
};
