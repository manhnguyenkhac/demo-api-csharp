# Phần 2: Nâng cao trong C#

⏱️ **Thời lượng:** 45 phút

## 📚 Nội dung

1. Khái niệm OOP trong C#
2. Tính đóng gói (Encapsulation)
3. Tính kế thừa (Inheritance)
4. Tính đa hình (Polymorphism)
5. Interface
6. Abstract Class
7. Collection & LINQ cơ bản
8. Exception Handling

---

## 1. Lập trình hướng đối tượng (OOP)

**OOP (Object-Oriented Programming)** là phương pháp lập trình dựa trên khái niệm "đối tượng", bao gồm dữ liệu (thuộc tính) và hành vi (phương thức).

### 4 Tính chất cơ bản của OOP:

1. **Encapsulation (Đóng gói)** - Che giấu thông tin, bảo vệ dữ liệu
2. **Inheritance (Kế thừa)** - Tái sử dụng code, tạo quan hệ cha-con
3. **Polymorphism (Đa hình)** - Một interface, nhiều implementation
4. **Abstraction (Trừu tượng)** - Ẩn chi tiết phức tạp, chỉ hiển thị tính năng cần thiết

---

## 2. Tính đóng gói (Encapsulation)

### Access Modifiers (Phạm vi truy cập)

```csharp
public class BankAccount
{
    // public: Truy cập từ bất kỳ đâu
    public string AccountNumber { get; set; }
    
    // private: Chỉ truy cập trong class
    private decimal balance;
    
    // protected: Truy cập trong class và class con
    protected string accountType;
    
    // internal: Truy cập trong cùng assembly
    internal DateTime createdDate;

    // Constructor
    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        balance = initialBalance;
        createdDate = DateTime.Now;
    }

    // Public method để truy cập private field
    public decimal GetBalance()
    {
        return balance;
    }

    // Validation trong setter
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Số tiền phải lớn hơn 0");
        }
        balance += amount;
        Console.WriteLine($"Đã nạp {amount:C}. Số dư: {balance:C}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Số tiền phải lớn hơn 0");
        }
        if (amount > balance)
        {
            throw new InvalidOperationException("Số dư không đủ");
        }
        balance -= amount;
        Console.WriteLine($"Đã rút {amount:C}. Số dư: {balance:C}");
    }
}

// Sử dụng
var account = new BankAccount("123456789", 1000m);
account.Deposit(500);    // ✅ OK
account.Withdraw(300);   // ✅ OK
// account.balance = 9999; // ❌ Lỗi: balance là private
```

### Properties với logic

```csharp
public class Person
{
    private int age;
    
    public int Age
    {
        get { return age; }
        set
        {
            if (value < 0 || value > 150)
            {
                throw new ArgumentException("Tuổi không hợp lệ");
            }
            age = value;
        }
    }

    // Property ngắn gọn với expression body
    public string Name { get; set; }
    public int BirthYear { get; set; }
    
    // Computed property
    public int CalculatedAge => DateTime.Now.Year - BirthYear;
}
```

---

## 3. Tính kế thừa (Inheritance)

### Base Class và Derived Class

```csharp
// Base class (class cha)
public class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public virtual void MakeSound()
    {
        Console.WriteLine("Động vật đang kêu...");
    }

    public void Eat()
    {
        Console.WriteLine($"{Name} đang ăn...");
    }
}

// Derived class (class con)
public class Dog : Animal
{
    public string Breed { get; set; }

    // Constructor gọi constructor của class cha
    public Dog(string name, int age, string breed) : base(name, age)
    {
        Breed = breed;
    }

    // Override phương thức của class cha
    public override void MakeSound()
    {
        Console.WriteLine($"{Name} (chó {Breed}): Gâu gâu!");
    }

    // Phương thức riêng của Dog
    public void Fetch()
    {
        Console.WriteLine($"{Name} đang đuổi theo bóng...");
    }
}

public class Cat : Animal
{
    public bool IsIndoor { get; set; }

    public Cat(string name, int age, bool isIndoor) : base(name, age)
    {
        IsIndoor = isIndoor;
    }

    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: Meo meo!");
    }

    public void Climb()
    {
        Console.WriteLine($"{Name} đang leo cây...");
    }
}

// Sử dụng
Dog dog = new Dog("Buddy", 3, "Golden Retriever");
dog.MakeSound();  // Output: Buddy (chó Golden Retriever): Gâu gâu!
dog.Eat();        // Kế thừa từ Animal
dog.Fetch();      // Phương thức riêng

Cat cat = new Cat("Whiskers", 2, true);
cat.MakeSound();  // Output: Whiskers: Meo meo!
cat.Climb();      // Phương thức riêng
```

