# Phần 5: Hoàn thiện & Kiểm thử API

⏱️ **Thời lượng:** 45 phút

## 📚 Nội dung

1. Test API với Postman
2. Test API với Swagger
3. Data Validation nâng cao
4. Error Handling & Exception Middleware
5. Logging
6. CORS Configuration
7. Authentication & Authorization (Khái niệm)
8. Best Practices
9. Tổng kết & Định hướng

---

## 1. Test API với Postman

### Cài đặt Postman

Download: https://www.postman.com/downloads/

### Tạo Collection

1. Mở Postman
2. Click **New** → **Collection**
3. Đặt tên: `Product API`
4. Thêm base URL variable: `{{baseUrl}}` = `https://localhost:5001`

### Test các endpoints:

#### 1. GET All Products

```
Method: GET
URL: {{baseUrl}}/api/products
Headers:
  Content-Type: application/json
```

#### 2. GET Product by ID

```
Method: GET
URL: {{baseUrl}}/api/products/1
```

#### 3. POST Create Product

```
Method: POST
URL: {{baseUrl}}/api/products
Headers:
  Content-Type: application/json
Body (raw JSON):
{
  "name": "Gaming Mouse",
  "description": "High DPI gaming mouse",
  "price": 750000,
  "category": "Electronics",
  "stock": 50
}
```

#### 4. PUT Update Product

```
Method: PUT
URL: {{baseUrl}}/api/products/4
Headers:
  Content-Type: application/json
Body (raw JSON):
{
  "id": 4,
  "name": "Gaming Mouse Pro",
  "description": "High DPI professional gaming mouse",
  "price": 850000,
  "category": "Electronics",
  "stock": 45
}
```

#### 5. DELETE Product

```
Method: DELETE
URL: {{baseUrl}}/api/products/4
```

#### 6. GET with Pagination

```
Method: GET
URL: {{baseUrl}}/api/products/paged?page=1&pageSize=5
```

#### 7. Search Products

```
Method: GET
URL: {{baseUrl}}/api/products/search?term=laptop
```

### Postman Tests Scripts

Thêm test scripts để tự động validate responses:

```javascript
// Test GET All Products
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response is an array", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.be.an('array');
});

pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

// Test POST Create
pm.test("Status code is 201", function () {
    pm.response.to.have.status(201);
});

pm.test("Product created has ID", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
    // Save ID for later use
    pm.environment.set("productId", jsonData.id);
});
```

### Export/Import Collection

- Export: Click **...** → **Export** → Chọn Collection v2.1
- Import: Chia sẻ file JSON với team

---

## 2. Test API với Swagger

### Truy cập Swagger UI

```
https://localhost:5001/swagger
```

### Các tính năng:

1. **Explore endpoints** - Xem tất cả API
2. **Try it out** - Test trực tiếp
3. **View schemas** - Xem model structure
4. **Response examples** - Xem response mẫu

### Tùy chỉnh Swagger

**Program.cs**
```csharp
using Microsoft.OpenApi.Models;
using System.Reflection;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "API để quản lý sản phẩm - Demo cho khóa học ASP.NET Core",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com",
            Url = new Uri("https://yourwebsite.com")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Thêm XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Authorization support (nếu có JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

### Enable XML Documentation

**ProductAPI.csproj**
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

---

## 3. Data Validation nâng cao

### Custom Validation Attributes

**Attributes/FutureDateAttribute.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Attributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime <= DateTime.Now)
                {
                    return new ValidationResult("Ngày phải là ngày trong tương lai");
                }
            }
            return ValidationResult.Success;
        }
    }
}
```

### DTOs (Data Transfer Objects)

**DTOs/CreateProductDto.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên phải từ 3-200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không quá 1000 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(1000, 1000000000, ErrorMessage = "Giá phải từ 1,000 đến 1,000,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không thể âm")]
        public int Stock { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1000, 1000000000)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
```

### Custom Validation in Controller

```csharp
[HttpPost]
public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
{
    // Custom validation
    if (await _productService.ExistsByNameAsync(dto.Name))
    {
        ModelState.AddModelError("Name", "Sản phẩm với tên này đã tồn tại");
        return BadRequest(ModelState);
    }

    // Map DTO to Model
    var product = new Product
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        Category = dto.Category,
        Stock = dto.Stock
    };

    var createdProduct = await _productService.CreateAsync(product);
    return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
}
```

---

## 4. Error Handling & Exception Middleware

### Global Exception Handler

**Middleware/ExceptionMiddleware.cs**
```csharp
using System.Net;
using System.Text.Json;

namespace ProductAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
```

### Đăng ký Middleware

**Program.cs**
```csharp
using ProductAPI.Middleware;

