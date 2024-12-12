// ��������������� ��������
function toggleCard(button) {
    const card = button.closest('.card-inner');
    card.classList.toggle('flipped');
}

function deleteCatalog(catalogId) {
    if (confirm("Are you sure you want to delete this catalog?")) {
        fetch(`/Catalog/DeleteConfirmed?id=${catalogId}`, { method: 'POST' })
            .then(response => {
                if (response.ok) {

                    // �������� �����������
                    const notification = document.getElementById('deleteNotification');
                    notification.style.display = 'block';

                    // ������� ����������� ����� 5 ������ � ��������� ��������
                    setTimeout(() => {
                        notification.style.display = 'none';
                        location.reload();
                    }, 5000);
                } else if (response.status === 403) {
                    // ������ �������
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

// �������� ���������� ���� ��������
function openCreateCatalogModal() {
    const modal = new bootstrap.Modal(document.getElementById('createCatalogModal'));
    modal.show();
}

// ���������� �������� ����� ��� ��������
document.getElementById('createCatalogForm').onsubmit = function (e) {
    e.preventDefault();
    const formData = new FormData(e.target);

    fetch('/Catalog/Create', {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            if (response.ok) {
                // ������ ��������� ����
                const modal = bootstrap.Modal.getInstance(document.getElementById('createCatalogModal'));
                modal.hide();

                // �������� �����������
                const notification = document.getElementById('successNotification');
                notification.style.display = 'block';

                // ������ ����������� ����� 5 ������
                setTimeout(() => {
                    notification.style.display = 'none';

                    // �������� �������� ����� ����, ��� ����������� ��������
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

// �������� ���������� ���� ��������������
function openEditCatalogModal(id, name, description, imageUrl) {

    if (!id || !name || !description) {
        console.error("Missing catalog data:", { id, name, description });
        alert("Failed to open edit modal. Catalog data is incomplete.");
        return;
    }
    document.getElementById('editCatalogId').value = id;
    document.getElementById('editCatalogName').value = name;
    document.getElementById('editCatalogDescription').value = description;
    document.getElementById('editCatalogImage').value = ''; // ������� ���� ��� ������ �����������

    const modal = new bootstrap.Modal(document.getElementById('editCatalogModal'));
    modal.show();
}

// ���������� �������� ����� ��� ��������������
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
    // �������� �������, ������������ ���������� ������
    const likesCountElement = document.getElementById(`likes-count-${catalogId}`);
    const currentLikes = parseInt(likesCountElement.textContent, 10); // ������� ���������� ������

    // ����������� ���������� ������ �� �������
    likesCountElement.textContent = currentLikes + 1;

    // ���������� ������ �� ������
    fetch(`/Catalog/Like/${catalogId}`, { method: 'POST' })
        .then(response => {
            if (response.ok) {
                return response.json(); // �������� ����������� �������� �� �������
            } else {
                throw new Error('Failed to like the catalog.');
            }
        })
        .then(data => {
            // ��������� ���������� ������ �� ��������� ������ �� �������
            likesCountElement.textContent = data.Likes;
        })
        .catch(error => {
            console.error('Error liking catalog:', error);

            // ���� ������ ���������� �������, ���������� ����� �� �������
            likesCountElement.textContent = currentLikes;
            alert('An error occurred while liking the catalog.');
        });
}











