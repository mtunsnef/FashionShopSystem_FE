const touchedFields = {
    fullName: false,
    phone: false,
    email: false,
    address: false
};


document.addEventListener("DOMContentLoaded", function () {
    initCheckoutPage();
});

function openModal() {
    document.getElementById("confirmOrderModal").style.display = "flex";
}

function closeModal() {
    document.getElementById("confirmOrderModal").style.display = "none";
}

function initCheckoutPage() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/dang-nhap';
        return;
    }

    renderCheckoutCart();
    setupRealtimeValidation();

    $("#fullName, #phone, #email, #address").on("input", toggleOrderButton);

    $("#checkoutForm").on("submit", function (e) {
        e.preventDefault();

        if (!validateCheckoutForm()) return;

        openModal();

    });

    $("#confirmOrderBtn").on("click", function () {
        placeOrder();
        closeModal();
    });
        
    toggleOrderButton();
    fillCheckoutForm();
}

function toggleOrderButton() {
    const isValid = validateCheckoutForm(false);
    $(".site-btn[type='submit']").prop("disabled", !isValid);
}

function renderCheckoutCart() {
    const cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };
    const $list = $("#checkoutProductList");
    const $total = $("#checkoutTotal");

    $list.empty();
    let total = 0;

    cart.items.forEach(item => {
        const itemTotal = item.quantity * item.unitPrice;
        total += itemTotal;

        $list.append(`
            <li>${item.productName} x${item.quantity}
                <span>${formatCurrency(itemTotal)}</span>
            </li>
        `);
    });

    $total.text(formatCurrency(total));
}

function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}

function setupRealtimeValidation() {
    const rules = getValidationRules();

    Object.keys(rules).forEach(field => {
        const $field = $(`#${field}`);

        $field.on("input", function () {
            touchedFields[field] = true;
            const value = $field.val().trim();
            const { validate, message } = rules[field];

            $(`#${field}Error`).text(validate(value) ? "" : message);
            toggleOrderButton();
        });

        $field.on("blur", function () {
            touchedFields[field] = true;
            $field.trigger("input");
        });
    });
}

function validateCheckoutForm(showErrors = true) {
    let isValid = true;
    const rules = getValidationRules();

    Object.keys(rules).forEach(field => {
        const value = $(`#${field}`).val().trim();
        const { validate, message } = rules[field];

        const isFieldValid = validate(value);

        if (!isFieldValid) {
            isValid = false;
            if (showErrors && touchedFields[field]) {
                $(`#${field}Error`).text(message);
            }
        } else {
            $(`#${field}Error`).text("");
        }
    });

    return isValid;
}

function getValidationRules() {
    return {
        fullName: {
            validate: v => v !== "",
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
}

function showRedirectPreloader(message = "Đang chuyển hướng đến trang thanh toán...") {
    $(".loader").show();
    $("#redirect-message").text(message).show();
    $("#preloder").css({
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        flexDirection: "column",
        backgroundColor: "rgba(0, 0, 0, 0.7)"
    });
    $("#preloder").fadeIn(350);
}

function placeOrder() {
    const cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };

    if (cart.items.length === 0) {
        showToast("Giỏ hàng trống!", "error");
        return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
        showToast("Vui lòng đăng nhập để đặt hàng.", "error");
        window.location.href = "/dang-nhap";
        return;
    }

    const orderData = {
        fullName: $("#fullName").val().trim(),
        phone: $("#phone").val().trim(),
        email: $("#email").val().trim(),
        shippingAddress: $("#address").val().trim(),
        note: $("#note").val(),
        cartItems: cart.items
    };

    $.ajax({
        url: "https://localhost:7242/api/Order/place-order",
        type: "POST",
        contentType: "application/json",
        headers: {
            "Authorization": `Bearer ${token}`
        },
        data: JSON.stringify(orderData),
        success: function (res) {
            if (res.Success) {
                const orderId = res.Data.OrderId;
                const total = res.Data.TotalAmount;

                showRedirectPreloader();

                $.ajax({
                    url: "https://localhost:7242/api/Checkout/create-payment-link",
                    type: "POST",
                    headers: {
                        "Authorization": `Bearer ${token}`
                    },
                    data: {
                        orderId: orderId,
                        AmountPayOs: total
                    },
                    success: function (payRes) {
                        console.log(payRes);
                        if (payRes.url) {
                            localStorage.removeItem("cart");
                            setTimeout(() => {
                                window.location.href = payRes.url;
                            }, 800);
                        } else {
                            showToast("Không lấy được liên kết thanh toán.", "error");
                            location.reload();
                        }
                    },
                    error: function () {
                        showToast("Không thể kết nối tới cổng thanh toán.", "error");
                        location.reload();
                    }
                });

            } else {
                showToast("Tạo đơn hàng thất bại.", "error");
            }
        },
        error: function (xhr, status, error) {
            console.error("Lỗi đặt hàng:", xhr.responseText || error);
            showToast("Đặt hàng thất bại. Vui lòng thử lại.");
        }
    });

    closeModal();
}


function fillCheckoutForm() {
    const userInfoRaw = localStorage.getItem("userInfo");

    if (!userInfoRaw) return;

    const userInfo = JSON.parse(userInfoRaw);

    $('#fullName').val(userInfo.fullName ?? "");
    $('#phone').val(userInfo.phone ?? "");
    $('#email').val(userInfo.email ?? "").prop('disabled', true);
    $('#address').val(userInfo.address ?? "");

}