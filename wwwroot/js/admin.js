// Скрывать/показывать каталоги при клике
const buttons = document.querySelectorAll('.catalog-toggle-btn');
buttons.forEach(button => {
    button.addEventListener('click', () => {
        const catalogs = button.nextElementSibling;
        catalogs.style.display = catalogs.style.display === 'none' ? 'flex' : 'none';
    });
});




