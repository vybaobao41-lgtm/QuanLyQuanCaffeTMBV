using Microsoft.Data.Sqlite;
using Backend.Interfaces;
using Backend.Models;
using System.Security.Cryptography;
using System.Text;

namespace Backend.DAL
{
    public class TaiKhoanDAL : ITaiKhoanDAL
    {
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = $tenDangNhap AND MatKhau = $matKhau;";
                lenh.Parameters.AddWithValue("$tenDangNhap", tenDangNhap);
                lenh.Parameters.AddWithValue("$matKhau", HashPassword(matKhau));

                var count = (long)(lenh.ExecuteScalar() ?? 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null)
                {
                    ketNoi.Close();
                }
            }
        }

        public bool CoTaiKhoanNaoChua()
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "SELECT COUNT(*) FROM TaiKhoan;";
                var count = (long)(lenh.ExecuteScalar() ?? 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null)
                {
                    ketNoi.Close();
                }
            }
        }

        public bool TaoTaiKhoan(TaiKhoan tk)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "INSERT INTO TaiKhoan (TenDangNhap, MatKhau) VALUES ($tenDangNhap, $matKhau);";
                lenh.Parameters.AddWithValue("$tenDangNhap", tk.TenDangNhap);
                lenh.Parameters.AddWithValue("$matKhau", HashPassword(tk.MatKhau));

                return lenh.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (ketNoi != null)
                {
                    ketNoi.Close();
                }
            }
        }
    }
}
