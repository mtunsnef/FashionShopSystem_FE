$(document).ready(function () {
    $(".add-cart").on("click", function (e) {
        e.preventDefault();

        const productId = parseInt($(this).data("product-id"));
        const productName = $(this).data("product-name");
        const unitPrice = parseFloat($(this).data("unit-price"));
        const productImage = $(this).data("product-image");

        let cart = JSON.parse(localStorage.getItem("cart")) || { items: [] };

        let existingItem = cart.items.find(item => item.productId === productId);
        if (existingItem) {
            existingItem.quantity += 1;
            existingItem.totalPrice = existingItem.quantity * existingItem.unitPrice;
        } else {
            cart.items.push({
                productId: productId,
                productName: productName,
                productImage: productImage,
                quantity: 1,
                unitPrice: unitPrice,
                totalPrice: unitPrice
            });
        }

        localStorage.setItem("cart", JSON.stringify(cart));

        showToast(`Đã thêm "${productName}" vào giỏ hàng.`, "success", 1500);
        updateCartBadge();
    });

    updateCartBadge();
});