### Sealed Class (Không cho kế thừa)

```csharp
public sealed class FinalClass
{
    // Class này không thể bị kế thừa
}

// ❌ Lỗi: Không thể kế thừa sealed class
// public class AnotherClass : FinalClass { }
```

---

## 4. Tính đa hình (Polymorphism)

### Compile-time Polymorphism (Method Overloading)

```csharp
public class Calculator
{
    // Method overloading - cùng tên, khác tham số
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        return a + b + c;
    }

    public double Add(double a, double b)
    {
        return a + b;
    }

    public string Add(string a, string b)
    {
        return a + b;
    }
}

// Sử dụng
Calculator calc = new Calculator();
Console.WriteLine(calc.Add(5, 3));           // Output: 8
Console.WriteLine(calc.Add(5, 3, 2));        // Output: 10
Console.WriteLine(calc.Add(5.5, 3.2));       // Output: 8.7
Console.WriteLine(calc.Add("Hello", " World")); // Output: Hello World
```

### Runtime Polymorphism (Method Overriding)

```csharp
public class Shape
{
    public virtual double GetArea()
    {
        return 0;
    }

    public virtual void Draw()
    {
        Console.WriteLine("Vẽ hình...");
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double GetArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override void Draw()
    {
        Console.WriteLine("Vẽ hình tròn");
    }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double GetArea()
    {
        return Width * Height;
    }

    public override void Draw()
    {
        Console.WriteLine("Vẽ hình chữ nhật");
    }
}

// Sử dụng polymorphism
List<Shape> shapes = new List<Shape>
{
    new Circle(5),
    new Rectangle(4, 6),
    new Circle(3)
};

foreach (Shape shape in shapes)
{
    shape.Draw();
    Console.WriteLine($"Diện tích: {shape.GetArea():F2}\n");
}
```

---

## 5. Interface

Interface định nghĩa "hợp đồng" mà class phải tuân theo.

```csharp
// Interface
public interface IPayment
{
    void ProcessPayment(decimal amount);
    bool ValidatePayment();
    string GetPaymentMethod();
}

// Interface có thể kế thừa interface khác
public interface IRefundable : IPayment
{
    void ProcessRefund(decimal amount);
}

// Implement interface
public class CreditCardPayment : IRefundable
{
    public string CardNumber { get; set; }

    public CreditCardPayment(string cardNumber)
    {
        CardNumber = cardNumber;
    }

    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Xử lý thanh toán {amount:C} qua thẻ tín dụng {CardNumber}");
    }

    public bool ValidatePayment()
    {
        // Logic validation
        return CardNumber.Length == 16;
    }

    public string GetPaymentMethod()
    {
        return "Credit Card";
    }

    public void ProcessRefund(decimal amount)
    {
        Console.WriteLine($"Hoàn tiền {amount:C} về thẻ {CardNumber}");
    }
}

public class PayPalPayment : IPayment
{
    public string Email { get; set; }

    public PayPalPayment(string email)
    {
        Email = email;
    }

    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Xử lý thanh toán {amount:C} qua PayPal ({Email})");
    }

    public bool ValidatePayment()
    {
        return Email.Contains("@");
    }

    public string GetPaymentMethod()
    {
        return "PayPal";
    }
}

// Sử dụng
List<IPayment> payments = new List<IPayment>
{
    new CreditCardPayment("1234567890123456"),
    new PayPalPayment("user@example.com")
};

foreach (var payment in payments)
{
    if (payment.ValidatePayment())
    {
        payment.ProcessPayment(100m);
        Console.WriteLine($"Phương thức: {payment.GetPaymentMethod()}\n");
    }
}
```

