namespace Backend.Models
{
    // ==============================================================
    // TÍNH TRỪU TƯỢNG (Abstraction): Lớp abstract - không thể tạo
    // đối tượng trực tiếp, buộc phải dùng ThucUong hoặc DoAn.
    // TÍNH ĐA HÌNH (Polymorphism): Phương thức TinhTien() là abstract,
    // mỗi lớp con sẽ override với cách tính khác nhau.
    // ==============================================================
    public abstract class SanPham
    {
        // ============================================================
        // TÍNH ĐÓNG GÓI (Encapsulation): Dùng private field + property
        // để kiểm soát dữ liệu đầu vào (validation).
        // ============================================================
        private int _id;
        private string _tenSanPham = "";
        private decimal _giaCoBan;
        private bool _dangBan;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string TenSanPham
        {
            get => _tenSanPham;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên sản phẩm không được để trống.");
                _tenSanPham = value.Trim();
            }
        }

        public decimal GiaCoBan
        {
            get => _giaCoBan;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Giá sản phẩm không được âm.");
                _giaCoBan = value;
            }
        }

        // Loại sản phẩm: "ThucUong" hoặc "DoAn"
        public string Loai { get; set; } = "";

        public bool DangBan
        {
            get => _dangBan;
            set => _dangBan = value;
        }

        // ============================================================
        // TÍNH ĐA HÌNH: Phương thức abstract, bắt buộc lớp con override.
        // ThuocTinhThem: chuỗi mô tả tùy chọn, ví dụ "Size L", "Ít đường"
        // Trả về giá thực tế sau khi tính thêm phụ phí.
        // ============================================================
        public abstract decimal TinhTien(string? thuocTinhThem);
    }
}
