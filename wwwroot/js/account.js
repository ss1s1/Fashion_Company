document.getElementById('registerForm').addEventListener('submit', function (e) {
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;
    if (password !== confirmPassword) {
        e.preventDefault();
        alert('Passwords do not match!');
    }
});


document.getElementById('registerForm').onsubmit = function (e) {
    e.preventDefault();  // Предотвращаем стандартную отправку формы
    const formData = new FormData(e.target);

    fetch('/User/Register', {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            if (response.ok) {
                // Закрываем модальное окно
                const registerModal = bootstrap.Modal.getInstance(document.getElementById('registerModal'));
                registerModal.hide();

                // Показываем уведомление
                const notification = document.getElementById('registerNotification');
                notification.style.display = 'block';

                // Скрываем уведомление через 5 секунд
                setTimeout(() => {
                    notification.style.display = 'none';
                    // Обновить страницу после того, как уведомление пропадет
                    location.reload();
                }, 2000);
            } else {
                return response.text().then(text => {
                    alert(`Registration failed: ${text}`);
                });
            }
            
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An unexpected error occurred.');
        });
};

document.getElementById('loginForm').onsubmit = function (e) {
    e.preventDefault();
    const formData = new FormData(e.target);

    fetch('/User/Login', {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            if (response.ok) {
                // Закрываем модальное окно
                const loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
                loginModal.hide();

                // Показываем уведомление
                const notification = document.getElementById('loginNotification');
                notification.style.display = 'block';

                // Скрываем уведомление через 5 секунд
                setTimeout(() => {
                    notification.style.display = 'none';
                    // Обновить страницу после того, как уведомление пропадет
                    location.reload();
                }, 2000);
            } else {
                // Обрабатываем ошибки
                response.text().then(errorMessage => {
                    alert(errorMessage); // Показываем сообщение об ошибке
                });
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An unexpected error occurred. Please try again later.');
        });
};

function logout() {
    fetch('/User/Logout', { method: 'POST' })
        .then(response => {
            if (response.ok) {
                // Показываем уведомление
                const notification = document.getElementById('logoutNotification');
                notification.style.display = 'block';

                // Скрываем уведомление через 5 секунд
                setTimeout(() => {
                    notification.style.display = 'none';
                    // Обновить страницу после того, как уведомление пропадет
                    location.reload();
                }, 2000);
            } else {
                alert('Error logging out');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An unexpected error occurred.');
        });
}


document.addEventListener('DOMContentLoaded', () => {
    const imageInput = document.getElementById('profileImageInput');
    const profilePreview = document.getElementById('profilePreview');

    if (imageInput && profilePreview) {
        imageInput.addEventListener('change', function (event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    profilePreview.src = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        });
    }
});







