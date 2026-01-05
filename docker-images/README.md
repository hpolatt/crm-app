# PKT App - Docker Deployment Kılavuzu

## Hızlı Başlangıç

### 1. Image'ları Yükle ve Çalıştır

```bash
# Image'ları yükle
docker load -i pkt-app-frontend.tar
docker load -i pkt-app-backend.tar
docker load -i pkt-app-database.tar

# Otomatik başlatma scripti ile (docker-compose gerekli)
./load-and-run.sh

# VEYA manuel başlatma (docker-compose olmadan)
./quick-start.sh
```

### 2. Manuel Başlatma (Adım Adım)

```bash
# Network oluştur
docker network create pkt-network

# Database başlat
docker run -d \
  --name pkt-database \
  --network pkt-network \
  -e POSTGRES_DB=pktappdb \
  -e POSTGRES_USER=pktadmin \
  -e POSTGRES_PASSWORD=Pkt2024Secure! \
  -p 5433:5432 \
  -v pkt_postgres_data:/var/lib/postgresql/data \
  pkt-app-database:latest

# Backend başlat (10 saniye bekle)
sleep 10
docker run -d \
  --name pkt-backend \
  --network pkt-network \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e ConnectionStrings__DefaultConnection="Host=pkt-database;Port=5432;Database=pktappdb;Username=pktadmin;Password=Pkt2024Secure!" \
  -p 8080:8080 \
  pkt-app-backend:latest

# Frontend başlat (5 saniye bekle)
sleep 5
docker run -d \
  --name pkt-frontend \
  --network pkt-network \
  -p 80:80 \
  pkt-app-frontend:latest
```

## Erişim Adresleri

- **Frontend**: http://localhost
- **Backend API**: http://localhost:8080
- **Database**: localhost:5433

## Kullanıcı Bilgileri

### Varsayılan Admin Kullanıcısı
- **Email**: admin@pktapp.com
- **Şifre**: Admin123!

### Excel Import Kullanıcısı
- **Email**: ekin@pktapp.com
- **Şifre**: Ekin123!

## Veritabanı Bilgileri

- **Host**: localhost (container içinden: pkt-database)
- **Port**: 5433 (host), 5432 (container)
- **Database**: pktappdb
- **Username**: pktadmin
- **Password**: Pkt2024Secure!

## Yönetim Komutları

### Container'ları Kontrol Et
```bash
docker ps --filter "name=pkt-"
```

### Log'ları Görüntüle
```bash
docker logs -f pkt-frontend
docker logs -f pkt-backend
docker logs -f pkt-database
```

### Container'ları Durdur
```bash
docker stop pkt-frontend pkt-backend pkt-database
```

### Container'ları Kaldır
```bash
docker rm pkt-frontend pkt-backend pkt-database
```

### Container'ları Yeniden Başlat
```bash
docker restart pkt-frontend pkt-backend pkt-database
```

### Volume'ları Temizle (DİKKAT: Tüm verileri siler!)
```bash
docker volume rm pkt_postgres_data
```

## Docker Compose ile Çalıştırma

Eğer `docker-compose.yml` ve `.env` dosyalarınız varsa:

```bash
docker-compose up -d
```

### Durdurma
```bash
docker-compose down
```

### Tamamen Temizleme (volume'lar dahil)
```bash
docker-compose down -v
```

## Sorun Giderme

### Container çalışmıyor
```bash
docker logs pkt-backend
docker logs pkt-database
```

### Port çakışması
Eğer 80, 8080 veya 5433 portları kullanılıyorsa, farklı portlar kullanın:
```bash
# Frontend için 8081 portu
docker run -d --name pkt-frontend -p 8081:80 pkt-app-frontend:latest

# Backend için 8082 portu
docker run -d --name pkt-backend -p 8082:8080 pkt-app-backend:latest

# Database için 5434 portu
docker run -d --name pkt-database -p 5434:5432 pkt-app-database:latest
```

### Database bağlantı hatası
Backend container'ının database'e bağlanabilmesi için 10-15 saniye bekleyin.

## Güncelleme

Yeni image'lar aldığınızda:

```bash
# Eski container'ları durdur ve kaldır
docker stop pkt-frontend pkt-backend pkt-database
docker rm pkt-frontend pkt-backend pkt-database

# Eski image'ları kaldır (opsiyonel)
docker rmi pkt-app-frontend:latest pkt-app-backend:latest pkt-app-database:latest

# Yeni image'ları yükle
docker load -i pkt-app-frontend.tar
docker load -i pkt-app-backend.tar
docker load -i pkt-app-database.tar

# Yeniden başlat
./quick-start.sh
```

## Yedekleme

### Database Yedekleme
```bash
docker exec pkt-database pg_dump -U pktadmin pktappdb > backup.sql
```

### Database Geri Yükleme
```bash
docker exec -i pkt-database psql -U pktadmin pktappdb < backup.sql
```
