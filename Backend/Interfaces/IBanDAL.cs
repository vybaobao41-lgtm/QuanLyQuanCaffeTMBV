using Backend.Models;

namespace Backend.Interfaces
{
    public interface IBanDAL
    {
        List<Ban> LayTatCa();
        Ban? LayTheoId(int id);
        void Them(Ban ban);
        void Sua(Ban ban);
        void Xoa(int id);
        void CapNhatTrangThai(int id, string trangThai);
    }
}
