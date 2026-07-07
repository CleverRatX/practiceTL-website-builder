(function () {
    const header = document.querySelector('.header');
    if (!header) return;

    const sections = [
        { el: document.querySelector('.hero'),       mode: 'light' },
        { el: document.querySelector('.advantages'), mode: 'dark' },
        { el: document.querySelector('.team'),       mode: 'light' },
        { el: document.querySelector('.platform'),   mode: 'hidden' },
        { el: document.querySelector('.directions'), mode: 'dark' },
        { el: document.querySelector('.slogan'),     mode: 'dark' },
        { el: document.querySelector('.vacancies'),  mode: 'dark' },
        { el: document.querySelector('.gallery'),    mode: 'light' },
        { el: document.querySelector('.work'),       mode: 'dark' },
        { el: document.querySelector('.bonus'),      mode: 'dark' },
    ].filter(s => s.el);

    function updateHeader() {
        const line = header.offsetHeight;
        let mode = 'dark';
        for (const s of sections) {
            if (s.el.getBoundingClientRect().top <= line) mode = s.mode;
        }
        header.classList.toggle('header--type-dark', mode === 'dark');
        header.classList.toggle('header--type-hidden', mode === 'hidden');
    }

    window.addEventListener('scroll', updateHeader, { passive: true });
    window.addEventListener('resize', updateHeader);
    window.addEventListener('load', updateHeader);
    updateHeader();
})();