### Interface vs Abstract Class

| Interface | Abstract Class |
|-----------|---------------|
| Không có implementation | Có thể có implementation |
| Một class có thể implement nhiều interface | Chỉ kế thừa một abstract class |
| Không có constructor | Có constructor |
| Tất cả members đều public | Có access modifiers |
| Dùng cho "can-do" relationship | Dùng cho "is-a" relationship |

---

## 6. Abstract Class

```csharp
public abstract class Employee
{
    public string Name { get; set; }
    public int Id { get; set; }

    public Employee(string name, int id)
    {
        Name = name;
        Id = id;
    }

    // Abstract method - phải override
    public abstract decimal CalculateSalary();

    // Virtual method - có thể override
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id} - Tên: {Name}");
    }

    // Concrete method - không thể override
    public void ClockIn()
    {
        Console.WriteLine($"{Name} đã check-in lúc {DateTime.Now:HH:mm}");
    }
}

public class FullTimeEmployee : Employee
{
    public decimal MonthlySalary { get; set; }

    public FullTimeEmployee(string name, int id, decimal monthlySalary) 
        : base(name, id)
    {
        MonthlySalary = monthlySalary;
    }

    public override decimal CalculateSalary()
    {
        return MonthlySalary;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Loại: Nhân viên chính thức - Lương: {MonthlySalary:C}");
    }
}

public class PartTimeEmployee : Employee
{
    public decimal HourlyRate { get; set; }
    public int HoursWorked { get; set; }

    public PartTimeEmployee(string name, int id, decimal hourlyRate, int hoursWorked) 
        : base(name, id)
    {
        HourlyRate = hourlyRate;
        HoursWorked = hoursWorked;
    }

    public override decimal CalculateSalary()
    {
        return HourlyRate * HoursWorked;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Loại: Nhân viên bán thời gian - {HoursWorked}h x {HourlyRate:C}");
    }
}

// Sử dụng
List<Employee> employees = new List<Employee>
{
    new FullTimeEmployee("Nguyễn Văn An", 1, 15000000m),
    new PartTimeEmployee("Trần Thị Bình", 2, 50000m, 80)
};

foreach (var emp in employees)
{
    emp.DisplayInfo();
    Console.WriteLine($"Lương tháng này: {emp.CalculateSalary():C}\n");
}
```

---

## 7. Collection & LINQ cơ bản

### Collections phổ biến

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

// List<T>
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
numbers.Add(6);
numbers.Remove(3);
Console.WriteLine($"Count: {numbers.Count}");

// Dictionary<TKey, TValue>
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "An", 25 },
    { "Bình", 30 },
    { "Cường", 28 }
};
ages["Dung"] = 22;
Console.WriteLine($"Tuổi của An: {ages["An"]}");

// HashSet<T> - Không chứa phần tử trùng lặp
HashSet<string> uniqueNames = new HashSet<string> { "An", "Bình", "An" };
Console.WriteLine($"Count: {uniqueNames.Count}"); // Output: 2

// Queue<T> - FIFO (First In First Out)
Queue<string> queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
string first = queue.Dequeue(); // "First"

