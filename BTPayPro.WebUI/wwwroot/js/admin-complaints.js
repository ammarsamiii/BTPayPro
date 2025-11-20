// Global variable to store current complaint ID
let currentComplaintId = null;

document.addEventListener('DOMContentLoaded', async function () {
    // Protect this page - redirect to login if not authenticated
    protectPage();

    // Update user name display
    const userNameElement = document.getElementById('userNameDisplay');
    if (userNameElement) {
        userNameElement.textContent = getUserName();
    }

    // Update profile photo
    await fetchProfilePhoto();
    updateTopbarProfilePhoto();

    // Load complaints data
    await loadAllComplaints();
});

// Load all complaints (admin view)
async function loadAllComplaints() {
    const container = document.getElementById('complaintsListContainer');
    container.innerHTML = '<p class="text-center text-muted">Loading complaints...</p>';

    try {
        console.log('Fetching complaints from:', `${AppConfig.API_BASE_URL}/Complaint/all`);
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Complaint/all`);
        console.log('Complaints API response:', response);

        if (response && response.ok) {
            const complaints = await response.json();
            console.log('Complaints data:', complaints);
            displayComplaints(complaints);
            updateStatistics(complaints);
        } else {
            console.error('Failed to load complaints, status:', response ? response.status : 'unknown');
            container.innerHTML = '<p class="text-center text-danger">Failed to load complaints. Status: ' + (response ? response.status : 'Unknown') + '</p>';
        }
    } catch (error) {
        console.error('Error loading complaints:', error);
        container.innerHTML = '<p class="text-center text-danger">Error loading complaints: ' + error.message + '</p>';
    }
}

// Update statistics
function updateStatistics(complaints) {
    if (!complaints || !Array.isArray(complaints)) {
        document.getElementById('pendingCount').textContent = '0';
        document.getElementById('resolvedCount').textContent = '0';
        return;
    }

    const pending = complaints.filter(c => c.complaintStatus === 'Pending').length;
    const resolved = complaints.filter(c => c.complaintStatus === 'Done').length;

    document.getElementById('pendingCount').textContent = pending;
    document.getElementById('resolvedCount').textContent = resolved;
}

// Display complaints list for admin
function displayComplaints(complaints) {
    const container = document.getElementById('complaintsListContainer');

    if (!complaints || !Array.isArray(complaints) || complaints.length === 0) {
        container.innerHTML = `
            <div class="text-center text-muted py-4">
                <i class="fas fa-inbox fa-3x mb-3"></i>
                <p>No complaints found</p>
            </div>
        `;
        return;
    }

    // Separate pending and resolved
    const pending = complaints.filter(c => c.complaintStatus === 'Pending');
    const resolved = complaints.filter(c => c.complaintStatus === 'Done');

    let html = '';

    // Pending complaints first
    if (pending.length > 0) {
        html += '<h5 class="mb-3">⏳ Pending Complaints (' + pending.length + ')</h5>';
        html += pending.map(c => renderComplaintCard(c)).join('');
        html += '<hr class="my-4">';
    }

    // Resolved complaints
    if (resolved.length > 0) {
        html += '<h5 class="mb-3">✅ Resolved Complaints (' + resolved.length + ')</h5>';
        html += resolved.map(c => renderComplaintCard(c)).join('');
    }

    container.innerHTML = html;
}

// Render single complaint card
function renderComplaintCard(c) {
    // Handle potential null/undefined values
    const complaintId = c.complaintId || c.ComplaintId || 'unknown';
    const userName = c.userName || c.UserName || c.userEmail || c.UserEmail || 'Unknown User';
    const userEmail = c.userEmail || c.UserEmail || 'N/A';
    const dateComplaint = c.dateComplaint || c.DateComplaint || new Date().toISOString();
    const channel = c.channel || c.Channel || 'N/A';
    const description = c.description || c.Description || 'No description provided';
    const complaintStatus = c.complaintStatus || c.ComplaintStatus || 'Pending';
    const adminResponse = c.adminResponse || c.AdminResponse || '';
    const responseDate = c.responseDate || c.ResponseDate || null;

    const isPending = complaintStatus === 'Pending';
    const statusBadge = isPending ?
        '<span class="badge badge-warning">⏳ Pending</span>' :
        '<span class="badge badge-success">✅ Done</span>';

    const hasResponse = adminResponse && adminResponse.trim() !== '';

    return `
        <div class="card mb-3 ${isPending ? 'border-warning' : 'border-success'}">
            <div class="card-header d-flex justify-content-between align-items-center ${isPending ? 'bg-warning text-dark' : 'bg-success text-white'}">
                <span>
                    <i class="fas fa-user mr-2"></i>
                    ${escapeHtml(userName)}
                    <small class="ml-2">(${escapeHtml(userEmail)})</small>
                </span>
                <span>${statusBadge}</span>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-9">
                        <p class="mb-2">
                            <strong><i class="fas fa-calendar-alt mr-1"></i> Date:</strong> ${formatDate(dateComplaint)}
                            <span class="ml-3"><strong><i class="fas fa-phone mr-1"></i> Channel:</strong> 
                            <span class="badge badge-info">${escapeHtml(channel)}</span></span>
                        </p>
                        <p class="mb-2"><strong><i class="fas fa-comment mr-1"></i> Complaint:</strong></p>
                        <p class="bg-light p-3 rounded">${escapeHtml(description)}</p>
                        
                        ${hasResponse ? `
                            <div class="mt-3">
                                <p class="mb-2"><strong><i class="fas fa-reply mr-1"></i> Your Response:</strong></p>
                                <div class="alert alert-info mb-0">
                                    <p class="mb-0">${escapeHtml(adminResponse)}</p>
                                    <hr>
                                    <p class="mb-0 small">
                                        <i class="fas fa-clock"></i> Responded on: ${formatDateTime(responseDate)}
                                    </p>
                                </div>
                            </div>
                        ` : ''}
                    </div>
                    <div class="col-md-3 text-right">
                        ${isPending ? `
                            <button class="btn btn-primary btn-block" onclick="openResponseModal('${complaintId}')">
                                <i class="fas fa-reply"></i> Respond
                            </button>
                        ` : `
                            <button class="btn btn-secondary btn-block" disabled>
                                <i class="fas fa-check"></i> Resolved
                            </button>
                        `}
                    </div>
                </div>
            </div>
        </div>
    `;
}

// Open response modal
async function openResponseModal(complaintId) {
    currentComplaintId = complaintId;

    try {
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Complaint/${complaintId}`);

        if (response && response.ok) {
            const complaint = await response.json();

            // Populate modal
            document.getElementById('modalCustomerName').textContent = complaint.userName || complaint.UserName || 'Unknown User';
            document.getElementById('modalCustomerEmail').textContent = complaint.userEmail || complaint.UserEmail || 'N/A';
            document.getElementById('modalDate').textContent = formatDate(complaint.dateComplaint || complaint.DateComplaint);
            document.getElementById('modalChannel').textContent = complaint.channel || complaint.Channel || 'N/A';
            document.getElementById('modalComplaint').textContent = complaint.description || complaint.Description || 'No description provided';
            document.getElementById('adminResponse').value = '';

            // Show modal
            $('#responseModal').modal('show');
        } else {
            showAlert('Failed to load complaint details', 'danger');
        }
    } catch (error) {
        console.error('Error loading complaint:', error);
        showAlert('Error loading complaint details', 'danger');
    }
}

