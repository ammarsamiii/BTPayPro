// Load sidebar based on user type
function loadSidebar() {
    const sidebar = document.getElementById('accordionSidebar');
    if (sidebar) {
        try {
            const userType = getUserType();

            // Create sidebar elements programmatically instead of using template literals
            // Clear existing content
            sidebar.innerHTML = '';

            // Sidebar - Brand
            const brandLink = document.createElement('a');
            brandLink.className = 'sidebar-brand d-flex align-items-center justify-content-center';
            brandLink.href = userType === 'Admin' ? '/dashboards/dashboard-admin.html' :
                (userType === 'Client' || userType === '0' ? '/dashboards/dashboard-client.html' :
                    (userType === 'Merchant' || userType === '1' ? '/dashboards/dashboard-vendor.html' : '/dashboards/dashboard-client.html'));
            brandLink.id = 'dashboardLink';

            const brandIconDiv = document.createElement('div');
            brandIconDiv.className = 'sidebar-brand-icon';

            const brandIconImg = document.createElement('img');
            brandIconImg.src = 'img/LogoBTPay.png';
            brandIconImg.style.width = '80px';
            brandIconImg.style.height = '90px';
            brandIconImg.style.borderRadius = '10px';

            brandIconDiv.appendChild(brandIconImg);

            const brandTextDiv = document.createElement('div');
            brandTextDiv.className = 'sidebar-brand-text mx-3';
            brandTextDiv.style.color = 'white !important';
            brandTextDiv.style.fontSize = '1.2rem !important';
            brandTextDiv.innerHTML = 'BTPay<span style="color:#00bfff;">Pro</span>';

            brandLink.appendChild(brandIconDiv);
            brandLink.appendChild(brandTextDiv);
            sidebar.appendChild(brandLink);

            // Divider
            const divider1 = document.createElement('hr');
            divider1.className = 'sidebar-divider my-0';
            sidebar.appendChild(divider1);

            // Nav Item - Dashboard
            const dashboardItem = document.createElement('li');
            dashboardItem.className = 'nav-item';

            const dashboardLink = document.createElement('a');
            dashboardLink.className = 'nav-link';
            dashboardLink.href = userType === 'Admin' ? '/dashboards/dashboard-admin.html' :
                (userType === 'Client' || userType === '0' ? '/dashboards/dashboard-client.html' :
                    (userType === 'Merchant' || userType === '1' ? '/dashboards/dashboard-vendor.html' : '/dashboards/dashboard-client.html'));
            dashboardLink.id = 'dashboardNavLink';

            const dashboardIcon = document.createElement('i');
            dashboardIcon.className = 'fas fa-fw fa-tachometer-alt';

            const dashboardSpan = document.createElement('span');
            dashboardSpan.textContent = 'Dashboard';

            dashboardLink.appendChild(dashboardIcon);
            dashboardLink.appendChild(dashboardSpan);
            dashboardItem.appendChild(dashboardLink);
            sidebar.appendChild(dashboardItem);

            if (userType === 'Admin') {
                // Divider
                const divider2 = document.createElement('hr');
                divider2.className = 'sidebar-divider';
                sidebar.appendChild(divider2);

                // Heading - Management
                const managementHeading = document.createElement('div');
                managementHeading.className = 'sidebar-heading';
                managementHeading.textContent = 'Management';
                sidebar.appendChild(managementHeading);

                // Nav Item - Users
                const usersItem = document.createElement('li');
                usersItem.className = 'nav-item';

                const usersLink = document.createElement('a');
                usersLink.className = 'nav-link';
                usersLink.href = '/admin-users.html';

                const usersIcon = document.createElement('i');
                usersIcon.className = 'fas fa-fw fa-users';

                const usersSpan = document.createElement('span');
                usersSpan.textContent = 'User Statistics';

                usersLink.appendChild(usersIcon);
                usersLink.appendChild(usersSpan);
                usersItem.appendChild(usersLink);
                sidebar.appendChild(usersItem);

                // Nav Item - Complaints
                const complaintsItem = document.createElement('li');
                complaintsItem.className = 'nav-item';

                const complaintsLink = document.createElement('a');
                complaintsLink.className = 'nav-link';
                complaintsLink.href = '/admin-complaints.html';

                const complaintsIcon = document.createElement('i');
                complaintsIcon.className = 'fas fa-fw fa-headset';

                const complaintsSpan = document.createElement('span');
                complaintsSpan.textContent = 'Manage Complaints';

                complaintsLink.appendChild(complaintsIcon);
                complaintsLink.appendChild(complaintsSpan);
                complaintsItem.appendChild(complaintsLink);
                sidebar.appendChild(complaintsItem);

            } else {
                // Divider
                const divider2 = document.createElement('hr');
                divider2.className = 'sidebar-divider';
                sidebar.appendChild(divider2);

                // Heading - Account
                const accountHeading = document.createElement('div');
                accountHeading.className = 'sidebar-heading';
                accountHeading.textContent = 'Account';
                sidebar.appendChild(accountHeading);

                // Nav Item - Profile (active)
                const profileItem = document.createElement('li');
                profileItem.className = 'nav-item active';

                const profileLink = document.createElement('a');
                profileLink.className = 'nav-link';
                profileLink.href = '/profile.html';

                const profileIcon = document.createElement('i');
                profileIcon.className = 'fas fa-fw fa-user';

                const profileSpan = document.createElement('span');
                profileSpan.textContent = 'Profile';

                profileLink.appendChild(profileIcon);
                profileLink.appendChild(profileSpan);
                profileItem.appendChild(profileLink);
                sidebar.appendChild(profileItem);

                // Divider
                const divider3 = document.createElement('hr');
                divider3.className = 'sidebar-divider';
                sidebar.appendChild(divider3);

                // Heading - Financial
                const financialHeading = document.createElement('div');
                financialHeading.className = 'sidebar-heading';
                financialHeading.textContent = 'Financial';
                sidebar.appendChild(financialHeading);

                // Nav Item - Wallet
                const walletItem = document.createElement('li');
                walletItem.className = 'nav-item';

                const walletLink = document.createElement('a');
                walletLink.className = 'nav-link';
                walletLink.href = '/wallet.html';

                const walletIcon = document.createElement('i');
                walletIcon.className = 'fas fa-fw fa-wallet';

                const walletSpan = document.createElement('span');
                walletSpan.textContent = 'Wallet';

                walletLink.appendChild(walletIcon);
                walletLink.appendChild(walletSpan);
                walletItem.appendChild(walletLink);
                sidebar.appendChild(walletItem);

                // Nav Item - Transactions
                const transactionsItem = document.createElement('li');
                transactionsItem.className = 'nav-item';

                const transactionsLink = document.createElement('a');
                transactionsLink.className = 'nav-link';
                transactionsLink.href = '/transactions.html';

                const transactionsIcon = document.createElement('i');
                transactionsIcon.className = 'fas fa-fw fa-exchange-alt';

                const transactionsSpan = document.createElement('span');
                transactionsSpan.textContent = 'Transactions';

                transactionsLink.appendChild(transactionsIcon);
                transactionsLink.appendChild(transactionsSpan);
                transactionsItem.appendChild(transactionsLink);
                sidebar.appendChild(transactionsItem);

                // Divider
                const divider4 = document.createElement('hr');
                divider4.className = 'sidebar-divider';
                sidebar.appendChild(divider4);

                // Heading - Support
                const supportHeading = document.createElement('div');
                supportHeading.className = 'sidebar-heading';
                supportHeading.textContent = 'Support';
                sidebar.appendChild(supportHeading);

                // Nav Item - Complaints
                const complaintsItem = document.createElement('li');
                complaintsItem.className = 'nav-item';

                const complaintsLink = document.createElement('a');
                complaintsLink.className = 'nav-link';
                complaintsLink.href = '/complaints.html';

                const complaintsIcon = document.createElement('i');
                complaintsIcon.className = 'fas fa-fw fa-ticket-alt';

                const complaintsSpan = document.createElement('span');
                complaintsSpan.textContent = 'Complaints';

                complaintsLink.appendChild(complaintsIcon);
                complaintsLink.appendChild(complaintsSpan);
                complaintsItem.appendChild(complaintsLink);
                sidebar.appendChild(complaintsItem);
            }

            // Divider
            const divider5 = document.createElement('hr');
            divider5.className = 'sidebar-divider d-none d-md-block';
            sidebar.appendChild(divider5);

            // Sidebar Toggler (Sidebar)
            const togglerDiv = document.createElement('div');
            togglerDiv.className = 'text-center d-none d-md-inline';

            const toggleButton = document.createElement('button');
            toggleButton.className = 'rounded-circle border-0';
            toggleButton.id = 'sidebarToggle';

            togglerDiv.appendChild(toggleButton);
            sidebar.appendChild(togglerDiv);

            // BT Logo at Bottom
            const footerLi = document.createElement('li');
            footerLi.className = 'sidebar-footer';

            const footerImg = document.createElement('img');
            footerImg.src = userType === 'Admin' ? 'img/BTlogo.png' : 'img/BTLogo.png';
            footerImg.alt = 'BT Logo';
            footerImg.className = 'img-fluid';
            footerImg.style.filter = 'none !important';
            footerImg.style.webkitFilter = 'none !important';
            footerImg.style.opacity = '1';

            footerLi.appendChild(footerImg);
            sidebar.appendChild(footerLi);

            // Initialize sidebar toggle functionality
            setTimeout(function () {
                if (typeof $ !== 'undefined') {
                    // Initialize sidebar toggle
                    if ($('#sidebarToggle').length) {
                        $('#sidebarToggle').off('click').on('click', function (e) {
                            e.preventDefault();
                            $('body').toggleClass('sidebar-toggled');
                            $('.sidebar').toggleClass('toggled');
                            if ($('.sidebar').hasClass('toggled')) {
                                $('.sidebar .collapse').collapse('hide');
                            }
                        });
                    }

                    // Initialize topbar toggle
                    if ($('#sidebarToggleTop').length) {
                        $('#sidebarToggleTop').off('click').on('click', function (e) {
                            e.preventDefault();
                            $('body').toggleClass('sidebar-toggled');
                            $('.sidebar').toggleClass('toggled');
                            if ($('.sidebar').hasClass('toggled')) {
                                $('.sidebar .collapse').collapse('hide');
                            }
                        });
                    }
                }
            }, 100);

        } catch (error) {
            console.error('Error generating sidebar content:', error);
            // Fallback sidebar using simpler approach
            sidebar.innerHTML = `
                <a class="sidebar-brand d-flex align-items-center justify-content-center" href="/">
                    <div class="sidebar-brand-icon">
                        <img src="img/LogoBTPay.png" style="width:80px; height:90px; border-radius:10px;">
                    </div>
                    <div class="sidebar-brand-text mx-3">BTPay<span style="color:#00bfff;">Pro</span></div>
                </a>
                <hr class="sidebar-divider my-0">
                <li class="nav-item">
                    <a class="nav-link" href="/">
                        <i class="fas fa-fw fa-tachometer-alt"></i>
                        <span>Dashboard</span>
                    </a>
                </li>
                <hr class="sidebar-divider d-none d-md-block">
                <li class="sidebar-footer">
                    <img src="img/BTLogo.png" alt="BT Logo" class="img-fluid" style="filter: none !important; -webkit-filter: none !important; opacity: 1;">
                </li>
            `;
        }
    }
}

