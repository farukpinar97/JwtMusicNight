/*
 * MP3 çalar köprüsü: .btn-play tıklamasında /Music/Stream/{id} istenir.
 * 403 -> paket uyarısı, 200 -> kaynak yazılır ve tema çalar.
 */
(function () {
    'use strict';

    var cache = {};          // songId -> blob URL (aynı şarkı tekrar indirilmesin)
    var currentId = null;    // şu an çalan / son başlatılan şarkı
    var loading = {};        // aynı anda iki kez istek atılmasın

    window.addEventListener('click', function (e) {
        var btn = e.target.closest && e.target.closest('.btn-play');
        if (!btn) return;

        var item = btn.closest('[data-id]');
        if (!item) return;

        var id = item.getAttribute('data-id');

        // Kaynak hazırsa tıklamayı temaya bırak (çal / duraklat)
        if (item.getAttribute('data-source')) { currentId = id; return; }

        // Henüz hazır değil: temanın handler'ına ulaşmasını engelle
        e.preventDefault();
        e.stopPropagation();

        if (cache[id]) {
            document.querySelectorAll('[data-id="' + id + '"]').forEach(function (el) {
                el.setAttribute('data-source', cache[id]);
            });
            currentId = id;
            btn.click();
            return;
        }

        if (loading[id]) return;
        loading[id] = true;
        btn.classList.add('loading');

        fetch('/Music/Stream/' + id, { credentials: 'same-origin' })
            .then(function (res) {
                // Cookie süresi dolmuşsa MVC login sayfasına yönlendirir
                if (res.redirected || res.status === 401) {
                    window.location.href = '/Login/Index';
                    return null;
                }
                if (res.status === 403) {
                    return res.json().catch(function () { return {}; }).then(function (data) {
                        showUpgradeModal(data);
                        return null;
                    });
                }
                if (!res.ok) {
                    return res.json().catch(function () { return {}; }).then(function (data) {
                        showErrorModal(data.message || 'Şarkı çalınamadı.');
                        return null;
                    });
                }
                return res.blob();
            })
            .then(function (blob) {
                if (!blob) return;
                var url = URL.createObjectURL(blob);
                cache[id] = url;
                // Sayfadaki aynı şarkıya ait tüm kartlara kaynağı yaz
                document.querySelectorAll('[data-id="' + id + '"]').forEach(function (el) {
                    el.setAttribute('data-source', url);
                });
                currentId = id;
                btn.click();   // tekrar tetikle -> bu sefer tema çalar
            })
            .catch(function () {
                showErrorModal('Sunucuya ulaşılamadı.');
            })
            .finally(function () {
                loading[id] = false;
                btn.classList.remove('loading');
            });
    }, true); // capture: tema handler'ından önce çalışır

    // ---- Çalan şarkıyı sayfada işaretle (kart / satır / hero) ----
    function markPlaying(playing) {
        document.querySelectorAll('[data-id].is-playing').forEach(function (el) { el.classList.remove('is-playing'); });
        if (playing && currentId) {
            document.querySelectorAll('[data-id="' + currentId + '"]').forEach(function (el) { el.classList.add('is-playing'); });
        }
    }
    // medya olayları kabarcıklanmaz; capture ile yakalıyoruz
    document.addEventListener('play', function (e) { if (e.target.tagName === 'AUDIO') markPlaying(true); }, true);
    document.addEventListener('pause', function (e) { if (e.target.tagName === 'AUDIO') markPlaying(false); }, true);
    document.addEventListener('ended', function (e) { if (e.target.tagName === 'AUDIO') markPlaying(false); }, true);
    // sayfa ajax ile değişince (pjax) yeni sayfadaki aynı şarkıyı tekrar işaretle
    if (window.jQuery) {
        window.jQuery(document).on('pjaxEnd', function () {
            var audio = document.querySelector('#plyrist audio');
            markPlaying(audio && !audio.paused);
            // önceden indirilmiş şarkıların kaynağını yeni kartlara da yaz
            Object.keys(cache).forEach(function (sid) {
                document.querySelectorAll('[data-id="' + sid + '"]').forEach(function (el) { el.setAttribute('data-source', cache[sid]); });
            });
        });
    }

    function showUpgradeModal(data) {
        var modal = $('#upgradeModal');
        modal.find('.js-user-package').text(data.userPackage || '-');
        modal.find('.js-required-package').text(data.requiredPackage || '-');
        if (data.message) modal.find('.js-message').text(data.message);
        modal.modal('show');
    }

    function showErrorModal(message) {
        var modal = $('#errorModal');
        modal.find('.js-message').text(message);
        modal.modal('show');
    }
})();