var app = builder.Build();

// Global exception handler
app.UseMiddleware<ExceptionMiddleware>();

// ... other middleware
```

### Custom Exceptions

**Exceptions/ProductNotFoundException.cs**
```csharp
namespace ProductAPI.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(int id) 
            : base($"Sản phẩm với ID {id} không tồn tại")
        {
        }
    }

    public class ProductAlreadyExistsException : Exception
    {
        public ProductAlreadyExistsException(string name) 
            : base($"Sản phẩm '{name}' đã tồn tại")
        {
        }
    }
}
```

---

## 5. Logging

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Logging in Controller

```csharp
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);
        
        try
        {
            var product = await _productService.GetByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound();
            }
            
            _logger.LogInformation("Successfully retrieved product: {ProductName}", product.Name);
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            throw;
        }
    }
}
```

### Serilog (Advanced Logging)

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

**Program.cs**
```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog();
    
    // ... rest of configuration
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
```

---

## 6. CORS Configuration

### Enable CORS

**Program.cs**
```csharp
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://myapp.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// ... rest of middleware
```

---

## 7. Authentication & Authorization (Khái niệm)

### JWT Authentication (Giới thiệu)

**JWT (JSON Web Token)** là phương pháp xác thực phổ biến cho API.

### Flow:

1. User đăng nhập → Server tạo JWT token
2. Client lưu token (localStorage/cookie)
3. Mỗi request gửi token trong header: `Authorization: Bearer <token>`
4. Server validate token → Cho phép/Từ chối request

### Cài đặt cơ bản:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Program.cs**
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
```

### Protect Endpoints

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize] // Yêu cầu authentication
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [AllowAnonymous] // Cho phép truy cập không cần đăng nhập
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll() { }

    [Authorize(Roles = "Admin")] // Chỉ Admin
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id) { }
}
```

---

## 8. Best Practices

### 1. API Versioning

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
```

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controller
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase { }
```

### 2. Response Caching

```csharp
builder.Services.AddResponseCaching();

app.UseResponseCaching();

// Controller
[HttpGet]
[ResponseCache(Duration = 60)] // Cache 60 seconds
public async Task<ActionResult<List<Product>>> GetAll() { }
```

### 3. Rate Limiting

```bash
dotnet add package AspNetCoreRateLimit
```

### 4. Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

app.MapHealthChecks("/health");
```

### 5. API Documentation

- ✅ Sử dụng XML comments
- ✅ Cung cấp ví dụ requests/responses
- ✅ Document error codes
- ✅ Versioning API

### 6. Security

- ✅ Luôn dùng HTTPS
- ✅ Validate tất cả inputs
- ✅ Implement authentication/authorization
- ✅ Rate limiting để chống DOS
- ✅ Không expose sensitive data trong errors

### 7. Performance

- ✅ Dùng async/await
- ✅ Implement caching
- ✅ Pagination cho large datasets
- ✅ Index database columns thường query
- ✅ Dùng Select() để chỉ lấy fields cần thiết

---

## 9. Tổng kết

### 🎉 Chúc mừng bạn đã hoàn thành khóa học!

### Những gì bạn đã học:

#### **Phần 1 - C# Cơ bản:**
- ✅ Cú pháp C#, kiểu dữ liệu, biến
- ✅ Câu lệnh điều kiện, vòng lặp
- ✅ Methods, classes, objects

#### **Phần 2 - C# Nâng cao:**
- ✅ OOP (Đóng gói, Kế thừa, Đa hình)
- ✅ Interface, Abstract class
- ✅ Collections & LINQ
- ✅ Exception Handling

#### **Phần 3 - ASP.NET Core API:**
- ✅ Cấu trúc Web API project
- ✅ Controllers, Routing
- ✅ HTTP Methods
- ✅ Dependency Injection
- ✅ Swagger

#### **Phần 4 - Entity Framework Core:**
- ✅ DbContext, DbSet
- ✅ Migrations
- ✅ CRUD operations
- ✅ Async/Await
- ✅ Repository Pattern

#### **Phần 5 - Testing & Best Practices:**
- ✅ Postman, Swagger testing
- ✅ Data validation
- ✅ Error handling
- ✅ Logging
- ✅ CORS
- ✅ Authentication/Authorization concepts

---

## 10. Định hướng học tiếp

### Nâng cao ASP.NET Core:

1. **Authentication & Authorization**
   - JWT Implementation
   - Identity Framework
   - OAuth 2.0, OpenID Connect

2. **Advanced EF Core**
   - Complex queries
   - Performance optimization
   - Transactions
   - Raw SQL

