# Phần 4: Làm việc với dữ liệu & CRUD API

⏱️ **Thời lượng:** 75 phút

## 📚 Nội dung

1. Giới thiệu Entity Framework Core
2. Cài đặt EF Core packages
3. Tạo Model và DbContext
4. Connection String
5. Migration
6. CRUD Operations với Database
7. Repository Pattern (optional)
8. Thực hành: Product API với Database

---

## 1. Giới thiệu Entity Framework Core

### EF Core là gì?

**Entity Framework Core** là ORM (Object-Relational Mapper) cho .NET, cho phép:
- Làm việc với database bằng C# objects
- Tự động generate SQL queries
- Hỗ trợ nhiều database: SQL Server, PostgreSQL, MySQL, SQLite, v.v.

### Code First vs Database First

| Code First | Database First |
|------------|----------------|
| Tạo models trước, generate database | Database có sẵn, generate models |
| Dùng migrations | Dùng scaffolding |
| Phổ biến cho dự án mới | Dùng cho legacy database |

### Các thành phần chính:

- **DbContext:** Quản lý kết nối database và operations
- **DbSet:** Đại diện cho một bảng trong database
- **Migrations:** Quản lý schema changes
- **LINQ to Entities:** Query database bằng LINQ

---

## 2. Cài đặt EF Core Packages

### Packages cần thiết

```bash
# EF Core SQL Server provider
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# EF Core Tools (cho migrations)
dotnet add package Microsoft.EntityFrameworkCore.Tools

# EF Core Design (cho migrations)
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Dùng SQLite (nhẹ nhàng hơn cho demo)

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### Kiểm tra trong .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

</Project>
```

---

## 3. Tạo Model và DbContext

### Model với Data Annotations

**Models/Product.cs**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên phải từ 3-200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int Stock { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
```

### Data Annotations phổ biến:

| Annotation | Mục đích |
|------------|----------|
| `[Key]` | Khóa chính |
| `[Required]` | Bắt buộc (NOT NULL) |
| `[StringLength]` | Độ dài chuỗi |
| `[Range]` | Giới hạn giá trị |
| `[Column]` | Tên và kiểu cột |
| `[Table]` | Tên bảng |
| `[ForeignKey]` | Khóa ngoại |
| `[NotMapped]` | Không map vào database |

### Tạo DbContext

**Data/AppDbContext.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

namespace ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet cho mỗi entity
        public DbSet<Product> Products { get; set; }

        // Fluent API configuration (tùy chọn)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Product
            modelBuilder.Entity<Product>(entity =>
            {
                // Index
                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.Category);

                // Default values
                entity.Property(p => p.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");

                // Precision
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop Dell XPS 15",
                    Description = "Laptop cao cấp cho lập trình viên",
                    Price = 35000000,
                    Category = "Electronics",
                    Stock = 10,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "iPhone 15 Pro",
                    Description = "Điện thoại thông minh cao cấp",
                    Price = 28000000,
                    Category = "Electronics",
                    Stock = 25,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "Bàn làm việc",
                    Description = "Bàn gỗ cao cấp",
                    Price = 3500000,
                    Category = "Furniture",
                    Stock = 5,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
```

---

## 4. Connection String

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ProductDB.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### SQL Server Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Đăng ký DbContext trong Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hoặc dùng SQL Server:
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký ProductService
builder.Services.AddScoped<IProductService, ProductService>();

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

---

## 5. Migration

### Tạo Migration

```bash
# Tạo migration đầu tiên
dotnet ef migrations add InitialCreate

# Tạo migration sau khi thay đổi model
dotnet ef migrations add AddUpdatedDateToProduct
```

### Apply Migration (tạo database)

```bash
# Áp dụng migrations vào database
dotnet ef database update

# Áp dụng đến migration cụ thể
dotnet ef database update InitialCreate

# Rollback về migration trước
dotnet ef database update PreviousMigrationName
```

### Xóa Migration

```bash
# Xóa migration chưa apply
dotnet ef migrations remove

# Xóa migration đã apply (phải revert database trước)
dotnet ef database update PreviousMigration
dotnet ef migrations remove
```

### Xem SQL Script

```bash
# Xem SQL sẽ được execute
dotnet ef migrations script

# Generate SQL từ migration cụ thể
dotnet ef migrations script FromMigration ToMigration
```

### Migration File Structure

**Migrations/20240101000000_InitialCreate.cs**
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                Description = table.Column<string>(maxLength: 1000, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Category = table.Column<string>(maxLength: 100, nullable: false),
                Stock = table.Column<int>(nullable: false),
                CreatedDate = table.Column<DateTime>(nullable: false),
                UpdatedDate = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Products");
    }
}
```

---

## 6. CRUD Operations với Database

### Update ProductService để dùng EF Core

**Services/ProductService.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            _logger.LogInformation("Getting all products from database");
            return await _context.Products
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting product with ID: {Id}", id);
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            _logger.LogInformation("Getting products in category: {Category}", category);
            return await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(string searchTerm)
        {
            _logger.LogInformation("Searching products with term: {Term}", searchTerm);
            
            return await _context.Products
                .Where(p => p.Name.Contains(searchTerm) || 
                           p.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _logger.LogInformation("Creating new product: {Name}", product.Name);
            
            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            _logger.LogInformation("Updating product with ID: {Id}", id);
            
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return false;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.Stock = product.Stock;
            existing.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting product with ID: {Id}", id);
            
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Products
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
```

