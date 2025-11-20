// Wallet Management Script
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
        await initializeWalletBalance();

        // Setup dashboard links based on user type
        setupDashboardLinks();

        // Check user type access
        const userType = getUserType();
        if (userType !== 'Client' && userType !== 'Merchant') {
            showAlert('Access denied. Wallet is only available for Clients and Merchants.', 'danger');
            setTimeout(() => {
                window.location.href = getDashboardUrl();
            }, 2000);
            return;
        }

        // Load wallet data
        await loadWallet();

        // Setup form submission
        const walletForm = document.getElementById('walletForm');
        if (walletForm) {
            walletForm.addEventListener('submit', handleWalletUpdate);
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

    // Load wallet data from API
    async function loadWallet() {
        const userId = localStorage.getItem('userId');
        console.log('Loading wallet for userId:', userId); // Debug log

        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        try {
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/details/${userId}`);
            console.log('Wallet API response status:', response ? response.status : 'null'); // Debug log

            if (response && response.ok) {
                const wallet = await response.json();
                console.log('Wallet data loaded:', wallet); // Debug log
                console.log('Wallet type:', typeof wallet); // Debug log
                console.log('Wallet keys:', Object.keys(wallet)); // Debug log
                console.log('Direct access test:', {
                    walletId: wallet.walletId,
                    WalletId: wallet.WalletId,
                    balance: wallet.balance,
                    Balance: wallet.Balance,
                    userId: wallet.userId,
                    UserId: wallet.UserId,
                    transactionLimit: wallet.transactionLimit,
                    TransactionLimit: wallet.TransactionLimit
                }); // Debug log
                console.log('Wallet properties:', {
                    walletId: wallet.walletId || wallet.WalletId,
                    balance: wallet.balance || wallet.Balance,
                    userId: wallet.userId || wallet.UserId,
                    accountId: wallet.accountId || wallet.AccountId,
                    transactionLimit: wallet.transactionLimit || wallet.TransactionLimit,
                    transactionCount: wallet.transactionCount || wallet.TransactionCount
                }); // Debug log
                populateWalletData(wallet);
                updateWalletSummary(wallet);
            } else {
                // Try to get error message from response
                let errorMessage = 'Failed to load wallet data';
                try {
                    const errorData = await response.json();
                    console.error('Error response from API:', errorData); // Debug log

                    // Show detailed error information
                    if (errorData.error) {
                        errorMessage = `Error: ${errorData.error}`;
                        if (errorData.innerError) {
                            errorMessage += `\nInner Error: ${errorData.innerError}`;
                        }
                        if (errorData.stackTrace) {
                            console.error('Stack trace:', errorData.stackTrace);
                        }
                    } else if (errorData.message) {
                        errorMessage = errorData.message;
                    }
                } catch (e) {
                    console.error('Failed to parse error response:', e); // Debug log
                }

                if (errorMessage.includes('not found')) {
                    showAlert('No wallet found for this user. Please contact support to create a wallet.', 'warning');
                } else {
                    showAlert(errorMessage, 'danger');
                }
            }
        } catch (error) {
            console.error('Error loading wallet:', error);
            showAlert('Error loading wallet data: ' + error.message, 'danger');
        }
    }

    // Populate wallet form with data
    function populateWalletData(wallet) {
        // Handle both PascalCase and camelCase property names
        const walletId = wallet.walletId || wallet.WalletId || '';
        const accountId = wallet.accountId || wallet.AccountId || '';
        const balance = wallet.balance || wallet.Balance || 0;
        const transactionLimit = wallet.transactionLimit || wallet.TransactionLimit || 0;
        const transactionCount = wallet.transactionCount || wallet.TransactionCount || 0;

        console.log('populateWalletData values:', { walletId, accountId, balance, transactionLimit, transactionCount }); // Debug log

        document.getElementById('walletId').value = walletId;
        if (walletId) {
            localStorage.setItem('walletId', walletId);
        }
        document.getElementById('accountId').value = accountId;
        document.getElementById('balance').value = balance;
        document.getElementById('transactionLimit').value = transactionLimit;

        // Update overview cards
        document.getElementById('cardBalance').textContent = `${balance.toFixed(2)} TND`;
        document.getElementById('cardLimit').textContent = `${transactionLimit.toFixed(2)} TND`;
        document.getElementById('cardTransactions').textContent = transactionCount;
    }

    // Update wallet summary card
    function updateWalletSummary(wallet) {
        // Handle both PascalCase and camelCase property names
        const userName = wallet.userName || wallet.UserName || getUserName() || 'User Name';
        const userEmail = wallet.userEmail || wallet.UserEmail || localStorage.getItem('userEmail') || 'user@example.com';
        const userId = wallet.userId || wallet.UserId || localStorage.getItem('userId') || 'N/A';
        const transactionCount = wallet.transactionCount || wallet.TransactionCount || 0;

        console.log('updateWalletSummary values:', { userName, userEmail, userId, transactionCount }); // Debug log

        document.getElementById('summaryName').textContent = userName;
        document.getElementById('summaryEmail').textContent = userEmail;
        document.getElementById('summaryUserId').textContent = userId;
        document.getElementById('summaryTransactions').textContent = `${transactionCount} Transactions`;
    }

    // Handle wallet update form submission
    async function handleWalletUpdate(event) {
        event.preventDefault();

        const userId = localStorage.getItem('userId');
        if (!userId) {
            showAlert('Error: User ID not found', 'danger');
            return;
        }

        const saveBtn = document.getElementById('saveBtn');
        saveBtn.disabled = true;
        saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';

        const updateData = {
            balance: parseFloat(document.getElementById('balance').value),
            transactionLimit: parseFloat(document.getElementById('transactionLimit').value)
        };

        try {
            // Get the walletId from the current loaded wallet
            const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/details/${userId}`);

            if (response && response.ok) {
                const wallet = await response.json();
                console.log('Wallet for update:', wallet); // Debug log
                console.log('Wallet keys for update:', Object.keys(wallet)); // Debug log
                console.log('Direct access test for update:', {
                    walletId: wallet.walletId,
                    WalletId: wallet.WalletId
                }); // Debug log
                const walletId = wallet.walletId || wallet.WalletId;
                console.log('Extracted walletId for update:', walletId); // Debug log

                // Now update the wallet
                console.log('Attempting to update wallet with ID:', walletId); // Debug log
                const updateResponse = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/update/${walletId}`, {
                    method: 'PUT',
                    body: JSON.stringify(updateData)
                });
                console.log('Update response status:', updateResponse ? updateResponse.status : 'null'); // Debug log

                if (updateResponse && updateResponse.ok) {
                    showAlert('Wallet updated successfully!', 'success');
                    // Add notification
                    if (typeof BTPayProNotifications !== 'undefined') {
                        BTPayProNotifications.addNotification(
                            BTPayProNotifications.NOTIFICATION_TYPES.WALLET_UPDATED,
                            'Wallet Updated',
                            `Your wallet has been updated. New balance: ${updateData.balance.toFixed(2)} TND`
                        );
                    }

                    // Update localStorage wallet balance
                    localStorage.setItem('walletBalance', updateData.balance.toString());

                    // Refresh wallet data
                    await loadWallet();

                    // Update topbar balance display
                    updateWalletBalanceDisplay();
                } else {
                    const error = await updateResponse.json();
                    showAlert(error.message || 'Failed to update wallet', 'danger');
                    // Add notification for failed update
                    if (typeof BTPayProNotifications !== 'undefined') {
                        BTPayProNotifications.addNotification(
                            BTPayProNotifications.NOTIFICATION_TYPES.TRANSACTION_FAILED,
                            'Wallet Update Failed',
                            `Failed to update wallet: ${error.message || 'Unknown error'}`
                        );
                    }
                }
            } else {
                showAlert('Failed to load wallet information', 'danger');
            }
        } catch (error) {
            console.error('Error updating wallet:', error);
            showAlert('Error updating wallet', 'danger');
        } finally {
            saveBtn.disabled = false;
            saveBtn.innerHTML = '<i class="fas fa-save"></i> Save Changes';
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

    // Make functions globally available
    window.loadWallet = loadWallet;
})();
