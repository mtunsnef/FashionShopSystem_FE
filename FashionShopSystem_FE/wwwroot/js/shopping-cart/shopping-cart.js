$(document).ready(function () {
    renderCart();

    $(document).on("click", ".btn-remove-cart", function () {
        const productId = parseInt($(this).data("product-id"));
        removeCartItem(productId);
    });

    $(document).on("click", ".btn-increase", function () {
        const productId = parseInt($(this).data("product-id"));
        updateQuantity(productId, +1);
    });

    $(document).on("click", ".btn-decrease", function () {
        const productId = parseInt($(this).data("product-id"));
        updateQuantity(productId, -1);
    });

});

function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}

function renderCart() {
    const cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };
    const $tbody = $("#cart-items-body");
    $tbody.empty();

    let subtotal = 0;

    const $btnCheckout = $("#btn-checkout");

    if (cart.items.length === 0) {
        $tbody.append(`
                <tr class="cart-empty-row">
                    <td colspan="4" class="text-center text-muted py-4">
                        <i class="fa fa-shopping-cart fa-2x mb-2"></i><br>
                        Giỏ hàng của bạn hiện đang trống.
                    </td>
                </tr>
            `);

        $(".cart__total ul").html(`
                <li>Tạm tính <span>0 ₫</span></li>
                <li>Thành tiền <span>0 ₫</span></li>
            `);

        $btnCheckout.addClass("disabled").attr("aria-disabled", "true").css("pointer-events", "none");
        return;
    }

    $.each(cart.items, function (i, item) {
        subtotal += item.totalPrice;

        const row = `
                <tr>
                    <td class="product__cart__item">
                        <div class="product__cart__item__pic">
                            <img src="${item.productImage}" alt="" style="width: 70px;">
                        </div>
                        <div class="product__cart__item__text">
                            <h6>${item.productName}</h6>
                            <h5>${formatCurrency(item.unitPrice)}</h5>
                        </div>
                    </td>
                    <td class="quantity__item">
                        <div class="quantity">
                            <div class="pro-qty-2 d-flex align-items-center gap-2">
                                <i class="fa fa-minus btn-decrease" data-product-id="${item.productId}" style="cursor:pointer;"></i>
                                <p class="qty-text m-0" data-product-id="${item.productId}" style="width: 50px; text-align:center;">${item.quantity}</p>
                                <i class="fa fa-plus btn-increase" data-product-id="${item.productId}" style="cursor:pointer;"></i>
                            </div>
                        </div>
                    </td>
                    <td class="cart__price" data-product-id="${item.productId}">
                        ${formatCurrency(item.totalPrice)}
                    </td>
                    <td class="cart__close">
                        <i class="fa fa-close btn-remove-cart" data-product-id="${item.productId}" style="cursor:pointer;"></i>
                    </td>
                </tr>
            `;

        $tbody.append(row);
    });

    $(".cart__total ul").html(`
            <li>Tạm tính <span>${formatCurrency(subtotal)}</span></li>
            <li>Thành tiền <span>${formatCurrency(subtotal)}</span></li>
        `);

    $btnCheckout.removeClass("disabled").removeAttr("aria-disabled").css("pointer-events", "auto");
}


function removeCartItem(productId) {
    let cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };
    cart.items = cart.items.filter(item => item.productId !== productId);
    localStorage.setItem("cart", JSON.stringify(cart));
    renderCart();
    updateCartBadge();
    showToast("Đã xóa sản phẩm khỏi giỏ hàng", "success", 1500);
}

function updateQuantity(productId, delta) {
    let cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };
    const item = cart.items.find(x => x.productId === productId);
    if (!item) return;

    item.quantity = Math.max(1, item.quantity + delta);
    item.totalPrice = item.quantity * item.unitPrice;
    localStorage.setItem("cart", JSON.stringify(cart));

    $(`p.qty-text[data-product-id="${productId}"]`).text(item.quantity);
    $(`td.cart__price[data-product-id="${productId}"]`).text(formatCurrency(item.totalPrice));

    updateCartTotal();
    updateCartBadge();
}

function updateCartTotal() {
    const cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };
    const subtotal = cart.items.reduce((sum, item) => sum + item.totalPrice, 0);

    $(".cart__total ul").html(`
            <li>Tạm tính <span>${formatCurrency(subtotal)}</span></li>
            <li>Thành tiền <span>${formatCurrency(subtotal)}</span></li>
        `);
}