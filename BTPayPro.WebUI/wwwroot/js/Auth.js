// BTPayPro Authentication Handler
// API_BASE_URL is defined in config.js

// Login Handler
async function handleLogin(event) {
    event.preventDefault();
    console.log('handleLogin called');

    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    const errorDiv = document.getElementById('loginError');
    const submitBtn = document.getElementById('loginBtn');

    console.log('Login attempt with email:', email);

    // Clear previous errors
    errorDiv.style.display = 'none';
    errorDiv.textContent = '';

    // Disable button and show loading state
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Logging in...';

    try {
        const response = await fetch(`${AppConfig.API_BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        });

        if (response.ok) {
            const data = await response.json();
            console.log('Login successful, response data:', data);

            // Store the token in localStorage
            localStorage.setItem(AppConfig.TOKEN_KEY, data.token);

            // Store user info
            localStorage.setItem(AppConfig.USER_EMAIL_KEY, data.email);
            localStorage.setItem('userName', data.userName);
            localStorage.setItem('userId', data.userId);
            localStorage.setItem('userType', data.userType);

            // Store wallet balance if available
            if (data.walletBalance !== null && data.walletBalance !== undefined) {
                localStorage.setItem('walletBalance', data.walletBalance.toString());
            }

            // Add welcome notification
            if (typeof BTPayProNotifications !== 'undefined') {
                BTPayProNotifications.addNotification(
                    BTPayProNotifications.NOTIFICATION_TYPES.PROFILE_UPDATED,
                    'Welcome Back',
                    `Welcome back, ${data.userName || 'User'}! You've successfully logged in.`
                );
            }

            // Redirect to the appropriate dashboard
            let redirectUrl = data.redirectUrl || '/login.html';
            console.log('Redirecting to:', redirectUrl);

            // Fallback logic if redirectUrl is undefined or empty
            if (!redirectUrl || redirectUrl === 'undefined' || redirectUrl === '/login.html') {
                // Try to determine redirect URL based on user type
                const userType = data.userType;
                console.log('Fallback - User type:', userType);
                if (userType === 'Client' || userType === '0') {
                    redirectUrl = '/dashboards/dashboard-client.html';
                } else if (userType === 'Merchant' || userType === '1') {
                    redirectUrl = '/dashboards/dashboard-vendor.html';
                } else if (userType === 'Admin' || userType === '2') {
                    redirectUrl = '/dashboards/dashboard-admin.html';
                }
                console.log('Fallback redirect URL:', redirectUrl);
            }

            window.location.href = redirectUrl;
        } else {
            console.log('Login failed with status:', response.status);
            // Handle error response - may be JSON or plain text
            let errorMessage = 'Invalid credentials. Please try again.';
            try {
                const data = await response.json();
                errorMessage = data.message || data || errorMessage;
            } catch {
                // If not JSON, try reading as text
                const text = await response.text();
                errorMessage = text || errorMessage;
            }

            errorDiv.textContent = errorMessage;
            errorDiv.style.display = 'block';
            submitBtn.disabled = false;
            submitBtn.innerHTML = 'Login';
        }
    } catch (error) {
        console.error('Login error:', error);
        errorDiv.textContent = 'An error occurred. Please check your connection and try again.';
        errorDiv.style.display = 'block';
        submitBtn.disabled = false;
        submitBtn.innerHTML = 'Login';
    }
}

// Check if user is already logged in
function checkAuthentication() {
    const token = localStorage.getItem(AppConfig.TOKEN_KEY);
    const currentPage = window.location.pathname;

    console.log('checkAuthentication: token=', token, 'currentPage=', currentPage);

    // If on login page and already authenticated, redirect to dashboard
    if (token && currentPage.includes('login.html')) {
        // Redirect based on user type
        const userType = localStorage.getItem('userType');
        console.log('User is authenticated with type:', userType);

        if (userType === 'Admin') {
            window.location.href = '/dashboards/dashboard-admin.html';
        } else if (userType === 'Merchant') {
            window.location.href = '/dashboards/dashboard-vendor.html';
        } else if (userType === 'Client') {
            window.location.href = '/dashboards/dashboard-client.html';
        } else {
            // If user type is unknown, clear storage and stay on login
            console.log('Unknown user type, clearing auth data');
            localStorage.removeItem(AppConfig.TOKEN_KEY);
            localStorage.removeItem('userType');
            return;
        }
    }
}

