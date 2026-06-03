namespace Backend.Models
{
    // ============================================================
    // TÍNH KẾ THỪA (Inheritance): ThucUong kế thừa SanPham.
    // TÍNH ĐA HÌNH (Polymorphism): Override TinhTien() - Thức uống
    // tính thêm 5.000đ nếu chọn "Size L".
    // ============================================================
    public class ThucUong : SanPham
    {
        // Phụ phí thêm khi chọn size lớn (5.000đ)
        private const decimal PHU_PHI_SIZE_L = 5000m;

        public ThucUong()
        {
            Loai = "ThucUong";
        }

        /// <summary>
        /// Tính tiền cho Thức Uống.
        /// Nếu thuocTinhThem chứa "Size L" => cộng thêm 5.000đ phụ phí.
        /// </summary>
        public override decimal TinhTien(string? thuocTinhThem)
        {
            decimal gia = GiaCoBan;

            // Kiểm tra tùy chọn size, nếu là "Size L" thì cộng thêm
            if (!string.IsNullOrWhiteSpace(thuocTinhThem) &&
                thuocTinhThem.Contains("Size L", StringComparison.OrdinalIgnoreCase))
            {
                gia += PHU_PHI_SIZE_L;
            }

            return gia;
        }
    }
}
