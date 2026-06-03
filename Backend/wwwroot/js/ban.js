/**
 * ban.js - Logic cho trang Quản lý Bàn (index.html)
 */
let danhSachBan = [];

async function taiDanhSachBan() {
    const container = document.getElementById('luoi-ban');
    container.innerHTML = '<div class="dang-tai">Đang tải danh sách bàn...</div>';

    const kq = await ApiBan.layTatCa();
    if (!kq.ok) {
        container.innerHTML = `<div class="thong-bao loi" style="display:block">⚠️ ${kq.loi}</div>`;
        return;
    }

    danhSachBan = kq.data;
    hienThiBan(danhSachBan);
}

function hienThiBan(danhSach) {
    const container = document.getElementById('luoi-ban');
    if (!danhSach.length) {
        container.innerHTML = '<div class="dang-tai">Chưa có bàn nào. Hãy thêm bàn mới!</div>';
        return;
    }

    container.innerHTML = danhSach.map(ban => `
        <div class="the-ban ${ban.trangThai === 'Trống' ? 'trong' : 'co-khach'}"
             onclick="chonBanDeQuanLy(${ban.id})">
            <span class="icon-ban">${ban.trangThai === 'Trống' ? '🪑' : '☕'}</span>
            <div class="ten-ban">${ban.tenBan}</div>
            <span class="trang-thai-ban ${ban.trangThai === 'Trống' ? 'trong' : 'co-khach'}">
                ${ban.trangThai}
            </span>
        </div>
    `).join('');
}

function chonBanDeQuanLy(banId) {
    const ban = danhSachBan.find(b => b.id === banId);
    if (!ban) return;

    document.getElementById('sua-ban-id').value = ban.id;
    document.getElementById('sua-ten-ban').value = ban.tenBan;
    document.getElementById('khu-vuc-sua-ban').style.display = 'block';
    document.getElementById('xoa-ban-id').value = ban.id;
    document.getElementById('thong-bao-xoa-ban').textContent = 
        `Bàn "${ban.tenBan}" - Trạng thái: ${ban.trangThai}`;
}

async function themBanMoi() {
    const ten = document.getElementById('ten-ban-moi').value.trim();
    if (!ten) {
        hienThongBao('thong-bao-ban', '⚠️ Vui lòng nhập tên bàn.', false);
        return;
    }

    const kq = await ApiBan.them({ tenBan: ten });
    if (kq.ok) {
        hienThongBao('thong-bao-ban', '✅ Thêm bàn thành công!', true);
        document.getElementById('ten-ban-moi').value = '';
        taiDanhSachBan();
    } else {
        hienThongBao('thong-bao-ban', `❌ ${kq.loi}`, false);
    }
}

async function suaBan() {
    const id = parseInt(document.getElementById('sua-ban-id').value);
    const ten = document.getElementById('sua-ten-ban').value.trim();
    if (!ten) { hienThongBao('thong-bao-ban', '⚠️ Tên bàn không được để trống.', false); return; }

    const kq = await ApiBan.sua(id, { tenBan: ten });
    if (kq.ok) {
        hienThongBao('thong-bao-ban', '✅ Cập nhật tên bàn thành công!', true);
        document.getElementById('khu-vuc-sua-ban').style.display = 'none';
        taiDanhSachBan();
    } else {
        hienThongBao('thong-bao-ban', `❌ ${kq.loi}`, false);
    }
}

async function xoaBan() {
    const id = parseInt(document.getElementById('xoa-ban-id').value);
    if (!id) { hienThongBao('thong-bao-ban', '⚠️ Vui lòng chọn bàn cần xóa.', false); return; }

    if (!confirm('Bạn có chắc muốn xóa bàn này không?')) return;

    const kq = await ApiBan.xoa(id);
    if (kq.ok) {
        hienThongBao('thong-bao-ban', '✅ Xóa bàn thành công!', true);
        document.getElementById('khu-vuc-sua-ban').style.display = 'none';
        taiDanhSachBan();
    } else {
        hienThongBao('thong-bao-ban', `❌ ${kq.loi}`, false);
    }
}

// Khởi chạy khi tải trang
document.addEventListener('DOMContentLoaded', () => {
    taiDanhSachBan();
});
