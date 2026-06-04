/**
 * order.js - Logic cho trang Gọi món & Thanh toán (order.html)
 */
let banDangChon = null;      // Bàn đang được chọn
let hoaDonHienTai = null;    // Hóa đơn đang mở
let danhSachSanPham = [];    // Cache danh sách sản phẩm
let danhSachBan = [];        // Cache danh sách bàn

async function taiTrangOrder() {
    // Tải danh sách bàn vào combobox
    const kqBan = await ApiBan.layTatCa();
    if (kqBan.ok) {
        const select = document.getElementById('cboChonBanOrder');
        select.innerHTML = '<option value="">-- Chọn bàn --</option>' +
            kqBan.data.map(b => `<option value="${b.id}">${b.tenBan} (${b.trangThai})</option>`).join('');
        danhSachBan = kqBan.data;
    }

    // Tải danh sách sản phẩm đang bán
    const kqSP = await ApiSanPham.layDangBan();
    if (kqSP.ok) {
        danhSachSanPham = kqSP.data;
        const select = document.getElementById('cboChonSanPham');
        select.innerHTML = '<option value="">-- Chọn món --</option>' +
            kqSP.data.map(sp => `
                <option value="${sp.id}" data-loai="${sp.loai}" data-gia="${sp.giaCoBan}">
                    ${sp.loai === 'ThucUong' ? '🥤' : '🍽️'} ${sp.tenSanPham} - ${dinhDangTien(sp.giaCoBan)}
                </option>
            `).join('');
    }
}

async function chonBanOrder() {
    const banId = parseInt(document.getElementById('cboChonBanOrder').value);
    if (!banId) {
        banDangChon = null;
        hoaDonHienTai = null;
        document.getElementById('khu-vuc-hoa-don').style.display = 'none';
        document.getElementById('khu-vuc-mo-ban').style.display = 'none';
        return;
    }

    const ban = danhSachBan.find(b => b.id === banId);
    banDangChon = ban;

    if (ban.trangThai === 'Trống') {
        // Bàn trống: hiện nút mở bàn
        document.getElementById('khu-vuc-hoa-don').style.display = 'none';
        document.getElementById('khu-vuc-mo-ban').style.display = 'block';
        document.getElementById('ten-ban-mo').textContent = ban.tenBan;
    } else {
        // Bàn có khách: lấy hóa đơn hiện tại
        document.getElementById('khu-vuc-mo-ban').style.display = 'none';
        await taiHoaDonHienTai(banId);
    }
}

async function moBan() {
    if (!banDangChon) return;

    const kq = await ApiHoaDon.moBan(banDangChon.id);
    if (kq.ok) {
        hienThongBao('thong-bao-order', `✅ Đã mở ${banDangChon.tenBan}!`, true);
        document.getElementById('khu-vuc-mo-ban').style.display = 'none';
        // Cập nhật combobox và tải hóa đơn
        await taiTrangOrder();
        document.getElementById('cboChonBanOrder').value = banDangChon.id;
        await taiHoaDonHienTai(banDangChon.id);
    } else {
        hienThongBao('thong-bao-order', `❌ ${kq.loi}`, false);
    }
}

async function taiHoaDonHienTai(banId) {
    const kq = await ApiHoaDon.layHoaDonCuaBan(banId);
    if (!kq.ok) {
        document.getElementById('khu-vuc-hoa-don').style.display = 'none';
        return;
    }

    hoaDonHienTai = kq.data;
    document.getElementById('khu-vuc-hoa-don').style.display = 'block';
    document.getElementById('ten-ban-hoa-don').textContent = hoaDonHienTai.tenBan;
    document.getElementById('gio-mo-ban').textContent = dinhDangNgayGio(hoaDonHienTai.thoiGianTao);

    await taiDanhSachMonDaGoi();
}

async function taiDanhSachMonDaGoi() {
    if (!hoaDonHienTai) return;

    const kq = await ApiChiTiet.layTheoHoaDon(hoaDonHienTai.id);
    const container = document.getElementById('danh-sach-mon-goi');

    if (!kq.ok || !kq.data.length) {
        container.innerHTML = '<div class="dang-tai">Chưa có món nào được gọi.</div>';
        document.getElementById('so-tong-tien').textContent = dinhDangTien(0);
        return;
    }

    let tongTien = 0;
    container.innerHTML = kq.data.map(ct => {
        tongTien += ct.thanhTien;
        return `
        <div class="dong-mon">
            <div>
                <strong>${ct.tenSanPham}</strong>
                ${ct.thuocTinhThem ? `<span class="chu-nho"> • ${ct.thuocTinhThem}</span>` : ''}
                <div class="chu-nho">x${ct.soLuong} × ${dinhDangTien(ct.donGiaBan)}</div>
            </div>
            <div class="flex-hang">
                <span class="gia-tien">${dinhDangTien(ct.thanhTien)}</span>
                <button class="btn btn-do btn-nho" onclick="xoaMonTrongHoaDon(${ct.id})">✕</button>
            </div>
        </div>
    `}).join('');

    document.getElementById('so-tong-tien').textContent = dinhDangTien(tongTien);
}

