# SecilStoreConfig

Uygulama ayarlarını (configuration) bir veritabanında tutan ve bu ayarları web arayüzünden yönetmeyi sağlayan bir projedir. Ayarlar kod içine yazılmak yerine veritabanından okunur, böylece kodu değiştirmeden ayarları güncelleyebiliriz.

Her uygulama kendi adıyla (`ApplicationName`) sadece kendi ayarlarını okur. Ayarlardan sadece "aktif" (`IsActive`) olanlar getirilir.

## Kullanılan Teknolojiler

- **.NET 8 / C#**
- **ASP.NET Core MVC** — web arayüzü için
- **Entity Framework Core 8** — veritabanı işlemleri için
- **SQLite** — veritabanı
- **xUnit** — testler için

## Proje Yapısı

```
SecilStoreConfig/
├── src/
│   ├── SecilStoreConfig.Library/   → Ayarları okuyan kütüphane (asıl mantık burada)
│   └── SecilStoreConfig.Web/       → Ayarları yönettiğimiz web arayüzü
└── tests/
    └── SecilStoreConfig.Library.Tests/  → Birim testleri
```

Veritabanı dosyası proje ilk çalıştığında otomatik oluşur ve içine birkaç örnek ayar eklenir.


## Nasıl Çalıştırılır

Gerekli: [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
# Projeyi indir
git clone https://github.com/elfcskn/SecilStoreConfig.git
cd SecilStoreConfig

# Derle
dotnet build

# Web arayüzünü çalıştır
dotnet run --project src/SecilStoreConfig.Web
```

Açılan sayfadan ayarları ekleyebilir, düzenleyebilir veya silebilirsin.

## Kütüphanenin Kullanımı

Ayarları okumak için `ConfigurationReader` sınıfı kullanılıyor. Uygulama adı, veritabanı bağlantısı
ve kaç milisaniyede bir ayarların yenileneceği bilgisiyle oluşturuluyor:

```csharp
using var reader = new ConfigurationReader(
    applicationName: "SERVICE-A",
    connectionString: "Data Source=config.db",
    refreshTimerIntervalInMs: 3000);

string siteName = await reader.GetValueAsync<string>("SiteName");
int maxItems    = await reader.GetValueAsync<int>("MaxItemCount");
double taxRate  = await reader.GetValueAsync<double>("TaxRate");
```

Notlar:
- `GetValueAsync<T>` ile ayarı istediğin tipe (`string`, `int`, `bool`, `double`) çevirerek alabilirsin.
- Ayar bulunamazsa `KeyNotFoundException` hatası verir.
- Ayarlar belirlenen süre kadar arada bir tekrar okunur, yani web arayüzünden yapılan değişiklik
  uygulamayı kapatmadan kısa süre içinde yansır.

## Testler

```
dotnet test
```
