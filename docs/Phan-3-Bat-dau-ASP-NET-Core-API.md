# Phần 3: Bắt đầu với ASP.NET Core API

⏱️ **Thời lượng:** 75 phút

## 📚 Nội dung

1. Giới thiệu ASP.NET Core Web API
2. Cấu trúc project Web API
3. Program.cs và Startup
4. Swagger UI
5. Dependency Injection (DI)
6. Controller và Routing
7. HTTP Methods (GET, POST, PUT, DELETE)
8. Thực hành: Xây dựng Product API

---

## 1. Giới thiệu ASP.NET Core Web API

### Web API là gì?

**Web API** (Application Programming Interface) là giao diện lập trình ứng dụng cho phép các ứng dụng giao tiếp với nhau qua HTTP/HTTPS.

### RESTful API

**REST (Representational State Transfer)** là kiến trúc phổ biến nhất cho Web API:

| HTTP Method | Mục đích | Ví dụ |
|-------------|----------|-------|
| GET | Lấy dữ liệu | GET /api/products |
| POST | Tạo mới | POST /api/products |
| PUT | Cập nhật toàn bộ | PUT /api/products/1 |
| PATCH | Cập nhật một phần | PATCH /api/products/1 |
| DELETE | Xóa | DELETE /api/products/1 |

### Status Codes phổ biến

- **200 OK** - Thành công
- **201 Created** - Tạo mới thành công
- **204 No Content** - Thành công nhưng không có dữ liệu trả về
- **400 Bad Request** - Yêu cầu không hợp lệ
- **401 Unauthorized** - Chưa xác thực
- **403 Forbidden** - Không có quyền truy cập
- **404 Not Found** - Không tìm thấy
- **500 Internal Server Error** - Lỗi server

---

## 2. Tạo Project Web API

### Tạo project bằng CLI

```bash
# Tạo solution
dotnet new sln -n ProductAPI

# Tạo Web API project
dotnet new webapi -n ProductAPI

# Thêm project vào solution
dotnet sln add ProductAPI/ProductAPI.csproj

# Chạy project
cd ProductAPI
dotnet run
```

### Tạo project bằng Visual Studio

1. File → New → Project
2. Chọn "ASP.NET Core Web API"
3. Đặt tên project: `ProductAPI`
4. Chọn .NET 8.0 (hoặc phiên bản mới nhất)
5. **Bỏ chọn** "Use controllers" nếu muốn dùng Minimal API
6. **Chọn** "Enable OpenAPI support" (Swagger)
7. Create

---

## 3. Cấu trúc Project Web API

```
ProductAPI/
├── Controllers/          # Chứa các API controllers
│   └── WeatherForecastController.cs
├── Models/              # Chứa các model/entity classes
├── Services/            # Business logic
├── Properties/
│   └── launchSettings.json
├── appsettings.json     # Cấu hình ứng dụng
├── appsettings.Development.json
├── Program.cs           # Entry point và configuration
└── ProductAPI.csproj    # Project file
```

---

## 4. Program.cs - Entry Point

### Program.cs (ASP.NET Core 6.0+)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Giải thích các thành phần:

#### **1. WebApplicationBuilder**
```csharp
var builder = WebApplication.CreateBuilder(args);
```
- Khởi tạo builder để cấu hình services và app

#### **2. AddControllers**
```csharp
builder.Services.AddControllers();
```
- Đăng ký controller services

#### **3. Swagger Configuration**
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```
- Cấu hình Swagger để tạo API documentation

#### **4. Build App**
```csharp
var app = builder.Build();
```
- Xây dựng application từ builder

#### **5. Middleware Pipeline**
```csharp
app.UseSwagger();        // Swagger JSON endpoint
app.UseSwaggerUI();      // Swagger UI
app.UseHttpsRedirection(); // Redirect HTTP → HTTPS
app.UseAuthorization();  // Authorization middleware
app.MapControllers();    // Map controller routes
```

---

## 5. Swagger UI

### Truy cập Swagger

Sau khi chạy ứng dụng:
```
https://localhost:5001/swagger
```

### Swagger làm gì?

- ✅ Tự động generate API documentation
- ✅ Hiển thị tất cả endpoints
- ✅ Test API trực tiếp từ browser
- ✅ Xem request/response schema
- ✅ Export OpenAPI specification

### Tùy chỉnh Swagger

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "API quản lý sản phẩm",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "email@example.com"
        }
    });
    
    // Thêm XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

---

## 6. Dependency Injection (DI)

### DI là gì?

**Dependency Injection** là design pattern cho phép "inject" dependencies vào class thay vì tạo trực tiếp trong class.

### Lợi ích của DI:

- ✅ Loose coupling
- ✅ Dễ test (mock dependencies)
- ✅ Dễ bảo trì và mở rộng
- ✅ Quản lý lifecycle tự động

### Service Lifetimes

```csharp
// Singleton - Một instance cho toàn ứng dụng
builder.Services.AddSingleton<ISingletonService, SingletonService>();

