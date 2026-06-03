using Backend.Models;

namespace Backend.Interfaces
{
    // ============================================================
    // TÍNH TRỪU TƯỢNG (Abstraction): Interface định nghĩa "hợp đồng"
    // cho tầng truy cập dữ liệu. DAL phải implement đúng các method này.
    // ============================================================
    public interface ISanPhamDAL
    {
        List<SanPham> LayTatCa();           // Lấy tất cả (kể cả ẩn)
        List<SanPham> LayDangBan();         // Chỉ lấy món đang bán
        SanPham? LayTheoId(int id);
        void Them(SanPham sanPham);
        void Sua(SanPham sanPham);
        void Xoa(int id);                   // Xóa mềm: đặt DangBan = false
    }
}
