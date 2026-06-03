using Backend.Models;

namespace Backend.Interfaces
{
    public interface IChiTietHoaDonDAL
    {
        List<ChiTietHoaDon> LayTheoHoaDon(int hoaDonId);
        void Them(ChiTietHoaDon chiTiet);
        void Xoa(int id);
    }
}
