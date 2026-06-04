using Backend.Interfaces;
using Backend.Models;
using Microsoft.Data.Sqlite;

namespace Backend.DAL
{
    public class HoaDonDAL : IHoaDonDAL
    {
        private HoaDon DocHoaDon(SqliteDataReader reader)
        {
            var hd = new HoaDon
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                BanId = reader.GetInt32(reader.GetOrdinal("BanId")),
                ThoiGianTao = reader.GetDateTime(reader.GetOrdinal("ThoiGianTao")),
                TongTien = reader.GetDecimal(reader.GetOrdinal("TongTien")),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
            };

            // ThoiGianThanhToan có thể null
            int idxThanhToan = reader.GetOrdinal("ThoiGianThanhToan");
            if (!reader.IsDBNull(idxThanhToan))
                hd.ThoiGianThanhToan = reader.GetDateTime(idxThanhToan);

            // TenBan (từ JOIN với bảng Ban)
            int idxTenBan = reader.GetOrdinal("TenBan");
            if (!reader.IsDBNull(idxTenBan))
                hd.TenBan = reader.GetString(idxTenBan);

            return hd;
        }

        public HoaDon? LayHoaDonChuaThanhToan(int banId)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                SELECT hd.*, b.TenBan 
                FROM HoaDon hd
                JOIN Ban b ON b.Id = hd.BanId
                WHERE hd.BanId = $banId AND hd.TrangThai = 'Chưa thanh toán'
                LIMIT 1;
            ";
                lenh.Parameters.AddWithValue("$banId", banId);

                using var reader = lenh.ExecuteReader();
                return reader.Read() ? DocHoaDon(reader) : null;
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

        public HoaDon? LayTheoId(int id)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                SELECT hd.*, b.TenBan 
                FROM HoaDon hd
                JOIN Ban b ON b.Id = hd.BanId
                WHERE hd.Id = $id;
            ";
                lenh.Parameters.AddWithValue("$id", id);

                using var reader = lenh.ExecuteReader();
                return reader.Read() ? DocHoaDon(reader) : null;
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

        public List<HoaDon> LayTatCa()
        {
            var danhSach = new List<HoaDon>();
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                SELECT hd.*, b.TenBan 
                FROM HoaDon hd
                JOIN Ban b ON b.Id = hd.BanId
                ORDER BY hd.ThoiGianTao DESC;
            ";

                using var reader = lenh.ExecuteReader();
                while (reader.Read())
                    danhSach.Add(DocHoaDon(reader));

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

        public int TaoBan(int banId)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                INSERT INTO HoaDon (BanId, ThoiGianTao, TongTien, TrangThai)
                VALUES ($banId, $thoiGian, 0, 'Chưa thanh toán');
                SELECT last_insert_rowid();
            ";
                lenh.Parameters.AddWithValue("$banId", banId);
                lenh.Parameters.AddWithValue("$thoiGian", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                return Convert.ToInt32(lenh.ExecuteScalar());
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

        public void CapNhatTongTien(int hoaDonId, decimal tongTien)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "UPDATE HoaDon SET TongTien = $tongTien WHERE Id = $id;";
                lenh.Parameters.AddWithValue("$tongTien", tongTien);
                lenh.Parameters.AddWithValue("$id", hoaDonId);
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

        public void ThanhToan(int banId)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                // Cập nhật hóa đơn: ghi thời gian thanh toán và đổi trạng thái
                var lenhHD = ketNoi.CreateCommand();
                lenhHD.CommandText = @"
                UPDATE HoaDon 
                SET ThoiGianThanhToan = $thoiGian, TrangThai = 'Đã thanh toán'
                WHERE BanId = $banId AND TrangThai = 'Chưa thanh toán';
            ";
                lenhHD.Parameters.AddWithValue("$thoiGian", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                lenhHD.Parameters.AddWithValue("$banId", banId);
                lenhHD.ExecuteNonQuery();

                // Cập nhật trạng thái bàn về "Trống"
                var lenhBan = ketNoi.CreateCommand();
                lenhBan.CommandText = "UPDATE Ban SET TrangThai = 'Trống' WHERE Id = $banId;";
                lenhBan.Parameters.AddWithValue("$banId", banId);
                lenhBan.ExecuteNonQuery();
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
