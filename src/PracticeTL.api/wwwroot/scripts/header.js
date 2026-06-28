(function () {
    const header = document.querySelector('.header');
    const hero = document.querySelector('.hero');
    const advantages = document.querySelector('.advantages');

    if (!header || !hero) return;

    function updateHeader() {
        const switchPoint = hero.offsetHeight - header.offsetHeight;
        const switchPoint1 = switchPoint + advantages.offsetHeight;

        const scrolled = (window.scrollY > switchPoint) && (window.scrollY < switchPoint1);
        
        header.classList.toggle('header--type-dark', scrolled);
    }

    window.addEventListener('scroll', updateHeader, { passive: true });
    window.addEventListener('resize', updateHeader);
    updateHeader();
})();
