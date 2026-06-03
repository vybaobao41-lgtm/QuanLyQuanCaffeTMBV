using Microsoft.Data.Sqlite;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.DAL
{
    public class TaiKhoanDAL : ITaiKhoanDAL
    {
        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = $tenDangNhap AND MatKhau = $matKhau;";
            lenh.Parameters.AddWithValue("$tenDangNhap", tenDangNhap);
            lenh.Parameters.AddWithValue("$matKhau", matKhau);

            var count = (long)(lenh.ExecuteScalar() ?? 0);
            return count > 0;
        }

        public bool CoTaiKhoanNaoChua()
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "SELECT COUNT(*) FROM TaiKhoan;";
            var count = (long)(lenh.ExecuteScalar() ?? 0);
            return count > 0;
        }

        public bool TaoTaiKhoan(TaiKhoan tk)
        {
            try
            {
                using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "INSERT INTO TaiKhoan (TenDangNhap, MatKhau) VALUES ($tenDangNhap, $matKhau);";
                lenh.Parameters.AddWithValue("$tenDangNhap", tk.TenDangNhap);
                lenh.Parameters.AddWithValue("$matKhau", tk.MatKhau);

                return lenh.ExecuteNonQuery() > 0;
            }
            catch
            {
                // Thường do lỗi trùng khóa chính (tên đăng nhập đã tồn tại)
                return false;
            }
        }
    }
}
