using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietHoaDonController : ControllerBase
    {
        private readonly IChiTietHoaDonDAL _chiTietDAL;
        private readonly ISanPhamDAL _sanPhamDAL;
        private readonly IHoaDonDAL _hoaDonDAL;

        public ChiTietHoaDonController(IChiTietHoaDonDAL chiTietDAL, ISanPhamDAL sanPhamDAL, IHoaDonDAL hoaDonDAL)
        {
            _chiTietDAL = chiTietDAL;
            _sanPhamDAL = sanPhamDAL;
            _hoaDonDAL = hoaDonDAL;
        }

        // GET /api/chitiethoadon/hoadon/{hoaDonId} - Lấy danh sách món của hóa đơn
        [HttpGet("hoadon/{hoaDonId}")]
        public IActionResult LayTheoHoaDon(int hoaDonId)
        {
            try
            {
                var danhSach = _chiTietDAL.LayTheoHoaDon(hoaDonId);
                return Ok(danhSach);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // POST /api/chitiethoadon - Thêm món vào hóa đơn
        [HttpPost]
        public IActionResult ThemMon([FromBody] ThemMonRequest request)
        {
            try
            {
                // Validation
                if (request.SoLuong <= 0)
                    return BadRequest(new { loi = "Số lượng phải lớn hơn 0." });

                // Lấy sản phẩm và tạo đúng loại object
                var sanPham = _sanPhamDAL.LayTheoId(request.SanPhamId);
                if (sanPham == null)
                    return NotFound(new { loi = "Không tìm thấy sản phẩm." });
                if (!sanPham.DangBan)
                    return BadRequest(new { loi = "Sản phẩm này hiện không còn phục vụ." });

                // Kiểm tra hóa đơn tồn tại
                var hoaDon = _hoaDonDAL.LayTheoId(request.HoaDonId);
                if (hoaDon == null)
                    return NotFound(new { loi = "Không tìm thấy hóa đơn." });
                if (hoaDon.TrangThai == "Đã thanh toán")
                    return BadRequest(new { loi = "Hóa đơn này đã được thanh toán." });

                // ===========================================================
                // ĐA HÌNH: Gọi TinhTien() - tự động chọn đúng phương thức
                // của ThucUong hoặc DoAn dựa vào loại thực tế của sanPham.
                // ===========================================================
                decimal donGiaBan = sanPham.TinhTien(request.ThuocTinhThem);
                decimal thanhTien = donGiaBan * request.SoLuong;

                var chiTiet = new ChiTietHoaDon
                {
                    HoaDonId = request.HoaDonId,
                    SanPhamId = request.SanPhamId,
                    SoLuong = request.SoLuong,
                    DonGiaBan = donGiaBan,
                    ThuocTinhThem = request.ThuocTinhThem,
                    ThanhTien = thanhTien
                };
                _chiTietDAL.Them(chiTiet);

                // Cập nhật tổng tiền hóa đơn
                var tatCaMon = _chiTietDAL.LayTheoHoaDon(request.HoaDonId);
                decimal tongTienMoi = tatCaMon.Sum(ct => ct.ThanhTien);
                _hoaDonDAL.CapNhatTongTien(request.HoaDonId, tongTienMoi);

                return Ok(new
                {
                    thongBao = "Thêm món thành công.",
                    donGiaBan,
                    thanhTien,
                    tongTien = tongTienMoi
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        // DELETE /api/chitiethoadon/{id} - Xóa món khỏi hóa đơn
        [HttpDelete("{id}")]
        public IActionResult XoaMon(int id)
        {
            try
            {
                _chiTietDAL.Xoa(id);
                return Ok(new { thongBao = "Đã xóa món khỏi hóa đơn." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }
    }

    public class ThemMonRequest
    {
        public int HoaDonId { get; set; }
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
        public string? ThuocTinhThem { get; set; }
    }
}