// Scoped - Một instance cho mỗi request
builder.Services.AddScoped<IScopedService, ScopedService>();

// Transient - Instance mới mỗi lần inject
builder.Services.AddTransient<ITransientService, TransientService>();
```

### Ví dụ về DI

```csharp
// Interface
public interface IProductService
{
    List<Product> GetAll();
    Product? GetById(int id);
    void Add(Product product);
}

// Implementation
public class ProductService : IProductService
{
    private static List<Product> _products = new();

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        _products.Add(product);
    }
}

// Đăng ký service trong Program.cs
builder.Services.AddScoped<IProductService, ProductService>();

// Inject vào Controller
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    // Constructor injection
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult<List<Product>> GetAll()
    {
        return Ok(_productService.GetAll());
    }
}
```

---

## 7. Controller và Routing

### Tạo Controller

```csharp
using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Actions here
    }
}
```

### Attributes quan trọng:

#### **[ApiController]**
- Tự động validate model
- Tự động trả về 400 Bad Request khi model không hợp lệ
- Binding source parameter inference

#### **[Route]**
```csharp
[Route("api/[controller]")]           // api/products
[Route("api/v1/[controller]")]        // api/v1/products
[Route("api/products")]               // api/products (hard-coded)
```

### Route Parameters

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    // GET: api/products
    [HttpGet]
    public ActionResult<List<Product>> GetAll() { }

    // GET: api/products/5
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id) { }

    // GET: api/products/category/electronics
    [HttpGet("category/{category}")]
    public ActionResult<List<Product>> GetByCategory(string category) { }

    // GET: api/products/search?name=laptop&minPrice=1000
    [HttpGet("search")]
    public ActionResult<List<Product>> Search([FromQuery] string name, [FromQuery] decimal minPrice) { }
}
```

---

## 8. HTTP Methods và Action Results

### GET - Lấy dữ liệu

```csharp
// GET: api/products
[HttpGet]
public ActionResult<List<Product>> GetAll()
{
    var products = _productService.GetAll();
    return Ok(products); // 200 OK
}

// GET: api/products/5
[HttpGet("{id}")]
public ActionResult<Product> GetById(int id)
{
    var product = _productService.GetById(id);
    
    if (product == null)
        return NotFound(); // 404 Not Found
    
    return Ok(product); // 200 OK
}
```

### POST - Tạo mới

```csharp
// POST: api/products
[HttpPost]
public ActionResult<Product> Create([FromBody] Product product)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // 400 Bad Request
    
    _productService.Add(product);
    
    // 201 Created với Location header
    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
}
```

### PUT - Cập nhật toàn bộ

```csharp
// PUT: api/products/5
[HttpPut("{id}")]
public ActionResult Update(int id, [FromBody] Product product)
{
    if (id != product.Id)
        return BadRequest(); // 400 Bad Request
    
    var existing = _productService.GetById(id);
    if (existing == null)
        return NotFound(); // 404 Not Found
    
    _productService.Update(product);
    
    return NoContent(); // 204 No Content
}
```

### DELETE - Xóa

```csharp
// DELETE: api/products/5
[HttpDelete("{id}")]
public ActionResult Delete(int id)
{
    var product = _productService.GetById(id);
    if (product == null)
        return NotFound(); // 404 Not Found
    
    _productService.Delete(id);
    
    return NoContent(); // 204 No Content
}
```

---

## 💻 Thực hành: Xây dựng Product API (In-Memory)

### Bước 1: Tạo Model

**Models/Product.cs**
```csharp
namespace ProductAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
```

### Bước 2: Tạo Service Interface

**Services/IProductService.cs**
```csharp
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product? GetById(int id);
        Product Create(Product product);
        bool Update(int id, Product product);
        bool Delete(int id);
        List<Product> GetByCategory(string category);
        List<Product> Search(string searchTerm);
    }
}
```

### Bước 3: Implement Service