// Stack<T> - LIFO (Last In First Out)
Stack<int> stack = new Stack<int>();
stack.Push(1);
stack.Push(2);
int top = stack.Pop(); // 2
```

### LINQ (Language Integrated Query)

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public int Stock { get; set; }
}

// Dữ liệu mẫu
List<Product> products = new List<Product>
{
    new Product { Id = 1, Name = "Laptop", Price = 20000000, Category = "Electronics", Stock = 10 },
    new Product { Id = 2, Name = "Mouse", Price = 200000, Category = "Electronics", Stock = 50 },
    new Product { Id = 3, Name = "Bàn", Price = 1500000, Category = "Furniture", Stock = 5 },
    new Product { Id = 4, Name = "Ghế", Price = 800000, Category = "Furniture", Stock = 15 },
    new Product { Id = 5, Name = "Keyboard", Price = 500000, Category = "Electronics", Stock = 30 }
};

// Where - Lọc dữ liệu
var expensiveProducts = products.Where(p => p.Price > 1000000);
Console.WriteLine("Sản phẩm giá > 1 triệu:");
foreach (var p in expensiveProducts)
{
    Console.WriteLine($"- {p.Name}: {p.Price:C}");
}

// Select - Chọn thuộc tính
var productNames = products.Select(p => p.Name);
Console.WriteLine("\nTên sản phẩm: " + string.Join(", ", productNames));

// OrderBy - Sắp xếp
var sortedByPrice = products.OrderBy(p => p.Price);
var sortedByPriceDesc = products.OrderByDescending(p => p.Price);

// First, FirstOrDefault
var firstProduct = products.First();
var expensiveProduct = products.FirstOrDefault(p => p.Price > 50000000); // null nếu không tìm thấy

// Any, All
bool hasExpensive = products.Any(p => p.Price > 10000000); // true
bool allInStock = products.All(p => p.Stock > 0); // true

// Count, Sum, Average, Min, Max
int totalProducts = products.Count();
int electronicsCount = products.Count(p => p.Category == "Electronics");
decimal totalValue = products.Sum(p => p.Price * p.Stock);
decimal avgPrice = products.Average(p => p.Price);
decimal minPrice = products.Min(p => p.Price);
decimal maxPrice = products.Max(p => p.Price);

Console.WriteLine($"\nTổng số sản phẩm: {totalProducts}");
Console.WriteLine($"Sản phẩm Electronics: {electronicsCount}");
Console.WriteLine($"Giá trị kho: {totalValue:C}");
Console.WriteLine($"Giá trung bình: {avgPrice:C}");

// GroupBy - Nhóm dữ liệu
var groupedByCategory = products.GroupBy(p => p.Category);
Console.WriteLine("\nSản phẩm theo danh mục:");
foreach (var group in groupedByCategory)
{
    Console.WriteLine($"{group.Key}: {group.Count()} sản phẩm");
    foreach (var p in group)
    {
        Console.WriteLine($"  - {p.Name}");
    }
}

// Query syntax (tương đương với method syntax)
var queryResult = from p in products
                  where p.Category == "Electronics"
                  orderby p.Price descending
                  select new { p.Name, p.Price };

foreach (var item in queryResult)
{
    Console.WriteLine($"{item.Name}: {item.Price:C}");
}
```

---

## 8. Exception Handling

### Try-Catch-Finally

```csharp
public class Calculator
{
    public int Divide(int a, int b)
    {
        try
        {
            return a / b;
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Lỗi: Không thể chia cho 0. {ex.Message}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi không xác định: {ex.Message}");
            return 0;
        }
        finally
        {
            // Code này luôn được thực thi
            Console.WriteLine("Hoàn tất phép tính");
        }
    }
}

// Sử dụng
Calculator calc = new Calculator();
int result = calc.Divide(10, 0); // Output: Lỗi: Không thể chia cho 0...
```

### Throw và Custom Exception

```csharp
// Custom exception
public class InvalidAgeException : Exception
{
    public InvalidAgeException() : base() { }
    
    public InvalidAgeException(string message) : base(message) { }
    
    public InvalidAgeException(string message, Exception innerException) 
        : base(message, innerException) { }
}

public class Person
{
    private int age;

    public int Age
    {
        get => age;
        set
        {
            if (value < 0 || value > 150)
            {
                throw new InvalidAgeException($"Tuổi {value} không hợp lệ. Tuổi phải từ 0-150.");
            }
            age = value;
        }
    }
}

// Sử dụng
try
{
    Person person = new Person();
    person.Age = 200; // Throw exception
}
catch (InvalidAgeException ex)
{
    Console.WriteLine($"Lỗi tuổi: {ex.Message}");
}
```

