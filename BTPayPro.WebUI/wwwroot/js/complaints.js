// Complaint Management Script (Client/Vendor View)
(function () {
    'use strict';

    // Protect the page
    protectPage();

    // Initialize on page load
    document.addEventListener('DOMContentLoaded', async function () {
        // Update user name display
        const userNameElement = document.getElementById('userNameDisplay');
        if (userNameElement) {
            userNameElement.textContent = getUserName();
        }

        // Update profile photo
        await fetchProfilePhoto();
        updateTopbarProfilePhoto();

        // Initialize wallet balance in topbar
        const userType = getUserType();
        if (userType === 'Client' || userType === 'Merchant') {
            await initializeWalletBalance();
        }

        // Setup dashboard links based on user type
        setupDashboardLinks();

        // Check user type access
        if (userType === 'Admin') {
            // Redirect admins to their complaints page
            window.location.href = '/admin-complaints.html';
            return;
        }

        // Load complaints
        await loadComplaints();

        // Setup form submission
        const complaintForm = document.getElementById('complaintForm');
        if (complaintForm) {
            complaintForm.addEventListener('submit', handleComplaintSubmit);
        }

        // Character counter
        const description = document.getElementById('description');
        const charCount = document.getElementById('charCount');
        if (description && charCount) {
            description.addEventListener('input', function () {
                charCount.textContent = this.value.length;
            });
        }
    });

    // Get dashboard URL based on user type
    function getDashboardUrl() {
        const userType = getUserType();
        switch (userType) {
            case 'Admin':
                return '/dashboards/dashboard-admin.html';
            case 'Client':
                return '/dashboards/dashboard-client.html';
            case 'Merchant':
                return '/dashboards/dashboard-vendor.html';
            default:
                return '/index.html';
        }
    }

    // Setup dashboard navigation links
    function setupDashboardLinks() {
        const dashboardUrl = getDashboardUrl();
        const dashboardLink = document.getElementById('dashboardLink');
        const dashboardNavLink = document.getElementById('dashboardNavLink');

        if (dashboardLink) {
            dashboardLink.href = dashboardUrl;
        }
        if (dashboardNavLink) {
            dashboardNavLink.href = dashboardUrl;
        }
    }

    // Handle complaint form submission
    async function handleComplaintSubmit(event) {
        event.preventDefault();

        const userId = localStorage.getItem('userId');
        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        const submitBtn = document.getElementById('submitBtn');
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Submitting...';

        const complaintData = {
            description: document.getElementById('description').value,
            channel: document.getElementById('channel').value
        };

        try {
            const token = getAuthToken();
            const response = await fetch(`${AppConfig.API_BASE_URL}/Complaint`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'UserId': userId
                },
                body: JSON.stringify(complaintData)
            });

            if (response.ok) {
                showAlert('Complaint submitted successfully! Our support team will review it soon.', 'success');
                // Add notification
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.COMPLAINT_SENT,
                        'Complaint Submitted',
                        `Your complaint via ${complaintData.channel} has been submitted successfully.`
                    );
                }

                // Reset form
                document.getElementById('complaintForm').reset();
                document.getElementById('charCount').textContent = '0';

                // Reload complaints
                await loadComplaints();
            } else {
                const error = await response.json();
                showAlert(error.message || 'Failed to submit complaint', 'danger');
                // Add notification for failed complaint
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                        'Complaint Submission Failed',
                        `Failed to submit complaint: ${error.message || 'Unknown error'}`
                    );
                }
            }
        } catch (error) {
            console.error('Error submitting complaint:', error);
            showAlert('Error submitting complaint', 'danger');
        } finally {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="fas fa-paper-plane"></i> Submit Complaint';
        }
    }

    // Load user's complaints
    async function loadComplaints() {
        const userId = localStorage.getItem('userId');
        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        const container = document.getElementById('complaintsListContainer');
        container.innerHTML = '<p class="text-center text-muted">Loading complaints...</p>';

        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Complaint/user/${userId}`);

            if (response && response.ok) {
                const complaints = await response.json();
                displayComplaints(complaints);
            } else {
                container.innerHTML = '<p class="text-center text-danger">Failed to load complaints</p>';
            }
        } catch (error) {
            console.error('Error loading complaints:', error);
            container.innerHTML = '<p class="text-center text-danger">Error loading complaints</p>';
        }
    }

    // Display complaints list
    function displayComplaints(complaints) {
        const container = document.getElementById('complaintsListContainer');

        if (!complaints || complaints.length === 0) {
            container.innerHTML = `
                <div class="text-center text-muted py-4">
                    <i class="fas fa-inbox fa-3x mb-3"></i>
                    <p>No complaints submitted yet</p>
                </div>
            `;
            return;
        }

        container.innerHTML = complaints.map(c => {
            const statusBadge = getStatusBadge(c.complaintStatus);
            const statusIcon = getStatusIcon(c.complaintStatus);
            const hasResponse = c.adminResponse && c.adminResponse.trim() !== '';

            return `
                <div class="card mb-3 ${c.complaintStatus === 'Done' ? 'border-success' : 'border-warning'}">
                    <div class="card-header d-flex justify-content-between align-items-center ${c.complaintStatus === 'Done' ? 'bg-success text-white' : 'bg-warning text-dark'}">
                        <span>
                            <i class="fas fa-calendar-alt mr-2"></i>
                            Submitted: ${formatDate(c.dateComplaint)}
                        </span>
                        <span>
                            ${statusIcon} ${statusBadge}
                        </span>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-12">
                                <p class="mb-2"><strong>Channel:</strong> <span class="badge badge-info">${escapeHtml(c.channel)}</span></p>
                                <p class="mb-2"><strong>Your Complaint:</strong></p>
                                <p class="bg-light p-3 rounded">${escapeHtml(c.description)}</p>
                                
                                ${hasResponse ? `
                                    <hr>
                                    <div class="alert alert-success" role="alert">
                                        <h6 class="alert-heading"><i class="fas fa-reply"></i> Admin Response</h6>
                                        <p class="mb-0">${escapeHtml(c.adminResponse)}</p>
                                        <hr>
                                        <p class="mb-0 small">
                                            <i class="fas fa-clock"></i> Responded on: ${formatDateTime(c.responseDate)}
                                        </p>
                                    </div>
                                ` : `
                                    <div class="alert alert-warning" role="alert">
                                        <i class="fas fa-hourglass-half"></i> Waiting for admin response...
                                    </div>
                                `}
                            </div>
                        </div>
                    </div>
                </div>
            `;
        }).join('');
    }

    // Get status badge
    function getStatusBadge(status) {
        if (status === 'Done') {
            return '<span class="badge badge-success">Done</span>';
        }
        return '<span class="badge badge-warning">Pending</span>';
    }

    // Get status icon
    function getStatusIcon(status) {
        if (status === 'Done') {
            return '✅';
        }
        return '⏳';
    }

    // Format date
    function formatDate(dateString) {
        if (!dateString) return '-';
        const parts = dateString.split('-');
        if (parts.length === 3) {
            return `${parts[2]}/${parts[1]}/${parts[0]}`;
        }
        return dateString;
    }

    // Format datetime
    function formatDateTime(dateTimeString) {
        if (!dateTimeString) return '-';
        const date = new Date(dateTimeString);
        return date.toLocaleString('en-GB', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    // Escape HTML
    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Show alert message
    function showAlert(message, type) {
        const alertContainer = document.getElementById('alertContainer');
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show`;
        alert.role = 'alert';
        alert.innerHTML = `
            ${message}
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
        `;

        alertContainer.innerHTML = '';
        alertContainer.appendChild(alert);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 150);
        }, 5000);

        // Scroll to top to show alert
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    // Make functions globally available
    window.loadComplaints = loadComplaints;
})();
