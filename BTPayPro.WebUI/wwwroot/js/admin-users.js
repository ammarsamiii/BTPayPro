// Test if script is loaded
console.log('Admin users script loaded');

// Add a delay to ensure all functions are available
setTimeout(() => {
    // Make functions globally accessible
    window.loadAllUsers = loadAllUsers;
    window.toggleAddUserForm = toggleAddUserForm;
    window.deleteUser = deleteUser;
    window.updateStatisticsFromAPI = updateStatisticsFromAPI;
    window.handleAddUserFormSubmit = handleAddUserFormSubmit;
    console.log('Functions made globally accessible');
}, 100);

// Toggle the add user form visibility
function toggleAddUserForm() {
    const formContainer = document.getElementById('addUserFormContainer');
    if (formContainer) {
        if (formContainer.style.display === 'none') {
            formContainer.style.display = 'block';
        } else {
            formContainer.style.display = 'none';
        }
    }
}

// Handle add user form submission
async function handleAddUserFormSubmit(event) {
    event.preventDefault();

    // Get form values
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const userType = document.getElementById('userType').value;
    const password = document.getElementById('password').value;
    const repeatPassword = document.getElementById('repeatPassword').value;

    // Basic validation
    if (!username || !email || !firstName || !lastName || !userType || !password || !repeatPassword) {
        showAlert('Please fill in all fields', 'warning');
        return;
    }

    if (password !== repeatPassword) {
        showAlert('Passwords do not match', 'warning');
        return;
    }

    // Create user data object - using the structure expected by the AuthController
    const userData = {
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
        userType: userType
    };

    try {
        // Make API call to create user using the register endpoint
        const response = await fetch(`${AppConfig.API_BASE_URL}/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });

        if (response && response.ok) {
            showAlert('User created successfully', 'success');
            // Reset form
            document.getElementById('addUserForm').reset();
            // Hide form
            toggleAddUserForm();
            // Refresh user list
            await loadAllUsers();
            // Refresh statistics
            await updateStatisticsFromAPI();
        } else {
            const errorText = await response.text();
            console.error('Error creating user:', errorText);
            showAlert('Error creating user: ' + errorText, 'danger');
        }
    } catch (error) {
        console.error('Error creating user:', error);
        showAlert('Error creating user: ' + error.message, 'danger');
    }
}

// Show alert message
function showAlert(message, type) {
    const alertContainer = document.getElementById('alertContainer');
    if (alertContainer) {
        alertContainer.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
    }
}

// Load all users from API - using the same approach as dashboard-admin
async function loadAllUsers() {
    try {
        // Check if authenticatedFetch is available
        if (typeof authenticatedFetch !== 'function') {
            console.error('authenticatedFetch function is not available');
            const tbody = document.getElementById('usersTableBody');
            if (tbody) {
                tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Authentication system not loaded</td></tr>';
            }
            return;
        }

        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/User`);

        if (response && response.ok) {
            const users = await response.json();

            // Log the raw users data for debugging
            console.log('Raw users data from API:', users);

            // Display all users in table
            await displayUsers(users);
        } else {
            // Show error state when users cannot be loaded
            const tbody = document.getElementById('usersTableBody');
            if (tbody) {
                tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">User data not available</td></tr>';
            }
        }
    } catch (error) {
        console.error('Error loading users:', error);
        // Show error state when users cannot be loaded
        const tbody = document.getElementById('usersTableBody');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">User data not available</td></tr>';
        }
    }
}

