namespace Backend.Models
{
    // Thực thể CHI TIẾT HÓA ĐƠN - từng dòng món khách đã gọi
    public class ChiTietHoaDon
    {
        public int Id { get; set; }
        public int HoaDonId { get; set; }
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = ""; // Để hiển thị trên UI
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }  // Giá tại thời điểm gọi món (bao gồm phụ phí)
        public string? ThuocTinhThem { get; set; } // Ví dụ: "Size L", "Ít đường"
        public decimal ThanhTien { get; set; }  // DonGiaBan * SoLuong
    }
}
