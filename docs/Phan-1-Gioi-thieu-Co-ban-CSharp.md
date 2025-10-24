# Phần 1: Giới thiệu & Cơ bản về C#

⏱️ **Thời lượng:** 60 phút

## 📚 Nội dung

1. Giới thiệu về C#, .NET, ASP.NET Core
2. Cấu trúc chương trình C#
3. Kiểu dữ liệu, biến, hằng số
4. Câu lệnh điều kiện
5. Vòng lặp
6. Phương thức (Methods)
7. Lớp (Class) và Đối tượng (Object)

---

## 1. Giới thiệu về C#, .NET, ASP.NET Core (5-10 phút)

### C# là gì?

**C#** (C Sharp) là ngôn ngữ lập trình hướng đối tượng, được phát triển bởi Microsoft. C# là ngôn ngữ hiện đại, mạnh mẽ và được sử dụng rộng rãi để:
- Xây dựng ứng dụng desktop (Windows Forms, WPF)
- Phát triển web (ASP.NET Core)
- Tạo game (Unity)
- Ứng dụng mobile (Xamarin, .NET MAUI)
- Ứng dụng đám mây (Azure)

### .NET là gì?

**.NET** là nền tảng phát triển phần mềm miễn phí, mã nguồn mở, đa nền tảng (Windows, macOS, Linux) được Microsoft phát triển.

**Các thành phần chính:**
- **.NET Runtime:** Môi trường chạy ứng dụng
- **.NET Libraries:** Thư viện chuẩn
- **.NET SDK:** Bộ công cụ phát triển

### ASP.NET Core là gì?

**ASP.NET Core** là framework để xây dựng ứng dụng web hiện đại:
- Web API (RESTful services)
- Web Applications (MVC, Razor Pages)
- Real-time apps (SignalR)
- Microservices

**Ưu điểm:**
- ✅ Đa nền tảng
- ✅ Hiệu suất cao
- ✅ Mã nguồn mở
- ✅ Dependency Injection tích hợp sẵn
- ✅ Tích hợp Swagger/OpenAPI

---

## 2. Cấu trúc chương trình C#

### Chương trình C# đơn giản

```csharp
using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```

### Giải thích cấu trúc:

1. **`using System;`** - Import namespace để sử dụng các class có sẵn
2. **`namespace`** - Tổ chức code thành các nhóm logic
3. **`class`** - Định nghĩa một lớp
4. **`Main`** - Điểm bắt đầu của chương trình
5. **`Console.WriteLine()`** - In ra console

### Top-level statements (C# 9.0+)

Từ C# 9.0, bạn có thể viết ngắn gọn hơn:

```csharp
using System;

Console.WriteLine("Hello, World!");
```

---

## 3. Kiểu dữ liệu, Biến, Hằng số

### Kiểu dữ liệu cơ bản

```csharp
// Số nguyên
int age = 25;
long population = 8000000000L;

// Số thực
double price = 99.99;
float temperature = 36.5f;
decimal money = 1000.50m; // Dùng cho tiền tệ

// Ký tự và chuỗi
char grade = 'A';
string name = "Nguyễn Văn A";

// Boolean
bool isActive = true;
bool isCompleted = false;

// DateTime
DateTime today = DateTime.Now;
```

### Khai báo biến

```csharp
// Khai báo tường minh
int number = 10;
string message = "Hello";

// Khai báo với var (type inference)
var count = 100;        // int
var text = "World";     // string
var pi = 3.14;          // double
```

### Hằng số (Constants)

```csharp
const double PI = 3.14159;
const string APP_NAME = "My Application";

// PI = 3.14; // ❌ Lỗi: Không thể thay đổi giá trị hằng số
```

### Nullable types

```csharp
int? nullableInt = null;  // int có thể null
string? nullableString = null;  // string có thể null (C# 8.0+)

// Kiểm tra null
if (nullableInt.HasValue)
{
    Console.WriteLine($"Value: {nullableInt.Value}");
}
```

---

## 4. Câu lệnh điều kiện

### If/Else

```csharp
int score = 85;

if (score >= 90)
{
    Console.WriteLine("Xuất sắc");
}
else if (score >= 80)
{
    Console.WriteLine("Giỏi");
}
else if (score >= 70)
{
    Console.WriteLine("Khá");
}
else if (score >= 60)
{
    Console.WriteLine("Trung bình");
}
else
{
    Console.WriteLine("Yếu");
}
```

