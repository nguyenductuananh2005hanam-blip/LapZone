# LapZone
Lapzone Web là một ứng dụng thương mại điện tử chuyên cung cấp thiết bị máy tính (laptop),
được xây dựng trên nền tảng Asp.net Core với kiến trúc MVC.
## 🚀 Công Nghệ Sử Dụng
*   **Backend:** Asp.net Core 10.0, C#
* 	**Database:** SQL Server 
*   **ORM:** Entity Framework Core
*   **Frontend:** HTML, CSS, Bootstrap, Razor
*   **Build Tool:** Visual Studio MSBuild

## 📋 Yêu Cầu Môi Trường (Prerequisites)
Để chạy được dự án này, máy tính của bạn cần cài đặt sẵn:
- .NET SDK 6.0 hoặc cao hơn
- Visual Studio 2022 hoặc Visual Studio Code
- SQL Server Management Studio (SSMS)

## ⚙️ Hướng Dẫn Cài Đặt & Chạy Chương Trình
### Bước 1: Khởi tạo Cơ sở dữ liệu (Database)
    1. Mở công cụ quản lý SQL Server của bạn     
    2. Tạo một cơ sở dữ liệu mới với tên `lapdb`
    IF EXISTS (SELECT * FROM sys.databases WHERE name = 'lapdb')
    BEGIN
        ALTER DATABASE lapdb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE lapdb;
    END
    GO
    3. Mở và chạy file script `lapdb.sql` (nằm ở thư mục gốc của dự án) vào database `lapdb` vừa tạo để tạo bảng và nạp dữ liệu mẫu.
### Bước 2: Cấu hình kết nối Database
    1. Mở file `appsettings.json` trong thư mục gốc của dự án.
    2. Tìm đến phần `"ConnectionStrings"` và thay đổi giá trị của `"DefaultConnection"` thành chuỗi kết nối của bạn, ví dụ:
    ```json
    {
      "ConnectionStrings": {
        "LapDbContext": "Server=(localdb)\\mssqllocaldb;Database=LapDbContext;Trusted_Connection=True;MultipleActiveResultSets=true",
        "DefaultConnection": "Server=.\\SQLEXPRESS;Database=lapdb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
      }
    }
    ```
    3. Lưu file `appsettings.json`.
    4. Mở Visual Studio, nhấn chuột phải vào dự án và chọn "Manage NuGet Packages". Cài đặt các gói cần thiết nếu chưa có:
       - `Microsoft.EntityFrameworkCore.SqlServer`
       - `Microsoft.EntityFrameworkCore.Tools`
       - `Microsoft.EntityFrameworkCore.Design`
    5. Tạo Model và DbContext từ database bằng lệnh Scaffold:
       - Đảm bảo database `lapdb` đã tồn tại trong SQL Server.
       - Mở **Tools > NuGet Package Manager > Package Manager Console** và chạy:
       ```powershell
       Scaffold-DbContext Name=ConnectionStrings:DefaultConnection Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context LapDbContext -Force
       ```
       - Hoặc nếu dùng terminal (.NET CLI):
       ```bash
       dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context LapDbContext --force
       ```
       - Sau khi chạy xong, thư mục `Models` sẽ chứa các class entity và file `LapDbContext.cs`.
    6. Đăng ký DbContext trong `Program.cs`:
       ```csharp
       builder.Services.AddDbContext<LapDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
       ```
       Nếu `LapDbContext.cs` có sinh sẵn phương thức `OnConfiguring` chứa chuỗi kết nối cứng thì hãy xóa nó đi.
### Bước 3: Chạy ứng dụng
    1. Khôi phục các gói và build dự án:
       ```bash
       dotnet restore
       dotnet build
       ```
    2. Chạy ứng dụng bằng một trong hai cách:
       - **Visual Studio:** nhấn `F5` (chạy có debug) hoặc `Ctrl + F5` (chạy không debug).
       - **Terminal (.NET CLI):** mở terminal tại thư mục gốc của dự án và chạy:
       ```bash
       dotnet run
       ```
    3. Mở trình duyệt và truy cập địa chỉ hiển thị trong terminal, thường là:
       - `https://localhost:7xxx`
       - `http://localhost:5xxx`

       (Số cổng cụ thể nằm trong file `Properties/launchSettings.json`.)
    4. Kiểm tra kết nối database: mở trang có dữ liệu từ bảng trong `lapdb`. Nếu dữ liệu hiển thị bình thường, kết nối đã thành công.
    5. Dừng ứng dụng: nhấn `Shift + F5` trong Visual Studio hoặc `Ctrl + C` trong terminal.

    **Một số lỗi thường gặp:**
    - `Cannot open database "lapdb"`: database chưa tồn tại hoặc sai tên trong chuỗi kết nối. Hãy kiểm tra lại SQL Server.
    - `A network-related or instance-specific error`: sai tên instance (ví dụ `.\SQLEXPRESS`) hoặc dịch vụ SQL Server chưa chạy.
    - `The certificate chain was issued by an authority that is not trusted`: thêm `TrustServerCertificate=True` vào chuỗi kết nối (bản mẫu ở Bước 2 đã có sẵn).
    - Lỗi HTTPS certificate khi chạy lần đầu: chạy `dotnet dev-certs https --trust`.
## 📂 Cấu Trúc Thư Mục Chính
```
Lapzone_test/
├── Controllers/            # Xử lý request, điều hướng và trả về View
│   ├── HomeController.cs
│   └── ...
├── Models/                 # Entity và DbContext (sinh bằng lệnh Scaffold)
│   ├── LapDbContext.cs
│   └── ...           
├── Views/                  # Giao diện Razor (.cshtml)
│   ├── Home/
│   ├── Shared/             # Layout, partial view dùng chung
│   │   └── _Layout.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/                # Tệp tĩnh
│   ├── css/
│   ├── js/
│   ├── lib/                # Thư viện frontend (Bootstrap, jQuery, ...)
│   └── images/
├── Properties/
│   └── launchSettings.json # Cấu hình cổng và môi trường khi chạy
├── appsettings.json        # Cấu hình ứng dụng, chuỗi kết nối database
├── Program.cs              # Điểm khởi chạy, đăng ký service và middleware
└── Lapzone_test.csproj          # Khai báo gói NuGet và cấu hình dự án
```

### Mô tả nhanh

| Thư mục / Tệp | Vai trò |
|---|---|
| `Controllers/` | Nhận request từ người dùng, gọi xử lý dữ liệu và trả về View |
| `Models/` | Chứa các class ánh xạ với bảng trong database và `LapDbContext` |
| `Views/` | Giao diện hiển thị cho người dùng (Razor) |
| `wwwroot/` | Chứa CSS, JavaScript, hình ảnh được truy cập trực tiếp từ trình duyệt |
| `appsettings.json` | Lưu chuỗi kết nối và các thiết lập cấu hình |
| `Program.cs` | Đăng ký DbContext, cấu hình routing và pipeline của ứng dụng |
