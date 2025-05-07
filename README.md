# Number Fight

Bu proje, ASP.NET Core kullanılarak geliştirilmiş bir web uygulamasıdır.

## Proje Yapısı

Proje aşağıdaki ana bileşenlerden oluşmaktadır:

- **Controllers/**: Uygulama kontrolcüleri
- **Models/**: Veri modelleri
- **Views/**: Görünüm dosyaları
- **Services/**: İş mantığı servisleri
- **Data/**: Veritabanı bağlantıları ve konfigürasyonları
- **ViewModels/**: Görünüm modelleri
- **Pages/**: Razor sayfaları
- **wwwroot/**: Statik dosyalar (CSS, JavaScript, resimler vb.)

## Gereksinimler

- .NET 6.0 veya üzeri
- SQLite (veritabanı olarak kullanılmaktadır)

## Kurulum

1. Projeyi klonlayın:
```bash
git clone [proje-url]
```

2. Proje dizinine gidin:
```bash
cd numberFightMayis
```

3. Bağımlılıkları yükleyin:
```bash
dotnet restore
```

4. Veritabanını oluşturun:
```bash
dotnet ef database update
```

5. Uygulamayı çalıştırın:
```bash
dotnet run
```

## Geliştirme

Projeyi geliştirmek için:

1. Visual Studio Code veya Visual Studio kullanabilirsiniz
2. Değişiklik yaptıktan sonra veritabanı güncellemelerini uygulamak için:
```bash
dotnet ef migrations add [migration-adı]
dotnet ef database update
```

## Lisans

Bu proje [MIT lisansı](LICENSE) altında lisanslanmıştır. 