function kiemTraLoaiSanPham() {
    const select = document.getElementById('cboChonSanPham');
    const option = select.options[select.selectedIndex];
    const loai = option?.dataset?.loai || '';
    const khuVucSize = document.getElementById('khu-vuc-tuy-chon');
    khuVucSize.style.display = loai === 'ThucUong' ? 'block' : 'none';
}

async function themMonVaoHoaDon() {
    if (!hoaDonHienTai) {
        hienThongBao('thong-bao-order', '⚠️ Vui lòng chọn bàn và mở hóa đơn trước.', false);
        return;
    }

    const sanPhamId = parseInt(document.getElementById('cboChonSanPham').value);
    const soLuong = parseInt(document.getElementById('txtSoLuong').value);
    const size = document.getElementById('cboChonSize').value;
    const ghiChu = document.getElementById('txtGhiChu').value.trim();

    if (!sanPhamId) { hienThongBao('thong-bao-order', '⚠️ Vui lòng chọn món.', false); return; }
    if (!soLuong || soLuong <= 0) { hienThongBao('thong-bao-order', '⚠️ Số lượng phải >= 1.', false); return; }

    // Kết hợp size và ghi chú vào thuộc tính thêm
    let thuocTinhThem = '';
    if (size) thuocTinhThem += size;
    if (ghiChu) thuocTinhThem += (thuocTinhThem ? ', ' : '') + ghiChu;

    const kq = await ApiChiTiet.themMon({
        hoaDonId: hoaDonHienTai.id,
        sanPhamId,
        soLuong,
        thuocTinhThem: thuocTinhThem || null
    });

    if (kq.ok) {
        hienThongBao('thong-bao-order', `✅ Đã thêm món! Đơn giá: ${dinhDangTien(kq.data.donGiaBan)}`, true);
        // Reset form chọn món
        document.getElementById('cboChonSanPham').value = '';
        document.getElementById('txtSoLuong').value = '1';
        document.getElementById('cboChonSize').value = '';
        document.getElementById('txtGhiChu').value = '';
        document.getElementById('khu-vuc-tuy-chon').style.display = 'none';
        await taiDanhSachMonDaGoi();
    } else {
        hienThongBao('thong-bao-order', `❌ ${kq.loi}`, false);
    }
}

async function xoaMonTrongHoaDon(chiTietId) {
    if (!confirm('Bạn có chắc muốn xóa món này?')) return;

    const kq = await ApiChiTiet.xoaMon(chiTietId);
    if (kq.ok) {
        await taiDanhSachMonDaGoi();
        // Cập nhật lại tổng tiền trong hóa đơn
        const kqHD = await ApiHoaDon.layHoaDonCuaBan(banDangChon.id);
        if (kqHD.ok) hoaDonHienTai = kqHD.data;
    } else {
        hienThongBao('thong-bao-order', `❌ ${kq.loi}`, false);
    }
}

async function thanhToan() {
    if (!banDangChon || !hoaDonHienTai) return;

    const tongTienText = document.getElementById('so-tong-tien').textContent;
    if (!confirm(`Xác nhận thanh toán ${tongTienText} cho ${hoaDonHienTai.tenBan}?`)) return;

    const kq = await ApiHoaDon.thanhToan(banDangChon.id);
    if (kq.ok) {
        hienThongBao('thong-bao-order', `✅ Thanh toán thành công! ${hoaDonHienTai.tenBan} đã được giải phóng.`, true);
        // Reset giao diện
        hoaDonHienTai = null;
        banDangChon = null;
        document.getElementById('khu-vuc-hoa-don').style.display = 'none';
        await taiTrangOrder();
        document.getElementById('cboChonBanOrder').value = '';
    } else {
        hienThongBao('thong-bao-order', `❌ ${kq.loi}`, false);
    }
}

// Khởi chạy khi tải trang
document.addEventListener('DOMContentLoaded', () => {
    taiTrangOrder();
});
