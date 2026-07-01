const errorBox = document.getElementById('error');

async function login() {
    errorBox.style.display = 'none';
    const body = {
        username: document.getElementById('username').value,
        password: document.getElementById('password').value
    };
    const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (res.ok) {
        window.location.href = '/admin.html';
    } else {
        errorBox.textContent = 'Неверный логин или пароль';
        errorBox.style.display = 'block';
    }
}

document.getElementById('password').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') login();
});