// Submit response
async function submitResponse() {
    if (!currentComplaintId) {
        showAlert('Error: Complaint ID not found', 'danger');
        return;
    }

    const responseText = document.getElementById('adminResponse').value.trim();
    if (!responseText) {
        showAlert('Please enter a response', 'warning');
        return;
    }

    const responseData = {
        adminResponse: responseText
    };

    try {
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Complaint/${currentComplaintId}/respond`, {
            method: 'PUT',
            body: JSON.stringify(responseData)
        });

        if (response && response.ok) {
            showAlert('Response sent successfully!', 'success');
            // Add notification
            if (typeof BTPayProNotifications !== 'undefined') {
                BTPayProNotifications.addNotification(
                    BTPayProNotifications.NOTIFICATION_TYPES.COMPLAINT_TREATED,
                    'Complaint Treated',
                    'Your complaint response has been sent successfully.'
                );
            }
            $('#responseModal').modal('hide');
            await loadAllComplaints();
        } else {
            const error = await response.json();
            showAlert(error.message || 'Failed to send response', 'danger');
            // Add notification for failed response
            if (typeof BTPayProNotifications !== 'undefined') {
                BTPayProNotifications.addNotification(
                    BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                    'Complaint Response Failed',
                    `Failed to send complaint response: ${error.message || 'Unknown error'}`
                );
            }
        }
    } catch (error) {
        console.error('Error sending response:', error);
        showAlert('Error sending response: ' + error.message, 'danger');
    }
}

// Format date helper
function formatDate(dateString) {
    if (!dateString) return 'N/A';
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
    } catch (e) {
        return 'N/A';
    }
}

// Format date and time helper
function formatDateTime(dateString) {
    if (!dateString) return 'N/A';
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
    } catch (e) {
        return 'N/A';
    }
}

// Escape HTML helper
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