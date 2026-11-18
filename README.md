# 🎧 Music Night – JWT Tabanlı Müzik Platformu

Bu proje, **Murat Yücedağ** eğitmenliğinde ASP.NET Core 8 ile geliştirilmiş, **Web API** ve **MVC arayüzü** olarak iki katmandan oluşan bir müzik dinleme platformudur. Kullanıcılar abonelik paketlerine (Basic, Gold, Premium, Elit) göre şarkılara erişir; yetki kontrolü JWT içindeki paket bilgisiyle API tarafında yapılır. Proje; şarkı, sanatçı ve tür yönetimi, top listeler, dinleme geçmişi, ML.NET ile kişisel öneriler ve kapsamlı bir yönetim paneli içerir. Tüm veriler MSSQL üzerinden dinamik olarak gelir.

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

<img width="1920" height="945" alt="01_Login" src="https://github.com/user-attachments/assets/484bf570-381c-4222-b300-29c4690cba31" />

<img width="1920" height="945" alt="02_Register" src="https://github.com/user-attachments/assets/166c40c4-5607-4bb0-ae41-389c90635280" />

<img width="1920" height="2313" alt="03_MusicList" src="https://github.com/user-attachments/assets/29aeddc3-6696-4ef0-ae9b-82b74cf32cbc" />

<img width="1920" height="2734" alt="04_TopList" src="https://github.com/user-attachments/assets/42247ebd-b0b6-489f-a1b9-288ec0dbf83e" />

<img width="1920" height="1123" alt="05_Genre" src="https://github.com/user-attachments/assets/dc1da8a8-1978-4bde-a26b-b24e944d14c4" />

<img width="1920" height="1123" alt="06_Artist" src="https://github.com/user-attachments/assets/8cf8a397-205f-4354-93f4-6ed3013eda67" />

<img width="1920" height="1299" alt="07_ArtistDetail" src="https://github.com/user-attachments/assets/a6f45c8d-dfdb-4e7a-ad27-88f573ebfb23" />

<img width="1920" height="1702" alt="08_SongDetail" src="https://github.com/user-attachments/assets/3f1986ec-1ce9-4dc4-8472-ebd28d4cc458" />

<img width="1920" height="1781" alt="09_Account" src="https://github.com/user-attachments/assets/361f6d8a-036d-4033-a457-1f760cf8a69d" />

<img width="1920" height="2383" alt="10_Package" src="https://github.com/user-attachments/assets/b12b440f-35ca-4075-8c87-cc9f34cb4dad" />

<img width="1920" height="1230" alt="11_AdminDashboard" src="https://github.com/user-attachments/assets/0c40167f-0d42-4d69-97ff-3f129a197274" />

<img width="1920" height="1206" alt="12_AdminUsers" src="https://github.com/user-attachments/assets/5f2db060-a987-46f2-a54f-60dbc2852d5a" />

<img width="1920" height="945" alt="13_AdminArtist" src="https://github.com/user-attachments/assets/7d018765-9d84-4e2a-ad0f-562d72dbe0ae" />

<img width="1920" height="945" alt="14_AdminArtistAdd" src="https://github.com/user-attachments/assets/82413d41-8867-4eab-9550-b44c18ce910d" />

<img width="1920" height="1813" alt="15_AdminSongs" src="https://github.com/user-attachments/assets/1108c231-fce8-4626-ae43-1d3c12421cf6" />

<img width="1920" height="995" alt="16_AdminSongUpdate" src="https://github.com/user-attachments/assets/cbb435b5-8901-4ebd-821d-9718db73d47f" />

<img width="1920" height="1813" alt="17_AdminSongDelete" src="https://github.com/user-attachments/assets/d1c86af0-6f81-4a8a-92cb-cf518ffe71f2" />

