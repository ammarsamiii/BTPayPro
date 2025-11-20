// BTPayPro Notifications Handler

// Notification types
const NOTIFICATION_TYPES = {
    TRANSACTION_SUBMITTED: 'transaction_submitted',
    TRANSACTION_COMPLETED: 'transaction_completed',
    TRANSACTION_CANCELED: 'transaction_canceled',
    TRANSACTION_FAILED: 'transaction_failed',
    PROFILE_UPDATED: 'profile_updated',
    WALLET_UPDATED: 'wallet_updated',
    COMPLAINT_SENT: 'complaint_sent',
    COMPLAINT_TREATED: 'complaint_treated'
};

// Initialize notifications system
function initNotifications() {
    console.log('Initializing notifications system');

    // Load notifications from localStorage
    loadNotifications();

    // Update notification counter
    updateNotificationCounter();

    // Set up periodic refresh (every 30 seconds)
    setInterval(refreshNotifications, 30000);
}

// Load notifications from localStorage
function loadNotifications() {
    const notifications = localStorage.getItem('notifications');
    if (notifications) {
        try {
            window.notifications = JSON.parse(notifications);
        } catch (e) {
            console.error('Error parsing notifications from localStorage:', e);
            window.notifications = [];
        }
    } else {
        window.notifications = [];
    }
    console.log('Loaded notifications:', window.notifications);
}

// Save notifications to localStorage
function saveNotifications() {
    if (window.notifications) {
        localStorage.setItem('notifications', JSON.stringify(window.notifications));
    }
}

// Add a new notification
function addNotification(type, title, message, userId = null) {
    // Check if this notification is for the current user
    const currentUserId = localStorage.getItem('userId');
    if (userId && currentUserId && userId !== currentUserId) {
        console.log('Notification not for current user, skipping');
        return;
    }

    // Create notification object
    const notification = {
        id: Date.now() + Math.random(), // Simple unique ID
        type: type,
        title: title,
        message: message,
        timestamp: new Date().toISOString(),
        read: false
    };

    // Load current notifications
    loadNotifications();

    // Add new notification at the beginning
    window.notifications.unshift(notification);

    // Keep only the last 50 notifications
    if (window.notifications.length > 50) {
        window.notifications = window.notifications.slice(0, 50);
    }

    // Save to localStorage
    saveNotifications();

    // Update UI
    updateNotificationCounter();
    updateNotificationDropdown();

    console.log('Added notification:', notification);
}

// Mark notification as read
function markNotificationAsRead(notificationId) {
    loadNotifications();

    const notification = window.notifications.find(n => n.id === notificationId);
    if (notification) {
        notification.read = true;
        saveNotifications();
        updateNotificationCounter();
        updateNotificationDropdown();
    }
}

// Mark all notifications as read
function markAllNotificationsAsRead() {
    loadNotifications();

    window.notifications.forEach(notification => {
        notification.read = true;
    });

    saveNotifications();
    updateNotificationCounter();
    updateNotificationDropdown();
}

// Get unread notifications count
function getUnreadNotificationsCount() {
    loadNotifications();
    return window.notifications.filter(n => !n.read).length;
}

// Update notification counter in UI
function updateNotificationCounter() {
    const counterElement = document.querySelector('.badge-counter');
    if (counterElement) {
        const unreadCount = getUnreadNotificationsCount();
        counterElement.textContent = unreadCount > 0 ? (unreadCount > 9 ? '9+' : unreadCount.toString()) : '';
        counterElement.style.display = unreadCount > 0 ? 'inline' : 'none';
    }
}

// Update notification dropdown in UI
function updateNotificationDropdown() {
    const dropdownElement = document.querySelector('.dropdown-list');
    if (dropdownElement) {
        loadNotifications();

        // Clear existing notifications
        const dropdownHeader = dropdownElement.querySelector('.dropdown-header');
        if (dropdownHeader) {
            // Remove all except the header
            while (dropdownHeader.nextSibling) {
                dropdownHeader.nextSibling.remove();
            }
        }

        // Add notifications
        if (window.notifications.length > 0) {
            window.notifications.forEach(notification => {
                const notificationElement = createNotificationElement(notification);
                dropdownElement.appendChild(notificationElement);
            });

            // Add "Show All" link
            const showAllLink = document.createElement('a');
            showAllLink.className = 'dropdown-item text-center small text-gray-500';
            showAllLink.href = '#';
            showAllLink.textContent = 'Show All Notifications';
            showAllLink.onclick = function (e) {
                e.preventDefault();
                markAllNotificationsAsRead();
            };
            dropdownElement.appendChild(showAllLink);
        } else {
            // No notifications message
            const noNotificationsElement = document.createElement('div');
            noNotificationsElement.className = 'dropdown-item text-center text-gray-500';
            noNotificationsElement.textContent = 'No notifications';
            dropdownElement.appendChild(noNotificationsElement);
        }
    }
}

