namespace TheatreManagementSystem.wwwroot.js.components
// File: wwwroot/js/components/footer.js
document.addEventListener('DOMContentLoaded', function () {
    const footerElement = document.getElementById('footer');
    if (!footerElement) return;

    // Get the current year
    const currentYear = new Date().getFullYear();

    // Build the footer HTML
    const footerHtml = `
    <footer class="mt-auto py-4 bg-dark text-white">
        <div class="container">
            <div class="row">
                <div class="col-md-4">
                    <h5>Theatre Management System</h5>
                    <p>Book your favorite movies with ease!</p>
                </div>
                <div class="col-md-4">
                    <h5>Quick Links</h5>
                    <ul class="list-unstyled">
                        <li><a href="/" class="text-white">Home</a></li>
                        <li><a href="/movies" class="text-white">Movies</a></li>
                        <li><a href="/theatres" class="text-white">Theatres</a></li>
                        <li><a href="/about" class="text-white">About Us</a></li>
                        <li><a href="/contact" class="text-white">Contact</a></li>
                    </ul>
                </div>
                <div class="col-md-4">
                    <h5>Contact Us</h5>
                    <address>
                        <i class="fas fa-map-marker-alt me-2"></i> 123 Movie Street<br>
                        <i class="fas fa-phone me-2"></i> (123) 456-7890<br>
                        <i class="fas fa-envelope me-2"></i> info@theatremanagement.com
                    </address>
                    <div class="mt-3">
                        <a href="#" class="text-white me-2"><i class="fab fa-facebook fa-lg"></i></a>
                        <a href="#" class="text-white me-2"><i class="fab fa-twitter fa-lg"></i></a>
                        <a href="#" class="text-white me-2"><i class="fab fa-instagram fa-lg"></i></a>
                    </div>
                </div>
            </div>
            <hr class="my-4">
            <div class="text-center">
                <p>&copy; ${currentYear} Theatre Management System. All rights reserved.</p>
            </div>
        </div>
    </footer>`;

    // Set the footer HTML
    footerElement.innerHTML = footerHtml;
});