// Logout Handler
function handleLogout() {
    localStorage.removeItem(AppConfig.TOKEN_KEY);
    localStorage.removeItem(AppConfig.USER_EMAIL_KEY);
    localStorage.removeItem('userName');
    localStorage.removeItem('userId');
    localStorage.removeItem('userType');
    localStorage.removeItem('walletBalance');
    localStorage.removeItem('profilePhotoUrl');
    window.location.href = '/login.html';
}

// Get wallet balance from localStorage
function getWalletBalance() {
    const balance = localStorage.getItem('walletBalance');
    return balance ? parseFloat(balance) : null;
}

// Get user type from localStorage
function getUserType() {
    return localStorage.getItem('userType');
}

// Get user name from localStorage
function getUserName() {
    return localStorage.getItem('userName') || 'User';
}

// Get profile photo URL from localStorage
function getProfilePhotoUrl() {
    return localStorage.getItem('profilePhotoUrl') || 'img/undraw_profile.svg';
}

// Update profile photo in topbar
function updateTopbarProfilePhoto() {
    const photoUrl = getProfilePhotoUrl();
    const topbarAvatar = document.querySelector('.topbar .img-profile');
    if (topbarAvatar) {
        topbarAvatar.src = photoUrl;
        topbarAvatar.onerror = function () {
            this.src = 'img/undraw_profile.svg';
        };
    }
}

// Fetch current profile photo from API
async function fetchProfilePhoto() {
    const userId = localStorage.getItem('userId');
    if (!userId) return;

    try {
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Profile/${userId}`);
        if (response && response.ok) {
            const profile = await response.json();
            if (profile.profilePhotoUrl) {
                localStorage.setItem('profilePhotoUrl', profile.profilePhotoUrl);
                updateTopbarProfilePhoto();
            }
        }
    } catch (error) {
        console.error('Error fetching profile photo:', error);
    }
}

// Fetch current wallet balance from API
async function fetchWalletBalance() {
    const userId = localStorage.getItem('userId');
    if (!userId) return null;

    try {
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/user/${userId}`);
        if (response && response.ok) {
            const data = await response.json();
            // Update localStorage with fresh balance
            // Handle both PascalCase and camelCase property names
            const accountBalance = data.AccountBalance || data.accountBalance;
            if (accountBalance !== null && accountBalance !== undefined) {
                localStorage.setItem('walletBalance', accountBalance.toString());
                return accountBalance;
            }
        }
    } catch (error) {
        console.error('Error fetching wallet balance:', error);
    }
    return null;
}