3. **Microservices**
   - Docker
   - Kubernetes
   - Message queues (RabbitMQ)
   - gRPC

4. **Testing**
   - Unit Testing (xUnit, NUnit)
   - Integration Testing
   - Mocking (Moq)

5. **CI/CD**
   - GitHub Actions
   - Azure DevOps
   - Docker deployment

6. **Cloud Platforms**
   - Azure App Service
   - AWS Lambda
   - Google Cloud

### Tài nguyên học thêm:

📚 **Documentation:**
- [Microsoft Docs - ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

🎥 **Video Courses:**
- Pluralsight: ASP.NET Core Path
- Udemy: Complete ASP.NET Core courses
- Microsoft Learn

📖 **Books:**
- "Pro ASP.NET Core" by Adam Freeman
- "C# in Depth" by Jon Skeet
- "Clean Architecture" by Robert C. Martin

👥 **Communities:**
- Stack Overflow
- Reddit: r/dotnet, r/csharp
- Discord: C# Community

---

## 💻 Project Ideas để thực hành

### Beginner:
1. **Todo API** - CRUD cho todo items
2. **Blog API** - Posts, comments, categories
3. **Contact API** - Contact management

### Intermediate:
4. **E-commerce API** - Products, orders, cart
5. **Library Management** - Books, members, borrowing
6. **Restaurant API** - Menu, orders, reservations

### Advanced:
7. **Social Media API** - Users, posts, likes, follows
8. **Banking API** - Accounts, transactions, transfers
9. **Booking System** - Hotels, rooms, reservations

---

## 📋 Checklist hoàn thành

Kiểm tra xem bạn đã nắm vững:

- [ ] Viết được chương trình C# cơ bản
- [ ] Hiểu và áp dụng OOP
- [ ] Tạo được ASP.NET Core Web API project
- [ ] Implement CRUD operations
- [ ] Sử dụng Entity Framework Core
- [ ] Tạo và apply migrations
- [ ] Test API với Postman/Swagger
- [ ] Implement validation
- [ ] Handle errors properly
- [ ] Logging trong ứng dụng
- [ ] Hiểu cơ bản về authentication

---

## 🎯 Next Steps

1. **Review:** Xem lại code đã viết, hiểu rõ từng phần
2. **Practice:** Làm lại từ đầu không nhìn tài liệu
3. **Build:** Tạo project riêng theo ý tưởng của bạn
4. **Learn:** Tìm hiểu các chủ đề nâng cao
5. **Share:** Chia sẻ kiến thức với người khác

---

## 🙏 Lời kết

Cảm ơn bạn đã theo dõi khóa học! Lập trình là hành trình học hỏi không ngừng. Hãy tiếp tục thực hành và khám phá.

**Chúc bạn thành công trên con đường trở thành ASP.NET Core Developer! 🚀**

---

**📞 Liên hệ & Hỗ trợ:**
- Email: your.email@example.com
- GitHub: github.com/yourusername
- Website: yourwebsite.com

---

## 🛡️ Security Best Practices

### 1. Input Sanitization:
```csharp
using System.Text.RegularExpressions;

public class InputValidator
{
    public static string SanitizeInput(string input)
    {
        // Remove potential SQL injection characters
        return Regex.Replace(input, @"[^\w\s@.-]", "");
    }
    
    public static bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}
```

### 2. Rate Limiting Example:
```csharp
// Nuget: AspNetCoreRateLimit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        }
    };
});
```

### 3. API Keys Authentication:
```csharp
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration config)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        var apiKey = config.GetValue<string>("ApiKey");
        if (!apiKey.Equals(providedKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        await _next(context);
    }
}
```

## 📊 Monitoring & Application Insights

### Azure Application Insights:
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]);

// Custom telemetry
public class ProductService
{
    private readonly TelemetryClient _telemetry;
    
    public ProductService(TelemetryClient telemetry)
    {
        _telemetry = telemetry;
    }
    
    public async Task<Product> CreateAsync(Product product)
    {
        _telemetry.TrackEvent("ProductCreated", 
            new Dictionary<string, string> { 
                { "ProductId", product.Id.ToString() },
                { "Category", product.Category }
            });
        
        // ... creation logic
    }
}
```

## 🎓 Certification & Career Path

### Microsoft Certifications:
- **AZ-204**: Developing Solutions for Microsoft Azure
- **AZ-400**: DevOps Engineer Expert
- **PL-400**: Microsoft Power Platform Developer

### Career Roadmap:
```
Junior Developer
    ↓
Mid-Level Developer (2-3 years)
    ↓
Senior Developer (5+ years)
    ↓
Lead Developer / Architect (7+ years)
    ↓
