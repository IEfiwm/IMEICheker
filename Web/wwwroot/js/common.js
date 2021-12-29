function simpleToast() {
    // Get the SIMPLE-TOAST DIV
    var x = document.getElementById("simpleToast");
    // Add the "show" class to DIV
    x.className = "show";
    // After 3 seconds, remove the show class from DIV
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}

function SendNotification(type, message) {

    var id = guidGenerator();

    $(document.body).append("<div id='" + id + "' class='toast show " + type + "'><span>" + message + "</span></div>");

    // After 3 seconds, remove the show class from DIV
    setTimeout(function () {
        $("#" + id).remove("");
    }, 3000);
}

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

function IsPhoneNumberValid(input) {
    // var regex = new RegExp('^(\\+98|0)?9\\d{9}$');
    var regex = new RegExp('^09\\d{9}$');
    var result = regex.test(input);

    return result;
}

function IsIMEIValid(s) {
    var etal = /^[0-9]{15}$/;
    if (!etal.test(s))
        return false;
    sum = 0; mul = 2; l = 14;
    for (i = 0; i < l; i++) {
        digit = s.substring(l - i - 1, l - i);
        tp = parseInt(digit, 10) * mul;
        if (tp >= 10)
            sum += (tp % 10) + 1;
        else
            sum += tp;
        if (mul == 1)
            mul++;
        else
            mul--;
    }
    chk = ((10 - (sum % 10)) % 10);
    if (chk != parseInt(s.substring(14, 15), 10))
        return false;
    return true;
}

function confirmPhoneNumber() {
    if ($("#phone").val() != "+98") {

        SendNotification("error", "کد کشور معتبر نمی باشد.");

        fs = false;

        return;
    } else if (!IsPhoneNumberValid("0" + $("#phoneNumber").val())) {

        SendNotification("error", "شماره تلفن همراه معتبر نمی باشد.");

        fs = false;

        return;
    }

    fs = true;

    $("#finalPhoneNumber").val("0" + $("#phoneNumber").val())
}

function confirmIMEI() {
    if (!IsIMEIValid($("#imei").val())) {

        SendNotification("error", "IMEI معتبر نمی باشد.");

        fs = false;

        return;
    }

    $.ajax({

        type: "GET",

        url: "/Device/Active/CheckIMEIIsExist?imei=" + $("#imei").val(),

        success: function (response) {
            if (!response) {
                fs = false;

                SendNotification("error", "IMEI موجود نمی باشد.");

                return;
            } else {
                fs = true;

                $("#finalImei").val($("#imei").val());
                $("#IMEIButton").click();
            }
        },
    });
}

function sendDocuments() {
    var fdata = new FormData();

    fdata.append($("#nationalCard_temp").get(0).files[0].name, $("#nationalCard_temp").get(0).files[0]);

    fdata.append($("#orderPic_temp").get(0).files[0].name, $("#orderPic_temp").get(0).files[0]);

    fdata.set('model.PhoneNumber', $("#finalPhoneNumber").val());

    fdata.set('model.DeviceIMEI', $("#finalImei").val());

    $.ajax({

        type: "POST",

        url: "/Device/Active/OnConfirm",

        beforeSend: function (xhr) {

            xhr.setRequestHeader("XSRF-TOKEN",

                $('input:hidden[name="__RequestVerificationToken"]').val());

        },

        data: fdata,

        contentType: false,

        processData: false,

        success: function (response) {
            if (response) {
                $("#overlay").show()
            } else {
                SendNotification("error", "خطا در بارگزاری اطلاعات.");
            }
        },
    });
}

$("#overlay").hide()