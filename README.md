# 🎧 Music Night – JWT Tabanlı Müzik Platformu

Bu proje, ASP.NET Core 8 ile geliştirilmiş, **Web API** ve **MVC arayüzü** olarak iki katmandan oluşan bir müzik dinleme platformudur. Kullanıcılar abonelik paketlerine (Basic, Gold, Premium, Elit) göre şarkılara erişir; yetki kontrolü JWT içindeki paket bilgisiyle API tarafında yapılır. Proje; şarkı, sanatçı ve tür yönetimi, top listeler, dinleme geçmişi, ML.NET ile kişisel öneriler ve kapsamlı bir yönetim paneli içerir. Tüm veriler MSSQL üzerinden dinamik olarak gelir.

## 📋 Proje Hakkında

Uygulama iki projeden oluşur: kimlik doğrulama, paket kontrolü ve MP3 akışını yöneten **JwtMusicNight.WebApi** ve kullanıcının müzik dinlediği, yöneticinin içerikleri yönettiği **JwtMusicNight.WebUI**.

Arayüz, Bepop müzik teması üzerine kurulmuştur: koyu tema, gerçek sanatçı ve albüm görselleri, alt kısımda sabit MP3 çalar ve sayfa değişse bile müziği durdurmayan ajax (pjax) geçişleri. Tüm sayfalar masaüstünde ve mobilde düzgün çalışır.

## 🎯 Öne Çıkan Özellikler

### 🎵 Kullanıcı Arayüzü

**Ana Sayfa** — En çok dinlenen şarkılardan oluşan otomatik geçişli slider, son dinlenenler, tüm şarkılar ve ML.NET ile "Bunları da sevebilirsiniz" önerileri

**Şarkı Detayı** — Sanatçı görseliyle geniş hero alanı, şarkı bilgileri ve aynı şarkıyı dinleyenlerin dinlediği diğer şarkılardan oluşan "Önerilenler" bölümü

**Paket Kontrolü** — Kullanıcının paketi şarkıyı desteklemiyorsa API 403 döner ve arayüzde *"Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin."* uyarısı pop-up olarak gösterilir

**MP3 Çalar** — Şarkılar API üzerinden akış (stream) olarak çalınır; ileri/geri sarma desteklenir, çalan şarkı kart ve satırlarda ekolayzer animasyonuyla işaretlenir

**Sanatçılar ve Türler** — Sanatçı listesi ve detay sayfası (sanatçının tüm şarkıları), türlere göre şarkı listeleri

**Top Listeler** — Türe göre filtreleme ve popülerlik / yenilik sıralaması

**Arama** — Şarkı adı, sanatçı veya türe göre Türkçe karakter ve büyük/küçük harf duyarsız arama

**Hesabım** — Mevcut paket, paket karşılaştırması, kilitli şarkılar ve dinleme geçmişi

**Arayüz Detayları** — Oklarla kaydırılan yatay şarkı satırları, kullanıcıya özel sabit avatar, tema ile uyumlu 403 / 404 / 500 hata sayfaları

### 🗂️ Yönetim Paneli

**Dashboard** — Toplam kullanıcı, şarkı, sanatçı ve dinlenme kartları; son 7 günün dinlenme grafiği, paket dağılımı ve en çok dinlenen şarkı / sanatçı listeleri

**Kullanıcılar** — Arama ve paket filtresiyle kullanıcı listesi, kullanıcının paketini değiştirme

**Şarkılar** — CRUD işlemleri, gerekli paket seçimi, sürükle-bırak MP3 yükleme, MP3'ü eksik şarkıları ve paketleri hızlı filtreleme

**Sanatçılar ve Türler** — CRUD işlemleri, doğrulanmış sanatçı işareti, listeden görsel seçme ya da bilgisayardan görsel yükleme

**Kullanılabilirlik** — Silme işlemleri için onay penceresi, işlem sonrası bildirimler, kaydetme sırasında buton yükleniyor durumu

### 🤖 Öneri Sistemi

**ML.NET** — Dinleme geçmişinden One-Class Matrix Factorization ile kişisel öneriler üretilir; model, dinleme geçmişi değiştikçe yeniden eğitilir

**Yedek Senaryo** — Yeterli veri olmayan yeni kullanıcılara dinlemedikleri en popüler şarkılar önerilir

**Birlikte Dinlenenler** — Şarkı detayında, aynı şarkıyı dinleyen kullanıcıların dinlediği diğer şarkılar listelenir

### 🔐 Güvenlik ve Yetkilendirme

**JWT Kimlik Doğrulama** — Giriş yapan kullanıcıya kullanıcı bilgileri, paket ve paket seviyesi claim'lerini içeren token üretilir

**Kayıt** — Yeni kullanıcılar varsayılan olarak Basic paketle kaydolur ve kendilerine rol atanmaz

**Rol Bazlı Erişim** — Yönetim paneli hem arayüzde hem API'de yalnızca `Admin` rolüne açıktır

**Oturum Yönetimi** — MVC tarafında token cookie oturumunda saklanır ve her API isteğine otomatik eklenir (DelegatingHandler); token süresi dolduğunda kullanıcı otomatik olarak çıkışa yönlendirilir

## 🛠️ Kullanılan Teknolojiler

| Teknoloji | Açıklama |
|---|---|
| ASP.NET Core 8 Web API | Kimlik doğrulama, paket kontrolü ve MP3 akışı |
| ASP.NET Core 8 MVC | Kullanıcı arayüzü ve Admin area |
| Entity Framework Core | Code First yaklaşımı ve migration'lar |
| MSSQL Server | İlişkisel veritabanı |
| ASP.NET Core Identity | Kullanıcı ve rol yönetimi |
| JWT Bearer | Token tabanlı kimlik doğrulama |
| AutoMapper | Entity ↔ DTO dönüşümleri |
| ML.NET | Matrix Factorization ile şarkı önerileri |
| Swagger | API dokümantasyonu ve test |
| Bepop Teması + Bootstrap + jQuery | Arayüz, pjax geçişleri ve MP3 çalar (Plyr) |

## 📷 Ekran Görüntüleri
