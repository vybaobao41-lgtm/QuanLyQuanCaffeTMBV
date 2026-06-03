/**
 * lichsu.js - Logic cho trang Lịch sử hóa đơn (lichsu.html)
 */

async function taiLichSu() {
    const tbody = document.getElementById('bang-lich-su-body');
    tbody.innerHTML = '<tr><td colspan="6" class="dang-tai">Đang tải lịch sử...</td></tr>';

    const kq = await ApiHoaDon.layTatCa();
    if (!kq.ok) {
        tbody.innerHTML = `<tr><td colspan="6" class="thong-bao loi" style="display:block">❌ ${kq.loi}</td></tr>`;
        return;
    }

    if (!kq.data.length) {
        tbody.innerHTML = '<tr><td colspan="6" class="dang-tai">Chưa có hóa đơn nào.</td></tr>';
        return;
    }

    tbody.innerHTML = kq.data.map(hd => `
        <tr>
            <td>#${hd.id}</td>
            <td>${hd.tenBan}</td>
            <td>${dinhDangNgayGio(hd.thoiGianTao)}</td>
            <td>${dinhDangNgayGio(hd.thoiGianThanhToan)}</td>
            <td class="gia-tien">${dinhDangTien(hd.tongTien)}</td>
            <td>
                <span class="badge ${hd.trangThai === 'Đã thanh toán' ? 'badge-da-tt' : 'badge-chua-tt'}">
                    ${hd.trangThai}
                </span>
            </td>
        </tr>
    `).join('');
}

// Khởi chạy khi tải trang
document.addEventListener('DOMContentLoaded', () => {
    taiLichSu();
});
