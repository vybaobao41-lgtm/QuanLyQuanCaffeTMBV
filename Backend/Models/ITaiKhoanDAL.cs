namespace Backend.Interfaces
{
    public interface ITaiKhoanDAL
    {
        bool KiemTraDangNhap(string tenDangNhap, string matKhau);
        bool CoTaiKhoanNaoChua();
        bool TaoTaiKhoan(Models.TaiKhoan tk);
    }
}