### Toán tử ba ngôi (Ternary Operator)

```csharp
int age = 18;
string status = (age >= 18) ? "Người lớn" : "Trẻ em";
Console.WriteLine(status); // Output: Người lớn
```

### Switch Statement

```csharp
int day = 3;
string dayName;

switch (day)
{
    case 1:
        dayName = "Thứ 2";
        break;
    case 2:
        dayName = "Thứ 3";
        break;
    case 3:
        dayName = "Thứ 4";
        break;
    case 4:
        dayName = "Thứ 5";
        break;
    case 5:
        dayName = "Thứ 6";
        break;
    case 6:
        dayName = "Thứ 7";
        break;
    case 7:
        dayName = "Chủ nhật";
        break;
    default:
        dayName = "Không hợp lệ";
        break;
}

Console.WriteLine(dayName);
```

### Switch Expression (C# 8.0+)

```csharp
int day = 3;
string dayName = day switch
{
    1 => "Thứ 2",
    2 => "Thứ 3",
    3 => "Thứ 4",
    4 => "Thứ 5",
    5 => "Thứ 6",
    6 => "Thứ 7",
    7 => "Chủ nhật",
    _ => "Không hợp lệ"
};

Console.WriteLine(dayName);
```

---

## 5. Vòng lặp

### For Loop

```csharp
// In số từ 1 đến 10
for (int i = 1; i <= 10; i++)
{
    Console.WriteLine(i);
}

// Tính tổng từ 1 đến 100
int sum = 0;
for (int i = 1; i <= 100; i++)
{
    sum += i;
}
Console.WriteLine($"Tổng: {sum}"); // Output: 5050
```

### While Loop

```csharp
int count = 1;
while (count <= 5)
{
    Console.WriteLine($"Count: {count}");
    count++;
}
```

### Do-While Loop

```csharp
int number = 1;
do
{
    Console.WriteLine(number);
    number++;
} while (number <= 5);
```

### Foreach Loop

```csharp
string[] fruits = { "Táo", "Chuối", "Cam", "Dưa hấu" };

foreach (string fruit in fruits)
{
    Console.WriteLine(fruit);
}

// Với List
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
foreach (int num in numbers)
{
    Console.WriteLine(num * 2);
}
```

### Break và Continue

```csharp
// Break - Thoát khỏi vòng lặp
for (int i = 1; i <= 10; i++)
{
    if (i == 6)
        break; // Dừng khi i = 6
    Console.WriteLine(i);
}

// Continue - Bỏ qua lần lặp hiện tại
for (int i = 1; i <= 10; i++)
{
    if (i % 2 == 0)
        continue; // Bỏ qua số chẵn
    Console.WriteLine(i); // Chỉ in số lẻ
}
```

---

## 6. Phương thức (Methods)

### Định nghĩa phương thức

```csharp
// Phương thức không trả về giá trị (void)
void SayHello()
{
    Console.WriteLine("Xin chào!");
}

// Phương thức có tham số
void SayHelloTo(string name)
{
    Console.WriteLine($"Xin chào, {name}!");
}

// Phương thức trả về giá trị
int Add(int a, int b)
{
    return a + b;
}

// Gọi phương thức
SayHello();
SayHelloTo("An");
int result = Add(5, 3);
Console.WriteLine($"Kết quả: {result}"); // Output: 8
```

### Tham số mặc định

```csharp
void Greet(string name = "Khách")
{
    Console.WriteLine($"Xin chào, {name}!");
}

Greet();          // Output: Xin chào, Khách!
Greet("Minh");    // Output: Xin chào, Minh!
```

### Named arguments

```csharp
void CreateUser(string name, int age, string email)
{
    Console.WriteLine($"Name: {name}, Age: {age}, Email: {email}");
}

// Gọi với named arguments
CreateUser(age: 25, name: "An", email: "an@example.com");
```

### Expression-bodied methods

```csharp
// Phương thức ngắn gọn
int Multiply(int a, int b) => a * b;
double GetCircleArea(double radius) => Math.PI * radius * radius;

Console.WriteLine(Multiply(4, 5));        // Output: 20
Console.WriteLine(GetCircleArea(10));     // Output: 314.159...
```

---

## 7. Lớp (Class) và Đối tượng (Object)

### Định nghĩa Class