### Common Exceptions

```csharp
// NullReferenceException
string text = null;
// int length = text.Length; // ❌ NullReferenceException

// ArgumentException
public void SetAge(int age)
{
    if (age < 0)
        throw new ArgumentException("Tuổi không thể âm", nameof(age));
}

// InvalidOperationException
public void Withdraw(decimal amount)
{
    if (amount > balance)
        throw new InvalidOperationException("Số dư không đủ");
}

// FileNotFoundException
// var content = File.ReadAllText("notexist.txt"); // ❌

// FormatException
// int number = int.Parse("abc"); // ❌
```

---

## 💻 Bài tập thực hành

### Bài tập: Hệ thống quản lý sản phẩm

Xây dựng hệ thống quản lý sản phẩm với các yêu cầu:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

// Interface
public interface IProduct
{
    int Id { get; set; }
    string Name { get; set; }
    decimal Price { get; set; }
    void DisplayInfo();
}

// Abstract class
public abstract class Product : IProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }

    protected Product(int id, string name, decimal price, string category)
    {
        Id = id;
        Name = name;
        Price = price;
        Category = category;
    }

    public abstract decimal CalculateTax();

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Tên: {Name}");
        Console.WriteLine($"Giá: {Price:C}");
        Console.WriteLine($"Danh mục: {Category}");
        Console.WriteLine($"Thuế: {CalculateTax():C}");
        Console.WriteLine($"Tổng: {(Price + CalculateTax()):C}");
    }
}

// Concrete classes
public class Electronics : Product
{
    public int WarrantyMonths { get; set; }

    public Electronics(int id, string name, decimal price, int warrantyMonths)
        : base(id, name, price, "Electronics")
    {
        WarrantyMonths = warrantyMonths;
    }

    public override decimal CalculateTax()
    {
        return Price * 0.10m; // 10% thuế
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Bảo hành: {WarrantyMonths} tháng");
    }
}

public class Clothing : Product
{
    public string Size { get; set; }

    public Clothing(int id, string name, decimal price, string size)
        : base(id, name, price, "Clothing")
    {
        Size = size;
    }

    public override decimal CalculateTax()
    {
        return Price * 0.05m; // 5% thuế
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Kích thước: {Size}");
    }
}

// Product Manager
public class ProductManager
{
    private List<Product> products = new List<Product>();

    public void AddProduct(Product product)
    {
        if (products.Any(p => p.Id == product.Id))
        {
            throw new InvalidOperationException($"Sản phẩm với ID {product.Id} đã tồn tại");
        }
        products.Add(product);
        Console.WriteLine($"✅ Đã thêm sản phẩm: {product.Name}");
    }

