function toggleMenu() {
    const nav = document.getElementById('mobileNav');
    const burger = document.querySelector('.burger-menu');

    nav.classList.toggle('active');
    burger.classList.toggle('active');
}

document.querySelectorAll('.main-nav a').forEach(link => {
    link.addEventListener('click', () => {
        document.getElementById('mobileNav').classList.remove('active');
        document.querySelector('.burger-menu').classList.remove('active');
    });
});