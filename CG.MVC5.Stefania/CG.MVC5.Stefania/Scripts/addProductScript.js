$(function () {
    $(".fa-close").addClass("cursorPointer");

    $("#buttonImage1").click(function () {
        $("#inputFileImage1").click();

    })
    $("#buttonImage2").click(function () {
        $("#inputFileImage2").click();

    })
    $("#buttonImage3").click(function () {
        $("#inputFileImage3").click();

    })
    $("#inputFileImage1").change(function (e) {
        fileCheck(e, e.originalEvent.srcElement.files[0]);

    })
    $("#inputFileImage2").change(function (e) {
        fileCheck(e, e.originalEvent.srcElement.files[0]);

    })
    $("#inputFileImage3").change(function (e) {
        fileCheck(e, e.originalEvent.srcElement.files[0]);

    })

    checkDeletePlatform();
    
    $("#platformList").bind('DOMNodeInserted DOMSubtreeModified DOMNodeRemoved',function() {
        checkDeletePlatform();
    })

    function imagePreview(e, file) {
        var img = document.createElement("img");

        var reader = new FileReader();
        reader.onloadend = function () {
            img.src = reader.result;
        }
        reader.readAsDataURL(file);
        $(e.target).parent().children("#spanFile").html(img);
    }
    function fileCheck(e, file) {
        var maxMbSize = 5;
        if (e.target.files.length == 0) {
            clickFaClose(e.target);
        } else {
            if (checkFileImage(e.target.files[0].type)) {
                if (((e.target.files[0].size) / Math.pow(1024, 2)) <= maxMbSize) {
                    imagePreview(e, file);
                    checkOpacityOfInputFileRemove(e);
                } else {
                    alertify.error("The size of file " + e.target.files[0].name + " is over than 5 MB");
                    $(e.target).val('');

                }
            } else {
                $(e.target).parent().children("#spanFile").html("<img src='/Content/Images/image_placeholder.png' />");
            }
        }
        
    }
    $("#formProduct").submit(function (event) {
        var flag = true;
        if (!checkTitle())
            flag = false;
        if (!checkPrice())
            flag = false;
        if (!checkImages())
            flag = false;
        if (!checkQuantity())
            flag = false;
        if (!checkDescription())
            flag = false;
        if (!checkCategory())
            flag = false;
        if (!checkStock())
            flag = false;
        return true && flag;
    })
    $("#formAddPlatform").submit(function (event) {
        if (!checkPlatform()) {
            addError("#namePlatform", "#validationPlatform", "Field must not contain html tag");
            return false;
        }
        return true;
    })
    function checkPlatform() {
        return checkHtmlElement($("#namePlatform").val());
    }
    function checkTitle() {
        if (!$("#inputTitle").val()) {
            return addError("#inputTitle", "#validationTitle", "The title must not be empty");
        }
        if (!checkHtmlElement($("#inputTitle").val()))
            return addError("#inputTitle", "#validationTitle", "The Title must not contain html tag");
        removeError("#inputTitle", "#validationTitle");
        return true;
    }

    function checkImages() {
        if (!$("#inputFileImage1").get(0).files.length > 0) {
            $(".productForm").css("border", "1px solid red");
            $("#validationImage").text("The product must contain at least one image");
            $("#validationImage").removeClass("hiddenLabel");
            return false;
        }
        $(".productForm").css("border", "1px solid #6495ED");
        $("#validationImage").addClass("hiddenLabel");
        return true;
    }
    function checkPrice() {
        if (!$("#price").val()) {
            return addError("#price", "#validationPrice", "The price must not be empty");
        }
        if ($("#price").val() <= 0) {
            return addError("#price", "#validationPrice", "The price must be > 0");
        }
        if (!$.isNumeric($("#price").val())) {
            return addError("#price", "#validationPrice", "The price must be a number");
        }
        if (!checkHtmlElement($("#price").val()))
            return addError("#price", "#validationPrice", "The price must not contain html tag");
        removeError("#price", "#validationPrice");
        return true;
    }
    function checkQuantity() {
        if (!$("#quantity").val())
            return addError("#quantity", "#validationQuantity", "The quantity must not be empty");
        if ($("#quantity").val() <= 0)
            return addError("#quantity", "#validationQuantity", "The quantity must be > 0");
        if (!$.isNumeric($("#quantity").val()))
            return addError("#quantity", "#validationQuantity", "The quantity must be a integer number");
        if (!checkHtmlElement($("#quantity").val()))
            return addError("#quantity", "#validationQuantity", "The quantity must not contain html tag");
        removeError("#quantity", "#validationQuantity");
        return true;
    }

    function checkCategory() {
        if (!$("#category").val())
            return addError("#category", "#validationCategory", "The category must not be empty");
        removeError("#category", "#validationCategory");
        return true;
    }
    function checkStock() {
        if (!$("#stock").val())
            return addError("#stock", "#validationStock", "The stock must not be empty");
        removeError("#stock", "#validationStock");
        return true;
    }
    function checkDescription() {
        if (!$("#textOfProduct").val())
            return addError("#textOfProduct", "#validationDescription", "The description must not be empty");
        if (!checkHtmlElement($("#textOfProduct").val()))
            return addError("#textOfProduct", "#validationDescription", "The description must not contain html tag");
        removeError("#textOfProduct", "#validationDescription");
        return true;
    }

    function addError(id, idError, text) {
        $(id).addClass("errorBorder");
        $(idError).text(text);
        $(idError).removeClass("hiddenLabel");
        return false;
    }
    function removeError(id, idError) {
        $(id).removeClass("errorBorder");
        $(idError).addClass("hiddenLabel");
    }
    function checkHtmlElement(text) {
        var regex = /<\/?[a-z][\s\S]*>/;
        if (!regex.test(text))
            return true;
        return false;
    }
    $("#price").focusout(function () {
        var value = $(this).val();
        value = parseFloat(value).toFixed(2);
        $(this).val(value);
    })
    function checkFileImage(file) {
        if (file == "image/jpg" || file == "image/png" || file == "image/jpeg")
            return true;
        alertify.error("The file must be an image");
        $(this).val('');
        return false;
    }

    if ($("#inputFileImage1").get(0).files.length == 0) {
        $("#img2").addClass("opacityFile");
        $("#img3").addClass("opacityFile");
    }
});
function clickFaClose(e) {
    $(e).parent().children("#spanFile").html("<img src='/Content/Images/image_placeholder.png' />");
    $(e).parent().children("input").val('');
    if ($(e).parent().attr("id") == "img1") {
        $("#img2").children("#spanFile").html("<img src='/Content/Images/image_placeholder.png' />");
        $("#img2").addClass("opacityFile");
        $("#inputFileImage2").val('');
        $("#img3").children("#spanFile").html("<img src='/Content/Images/image_placeholder.png' />");
        $("#img3").addClass("opacityFile");
        $("#inputFileImage3").val('');
    } else {
        if ($(e).parent().attr("id") == "img2") {
            $("#img3").children("#spanFile").html("<img src='/Content/Images/image_placeholder.png' />");
            $("#img3").addClass("opacityFile");
            $("#inputFileImage3").val('');
        }
    }
}
function checkOpacityOfInputFileRemove(e) {
    if ($(e.target).parent().attr("id") == "img1") {
        $("#img2").removeClass("opacityFile");
    } else {
        if ($(e.target).parent().attr("id") == "img2") {
            $("#img3").removeClass("opacityFile");
        }
    }
}
function onCompleteAddPlatform() {
    $("#buttonCloseModal").click();
    alertify.success("Platform added with success");
}
function onCompleteDeletePlatform() {
    $("#buttonCloseModalDelete").click();
    alertify.success("Platform removed with success");
}
function checkDeletePlatform() {
    if ($("#platformList option").length == 1) {
        $("#deletePlatform").addClass("hiddenLabel");
    } else {
        $("#deletePlatform").removeClass("hiddenLabel");
    }
}