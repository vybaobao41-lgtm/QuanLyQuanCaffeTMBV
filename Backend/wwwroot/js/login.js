// login.js - Xử lý đăng nhập và khởi tạo tài khoản

document.addEventListener('DOMContentLoaded', () => {
    // Nếu đã đăng nhập rồi thì chuyển thẳng vào index.html
    if (localStorage.getItem('daDangNhap') === 'true') {
        window.location.href = '/index.html';
        return;
    }
    
    // Kiểm tra xem hệ thống đã có tài khoản nào chưa
    kiemTraKhoiTao();
});

async function kiemTraKhoiTao() {
    try {
        const response = await fetch('/api/auth/kiem-tra-khoi-tao');
        const data = await response.json();
        
        if (data.daKhoiTao === false) {
            // Chưa có tài khoản, hiển thị form đăng ký
            document.getElementById('tieu-de-form').textContent = 'Khởi Tạo Hệ Thống';
            document.getElementById('mo-ta-form').textContent = 'Vui lòng tạo tài khoản quản trị đầu tiên';
            document.getElementById('form-login').style.display = 'none';
            document.getElementById('form-register').style.display = 'block';
        } else {
            // Đã có tài khoản, hiển thị form đăng nhập bình thường
            document.getElementById('tieu-de-form').textContent = 'Đăng Nhập';
            document.getElementById('mo-ta-form').textContent = 'Hệ Thống Quản Lý Quán Cafe';
            document.getElementById('form-login').style.display = 'block';
            document.getElementById('form-register').style.display = 'none';
        }
    } catch (e) {
        console.error("Lỗi kiểm tra khởi tạo:", e);
    }
}

async function xuLyDangNhap() {
    const tenDangNhap = document.getElementById('tenDangNhap').value.trim();
    const matKhau = document.getElementById('matKhau').value;
    
    if (!tenDangNhap || !matKhau) {
        hienThongBao('thong-bao-login', '⚠️ Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.', false);
        return;
    }

    try {
        const response = await fetch('/api/auth/dang-nhap', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ tenDangNhap, matKhau })
        });
        
        if (response.ok) {
            hienThongBao('thong-bao-login', '✅ Đăng nhập thành công!', true);
            localStorage.setItem('daDangNhap', 'true');
            localStorage.setItem('tenDangNhap', tenDangNhap);
            
            setTimeout(() => {
                window.location.href = '/index.html';
            }, 1000);
        } else {
            const errorText = await response.text();
            hienThongBao('thong-bao-login', `❌ ${errorText}`, false);
        }
    } catch (error) {
        hienThongBao('thong-bao-login', '❌ Không thể kết nối tới máy chủ.', false);
    }
}

async function xuLyTaoTaiKhoan() {
    const tenDangNhap = document.getElementById('tenTaoMoi').value.trim();
    const matKhau = document.getElementById('matKhauMoi').value;
    
    if (!tenDangNhap || !matKhau) {
        hienThongBao('thong-bao-register', '⚠️ Vui lòng nhập đầy đủ tên và mật khẩu.', false);
        return;
    }

    try {
        const response = await fetch('/api/auth/tao-tai-khoan', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ tenDangNhap, matKhau })
        });
        
        if (response.ok) {
            hienThongBao('thong-bao-register', '✅ Đã tạo tài khoản thành công! Đang tự động đăng nhập...', true);
            // Sau khi tạo xong, cho đăng nhập luôn
            localStorage.setItem('daDangNhap', 'true');
            localStorage.setItem('tenDangNhap', tenDangNhap);
            
            setTimeout(() => {
                window.location.href = '/index.html';
            }, 1500);
        } else {
            const errorText = await response.text();
            hienThongBao('thong-bao-register', `❌ ${errorText}`, false);
        }
    } catch (error) {
        hienThongBao('thong-bao-register', '❌ Không thể kết nối tới máy chủ.', false);
    }
}

