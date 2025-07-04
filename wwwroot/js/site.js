// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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
