# API Reference - Todo List API

## Base URL
- **HTTPS:** `https://localhost:7110`
- **HTTP:** `http://localhost:5070`

## Authentication
Hiện tại API không yêu cầu authentication. Trong production, nên thêm JWT hoặc API Key authentication.

## Content Type
Tất cả requests và responses đều sử dụng `application/json`.

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Tiêu đề không được để trống"]
  }
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Không tìm thấy công việc với ID 999"
}
```

## Endpoints

### 1. Get All Todos
Lấy danh sách tất cả công việc.

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
    "createdAt": "2024-10-22T10:00:00.000Z",
    "completedAt": null
  },
  {
    "id": 2,
    "title": "Tạo Todo API",
    "description": "Xây dựng API quản lý công việc",
    "isCompleted": true,
    "createdAt": "2024-10-21T09:00:00.000Z",
    "completedAt": "2024-10-22T14:30:00.000Z"
  }
]
```

### 2. Get Todo by ID
Lấy thông tin một công việc cụ thể.

```http
GET /api/todos/{id}
```

**Parameters:**
- `id` (int, required): ID của công việc

**Response:**
```json
{
  "id": 1,
  "title": "Học ASP.NET Core",
  "description": "Tìm hiểu về Web API",
  "isCompleted": false,
  "createdAt": "2024-10-22T10:00:00.000Z",
  "completedAt": null
}
```

### 3. Create Todo
Thêm công việc mới.

```http
POST /api/todos
Content-Type: application/json

{
  "title": "Tên công việc",
  "description": "Mô tả chi tiết (tùy chọn)"
}
```

**Request Body:**
```json
{
  "title": "string (required)",
  "description": "string (optional)"
}
```

**Response (201 Created):**
```json
{
  "id": 4,
  "title": "Tên công việc",
  "description": "Mô tả chi tiết",
  "isCompleted": false,
  "createdAt": "2024-10-22T15:30:00.000Z",
  "completedAt": null
}
```

### 4. Toggle Todo Completion
Đánh dấu hoàn thành hoặc chưa hoàn thành.

```http
PUT /api/todos/{id}
```

**Parameters:**
- `id` (int, required): ID của công việc

**Response:**
```json
{
  "id": 1,
  "title": "Học ASP.NET Core",
  "description": "Tìm hiểu về Web API",
  "isCompleted": true,
  "createdAt": "2024-10-22T10:00:00.000Z",
  "completedAt": "2024-10-22T16:00:00.000Z"
}
```

### 5. Delete Todo
Xóa công việc.

```http
DELETE /api/todos/{id}
```

**Parameters:**
- `id` (int, required): ID của công việc

**Response (200 OK):**
```json
{
  "message": "Đã xóa công việc 'Tên công việc' thành công"
}
```

### 6. Get Statistics
Lấy thống kê tổng quan về công việc.

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

## Data Models

### Todo
```typescript
interface Todo {
  id: number;                    // ID duy nhất (auto-generated)
  title: string;                 // Tiêu đề công việc
  description: string;           // Mô tả chi tiết
  isCompleted: boolean;          // Trạng thái hoàn thành
  createdAt: string;            // Thời gian tạo (ISO 8601)
  completedAt: string | null;   // Thời gian hoàn thành (ISO 8601)
}
```

### CreateTodoRequest
```typescript
interface CreateTodoRequest {
  title: string;                 // Tiêu đề (bắt buộc)
  description?: string;          // Mô tả (tùy chọn)
}
```

### Statistics
```typescript
interface Statistics {
  total: number;                // Tổng số công việc
  completed: number;            // Số công việc đã hoàn thành
  pending: number;              // Số công việc chưa hoàn thành
  completionRate: number;       // Tỷ lệ hoàn thành (%)
}
```

## Rate Limiting
Hiện tại API không có rate limiting. Trong production nên thêm:
- Request rate limiting
- IP-based throttling
- API key quotas

## CORS
API hiện tại cho phép tất cả origins. Trong production nên cấu hình CORS policy phù hợp.

## Monitoring & Logging
- Sử dụng Serilog cho structured logging
- Thêm health checks endpoint
- Metrics với Application Insights
