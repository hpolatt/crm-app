# PKTApp - Production Tracking Application

Bu proje, üretim takip ve yönetim sistemidir. Reaktör bazlı üretim süreçlerini takip etmek için geliştirilmiştir.

## Veritabanı Yapısı

Uygulama 4 ana tablodan oluşur:

### Tanım Tabloları

1. **DelayReasons** - Gecikme Nedenleri
   - Üretim sürecinde oluşan gecikmelerin nedenlerini tanımlar

2. **Reactors** - Reaktörler
   - Üretim için kullanılan reaktörleri tanımlar

3. **Products** - Ürünler
   - Üretilen ürünlerin detaylarını içerir
   - SBU, Ürün Kodu, Ürün Adı
   - Min/Max Üretim Miktarları
   - Üretim Süresi (Saat)

### Transaction Tablosu

4. **PktTransactions** - PKT İşlemleri
   - Üretim işlemlerinin takip edildiği ana tablodur
   - Reaktör, Ürün ve Gecikme Nedeni ile ilişkilidir
   - İş Emri No, Lot No
   - Başlangıç ve Bitiş Zamanları
   - Gerçek Üretim Süresi, Gecikme Süresi, Yıkama Süresi
   - Kostik Miktarı (Kg)

## Teknoloji Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture / Onion Architecture
  - Domain Layer (Entities)
  - Application Layer (DTOs, Interfaces)
  - Infrastructure Layer (Data Access, Repositories)
  - Presentation Layer (API Controllers)

## Proje Yapısı

```
backend/
├── Core/
│   ├── CRM.Domain/           # Entity sınıfları
│   └── CRM.Application/       # DTOs ve Interfaces
├── Infrastructure/
│   └── CRM.Persistence/       # DbContext, Repositories, UnitOfWork
├── PktApp.API/                # API Controllers
└── database/                  # SQL scriptleri
    ├── 02_create_tables.sql
    ├── 03_create_indexes.sql
    └── 05_seed_data.sql
```

## API Endpoints

### DelayReasons (Gecikme Nedenleri)
- `GET /api/delayreasons` - Tüm gecikme nedenlerini listele
- `GET /api/delayreasons/{id}` - Belirli bir gecikme nedenini getir
- `POST /api/delayreasons` - Yeni gecikme nedeni ekle
- `PUT /api/delayreasons/{id}` - Gecikme nedenini güncelle
- `DELETE /api/delayreasons/{id}` - Gecikme nedenini sil

### Reactors (Reaktörler)
- `GET /api/reactors` - Tüm reaktörleri listele
- `GET /api/reactors/{id}` - Belirli bir reaktörü getir
- `POST /api/reactors` - Yeni reaktör ekle
- `PUT /api/reactors/{id}` - Reaktörü güncelle
- `DELETE /api/reactors/{id}` - Reaktörü sil

### Products (Ürünler)
- `GET /api/products` - Tüm ürünleri listele
- `GET /api/products/{id}` - Belirli bir ürünü getir
- `POST /api/products` - Yeni ürün ekle
- `PUT /api/products/{id}` - Ürünü güncelle
- `DELETE /api/products/{id}` - Ürünü sil

### PktTransactions (PKT İşlemleri)
- `GET /api/pkttransactions` - Tüm işlemleri listele
- `GET /api/pkttransactions/{id}` - Belirli bir işlemi getir
- `POST /api/pkttransactions` - Yeni işlem ekle
- `PUT /api/pkttransactions/{id}` - İşlemi güncelle
- `DELETE /api/pkttransactions/{id}` - İşlemi sil

## Kurulum

### Veritabanı

1. PostgreSQL veritabanını oluşturun
2. SQL scriptlerini sırayla çalıştırın:
   ```bash
   psql -U postgres -d pktapp -f database/02_create_tables.sql
   psql -U postgres -d pktapp -f database/03_create_indexes.sql
   psql -U postgres -d pktapp -f database/05_seed_data.sql
   ```

### Backend API

1. Connection string'i güncelleyin (`appsettings.json`):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=pktapp;Username=postgres;Password=yourpassword"
     }
   }
   ```

2. Projeyi derleyin ve çalıştırın:
   ```bash
   cd backend/PktApp.API
   dotnet build
   dotnet run
   ```

3. Swagger UI: `http://localhost:5000/swagger`

## Geliştirme Notları

- Tüm entity'ler `BaseEntity` sınıfından türemiştir (Id, CreatedAt, UpdatedAt)
- Repository Pattern ve Unit of Work Pattern kullanılmıştır
- API yanıtları `ApiResponse<T>` wrapper sınıfı ile sarılmıştır
- CORS tüm origin'ler için açıktır (production'da güncellenmeli)

## Namespace Değişiklikleri

Proje CRM uygulamasından refactore edilmiştir:
- `CRM.Domain` → `PKT.Domain`
- `CRM.Application` → `PKT.Application`
- `CRM.Persistence` → `PKT.Persistence`
- `PktApp` → `PKTApp`
