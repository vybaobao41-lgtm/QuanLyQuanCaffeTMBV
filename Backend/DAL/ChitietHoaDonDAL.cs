using Backend.Interfaces;
using Backend.Models;
using Microsoft.Data.Sqlite;

namespace Backend.DAL
{
    public class ChiTietHoaDonDAL : IChiTietHoaDonDAL
    {
        public List<ChiTietHoaDon> LayTheoHoaDon(int hoaDonId)
        {
            var danhSach = new List<ChiTietHoaDon>();
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                SELECT ct.*, sp.TenSanPham
                FROM ChiTietHoaDon ct
                JOIN SanPham sp ON sp.Id = ct.SanPhamId
                WHERE ct.HoaDonId = $hoaDonId
                ORDER BY ct.Id;
            ";
                lenh.Parameters.AddWithValue("$hoaDonId", hoaDonId);

                using var reader = lenh.ExecuteReader();
                while (reader.Read())
                {
                    var chiTiet = new ChiTietHoaDon
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        HoaDonId = reader.GetInt32(reader.GetOrdinal("HoaDonId")),
                        SanPhamId = reader.GetInt32(reader.GetOrdinal("SanPhamId")),
                        TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                        SoLuong = reader.GetInt32(reader.GetOrdinal("SoLuong")),
                        DonGiaBan = reader.GetDecimal(reader.GetOrdinal("DonGiaBan")),
                        ThanhTien = reader.GetDecimal(reader.GetOrdinal("ThanhTien"))
                    };

                    int idxThuocTinh = reader.GetOrdinal("ThuocTinhThem");
                    if (!reader.IsDBNull(idxThuocTinh))
                        chiTiet.ThuocTinhThem = reader.GetString(idxThuocTinh);

                    danhSach.Add(chiTiet);
                }

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

        public void Them(ChiTietHoaDon chiTiet)
        {
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = @"
                INSERT INTO ChiTietHoaDon (HoaDonId, SanPhamId, SoLuong, DonGiaBan, ThuocTinhThem, ThanhTien)
                VALUES ($hoaDonId, $sanPhamId, $soLuong, $donGia, $thuocTinh, $thanhTien);
            ";
                lenh.Parameters.AddWithValue("$hoaDonId", chiTiet.HoaDonId);
                lenh.Parameters.AddWithValue("$sanPhamId", chiTiet.SanPhamId);
                lenh.Parameters.AddWithValue("$soLuong", chiTiet.SoLuong);
                lenh.Parameters.AddWithValue("$donGia", chiTiet.DonGiaBan);
                lenh.Parameters.AddWithValue("$thuocTinh", (object?)chiTiet.ThuocTinhThem ?? DBNull.Value);
                lenh.Parameters.AddWithValue("$thanhTien", chiTiet.ThanhTien);
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
            SqliteConnection ketNoi = null;
            try
            {
                ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
                ketNoi.Open();

                var lenh = ketNoi.CreateCommand();
                lenh.CommandText = "DELETE FROM ChiTietHoaDon WHERE Id = $id;";
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