    public void RemoveProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            throw new InvalidOperationException($"Không tìm thấy sản phẩm với ID {id}");
        }
        products.Remove(product);
        Console.WriteLine($"✅ Đã xóa sản phẩm: {product.Name}");
    }

    public void DisplayAllProducts()
    {
        Console.WriteLine("\n=== DANH SÁCH SẢN PHẨM ===");
        foreach (var product in products)
        {
            product.DisplayInfo();
            Console.WriteLine("---");
        }
    }

    public void FilterByCategory(string category)
    {
        var filtered = products.Where(p => p.Category == category);
        Console.WriteLine($"\n=== SẢN PHẨM DANH MỤC: {category} ===");
        foreach (var product in filtered)
        {
            Console.WriteLine($"- {product.Name} ({product.Price:C})");
        }
    }

    public void FilterByPriceRange(decimal min, decimal max)
    {
        var filtered = products.Where(p => p.Price >= min && p.Price <= max)
                               .OrderBy(p => p.Price);
        Console.WriteLine($"\n=== SẢN PHẨM GIÁ TỪ {min:C} - {max:C} ===");
        foreach (var product in filtered)
        {
            Console.WriteLine($"- {product.Name} ({product.Price:C})");
        }
    }

    public void GetStatistics()
    {
        if (!products.Any())
        {
            Console.WriteLine("Không có sản phẩm nào");
            return;
        }

        Console.WriteLine("\n=== THỐNG KÊ ===");
        Console.WriteLine($"Tổng số sản phẩm: {products.Count}");
        Console.WriteLine($"Giá trung bình: {products.Average(p => p.Price):C}");
        Console.WriteLine($"Giá thấp nhất: {products.Min(p => p.Price):C}");
        Console.WriteLine($"Giá cao nhất: {products.Max(p => p.Price):C}");
        Console.WriteLine($"Tổng giá trị: {products.Sum(p => p.Price):C}");

        var groupedByCategory = products.GroupBy(p => p.Category);
        Console.WriteLine("\nTheo danh mục:");
        foreach (var group in groupedByCategory)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()} sản phẩm");
        }
    }
}

// Program
class Program
{
    static void Main()
    {
        ProductManager manager = new ProductManager();

        try
        {
            // Thêm sản phẩm
            manager.AddProduct(new Electronics(1, "Laptop Dell XPS", 25000000, 24));
            manager.AddProduct(new Electronics(2, "iPhone 15", 20000000, 12));
            manager.AddProduct(new Clothing(3, "Áo Polo", 500000, "L"));
            manager.AddProduct(new Clothing(4, "Quần Jean", 800000, "M"));

            // Hiển thị tất cả
            manager.DisplayAllProducts();

            // Lọc theo danh mục
            manager.FilterByCategory("Electronics");

            // Lọc theo giá
            manager.FilterByPriceRange(500000, 5000000);

            // Thống kê
            manager.GetStatistics();

            // Xóa sản phẩm
            manager.RemoveProduct(2);

            // Hiển thị lại sau khi xóa
            manager.DisplayAllProducts();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi: {ex.Message}");
        }
    }
}
```

---

## 📝 Tóm tắt

Trong phần này, chúng ta đã học:

✅ **OOP:** 4 tính chất cơ bản (Đóng gói, Kế thừa, Đa hình, Trừu tượng)  
✅ **Encapsulation:** Access modifiers, Properties với validation  
✅ **Inheritance:** Base class, Derived class, Override  
✅ **Polymorphism:** Method overloading, Method overriding  
✅ **Interface:** Định nghĩa "hợp đồng", implement nhiều interface  
✅ **Abstract Class:** Kết hợp abstract và concrete members  
✅ **Collections:** List, Dictionary, HashSet, Queue, Stack  
✅ **LINQ:** Where, Select, OrderBy, GroupBy, Aggregate functions  
✅ **Exception Handling:** Try-Catch-Finally, Custom exceptions  

---

## ➡️ Tiếp theo

Chuyển sang [Phần 3: Bắt đầu với ASP.NET Core API](./Phan-3-Bat-dau-ASP-NET-Core-API.md) để bắt đầu xây dựng Web API!

---

## 💼 Ứng dụng thực tế của OOP

### Design Patterns phổ biến trong C#:

#### 1. **Singleton Pattern**
```csharp
public sealed class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();

    private Logger() { }

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
    }

    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }
}

// Sử dụng
Logger.Instance.Log("Application started");
```

#### 2. **Factory Pattern**
```csharp
public interface INotification
{
    void Send(string message);
}

public class EmailNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"Email: {message}");
}

public class SmsNotification : INotification
{
    public void Send(string message) => Console.WriteLine($"SMS: {message}");
}

