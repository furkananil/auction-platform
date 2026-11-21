# Auction Platform

Auction Platform, ürünlerin listelenebildiği, açık artırmaların yönetilebildiği ve kullanıcıların gerçek zamanlı teklif verebildiği .NET 9 tabanlı bir mikroservis uygulamasıdır.

Proje; servislerin kendi veritabanlarına sahip olduğu, servisler arası iletişimde REST, gRPC ve olay tabanlı mesajlaşmanın birlikte kullanıldığı bir yapı sunar.

## Özellikler

- Kullanıcı kaydı, giriş ve JWT tabanlı kimlik doğrulama
- Ürün ve kategori kataloğu
- Açık artırma oluşturma ve otomatik kapatma
- Kimliği doğrulanmış kullanıcılar için teklif verme
- İdempotency anahtarı ile güvenli teklif işlemleri
- Kafka üzerinden açık artırma ve teklif olayları
- SignalR ile gerçek zamanlı teklif bildirimleri
- Açık artırma tamamlandığında sipariş oluşturma
- gRPC üzerinden katalog ve ödeme servisleriyle iletişim
- RabbitMQ/MassTransit ile e-posta bildirim kuyruğu
- Redis önbellekleme
- PostgreSQL ile servis başına ayrı veri saklama
- YARP tabanlı API Gateway
- Seq ile merkezi log izleme

## Mimari

```text
Client
  |
  v
API Gateway (:5002)
  |
  +--> Identity Service ------> IdentityDb
  +--> Catalog Service -------> CatalogDb
  +--> Auction Service -------> AuctionDb
  +--> Bidding Service -------> BiddingDb
  +--> Ordering Service ------> OrderingDb
  +--> Payment Service -------> PaymentDb
  +--> Notification Service
  +--> Realtime Service ------> SignalR / Kafka

Kafka       : servisler arası açık artırma ve teklif olayları
RabbitMQ    : e-posta bildirimleri
Redis       : açık artırma önbelleği
Seq         : log izleme
```

### Servisler

| Servis | Sorumluluk | Compose portu |
|---|---|---:|
| `ApiGateway` | İstek yönlendirme, JWT doğrulama ve merkezi giriş noktası | `5002` |
| `IdentityService` | Kullanıcı kaydı, giriş ve JWT üretimi | `5009` |
| `CatalogService` | Ürün ve katalog yönetimi | `5000` |
| `AuctionService` | Açık artırma oluşturma, takip etme ve kapatma | `5004` |
| `BiddingService` | Teklif oluşturma ve teklif olayları | `5001` |
| `OrderingService` | Kapanan açık artırmalar için sipariş akışı | `5005` |
| `PaymentService` | gRPC ödeme işlemleri | `5006` / `5007` |
| `NotificationService` | E-posta bildirimlerini kuyruğa gönderme | `5008` |
| `NotificationDelivery.Worker` | E-posta kuyruğunu tüketme | - |
| `RealtimeService` | SignalR üzerinden canlı teklif güncellemeleri | `5102` |

## Teknoloji yığını

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9
- PostgreSQL 16
- Apache Kafka ve ZooKeeper
- RabbitMQ 3
- Redis 7
- SignalR
- gRPC / Protocol Buffers
- YARP Reverse Proxy
- MediatR
- MassTransit
- Serilog ve Seq
- Docker Compose

