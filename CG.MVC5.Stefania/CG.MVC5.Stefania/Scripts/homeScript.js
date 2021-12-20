$(function () {
    var maxNumPage = 3;
    if ($("#showAddProduct").length)
        alertify.success("Product added with success");
    if ($("#showEditProduct").length)
        alertify.success("Product updated with success");
    if ($("#showDeleteProduct").length)
        alertify.success("Product deleted with success");
    if ($("#showCheckoutGood").length)
        alertify.success("Order's checkout completed with success");

    if ($("#showCheckoutBad").length)
        alertify.error("Order's checkout impossibile to complete, because product's selected not available");
    if ($("#showLogin").length)
        alertify.success("Login successful");
    if ($("#showLogout").length)
        alertify.success("Logout successful");

    $("#invalidateProduct").addClass("cursorPointer");
    $("#deleteProduct").addClass("cursorPointer");

    $(".badge").html(countElementOfShoppingCart());

    if (totalNumPage > maxNumPage) {
        if (currentPage <= maxNumPage - 1) {
            $("li[data-page]").each(function () {
                if ($(this).attr("data-page") > maxNumPage && $(this).attr("data-page") != totalNumPage) {
                    $(this).hide();
                }
                if ($(this).attr("data-page") == maxNumPage)
                    $(this).after("<li class='page-item disabled'><a class='page-link' href='javascript:void(0)' >...</a ></li>");
            })
        } else {
            if (currentPage >= (totalNumPage - (maxNumPage - 1))) {
                $("li[data-page]").each(function () {
                    if ($(this).attr("data-page") == 1) {
                        $(this).after("<li class='page-item disabled'><a class='page-link' href='javascript:void(0)' >...</a ></li>");
                    } else {
                        if ($(this).attr("data-page") < totalNumPage - (maxNumPage-1))
                            $(this).hide();
                    }
                })

            } else {
                var halfOfLimit = Math.floor(maxNumPage / 2);
                $("li[data-page]").each(function () {
                    if ($(this).attr("data-page") == 1) {
                        $(this).after("<li class='page-item disabled'><a class='page-link' href='javascript:void(0)' >...</a ></li>");
                    } else {
                        if (halfOfLimit % 2 == 1) {
                            if (($(this).attr("data-page") < currentPage - halfOfLimit || $(this).attr("data-page") > currentPage + halfOfLimit) && $(this).attr("data-page") != totalNumPage)
                                $(this).hide();
                        } else {
                            if (($(this).attr("data-page") < currentPage - (halfOfLimit - 1) || $(this).attr("data-page") > currentPage + halfOfLimit) && $(this).attr("data-page") != totalNumPage)
                                $(this).hide();
                        }
                        if ($(this).attr("data-page") == currentPage + halfOfLimit)
                            $(this).after("<li class='page-item disabled'><a class='page-link' href='javascript:void(0)' >...</a ></li>");
                    }
                })

            }
        }  
    }

    $(".card").hover(function () {
        $(this).addClass("selectedCard");
    }, function () {
        $(this).removeClass("selectedCard");
    })

    $("#navbarListCartElement").bind("DOMNodeInserted DOMNodeRemoved", function () {
        $(".badge").html(countElementOfShoppingCart());
    })

    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass('show');
        }
        var $subMenu = $(this).next('.dropdown-menu');
        $subMenu.toggleClass('show');


        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass('show');
        });


        return false;
    });
});
function selectPage(currentPage, element) {
    if ($(element).attr("data-page") == currentPage)
        $(element).addClass("active");
}
function clickDeleteProduct(e, id) {
    $(e).removeAttr('href');
    alertify.confirm('Delete Product', 'Are you sure you want to delete the product?',
        function () {
            window.location.href = "/Home/Delete/" + id;
        }
        , function () {
            alertify.error('Cancel');
        });
}

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
