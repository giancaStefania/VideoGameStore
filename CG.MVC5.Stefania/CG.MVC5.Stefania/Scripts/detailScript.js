$(function () {
    $(".badge").html(countElementOfShoppingCart());

    $("#navbarListCartElement").bind("DOMNodeInserted DOMNodeRemoved", function () {
        $(".badge").html(countElementOfShoppingCart());
    })
})
function countElementOfShoppingCart() {
    result = 0;
    $("#navbarListCartElement div[data-product]").each(function () {
        if ($(this).text() != "No Items Selected")
            result++;
    })
    return result;
}
function onCompleteAddCart() {
    alertify.success("Product added to cart");
}
function onCompleteDeleteCart() {
    alertify.error("Product deleted to cart");
}
