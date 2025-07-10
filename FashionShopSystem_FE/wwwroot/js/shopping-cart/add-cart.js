$(document).ready(function () {
    $(".add-cart").on("click", function (e) {
        e.preventDefault();

        const $btn = $(this);

        const productId = parseInt($btn.data("product-id"));
        const productName = $btn.data("product-name");
        const unitPrice = parseFloat($btn.data("unit-price"));
        const productImage = $btn.data("product-image");

        let quantity = 1;
        const $qtyInput = $btn.closest(".product__details__text, .product-item, .pro-qty, .quantity").find(".pro-qty input");
        if ($qtyInput.length) {
            quantity = parseInt($qtyInput.val()) || 1;
        }

        let cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };

        let existingItem = cart.items.find(item => item.productId === productId);
        if (existingItem) {
            existingItem.quantity += quantity;
            existingItem.totalPrice = existingItem.quantity * existingItem.unitPrice;
        } else {
            cart.items.push({
                productId: productId,
                productName: productName,
                productImage: productImage,
                quantity: quantity,
                unitPrice: unitPrice,
                totalPrice: quantity * unitPrice
            });
        }

        localStorage.setItem("cart", JSON.stringify(cart));

        showToast(`Đã thêm ${quantity} sản phẩm "${productName}" vào giỏ hàng.`, "success", 1500);
        updateCartBadge();
    });

    updateCartBadge();
});