// Create notification element for dropdown
function createNotificationElement(notification) {
    const element = document.createElement('a');
    element.className = 'dropdown-item d-flex align-items-center';
    element.href = '#';
    element.onclick = function (e) {
        e.preventDefault();
        markNotificationAsRead(notification.id);
    };

    // Add icon based on notification type
    const iconContainer = document.createElement('div');
    iconContainer.className = 'mr-3';

    const iconCircle = document.createElement('div');
    iconCircle.className = 'icon-circle';

    const icon = document.createElement('i');
    icon.className = 'fas text-white';

    switch (notification.type) {
        case NOTIFICATION_TYPES.TRANSACTION_SUBMITTED:
            iconCircle.classList.add('bg-warning');
            icon.classList.add('fa-paper-plane');
            break;
        case NOTIFICATION_TYPES.TRANSACTION_COMPLETED:
            iconCircle.classList.add('bg-success');
            icon.classList.add('fa-check-circle');
            break;
        case NOTIFICATION_TYPES.TRANSACTION_CANCELED:
            iconCircle.classList.add('bg-danger');
            icon.classList.add('fa-times-circle');
            break;
        case NOTIFICATION_TYPES.TRANSACTION_FAILED:
            iconCircle.classList.add('bg-danger');
            icon.classList.add('fa-exclamation-triangle');
            break;
        case NOTIFICATION_TYPES.PROFILE_UPDATED:
            iconCircle.classList.add('bg-info');
            icon.classList.add('fa-user-edit');
            break;
        case NOTIFICATION_TYPES.WALLET_UPDATED:
            iconCircle.classList.add('bg-success');
            icon.classList.add('fa-wallet');
            break;
        case NOTIFICATION_TYPES.COMPLAINT_SENT:
            iconCircle.classList.add('bg-warning');
            icon.classList.add('fa-ticket-alt');
            break;
        case NOTIFICATION_TYPES.COMPLAINT_TREATED:
            iconCircle.classList.add('bg-success');
            icon.classList.add('fa-check');
            break;
        default:
            iconCircle.classList.add('bg-primary');
            icon.classList.add('fa-bell');
    }

    iconCircle.appendChild(icon);
    iconContainer.appendChild(iconCircle);
    element.appendChild(iconContainer);

    // Add content
    const contentContainer = document.createElement('div');

    const titleElement = document.createElement('div');
    titleElement.className = 'font-weight-bold';
    titleElement.textContent = notification.title;

    const messageElement = document.createElement('div');
    messageElement.className = 'small text-gray-500';
    messageElement.textContent = formatDate(notification.timestamp) + ' - ' + notification.message;

    contentContainer.appendChild(titleElement);
    contentContainer.appendChild(messageElement);
    element.appendChild(contentContainer);

    // Add visual indication for unread notifications
    if (!notification.read) {
        element.classList.add('unread-notification');
        element.style.backgroundColor = '#f8f9fc';
    }

    return element;
}

// Format date for display
function formatDate(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffDays = Math.floor(diffMs / 86400000);
    const diffHrs = Math.floor((diffMs % 86400000) / 3600000);
    const diffMins = Math.floor(((diffMs % 86400000) % 3600000) / 60000);

    if (diffDays > 0) {
        return diffDays + 'd ago';
    } else if (diffHrs > 0) {
        return diffHrs + 'h ago';
    } else if (diffMins > 0) {
        return diffMins + 'm ago';
    } else {
        return 'Just now';
    }
}

// Refresh notifications (in a real app, this would fetch from server)
function refreshNotifications() {
    console.log('Refreshing notifications');
    loadNotifications();
    updateNotificationCounter();
    updateNotificationDropdown();
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize on dashboard pages
    if (window.location.pathname.includes('/dashboards/')) {
        initNotifications();
    }
});

// Export functions for use in other modules
window.BTPayProNotifications = {
    NOTIFICATION_TYPES,
    addNotification,
    markNotificationAsRead,
    markAllNotificationsAsRead,
    getUnreadNotificationsCount
};