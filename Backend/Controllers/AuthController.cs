using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITaiKhoanDAL _taiKhoanDAL;

        public AuthController(ITaiKhoanDAL taiKhoanDAL)
        {
            _taiKhoanDAL = taiKhoanDAL;
        }

        [HttpGet("kiem-tra-khoi-tao")]
        public IActionResult KiemTraKhoiTao()
        {
            bool daKhoiTao = _taiKhoanDAL.CoTaiKhoanNaoChua();
            return Ok(new { daKhoiTao });
        }

        [HttpPost("tao-tai-khoan")]
        public IActionResult TaoTaiKhoan([FromBody] TaiKhoan tk)
        {
            if (string.IsNullOrEmpty(tk.TenDangNhap) || string.IsNullOrEmpty(tk.MatKhau))
            {
                return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");
            }

            // Chỉ cho phép tạo nếu chưa có tài khoản nào
            if (_taiKhoanDAL.CoTaiKhoanNaoChua())
            {
                return BadRequest("Hệ thống đã được khởi tạo. Không thể tạo thêm tài khoản quản trị từ bên ngoài.");
            }

            bool thanhCong = _taiKhoanDAL.TaoTaiKhoan(tk);
            if (thanhCong)
            {
                return Ok(new { success = true, message = "Tạo tài khoản thành công." });
            }
            else
            {
                return BadRequest("Tạo tài khoản thất bại (Tên đăng nhập có thể đã tồn tại).");
            }
        }

        [HttpPost("dang-nhap")]
        public IActionResult DangNhap([FromBody] TaiKhoan tk)
        {
            if (string.IsNullOrEmpty(tk.TenDangNhap) || string.IsNullOrEmpty(tk.MatKhau))
            {
                return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");
            }

            bool hopLe = _taiKhoanDAL.KiemTraDangNhap(tk.TenDangNhap, tk.MatKhau);
            if (hopLe)
            {
                return Ok(new { success = true, message = "Đăng nhập thành công" });
            }
            else
            {
                return BadRequest("Sai tên đăng nhập hoặc mật khẩu.");
            }
        }
    }
}
