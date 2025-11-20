document.getElementById("loginForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch("http://localhost:17010/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            const text = await response.text();
            document.getElementById("errorMsg").textContent = text || "Login failed.";
            return;
        }

        const data = await response.json();

        // Save JWT token for later authenticated API calls
        localStorage.setItem("token", data.token);

        // Redirect user to role-based dashboard
        window.location.href = data.redirectUrl;

    } catch (err) {
        console.error("Error:", err);
        document.getElementById("errorMsg").textContent = "Something went wrong. Try again.";
    }
});
