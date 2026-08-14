document.addEventListener('DOMContentLoaded', function () {
    var hamburgerBtn = document.getElementById('hamburgerBtn');
    var sideMenu = document.getElementById('sideMenu');
    var overlay = document.getElementById('sideMenuOverlay');

    if (!hamburgerBtn || !sideMenu || !overlay) {
        return;
    }

    function closeMenu() {
        sideMenu.classList.remove('open');
        overlay.classList.remove('open');
        hamburgerBtn.setAttribute('aria-expanded', 'false');
    }

    function toggleMenu() {
        var isOpen = sideMenu.classList.toggle('open');
        overlay.classList.toggle('open', isOpen);
        hamburgerBtn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    }

    hamburgerBtn.addEventListener('click', toggleMenu);
    overlay.addEventListener('click', closeMenu);
});
