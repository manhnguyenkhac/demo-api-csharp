# Getting Started - Todo List API

Hướng dẫn chi tiết để setup và chạy Todo List API.

## 📋 Yêu cầu hệ thống

### Phần mềm cần thiết
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio 2022** hoặc **VS Code** với C# extension
- **Git** - [Download](https://git-scm.com/downloads)

### Kiểm tra .NET SDK
```bash
dotnet --version
# Kết quả mong đợi: 9.0.x
```

## 🚀 Setup Project

### Bước 1: Clone Repository
```bash
git clone https://github.com/manhnguyenkhac/demo-api-csharp.git
cd demo-api-csharp
```

### Bước 2: Restore Dependencies
```bash
dotnet restore
```

### Bước 3: Build Project
```bash
dotnet build
```

### Bước 4: Chạy Application
```bash
# Chạy với HTTPS (khuyến nghị)
dotnet run --launch-profile https

# Hoặc chạy với HTTP
dotnet run --launch-profile http
```

## 🌐 Truy cập API

Sau khi chạy thành công, bạn sẽ thấy output như sau:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7110
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5070
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### URLs quan trọng:
- **Swagger UI (HTTPS):** https://localhost:7110/swagger
- **Swagger UI (HTTP):** http://localhost:5070/swagger
- **API Base URL (HTTPS):** https://localhost:7110/api
- **API Base URL (HTTP):** http://localhost:5070/api

## 🧪 Test API

### Cách 1: Sử dụng Swagger UI (Khuyến nghị)
1. Mở https://localhost:7110/swagger
2. Bạn sẽ thấy tất cả endpoints với documentation
3. Click "Try it out" để test từng endpoint
4. Nhập dữ liệu và click "Execute"

### Cách 2: Sử dụng cURL
```bash
# Lấy danh sách todos
curl -X GET "https://localhost:7110/api/todos" -k

# Thêm todo mới
curl -X POST "https://localhost:7110/api/todos" \
  -H "Content-Type: application/json" \
  -d '{"title": "Test Todo", "description": "Test Description"}' \
  -k

# Cập nhật trạng thái
curl -X PUT "https://localhost:7110/api/todos/1" -k

# Xóa todo
curl -X DELETE "https://localhost:7110/api/todos/1" -k
```

### Cách 3: Sử dụng PowerShell
```powershell
# Lấy danh sách
$todos = Invoke-RestMethod -Uri "https://localhost:7110/api/todos" -Method GET -SkipCertificateCheck
$todos | ConvertTo-Json

# Thêm mới
$body = @{
    title = "PowerShell Test"
    description = "Test từ PowerShell"
} | ConvertTo-Json

$newTodo = Invoke-RestMethod -Uri "https://localhost:7110/api/todos" -Method POST -Body $body -ContentType "application/json" -SkipCertificateCheck
$newTodo | ConvertTo-Json
```

### Cách 4: Sử dụng Postman
1. Import collection từ file `postman/Todo-API.postman_collection.json`
2. Hoặc tạo request thủ công:
   - Base URL: `https://localhost:7110`
   - Disable SSL verification trong Settings

## 🔧 Development

### Cấu trúc Project
```
demo-api-csharp/
├── Controllers/
│   └── TodoController.cs          # API Controller
├── Models/
│   └── Todo.cs                    # Data Model
├── docs/
│   ├── API-Reference.md           # API Documentation
│   └── Getting-Started.md         # Setup Guide
├── Program.cs                     # Main Application
├── Properties/
│   └── launchSettings.json       # Launch Configuration
├── README.md                      # Project Overview
└── demo-api-csharp.csproj        # Project File
```

### Thêm Endpoint mới
1. Mở `Controllers/TodoController.cs`
2. Thêm method mới với attribute `[HttpMethod]`
3. Implement logic
4. Test qua Swagger UI

### Thêm Model mới
1. Tạo file trong folder `Models/`
2. Định nghĩa properties
3. Sử dụng trong Controller

## 🐛 Troubleshooting

### Lỗi thường gặp

#### 1. Port đã được sử dụng
```
System.IO.IOException: Failed to bind to address http://127.0.0.1:5070: address already in use.
```

**Giải pháp:**
```bash
# Tìm process đang sử dụng port
netstat -ano | findstr :5070
netstat -ano | findstr :7110

# Kill process (thay PID bằng process ID thực tế)
taskkill /PID <PID> /F
```

#### 2. SSL Certificate Error
```
The SSL connection could not be established
```

**Giải pháp:**
- Sử dụng HTTP thay vì HTTPS: `http://localhost:5070`
- Hoặc thêm `-k` flag cho cURL
- Hoặc disable SSL verification trong client

#### 3. 404 Not Found
```
The remote server returned an error: (404) Not Found.
```

**Giải pháp:**
- Kiểm tra URL có đúng không
- Đảm bảo ứng dụng đang chạy
- Kiểm tra route trong Controller

#### 4. Build Errors
```
error CS0246: The type or namespace name 'Todo' could not be found
```

**Giải pháp:**
```bash
# Clean và rebuild
dotnet clean
dotnet build
```

### Debug Mode
```bash
# Chạy với debug information
dotnet run --configuration Debug --verbosity detailed
```

### Logs
Ứng dụng sử dụng built-in logging. Để xem logs chi tiết:
```bash
dotnet run --verbosity detailed
```

## 📚 Tài liệu tham khảo

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Swagger/OpenAPI](https://swagger.io/docs/)
- [REST API Best Practices](https://restfulapi.net/)
- [HTTP Status Codes](https://httpstatuses.com/)

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Tạo Pull Request

## 📞 Support

Nếu gặp vấn đề, hãy:
1. Kiểm tra [Issues](https://github.com/manhnguyenkhac/demo-api-csharp/issues)
2. Tạo issue mới với thông tin chi tiết
3. Liên hệ: manhnguyenkhac@example.com

---

**Happy Coding! 🚀**
