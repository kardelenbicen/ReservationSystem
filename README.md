🏨 ReservationSystem - Kapsamlı Rezervasyon Yönetim Sistemi

📌 Proje Hakkında

Bu proje, bir Toplantı Odası Rezervasyon Sistemi'dir. ASP.NET Core MVC altyapısı kullanılarak geliştirilmiştir ve kullanıcıların toplantı odalarını kolayca rezerve etmelerini sağlar. Yönetici paneli, rezervasyon onay/reddet işlemleri ve toplantı odası yönetimi gibi yeteneklerle donatılmıştır.

🚀 Özellikler

Kullanıcılar için rezervasyon oluşturma, görme ve iptal etme

Yöneticiler için rezervasyon onaylama veya reddetme

Takvim görünümü ile toplantı odalarının doluluk durumunu görüntüleme

Toplantı odası ekleme, silme ve düzenleme

Temel ödeme ekranı (ödeme simülasyonu)

Gelecek 24 saat içindeki rezervasyonlar için bildirim kısmı

Bootstrap ve özelleştirilmiş CSS ile modern arayüz tasarımı

🛠 Kullanılan Teknolojiler

ASP.NET Core MVC (.NET 9.0)

Entity Framework Core (veri erişim ve migrations)

SQL Server veya local veritabanı

Bootstrap (arayüz tasarımı)

jQuery (dinamik işlemler ve validasyon)

📂 Proje Yapısı

Controllers/ – Kullanıcı ve yönetici işlemlerini yöneten C# Controller sınıfları

Models/ – Veritabanı modelleri (Reservation, MeetingRoom vb.)

Views/ – Razor sayfaları (kullanıcı ve yönetici arayüzü)

Services/ – E-posta gönderimi gibi servis sınıfları

wwwroot/ – CSS, JS ve resimler

Migrations/ – Entity Framework Core migrations dosyaları

⚙️ Kurulum ve Çalıştırma

Gerekli: Visual Studio 2022+ ve .NET 9.0 SDK

Bu repoyu klonlayın:

git clone https://github.com/kardelenbicen/ReservationSystem.git

Çözümü Visual Studio ile açın.

appsettings.json dosyasında veritabanı bağlantı ayarlarını yapılandırın.

Veritabanını oluşturmak için aşağıdaki komutu çalıştırın:

dotnet ef database update

Çözümü Debug veya Release modunda çalıştırın.

🖼 Ekran Görüntüleri 

Rezervasyon listesi sayfası
<img width="1350" height="832" alt="image" src="https://github.com/user-attachments/assets/9cd38b10-d7c9-434b-ad05-7382e539ffe6" />

Takvim görünümü
<img width="1890" height="847" alt="image" src="https://github.com/user-attachments/assets/402e62fe-d6d0-4250-a0b3-e96907f19aae" />

Toplantı odası yönetim sayfası
<img width="1919" height="875" alt="image" src="https://github.com/user-attachments/assets/b1cf357f-7519-4573-93d4-25e68b7dc284" />

Sepet Görünümü 
<img width="1377" height="472" alt="image" src="https://github.com/user-attachments/assets/e014573e-032d-476c-bcf8-de080cb3b783" />

Ödeme Ekranı 
<img width="732" height="781" alt="image" src="https://github.com/user-attachments/assets/f3491652-25f1-4048-8ed5-4dfced7b3ecf" />

Bekleyen Rezervasyonlar
<img width="1918" height="870" alt="image" src="https://github.com/user-attachments/assets/6d227a60-b34b-4b3a-88dd-7e6da668b5ad" />

Bildirimler
<img width="1901" height="851" alt="image" src="https://github.com/user-attachments/assets/bd6ed3f6-91b5-4871-b574-c2761082622e" />

👨‍💻 Geliştirici
Kardelen Biçen
- GitHub: @kardelenbicen
- Email: kardelen.bicen.tr@gmail.com

- Email ile iletişime geçin
- Dokümantasyonu kontrol edin
