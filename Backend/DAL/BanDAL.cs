using Backend.Interfaces;
using Backend.Models;
using Microsoft.Data.Sqlite;

namespace Backend.DAL
{
    public class BanDAL : IBanDAL
    {
        private Ban DocBan(SqliteDataReader reader)
        {
            return new Ban
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TenBan = reader.GetString(reader.GetOrdinal("TenBan")),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
            };
        }

        public List<Ban> LayTatCa()
        {
            var danhSach = new List<Ban>();
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "SELECT * FROM Ban ORDER BY Id;";

            using var reader = lenh.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocBan(reader));

            return danhSach;
        }

        public Ban? LayTheoId(int id)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "SELECT * FROM Ban WHERE Id = $id;";
            lenh.Parameters.AddWithValue("$id", id);

            using var reader = lenh.ExecuteReader();
            return reader.Read() ? DocBan(reader) : null;
        }

        public void Them(Ban ban)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "INSERT INTO Ban (TenBan, TrangThai) VALUES ($ten, 'Trống');";
            lenh.Parameters.AddWithValue("$ten", ban.TenBan);
            lenh.ExecuteNonQuery();
        }

        public void Sua(Ban ban)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "UPDATE Ban SET TenBan = $ten WHERE Id = $id;";
            lenh.Parameters.AddWithValue("$ten", ban.TenBan);
            lenh.Parameters.AddWithValue("$id", ban.Id);
            lenh.ExecuteNonQuery();
        }

        public void Xoa(int id)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "DELETE FROM Ban WHERE Id = $id;";
            lenh.Parameters.AddWithValue("$id", id);
            lenh.ExecuteNonQuery();
        }

        public void CapNhatTrangThai(int id, string trangThai)
        {
            using var ketNoi = new SqliteConnection(DatabaseHelper.ChuoiKetNoi);
            ketNoi.Open();

            var lenh = ketNoi.CreateCommand();
            lenh.CommandText = "UPDATE Ban SET TrangThai = $tt WHERE Id = $id;";
            lenh.Parameters.AddWithValue("$tt", trangThai);
            lenh.Parameters.AddWithValue("$id", id);
            lenh.ExecuteNonQuery();
        }
    }
}
