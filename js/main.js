(function($) {
    "use strict";
    var fullHeight = function() {
        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function(){ $('.js-fullheight').css('height', $(window).height()); });
    };
    fullHeight();
    $('#sidebarCollapse').on('click', function () { $('#sidebar').toggleClass('active'); });

    document.addEventListener("DOMContentLoaded", function () {
        var sidebar = document.getElementById("adminSidebar") || document.getElementById("trainerSidebar") || document.getElementById("traineeSidebar");
        if (!sidebar) return;
        var style = document.createElement("style");
        style.innerHTML = "@media(max-width:900px){.main-layout{display:block!important;min-height:auto!important}.sidebar{position:fixed!important;left:0!important;top:0!important;width:280px!important;max-width:86vw!important;height:100vh!important;max-height:100vh!important;z-index:10001!important;overflow-y:auto!important;transform:translateX(-105%)!important;transition:transform .28s ease!important;box-shadow:4px 0 18px rgba(0,0,0,.25)!important}.sidebar.mobile-open{transform:translateX(0)!important}.sidebar.collapsed{width:280px!important;min-width:280px!important;display:block!important;transform:translateX(-105%)!important}.content-area{width:100%!important;min-width:0!important;padding:10px!important}.mobile-sidebar-backdrop{position:fixed;inset:0;background:rgba(0,0,0,.45);z-index:10000;opacity:0;visibility:hidden;transition:opacity .28s ease,visibility .28s ease}.mobile-sidebar-backdrop.show{opacity:1;visibility:visible}}@media(prefers-reduced-motion:reduce){.sidebar,.mobile-sidebar-backdrop{transition:none!important}}";
        document.head.appendChild(style);
        var backdrop = document.createElement("div");
        backdrop.className = "mobile-sidebar-backdrop";
        document.body.appendChild(backdrop);
        function mobile() { return window.matchMedia && window.matchMedia("(max-width:900px)").matches; }
        function closeMenu() { sidebar.classList.add("collapsed"); sidebar.classList.remove("mobile-open"); backdrop.classList.remove("show"); }
        if (mobile()) closeMenu();
        var toggle = document.querySelector(".toggle-btn");
        if (toggle) toggle.addEventListener("click", function () { window.setTimeout(function () { if (!mobile()) return; if (sidebar.classList.contains("collapsed")) { sidebar.classList.remove("mobile-open"); backdrop.classList.remove("show"); } else { sidebar.classList.add("mobile-open"); backdrop.classList.add("show"); } }, 0); });
        backdrop.addEventListener("click", closeMenu);
        document.addEventListener("keydown", function(e) { if (e.key === "Escape" && mobile()) closeMenu(); });
        document.querySelectorAll(".sidebar a").forEach(function(link) { link.addEventListener("click", function() { if (mobile()) closeMenu(); }); });
        window.addEventListener("resize", function() { if (!mobile()) { sidebar.classList.remove("mobile-open"); backdrop.classList.remove("show"); } });
    });
})(jQuery);
