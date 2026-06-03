using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonDAL _hoaDonDAL;
        private readonly IBanDAL _banDAL;
        private readonly IChiTietHoaDonDAL _chiTietDAL;

        public HoaDonController(IHoaDonDAL hoaDonDAL, IBanDAL banDAL, IChiTietHoaDonDAL chiTietDAL)
        {
            _hoaDonDAL = hoaDonDAL;
            _banDAL = banDAL;
            _chiTietDAL = chiTietDAL;
        }

        // GET /api/hoadon - Lấy tất cả hóa đơn (lịch sử)
        [HttpGet]
        public IActionResult LayTatCa()
        {
            try
            {
                return Ok(_hoaDonDAL.LayTatCa());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // GET /api/hoadon/ban/{banId} - Lấy hóa đơn hiện tại của bàn (chưa thanh toán)
        [HttpGet("ban/{banId}")]
        public IActionResult LayHoaDonCuaBan(int banId)
        {
            try
            {
                var hoaDon = _hoaDonDAL.LayHoaDonChuaThanhToan(banId);
                if (hoaDon == null)
                    return NotFound(new { loi = "Bàn này chưa có hóa đơn." });

                // Lấy kèm danh sách chi tiết
                hoaDon.TongTien = _chiTietDAL.LayTheoHoaDon(hoaDon.Id).Sum(ct => ct.ThanhTien);
                return Ok(hoaDon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // POST /api/hoadon/mo-ban - Mở bàn (tạo hóa đơn mới)
        [HttpPost("mo-ban")]
        public IActionResult MoBan([FromBody] MoBanRequest request)
        {
            try
            {
                var ban = _banDAL.LayTheoId(request.BanId);
                if (ban == null)
                    return NotFound(new { loi = "Không tìm thấy bàn." });

                if (ban.TrangThai == "Có khách")
                    return BadRequest(new { loi = "Bàn này đang có khách." });

                // Tạo hóa đơn mới và cập nhật trạng thái bàn
                int hoaDonId = _hoaDonDAL.TaoBan(request.BanId);
                _banDAL.CapNhatTrangThai(request.BanId, "Có khách");

                return Ok(new { thongBao = "Mở bàn thành công.", hoaDonId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // POST /api/hoadon/thanh-toan/{banId} - Thanh toán
        [HttpPost("thanh-toan/{banId}")]
        public IActionResult ThanhToan(int banId)
        {
            try
            {
                var hoaDon = _hoaDonDAL.LayHoaDonChuaThanhToan(banId);
                if (hoaDon == null)
                    return NotFound(new { loi = "Không tìm thấy hóa đơn chưa thanh toán của bàn này." });

                if (hoaDon.TongTien == 0)
                    return BadRequest(new { loi = "Hóa đơn chưa có món nào. Vui lòng thêm món trước khi thanh toán." });

                _hoaDonDAL.ThanhToan(banId);
                return Ok(new { thongBao = "Thanh toán thành công. Bàn đã được giải phóng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }
    }

    public class MoBanRequest
    {
        public int BanId { get; set; }
    }
}
