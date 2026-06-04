using Backend.DAL;
using Backend.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controllers
builder.Services.AddControllers();

// ============================================================
// TÍNH TRỪU TƯỢNG: Đăng ký các Interface với class cụ thể.
// Controller chỉ biết về Interface, không biết class thật.
// ============================================================
builder.Services.AddScoped<ISanPhamDAL, SanPhamDAL>();
builder.Services.AddScoped<IBanDAL, BanDAL>();
builder.Services.AddScoped<IHoaDonDAL, HoaDonDAL>();
builder.Services.AddScoped<IChiTietHoaDonDAL, ChiTietHoaDonDAL>();
builder.Services.AddScoped<ITaiKhoanDAL, TaiKhoanDAL>();

// Cho phép Frontend HTML/JS gọi API (CORS - tắt giới hạn origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ChoPhepTatCa", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Khởi tạo CSDL SQLite và nạp dữ liệu mẫu khi khởi động
DatabaseHelper.KhoiTaoCSDL();

app.UseCors("ChoPhepTatCa");

// Phục vụ file tĩnh: Frontend HTML/JS/CSS từ thư mục "wwwroot"
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

// Khi truy cập "/" thì trả về login.html để bắt buộc đăng nhập
app.MapFallbackToFile("login.html");

// Khởi chạy Web API dưới luồng nền (Background thread)
Task.Run(() => app.Run("http://localhost:5000"));

// Khởi chạy Windows Forms trên luồng STA (Single-Threaded Apartment) để tránh lỗi COM thread mode
var uiThread = new Thread(() =>
{
    System.Windows.Forms.Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
    System.Windows.Forms.Application.EnableVisualStyles();
    System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

    var formMain = new System.Windows.Forms.Form
    {
        Text = "☕ Hệ Thống Quản Lý Quán Cafe (Desktop App)",
        Width = 1350,
        Height = 850,
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
    };

    // Đặt icon tùy chỉnh cho cửa sổ ứng dụng desktop bằng file .ico
    try
    {
        string iconPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images", "logo.ico");
        if (!System.IO.File.Exists(iconPath))
        {
            iconPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.ico");
        }

        if (System.IO.File.Exists(iconPath))
        {
            formMain.Icon = new System.Drawing.Icon(iconPath);
        }
    }
    catch
    {
        // Bỏ qua nếu có lỗi nạp icon để chương trình vẫn chạy
    }

    var webView = new Microsoft.Web.WebView2.WinForms.WebView2
    {
        Dock = System.Windows.Forms.DockStyle.Fill
    };

    formMain.Controls.Add(webView);

    formMain.Load += async (s, e) =>
    {
        try
        {
            // Chờ 1 giây để server Web API khởi động hoàn tất
            await Task.Delay(1200);
            await webView.EnsureCoreWebView2Async();
            webView.Source = new Uri("http://localhost:5000");
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                $"Không thể tải giao diện WebView2: {ex.Message}",
                "Lỗi Giao Diện",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error
            );
        }
    };

    System.Windows.Forms.Application.Run(formMain);
});

// Thiết lập luồng STA bắt buộc cho WebView2
uiThread.SetApartmentState(ApartmentState.STA);
uiThread.Start();
uiThread.Join(); // Đợi luồng giao diện kết thúc thì tắt ứng dụng