## Gereksinimler

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Docker Compose
- Yerel geliştirme için [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Git

## Kurulum ve çalıştırma

1. Repoyu klonlayın:

   ```bash
   git clone https://github.com/furkananil/auction-platform.git
   cd auction-platform
   ```

2. İsteğe bağlı olarak kök dizinde `.env` dosyası oluşturun. Varsayılan geliştirme değerlerini değiştirmek için örnek:

   ```env
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=change-me
   JWT_KEY=replace-with-a-long-random-secret
   JWT_ISSUER=AuctionIdentityService
   JWT_AUDIENCE=AuctionMicroservices
   ```

3. Tüm altyapıyı ve servisleri başlatın:

   ```bash
   docker compose up --build
   ```

4. Servisleri arka planda çalıştırmak için:

   ```bash
   docker compose up --build -d
   ```

5. Logları takip etmek için:

   ```bash
   docker compose logs -f apigateway
   ```

6. Ortamı durdurmak için:

   ```bash
   docker compose down
   ```

Verileri de silerek temiz bir başlangıç yapmak için `docker compose down -v` kullanılabilir. Bu komut kalıcı Docker volume'larını da sileceği için dikkatli kullanılmalıdır.

## Erişim noktaları

### API Gateway

Uygulamaya istemci tarafından önerilen giriş noktası:

```text
http://localhost:5002
```

Gateway üzerinden tanımlı ana rotalar:

| Rota | Açıklama |
|---|---|
| `/api/auth/*` | Kayıt ve giriş |
| `/api/products/*` | Ürün işlemleri |
| `/api/auctions/*` | Açık artırma işlemleri |
| `/api/bids/*` | Teklif işlemleri; JWT gerekir |
| `/api/orders/*` | Sipariş işlemleri |
| `/api/payments/*` | Gateway'de tanımlı ödeme rotası; ödeme servisi şu anda gRPC sunar |
| `/api/notifications/*` | Bildirim işlemleri |
| `/auctionhub/*` | Gerçek zamanlı iletişim |

### Doğrudan servis portları

Swagger etkin olan API servislerinde geliştirme ortamında `/swagger` adresi kullanılabilir. Servisler gerektiğinde doğrudan da çağrılabilir:

```text
Catalog       http://localhost:5000
Bidding       http://localhost:5001
ApiGateway    http://localhost:5002
Auction       http://localhost:5004
Ordering      http://localhost:5005
Payment HTTP  http://localhost:5006
Notification  http://localhost:5008
Identity      http://localhost:5009
Realtime      http://localhost:5102
Seq           http://localhost:5341
RabbitMQ UI   http://localhost:15672
```

## Temel API örnekleri

### Kullanıcı kaydı

```http
POST http://localhost:5002/api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

### Giriş

```http
POST http://localhost:5002/api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

Giriş yanıtındaki JWT, kimlik doğrulaması gerektiren isteklerde kullanılmalıdır:

```http
Authorization: Bearer <token>
```

### Ürün oluşturma

```http
POST http://localhost:5002/api/products
Content-Type: application/json

{
  "name": "Koleksiyon Ürünü",
  "description": "Açık artırmaya sunulan ürün",
  "sellerId": "00000000-0000-0000-0000-000000000001",
  "categoryId": "00000000-0000-0000-0000-000000000002",
  "startingPrice": 1000,
  "imageUrls": [
    "https://example.com/product.jpg"
  ]
}
```

### Açık artırma oluşturma

```http
POST http://localhost:5002/api/auctions
Content-Type: application/json

{
  "productId": "00000000-0000-0000-0000-000000000003",
  "sellerId": "00000000-0000-0000-0000-000000000001",
  "startingPrice": 1000,
  "startTime": "2026-01-01T10:00:00Z",
  "endTime": "2026-01-02T10:00:00Z"
}
```

### Teklif verme

Teklif veren kullanıcı JWT içinden belirlenir; `bidderId` istemci gövdesinden alınmaz.

```http
POST http://localhost:5002/api/bids
Authorization: Bearer <token>
Content-Type: application/json

{
  "auctionId": "00000000-0000-0000-0000-000000000003",
  "productId": "00000000-0000-0000-0000-000000000003",
  "amount": 1250,
  "idempotencyKey": "unique-request-key-001"
}
```

## SignalR ile canlı teklifler

Realtime servisi SignalR hub'ını şu adreste sunar:

```text
http://localhost:5102/auction-hub
```

Bir istemci açık artırma grubuna katılmak için `JoinAuctionGroup(auctionId)`, ayrılmak için `LeaveAuctionGroup(auctionId)` hub metotlarını çağırabilir. Kafka'dan gelen teklif olayları ilgili açık artırma grubuna yayınlanır.

## Servisler arası iletişim

- **REST:** İstemci ile API servisleri ve API Gateway arasındaki HTTP iletişimi.
- **gRPC:** Catalog servisinin ürün sorguları ve Payment servisinin ödeme işlemleri.
- **Kafka:** Teklif, açık artırma ve açık artırmanın kapanması gibi entegrasyon olayları.
- **RabbitMQ/MassTransit:** Bildirim servisinden e-posta teslim worker'ına mesaj aktarımı.
- **Outbox:** Auction, Bidding ve ilgili servislerde olayların güvenilir şekilde yayınlanmasına yardımcı olan kalıcı mesaj yaklaşımı.

## Veritabanları ve altyapı portları

| Bileşen | Host portu |
|---|---:|
| Catalog PostgreSQL | `5434` |
| Bidding PostgreSQL | `5435` |
| Auction PostgreSQL | `5436` |
| Ordering PostgreSQL | `5437` |
| Payment PostgreSQL | `5438` |
| Identity PostgreSQL | `5439` |
| Redis | `6379` |
| ZooKeeper | `2181` |
| Kafka | `9092` |
| RabbitMQ AMQP | `5672` |
| RabbitMQ Management | `15672` |
| Seq | `5341` |

Her servis kendi veritabanı ve migration'larıyla ayrıştırılmıştır. Docker Compose ilk çalıştırmada PostgreSQL volume'larını oluşturur.

## Yerel geliştirme

Her servis bağımsız bir solution olarak yapılandırılmıştır:

```text
AuctionService/AuctionService.sln
BiddingService/BiddingService.sln
CatalogService/CatalogService.sln
IdentityService/IdentityService.sln
OrderingService/OrderingService.sln
PaymentService/PaymentService.sln
NotificationService/NotificationService.sln
```

Bir servisi Docker dışında çalıştırmak için ilgili solution dizininde:

```bash
dotnet restore
dotnet build
dotnet run --project <Service>.Api/<Service>.Api.csproj
```

Yerel çalıştırmada PostgreSQL, Kafka, Redis ve RabbitMQ bağlantılarının ilgili `appsettings.Development.json` dosyalarındaki değerlerle uyumlu olduğundan emin olun.

## Proje yapısı

Her domain servisi mümkün olduğunca aşağıdaki katmanlara ayrılmıştır:

```text
<Service>/
├── <Service>.Api/            # HTTP/gRPC giriş noktaları ve controller'lar
├── <Service>.Application/    # Use case, command/query ve uygulama kuralları
├── <Service>.Domain/         # Entity, event, enum ve repository sözleşmeleri
└── <Service>.Infrastructure/ # EF Core, repository, cache ve background job'lar
```

## Güvenlik notları

`docker-compose.yml` içindeki şifreler ve JWT varsayılanları yalnızca geliştirme içindir. Üretim ortamında:

- Güçlü ve rastgele secret değerleri kullanın.
- Secret'ları kaynak koda veya compose dosyasına yazmayın; secret manager/environment variable kullanın.
- HTTPS ve güvenli Kafka/RabbitMQ bağlantılarını etkinleştirin.
- Gateway ve servislerin yetkilendirme politikalarını üretim gereksinimlerine göre gözden geçirin.

