(function () {
    const header = document.querySelector('.header');
    const hero = document.querySelector('.hero');
    if (!header || !hero) return;

    function updateHeader() {
        const switchPoint = hero.offsetHeight - header.offsetHeight;
        const scrolled = window.scrollY > switchPoint;
        header.classList.toggle('header--type-dark', scrolled);
    }

    window.addEventListener('scroll', updateHeader, { passive: true });
    window.addEventListener('resize', updateHeader);
    updateHeader();
})();
