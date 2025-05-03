// booking/checkout.js
document.addEventListener('DOMContentLoaded', function () {
    // Get query parameters
    const urlParams = new URLSearchParams(window.location.search);
    const screeningId = urlParams.get('screeningId');
    const selectedSeats = urlParams.get('selectedSeats')?.split(',') || [];

    if (!screeningId || selectedSeats.length === 0) {
        window.location.href = '/movies';
        return;
    }

    // Check authentication
    if (!localStorage.getItem('authToken')) {
        window.location.href = `/auth/login?returnUrl=/bookings/checkout?screeningId=${screeningId}&selectedSeats=${selectedSeats.join(',')}`;
        return;
    }

    // DOM elements
    const loadingIndicator = document.createElement('div');
    loadingIndicator.className = 'text-center py-5';
    loadingIndicator.innerHTML = `
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
        <p class="mt-2">Loading checkout information...</p>
    `;
    document.querySelector('main').appendChild(loadingIndicator);

    // Load screening, movie, and theatre information
    let screening, movie, theatre, totalPrice;

    // First load the screening details
    apiService.screenings.getById(screeningId)
        .then(data => {
            screening = data;

            // Load movie details
            return apiService.movies.getById(screening.movieId);
        })
        .then(data => {
            movie = data;

            // Load theatre details
            return apiService.get(`/theatres/${screening.theatreId}`);
        })
        .then(data => {
            theatre = data;

            // Calculate total price for selected seats
            return apiService.bookings.calculatePrice(screeningId, selectedSeats);
        })
        .then(data => {
            totalPrice = data.totalPrice;

            // Remove loading indicator
            loadingIndicator.remove();

            // Render the page content
            renderPageContent();
        })
        .catch(error => {
            console.error('Error loading checkout information:', error);
            loadingIndicator.innerHTML = `
                <div class="alert alert-danger">
                    <p>Failed to load checkout information. Please try again later.</p>
                    <a href="/movies" class="btn btn-primary mt-3">Return to Movies</a>
                </div>
            `;
        });

    // Function to render the page content
    function renderPageContent() {
        // Create the main structure
        const container = document.createElement('div');
        container.className = 'container';

        // Add breadcrumb and page title
        container.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h2><i class="fas fa-shopping-cart me-2"></i>Booking Checkout</h2>
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb mb-0">
                        <li class="breadcrumb-item"><a href="/">Home</a></li>
                        <li class="breadcrumb-item"><a href="/movies/${movie.id}">Movie Details</a></li>
                        <li class="breadcrumb-item"><a href="/bookings/screening/${screening.id}">Select Seats</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Checkout</li>
                    </ol>
                </nav>
            </div>
        `;

        // Create the checkout form and order summary
        const checkoutSection = document.createElement('div');
        checkoutSection.className = 'row';
        checkoutSection.innerHTML = `
            <!-- Order Summary -->
            <div class="col-md-8">
                <div class="card shadow-sm mb-4">
                    <div class="card-header bg-white">
                        <h5 class="mb-0">Order Summary</h5>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-4">
                                <img src="${movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster'}"
                                     class="img-fluid rounded" alt="Movie Poster">
                            </div>
                            <div class="col-md-8">
                                <h4>${movie.title}</h4>
                                <p>
                                    <span class="badge bg-primary me-2">${movie.genre}</span>
                                    <span class="badge bg-secondary me-2">${movie.rating}</span>
                                    <span class="badge bg-info text-dark">${movie.durationMinutes} min</span>
                                </p>
                                <p><strong>Theatre:</strong> ${theatre.name}</p>
                                <p><strong>Screen:</strong> Screen ${screening.screenNumber}</p>
                                <p><strong>Date & Time:</strong> ${new Date(screening.startTime).toLocaleString()}</p>
                                <p><strong>Format:</strong> ${screening.format}</p>
                            </div>
                        </div>

                        <div class="card">
                            <div class="card-header">
                                <h6 class="mb-0">Selected Seats</h6>
                            </div>
                            <div class="card-body">
                                <div class="row mb-2 fw-bold">
                                    <div class="col-4">Seat</div>
                                    <div class="col-4">Type</div>
                                    <div class="col-4 text-end">Price</div>
                                </div>
                                <hr>
                                <div id="seatsContainer"></div>
                                <hr>
                                <div class="row fw-bold">
                                    <div class="col-8">Total</div>
                                    <div class="col-4 text-end" id="totalPrice">$${totalPrice.toFixed(2)}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Payment Information -->
            <div class="col-md-4">
                <div class="card shadow-sm">
                    <div class="card-header bg-white">
                        <h5 class="mb-0">Payment Information</h5>
                    </div>
                    <div class="card-body">
                        <form id="paymentForm">
                            <input type="hidden" name="screeningId" value="${screening.id}">
                            <input type="hidden" name="selectedSeats" value="${selectedSeats.join(',')}">

                            <div class="mb-3">
                                <label for="paymentMethod" class="form-label">Payment Method</label>
                                <select class="form-select" id="paymentMethod" name="paymentMethod" required>
                                    <option value="">Select payment method</option>
                                    <option value="Credit Card">Credit Card</option>
                                    <option value="Debit Card">Debit Card</option>
                                    <option value="PayPal">PayPal</option>
                                    <option value="Apple Pay">Apple Pay</option>
                                    <option value="Google Pay">Google Pay</option>
                                </select>
                            </div>

                            <!-- Credit Card Information -->
                            <div id="creditCardForm" class="d-none">
                                <div class="mb-3">
                                    <label for="cardNumber" class="form-label">Card Number</label>
                                    <input type="text" class="form-control" id="cardNumber" placeholder="**** **** **** ****">
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <label for="expiryDate" class="form-label">Expiry Date</label>
                                        <input type="text" class="form-control" id="expiryDate" placeholder="MM/YY">
                                    </div>
                                    <div class="col-md-6 mb-3">
                                        <label for="cvv" class="form-label">CVV</label>
                                        <input type="text" class="form-control" id="cvv" placeholder="***">
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <label for="cardHolderName" class="form-label">Cardholder Name</label>
                                    <input type="text" class="form-control" id="cardHolderName">
                                </div>
                            </div>

                            <div class="mb-3 form-check">
                                <input type="checkbox" class="form-check-input" id="termsCheck" required>
                                <label class="form-check-label" for="termsCheck">I agree to the terms and conditions</label>
                            </div>

                            <div class="alert alert-info" role="alert">
                                <i class="fas fa-info-circle me-2"></i> This is a demonstration system. No actual payment will be processed.
                            </div>

                            <div class="d-grid gap-2">
                                <button type="submit" class="btn btn-primary">Complete Booking</button>
                                <a href="/bookings/screening/${screening.id}" class="btn btn-outline-secondary">Back to Seat Selection</a>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        `;
        container.appendChild(checkoutSection);

        // Replace the loading indicator with the content
        document.querySelector('main').appendChild(container);

        // Populate the seats container
        const seatsContainer = document.getElementById('seatsContainer');

        // For demonstration purposes, we'll use dummy data for seat types
        const seatTypes = {
            'A': 'STANDARD',
            'B': 'STANDARD',
            'C': 'PREMIUM',
            'D': 'PREMIUM',
            'E': 'VIP',
            'F': 'VIP'
        };

        // Calculate price per seat (for demo purposes)
        const pricePerSeat = totalPrice / selectedSeats.length;

        selectedSeats.forEach(seat => {
            const rowName = seat[0];
            const seatType = seatTypes[rowName] || 'STANDARD';

            const seatRow = document.createElement('div');
            seatRow.className = 'row mb-2';
            seatRow.innerHTML = `
                <div class="col-4">
                    <span class="badge bg-primary">${seat}</span>
                </div>
                <div class="col-4">${seatType}</div>
                <div class="col-4 text-end">$${(pricePerSeat).toFixed(2)}</div>
            `;

            seatsContainer.appendChild(seatRow);
        });

        // Handle the payment method change
        const paymentMethodSelect = document.getElementById('paymentMethod');
        const creditCardForm = document.getElementById('creditCardForm');

        paymentMethodSelect.addEventListener('change', function () {
            if (this.value === 'Credit Card' || this.value === 'Debit Card') {
                creditCardForm.classList.remove('d-none');
            } else {
                creditCardForm.classList.add('d-none');
            }
        });

        // Handle the payment form submission
        document.getElementById('paymentForm').addEventListener('submit', function (e) {
            e.preventDefault();

            const paymentMethod = document.getElementById('paymentMethod').value;

            if (!paymentMethod) {
                alert('Please select a payment method');
                return;
            }

            // Show loading state
            const submitButton = this.querySelector('button[type="submit"]');
            const originalText = submitButton.textContent;
            submitButton.disabled = true;
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';

            // Create the booking
            apiService.bookings.create(screeningId, selectedSeats, paymentMethod)
                .then(booking => {
                    // Redirect to booking confirmation page
                    window.location.href = `/bookings/${booking.id}?success=true`;
                })
                .catch(error => {
                    console.error('Error creating booking:', error);
                    alert('Failed to create booking. Please try again later.');

                    // Reset button
                    submitButton.disabled = false;
                    submitButton.textContent = originalText;
                });
        });
    }
});