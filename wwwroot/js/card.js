// Переворачивание карточек
function toggleCard(button) {
    const card = button.closest('.card-inner');
    card.classList.toggle('flipped');
}

function deleteCatalog(catalogId) {
    if (confirm("Are you sure you want to delete this catalog?")) {
        fetch(`/Catalog/DeleteConfirmed?id=${catalogId}`, { method: 'POST' })
            .then(response => {
                if (response.ok) {

                    // Показать уведомление
                    const notification = document.getElementById('deleteNotification');
                    notification.style.display = 'block';

                    // Убираем уведомление через 5 секунд и обновляем страницу
                    setTimeout(() => {
                        notification.style.display = 'none';
                        location.reload();
                    }, 5000);
                } else if (response.status === 403) {
                    // Ошибка доступа
                    alert("You are not authorized to delete this catalog.");
                } else {
                    alert("Error deleting catalog.");
                }
            })
            .catch(err => {
                console.error(err);
                alert("An unexpected error occurred.");
            });
    }
}

// Открытие модального окна создания
function openCreateCatalogModal() {
    const modal = new bootstrap.Modal(document.getElementById('createCatalogModal'));
    modal.show();
}

// Обработчик отправки формы для создания
document.getElementById('createCatalogForm').onsubmit = function (e) {
    e.preventDefault();
    const formData = new FormData(e.target);

    fetch('/Catalog/Create', {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            if (response.ok) {
                // Скрыть модальное окно
                const modal = bootstrap.Modal.getInstance(document.getElementById('createCatalogModal'));
                modal.hide();

                // Показать уведомление
                const notification = document.getElementById('successNotification');
                notification.style.display = 'block';

                // Скрыть уведомление через 5 секунд
                setTimeout(() => {
                    notification.style.display = 'none';

                    // Обновить страницу после того, как уведомление пропадет
                    location.reload();
                }, 5000);
            } else {
                alert("Error creating catalog.");
            }
        })
        .catch(error => {
            console.error("Error:", error);
            alert("An unexpected error occurred.");
        });
};

// Открытие модального окна редактирования
function openEditCatalogModal(id, name, description, imageUrl) {

    if (!id || !name || !description) {
        console.error("Missing catalog data:", { id, name, description });
        alert("Failed to open edit modal. Catalog data is incomplete.");
        return;
    }
    document.getElementById('editCatalogId').value = id;
    document.getElementById('editCatalogName').value = name;
    document.getElementById('editCatalogDescription').value = description;
    document.getElementById('editCatalogImage').value = ''; // Очищаем поле для нового изображения

    const modal = new bootstrap.Modal(document.getElementById('editCatalogModal'));
    modal.show();
}

// Обработчик отправки формы для редактирования
document.getElementById('editCatalogForm').onsubmit = function (e) {
    e.preventDefault();
    const formData = new FormData(e.target);


    

    fetch('/Catalog/Edit', {
        method: 'POST',
        body: formData,
    })
        .then((res) => {
            if (res.ok) {
                const notification = document.getElementById('editNotification');
                notification.style.display = 'block';

                setTimeout(() => {
                    notification.style.display = 'none';
                    location.reload();
                }, 5000);

                const modal = bootstrap.Modal.getInstance(document.getElementById('editCatalogModal'));
                modal.hide();
            } else {
                alert("Error editing catalog.");
            }
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("An unexpected error occurred.");
        });
};


function likeCatalog(catalogId) {
    // Получаем элемент, отображающий количество лайков
    const likesCountElement = document.getElementById(`likes-count-${catalogId}`);
    const currentLikes = parseInt(likesCountElement.textContent, 10); // Текущее количество лайков

    // Увеличиваем количество лайков на клиенте
    likesCountElement.textContent = currentLikes + 1;

    // Отправляем запрос на сервер
    fetch(`/Catalog/Like/${catalogId}`, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                return response.json(); // Получаем обновленное значение от сервера
            } else {
                throw new Error('Failed to like the catalog.');
            }
        })
        .then(data => {
            // Обновляем количество лайков на основании ответа от сервера
            likesCountElement.textContent = data.Likes;
        })
        .catch(error => {
            console.error('Error liking catalog:', error);

            // Если запрос завершился ошибкой, откатываем лайки на клиенте
            likesCountElement.textContent = currentLikes;
            alert('An error occurred while liking the catalog.');
        });
}