```csharp
public class Person
{
    // Thuộc tính (Properties)
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }

    // Constructor
    public Person(string name, int age, string email)
    {
        Name = name;
        Age = age;
        Email = email;
    }

    // Phương thức (Method)
    public void Introduce()
    {
        Console.WriteLine($"Xin chào, tôi là {Name}, {Age} tuổi.");
    }

    public void SendEmail(string message)
    {
        Console.WriteLine($"Gửi email đến {Email}: {message}");
    }
}
```

### Tạo và sử dụng Object

```csharp
// Tạo đối tượng
Person person1 = new Person("Nguyễn Văn An", 25, "an@example.com");

// Gọi phương thức
person1.Introduce(); // Output: Xin chào, tôi là Nguyễn Văn An, 25 tuổi.
person1.SendEmail("Chào bạn!"); // Output: Gửi email đến an@example.com: Chào bạn!

// Truy cập thuộc tính
Console.WriteLine(person1.Name); // Output: Nguyễn Văn An
person1.Age = 26;
Console.WriteLine(person1.Age); // Output: 26
```

### Object Initializer

```csharp
// Cách 1: Dùng constructor
Person person2 = new Person("Trần Thị Bình", 30, "binh@example.com");

// Cách 2: Object initializer
Person person3 = new Person
{
    Name = "Lê Văn Cường",
    Age = 28,
    Email = "cuong@example.com"
};
```

### Auto-implemented Properties

```csharp
public class Product
{
    // Properties với getter và setter tự động
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // Read-only property
    public string Currency { get; } = "VND";
    
    // Property với logic
    private decimal _discount;
    public decimal Discount 
    { 
        get => _discount;
        set
        {
            if (value >= 0 && value <= 100)
                _discount = value;
        }
    }
    
    // Computed property
    public decimal FinalPrice => Price * (1 - Discount / 100);
}
```

---

## 💻 Bài tập thực hành

### Bài 1: Tính toán cơ bản

Viết chương trình C# tính diện tích và chu vi hình chữ nhật:

```csharp
// Giải pháp
using System;

class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double GetArea()
    {
        return Width * Height;
    }

    public double GetPerimeter()
    {
        return 2 * (Width + Height);
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Chiều rộng: {Width}");
        Console.WriteLine($"Chiều cao: {Height}");
        Console.WriteLine($"Diện tích: {GetArea()}");
        Console.WriteLine($"Chu vi: {GetPerimeter()}");
    }
}

// Sử dụng
Rectangle rect = new Rectangle(5, 10);
rect.DisplayInfo();
```

### Bài 2: Xử lý chuỗi

Viết chương trình phân tích chuỗi:

```csharp
using System;

class StringAnalyzer
{
    public void AnalyzeString(string text)
    {
        Console.WriteLine($"Chuỗi gốc: {text}");
        Console.WriteLine($"Độ dài: {text.Length}");
        Console.WriteLine($"Viết hoa: {text.ToUpper()}");
        Console.WriteLine($"Viết thường: {text.ToLower()}");
        
        // Đếm số từ
        string[] words = text.Split(' ');
        Console.WriteLine($"Số từ: {words.Length}");
        
        // Đảo ngược chuỗi
        char[] charArray = text.ToCharArray();
        Array.Reverse(charArray);
        string reversed = new string(charArray);
        Console.WriteLine($"Chuỗi đảo: {reversed}");
    }
}

// Sử dụng
StringAnalyzer analyzer = new StringAnalyzer();
analyzer.AnalyzeString("Hello World from C#");
```

### Bài 3: Quản lý sinh viên

Tạo class Student và quản lý danh sách:

```csharp
using System;
using System.Collections.Generic;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public double GPA { get; set; }

    public Student(int id, string name, int age, double gpa)
    {
        Id = id;
        Name = name;
        Age = age;
        GPA = gpa;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id} | Tên: {Name} | Tuổi: {Age} | GPA: {GPA:F2}");
    }

    public string GetGrade()
    {
        if (GPA >= 3.6) return "Xuất sắc";
        if (GPA >= 3.2) return "Giỏi";
        if (GPA >= 2.5) return "Khá";
        if (GPA >= 2.0) return "Trung bình";
        return "Yếu";
    }
}

class Program
{
    static void Main()
    {
        List<Student> students = new List<Student>
        {
            new Student(1, "Nguyễn Văn An", 20, 3.8),
            new Student(2, "Trần Thị Bình", 21, 3.5),
            new Student(3, "Lê Văn Cường", 22, 2.9),
            new Student(4, "Phạm Thị Dung", 20, 3.9)
        };

        Console.WriteLine("=== DANH SÁCH SINH VIÊN ===");
        foreach (var student in students)
        {
            student.DisplayInfo();
            Console.WriteLine($"Xếp loại: {student.GetGrade()}\n");
        }

        // Tìm sinh viên có GPA cao nhất
        Student topStudent = students[0];
        foreach (var student in students)
        {
            if (student.GPA > topStudent.GPA)
            {
                topStudent = student;
            }
        }

        Console.WriteLine("=== SINH VIÊN XUẤT SẮC NHẤT ===");
        topStudent.DisplayInfo();
    }
}
```