### Update Interface

**Services/IProductService.cs**
```csharp
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByCategoryAsync(string category);
        Task<List<Product>> SearchAsync(string searchTerm);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<List<Product>> GetPagedAsync(int page, int pageSize);
    }
}
```

### Update Controller để dùng async

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
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Lấy sản phẩm với phân trang
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(new { message = "Page và PageSize phải lớn hơn 0" });

            var products = await _productService.GetPagedAsync(page, pageSize);
            var totalCount = await _productService.GetTotalCountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return Ok(new
            {
                data = products,
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    hasNextPage = page < totalPages,
                    hasPreviousPage = page > 1
                }
            });
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });
            
            return Ok(product);
        }

        /// <summary>
        /// Lấy sản phẩm theo danh mục
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Product>>> GetByCategory(string category)
        {
            var products = await _productService.GetByCategoryAsync(category);
            return Ok(products);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Product>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { message = "Search term is required" });
            
            var products = await _productService.SearchAsync(term);
            return Ok(products);
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var createdProduct = await _productService.CreateAsync(product);
            
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
        public async Task<ActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest(new { message = "ID mismatch" });
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var success = await _productService.UpdateAsync(id, product);
            
            if (!success)
                return NotFound(new { message = $"Product with ID {id} not found" });
            
            return NoContent();
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            
            if (!success)
                return NotFound(new { message = $"Product with ID {id} not found" });
            
            return NoContent();
        }
    }
}
```

---

## 7. Repository Pattern (Optional)

### Tại sao dùng Repository Pattern?

- ✅ Tách biệt data access logic
- ✅ Dễ test (mock repository)
- ✅ Tái sử dụng code
- ✅ Thay đổi data source dễ dàng

### Generic Repository

**Repositories/IRepository.cs**
```csharp
using System.Linq.Expressions;

namespace ProductAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
```

**Repositories/Repository.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using System.Linq.Expressions;

namespace ProductAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
```

---

## 💻 Bài tập thực hành hoàn chỉnh

### Các bước thực hiện:

```bash
# 1. Cài đặt packages
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

# 2. Tạo Migration
dotnet ef migrations add InitialCreate

# 3. Apply Migration (tạo database)
dotnet ef database update

# 4. Chạy ứng dụng
dotnet run

# 5. Test API qua Swagger
# Truy cập: https://localhost:5001/swagger
```

### Test scenarios:

1. **GET all products** - Xem danh sách sản phẩm seed data
2. **GET by ID** - Lấy sản phẩm có ID = 1
3. **POST create** - Tạo sản phẩm mới
4. **PUT update** - Cập nhật sản phẩm vừa tạo
5. **DELETE** - Xóa sản phẩm
6. **GET with pagination** - Test phân trang
7. **SEARCH** - Tìm kiếm theo tên

---

## 📝 Tóm tắt

Trong phần này, chúng ta đã học:

✅ **Entity Framework Core:** ORM cho .NET  
✅ **DbContext:** Quản lý database operations  
✅ **Data Annotations:** Validation và database configuration  
✅ **Connection String:** Kết nối với database  
✅ **Migrations:** Quản lý database schema  
✅ **Async/Await:** Non-blocking database operations  
✅ **CRUD với Database:** Create, Read, Update, Delete  
✅ **Pagination:** Phân trang kết quả  
✅ **Repository Pattern:** Tách biệt data access logic  

---

## ➡️ Tiếp theo

Chuyển sang [Phần 5: Hoàn thiện & Kiểm thử API](./Phan-5-Hoan-thien-Kiem-thu-API.md) để học về validation, middleware, và authentication!

---

## 🎯 Real-world Database Scenarios

### Connection Strings cho các Database phổ biến:

```json
{
  "ConnectionStrings": {
    // SQL Server
    "SqlServer": "Server=localhost;Database=ProductDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True",
    
    // PostgreSQL
    "PostgreSQL": "Host=localhost;Database=ProductDB;Username=postgres;Password=password",
    
    // MySQL
    "MySQL": "Server=localhost;Database=ProductDB;User=root;Password=password;",
    
    // SQLite (Local file)
    "SQLite": "Data Source=ProductDB.db",
    
    // Azure SQL
    "AzureSQL": "Server=tcp:yourserver.database.windows.net,1433;Database=ProductDB;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

### Install Providers:

```bash
# SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql

# SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# In-Memory (Testing)
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

## 💡 EF Core Best Practices

### 1. N+1 Query Problem:

