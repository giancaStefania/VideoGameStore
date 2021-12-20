$(function () {
    if ($("#showOrderDeleted").length) {
        alertify.success("Order deleted with success");
    }
    if ($("#showStateOfOrder").length) {
        alertify.error("Impossible to delete the order because order's state result deleted");
    }
    if ($("#showUserOfOrder").length) {
        alertify.error("Impossibile to delete the order because isn't your order");
    }
    if ($("#showOrderConfirmed").length)
        alertify.success("Order confirmed with success");

    $(".badge").html(countElementOfShoppingCart());

    $("#navbarListCartElement").bind("DOMNodeInserted DOMNodeRemoved", function () {
        $(".badge").html(countElementOfShoppingCart());
    })
    $("#scrollButton").click(function () {
        $("body, html").animate({ scrollTop: 0 },500);
    })

    $(window).scroll(function () {
        if ($("body").scrollTop() > 20 || $("html").scrollTop() > 20) {
            $("#scrollButton").css("display", "block");
        } else {
            $("#scrollButton").css("display", "none");
        }
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