namespace Backend.Models
{
    // ============================================================
    // TÍNH KẾ THỪA (Inheritance): DoAn kế thừa SanPham.
    // TÍNH ĐA HÌNH (Polymorphism): Override TinhTien() - Đồ ăn
    // không có phụ phí thêm, giá cố định.
    // ============================================================
    public class DoAn : SanPham
    {
        public DoAn()
        {
            Loai = "DoAn";
        }

        /// <summary>
        /// Tính tiền cho Đồ Ăn.
        /// Đồ ăn không có phụ phí thêm, trả về giá cơ bản.
        /// </summary>
        public override decimal TinhTien(string? thuocTinhThem)
        {
            // Đồ ăn: không tính thêm phụ phí, trả về giá gốc
            return GiaCoBan;
        }
    }
}
