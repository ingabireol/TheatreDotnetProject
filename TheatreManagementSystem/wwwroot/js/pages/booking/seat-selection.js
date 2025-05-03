namespace TheatreManagementSystem.wwwroot.js.pages.booking
// booking/seat-selection.js
document.addEventListener('DOMContentLoaded', function () {
    // Get screening ID from URL
    const urlParams = new URLSearchParams(window.location.search);
    const screeningId = window.location.pathname.split('/').pop();

    if (!screeningId) {
        window.location.href = '/movies';
        return;
    }

    // DOM elements
    const loadingIndicator = document.createElement('div');
    loadingIndicator.className = 'text-center py-5';
    loadingIndicator.innerHTML = `
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
        <p class="mt-2">Loading screening information...</p>
    `;
    document.querySelector('main').appendChild(loadingIndicator);

    // Check authentication
    if (!localStorage.getItem('authToken')) {
        window.location.href = `/auth/login?returnUrl=/bookings/screening/${screeningId}`;
        return;
    }

    // Load screening, movie, and theatre information
    let screening, movie, theatre, seats, bookedSeats;

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

            // Load seats for this theatre and screen
            return apiService.get(`/seats/by-theatre-screen?theatreId=${theatre.id}&screenNumber=${screening.screenNumber}`);
        })
        .then(data => {
            seats = data;

            // Load booked seats for this screening
            return apiService.screenings.getBookedSeats(screeningId);
        })
        .then(data => {
            bookedSeats = data;

            // Remove loading indicator
            loadingIndicator.remove();

            // Render the page content
            renderPageContent();
        })
        .catch(error => {
            console.error('Error loading screening information:', error);
            loadingIndicator.innerHTML = `
                <div class="alert alert-danger">
                    <p>Failed to load screening information. Please try again later.</p>
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
                <h2><i class="fas fa-couch me-2"></i>Select Your Seats</h2>
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb mb-0">
                        <li class="breadcrumb-item"><a href="/">Home</a></li>
                        <li class="breadcrumb-item"><a href="/movies/${movie.id}">Movie Details</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Select Seats</li>
                    </ol>
                </nav>
            </div>
        `;

        // Add movie and screening info
        const infoSection = document.createElement('div');
        infoSection.className = 'row mb-4';
        infoSection.innerHTML = `
            <div class="col-md-3">
                <img src="${movie.posterImageUrl || 'https://placehold.co/300x450?text=No+Poster'}"
                     class="img-fluid rounded" alt="Movie Poster">
            </div>
            <div class="col-md-9">
                <div class="card h-100">
                    <div class="card-body">
                        <h3>${movie.title}</h3>
                        <div class="mb-3">
                            <span class="badge bg-primary me-2">${movie.genre}</span>
                            <span class="badge bg-secondary me-2">${movie.rating}</span>
                            <span class="badge bg-info text-dark">${movie.durationMinutes} min</span>
                        </div>
                        <p><strong>Theatre:</strong> ${theatre.name}</p>
                        <p><strong>Screen:</strong> Screen ${screening.screenNumber}</p>
                        <p><strong>Date & Time:</strong> ${new Date(screening.startTime).toLocaleString()}</p>
                        <p><strong>Format:</strong> ${screening.format}</p>
                        <p><strong>Base Price:</strong> $${screening.basePrice.toFixed(2)}</p>
                    </div>
                </div>
            </div>
        `;
        container.appendChild(infoSection);

        // Add seat selection section
        const seatSelectionCard = document.createElement('div');
        seatSelectionCard.className = 'card shadow-sm mb-4';
        seatSelectionCard.innerHTML = `
            <div class="card-header bg-white">
                <h5 class="mb-0">Choose Your Seats</h5>
            </div>
            <div class="card-body">
                <div class="seat-container">
                    <div class="screen">SCREEN</div>
                    <div id="seats-container"></div>
                    
                    <!-- Legend -->
                    <div class="seat-legend">
                        <div class="legend-item">
                            <div class="legend-seat bg-secondary"></div>
                            <span>Standard</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-seat bg-warning"></div>
                            <span>Premium</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-seat" style="background-color: #6f42c1;"></div>
                            <span>VIP</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-seat bg-success"></div>
                            <span>Accessible</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-seat bg-primary"></div>
                            <span>Selected</span>
                        </div>
                        <div class="legend-item">
                            <div class="legend-seat bg-danger"></div>
                            <span>Booked</span>
                        </div>
                    </div>
                </div>

                <!-- Selected Seats Summary -->
                <div class="card mt-4">
                    <div class="card-body">
                        <h5>Selected Seats</h5>
                        <div id="selectedSeatsContainer">
                            <p id="noSeatsSelected">No seats selected</p>
                            <div id="selectedSeatsList" class="d-none"></div>
                        </div>
                        <hr>
                        <div class="d-flex justify-content-between">
                            <h5>Total Price:</h5>
                            <h5 id="totalPrice">$0.00</h5>
                        </div>
                        <form id="bookingForm">
                            <input type="hidden" id="selectedSeatsInput" name="selectedSeats">
                            <button type="submit" class="btn btn-primary mt-3 w-100" id="continueBtn" disabled>
                                Continue to Checkout
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        `;
        container.appendChild(seatSelectionCard);

        // Replace the loading indicator with the content
        document.querySelector('main').appendChild(container);

        // Organize seats by row
        const seatsByRow = {};

        seats.forEach(seat => {
            const row = seat.rowName;
            if (!seatsByRow[row]) {
                seatsByRow[row] = [];
            }
            seatsByRow[row].push(seat);
        });

        // Sort rows alphabetically
        const sortedRows = Object.keys(seatsByRow).sort();

        // Create seat rows and add them to the seat container
        const seatsContainer = document.getElementById('seats-container');

        sortedRows.forEach(rowName => {
            // Create a row div
            const rowDiv = document.createElement('div');
            rowDiv.className = 'seat-row';

            // Add row label
            const rowLabel = document.createElement('div');
            rowLabel.className = 'row-name';
            rowLabel.textContent = rowName;
            rowDiv.appendChild(rowLabel);

            // Sort seats by number and add them to the row
            seatsByRow[rowName]
                .sort((a, b) => a.seatNumber - b.seatNumber)
                .forEach(seat => {
                    const seatId = seat.rowName + seat.seatNumber;
                    const isBooked = bookedSeats.includes(seatId);

                    const seatElement = document.createElement('div');
                    seatElement.className = `seat-item ${seat.seatType.toLowerCase()} ${isBooked ? 'booked' : ''}`;
                    seatElement.textContent = seatId;
                    seatElement.dataset.seat = seatId;
                    seatElement.dataset.row = seat.rowName;
                    seatElement.dataset.number = seat.seatNumber;
                    seatElement.dataset.type = seat.seatType;
                    seatElement.dataset.price = (screening.basePrice * seat.priceMultiplier).toFixed(2);

                    if (!isBooked) {
                        seatElement.addEventListener('click', toggleSeatSelection);
                    }

                    rowDiv.appendChild(seatElement);
                });

            seatsContainer.appendChild(rowDiv);
        });

        // Setup the booking form submission
        document.getElementById('bookingForm').addEventListener('submit', function (e) {
            e.preventDefault();

            const selectedSeats = document.getElementById('selectedSeatsInput').value;
            if (!selectedSeats) {
                alert('Please select at least one seat');
                return;
            }

            window.location.href = `/bookings/checkout?screeningId=${screeningId}&selectedSeats=${selectedSeats}`;
        });
    }

    // Track selected seats
    const selectedSeats = [];

    // Function to toggle seat selection
    function toggleSeatSelection() {
        // Skip if the seat is already booked
        if (this.classList.contains('booked')) {
            return;
        }

        const seatId = this.dataset.seat;
        const seatPrice = parseFloat(this.dataset.price);
        const seatType = this.dataset.type;

        // Toggle selection
        if (this.classList.contains('selected')) {
            // Remove from selected
            this.classList.remove('selected');
            const index = selectedSeats.findIndex(seat => seat.id === seatId);
            if (index !== -1) {
                selectedSeats.splice(index, 1);
            }
        } else {
            // Add to selected
            this.classList.add('selected');
            selectedSeats.push({
                id: seatId,
                price: seatPrice,
                type: seatType
            });
        }

        // Update the summary
        updateSelectedSeatsSummary();
    }

    // Function to update the selected seats summary
    function updateSelectedSeatsSummary() {
        const noSeatsSelected = document.getElementById('noSeatsSelected');
        const selectedSeatsList = document.getElementById('selectedSeatsList');
        const totalPriceElement = document.getElementById('totalPrice');
        const continueBtn = document.getElementById('continueBtn');
        const selectedSeatsInput = document.getElementById('selectedSeatsInput');

        if (selectedSeats.length === 0) {
            noSeatsSelected.classList.remove('d-none');
            selectedSeatsList.classList.add('d-none');
            totalPriceElement.textContent = '$0.00';
            continueBtn.disabled = true;
            selectedSeatsInput.value = '';
        } else {
            noSeatsSelected.classList.add('d-none');
            selectedSeatsList.classList.remove('d-none');

            // Sort selected seats for cleaner display
            selectedSeats.sort((a, b) => a.id.localeCompare(b.id));

            // Display selected seats
            selectedSeatsList.innerHTML = '';
            let totalPrice = 0;

            selectedSeats.forEach(seat => {
                const seatElement = document.createElement('div');
                seatElement.className = 'mb-2 d-flex justify-content-between';
                seatElement.innerHTML = `
                    <div>
                        <span class="badge bg-${getSeatColor(seat.type)} me-2">${seat.id}</span>
                        <span class="text-muted">${seat.type.toLowerCase()}</span>
                    </div>
                    <div>$${seat.price.toFixed(2)}</div>
                `;
                selectedSeatsList.appendChild(seatElement);
                totalPrice += seat.price;
            });

            // Update the form input value and total price
            totalPriceElement.textContent = '$' + totalPrice.toFixed(2);
            selectedSeatsInput.value = selectedSeats.map(seat => seat.id).join(',');
            continueBtn.disabled = false;
        }
    }

    // Helper function to get seat color class
    function getSeatColor(seatType) {
        switch (seatType) {
            case 'STANDARD': return 'secondary';
            case 'PREMIUM': return 'warning text-dark';
            case 'VIP': return 'primary';
            case 'ACCESSIBLE': return 'success';
            default: return 'secondary';
        }
    }
});