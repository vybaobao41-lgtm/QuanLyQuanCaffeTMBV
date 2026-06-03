namespace Backend.Models
{
    // Thực thể BÀN - lưu trạng thái từng bàn trong quán
    public class Ban
    {
        private int _id;
        private string _tenBan = "";

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string TenBan
        {
            get => _tenBan;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên bàn không được để trống.");
                _tenBan = value.Trim();
            }
        }

        // Trạng thái: "Trống" hoặc "Có khách"
        public string TrangThai { get; set; } = "Trống";
    }
}
