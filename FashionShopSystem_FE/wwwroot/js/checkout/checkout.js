document.addEventListener("DOMContentLoaded", function () {
    initCheckoutPage();
});

function initCheckoutPage() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/dang-nhap';
        return;
    }

    renderCheckoutCart();
    setupRealtimeValidation();

    $("#checkoutForm").on("submit", function (e) {
    e.preventDefault();

    const cart = JSON.parse(localStorage.getItem("cart")) || {items: [] };
    if (cart.items.length === 0) {
        alert("Giỏ hàng trống!");
        return;
    }

    if (!validateCheckoutForm()) return;

    const orderData = {
        fullName: $("#fullName").val().trim(),
        phone: $("#phone").val().trim(),
        email: $("#email").val().trim(),
        address: $("#address").val().trim(),
        note: $("#note").val(),
        cartItems: cart.items
    };

    $.ajax({
        url: "/api/orders",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(orderData),
        success: function () {
            localStorage.removeItem("cart");
            window.location.href = "/thanh-toan-thanh-cong";
            },
        error: function (xhr, status, error) {
            console.error("❌ Lỗi đặt hàng:", xhr.responseText || error);
            alert("Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.");
            }
        });
    });
}

function renderCheckoutCart() {
    const cart = JSON.parse(localStorage.getItem("cart")) || {items: [] };
const $list = $("#checkoutProductList");
const $total = $("#checkoutTotal");

$list.empty();
let total = 0;

    cart.items.forEach(item => {
        const itemTotal = item.quantity * item.unitPrice;
total += itemTotal;
$list.append(`<li>${item.productName} x${item.quantity} <span>${formatCurrency(itemTotal)}</span></li>`);
    });

$total.text(formatCurrency(total));
}

function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', {style: 'currency', currency: 'VND' });
}

function setupRealtimeValidation() {
    const rules = {
    fullName: { validate: v => v !== "",
    message: "Vui lòng nhập họ và tên."
            },
    phone: {
        validate: v => /^0\d{9,12}$/.test(v),
        message: "Số điện thoại phải từ 10 đến 13 chữ số và bắt đầu bằng 0."
    },
    email: {
        validate: v => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v),
    message: "Email không hợp lệ."
            },
    address: {
        validate: v => v !== "",
    message: "Vui lòng nhập địa chỉ nhận hàng."
            }
    };

    Object.keys(rules).forEach(field => {
    $(`#${field}`).on("input", function () {
        const value = $(this).val().trim();
        const { validate, message } = rules[field];
        $(`#${field}Error`).text(validate(value) ? "" : message);
    });
    });
}

function validateCheckoutForm() {
    let isValid = true;

$("#fullName, #phone, #email, #address").each(function () {
    $(this).trigger("input"); 
const errorId = `#${this.id}Error`;
if ($(errorId).text() !== "") {
    isValid = false;
        }
    });

return isValid;
}
