# ☕ Quản Lý Quán Cafe

Ứng dụng desktop quản lý quán cafe được xây dựng bằng **C# ASP.NET Core** (backend) + **HTML/CSS/JavaScript** (giao diện) + **SQLite** (cơ sở dữ liệu).

---

## Cách Chạy Ứng Dụng

### Yêu Cầu
- .NET 10 SDK (hoặc mới hơn)
- Windows (chạy như ứng dụng desktop)

### Hướng Dẫn Chạy
Nhấp đúp vào file **`ChayUngDung.bat`** hoặc chạy lệnh:
```bash
cd Backend
dotnet run
```

Cửa sổ ứng dụng **Windows Forms Desktop** sẽ tự động hiển thị với giao diện HTML/CSS được tích hợp hoàn toàn bên trong cửa sổ ứng dụng (sử dụng Microsoft Edge WebView2). Không mở trình duyệt web bên ngoài.

---

## Kiến Trúc Dự Án (3 Tầng)

```
QuanLyCafe/
├── Backend/                     ← C# ASP.NET Core WebAPI
│   ├── Models/                  ← TẦNG 1: Entity Classes (OOP)
│   │   ├── SanPham.cs          ← Abstract class (Trừu tượng + Đóng gói)
│   │   ├── ThucUong.cs         ← Kế thừa SanPham (tính thêm phí size)
│   │   ├── DoAn.cs             ← Kế thừa SanPham (không tính thêm)
│   │   ├── Ban.cs
│   │   ├── HoaDon.cs
│   │   └── ChiTietHoaDon.cs
│   ├── Interfaces/              ← TẦNG 1.5: Hợp đồng DAL (Trừu tượng)
│   │   ├── ISanPhamDAL.cs
│   │   ├── IBanDAL.cs
│   │   ├── IHoaDonDAL.cs
│   │   └── IChiTietHoaDonDAL.cs
│   ├── DAL/                     ← TẦNG 2: Data Access Layer
│   │   ├── DatabaseHelper.cs   ← Kết nối SQLite + Tạo bảng + Seed Data
│   │   ├── SanPhamDAL.cs
│   │   ├── BanDAL.cs
│   │   ├── HoaDonDAL.cs
│   │   └── ChiTietHoaDonDAL.cs
│   ├── Controllers/             ← TẦNG 3: API Endpoints (REST)
│   │   ├── SanPhamController.cs
│   │   ├── BanController.cs
│   │   ├── HoaDonController.cs
│   │   └── ChiTietHoaDonController.cs
│   ├── wwwroot/                 ← TẦNG TRÌNH BÀY: HTML/JS/CSS
│   │   ├── index.html
│   │   ├── css/style.css
│   │   └── js/
│   │       ├── api.js          ← Gọi API từ JS
│   │       └── app.js          ← Logic giao diện
│   └── Program.cs              ← Khởi động + Cấu hình DI + CORS
└── ChayUngDung.bat              ← Script chạy ứng dụng
```

---

## 4 Tính Chất OOP - Cách Áp Dụng

### 1. Đóng Gói (Encapsulation)
**Nơi áp dụng:** `Models/SanPham.cs`, `Models/Ban.cs`

**Cách làm:** Dùng `private` field kết hợp `public` property có getter/setter để kiểm tra dữ liệu trước khi lưu.

```csharp
private decimal _giaCoBan;
public decimal GiaCoBan {
    get => _giaCoBan;
    set {
        if (value < 0)
            throw new ArgumentException("Giá không được âm.");
        _giaCoBan = value;
    }
}
```
**Ý nghĩa:** Bảo vệ dữ liệu, không cho phép đặt giá âm hay tên rỗng từ bên ngoài.

---

### 2. Kế Thừa (Inheritance)
**Nơi áp dụng:** `Models/ThucUong.cs` và `Models/DoAn.cs` kế thừa `Models/SanPham.cs`

```csharp
public abstract class SanPham { ... }    // Lớp cha
public class ThucUong : SanPham { ... }  // Lớp con 1
public class DoAn     : SanPham { ... }  // Lớp con 2
```
**Ý nghĩa:** Tái sử dụng thuộc tính chung (Id, TenSanPham, GiaCoBan...), mỗi lớp con chỉ cần định nghĩa phần riêng.

