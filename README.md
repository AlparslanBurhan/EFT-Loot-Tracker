# EFT-Loot-Tracker

<div align="center">
  <img src="assets/EFT-Loot-Tracker.ico" alt="EFT-Loot-Tracker Logo" width="128" height="128">
  
  **Escape from Tarkov** oyuncuları için profesyonel eşya takip uygulaması
  
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
  [![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
  [![Platform](https://img.shields.io/badge/Platform-Windows-0078D4?logo=windows)](https://www.microsoft.com/windows)
</div>

---

## 📖 İçindekiler

- [Genel Bakış](#-genel-bakış)
- [Öne Çıkan Özellikler](#-öne-çıkan-özellikler)
- [Kurulum](#-kurulum)
  - [Son Kullanıcı Kurulumu](#son-kullanıcı-kurulumu-önerilen)
  - [Geliştirici Kurulumu](#geliştirici-kurulumu)
- [Kullanım](#-kullanım)
- [Teknik Detaylar](#-teknik-detaylar)
- [Proje Yapısı](#-proje-yapısı)
- [Veri Yönetimi](#-veri-yönetimi)
- [Installer Oluşturma](#-installer-oluşturma)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)

---

## 🎯 Genel Bakış

**EFT-Loot-Tracker**, Escape from Tarkov oyuncuları için geliştirilmiş, görevler (Quests) ve sığınak (Hideout) geliştirmeleri için gereken eşyaları takip etmeyi kolaylaştıran modern ve kullanıcı dostu bir **Windows masaüstü uygulamasıdır**.

Uygulama, EFT Wiki üzerinden otomatik olarak güncel verileri çekerek oyuncuların hangi eşyaları toplaması gerektiğini, kaç adet gerektiğini ve "Found in Raid" (FIR) durumunu görselleştirerek sunlar.

### 🎮 Kimler İçin?

- **Yeni oyuncular**: Görev gereksinimlerini takip etmek isteyenler
- **Deneyimli oyuncular**: Sığınak geliştirmeleri için eşya biriktirmek isteyenler
- **Collector görevini yapanlar**: 200+ eşya gereksinimini takip etmek isteyenler
- **Organizasyon severler**: Stashlerini düzenli tutmak isteyenler

---

## 🚀 Öne Çıkan Özellikler

### 🔄 Otomatik Veri Yönetimi
- **İlk açılışta otomatik veri çekme**: Program ilk kez açıldığında tüm verileri EFT Wiki'den otomatik olarak indirir
- **Akıllı önbellekleme**: Veriler yerel olarak saklanır ve 24 saatte bir otomatik güncellenir
- **Eksik veri kontrolü**: Manifest dosyası silinse bile otomatik olarak yeniden indirilir
- **Paralel ikon indirme**: 10 eşzamanlı bağlantı ile hızlı ikon indirme
- **Offline çalışma**: Veriler bir kez indirildikten sonra internet bağlantısı olmadan çalışır

### 📊 Detaylı Takip Sistemi
- **Görev bazlı takip**: Her eşya için hangi görevlerde kaç adet gerektiği
- **Sığınak modül takibi**: Hideout geliştirmeleri için gereken eşyalar
- **FIR (Found in Raid) göstergesi**: Hangi eşyaların FIR olması gerektiği etiketlerle belirtilir
- **Toplam gereksinim hesaplama**: Tüm görevler ve modüller için toplam ihtiyaç
- **Kategori etiketleri**: Eşya türlerine göre renk kodlu kategoriler

### 🔍 Gelişmiş Arama ve Filtreleme
- **Canlı arama**: Eşya ismine göre anlık filtreleme
- **Kategori filtreleme**: 
  - Tüm eşyalar
  - Sadece görev eşyaları
  - Sadece sığınak eşyaları
  - Collector görevi eşyaları (ayrı sekme)
- **Akıllı sıralama**: İsme, kategoriye veya gereksinim sayısına göre sıralama

### 🎨 Modern ve Optimize Arayüz
- **Karanlık Tema**: Tarkov estetiğine uygun modern, göz yormayan tasarım
- **Yüksek çözünürlük desteği**: HD/4K ekranlar için optimize edilmiş
- **Responsive tasarım**: Farklı pencere boyutlarında uyumlu çalışır
- **Hover efektleri**: İnteraktif kullanıcı deneyimi
- **Yüksek çözünürlüklü ikonlar**: Wiki'den çekilen orijinal eşya görselleri

### 💾 Güvenli Veri Depolama
- **ProgramData kullanımı**: `C:\ProgramData\EFT-Loot-Tracker` klasöründe merkezi depolama
- **Çoklu kullanıcı desteği**: Tüm Windows kullanıcıları aynı verileri paylaşır
- **Otomatik yedekleme**: Veriler JSON formatında düzenli şekilde saklanır
- **Kolay yönetim**: Klasör yapısı şeffaf ve erişilebilir

---

## 📦 Kurulum

### Son Kullanıcı Kurulumu (Önerilen)

#### 1. Gereksinimler
- **İşletim Sistemi**: Windows 10 (1809 veya üstü) / Windows 11
- **Mimari**: x64 (64-bit)
- **.NET Runtime**: [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (x64)
- **İnternet Bağlantısı**: İlk veri indirme için gerekli

#### 2. Installer ile Kurulum

1. **Installer'ı İndirin**: 
   - [Releases](https://github.com/AlparslanBurhan/EFT-Loot-Tracker/releases) sayfasından en son `EFT-Loot-Tracker_Setup_v1.0.0.exe` dosyasını indirin

2. **Installer'ı Çalıştırın**:
   - İndirilen `.exe` dosyasına çift tıklayın
   - Windows SmartScreen uyarısı çıkarsa "Daha fazla bilgi" → "Yine de çalıştır" seçin

3. **Kurulum Adımları**:
   - **Lisans Anlaşması**: MIT lisansını okuyup kabul edin
   - **Kurulum Konumu**: Varsayılan: `C:\ProgramData\EFT-Loot-Tracker`
   - **Kısayollar**: Masaüstü ve Başlat menüsü kısayolları oluşturulur
   - **Tamamlandı**: Kurulum sonrası otomatik başlatma seçeneği

4. **İlk Çalıştırma**:
   - Program ilk açılışta tüm verileri otomatik olarak indirecektir
   - İkon indirme işlemi birkaç dakika sürebilir
   - İndirme tamamlandığında tüm eşyalar listelenecektir

#### 3. Kurulum Sonrası
- Veriler: `C:\ProgramData\EFT-Loot-Tracker\data`
- İkonlar: `C:\ProgramData\EFT-Loot-Tracker\cache\icons`
- Program otomatik olarak 24 saatte bir verileri günceller

---

### Geliştirici Kurulumu

#### 1. Gereksinimler
- **İşletim Sistemi**: Windows 10/11
- **.NET SDK**: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) veya üstü
- **IDE** (Opsiyonel): Visual Studio 2022, VS Code veya Rider
- **Git**: Projeyi klonlamak için

#### 2. Proje Kurulumu

```bash
# Projeyi klonlayın
git clone https://github.com/AlparslanBurhan/EFT-Loot-Tracker.git

# Proje dizinine gidin
cd EFT-Loot-Tracker

# Bağımlılıkları yükleyin
dotnet restore

# Projeyi derleyin
dotnet build

# Uygulamayı çalıştırın
dotnet run
```

#### 3. Release Build Oluşturma

```bash
# Release modunda derleyin
dotnet build -c Release

# Yayınlanabilir sürüm oluşturun (Framework-dependent)
dotnet publish -c Release -r win-x64 --self-contained false

# Yayınlanabilir sürüm oluşturun (Self-contained)
dotnet publish -c Release -r win-x64 --self-contained true
```

#### 4. Inno Setup ile Installer Oluşturma

Detaylı bilgi için [INSTALLER_README.md](INSTALLER_README.md) dosyasına bakın.

```bash
# 1. Release build oluşturun
dotnet build -c Release

# 2. Inno Setup'ı açın ve setup.iss dosyasını yükleyin
# 3. Build → Compile seçeneğine tıklayın
# 4. Oluşturulan installer: installer_output/EFT-Loot-Tracker_Setup_v1.0.0.exe
```

---

## 🎮 Kullanım

### Ana Sekme (All Items)

1. **Eşya Arama**:
   - Üst kısımdaki arama kutusuna eşya adını yazın
   - Sonuçlar anlık olarak filtrelenir

2. **Kategori Filtreleme**:
   - "All Items": Tüm eşyaları gösterir
   - "Quest Items": Sadece görev için gereken eşyalar
   - "Hideout Items": Sadece sığınak geliştirmeleri için gerekli eşyalar

3. **Eşya Kartları**:
   - **Sol üst**: Eşya ikonu
   - **Sağ üst**: Kategori etiketi
   - **Ortada**: Eşya adı (tıklanabilir Wiki linki)
   - **Alt kısım**: Görev ve sığınak gereksinimleri (tag'ler halinde)
   - **Toplam**: Toplam gereksinim sayısı ve FIR durumu

4. **Tag Anlamları**:
   - 🎯 **Quest Tag'leri**: `[Görev Adı] x3 (FIR)`
   - 🏠 **Hideout Tag'leri**: `[Modül Adı] x5`
   - ⭐ **FIR Göstergesi**: Sarı renk = FIR gerekli

### Collector Sekmesi

- Collector görevi için gereken 200+ eşyayı gösterir
- Özel olarak bu görev için optimize edilmiş liste
- Tüm eşyalar FIR olarak işaretlenmiştir

### Veri Güncelleme

- **Otomatik**: Program 24 saatte bir otomatik günceller
- **Manuel**: "Force Update" butonu ile manuel güncelleme
- **Offline**: Veriler indirildikten sonra internet olmadan çalışır

---

## 🛠️ Teknik Detaylar

### Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| **.NET** | 10.0 | Ana framework |
| **WPF** | - | Kullanıcı arayüzü |
| **C#** | 11.0+ | Programlama dili |
| **HtmlAgilityPack** | 1.12.4 | Web scraping |
| **Newtonsoft.Json** | 13.0.4 | JSON serileştirme |
| **HttpClient** | - | HTTP istekleri |
| **Inno Setup** | 6.x | Installer oluşturma |

### Performans Özellikleri

- **Paralel İndirme**: 10 eşzamanlı bağlantı ile ikon indirme
- **Asenkron İşlemler**: Tüm I/O işlemleri async/await ile
- **Önbellekleme**: İndirilen ikonlar ve veriler yerel olarak saklanır
- **Bellek Yönetimi**: LINQ optimizasyonları ve dispose pattern
- **UI Thread Güvenliği**: Dispatcher kullanımı ile thread-safe UI güncellemeleri

### Güvenlik

- **MIT Lisans**: Açık kaynak ve atıf zorunluluğu ile
- **Güvenli HTTP**: Modern TLS desteği
- **Veri Doğrulama**: JSON şema validasyonu
- **Hata Yönetimi**: Try-catch blokları ile güvenli çalışma
- **İzinler**: ProgramData klasörü için tam kullanıcı erişimi

---

## 📁 Proje Yapısı

```
EFT-Loot-Tracker/
├── 📄 App.xaml                      # WPF uygulama tanımı
├── 📄 App.xaml.cs                   # Uygulama başlatma mantığı
├── 📄 MainWindow.xaml               # Ana pencere UI tanımı
├── 📄 MainWindow.xaml.cs            # Ana pencere mantığı
├── 📄 HtmlInputDialog.xaml          # HTML input dialog UI
├── 📄 HtmlInputDialog.xaml.cs       # Dialog mantığı
├── 📄 AssemblyInfo.cs               # Assembly metadata
├── 📄 EFTLootTracker.csproj         # Proje dosyası
├── 📄 setup.iss                     # Inno Setup installer script
├── 📄 LICENSE.txt                   # MIT Lisans (TR/EN)
├── 📄 README.md                     # Bu dosya
├── 📄 INSTALLER_README.md           # Installer dokümantasyonu
│
├── 📂 Models/                       # Veri modelleri
│   └── 📄 LootItem.cs              # Eşya veri yapısı
│
├── 📂 Services/                     # İş mantığı servisleri
│   ├── 📄 ScraperService.cs        # Wiki scraping mantığı
│   ├── 📄 DataService.cs           # Veri yönetimi ve dosya işlemleri
│   └── 📄 UpdateManager.cs         # Güncelleme yöneticisi
│
├── 📂 Converters/                   # WPF value converters
│   └── 📄 TabWidthConverter.cs     # Sekme genişlik dönüştürücü
│
├── 📂 assets/                       # Görsel varlıklar
│   └── 🖼️ EFT-Loot-Tracker.ico    # Uygulama ikonu
│
├── 📂 data/                         # Veri dosyaları (boş kurulur)
│   ├── 📄 manifest.json            # Ana eşya verisi (otomatik indirilir)
│   ├── 📄 collector.json           # Collector verisi (otomatik indirilir)
│   └── 📄 collector_static.html    # Collector HTML şablonu
│
├── 📂 cache/                        # Önbellek klasörü
│   └── 📂 icons/                   # İndirilen eşya ikonları (otomatik)
│
├── 📂 bin/                          # Derleme çıktıları
│   └── 📂 Debug|Release/           # Build konfigürasyonları
│
└── 📂 obj/                          # Geçici build dosyaları
```

### Mimari Açıklama

#### **Models/** - Veri Katmanı
- `LootItem.cs`: Eşya verilerini temsil eden model sınıfı
  - Özellikler: Name, IconUrl, LocalIconPath, Requirements, Quests, HideoutModules
  - JSON serileştirme desteği

#### **Services/** - İş Mantığı Katmanı
- `ScraperService.cs`: 
  - EFT Wiki'den HTML scraping
  - Quest ve Hideout verilerini parse etme
  - Collector özel verileri işleme
  
- `DataService.cs`:
  - ProgramData klasörü yönetimi
  - JSON dosya okuma/yazma
  - İkon indirme ve önbellekleme
  - Dosya varlık kontrolü
  
- `UpdateManager.cs`:
  - Veri güncelleme mantığı
  - Zaman tabanlı otomatik güncelleme (24h)
  - Progress tracking ve event handling

#### **Converters/** - UI Yardımcı Sınıflar
- `TabWidthConverter.cs`: Sekme genişliklerini dinamik olarak ayarlar

#### **MainWindow** - Sunum Katmanı
- XAML: UI tasarımı, stil tanımları, veri bağlama
- Code-behind: Event handling, kullanıcı etkileşimi, veri görselleştirme

---

## 💾 Veri Yönetimi

### Klasör Yapısı

Kurulum sonrası tüm veriler şu konumda saklanır:

```
C:\ProgramData\EFT-Loot-Tracker/
├── data/
│   ├── manifest.json           # ~15 MB - Tüm eşya verileri
│   └── collector.json          # ~2 MB - Collector verileri
│
└── cache/
    └── icons/                  # ~50 MB - 1000+ eşya ikonu
        ├── Item1.png
        ├── Item2.png
        └── ...
```

### Veri Akışı

```
┌─────────────────┐
│  İlk Açılış     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐      Hayır    ┌──────────────────┐
│ manifest.json   ├──────────────>│ EFT Wiki'den     │
│ var mı?         │                │ Veri Çek         │
└────────┬────────┘                └────────┬─────────┘
         │ Evet                             │
         │                                  ▼
         │                    ┌──────────────────────┐
         │                    │ İkonları İndir       │
         │                    │ (10 paralel)         │
         │                    └────────┬─────────────┘
         │                             │
         │                             ▼
         │                    ┌──────────────────────┐
         │                    │ JSON Kaydet          │
         │                    │ (manifest.json)      │
         │                    └────────┬─────────────┘
         │                             │
         ▼                             ▼
┌─────────────────────────────────────────┐
│          Verileri Görüntüle             │
└─────────────────────────────────────────┘
```

### Otomatik Güncelleme Mekanizması

1. **İlk Açılış**: 
   - `manifest.json` yoksa → İnternetten çek
   - `collector.json` yoksa → İnternetten çek

2. **Sonraki Açılışlar**:
   - Dosya var mı? → Var
   - Son güncelleme tarihi kontrol
   - 24 saatten eski mi? → Güncelle
   - Değilse → Yerel dosyayı kullan

3. **Dosya Silinirse**:
   - Program otomatik tespit eder
   - İnternetten yeniden indirir
   - Kullanıcı müdahalesine gerek yok

### JSON Veri Yapısı

```json
[
  {
    "Name": "Graphics card",
    "IconUrl": "https://static.wikia.nocookie.net/.../Graphics_card.png",
    "LocalIconPath": "C:\\ProgramData\\EFT-Loot-Tracker\\cache\\icons\\Graphics card.png",
    "Requirements": {
      "Total": 15,
      "FoundInRaid": 10
    },
    "Quests": [
      {
        "Name": "Farming - Part 4",
        "Count": 10,
        "IsFir": true
      }
    ],
    "HideoutModules": [
      {
        "Name": "Bitcoin Farm Level 1",
        "Count": 1
      }
    ],
    "Category": "Electronics",
    "WikiUrl": "https://escapefromtarkov.fandom.com/wiki/Graphics_card",
    "LastUpdated": "2025-12-30T10:30:00"
  }
]
```

---

## 🔧 Installer Oluşturma

### Gereksinimler

1. **Inno Setup 6.x**: [İndir](https://jrsoftware.org/isdl.php)
2. **.NET 10 SDK**: Projeyi derlemek için

### Adımlar

1. **Projeyi Derle**:
```powershell
dotnet build -c Release
```

2. **Inno Setup ile Derle**:
- Inno Setup'ı açın
- `setup.iss` dosyasını yükleyin
- **Build** → **Compile** seçeneğine tıklayın

3. **Çıktı**:
- Installer: `installer_output/EFT-Loot-Tracker_Setup_v1.0.0.exe`
- Boyut: ~5-10 MB (veriler dahil değil)

### Installer Özellikleri

- ✅ MIT Lisans gösterimi (Türkçe)
- ✅ ProgramData'ya kurulum
- ✅ Masaüstü ve Başlat menüsü kısayolları
- ✅ Özel uygulama ikonu
- ✅ Kaldırma programı
- ✅ Otomatik klasör yapısı oluşturma
- ✅ Tüm kullanıcılar için erişim izinleri

Detaylı bilgi: [INSTALLER_README.md](INSTALLER_README.md)

---

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Projeye katkıda bulunmak için:

### 1. Hata Bildirimi
- [Issues](https://github.com/AlparslanBurhan/EFT-Loot-Tracker/issues) sayfasından yeni bir issue açın
- Hatayı detaylı açıklayın
- Ekran görüntüleri ekleyin
- Sistem bilgilerinizi belirtin

### 2. Özellik İsteği
- [Issues](https://github.com/AlparslanBurhan/EFT-Loot-Tracker/issues) sayfasından "Feature Request" etiketi ile issue açın
- Özelliği detaylı açıklayın
- Kullanım senaryolarını belirtin

### 3. Pull Request
```bash
# 1. Projeyi fork edin
# 2. Yeni bir branch oluşturun
git checkout -b feature/YeniOzellik

# 3. Değişikliklerinizi yapın ve commit edin
git commit -m "Yeni özellik: XYZ eklendi"

# 4. Branch'inizi push edin
git push origin feature/YeniOzellik

# 5. Pull Request oluşturun
```

### Kod Standartları
- C# kod standartlarına uyun
- XAML formatlamasına dikkat edin
- Yorum satırları ekleyin
- Unit test yazın (opsiyonel ama tercih edilir)

---

## 📄 Lisans

Bu proje **MIT Lisansı** ile lisanslanmıştır. Detaylar için [LICENSE.txt](LICENSE.txt) dosyasına bakın.

### Zorunlu Atıf Şartları

Bu projeyi kullanırken aşağıdaki atfı yapmalısınız:

```
Bu proje EFT-Loot-Tracker (Alparslan Burhan) yazılımını kullanmaktadır.
Kaynak: https://github.com/AlparslanBurhan/EFT-Loot-Tracker
```

### İzinler
✅ Ticari kullanım  
✅ Değiştirme  
✅ Dağıtım  
✅ Özel kullanım  

### Koşullar
⚠️ Lisans ve telif hakkı bildiriminin korunması  
⚠️ Atıfta bulunma zorunluluğu  

### Sorumluluk
❌ Sorumluluk yok  
❌ Garanti yok  

---

## 🌟 Özel Teşekkürler

- **Battlestate Games**: Escape from Tarkov oyunu için
- **EFT Wiki Topluluğu**: Güncel veri kaynağı için
- **Open Source Community**: Kullanılan kütüphaneler için

---

## 📞 İletişim

- **Geliştirici**: Alparslan Burhan
- **GitHub**: [@AlparslanBurhan](https://github.com/AlparslanBurhan)
- **Proje Linki**: [https://github.com/AlparslanBurhan/EFT-Loot-Tracker](https://github.com/AlparslanBurhan/EFT-Loot-Tracker)
- **Issues**: [GitHub Issues](https://github.com/AlparslanBurhan/EFT-Loot-Tracker/issues)

---

## 📊 Proje İstatistikleri

- **Toplam Takip Edilen Eşya**: ~1000+
- **Görev Sayısı**: 200+
- **Hideout Modül Sayısı**: 25+
- **Kod Satırı**: ~5000+
- **Veri Boyutu**: ~17 MB
- **İkon Önbelleği**: ~50 MB

---

## 🔄 Güncellemeler

### v1.0.0 (30 Aralık 2025)
- ✨ İlk stabil sürüm
- 🎯 Tam otomatik veri çekme
- 💾 ProgramData entegrasyonu
- 📦 Inno Setup installer
- 🎨 Modern UI tasarımı
- 🔍 Gelişmiş arama ve filtreleme
- 📝 Tam Türkçe ve İngilizce dokümantasyon

---

<div align="center">
  
**⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın! ⭐**

*Bu proje Escape from Tarkov topluluğu için sevgiyle geliştirilmiştir.*

[🔝 Başa Dön](#eft-loot-tracker)

</div>