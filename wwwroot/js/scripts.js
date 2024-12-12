// Для плавного скроллинга на странице
document.addEventListener('DOMContentLoaded', function () {
    const scrollToTopButtons = document.querySelectorAll('.scroll-to-top');

    scrollToTopButtons.forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const scrollToTopButton = document.getElementById("scrollToTop");
    // Показываем кнопку только если есть прокрутка
    function toggleButtonVisibility() {
        const hasScroll = document.body.scrollHeight > window.innerHeight; // Проверяем, длиннее ли контент экрана
        if (window.scrollY > 200 && hasScroll) {
            scrollToTopButton.classList.add("show");
        } else {
            scrollToTopButton.classList.remove("show");
        }
    }
    // Обрабатываем события прокрутки и изменения размеров окна
    window.addEventListener("scroll", toggleButtonVisibility);
    window.addEventListener("resize", toggleButtonVisibility);
});