**Services/ProductService.cs**
```csharp
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private static List<Product> _products = new List<Product>
        {
            new Product 
            { 
                Id = 1, 
                Name = "Laptop Dell XPS 15", 
                Description = "Laptop cao cấp cho lập trình viên",
                Price = 35000000, 
                Category = "Electronics", 
                Stock = 10 
            },
            new Product 
            { 
                Id = 2, 
                Name = "iPhone 15 Pro", 
                Description = "Điện thoại thông minh cao cấp",
                Price = 28000000, 
                Category = "Electronics", 
                Stock = 25 
            },
            new Product 
            { 
                Id = 3, 
                Name = "Bàn làm việc", 
                Description = "Bàn gỗ cao cấp",
                Price = 3500000, 
                Category = "Furniture", 
                Stock = 5 
            }
        };

        private static int _nextId = 4;

        public List<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public Product Create(Product product)
        {
            product.Id = _nextId++;
            product.CreatedDate = DateTime.Now;
            _products.Add(product);
            return product;
        }

        public bool Update(int id, Product product)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.Stock = product.Stock;

            return true;
        }

        public bool Delete(int id)
        {
            var product = GetById(id);
            if (product == null)
                return false;

            _products.Remove(product);
            return true;
        }

        public List<Product> GetByCategory(string category)
        {
            return _products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Product> Search(string searchTerm)
        {
            return _products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
```

### Bước 4: Tạo Controller

**Controllers/ProductsController.cs**
```csharp
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả sản phẩm
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> GetAll()
        {
            _logger.LogInformation("Getting all products");
            var products = _productService.GetAll();
            return Ok(products);
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> GetById(int id)
        {
            _logger.LogInformation("Getting product with ID: {Id}", id);
            var product = _productService.GetById(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            
            return Ok(product);
        }

        /// <summary>
        /// Lấy sản phẩm theo danh mục
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> GetByCategory(string category)
        {
            _logger.LogInformation("Getting products in category: {Category}", category);
            var products = _productService.GetByCategory(category);
            return Ok(products);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> Search([FromQuery] string term)
        {
            _logger.LogInformation("Searching products with term: {Term}", term);
            
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { message = "Search term is required" });
            
            var products = _productService.Search(term);
            return Ok(products);
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> Create([FromBody] Product product)
        {
            _logger.LogInformation("Creating new product: {Name}", product.Name);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var createdProduct = _productService.Create(product);
            
            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdProduct.Id }, 
                createdProduct
            );
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Update(int id, [FromBody] Product product)
        {
            _logger.LogInformation("Updating product with ID: {Id}", id);
            
            if (id != product.Id)
                return BadRequest(new { message = "ID mismatch" });
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var success = _productService.Update(id, product);
            
            if (!success)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            
            return NoContent();
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            _logger.LogInformation("Deleting product with ID: {Id}", id);
            
            var success = _productService.Delete(id);
            
            if (!success)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            
            return NoContent();
        }
    }
}
```

### Bước 5: Đăng ký Service trong Program.cs

```csharp
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Đăng ký ProductService
builder.Services.AddSingleton<IProductService, ProductService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Bước 6: Chạy và Test

```bash
dotnet run
```

Truy cập Swagger UI:
```
https://localhost:5001/swagger
```

### Test với Postman hoặc cURL

```bash
# GET all products
curl https://localhost:5001/api/products

# GET by ID
curl https://localhost:5001/api/products/1

# POST create
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mouse Logitech",
    "description": "Chuột gaming",
    "price": 500000,
    "category": "Electronics",
    "stock": 50
  }'

# PUT update
curl -X PUT https://localhost:5001/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Laptop Dell XPS 15 Updated",
    "description": "Laptop cao cấp",
    "price": 37000000,
    "category": "Electronics",
    "stock": 8
  }'

# DELETE
curl -X DELETE https://localhost:5001/api/products/1
```

---

## 📝 Tóm tắt

Trong phần này, chúng ta đã học:

✅ **Web API & REST:** Kiến trúc RESTful, HTTP methods, status codes  
✅ **Cấu trúc project:** Controllers, Models, Services  
✅ **Program.cs:** Configuration, middleware pipeline  
✅ **Swagger UI:** Auto-generate documentation, test API  
✅ **Dependency Injection:** Service lifetimes, constructor injection  
✅ **Controller:** Routing, attributes, action results  
✅ **CRUD Operations:** GET, POST, PUT, DELETE  
✅ **Thực hành:** Product API với in-memory storage  

---

## ➡️ Tiếp theo

Chuyển sang [Phần 4: Làm việc với dữ liệu & CRUD API](./Phan-4-Lam-viec-voi-du-lieu-CRUD-API.md) để tích hợp Entity Framework Core và Database!

---

## 🌐 Real-world API Examples

### Các API nổi tiếng xây dựng bằng ASP.NET Core:

1. **Stack Overflow API** - Q&A platform
2. **Bing APIs** - Microsoft search services
3. **Azure APIs** - Cloud services
4. **GitHub Webhook APIs** - Integration services

### Open Source Projects để học:

```bash
# Clone và học từ các project thực tế
git clone https://github.com/dotnet/eShopOnWeb
git clone https://github.com/jasontaylordev/CleanArchitecture
git clone https://github.com/dotnet-architecture/eShopOnContainers
```

## 💡 Best Practices cho Web API

### 1. API Naming Conventions:
```
✅ Good:
GET    /api/products
GET    /api/products/123
POST   /api/products
PUT    /api/products/123
DELETE /api/products/123

