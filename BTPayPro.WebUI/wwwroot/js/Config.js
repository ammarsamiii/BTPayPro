// BTPayPro Configuration
const AppConfig = {
    // API Base URL - Dynamically determine based on current origin
    // This allows the frontend to work with different ports
    API_BASE_URL: window.location.origin.replace('7086', '5117').replace('5029', '5117') + '/api',

    // Token storage key
    TOKEN_KEY: 'authToken',
    USER_EMAIL_KEY: 'userEmail'
};
