
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

$(document).ready(function () {
    $("#registerForm").submit(function (e) {
        var email = $("#email").val();
        var password = $("#password").val();
        var confirmPassword = $("#confirmPassword").val();
        var errorMsg = "";

        if (!email) {
            errorMsg += "E-posta boş bırakılamaz\n";
        } else if (!validateEmail(email)) {
            errorMsg += "Geçerli bir e-posta girin\n";
        }
        if (!password) {
            errorMsg += "Şifre boş olamaz\n";
        } else if (password.length < 6) {
            errorMsg += "Şifre en az 6 karakter olmalı\n";
        }
        if (password !== confirmPassword) {
            errorMsg += "Şifreler aynı değil\n";
        }
        if (errorMsg) {
            alert(errorMsg);
            e.preventDefault();
        }
    });
});
function validateEmail(email) {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}
$(document).ready(function () {
    $("#loginForm").submit(function (e) {
        var email = $("#loginEmail").val();
        var password = $("#loginPassword").val();
        var errorMsg = "";

        if (!email) {
            errorMsg += "E-posta boş olamaz\n";
        } else if (!validateEmail(email)) {
            errorMsg += "Geçerli bir e-posta girin\n";
        }
        if (!password) {
            errorMsg += "Şifre boş olamaz\n";
        }
        if (errorMsg) {
            alert(errorMsg);
            e.preventDefault();
        }
    });
});
$(document).ready(function () {
    $("#forgotPasswordForm").submit(function (e) {
        var email = $("#forgotEmail").val();
        var errorMsg = "";

        if (!email) {
            errorMsg += "E-posta boş olamaz\n";
        } else if (!validateEmail(email)) {
            errorMsg += "Geçerli bir e-posta girin\n";
        }
        if (errorMsg) {
            alert(errorMsg);
            e.preventDefault();
        }

    });
});
$(document).ready(function () {
    $("#resetPasswordForm").submit(function (e) {
        var email = $("#resetEmail").val();
        var password = $("#resetPassword").val();
        var confirmPassword = $("resetConfirmPassword").val();
        var errorMsg = "";
        if (!email) {
            errorMsg += "E-posta boş olamaz\n"
        } else if (!validateEmail(email)) {
            errorMsg += "Geçerli bir e-posta giriniz\n";
        }
        if (!password) {
            errorMsg += "Şifre boş olamaz\n";
        } else if (password.length < 6) {
            errorMsg += "Şifre en az 6 karakter olmalı\n";
        }
        if (password !== confirmPassword) {
            errorMsg += "Şifreler farklı\n";
        }
        if (errorMsg) {
            alert(errorMsg);
            e.preventDefault();
        }
    });
});
$(document).ready(function () {
    $("#reservationForm").submit(function (e) {
        var eventName = $("#eventName").val();
        var description = $("#description").val();
        var startTime = $("#StartTime").val();
        var endTime = $("#EndTime").val();
        var errorMsg = "";
        if (!eventName) {
            errorMsg += "Etkinlik adı boş olamaz\n";
        }
        if (!description) {
            errorMsg += "Açıklama boş olamaz\n";
        }
        if (!startTime) {
            errorMsg += "Başlangıç tarih ve saati seçilmelidir\n";;
        }
        if (!endTime) {
            errorMsg += "Bitiş tarihi ve saati seçilmelidir\n";
        }
        if (startTime && endTime && startTime >= endTime) {
            errorMsg += "Bitiş zamanı başlangıç zamanından önce olamaz\n";
        }
        if (errorMsg) {
            alert(errorMsg);
            e.preventDefault();
        }
    });
});