Engineering Manager / Principal Engineer
```

### Skills to Master:
1. ✅ C# & .NET fundamentals
2. ✅ ASP.NET Core Web API
3. ✅ Entity Framework Core
4. ✅ SQL & Database design
5. ⬜ Microservices architecture
6. ⬜ Docker & Kubernetes
7. ⬜ Cloud platforms (Azure/AWS)
8. ⬜ CI/CD pipelines
9. ⬜ Testing (Unit, Integration, E2E)
10. ⬜ Design patterns & Clean architecture

## 🌟 Real-World Success Stories

### Case Study: Stack Overflow Migration
> Stack Overflow chuyển từ .NET Framework sang .NET Core và đạt được:
> - **50% giảm response time**
> - **30% giảm server costs**
> - **Cross-platform deployment**

### Case Study: Azure DevOps
> Microsoft Azure DevOps được xây dựng hoàn toàn trên ASP.NET Core với:
> - **99.9% uptime SLA**
> - **Millions of requests per day**
> - **Global distribution**

## 💡 Pro Tips

### 1. Use Environment-Specific Settings:
```json
// appsettings.Development.json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}

// appsettings.Production.json
{
  "DetailedErrors": false,
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### 2. Structured Logging với Serilog:
```csharp
Log.Information("User {UserId} created product {ProductId} in {ElapsedMs}ms", 
    userId, productId, elapsed);
```

### 3. Health Checks Dashboard:
```csharp
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI();
```

## 📚 Additional Resources

### GitHub Repositories để Follow:
- [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) - ASP.NET Core source
- [dotnet/efcore](https://github.com/dotnet/efcore) - EF Core source
- [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
- [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)

### Podcasts:
- [.NET Rocks!](https://www.dotnetrocks.com/)
- [The .NET Core Podcast](https://dotnetcore.show/)
- [Merge Conflict](https://www.mergeconflict.fm/)

### YouTube Channels:
- [dotNET](https://www.youtube.com/c/dotNET) - Official Microsoft
- [IAmTimCorey](https://www.youtube.com/c/IAmTimCorey)
- [Nick Chapsas](https://www.youtube.com/c/Elfocrash)
- [Raw Coding](https://www.youtube.com/c/RawCoding)
- [Milan Jovanović](https://www.youtube.com/c/MilanJovanovicTech)

### Newsletters:
- [ASP.NET Community Standup](https://dotnet.microsoft.com/live/aspnet-community-standup)
- [.NET Weekly](https://www.dotnetweekly.com/)
- [C# Digest](https://csharpdigest.net/)

### Twitter Accounts to Follow:
- [@davidfowl](https://twitter.com/davidfowl) - ASP.NET Core Architect
- [@Nick_Craver](https://twitter.com/Nick_Craver) - Stack Overflow Architecture
- [@shanselman](https://twitter.com/shanselman) - Scott Hanselman
- [@dotnet](https://twitter.com/dotnet) - Official .NET

## 🎯 Practice Projects (Theo level)

### Beginner Level:
1. **Personal Blog API** - Posts, comments, tags
2. **Weather Dashboard** - Consuming external APIs
3. **Task Manager** - Todo app with categories

### Intermediate Level:
4. **E-Learning Platform** - Courses, lessons, enrollments
5. **Inventory Management** - Products, warehouses, stock
6. **Recipe Sharing** - Recipes, ingredients, ratings

### Advanced Level:
7. **Social Network** - Users, posts, likes, follows, messages
8. **E-commerce Platform** - Products, orders, payments, shipping
9. **Real-time Chat** - SignalR, WebSockets, notifications

### Expert Level:
10. **Multi-tenant SaaS** - Organizations, subscriptions, billing
11. **Microservices Platform** - Multiple services, API Gateway, Event Bus
12. **CI/CD Platform** - Build pipelines, deployments, monitoring

---

**📚 Tài liệu tham khảo:**

#### Official Documentation:
- [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/fundamentals/best-practices)
- [API Design Guidelines](https://docs.microsoft.com/azure/architecture/best-practices/api-design)
- [Security Best Practices](https://docs.microsoft.com/aspnet/core/security/)

#### Testing:
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions](https://fluentassertions.com/)
- [Integration Testing](https://docs.microsoft.com/aspnet/core/test/integration-tests)

#### DevOps:
- [GitHub Actions for .NET](https://docs.github.com/actions/guides/building-and-testing-net)
- [Azure DevOps Pipelines](https://docs.microsoft.com/azure/devops/pipelines/)
- [Docker for .NET](https://docs.docker.com/samples/dotnetcore/)

#### Performance:
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [MiniProfiler](https://miniprofiler.com/)
- [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/asp-net-core)

#### Architecture:
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)

