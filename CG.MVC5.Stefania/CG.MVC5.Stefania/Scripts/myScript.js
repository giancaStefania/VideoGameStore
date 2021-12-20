$(function () {

    $('[data-toggle="popover"]').popover();

    $("#inputPassword").focusout(function () {
        if (!checkPassword()) {
            $("#checkPassword").removeClass("hiddenLabel");
            $("#inputPassword").addClass("errorBorder");
        }
        else {
            $("#checkPassword").addClass("hiddenLabel");
            $("#inputPassword").removeClass("errorBorder");
        }
    })
    $("#inputEmail").focusout(function () {
        if (!checkEmail()) {
            $("#checkEmail").removeClass("hiddenLabel");
            $("#inputEmail").addClass("errorBorder");
        }
        else {
            $("#checkEmail").addClass("hiddenLabel");
            $("#inputEmail").removeClass("errorBorder");
        }

    })
    $("#inputRepeatPassword").focusout(function () {
        if (checkConfirmPassword()) {
            $("#checkConfirmPassword").addClass("hiddenLabel");
            $("#inputRepeatPassword").removeClass("errorBorder");
        } else {
            $("#checkConfirmPassword").removeClass("hiddenLabel");
            $("#inputRepeatPassword").addClass("errorBorder");
        }
    })
    $(".myForm > #formLogin").submit(function (event) {
        return myCheckForm();
    })
    $(".myForm > #customForm").submit(function (event) {
        if (checkEmail()) {
            return true;
        }
        $("#checkEmail").removeClass("hiddenLabel");
        $("#inputEmail").addClass("errorBorder");
        return false;
    })
    $(".myForm > #formResetPassword").submit(function (event) {
        if (checkPassword() && checkConfirmPassword())
            return true;
        if (!checkPassword()) {
            $("#checkPassword").removeClass("hiddenLabel");
            $("#inputPassword").addClass("errorBorder");
        }
        if (!checkConfirmPassword()) {
            $("#checkConfirmPassword").removeClass("hiddenLabel");
            $("#inputRepeatPassword").addClass("errorBorder");
        }
        return false;
    })
    function myCheckForm() {
        if (checkPassword() && checkEmail())
            return true;
        else {
            if (!checkPassword()) {
                $("#checkPassword").removeClass("hiddenLabel");
                $("#inputPassword").addClass("errorBorder");
            }
            if (!checkEmail()) {
                $("#checkEmail").removeClass("hiddenLabel");
                $("#inputEmail").addClass("errorBorder");
            }
        }
            return false;
    }
    function checkPassword() {
        var reg = new RegExp("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        var valorePass = $("#inputPassword").val();
        if (reg.test(valorePass))
            return true;
        else
            return false;
    }
    function checkEmail() {
        var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
        if (filter.test($("#inputEmail").val()))
            return true;
        else
            return false;
    }
    function checkConfirmPassword() {
        if ($("#inputPassword").val() == $("#inputRepeatPassword").val())
            return true;
        return false;
    }
    if ($("#showSuccess").length)
        alertify.success("Email sended with success, please check your mailbox");
    if ($("#showFailure").length)
        alertify.error("Your account already received this email. Please check your mailbox");
    if ($("#showExpired").length)
        alertify.error("Link has expired. If you want to change the password, you need to click Forgot Password in this page.");
    if ($("#showChangePassword").length)
        alertify.success("Password has been changed with success");
});