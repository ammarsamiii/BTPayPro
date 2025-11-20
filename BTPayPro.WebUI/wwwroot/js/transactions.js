// Transaction Management Script
(function () {
    'use strict';

    let currentTransactionId = null;

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
        await initializeWalletBalance();

        // Setup dashboard links based on user type
        setupDashboardLinks();

        // Check user type access
        const userType = getUserType();
        if (userType !== 'Client' && userType !== 'Merchant') {
            showAlert('Access denied. Transactions are only available for Clients and Merchants.', 'danger');
            setTimeout(() => {
                window.location.href = getDashboardUrl();
            }, 2000);
            return;
        }

        // Show only the relevant accounting report section based on user type
        if (userType === 'Client') {
            document.getElementById('autmpaySection').style.display = 'block';
        } else if (userType === 'Merchant') {
            document.getElementById('cpmpaySection').style.display = 'block';
        }

        // Load transactions
        await loadTransactions();
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

    // Load transactions from API
    async function loadTransactions() {
        const userId = localStorage.getItem('userId');
        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/user/${userId}`);

            if (response && response.ok) {
                const transactions = await response.json();
                displayTransactions(transactions);
            } else {
                showAlert('Failed to load transactions', 'danger');
                document.getElementById('transactionsBody').innerHTML =
                    '<tr><td colspan="7" class="text-center">Failed to load transactions</td></tr>';
            }
        } catch (error) {
            console.error('Error loading transactions:', error);
            showAlert('Error loading transactions', 'danger');
            document.getElementById('transactionsBody').innerHTML =
                '<tr><td colspan="7" class="text-center">Error loading transactions</td></tr>';
        }
    }

    // Display transactions in table
    function displayTransactions(transactions) {
        const tbody = document.getElementById('transactionsBody');

        if (!transactions || transactions.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center">No transactions found</td></tr>';
            return;
        }

        tbody.innerHTML = transactions.map(t => {
            const isPendingTransfer = t.transactionType === 'TransferRequest' && t.status === 'Pending';
            let actions = `
                <button class="btn btn-sm btn-info" onclick="viewTransaction('${t.transactionId}')" title="View Details">
                    <i class="fas fa-eye"></i>
                </button>
            `;
            if (isPendingTransfer) {
                actions += `
                <button class="btn btn-sm btn-success ml-2" onclick="acceptTransfer('${t.transactionId}')" title="Accept Transfer">
                    <i class="fas fa-check"></i>
                </button>`;
            }

            return `
            <tr>
                <td>${formatDate(t.transactionDate)}</td>
                <td>${escapeHtml(t.transactionType)}</td>
                <td class="text-right font-weight-bold ${t.transactionAmount >= 0 ? 'text-success' : 'text-danger'}">
                    ${t.transactionAmount.toFixed(2)} TND
                </td>
                <td>${getStatusBadge(t.status)}</td>
                <td class="text-right">${t.comission.toFixed(2)} TND</td>
                <td>${escapeHtml(t.externalOrderId || '-')}</td>
                <td class="text-center">${actions}</td>
            </tr>`;
        }).join('');
    }

    // Format date for display
    function formatDate(dateString) {
        if (!dateString) return '-';
        const parts = dateString.split('-');
        if (parts.length === 3) {
            return `${parts[2]}/${parts[1]}/${parts[0]}`;
        }
        return dateString;
    }

    // Get status badge HTML
    function getStatusBadge(status) {
        const statusMap = {
            'Created': 'secondary',
            'Pending': 'warning',
            'Completed': 'success',
            'Failed': 'danger',
            'Cancelled': 'dark'
        };
        const badgeClass = statusMap[status] || 'secondary';
        return `<span class="badge badge-${badgeClass}">${status}</span>`;
    }

    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // View transaction details
    async function viewTransaction(transactionId) {
        currentTransactionId = transactionId;

        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/details/${transactionId}`);

            if (response && response.ok) {
                const transaction = await response.json();
                populateModal(transaction);
                $('#transactionModal').modal('show');
            } else {
                showAlert('Failed to load transaction details', 'danger');
            }
        } catch (error) {
            console.error('Error loading transaction details:', error);
            showAlert('Error loading transaction details', 'danger');
        }
    }

    // Populate modal with transaction data
    function populateModal(transaction) {
        document.getElementById('modalTransactionId').value = transaction.transactionId;
        document.getElementById('modalDate').value = formatDate(transaction.transactionDate);
        document.getElementById('modalType').value = transaction.transactionType;
        document.getElementById('modalAmount').value = transaction.transactionAmount;
        document.getElementById('modalStatus').value = transaction.status;
        document.getElementById('modalCommission').value = transaction.comission;
        document.getElementById('modalOrderId').value = transaction.externalOrderId || '';
    }

    // Save transaction updates
    async function saveTransaction() {
        if (!currentTransactionId) {
            showAlert('Error: Transaction ID not found', 'danger');
            return;
        }

        const updateData = {
            transactionType: document.getElementById('modalType').value,
            transactionAmount: parseFloat(document.getElementById('modalAmount').value),
            status: document.getElementById('modalStatus').value
        };

        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/update/${currentTransactionId}`, {
                method: 'PUT',
                body: JSON.stringify(updateData)
            });

            if (response && response.ok) {
                showAlert('Transaction updated successfully!', 'success');
                // Add notification
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_COMPLETED,
                        'Transaction Updated',
                        `Transaction has been updated to status: ${updateData.status}`
                    );
                }
                $('#transactionModal').modal('hide');
                await loadTransactions();
            } else {
                const error = await response.json();
                showAlert(error.message || 'Failed to update transaction', 'danger');
                // Add notification for failed update
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                        'Transaction Update Failed',
                        `Failed to update transaction: ${error.message || 'Unknown error'}`
                    );
                }
            }
        } catch (error) {
            console.error('Error updating transaction:', error);
            showAlert('Error updating transaction', 'danger');
        }
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

    // Open create transaction modal
    function openCreateTransactionModal() {
        // Reset form
        document.getElementById('createTransactionForm').reset();
        $('#createTransactionModal').modal('show');
    }

    // Create new transaction
    async function createTransaction() {
        const transactionType = document.getElementById('newTransactionType').value;
        const amount = parseFloat(document.getElementById('newTransactionAmount').value);

        if (!transactionType || !amount) {
            showAlert('Please fill in all required fields', 'warning');
            return;
        }

        const userId = localStorage.getItem('userId');
        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        try {
            // First, get user's wallet ID
            const walletResponse = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/user/${userId}`);

            if (!walletResponse || !walletResponse.ok) {
                showAlert('Failed to get wallet information', 'danger');
                return;
            }

            const walletData = await walletResponse.json();
            const walletId = walletData.walletId;

            // Determine if amount should be negative
            let finalAmount = amount;
            if (['Withdrawal', 'Payment', 'Transfer'].includes(transactionType)) {
                finalAmount = -Math.abs(amount);
            } else {
                finalAmount = Math.abs(amount);
            }

            // Create transaction
            const transactionData = {
                transactionType: transactionType,
                transactionAmount: finalAmount,
                walletId: walletId
            };

            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/create`, {
                method: 'POST',
                body: JSON.stringify(transactionData)
            });

            if (response && response.ok) {
                showAlert('Transaction created successfully!', 'success');
                // Add notification
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_SUBMITTED,
                        'Transaction Submitted',
                        `A new ${transactionType} transaction of ${Math.abs(finalAmount).toFixed(2)} TND has been submitted.`
                    );
                }
                $('#createTransactionModal').modal('hide');
                await loadTransactions();
            } else {
                const error = await response.json();
                showAlert(error.message || 'Failed to create transaction', 'danger');
                // Add notification for failed transaction
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                        'Transaction Failed',
                        `Failed to create ${transactionType} transaction: ${error.message || 'Unknown error'}`
                    );
                }
            }
        } catch (error) {
            console.error('Error creating transaction:', error);
            showAlert('Error creating transaction', 'danger');
        }
    }

    // Request transfer modal
    function openTransferRequestModal() {
        document.getElementById('requestTransferForm').reset();
        $('#requestTransferModal').modal('show');
    }

    async function requestTransfer() {
        const recipientEmail = document.getElementById('recipientEmail').value.trim();
        const amount = parseFloat(document.getElementById('transferAmount').value);
        if (!recipientEmail || !amount || amount <= 0) {
            showAlert('Please provide valid recipient email and amount', 'warning');
            return;
        }

        const userId = localStorage.getItem('userId');
        let senderWalletId = localStorage.getItem('walletId');
        if (!senderWalletId) {
            try {
                const w = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/user/${userId}`);
                if (w && w.ok) {
                    const data = await w.json();
                    senderWalletId = data.WalletId || data.walletId;
                    localStorage.setItem('walletId', senderWalletId);
                }
            } catch (e) { /* ignore */ }
        }
        if (!senderWalletId) {
            showAlert('Could not determine your wallet ID', 'danger');
            return;
        }

        try {
            // Get recipient wallet by email
            const searchUrl = `${AppConfig.API_BASE_URL}/Profile/search?q=${encodeURIComponent(recipientEmail)}`;
            const searchResponse = await authenticatedFetch(searchUrl);
            if (!searchResponse || !searchResponse.ok) {
                showAlert('Failed to find recipient', 'danger');
                return;
            }
            const recipients = await searchResponse.json();
            if (!recipients || recipients.length === 0) {
                showAlert('No user found with that email', 'warning');
                return;
            }
            const recipient = recipients[0];
            const recipientWalletId = recipient.WalletId || recipient.walletId;
            if (!recipientWalletId) {
                showAlert('Recipient does not have a wallet', 'warning');
                return;
            }

            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/transfer/request`, {
                method: 'POST',
                body: JSON.stringify({ senderWalletId, recipientWalletId, amount })
            });
            if (response && response.ok) {
                showAlert('Transfer request sent!', 'success');
                // Add notification
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_SUBMITTED,
                        'Transfer Request Sent',
                        `A transfer request of ${amount.toFixed(2)} TND has been sent to ${recipient.email || recipientEmail}.`
                    );
                }
                $('#requestTransferModal').modal('hide');
                await loadTransactions();
            } else {
                const err = await response.json();
                showAlert(err.message || 'Failed to send transfer request', 'danger');
                // Add notification for failed transfer
                if (typeof BTPayProNotifications !== 'undefined') {
                    BTPayProNotifications.addNotification(
                        BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                        'Transfer Request Failed',
                        `Failed to send transfer request: ${err.message || 'Unknown error'}`
                    );
                }
            }
        } catch (error) {
            console.error('Error requesting transfer:', error);
            showAlert('Error requesting transfer', 'danger');
        }
    }

    async function acceptTransfer(transactionId) {
        let recipientWalletId = localStorage.getItem('walletId');
        const userId = localStorage.getItem('userId');
        if (!recipientWalletId) {
            try {
                const w = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/user/${userId}`);
                if (w && w.ok) {
                    const data = await w.json();
                    recipientWalletId = data.walletId;
                    localStorage.setItem('walletId', recipientWalletId);
                }
            } catch (e) { /* ignore */ }
        }
        if (!recipientWalletId) {
            showAlert('Could not determine your wallet ID', 'danger');
            return;
        }
        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Transaction/transfer/${transactionId}/accept`, {
                method: 'PUT',
                body: JSON.stringify({ recipientWalletId })
            });
            if (response && response.ok) {
                showAlert('Transfer accepted successfully!', 'success');
                await loadTransactions();
                await fetchWalletBalance();
                updateWalletBalanceDisplay();
            } else {
                const err = await response.json();
                showAlert(err.message || 'Failed to accept transfer', 'danger');
            }
        } catch (error) {
            console.error('Error accepting transfer:', error);
            showAlert('Error accepting transfer', 'danger');
        }
    }
    // Generate Autmpay Report (for Clients)
    async function generateAutmpayReport() {
        const userType = getUserType();
        if (userType !== 'Client') {
            showAlert('Autmpay reports are only available for Clients', 'warning');
            return;
        }

        showReportStatus('Please upload your Autmpay file to generate report...', 'info');

        // Create file input for Autmpay
        const fileInput = document.createElement('input');
        fileInput.type = 'file';
        // Allow any file type to be uploaded to support files without extensions
        fileInput.accept = '';

        fileInput.onchange = async (event) => {
            const file = event.target.files[0];
            if (!file) {
                showReportStatus('No file selected', 'warning');
                return;
            }

            showReportStatus('Uploading and processing Autmpay file...', 'info');

            // Check if file is CSV or can be processed as CSV
            console.log('Selected Autmpay file:', file.name, file.type);

            try {
                // Create form data with proper content type
                const formData = new FormData();
                formData.append('file', file);

                showReportStatus('Processing file... Please wait.', 'info');

                // Log file details for debugging
                console.log('Selected Autmpay file:', file.name, file.type, file.size);

                // Use a direct fetch for file uploads to avoid Content-Type conflicts
                const token = getAuthToken();
                const response = await fetch(`${AppConfig.API_BASE_URL}/Autmpay/upload`, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                // Handle authentication errors
                if (response.status === 401) {
                    localStorage.removeItem('authToken');
                    window.location.href = '/login.html';
                    return;
                }

                // Log response for debugging
                console.log('Autmpay API Response:', response);

                if (response && response.ok) {
                    try {
                        // Get the file content and create download
                        const blob = await response.blob();

                        // Ensure we're handling CSV content properly
                        const csvBlob = new Blob([blob], { type: 'text/csv' });

                        // Create a temporary URL for the blob
                        const url = window.URL.createObjectURL(csvBlob);

                        // Create a temporary anchor element
                        const a = document.createElement('a');
                        a.style.display = 'none';
                        a.href = url;

                        // Set the filename
                        let filename = `Autmpay_Report_Client_${new Date().toISOString().slice(0, 10)}.csv`;
                        const contentDisposition = response.headers.get('content-disposition');
                        if (contentDisposition) {
                            const filenameMatch = contentDisposition.match(/filename="?([^";]+)"?/);
                            if (filenameMatch && filenameMatch[1]) {
                                filename = filenameMatch[1];
                            }
                        }
                        a.download = filename;

                        // Append to the document, click, and remove
                        document.body.appendChild(a);
                        a.click();
                        document.body.removeChild(a);

                        // Clean up the URL object
                        window.URL.revokeObjectURL(url);

                        showReportStatus('Autmpay report generated and downloaded successfully!', 'success');
                    } catch (downloadError) {
                        console.error('Error during download:', downloadError);
                        // Fallback: try to open in new tab
                        try {
                            const blob = await response.blob();
                            const url = window.URL.createObjectURL(blob);
                            window.open(url, '_blank');
                            showReportStatus('Autmpay report opened in new tab. Please save it manually.', 'warning');
                        } catch (fallbackError) {
                            console.error('Fallback download also failed:', fallbackError);
                            showReportStatus('Error generating Autmpay report. Please check console for details.', 'danger');
                        }
                    }
                } else {
                    let errorText = 'Unknown error';
                    try {
                        errorText = await response.text();
                    } catch (e) {
                        errorText = `HTTP Error: ${response.status} ${response.statusText}`;
                    }
                    showReportStatus('Error generating Autmpay report: ' + errorText, 'danger');
                }
            } catch (error) {
                console.error('Error generating Autmpay report:', error);
                showReportStatus('Error generating Autmpay report: ' + error.message, 'danger');
            }
        };

        fileInput.click();
    }

    // Generate CPMPay Report (for Vendors)
    async function generateCPMPayReport() {
        const userType = getUserType();
        if (userType !== 'Merchant') {
            showAlert('CPMPay reports are only available for Vendors', 'warning');
            return;
        }

        showReportStatus('Please upload your CPMPay file to generate report...', 'info');

        // Create file input for CPMPay
        const fileInput = document.createElement('input');
        fileInput.type = 'file';
        // Allow any file type to be uploaded to support files without extensions
        fileInput.accept = '';

        fileInput.onchange = async (event) => {
            const file = event.target.files[0];
            if (!file) {
                showReportStatus('No file selected', 'warning');
                return;
            }

            showReportStatus('Uploading and processing CPMPay file...', 'info');

            // Check if file is CSV or can be processed as CSV
            console.log('Selected CPMPay file:', file.name, file.type);

            try {
                // Create form data with proper content type
                const formData = new FormData();
                formData.append('file', file);

                showReportStatus('Processing file... Please wait.', 'info');

                // Log file details for debugging
                console.log('Selected CPMPay file:', file.name, file.type, file.size);

                // Use a direct fetch for file uploads to avoid Content-Type conflicts
                const token = getAuthToken();
                const response = await fetch(`${AppConfig.API_BASE_URL}/CPMPay/ParseCpmpayFile`, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                // Handle authentication errors
                if (response.status === 401) {
                    localStorage.removeItem('authToken');
                    window.location.href = '/login.html';
                    return;
                }

                // Log response for debugging
                console.log('CPMPay API Response:', response);

                if (response && response.ok) {
                    try {
                        // Get the file content and create download
                        const blob = await response.blob();

                        // Ensure we're handling CSV content properly
                        const csvBlob = new Blob([blob], { type: 'text/csv' });

                        // Create a temporary URL for the blob
                        const url = window.URL.createObjectURL(csvBlob);

                        // Create a temporary anchor element
                        const a = document.createElement('a');
                        a.style.display = 'none';
                        a.href = url;

                        // Set the filename
                        let filename = `CPMPay_Report_Vendor_${new Date().toISOString().slice(0, 10)}.csv`;
                        const contentDisposition = response.headers.get('content-disposition');
                        if (contentDisposition) {
                            const filenameMatch = contentDisposition.match(/filename="?([^";]+)"?/);
                            if (filenameMatch && filenameMatch[1]) {
                                filename = filenameMatch[1];
                            }
                        }
                        a.download = filename;

                        // Append to the document, click, and remove
                        document.body.appendChild(a);
                        a.click();
                        document.body.removeChild(a);

                        // Clean up the URL object
                        window.URL.revokeObjectURL(url);

                        showReportStatus('CPMPay report generated and downloaded successfully!', 'success');
                    } catch (downloadError) {
                        console.error('Error during download:', downloadError);
                        // Fallback: try to open in new tab
                        try {
                            const blob = await response.blob();
                            const url = window.URL.createObjectURL(blob);
                            window.open(url, '_blank');
                            showReportStatus('CPMPay report opened in new tab. Please save it manually.', 'warning');
                        } catch (fallbackError) {
                            console.error('Fallback download also failed:', fallbackError);
                            showReportStatus('Error generating CPMPay report. Please check console for details.', 'danger');
                        }
                    }
                } else {
                    let errorText = 'Unknown error';
                    try {
                        errorText = await response.text();
                    } catch (e) {
                        errorText = `HTTP Error: ${response.status} ${response.statusText}`;
                    }
                    showReportStatus('Error generating CPMPay report: ' + errorText, 'danger');
                }
            } catch (error) {
                console.error('Error generating CPMPay report:', error);
                showReportStatus('Error generating CPMPay report: ' + error.message, 'danger');
            }
        };

        fileInput.click();
    }

    // Show report status message
    function showReportStatus(message, type) {
        const statusDiv = document.getElementById('reportStatus');
        statusDiv.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
    }

    // Make functions globally available
    window.loadTransactions = loadTransactions;
    window.viewTransaction = viewTransaction;
    window.saveTransaction = saveTransaction;
    window.openCreateTransactionModal = openCreateTransactionModal;
    window.createTransaction = createTransaction;
    window.openTransferRequestModal = openTransferRequestModal;
    window.requestTransfer = requestTransfer;
    window.acceptTransfer = acceptTransfer;
    window.generateAutmpayReport = generateAutmpayReport;
    window.generateCPMPayReport = generateCPMPayReport;
})();
