namespace Backend.Models
{
    // Thực thể HÓA ĐƠN - thông tin tổng quát một phiên khách
    public class HoaDon
    {
        public int Id { get; set; }
        public int BanId { get; set; }
        public string TenBan { get; set; } = ""; // Thêm để hiển thị UI
        public DateTime ThoiGianTao { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; } // Null = chưa thanh toán
        public decimal TongTien { get; set; } = 0;
        // Trạng thái: "Chưa thanh toán" hoặc "Đã thanh toán"
        public string TrangThai { get; set; } = "Chưa thanh toán";
    }
}
