document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".sidebar");
    const main = document.getElementById('main');
    const toggler = document.querySelector(".navbar-toggler");
    const icon = toggler.querySelector("i");

   
    if (localStorage.getItem('sidebarCollapsed') === 'true') {
        sidebar.classList.add('collapsed');
        main.classList.add('expanded');
    }

    toggler.addEventListener("click", () => {
        sidebar.classList.toggle("collapsed");
        main.classList.toggle("expanded");

   
        icon.classList.toggle("bi-list");
        icon.classList.toggle("bi-x");

    
        localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
    });
});