# EFT Loot Tracker

**EFT Loot Tracker**, Escape from Tarkov oyuncuları için geliştirilmiş, görevler (Quests) ve sığınak (Hideout) geliştirmeleri için gereken eşyaları takip etmeyi kolaylaştıran modern ve kullanıcı dostu bir Windows masaüstü uygulamasıdır.

## 🚀 Öne Çıkan Özellikler

- **Otomatik Veri Güncelleme**: EFT Wiki üzerinden en güncel eşya gereksinimlerini (Görev, Sığınak, Sayılar) otomatik olarak çeker.
- **Detaylı Takip Sistemi**: Her eşya için hangi görevde kaç adet gerektiği ve "Found In Raid" (FIR) statüsü etiketler (tagler) halinde sunulur.
- **Gelişmiş Arama ve Filtreleme**: 
  - İsim bazlı canlı arama.
  - Görev veya Sığınak modülüne göre kategori bazlı filtreleme.
- **Modern EFT Teması**: Karanlık ve Tarkov estetiğine uygun, premium bir kullanıcı arayüzü.
- **Yerel Cache Sistemi**: Verileri ve simgeleri yerelde depolayarak hızlı bir kullanıcı deneyimi sunar.
- **Yüksek Çözünürlüklü İkonlar**: Wiki'den çekilen eşya ikonları yerel olarak saklanır.

## 🛠️ Kullanılan Teknolojiler

- **Platform**: .NET 10 (WPF - Windows Presentation Foundation)
- **Kütüphaneler**: 
  - `HtmlAgilityPack`: Web scraping işlemleri için.
  - `Newtonsoft.Json`: Veri serileşimi ve yerel depolama için.
  - `HttpClient`: Modern ağ istekleri için.

## 📁 Proje Yapısı

- **Models/**: `LootItem` ve gereksinim veri yapılarını içerir.
- **Services/**:
  - `ScraperService`: Wiki tarama ve veri işleme mantığı.
  - `DataService`: Dosya sistemi, cache ve ikon indirme yönetimi.
  - `UpdateManager`: Veri tazeliği ve senkronizasyon yönetimi.
- **MainWindow.xaml/cs**: Ana kullanıcı arayüzü ve etkileşim mantığı.

## 💻 Kurulum ve Çalıştırma

1. **Gereksinimler**: Bilgisayarınızda [.NET 10 SDK](https://dotnet.microsoft.com/download) kurulu olmalıdır.
2. **Klonlama**:
   ```bash
   git clone https://github.com/AlparslanBurhan/EFT-Item-Tracker.git
   ```
3. **Çalıştırma**: Proje dizininde terminali açın:
   ```bash
   dotnet run
   ```

## 📸 Ekran Görüntüleri

Uygulama, karmaşık gereksinim listelerini bile düzenli etiketler halinde sunarak ekran karmaşasını önler ve en kritik bilgileri (Toplam gereken / FIR gereken) anında görmenizi sağlar.

---
*Bu proje Escape from Tarkov topluluğu için yardımcı bir araç olarak geliştirilmiştir.*