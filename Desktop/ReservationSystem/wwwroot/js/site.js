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
        // Şifre tekrar kontrolü kaldırıldı
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
    // Sadece rezervasyon ekranında çalışsın
    if (window.location.pathname.toLowerCase().includes('/reservations/create')) {
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
                errorMsg += "Başlangıç tarih ve saati seçilmelidir\n";
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
    }
});
document.addEventListener('DOMContentLoaded', function () {
    var roomTypeSelect = document.getElementById('roomType');
    var roomSelect = document.getElementById('roomSelect');
    var eventName = document.getElementById('eventName');
    var description = document.getElementById('description');
    var startTime = document.getElementById('startTime');
    var endTime = document.getElementById('endTime');

    function updateRoomOptions() {
        var selectedType = roomTypeSelect.value;
        for (var i = 0; i < roomSelect.options.length; i++) {
            var option = roomSelect.options[i];
            if (!option.value) continue;
            if (option.getAttribute('data-roomtype') === selectedType) {
                option.style.display = '';
            } else {
                option.style.display = 'none';
            }
        }
        roomSelect.value = '';
        disableInputs();
    }

    function disableInputs() {
        if (eventName) eventName.disabled = true;
        if (description) description.disabled = true;
        if (startTime) startTime.disabled = true;
        if (endTime) endTime.disabled = true;
    }

    function enableInputs() {
        if (eventName) eventName.disabled = false;
        if (description) description.disabled = false;
        if (startTime) startTime.disabled = false;
        if (endTime) endTime.disabled = false;
    }

    if (roomTypeSelect && roomSelect) {
        roomTypeSelect.addEventListener('change', function () {
            updateRoomOptions();
        });

        roomSelect.addEventListener('change', function () {
            if (roomTypeSelect.value && roomSelect.value) {
                enableInputs();
            } else {
                disableInputs();
            }
        });

        disableInputs();
    }
});

$(document).ready(function() {
    $('#showReservationForm').click(function() {
        var isAuthenticated = 'True' === 'True';
        if (!isAuthenticated) {
            alert('Rezervasyon yapmak için önce giriş yapınız.');
            return;
        }
        if ($('#reservationModal').is(':visible')) return;
        $.get('/Reservations/Create', function(data) {
            var formDiv = $(data).find('.row').parent().html();
            var panelHtml = '<div style="text-align:center;margin-bottom:12px;font-size:1.2rem;font-weight:600;">Rezervasyon Yap</div>' + formDiv;
            $('#reservationModalBody').html(panelHtml);
            $('#reservationModalBody').css({'display':'flex','flexDirection':'column','alignItems':'center','justifyContent':'center','width':'100%'});
            flatpickr("#StartTime", {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                time_24hr: true
            });
            flatpickr("#EndTime", {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                time_24hr: true
            });
            $('#reservationForm').css({ 'margin':'0 auto', 'width':'100%', 'max-width':'600px', 'display':'flex', 'flexDirection':'column', 'gap':'10px', 'fontSize':'1rem', 'fontFamily':'Segoe UI, Arial, sans-serif', 'alignItems':'center', 'justifyContent':'center' });
            $('#reservationForm .mb-3').css({'margin-bottom':'4px'});
            $('#reservationForm label').css({'fontWeight':'500','marginBottom':'2px','fontSize':'0.95rem'});
            $('#reservationForm input, #reservationForm select, #reservationForm textarea, #reservationForm button, #reservationForm a.btn-secondary').css({'width':'100%','minWidth':'400px','maxWidth':'580px'});
            $('#reservationForm input, #reservationForm select').css({'fontSize':'0.95rem','padding':'3px 8px','borderRadius':'5px','border':'1px solid #bbb','marginBottom':'1px','width':'100%','minWidth':'350px','boxSizing':'border-box','height':'28px','minHeight':'28px'});
            $('#reservationForm textarea').css({'fontSize':'0.95rem','padding':'3px 8px','borderRadius':'5px','border':'1px solid #bbb','marginBottom':'1px','width':'100%','minWidth':'350px','boxSizing':'border-box','height':'38px','minHeight':'38px','resize':'vertical'});
            $('#reservationForm button[type=submit]').addClass('w-100 btn btn-success').css({'marginTop':'6px','fontWeight':'600','fontSize':'0.95rem','padding':'7px 0','display':'block'});
            $('#reservationForm a.btn-secondary').addClass('w-100 btn btn-secondary').css({'marginTop':'4px','fontWeight':'600','fontSize':'0.95rem','padding':'7px 0','display':'block'});
            // --- ODA TÜRÜ SCRIPT ---
            var roomTypeSelect = document.getElementById('roomType');
            var roomSelect = document.getElementById('roomSelect');
            var eventName = document.getElementById('eventName');
            var description = document.getElementById('description');
            var startTime = document.getElementById('StartTime');
            var endTime = document.getElementById('EndTime');
            function disableInputs() {
                if (eventName) eventName.disabled = true;
                if (description) description.disabled = true;
                if (startTime) startTime.disabled = true;
                if (endTime) endTime.disabled = true;
            }
            function enableInputs() {
                if (eventName) eventName.disabled = false;
                if (description) description.disabled = false;
                if (startTime) startTime.disabled = false;
                if (endTime) endTime.disabled = false;
            }
            if (roomTypeSelect && roomSelect) {
                roomSelect.disabled = true;
                disableInputs();
                roomTypeSelect.addEventListener('change', function() {
                    if (roomTypeSelect.value) {
                        roomSelect.disabled = false;
                        for (var i = 0; i < roomSelect.options.length; i++) {
                            var option = roomSelect.options[i];
                            if (!option.value) continue;
                            var roomTypes = option.getAttribute('data-roomtype');
                            var show = false;
                            if (roomTypes) {
                                var roomTypesArr = roomTypes.split(',');
                                if (roomTypesArr.includes(roomTypeSelect.value)) {
                                    show = true;
                                }
                            }
                            option.style.display = show ? '' : 'none';
                        }
                        roomSelect.value = '';
                        disableInputs();
                    } else {
                        roomSelect.disabled = true;
                        roomSelect.value = '';
                        disableInputs();
                    }
                });
                roomSelect.addEventListener('change', function() {
                    if (roomTypeSelect.value && roomSelect.value) {
                        enableInputs();
                    } else {
                        disableInputs();
                    }
                });
            }
            // --- SON ---
            // --- FORM SUBMIT TOAST ---
            $('#reservationForm').off('submit').on('submit', function(e) {
                e.preventDefault();
                var form = this;
                var data = new FormData(form);
                fetch(form.action, {
                    method: 'POST',
                    body: data
                })
                .then(r => r.json())
                .then(res => {
                    if (res.success) {
                        showToast(res.message, 'success');
                        setTimeout(function() {
                            window.location.href = '/MeetingRooms/Index';
                        }, 1200);
                    } else {
                        showToast(res.message, 'error');
                    }
                });
            });
            // --- SON ---
        });
        $('#reservationModal').fadeIn();
    });
    $('#reservationModal').on('click', function(e) {
        if (e.target === this) {
            $(this).fadeOut();
        }
    });
});
