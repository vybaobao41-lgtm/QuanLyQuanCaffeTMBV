/**
 * menu.js - Logic cho trang Quản lý Thực đơn (menu.html)
 */
let danhSachSanPham = [];

async function taiDanhSachMenu() {
    const tbody = document.getElementById('bang-menu-body');
    tbody.innerHTML = '<tr><td colspan="6" class="dang-tai">Đang tải menu...</td></tr>';

    const kq = await ApiSanPham.layTatCa();
    if (!kq.ok) {
        tbody.innerHTML = `<tr><td colspan="6" class="thong-bao loi" style="display:block">❌ ${kq.loi}</td></tr>`;
        return;
    }

    danhSachSanPham = kq.data;
    if (!danhSachSanPham.length) {
        tbody.innerHTML = '<tr><td colspan="6" class="dang-tai">Chưa có sản phẩm nào.</td></tr>';
        return;
    }

    tbody.innerHTML = danhSachSanPham.map(sp => `
        <tr>
            <td>${sp.id}</td>
            <td><strong>${sp.tenSanPham}</strong></td>
            <td>
                <span class="badge ${sp.loai === 'ThucUong' ? 'badge-thuc-uong' : 'badge-do-an'}">
                    ${sp.loai === 'ThucUong' ? '🥤 Thức Uống' : '🍽️ Đồ Ăn'}
                </span>
            </td>
            <td class="gia-tien">${dinhDangTien(sp.giaCoBan)}</td>
            <td>
                <span class="badge ${sp.dangBan ? 'badge-da-tt' : 'badge-chua-tt'}">
                    ${sp.dangBan ? '✅ Đang bán' : '🚫 Tạm ẩn'}
                </span>
            </td>
            <td>
                <button class="btn btn-xam btn-nho" onclick="moModalSua(${sp.id})">✏️ Sửa</button>
                <button class="btn btn-do btn-nho" onclick="xoaSanPham(${sp.id})">🗑️ Ẩn</button>
            </td>
        </tr>
    `).join('');
}

function moModalThem() {
    document.getElementById('modal-them-sp').classList.add('hien');
    document.getElementById('form-them-sp').reset();
    hienThongBao('thong-bao-them-sp', '', true);
}

function dongModal(modalId) {
    document.getElementById(modalId).classList.remove('hien');
}

async function themSanPham() {
    const ten = document.getElementById('them-ten-sp').value.trim();
    const gia = parseFloat(document.getElementById('them-gia-sp').value);
    const loai = document.getElementById('them-loai-sp').value;

    if (!ten) { hienThongBao('thong-bao-them-sp', '⚠️ Tên sản phẩm không được để trống.', false); return; }
    if (isNaN(gia) || gia < 0) { hienThongBao('thong-bao-them-sp', '⚠️ Giá phải >= 0.', false); return; }

    const kq = await ApiSanPham.them({ tenSanPham: ten, giaCoBan: gia, loai, dangBan: true });
    if (kq.ok) {
        hienThongBao('thong-bao-them-sp', '✅ Thêm sản phẩm thành công!', true);
        setTimeout(() => { dongModal('modal-them-sp'); taiDanhSachMenu(); }, 1000);
    } else {
        hienThongBao('thong-bao-them-sp', `❌ ${kq.loi}`, false);
    }
}

async function moModalSua(id) {
    const sp = danhSachSanPham.find(s => s.id === id);
    if (!sp) return;

    document.getElementById('sua-sp-id').value = sp.id;
    document.getElementById('sua-ten-sp').value = sp.tenSanPham;
    document.getElementById('sua-gia-sp').value = sp.giaCoBan;
    document.getElementById('sua-loai-sp').value = sp.loai;
    document.getElementById('sua-dang-ban-sp').value = sp.dangBan ? '1' : '0';
    document.getElementById('modal-sua-sp').classList.add('hien');
    hienThongBao('thong-bao-sua-sp', '', true);
}

async function luuSuaSanPham() {
    const id = parseInt(document.getElementById('sua-sp-id').value);
    const ten = document.getElementById('sua-ten-sp').value.trim();
    const gia = parseFloat(document.getElementById('sua-gia-sp').value);
    const loai = document.getElementById('sua-loai-sp').value;
    const dangBan = document.getElementById('sua-dang-ban-sp').value === '1';

    if (!ten) { hienThongBao('thong-bao-sua-sp', '⚠️ Tên không được để trống.', false); return; }
    if (isNaN(gia) || gia < 0) { hienThongBao('thong-bao-sua-sp', '⚠️ Giá phải >= 0.', false); return; }

    const kq = await ApiSanPham.sua(id, { tenSanPham: ten, giaCoBan: gia, loai, dangBan });
    if (kq.ok) {
        hienThongBao('thong-bao-sua-sp', '✅ Cập nhật thành công!', true);
        setTimeout(() => { dongModal('modal-sua-sp'); taiDanhSachMenu(); }, 1000);
    } else {
        hienThongBao('thong-bao-sua-sp', `❌ ${kq.loi}`, false);
    }
}

async function xoaSanPham(id) {
    const sp = danhSachSanPham.find(s => s.id === id);
    if (!confirm(`Bạn có muốn ẩn "${sp?.tenSanPham}" khỏi menu không?`)) return;

    const kq = await ApiSanPham.xoa(id);
    if (kq.ok) {
        hienThongBao('thong-bao-menu', '✅ Đã ẩn sản phẩm khỏi menu!', true);
        taiDanhSachMenu();
    } else {
        hienThongBao('thong-bao-menu', `❌ ${kq.loi}`, false);
    }
}

// Khởi chạy khi tải trang
document.addEventListener('DOMContentLoaded', () => {
    taiDanhSachMenu();
});
