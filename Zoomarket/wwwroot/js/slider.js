document.addEventListener("DOMContentLoaded", function () {
    // Проверяем, есть ли слайдер на странице
    const slider = document.querySelector('.hero-slider');
    if (!slider) return;

    let currentSlide = 0;
    const slides = document.querySelectorAll('.slide');
    const dots = document.querySelectorAll('.dot');

    // Функция для показа конкретного слайда
    window.showSlide = function (index) {
        slides.forEach(s => s.classList.remove('active'));
        dots.forEach(d => d.classList.remove('active'));

        slides[index].classList.add('active');
        dots[index].classList.add('active');
        currentSlide = index;
    }

    // Следующий слайд
    window.nextSlide = function () {
        let next = (currentSlide + 1) % slides.length;
        window.showSlide(next);
    }

    // Предыдущий слайд
    window.prevSlide = function () {
        let prev = (currentSlide - 1 + slides.length) % slides.length;
        window.showSlide(prev);
    }

    // Переход по клику на точку
    window.goToSlide = function (index) {
        window.showSlide(index);
    }

    // Автоматическое переключение каждые 5 секунд
    setInterval(window.nextSlide, 5000);
});