// Fetch complete wallet data including transaction limit
async function fetchWalletData() {
    const userId = localStorage.getItem('userId');
    if (!userId) return null;

    try {
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Wallet/details/${userId}`);
        if (response && response.ok) {
            const wallet = await response.json();
            // Handle both PascalCase and camelCase property names
            const accountBalance = wallet.balance || wallet.Balance;
            const transactionLimit = wallet.transactionLimit || wallet.TransactionLimit || 0; // Default to 0 if not present

            if (accountBalance !== null && accountBalance !== undefined) {
                localStorage.setItem('walletBalance', accountBalance.toString());
            }

            return {
                balance: accountBalance,
                transactionLimit: transactionLimit
            };
        }
    } catch (error) {
        console.error('Error fetching wallet data:', error);
    }
    return null;
}

// Update wallet balance display in the UI
function updateWalletBalanceDisplay() {
    const balanceElement = document.getElementById('walletBalanceDisplay');
    if (balanceElement) {
        const balance = getWalletBalance();
        if (balance !== null) {
            const oldBalance = balanceElement.textContent;
            const newBalance = `${balance.toFixed(2)} TND`;

            // Only animate if balance changed
            if (oldBalance !== newBalance) {
                balanceElement.textContent = newBalance;
                balanceElement.classList.add('balance-updated');
                setTimeout(() => {
                    balanceElement.classList.remove('balance-updated');
                }, 500);
            } else {
                balanceElement.textContent = newBalance;
            }
        }
    }
}

// Initialize wallet balance on page load for dashboards
async function initializeWalletBalance() {
    const userType = getUserType();

    // Only show wallet for Client and Merchant
    if (userType === 'Client' || userType === 'Merchant') {
        // Fetch fresh balance from API
        await fetchWalletBalance();
        updateWalletBalanceDisplay();

        // Update balance every 30 seconds
        setInterval(async () => {
            await fetchWalletBalance();
            updateWalletBalanceDisplay();
        }, 30000);
    }
}

// Initialize wallet data (balance and transaction limit) on page load for dashboards
async function initializeWalletData() {
    const userType = getUserType();

    // Only show wallet for Client and Merchant
    if (userType === 'Client' || userType === 'Merchant') {
        // Fetch complete wallet data from API
        const walletData = await fetchWalletData();
        if (walletData) {
            updateWalletBalanceDisplay();
            updateTransactionLimitDisplay(walletData.transactionLimit);
        } else {
            // Fallback: Try to update with any available data
            const walletBalance = getWalletBalance();
            if (walletBalance !== null) {
                updateWalletBalanceDisplay();
            }
            // For transaction limit, we don't have a localStorage fallback, so we'll show 0.00 TND
            updateTransactionLimitDisplay(0);
        }

        // Update data every 30 seconds
        setInterval(async () => {
            const walletData = await fetchWalletData();
            if (walletData) {
                updateWalletBalanceDisplay();
                updateTransactionLimitDisplay(walletData.transactionLimit);
            } else {
                // Fallback: Try to update with any available data
                const walletBalance = getWalletBalance();
                if (walletBalance !== null) {
                    updateWalletBalanceDisplay();
                }
                // For transaction limit, we don't have a localStorage fallback, so we'll show 0.00 TND
                updateTransactionLimitDisplay(0);
            }
        }, 30000);
    }
}

// Update transaction limit display in the UI
function updateTransactionLimitDisplay(transactionLimit) {
    const limitElements = document.querySelectorAll('#dashboardTransactionLimit');
    limitElements.forEach(element => {
        if (element) {
            // Handle null, undefined, or NaN values
            if (transactionLimit !== null && transactionLimit !== undefined && !isNaN(transactionLimit)) {
                element.textContent = `${parseFloat(transactionLimit).toFixed(2)} TND`;
            } else {
                element.textContent = '0.00 TND';
            }
        }
    });
}

// Get Auth Token for API calls
function getAuthToken() {
    return localStorage.getItem(AppConfig.TOKEN_KEY);
}

// Make authenticated API request
async function authenticatedFetch(url, options = {}) {
    const token = getAuthToken();

    if (!token) {
        window.location.href = '/login.html';
        return;
    }

    const headers = {
        ...options.headers,
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };

    try {
        console.log('Making API request to:', url); // Debug log
        const response = await fetch(url, { ...options, headers });
        console.log('API response status:', response ? response.status : 'null'); // Debug log

        if (response && response.status === 401) {
            // Token expired or invalid
            localStorage.removeItem('authToken');
            window.location.href = '/login.html';
            return;
        }

        return response;
    } catch (error) {
        console.error('API request error:', error);
        throw error;
    }
}

// Protect dashboard pages - call this on dashboard pages to ensure user is authenticated
function protectPage() {
    const token = getAuthToken();
    console.log('protectPage: token=', token);
    if (!token) {
        console.log('No token found, redirecting to login');
        window.location.href = '/login.html';
    }
}

// Update user type badge based on user type
function updateUserTypeBadge() {
    const userType = getUserType();
    const userTypeBadge = document.getElementById('userTypeBadge');
    const userTypeText = document.getElementById('userTypeText');

    if (userTypeBadge && userTypeText) {
        // Set the text based on user type
        if (userType === 'Admin') {
            userTypeText.textContent = 'Admin Dashboard';
            userTypeBadge.classList.remove('badge-primary', 'badge-info', 'badge-success');
            userTypeBadge.classList.add('badge-danger');
            userTypeBadge.innerHTML = '<i class="fas fa-shield-alt"></i> ' + userTypeText.textContent;
        } else if (userType === 'Client') {
            userTypeText.textContent = 'Client Dashboard';
            userTypeBadge.classList.remove('badge-primary', 'badge-danger', 'badge-success');
            userTypeBadge.classList.add('badge-info');
            userTypeBadge.innerHTML = '<i class="fas fa-user"></i> ' + userTypeText.textContent;
        } else if (userType === 'Merchant') {
            userTypeText.textContent = 'Merchant Dashboard';
            userTypeBadge.classList.remove('badge-primary', 'badge-danger', 'badge-info');
            userTypeBadge.classList.add('badge-success');
            userTypeBadge.innerHTML = '<i class="fas fa-store"></i> ' + userTypeText.textContent;
        } else {
            // For non-dashboard pages, keep the existing text but update the badge color
            userTypeBadge.classList.remove('badge-danger', 'badge-info', 'badge-success');
            userTypeBadge.classList.add('badge-primary');
            if (userType) {
                userTypeBadge.innerHTML = '<i class="fas fa-user"></i> ' + userType + ' ' + userTypeText.textContent;
            }
        }
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    checkAuthentication();
    updateUserTypeBadge();
});