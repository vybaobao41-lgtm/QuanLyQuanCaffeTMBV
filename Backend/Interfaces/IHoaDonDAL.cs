using Backend.Models;

namespace Backend.Interfaces
{
    public interface IHoaDonDAL
    {
        HoaDon? LayHoaDonChuaThanhToan(int banId);
        HoaDon? LayTheoId(int id);
        List<HoaDon> LayTatCa();
        int TaoBan(int banId);                      // Tạo hóa đơn mới, trả về Id
        void CapNhatTongTien(int hoaDonId, decimal tongTien);
        void ThanhToan(int banId);                  // Chốt thanh toán theo bàn
    }
}
