using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanController : ControllerBase
    {
        private readonly IBanDAL _banDAL;

        public BanController(IBanDAL banDAL)
        {
            _banDAL = banDAL;
        }

        // GET /api/ban
        [HttpGet]
        public IActionResult LayTatCa()
        {
            try
            {
                return Ok(_banDAL.LayTatCa());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi: {ex.Message}" });
            }
        }

        // GET /api/ban/{id}
        [HttpGet("{id}")]
        public IActionResult LayTheoId(int id)
        {
            try
            {
                var ban = _banDAL.LayTheoId(id);
                if (ban == null)
                    return NotFound(new { loi = "Không tìm thấy bàn." });
                return Ok(ban);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // POST /api/ban - Thêm bàn mới
        [HttpPost]
        public IActionResult Them([FromBody] BanRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.TenBan))
                    return BadRequest(new { loi = "Tên bàn không được để trống." });

                var ban = new Ban { TenBan = request.TenBan };
                _banDAL.Them(ban);
                return Ok(new { thongBao = "Thêm bàn thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { loi = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi: {ex.Message}" });
            }
        }

        // PUT /api/ban/{id} - Sửa tên bàn
        [HttpPut("{id}")]
        public IActionResult Sua(int id, [FromBody] BanRequest request)
        {
            try
            {
                var ban = _banDAL.LayTheoId(id);
                if (ban == null)
                    return NotFound(new { loi = "Không tìm thấy bàn." });

                if (string.IsNullOrWhiteSpace(request.TenBan))
                    return BadRequest(new { loi = "Tên bàn không được để trống." });

                ban.TenBan = request.TenBan;
                _banDAL.Sua(ban);
                return Ok(new { thongBao = "Cập nhật tên bàn thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { loi = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi: {ex.Message}" });
            }
        }

        // DELETE /api/ban/{id} - Xóa bàn
        [HttpDelete("{id}")]
        public IActionResult Xoa(int id)
        {
            try
            {
                var ban = _banDAL.LayTheoId(id);
                if (ban == null)
                    return NotFound(new { loi = "Không tìm thấy bàn." });
                if (ban.TrangThai == "Có khách")
                    return BadRequest(new { loi = "Không thể xóa bàn đang có khách." });

                _banDAL.Xoa(id);
                return Ok(new { thongBao = "Xóa bàn thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi: {ex.Message}" });
            }
        }
    }

    public class BanRequest
    {
        public string TenBan { get; set; } = "";
    }
}