❌ Bad:
GET    /api/getProducts
POST   /api/createProduct
GET    /api/product/get/123
```

### 2. Versioning Strategy:
```csharp
// URL versioning
[Route("api/v1/[controller]")]
[Route("api/v2/[controller]")]

// Header versioning
[ApiVersion("1.0")]
[ApiVersion("2.0")]

// Query string versioning
// GET /api/products?api-version=1.0
```

### 3. Error Response Format:
```csharp
public class ApiError
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Detail { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Usage
return BadRequest(new ApiError 
{ 
    StatusCode = 400,
    Message = "Invalid request",
    Detail = "Product name is required"
});
```

## 📊 Performance Tips

### 1. Response Compression:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

app.UseResponseCompression();
```

### 2. Output Caching:
```csharp
builder.Services.AddOutputCache();

app.UseOutputCache();

// Controller
[OutputCache(Duration = 60)]
[HttpGet]
public async Task<ActionResult> GetProducts() { }
```

## 🎯 Testing với Thunder Client (VS Code)

Thunder Client là extension nhẹ hơn Postman:

```json
{
  "clientName": "Thunder Client",
  "collectionName": "Product API",
  "requests": [
    {
      "name": "Get All Products",
      "method": "GET",
      "url": "{{base_url}}/api/products"
    },
    {
      "name": "Create Product",
      "method": "POST",
      "url": "{{base_url}}/api/products",
      "body": {
        "name": "New Product",
        "price": 999.99
      }
    }
  ]
}
```

## 🔧 Useful NuGet Packages

```bash
# API Documentation
dotnet add package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Annotations

# Model Validation
dotnet add package FluentValidation.AspNetCore

# API Versioning
dotnet add package Microsoft.AspNetCore.Mvc.Versioning

# AutoMapper (DTO mapping)
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Health Checks
dotnet add package AspNetCore.HealthChecks.UI
```

## 📚 Case Studies

### Example: E-commerce API Architecture
```
┌─────────────────┐
│   Client Apps   │
│  (Web, Mobile)  │
└────────┬────────┘
         │
    ┌────▼─────┐
    │ API GW   │ (Gateway)
    └────┬─────┘
         │
    ┌────▼──────────┬──────────┬──────────┐
    │               │          │          │
┌───▼────┐  ┌──────▼───┐  ┌──▼────┐  ┌──▼────┐
│Product │  │ Order    │  │ User  │  │Payment│
│Service │  │ Service  │  │Service│  │Service│
└────────┘  └──────────┘  └───────┘  └───────┘
```

---

**📚 Tài liệu tham khảo:**

#### Official Documentation:
- [ASP.NET Core Web API Tutorial](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api)
- [Dependency Injection](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [Routing in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/routing)
- [Minimal APIs](https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis)

#### Video Tutorials:
- [ASP.NET Core Web API Full Course - freeCodeCamp](https://www.youtube.com/watch?v=_8nLSsK5NDo)
- [REST API Tutorial - Programming with Mosh](https://www.youtube.com/watch?v=fmvcAzHpsk8)
- [ASP.NET Core Series - IAmTimCorey](https://www.youtube.com/c/IAmTimCorey)

#### Books:
- **"Building Web APIs with ASP.NET Core"** by Valerio De Sanctis
- **"Pro ASP.NET Core 8"** by Adam Freeman
- **"ASP.NET Core in Action"** by Andrew Lock

#### Sample Projects:
- [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) - E-commerce reference
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Practical ASP.NET Core](https://github.com/dodyg/practical-aspnetcore)

#### API Design Guidelines:
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)
- [RESTful API Best Practices](https://restfulapi.net/)
- [HTTP Status Codes Reference](https://httpstatuses.com/)

#### Tools:
- [Postman](https://www.postman.com/) - API testing platform
- [Insomnia](https://insomnia.rest/) - API client
- [Thunder Client](https://www.thunderclient.com/) - VS Code extension
- [HTTPie](https://httpie.io/) - Command line HTTP client

