# EFT-Loot-Tracker Build ve Kurulum Rehberi

Bu dosya, projenizi en küçük boyutta ve tek bir dosya olacak şekilde derlemek ve Inno Setup ile kurulum dosyası oluşturmak için gerekli adımları içerir.

## 1. Optimize Edilmiş Derleme (Build)

Aşağıdaki komut, uygulamayı şu özelliklerle derler:
- **Tek Dosya (Single File)**: Tüm bağımlılıklar tek bir `.exe` içinde toplanır.
- **Sadece Türkçe**: Sadece Türkçe dil kaynakları dahil edilir (`tr`).
- **Küçültülmüş Boyut**: Çalışma zamanı (runtime) dahil edilmez (Self-Contained: false), bu sayede exe boyutu minimumda kalır.
- **Performans**: ReadyToRun özelliği ile açılış hızı optimize edilir.

### Derleme Komutu:
Terminalde (PowerShell veya CMD) proje ana dizininde şu komutu çalıştırın:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:SatelliteResourceLanguages=tr -p:DebugType=none -p:DebugSymbols=false
```

Derlenen dosyalar şu klasörde oluşacaktır:
`bin\Release\net10.0-windows\win-x64\publish\`

## 2. Kurulum Dosyası Oluşturma (Inno Setup)

Projeye eklenen `setup.iss` dosyasını Inno Setup derleyicisi ile açın ve derleyin (Build).

### Özellikler:
- **Kurulum Dizini**: Uygulama `C:\ProgramData\EFT-Loot-Tracker` klasörüne kurulur.
- **Kısayollar**: Başlat menüsü ve isteğe bağlı masaüstü kısayolu oluşturur.
- **Dil**: Kurulum arayüzü Türkçedir.
- **Yetki**: `ProgramData` klasörüne yazabilmek için yönetici yetkisi ile çalışır.

> [!NOTE]
> Uygulama ilk çalıştığında kendi yanında `data` ve `cache` klasörlerini oluşturacaktır. `ProgramData` dizini genellikle yazma izni gerektirdiğinden, uygulamanın yönetici olarak çalışması veya bu klasöre yazma izni olması gerekebilir. (Inno Setup scripti admin yetkisiyle kurulur).
