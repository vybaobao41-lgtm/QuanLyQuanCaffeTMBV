/**
 * api.js - Tập hợp các hàm gọi API tới Backend C#
 * Tất cả hàm đều dùng async/await và trả về { ok, data, loi }
 */

// KIỂM TRA ĐĂNG NHẬP TOÀN CỤC (trừ trang login)
if (!window.location.pathname.includes('login.html')) {
    if (localStorage.getItem('daDangNhap') !== 'true') {
        window.location.href = '/login.html';
    }
}

function dangXuat() {
    localStorage.removeItem('daDangNhap');
    localStorage.removeItem('tenDangNhap');
    window.location.href = '/login.html';
}

const URL_GEC = 'http://localhost:5000/api';

/**
 * Hàm gọi API chung - xử lý lỗi tập trung
 */
async function goiApi(endpoint, phuongThuc = 'GET', duLieu = null) {
    try {
        const tuyChon = {
            method: phuongThuc,
            headers: { 'Content-Type': 'application/json' }
        };
        if (duLieu) tuyChon.body = JSON.stringify(duLieu);

        const phanHoi = await fetch(`${URL_GEC}${endpoint}`, tuyChon);
        const ketQua = await phanHoi.json();

        if (phanHoi.ok) {
            return { ok: true, data: ketQua };
        } else {
            return { ok: false, loi: ketQua.loi || 'Lỗi không xác định từ server.' };
        }
    } catch (loi) {
        return { ok: false, loi: 'Không thể kết nối đến server. Hãy chắc chắn backend đang chạy.' };
    }
}

// ===================== SẢN PHẨM =====================
const ApiSanPham = {
    layDangBan: () => goiApi('/sanpham'),
    layTatCa: () => goiApi('/sanpham/tat-ca'),
    layTheoId: (id) => goiApi(`/sanpham/${id}`),
    them: (duLieu) => goiApi('/sanpham', 'POST', duLieu),
    sua: (id, duLieu) => goiApi(`/sanpham/${id}`, 'PUT', duLieu),
    xoa: (id) => goiApi(`/sanpham/${id}`, 'DELETE')
};

// ===================== BÀN =====================
const ApiBan = {
    layTatCa: () => goiApi('/ban'),
    layTheoId: (id) => goiApi(`/ban/${id}`),
    them: (duLieu) => goiApi('/ban', 'POST', duLieu),
    sua: (id, duLieu) => goiApi(`/ban/${id}`, 'PUT', duLieu),
    xoa: (id) => goiApi(`/ban/${id}`, 'DELETE')
};

// ===================== HÓA ĐƠN =====================
const ApiHoaDon = {
    layTatCa: () => goiApi('/hoadon'),
    layHoaDonCuaBan: (banId) => goiApi(`/hoadon/ban/${banId}`),
    moBan: (banId) => goiApi('/hoadon/mo-ban', 'POST', { banId }),
    thanhToan: (banId) => goiApi(`/hoadon/thanh-toan/${banId}`, 'POST')
};

// ===================== CHI TIẾT HÓA ĐƠN =====================
const ApiChiTiet = {
    layTheoHoaDon: (hoaDonId) => goiApi(`/chitiethoadon/hoadon/${hoaDonId}`),
    themMon: (duLieu) => goiApi('/chitiethoadon', 'POST', duLieu),
    xoaMon: (id) => goiApi(`/chitiethoadon/${id}`, 'DELETE')
};

// ===================== TIỆN ÍCH =====================

/**
 * Định dạng số tiền: 25000 => "25.000 đ"
 */
function dinhDangTien(so) {
    return new Intl.NumberFormat('vi-VN').format(so) + ' đ';
}

/**
 * Định dạng ngày giờ
 */
function dinhDangNgayGio(chuoi) {
    if (!chuoi) return '--';
    const ngay = new Date(chuoi);
    return ngay.toLocaleString('vi-VN', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });
}

/**
 * Hiện thông báo thành công / lỗi
 * @param {string} idPhanTu - ID phần tử HTML chứa thông báo
 * @param {string} noiDung - Nội dung thông báo
 * @param {boolean} thanhCong - true = xanh, false = đỏ
 */
function hienThongBao(idPhanTu, noiDung, thanhCong = true) {
    const phanTu = document.getElementById(idPhanTu);
    if (!phanTu) return;
    phanTu.textContent = noiDung;
    phanTu.className = `thong-bao ${thanhCong ? 'thanh-cong' : 'loi'}`;
    // Tự ẩn sau 4 giây
    setTimeout(() => {
        phanTu.className = 'thong-bao';
        phanTu.textContent = '';
    }, 4000);
}
