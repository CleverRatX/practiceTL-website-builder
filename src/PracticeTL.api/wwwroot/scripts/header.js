(function () {
    const header = document.querySelector('.header');
    if (!header) return;

    const sections = [
        { el: document.querySelector('.hero'), mode: 'light' },
        { el: document.querySelector('.advantages'), mode: 'dark' },
        { el: document.querySelector('.team'), mode: 'light' },
        { el: document.querySelector('.platform'), mode: 'hidden' },
    ].filter(s => s.el);

    function topOf(el) {
        return el.getBoundingClientRect().top + window.scrollY;
    }

    function updateHeader() {
        const line = window.scrollY + header.offsetHeight;
        let mode = 'light';
        for (const s of sections) {
            if (topOf(s.el) <= line) mode = s.mode;
        }
        header.classList.toggle('header--type-dark', mode === 'dark');
        header.classList.toggle('header--type-hidden', mode === 'hidden');
    }

    window.addEventListener('scroll', updateHeader, { passive: true });
    window.addEventListener('resize', updateHeader);
    updateHeader();
})();