---

## 📝 Tóm tắt

Trong phần này, chúng ta đã học:

✅ Giới thiệu về C#, .NET, và ASP.NET Core  
✅ Cấu trúc chương trình C#  
✅ Các kiểu dữ liệu cơ bản (int, string, bool, double, ...)  
✅ Biến và hằng số  
✅ Câu lệnh điều kiện (if/else, switch)  
✅ Vòng lặp (for, while, foreach)  
✅ Phương thức (methods) và tham số  
✅ Class và Object  
✅ Properties và Methods trong Class  

---

## ➡️ Tiếp theo

Chuyển sang [Phần 2: Nâng cao trong C#](./Phan-2-Nang-cao-CSharp.md) để tìm hiểu về OOP, LINQ, và Exception Handling.

---

## 🎯 Ví dụ thực tế

### Các công ty sử dụng C#:
- **Microsoft** - Windows, Azure, Office 365
- **Stack Overflow** - Website Q&A lớn nhất thế giới
- **Siemens** - Hệ thống công nghiệp
- **Alibaba** - E-commerce platform
- **Delivery Hero** - Food delivery platform

### Case Study: Todo Application

Xem ví dụ hoàn chỉnh tại:
- [Microsoft Todo App Sample](https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/tutorials/first-web-api/samples)
- [.NET Samples Browser](https://docs.microsoft.com/samples/browse/?products=dotnet)

## 💡 Tips & Tricks

### 1. Sử dụng Visual Studio shortcuts:
- `Ctrl + K, Ctrl + D` - Format document
- `Ctrl + .` - Quick actions and refactorings
- `F5` - Start debugging
- `Ctrl + F5` - Start without debugging
- `Ctrl + Space` - IntelliSense

### 2. Debug hiệu quả:
```csharp
// Sử dụng breakpoint và Watch window
int sum = 0;
for (int i = 1; i <= 10; i++)
{
    sum += i; // Đặt breakpoint ở đây
    Console.WriteLine($"i = {i}, sum = {sum}");
}
```

### 3. Code snippet shortcuts:
- `cw` + Tab Tab → `Console.WriteLine()`
- `ctor` + Tab Tab → Constructor
- `prop` + Tab Tab → Property

## 📊 Thống kê thú vị

> **C# là ngôn ngữ thứ 5 phổ biến nhất** theo TIOBE Index 2024
> 
> **80% ứng dụng Windows** được viết bằng C# hoặc .NET
>
> **Hơn 5 triệu developers** sử dụng C# trên toàn thế giới

## 🎓 Chứng chỉ liên quan

- **Microsoft Certified: Azure Developer Associate**
- **Microsoft Certified: .NET Developer**
- **C# Programming Certificate** - FreeCodeCamp

---

**📚 Tài liệu tham khảo:**

#### Documentation:
- [C# Programming Guide](https://docs.microsoft.com/dotnet/csharp/programming-guide/)
- [C# Fundamentals](https://docs.microsoft.com/dotnet/csharp/fundamentals/)
- [C# Language Reference](https://docs.microsoft.com/dotnet/csharp/language-reference/)

#### Video Tutorials:
- [C# Tutorial for Beginners - Programming with Mosh](https://www.youtube.com/watch?v=gfkTfcpWqAY)
- [C# Fundamentals for Absolute Beginners - Microsoft](https://channel9.msdn.com/Series/CSharp-Fundamentals-for-Absolute-Beginners)

#### Interactive Learning:
- [Microsoft Learn - C# Path](https://docs.microsoft.com/learn/paths/csharp-first-steps/)
- [.NET Fiddle](https://dotnetfiddle.net/) - Practice online
- [Exercism - C# Track](https://exercism.org/tracks/csharp)

#### Blogs & Articles:
- [C# Corner](https://www.c-sharpcorner.com/)
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)
- [Jon Skeet's Coding Blog](https://codeblog.jonskeet.uk/)

