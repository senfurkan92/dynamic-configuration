# Dynamic Configuration Management System

Bu proje, **.NET 8** ile geliştirilmiş dinamik bir konfigürasyon yönetim sistemidir.  
Amaç, farklı servislerin (`SERVICE-A`, `SERVICE-B` vb.) konfigürasyon verilerini merkezi bir **storage** üzerinden dinamik olarak yönetmek ve bu verileri uygulama yeniden başlatılmadan güncelleyebilmektir.

---

## 🚀 Özellikler

- **Dinamik konfigürasyon yönetimi:**  
  Konfigürasyon değerleri runtime’da storage’dan okunur ve belirli aralıklarla otomatik güncellenir.

- **Merkezi storage yapısı:**  
  Konfigürasyon kayıtları MongoDB / Redis yapılarında tutulur.

- **Tip güvenli erişim:**  
  `GetValue<T>(string key)` metodu ile konfigürasyon değerlerine strongly-typed şekilde erişim sağlanır.

- **Uygulama bazlı yapı:**  
  Her servis yalnızca kendi `ApplicationName` değerine ait konfigürasyon kayıtlarını görebilir.

- **Kapsamlı hata toleransı:**  
  Storage’a erişim sorunlarında sistem en son başarılı konfigürasyon değerleriyle çalışmaya devam eder.

- **Web arayüzü:**  
  Storage üzerindeki kayıtlar listelenebilir, güncellenebilir, silinebilir ve yeni kayıtlar eklenebilir. Arayüz vue cdn ile geliştirilmiştir.

---

## 🧱 Mimarî Yapı

Bu proje, **Katmanlı Mimari**, **Clean Architecture** ve **Onion Architecture** prensipleri esas alınarak geliştirilmiştir.  
Amaç; bağımlılıkları minimuma indirmek, test edilebilirliği artırmak ve sürdürülebilir bir yapı oluşturmaktır.

---
## 🌟 Ekstra Özellikler ve İyileştirmeler

Bu proje, temel gereksinimlerin ötesine geçerek ek puan kriterlerini karşılayacak şekilde tasarlanmıştır.  
Aşağıda uygulanan geliştirmeler ve kullanılan teknolojiler listelenmiştir:

---

### 📨 Message Broker Entegrasyonu (RabbitMQ)

- **RabbitMQ** kullanılarak mesaj tabanlı iletişim sistemi kurulmuştur.  
- Konfigürasyon güncellemeleri, değişiklik bildirimleri ve event publishing işlemleri message queue üzerinden yönetilmektedir.  

---

### ⚙️ Async/Await Kullanımı

- Tüm IO tabanlı işlemler (storage erişimi, cache yenileme, API çağrıları vb.) **asenkron** olarak yürütülmektedir.  
- **Async/Await** kullanılarak performans artırılmış, thread blocking önlenmiştir.

---

### 🔒 Concurrency Kontrol Mekanizması

- Olası **yarış durumlarını (race condition)** önlemek için **timestamp tabanlı concurrency kontrolü** uygulanmıştır.  
- Her konfigürasyon kaydı bir **`LastModifiedTimestamp`** değeriyle birlikte tutulmaktadır.  
- Güncelleme işlemi sırasında storage üzerindeki mevcut timestamp değeri ile gönderilen değerin karşılaştırılması yapılır.  
- Eğer kayıt bu sürede başka bir işlem tarafından değiştirilmişse, işlem reddedilir ve **concurrency conflict** hatası üretilir.  
- Bu sayede aynı kaydın eşzamanlı olarak birden fazla işlem tarafından değiştirilmesi engellenmiştir.

> 💡 *Bu yaklaşım sayesinde hem veri bütünlüğü sağlanır hem de gereksiz kilitleme (lock contention) maliyetleri ortadan kaldırılır.*

---

### 🧩 Design & Architectural Pattern’ler

Proje, birçok yazılım tasarım kalıbı ve mimari desen içermektedir:

- **Clean Architecture**  
- **Onion Architecture**  
- **Repository Pattern**  
- **Dependency Injection**  
- **CQRS Pattern** 

Bu sayede proje modüler, test edilebilir ve genişletilebilir bir yapıya kavuşmuştur.

---

### 🧪 Unit Testler

- Gerekli methodlar için **unit test** senaryoları oluşturulmuştur.  
- Testler, **xUnit** kütüphanesi kullanılarak yazılmıştır.  

---

## 🐳 Docker & Deployment

Proje tamamen **Docker Compose** altyapısı kullanılarak container tabanlı şekilde çalıştırılabilir.  
Tüm servisler tek komutla ayağa kalkacak şekilde yapılandırılmıştır.

---

### 🚀 Servis Bileşenleri

`docker-compose.yml` dosyası aşağıdaki bileşenleri içerir:

| Servis | Açıklama | Port |
|--------|-----------|------|
| **mongodb** | Konfigürasyon verilerinin tutulduğu MongoDB veritabanı | `27017` |
| **redis** | Cache katmanı (konfigürasyon verilerinin hızlı erişimi için) | `6379` |
| **rabbitmq** | Message Broker (event-based iletişim) | `5672` / `15672` |
| **admin-panel** | Web tabanlı yönetim arayüzü (konfigürasyonları görüntüleme/güncelleme) | `8080` |

---

### ⚙️ Çalıştırma Adımları

1. **Docker Compose ile build & run işlemini başlatın:**

```bash
docker-compose up -d --build





