/*
 * Music Night · arayüz davranışları
 * - Header: sayfa kaydırılınca koyulaşır
 * - Ana sayfa slider'ı (otomatik geçiş + noktalar)
 * - Tema sayfayı ajax ile değiştirdiğinde (pjax) her şeyi yeniden kurar
 */
(function () {
    'use strict';

    var header = document.getElementById('header');
    function onScroll() {
        if (header) header.classList.toggle('is-scrolled', window.scrollY > 24);
    }
    window.addEventListener('scroll', onScroll, { passive: true });

    var sliderTimer = null;
    function initSliders() {
        if (sliderTimer) { clearInterval(sliderTimer); sliderTimer = null; }
        document.querySelectorAll('.mn-slider').forEach(function (slider) {
            var slides = slider.querySelectorAll('.mn-slide');
            var dots = slider.querySelectorAll('.mn-dots button');
            if (slides.length < 2) return;
            var index = 0;

            function show(i) {
                index = (i + slides.length) % slides.length;
                slides.forEach(function (s, k) { s.classList.toggle('active', k === index); });
                dots.forEach(function (d, k) { d.classList.toggle('active', k === index); });
            }
            function start() { sliderTimer = setInterval(function () { show(index + 1); }, 7000); }

            dots.forEach(function (d, k) {
                d.addEventListener('click', function () { clearInterval(sliderTimer); show(k); start(); });
            });
            // kullanıcı slider üzerindeyken (ör. Play'e basmak üzereyken) geçiş durur
            slider.addEventListener('mouseenter', function () { clearInterval(sliderTimer); });
            slider.addEventListener('mouseleave', function () { clearInterval(sliderTimer); start(); });
            start();
        });
    }

    // Arama formu: sayfayı yeniden yüklemeden (müzik durmadan) sonuç sayfasını aç
    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form.classList || !form.classList.contains('js-search-form')) return;
        var q = (form.querySelector('input[name=q]').value || '').trim();
        var link = document.getElementById('mnSearchLink');
        if (!link) return;               // link yoksa normal gönderim
        e.preventDefault();
        link.setAttribute('href', '/Music/Search?q=' + encodeURIComponent(q));
        link.click();
    });

    // 403 pop-up'ındaki "Paketleri incele": modalı kapat, sayfayı pjax ile aç
    document.addEventListener('click', function (e) {
        var a = e.target.closest && e.target.closest('.js-modal-link');
        if (!a) return;
        var link = document.getElementById('mnSearchLink');
        if (window.jQuery) window.jQuery('.modal').modal('hide');
        if (!link) return;
        e.preventDefault();
        link.setAttribute('href', a.getAttribute('href'));
        link.click();
    });

    // ---- Yatay satırlar: ‹ › oklar ----
    function updateScrollState(wrap) {
        var row = wrap.querySelector('.mn-scroll');
        var nav = document.querySelector('[data-scroll-nav="' + wrap.id + '"]');
        if (!row) return;
        var max = row.scrollWidth - row.clientWidth;
        var canLeft = row.scrollLeft > 12;
        var canRight = row.scrollLeft < max - 12;
        wrap.classList.toggle('can-left', canLeft);
        wrap.classList.toggle('can-right', canRight);
        if (nav) {
            nav.hidden = max <= 12;                       // satır ekrana sığıyorsa okları gizle
            nav.querySelector('[data-dir="-1"]').disabled = !canLeft;
            nav.querySelector('[data-dir="1"]').disabled = !canRight;
        }
    }
    function initScrollRows() {
        document.querySelectorAll('.mn-scroll-wrap').forEach(function (wrap) {
            var row = wrap.querySelector('.mn-scroll');
            if (!row || wrap.dataset.ready) { if (row) updateScrollState(wrap); return; }
            wrap.dataset.ready = '1';
            row.addEventListener('scroll', function () { updateScrollState(wrap); }, { passive: true });
            updateScrollState(wrap);
        });
    }
    document.addEventListener('click', function (e) {
        var btn = e.target.closest && e.target.closest('.mn-arrow');
        if (!btn) return;
        var nav = btn.closest('[data-scroll-nav]');
        var wrap = nav && document.getElementById(nav.getAttribute('data-scroll-nav'));
        var row = wrap && wrap.querySelector('.mn-scroll');
        if (!row) return;
        var dir = parseInt(btn.getAttribute('data-dir'), 10);
        row.scrollBy({ left: dir * Math.max(200, row.clientWidth * 0.85), behavior: 'smooth' });
    });
    window.addEventListener('resize', function () {
        document.querySelectorAll('.mn-scroll-wrap').forEach(updateScrollState);
    });

    function initPage() {
        onScroll();
        initScrollRows();
        initSliders();
        if (window.feather) window.feather.replace();
    }

    document.addEventListener('DOMContentLoaded', initPage);
    // tema (ajax.js) sayfa değiştirince
    if (window.jQuery) {
        window.jQuery(document).on('pjaxEnd', function () {
            initPage();
            // aktif menü vurgusu
            var path = location.pathname.toLowerCase();
            document.querySelectorAll('.mn-header .navbar-nav .nav-item').forEach(function (li) {
                var a = li.querySelector('a');
                var href = (a && a.getAttribute('href') || '').toLowerCase();
                li.classList.toggle('active', href !== '/' && path.indexOf(href) === 0);
            });
        });
    }
})();