```csharp
// ❌ Bad - N+1 queries
var orders = context.Orders.ToList();
foreach (var order in orders)
{
    var customer = context.Customers.Find(order.CustomerId); // N queries
}

// ✅ Good - Single query with Include
var orders = context.Orders
    .Include(o => o.Customer)
    .ToList();
```

### 2. AsNoTracking for Read-Only Queries:

```csharp
// ❌ Bad - Tracking entities unnecessarily
var products = context.Products.ToList();

// ✅ Good - No tracking for better performance
var products = context.Products.AsNoTracking().ToList();
```

### 3. Pagination Best Practice:

```csharp
public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize)
{
    var query = _context.Products.AsNoTracking();
    
    var totalCount = await query.CountAsync();
    var items = await query
        .OrderBy(p => p.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Product>
    {
        Items = items,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
```

### 4. Soft Delete Pattern:

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

// Global query filter
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => !p.IsDeleted);

// Soft delete
public async Task SoftDeleteAsync(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product != null)
    {
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
```

## 📊 Performance Optimization

### 1. Index Your Queries:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        // Single column index
        entity.HasIndex(p => p.Name);
        
        // Composite index
        entity.HasIndex(p => new { p.Category, p.Price });
        
        // Unique index
        entity.HasIndex(p => p.Sku).IsUnique();
    });
}
```

### 2. Select Only What You Need:

```csharp
// ❌ Bad - Loading entire entity
var products = await _context.Products
    .Where(p => p.Category == "Electronics")
    .ToListAsync();

// ✅ Good - Projection
var products = await _context.Products
    .Where(p => p.Category == "Electronics")
    .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
    .ToListAsync();
```

### 3. Compiled Queries:

```csharp
private static readonly Func<AppDbContext, int, Task<Product>> _getProductById =
    EF.CompileAsyncQuery((AppDbContext context, int id) =>
        context.Products.FirstOrDefault(p => p.Id == id));

public async Task<Product?> GetByIdOptimizedAsync(int id)
{
    return await _getProductById(_context, id);
}
```

## 🔧 Advanced Features

### 1. Transactions:

```csharp
public async Task TransferInventoryAsync(int fromProductId, int toProductId, int quantity)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        var fromProduct = await _context.Products.FindAsync(fromProductId);
        var toProduct = await _context.Products.FindAsync(toProductId);
        
        fromProduct.Stock -= quantity;
        toProduct.Stock += quantity;
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 2. Raw SQL Queries:

```csharp
// Execute raw SQL
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 1000)
    .ToListAsync();

// Stored procedure
var result = await _context.Products
    .FromSqlRaw("EXEC GetProductsByCategory @Category", 
        new SqlParameter("@Category", "Electronics"))
    .ToListAsync();
```

### 3. Global Query Filters:

```csharp
public class AppDbContext : DbContext
{
    private readonly string _tenantId;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Multi-tenancy filter
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.TenantId == _tenantId);
    }
}
```

## 🎓 Learning Resources

### Sample Projects:
```bash
# Clean Architecture với EF Core
git clone https://github.com/jasontaylordev/CleanArchitecture

# eShop với Microservices
git clone https://github.com/dotnet-architecture/eShopOnContainers

# Real World App
git clone https://github.com/gothinkster/aspnetcore-realworld-example-app
```

---

**📚 Tài liệu tham khảo:**

#### Official Documentation:
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Migrations Overview](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [DbContext Lifetime](https://docs.microsoft.com/ef/core/dbcontext-configuration/)
- [Performance Best Practices](https://docs.microsoft.com/ef/core/performance/)

#### Video Tutorials:
- [Entity Framework Core Full Course - freeCodeCamp](https://www.youtube.com/watch?v=SryQxUeChMc)
- [EF Core Tutorial - Kudvenkat](https://www.youtube.com/playlist?list=PL6n9fhu94yhV4axZz0CzQiLwAZLuqb-Np)
- [Advanced EF Core - Julie Lerman](https://www.pluralsight.com/courses/entity-framework-core-6-fundamentals)

#### Books:
- **"Entity Framework Core in Action"** by Jon P Smith
- **"Programming Entity Framework Core"** by Julia Lerman
- **"Mastering Entity Framework Core 2.0"** by Prabhakaran Anbazhagan

#### Tools:
- [EF Core Power Tools](https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools) - VS Extension
- [LINQPad](https://www.linqpad.net/) - Test EF queries
- [Azure Data Studio](https://azure.microsoft.com/products/data-studio/) - Database management
- [SQL Server Management Studio](https://docs.microsoft.com/sql/ssms/) - SSMS

#### Blogs & Articles:
- [Julie Lerman's Blog](https://thedatafarm.com/blog/)
- [Jon P Smith's Blog](https://www.thereformedprogrammer.net/)
- [EF Core Weekly](https://efcoreweekly.com/) - Newsletter

#### Performance:
- [EF Core Performance Best Practices](https://docs.microsoft.com/ef/core/performance/)
- [Benchmark.NET](https://benchmarkdotnet.org/) - Performance testing
- [MiniProfiler](https://miniprofiler.com/) - Database profiling