public class NotificationFactory
{
    public static INotification Create(string type)
    {
        return type.ToLower() switch
        {
            "email" => new EmailNotification(),
            "sms" => new SmsNotification(),
            _ => throw new ArgumentException("Invalid notification type")
        };
    }
}
```

## 💡 LINQ Tips & Tricks

### Query Performance:
```csharp
// ❌ Bad - Multiple database queries
var products = context.Products.ToList();
var expensive = products.Where(p => p.Price > 1000).ToList();

// ✅ Good - Single query
var expensive = context.Products.Where(p => p.Price > 1000).ToList();
```

### Useful LINQ Methods:
```csharp
var numbers = Enumerable.Range(1, 100);

// Take first/last n items
var first10 = numbers.Take(10);
var last10 = numbers.TakeLast(10);

// Skip pagination
var page2 = numbers.Skip(10).Take(10);

// Distinct
var unique = numbers.Distinct();

// Zip - combine two sequences
var names = new[] { "An", "Binh", "Cuong" };
var ages = new[] { 25, 30, 28 };
var combined = names.Zip(ages, (name, age) => $"{name} is {age}");
```

## 🎯 Real-world Examples

### E-commerce System với OOP:
```csharp
// Abstract Payment Processor
public abstract class PaymentProcessor
{
    public abstract bool ProcessPayment(decimal amount);
    
    public void SendReceipt(string email)
    {
        Console.WriteLine($"Receipt sent to {email}");
    }
}

public class CreditCardProcessor : PaymentProcessor
{
    public override bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing credit card payment: {amount:C}");
        return true;
    }
}

public class PayPalProcessor : PaymentProcessor
{
    public override bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing PayPal payment: {amount:C}");
        return true;
    }
}

// Shopping Cart
public class ShoppingCart
{
    private List<Product> items = new();
    
    public void AddItem(Product product) => items.Add(product);
    
    public decimal Total => items.Sum(p => p.Price);
    
    public bool Checkout(PaymentProcessor processor)
    {
        return processor.ProcessPayment(Total);
    }
}
```

## 📊 Performance Benchmarks

> **LINQ vs Loop Performance:**
> - **Simple queries**: LINQ ~5-10% slower
> - **Complex queries**: LINQ code đọc dễ hơn, maintain tốt hơn
> - **Database queries**: Hiệu suất tương đương khi dùng IQueryable

## 🎓 Advanced Topics để học tiếp

- **Delegates & Events**
- **Async/Await & Task Parallel Library**
- **Reflection & Attributes**
- **Generics Constraints**
- **Extension Methods**
- **Expression Trees**

---

**📚 Tài liệu tham khảo:**

#### Documentation:
- [OOP in C#](https://docs.microsoft.com/dotnet/csharp/fundamentals/tutorials/oop)
- [LINQ Documentation](https://docs.microsoft.com/dotnet/csharp/linq/)
- [Exception Handling](https://docs.microsoft.com/dotnet/csharp/fundamentals/exceptions/)

#### Books:
- **"C# in Depth"** by Jon Skeet - Deep dive vào C#
- **"Head First Design Patterns"** - Design patterns với C#
- **"Effective C#"** by Bill Wagner - Best practices

#### Video Courses:
- [LINQ Tutorial - kudvenkat](https://www.youtube.com/playlist?list=PL6n9fhu94yhWi8K02Eqxp3Xyh_OmQ0Rp6)
- [Design Patterns in C#](https://www.youtube.com/playlist?list=PLrhzvIcii6GNjpARdnO4ueTUAVR9eMBpc)

#### Practice Sites:
- [LeetCode](https://leetcode.com/) - Algorithm practice
- [HackerRank C#](https://www.hackerrank.com/domains/tutorials/10-days-of-csharp)
- [Codewars](https://www.codewars.com/) - C# challenges

#### Blogs:
- [Jon Skeet's Blog](https://codeblog.jonskeet.uk/)
- [Stephen Cleary's Blog](https://blog.stephencleary.com/)
- [Eric Lippert's Blog](https://ericlippert.com/)

