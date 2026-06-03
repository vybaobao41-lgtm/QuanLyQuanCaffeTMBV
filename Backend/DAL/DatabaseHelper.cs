using Microsoft.Data.Sqlite;

namespace Backend.DAL
{
    /// <summary>
    /// Lớp trợ giúp kết nối SQLite và khởi tạo CSDL.
    /// Chứa dữ liệu mẫu (Seed Data) cho Menu ban đầu.
    /// </summary>
    public static class DatabaseHelper
    {
        // Đường dẫn file SQLite - đặt ngay cạnh file .exe
        private static readonly string _dbPath = "quanlycafe.db";
        public static string ChuoiKetNoi => $"Data Source={_dbPath}";

        /// <summary>
        /// Khởi tạo CSDL: Tạo bảng nếu chưa có, nạp dữ liệu mẫu.
        /// Gọi một lần khi khởi động ứng dụng.
        /// </summary>
        public static void KhoiTaoCSDL()
        {
            using var ketNoi = new SqliteConnection(ChuoiKetNoi);
            ketNoi.Open();

            // Bật FOREIGN KEY support trong SQLite
            var pragmaCmd = ketNoi.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCmd.ExecuteNonQuery();

            // Tạo bảng BAN
            var taoHang = ketNoi.CreateCommand();
            taoHang.CommandText = @"
                CREATE TABLE IF NOT EXISTS Ban (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenBan    TEXT NOT NULL,
                    TrangThai TEXT NOT NULL DEFAULT 'Trống'
                );

                CREATE TABLE IF NOT EXISTS SanPham (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenSanPham  TEXT NOT NULL,
                    GiaCoBan    REAL NOT NULL CHECK(GiaCoBan >= 0),
                    Loai        TEXT NOT NULL,
                    DangBan     INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS HoaDon (
                    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
                    BanId              INTEGER NOT NULL,
                    ThoiGianTao        DATETIME NOT NULL,
                    ThoiGianThanhToan  DATETIME NULL,
                    TongTien           REAL NOT NULL DEFAULT 0,
                    TrangThai          TEXT NOT NULL DEFAULT 'Chưa thanh toán',
                    FOREIGN KEY (BanId) REFERENCES Ban(Id)
                );

                CREATE TABLE IF NOT EXISTS ChiTietHoaDon (
                    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                    HoaDonId      INTEGER NOT NULL,
                    SanPhamId     INTEGER NOT NULL,
                    SoLuong       INTEGER NOT NULL CHECK(SoLuong > 0),
                    DonGiaBan     REAL NOT NULL,
                    ThuocTinhThem TEXT NULL,
                    ThanhTien     REAL NOT NULL,
                    FOREIGN KEY (HoaDonId) REFERENCES HoaDon(Id),
                    FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id)
                );
            ";
            taoHang.ExecuteNonQuery();

            // Seed dữ liệu mẫu nếu bảng đang trống
            NapDuLieuMau(ketNoi);
        }

        /// <summary>
        /// Nạp dữ liệu mẫu ban đầu nếu bảng trống.
        /// </summary>
        private static void NapDuLieuMau(SqliteConnection ketNoi)
        {
            // Kiểm tra bảng SanPham có dữ liệu chưa
            var kiemTra = ketNoi.CreateCommand();
            kiemTra.CommandText = "SELECT COUNT(*) FROM SanPham;";
            var soLuong = (long)(kiemTra.ExecuteScalar() ?? 0);

            if (soLuong == 0)
            {
                // Thêm sản phẩm mẫu: Thức Uống
                var themSanPham = ketNoi.CreateCommand();
                themSanPham.CommandText = @"
                    INSERT INTO SanPham (TenSanPham, GiaCoBan, Loai, DangBan) VALUES
                    ('Cà Phê Đen',   25000, 'ThucUong', 1),
                    ('Cà Phê Sữa',   30000, 'ThucUong', 1),
                    ('Bạc Xỉu',      32000, 'ThucUong', 1),
                    ('Trà Đào',      35000, 'ThucUong', 1),
                    ('Sinh Tố Bơ',   45000, 'ThucUong', 1),
                    ('Nước Cam',     30000, 'ThucUong', 1),
                    ('Bánh Mì',      25000, 'DoAn',     1),
                    ('Bánh Croissant', 30000, 'DoAn',   1),
                    ('Cheesecake',   45000, 'DoAn',     1),
                    ('Khoai Tây Chiên', 35000, 'DoAn',  1);
                ";
                themSanPham.ExecuteNonQuery();
            }

            // Kiểm tra bảng Ban có dữ liệu chưa
            var kiemTraBan = ketNoi.CreateCommand();
            kiemTraBan.CommandText = "SELECT COUNT(*) FROM Ban;";
            var soBan = (long)(kiemTraBan.ExecuteScalar() ?? 0);

            if (soBan == 0)
            {
                var themBan = ketNoi.CreateCommand();
                themBan.CommandText = @"
                    INSERT INTO Ban (TenBan, TrangThai) VALUES
                    ('Bàn 1', 'Trống'),
                    ('Bàn 2', 'Trống'),
                    ('Bàn 3', 'Trống'),
                    ('Bàn 4', 'Trống'),
                    ('Bàn VIP 1', 'Trống'),
                    ('Bàn VIP 2', 'Trống');
                ";
                themBan.ExecuteNonQuery();
            }
        }
    }
}
