function showToast(message, type) {
    var toast = document.getElementById('toast');
    var msg = document.getElementById('toast-message');
    var close = document.getElementById('toast-close');
    toast.style.background = type === 'success' ? '#28a745' : '#dc3545';
    msg.textContent = message;
    toast.style.display = 'flex';
    var timeout = setTimeout(function() {
        toast.style.display = 'none';
    }, 4000);
    close.onclick = function() {
        toast.style.display = 'none';
        clearTimeout(timeout);
    };
}

document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById('darkModeToggle');
    if (!btn) return;
    btn.addEventListener('click', function () {
        document.body.classList.toggle('dark-mode');
        if (document.body.classList.contains('dark-mode')) {
            localStorage.setItem('theme', 'dark');
        } else {
            localStorage.setItem('theme', 'light');
        }
    });
    if (localStorage.getItem('theme') === 'dark') {
        document.body.classList.add('dark-mode');
    }
});
