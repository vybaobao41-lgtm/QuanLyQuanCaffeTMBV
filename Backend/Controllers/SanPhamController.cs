using Backend.DAL;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamController : ControllerBase
    {
        private readonly ISanPhamDAL _sanPhamDAL;

        // Dùng Dependency Injection - Controller nhận interface, không biết về class cụ thể
        public SanPhamController(ISanPhamDAL sanPhamDAL)
        {
            _sanPhamDAL = sanPhamDAL;
        }

        // GET /api/sanpham - Lấy các món đang bán (dùng cho giao diện gọi món)
        [HttpGet]
        public IActionResult LayDanhSachDangBan()
        {
            try
            {
                var danhSach = _sanPhamDAL.LayDangBan();
                return Ok(danhSach);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi khi lấy danh sách món: {ex.Message}" });
            }
        }

        // GET /api/sanpham/tat-ca - Lấy tất cả (dùng cho trang quản lý menu)
        [HttpGet("tat-ca")]
        public IActionResult LayTatCa()
        {
            try
            {
                var danhSach = _sanPhamDAL.LayTatCa();
                return Ok(danhSach);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi: {ex.Message}" });
            }
        }

        // GET /api/sanpham/{id}
        [HttpGet("{id}")]
        public IActionResult LayTheoId(int id)
        {
            try
            {
                var sanPham = _sanPhamDAL.LayTheoId(id);
                if (sanPham == null)
                    return NotFound(new { loi = "Không tìm thấy sản phẩm." });
                return Ok(sanPham);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = ex.Message });
            }
        }

        // POST /api/sanpham - Thêm sản phẩm mới
        [HttpPost]
        public IActionResult Them([FromBody] SanPhamRequest request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.TenSanPham))
                    return BadRequest(new { loi = "Tên sản phẩm không được để trống." });
                if (request.GiaCoBan < 0)
                    return BadRequest(new { loi = "Giá sản phẩm không được âm." });
                if (request.Loai != "ThucUong" && request.Loai != "DoAn")
                    return BadRequest(new { loi = "Loại sản phẩm phải là 'ThucUong' hoặc 'DoAn'." });

                // Tạo đúng loại object nhờ tính đa hình
                SanPham sanPham = request.Loai == "ThucUong" ? new ThucUong() : new DoAn();
                sanPham.TenSanPham = request.TenSanPham;
                sanPham.GiaCoBan = request.GiaCoBan;
                sanPham.DangBan = true;

                _sanPhamDAL.Them(sanPham);
                return Ok(new { thongBao = "Thêm sản phẩm thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { loi = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        // PUT /api/sanpham/{id} - Sửa sản phẩm
        [HttpPut("{id}")]
        public IActionResult Sua(int id, [FromBody] SanPhamRequest request)
        {
            try
            {
                var sanPhamHienTai = _sanPhamDAL.LayTheoId(id);
                if (sanPhamHienTai == null)
                    return NotFound(new { loi = "Không tìm thấy sản phẩm." });

                if (string.IsNullOrWhiteSpace(request.TenSanPham))
                    return BadRequest(new { loi = "Tên sản phẩm không được để trống." });
                if (request.GiaCoBan < 0)
                    return BadRequest(new { loi = "Giá sản phẩm không được âm." });

                // Tạo object mới với loại đúng
                SanPham sanPhamMoi = request.Loai == "ThucUong" ? new ThucUong() : new DoAn();
                sanPhamMoi.Id = id;
                sanPhamMoi.TenSanPham = request.TenSanPham;
                sanPhamMoi.GiaCoBan = request.GiaCoBan;
                sanPhamMoi.DangBan = request.DangBan;

                _sanPhamDAL.Sua(sanPhamMoi);
                return Ok(new { thongBao = "Cập nhật sản phẩm thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { loi = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        // DELETE /api/sanpham/{id} - Ẩn sản phẩm (xóa mềm)
        [HttpDelete("{id}")]
        public IActionResult Xoa(int id)
        {
            try
            {
                var sanPham = _sanPhamDAL.LayTheoId(id);
                if (sanPham == null)
                    return NotFound(new { loi = "Không tìm thấy sản phẩm." });

                _sanPhamDAL.Xoa(id);
                return Ok(new { thongBao = "Đã ẩn sản phẩm khỏi menu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { loi = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }

    // DTO (Data Transfer Object) - nhận dữ liệu từ Frontend
    public class SanPhamRequest
    {
        public string TenSanPham { get; set; } = "";
        public decimal GiaCoBan { get; set; }
        public string Loai { get; set; } = "";
        public bool DangBan { get; set; } = true;
    }
}