// Load profile data function (referenced in HTML)
async function loadProfile() {
    console.log('Loading profile data...');

    try {
        const userId = localStorage.getItem('userId');
        if (!userId) {
            console.error('User ID not found in localStorage');
            return;
        }

        // Fetch user profile data from API
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Profile/${userId}`);

        if (response && response.ok) {
            const profileData = await response.json();
            console.log('Profile data loaded:', profileData);

            // Populate form fields with profile data
            document.getElementById('firstName').value = profileData.firstName || '';
            document.getElementById('lastName').value = profileData.lastName || '';
            document.getElementById('email').value = profileData.email || '';
            document.getElementById('phoneNumber').value = profileData.phoneNumber || '';
            document.getElementById('projectName').value = profileData.projectName || '';

            // Set user type (read-only)
            const userType = profileData.userType || localStorage.getItem('userType') || 'User';
            document.getElementById('userType').value = userType;

            // Update profile summary section
            document.getElementById('profileName').textContent = `${profileData.firstName || ''} ${profileData.lastName || ''}`.trim() || 'User';
            document.getElementById('profileEmail').textContent = profileData.email || '';
            document.getElementById('profileUserType').textContent = userType;
            document.getElementById('profilePhone').textContent = profileData.phoneNumber || 'Not set';
            document.getElementById('profileProject').textContent = profileData.projectName || 'Not set';

            // Update profile photo
            const profilePhotoUrl = profileData.profilePhotoUrl || 'img/undraw_profile.svg';
            const profilePhoto = document.getElementById('profilePhoto');
            if (profilePhoto) {
                profilePhoto.src = profilePhotoUrl;
            }

            // Store profile photo URL in localStorage
            localStorage.setItem('profilePhotoUrl', profilePhotoUrl);
        } else {
            console.error('Failed to load profile data, status:', response ? response.status : 'unknown');
            showAlert('Failed to load profile data', 'danger');
        }
    } catch (error) {
        console.error('Error loading profile data:', error);
        showAlert('Error loading profile data: ' + error.message, 'danger');
    }
}

// Handle photo upload function (referenced in HTML)
function handlePhotoUpload(event) {
    console.log('Photo upload triggered', event);
    const file = event.target.files[0];
    if (file) {
        // In a real implementation, you would upload the file to the server here
        alert('Photo upload functionality would upload the selected file in a complete implementation');

        // For now, just show a preview
        const reader = new FileReader();
        reader.onload = function (e) {
            const profilePhoto = document.getElementById('profilePhoto');
            if (profilePhoto) {
                profilePhoto.src = e.target.result;
            }
        };
        reader.readAsDataURL(file);
    }
}

// Show alert message
function showAlert(message, type = 'info') {
    const alertContainer = document.getElementById('alertContainer');
    if (alertContainer) {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
        alertContainer.innerHTML = alertHtml;

        // Auto-dismiss success alerts after 3 seconds
        if (type === 'success') {
            setTimeout(() => {
                const alert = alertContainer.querySelector('.alert');
                if (alert) {
                    alert.remove();
                }
            }, 3000);
        }
    }
}

// Handle profile update
async function handleProfileUpdate(event) {
    event.preventDefault();
    console.log('Profile update submitted');

    // Get form values
    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const phoneNumber = document.getElementById('phoneNumber').value;
    const projectName = document.getElementById('projectName').value;

    const userId = localStorage.getItem('userId');
    if (!userId) {
        showAlert('User not authenticated', 'danger');
        return;
    }

    // Prepare update data
    const updateData = {
        userId: userId,
        firstName: firstName,
        lastName: lastName,
        phoneNumber: phoneNumber,
        projectName: projectName
        // Note: email and userType are not included as they shouldn't be editable
    };

    try {
        // Send update request to API
        const response = await authenticatedFetch(`${AppConfig.API_BASE_URL}/Profile/${userId}`, {
            method: 'PUT',
            body: JSON.stringify(updateData)
        });

        if (response && response.ok) {
            showAlert('Profile updated successfully!', 'success');
            // Reload profile data to reflect changes
            await loadProfile();
        } else {
            const errorText = await response.text();
            console.error('Failed to update profile:', errorText);
            showAlert('Failed to update profile: ' + errorText, 'danger');
        }
    } catch (error) {
        console.error('Error updating profile:', error);
        showAlert('Error updating profile: ' + error.message, 'danger');
    }
}

// DOMContentLoaded event handler
document.addEventListener('DOMContentLoaded', async function () {
    console.log('Profile page loaded, initializing...');

    // Protect this page - redirect to login if not authenticated
    protectPage();

    // Load sidebar based on user type
    loadSidebar();

    // Update user name display
    const userNameElement = document.getElementById('userNameDisplay');
    if (userNameElement) {
        userNameElement.textContent = getUserName();
    }

    // Update profile photo
    await fetchProfilePhoto();
    updateTopbarProfilePhoto();

    // Load profile data
    await loadProfile();

    // Set up form submission handler
    const profileForm = document.getElementById('profileForm');
    if (profileForm) {
        profileForm.addEventListener('submit', handleProfileUpdate);
    }

    // Set up photo upload handler
    const photoUpload = document.getElementById('photoUpload');
    if (photoUpload) {
        photoUpload.addEventListener('change', handlePhotoUpload);
    }
});