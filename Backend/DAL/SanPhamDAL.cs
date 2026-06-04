using Backend.Interfaces;
using Backend.Models;
using Microsoft.Data.Sqlite;

namespace Backend.DAL
{
    /// <summary>
    /// Lớp truy cập dữ liệu cho Sản Phẩm.
    /// Implement interface ISanPhamDAL (tính trừu tượng).
    /// </summary>
    public class SanPhamDAL : ISanPhamDAL
    {
        /// <summary>
        /// Đọc dữ liệu từ database và tạo đúng loại object (ThucUong hoặc DoAn).
        /// Đây là nơi tính ĐA HÌNH được kích hoạt: dựa vào cột "Loai".
        /// </summary>
        private SanPham DocSanPham(SqliteDataReader reader)
        {
            string loai = reader.GetString(reader.GetOrdinal("Loai"));

            // Dựa vào cột "Loai" để tạo đúng loại object
            SanPham sanPham = loai == "ThucUong" ? new ThucUong() : new DoAn();

            sanPham.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            sanPham.TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham"));
            sanPham.GiaCoBan = reader.GetDecimal(reader.GetOrdinal("GiaCoBan"));
            sanPham.Loai = loai;
            sanPham.DangBan = reader.GetInt32(reader.GetOrdinal("DangBan")) == 1;

            return sanPham;
        }

        public List<SanPham> LayTatCa()
        {
            var danhSach = new List<SanPham>();
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "SELECT * FROM SanPham ORDER BY Loai, TenSanPham;";

                using var reader = lenh.ExecuteReader();
                while (reader.Read())
                    danhSach.Add(DocSanPham(reader));

                return danhSach;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }

        public List<SanPham> LayDangBan()
        {
            var danhSach = new List<SanPham>();
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "SELECT * FROM SanPham WHERE DangBan = 1 ORDER BY Loai, TenSanPham;";

                using var reader = lenh.ExecuteReader();
                while (reader.Read())
                    danhSach.Add(DocSanPham(reader));

                return danhSach;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }

        public SanPham? LayTheoId(int id)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "SELECT * FROM SanPham WHERE Id = $id;";
                lenh.Parameters.AddWithValue("$id", id);

                using var reader = lenh.ExecuteReader();
                return reader.Read() ? DocSanPham(reader) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }

        public void Them(SanPham sanPham)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                INSERT INTO SanPham (TenSanPham, GiaCoBan, Loai, DangBan)
                VALUES ($ten, $gia, $loai, $dangBan);
            ";
                lenh.Parameters.AddWithValue("$ten", sanPham.TenSanPham);
                lenh.Parameters.AddWithValue("$gia", sanPham.GiaCoBan);
                lenh.Parameters.AddWithValue("$loai", sanPham.Loai);
                lenh.Parameters.AddWithValue("$dangBan", sanPham.DangBan ? 1 : 0);
                lenh.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }

        public void Sua(SanPham sanPham)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                UPDATE SanPham 
                SET TenSanPham = $ten, GiaCoBan = $gia, Loai = $loai, DangBan = $dangBan
                WHERE Id = $id;
            ";
                lenh.Parameters.AddWithValue("$ten", sanPham.TenSanPham);
                lenh.Parameters.AddWithValue("$gia", sanPham.GiaCoBan);
                lenh.Parameters.AddWithValue("$loai", sanPham.Loai);
                lenh.Parameters.AddWithValue("$dangBan", sanPham.DangBan ? 1 : 0);
                lenh.Parameters.AddWithValue("$id", sanPham.Id);
                lenh.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }

        public void Xoa(int id)
        {
            // Xóa mềm: chỉ đặt DangBan = 0, không xóa dữ liệu thật sự
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "UPDATE SanPham SET DangBan = 0 WHERE Id = $id;";
                lenh.Parameters.AddWithValue("$id", id);
                lenh.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi CSDL: " + ex.Message);
            }
            finally
            {
                if (ketNoi != null) ketNoi.Close();
            }
        }
    }
}