---

### 3. Đa Hình (Polymorphism)
**Nơi áp dụng:** Phương thức `TinhTien()` trong `ThucUong` và `DoAn`, được gọi ở `ChiTietHoaDonController.cs`

```csharp
// ThucUong: tính thêm 5.000đ nếu chọn Size L
public override decimal TinhTien(string? thuocTinhThem) {
    decimal gia = GiaCoBan;
    if (thuocTinhThem?.Contains("Size L") == true) gia += 5000;
    return gia;
}

// DoAn: không có phụ phí, giá cố định
public override decimal TinhTien(string? thuocTinhThem) {
    return GiaCoBan;
}
```

**Cách kích hoạt trong Controller:**
```csharp
// Backend dựa vào cột "Loai" để tạo đúng loại object
SanPham sp = loai == "ThucUong" ? new ThucUong() : new DoAn();

// Gọi TinhTien() - tự động chọn đúng phương thức của ThucUong hoặc DoAn
decimal donGiaBan = sp.TinhTien(request.ThuocTinhThem);
```
**Ý nghĩa:** Cùng một lời gọi `TinhTien()` nhưng hành vi khác nhau tùy loại đối tượng.

---

### 4. Trừu Tượng (Abstraction)
**Nơi áp dụng:** `Interfaces/ISanPhamDAL.cs`, `IBanDAL.cs`, `IHoaDonDAL.cs`, `IChiTietHoaDonDAL.cs`

```csharp
public interface ISanPhamDAL {
    List<SanPham> LayTatCa();
    void Them(SanPham sanPham);
    void Sua(SanPham sanPham);
    void Xoa(int id);
}
```
**Đăng ký trong `Program.cs`:**
```csharp
builder.Services.AddScoped<ISanPhamDAL, SanPhamDAL>();
```
**Ý nghĩa:** Controller chỉ biết về Interface, không phụ thuộc vào class cụ thể. Muốn đổi từ SQLite sang MySQL chỉ cần đổi 1 dòng trong `Program.cs`.

---

## Danh Sách API

| Phương Thức | Endpoint | Mô Tả |
|-------------|----------|-------|
| GET | `/api/sanpham` | Lấy menu đang bán |
| GET | `/api/sanpham/tat-ca` | Lấy tất cả sản phẩm |
| POST | `/api/sanpham` | Thêm sản phẩm |
| PUT | `/api/sanpham/{id}` | Sửa sản phẩm |
| DELETE | `/api/sanpham/{id}` | Ẩn sản phẩm |
| GET | `/api/ban` | Lấy danh sách bàn |
| POST | `/api/ban` | Thêm bàn |
| PUT | `/api/ban/{id}` | Sửa bàn |
| DELETE | `/api/ban/{id}` | Xóa bàn |
| GET | `/api/hoadon` | Lịch sử hóa đơn |
| GET | `/api/hoadon/ban/{banId}` | Hóa đơn hiện tại của bàn |
| POST | `/api/hoadon/mo-ban` | Mở bàn (tạo hóa đơn) |
| POST | `/api/hoadon/thanh-toan/{banId}` | Thanh toán |
| GET | `/api/chitiethoadon/hoadon/{id}` | Danh sách món trong hóa đơn |
| POST | `/api/chitiethoadon` | Thêm món vào hóa đơn |
| DELETE | `/api/chitiethoadon/{id}` | Xóa món khỏi hóa đơn |

---

## Quy Trình Gitflow

```
main
  ├── feature/khoi-tao-du-an       ✅ Khởi tạo solution và cấu trúc
  ├── feature/models-va-database   ✅ Entity classes + SQLite
  ├── feature/dal-va-interfaces    ✅ Interfaces + DAL
  ├── feature/api-controllers      ✅ REST API Controllers
  └── feature/giao-dien-html       ✅ Frontend HTML/JS/CSS
```

---

## Tính Năng

- ✅ **Quản lý bàn**: Xem sơ đồ bàn, thêm/sửa/xóa bàn, phân biệt trạng thái (màu sắc)
- ✅ **Quản lý menu**: Thêm/sửa/xóa (ẩn) đồ ăn và thức uống
- ✅ **Gọi món**: Chọn bàn, chọn món, nhập số lượng, tùy chọn size (Size L +5.000đ)
- ✅ **Thanh toán**: Tự động tính tổng, xuất hóa đơn, giải phóng bàn
- ✅ **Lịch sử**: Xem toàn bộ hóa đơn đã thanh toán

