# Todo List API

Một API đơn giản để quản lý danh sách công việc (Todo List) được xây dựng bằng ASP.NET Core 9.0.

## 🚀 Tính năng

- ✅ **GET** `/api/todos` - Lấy danh sách tất cả công việc
- ✅ **GET** `/api/todos/{id}` - Lấy thông tin một công việc theo ID
- ✅ **POST** `/api/todos` - Thêm công việc mới
- ✅ **PUT** `/api/todos/{id}` - Đánh dấu hoàn thành/chưa hoàn thành
- ✅ **DELETE** `/api/todos/{id}` - Xóa công việc
- ✅ **GET** `/api/todos/stats` - Lấy thống kê công việc

## 🛠️ Công nghệ sử dụng

- **ASP.NET Core 9.0** - Web API framework
- **Swagger/OpenAPI** - API documentation và testing
- **C#** - Ngôn ngữ lập trình
- **In-memory storage** - Lưu trữ dữ liệu tạm thời

## 📋 Cấu trúc Project

```
demo-api-csharp/
├── Controllers/
│   └── TodoController.cs      # API Controller
├── Models/
│   └── Todo.cs                # Todo model
├── Program.cs                 # Main application
├── Properties/
│   └── launchSettings.json   # Launch configuration
└── README.md                  # Documentation
```

## 🚀 Cách chạy

### Yêu cầu
- .NET 9.0 SDK
- Visual Studio 2022 hoặc VS Code

### Bước 1: Clone repository
```bash
git clone https://github.com/manhnguyenkhac/demo-api-csharp.git
cd demo-api-csharp
```

### Bước 2: Restore packages
```bash
dotnet restore
```

### Bước 3: Chạy ứng dụng
```bash
dotnet run --launch-profile https
```

### Bước 4: Truy cập Swagger UI
- **HTTPS:** https://localhost:7110/swagger
- **HTTP:** http://localhost:5070/swagger

## 📖 API Documentation

### 1. Lấy danh sách công việc
```http
GET /api/todos
```

**Response:**
```json
[
  {
    "id": 1,
    "title": "Học ASP.NET Core",
    "description": "Tìm hiểu về Web API",
    "isCompleted": false,
    "createdAt": "2024-10-22T10:00:00",
    "completedAt": null
  }
]
```

### 2. Lấy thông tin một công việc
```http
GET /api/todos/{id}
```

### 3. Thêm công việc mới
```http
POST /api/todos
Content-Type: application/json

{
  "title": "Tên công việc",
  "description": "Mô tả công việc (tùy chọn)"
}
```

### 4. Cập nhật trạng thái hoàn thành
```http
PUT /api/todos/{id}
```

### 5. Xóa công việc
```http
DELETE /api/todos/{id}
```

### 6. Lấy thống kê
```http
GET /api/todos/stats
```

**Response:**
```json
{
  "total": 10,
  "completed": 3,
  "pending": 7,
  "completionRate": 30.0
}
```

## 🧪 Test API

### Sử dụng Swagger UI
1. Mở https://localhost:7110/swagger
2. Click "Try it out" trên endpoint muốn test
3. Nhập thông tin và click "Execute"

### Sử dụng cURL
```bash
# Lấy danh sách
curl -X GET "https://localhost:7110/api/todos"

# Thêm mới
curl -X POST "https://localhost:7110/api/todos" \
  -H "Content-Type: application/json" \
  -d '{"title": "Test Todo", "description": "Test Description"}'

# Cập nhật trạng thái
curl -X PUT "https://localhost:7110/api/todos/1"

# Xóa
curl -X DELETE "https://localhost:7110/api/todos/1"
```

### Sử dụng PowerShell
```powershell
# Lấy danh sách
Invoke-RestMethod -Uri "https://localhost:7110/api/todos" -Method GET

# Thêm mới
$body = @{
    title = "Test Todo"
    description = "Test Description"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7110/api/todos" -Method POST -Body $body -ContentType "application/json"
```

## 📊 Model Schema

### Todo Model
```csharp
public class Todo
{
    public int Id { get; set; }                    // ID duy nhất
    public string Title { get; set; }             // Tiêu đề công việc
    public string Description { get; set; }       // Mô tả chi tiết
    public bool IsCompleted { get; set; }         // Trạng thái hoàn thành
    public DateTime CreatedAt { get; set; }      // Thời gian tạo
    public DateTime? CompletedAt { get; set; }    // Thời gian hoàn thành
}
```

### CreateTodoRequest
```csharp
public class CreateTodoRequest
{
    public string Title { get; set; }             // Tiêu đề (bắt buộc)
    public string? Description { get; set; }       // Mô tả (tùy chọn)
}
```

## 🎯 Mục tiêu học tập

Sau khi hoàn thành project này, bạn sẽ:

- ✅ Hiểu cách tạo ASP.NET Core Web API
- ✅ Biết cách tạo Controller và Route
- ✅ Sử dụng Model để nhận và trả dữ liệu JSON
- ✅ Thực hành các HTTP methods: GET, POST, PUT, DELETE
- ✅ Sử dụng Swagger để test API
- ✅ Hiểu về API documentation

## 🔧 Phát triển thêm

### Thêm tính năng
- [ ] Authentication & Authorization
- [ ] Database integration (SQL Server, PostgreSQL)
- [ ] Validation với Data Annotations
- [ ] Logging và Error Handling
- [ ] Unit Tests
- [ ] Docker containerization

### Cải thiện code
- [ ] Repository Pattern
- [ ] Dependency Injection
- [ ] AutoMapper cho DTO mapping
- [ ] FluentValidation
- [ ] Caching

## 📝 License

MIT License - Xem file LICENSE để biết thêm chi tiết.

## 👨‍💻 Author

**Manh Nguyen Khac**
- GitHub: [@manhnguyenkhac](https://github.com/manhnguyenkhac)
- Email: manhnguyenkhac@example.com

---

**Happy Coding! 🚀**
