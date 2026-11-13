document.addEventListener("DOMContentLoaded", function () {
 
    const slider = document.querySelector('.hero-slider');
    if (!slider) return;

    let currentSlide = 0;
    const slides = document.querySelectorAll('.slide');
    const dots = document.querySelectorAll('.dot');

   
    window.showSlide = function (index) {
        slides.forEach(s => s.classList.remove('active'));
        dots.forEach(d => d.classList.remove('active'));

        slides[index].classList.add('active');
        dots[index].classList.add('active');
        currentSlide = index;
    }

    
    window.nextSlide = function () {
        let next = (currentSlide + 1) % slides.length;
        window.showSlide(next);
    }

    
    window.prevSlide = function () {
        let prev = (currentSlide - 1 + slides.length) % slides.length;
        window.showSlide(prev);
    }

    
    window.goToSlide = function (index) {
        window.showSlide(index);
    }

    setInterval(window.nextSlide, 5000);
});