---

##  Công Nghệ Sử Dụng

| Thành Phần | Công Nghệ |
|-----------|-----------|
| Backend | C# ASP.NET Core 10 |
| Cơ sở dữ liệu | SQLite (Microsoft.Data.Sqlite) |
| Giao diện | HTML5 + CSS3 + JavaScript (Vanilla) |
| Font chữ | Be Vietnam Pro (Google Fonts) |
| Kiến trúc | REST API + 3 Tầng |


---

## Hướng Dẫn Chạy Giao Diện Desktop Bằng Lệnh `dotnet run`

Nếu bạn muốn chạy ứng dụng trực tiếp bằng lệnh để hiện ra cửa sổ giao diện phần mềm (desktop app) mà không cần đóng gói thành file `.exe`, hãy làm theo các bước chi tiết sau:

### 1. Nơi Thực Hiện Lệnh
Bạn cần chạy lệnh trong giao diện dòng lệnh (**Terminal**, **Command Prompt** hoặc **PowerShell**) và di chuyển vào thư mục **`Backend`** của dự án.

### 2. Các Bước Thực Hiện

#### **Bước 1:** Mở Terminal/Command Prompt trên máy tính của bạn.
*Nếu bạn đang mở dự án này bằng VS Code, chỉ cần nhấn tổ hợp phím `Ctrl + \`` (hoặc vào menu `Terminal` -> `New Terminal`).*

#### **Bước 2:** Di chuyển vào thư mục chứa mã nguồn Backend bằng lệnh:
```bash
cd Backend
```

#### **Bước 3:** Chạy lệnh khởi động ứng dụng:
```bash
dotnet run
```
*Lưu ý: Trong lần đầu tiên chạy, hệ thống sẽ tự động tải các gói thư viện cần thiết (như SQLite và WebView2) về máy của bạn.*

---

### Cách Hoạt Động Của Hệ Thống Khi Chạy Lệnh `dotnet run`

1. **Khởi chạy Web API (Luồng nền):** Lệnh sẽ khởi động một Web API chạy ngầm tại địa chỉ local `http://localhost:5000` để xử lý dữ liệu và SQLite.
2. **Khởi chạy Windows Forms (Luồng giao diện):** Đồng thời, ứng dụng tự động mở ra một cửa sổ Windows Form có kích thước 1350x850 với tiêu đề `" Hệ Thống Quản Lý Quán Cafe (Desktop App)"`.
3. **Tải Giao Diện Bằng WebView2:** Bên trong cửa sổ Windows Form đó, thư viện `Microsoft Edge WebView2` được tích hợp sẵn sẽ tự động kết nối đến `http://localhost:5000` và hiển thị trực tiếp giao diện HTML/CSS/JS mà **không cần mở trình duyệt web bên ngoài** (như Chrome, Edge hay Firefox).
4. **Đóng Ứng Dụng:** Khi bạn tắt cửa sổ Windows Form của phần mềm, tiến trình Web API chạy ngầm cũng sẽ tự động kết thúc để giải phóng tài nguyên.

---

### Một Số Lưu Ý & Xử Lý Lỗi Thường Gặp

- **Lỗi không mở được cửa sổ giao diện:** Đảm bảo máy tính của bạn đang chạy hệ điều hành **Windows** (vì Windows Forms chỉ hỗ trợ hệ điều hành Windows).
- **Yêu cầu cài đặt WebView2:** WebView2 được tích hợp sẵn trên các phiên bản Windows 10 và Windows 11 mới nhất. Nếu hệ thống thông báo lỗi thiếu WebView2, bạn chỉ cần tải và cài đặt bản **Evergreen Bootstrapper** từ trang chủ Microsoft WebView2.
- **Tập tin chạy nhanh nhanh hơn:** Bạn cũng có thể click đúp chuột trực tiếp vào file **`ChayUngDung.bat`** ở thư mục gốc của dự án để chạy nhanh ứng dụng mà không cần gõ lệnh thủ công.