// Display users in table - using the same approach as dashboard-admin
async function displayUsers(users) {
    const tbody = document.getElementById('usersTableBody');

    // Always clear the table first
    if (!tbody) {
        console.error('Could not find usersTableBody element');
        return;
    }

    // Check if users array is empty or invalid
    if (!users || !Array.isArray(users) || users.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center">No users found in database</td></tr>';
        return;
    }

    // Generate table rows
    const rows = users.map(user => {
        // Log each user for debugging
        console.log('Processing user:', user);

        // Extract available properties from the user object with fallbacks
        const email = user.email || user.Email || 'N/A';
        const userId = user.userId || user.UserId || user.id || 'N/A';
        const firstName = user.firstName || user.FirstName || user.firstname || '';
        const lastName = user.lastName || user.LastName || user.lastname || '';
        const userType = user.userType || user.UserType || user.type || user.Type || '';

        // Format user type badge
        const typeMap = {
            'Client': 'primary',
            'Merchant': 'warning',
            'Admin': 'danger'
        };
        const typeClass = typeMap[userType] || 'secondary';
        const typeBadge = `<span class="badge badge-${typeClass}">${userType || 'Unknown'}</span>`;

        return `
        <tr id="user-row-${userId}">
            <td>${lastName || 'N/A'}</td>
            <td>${firstName || 'N/A'}</td>
            <td>${email}</td>
            <td>${typeBadge}</td>
            <td>
                <button class="btn btn-danger btn-sm" onclick="deleteUser('${userId}', this)">
                    <i class="fas fa-trash"></i> Delete
                </button>
            </td>
        </tr>`;
    });

    const html = rows.join('');
    tbody.innerHTML = html;
}

// Delete user from the block (client-side only)
function deleteUser(userId, buttonElement) {
    if (!userId || userId === 'N/A') {
        console.error('No user ID provided for deletion');
        alert('No user ID provided for deletion');
        return;
    }

    // Confirm deletion
    if (!confirm('Are you sure you want to delete this user from the list?')) {
        return;
    }

    try {
        // Remove the row from the table
        const row = buttonElement.closest('tr');
        if (row) {
            row.remove();
            console.log('User row removed successfully for user ID:', userId);
            alert('User removed from the list successfully');

            // Refresh statistics since we've removed a user
            updateStatisticsFromAPI();
        } else {
            console.error('Could not find row to remove for user ID:', userId);
            alert('Could not find user row to remove');
        }
    } catch (error) {
        console.error('Error removing user row:', error);
        alert(`Error removing user: ${error.message}`);
    }
}

// Update statistics by fetching data directly from the API
async function updateStatisticsFromAPI() {
    try {
        // Check if authenticatedFetch is available
        if (typeof authenticatedFetch !== 'function') {
            console.error('authenticatedFetch function is not available');
            resetStatistics();
            return;
        }

        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/User`);

        if (response && response.ok) {
            const users = await response.json();

            // Update statistics directly from the fetched data
            updateStatistics(users);
        } else {
            resetStatistics();
        }
    } catch (error) {
        console.error('Error loading statistics:', error);
        resetStatistics();
    }
}

// Reset statistics to zero
function resetStatistics() {
    document.getElementById('totalCount').textContent = '0';
    document.getElementById('clientCount').textContent = '0';
    document.getElementById('merchantCount').textContent = '0';
    document.getElementById('adminCount').textContent = '0';
}

// Update statistics
function updateStatistics(users) {
    const totalUsers = users.length;
    let clientCount = 0;
    let merchantCount = 0;
    let adminCount = 0;

    // Count user types properly
    users.forEach(user => {
        const userType = user.userType || user.UserType || '';
        switch (userType) {
            case 'Client':
            case '0':
                clientCount++;
                break;
            case 'Merchant':
            case '1':
                merchantCount++;
                break;
            case 'Admin':
            case '2':
                adminCount++;
                break;
        }
    });

    document.getElementById('totalCount').textContent = totalUsers;
    document.getElementById('clientCount').textContent = clientCount;
    document.getElementById('merchantCount').textContent = merchantCount;
    document.getElementById('adminCount').textContent = adminCount;
}

document.addEventListener('DOMContentLoaded', async function () {
    console.log('DOM loaded, initializing admin users page');

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

    // Load statistics data
    console.log('Loading statistics data');
    await updateStatisticsFromAPI();

    // Load users data
    console.log('Loading all users data');
    await loadAllUsers();

    // Add event listener for the add user form
    const addUserForm = document.getElementById('addUserForm');
    if (addUserForm) {
        addUserForm.addEventListener('submit', handleAddUserFormSubmit);
    }

    // Also make functions available after DOM is loaded
    window.loadAllUsers = loadAllUsers;
    window.toggleAddUserForm = toggleAddUserForm;
    window.deleteUser = deleteUser;
    window.updateStatisticsFromAPI = updateStatisticsFromAPI;
    window.handleAddUserFormSubmit = handleAddUserFormSubmit;
    console.log('Functions made globally accessible after DOM loaded');
});