$(function () {
    $("#loginBtn").click(function (ev) {
        var obj = {};
        obj.userID = $("#UIDTB").val();
        obj.PSW = $("#PSWTB").val();
        obj.checkCode = $("#checkCodeTB").val();
        if (obj.checkCode.length == 4) {
            $.myAjax({
                loadEle: $("#loginDiv"),
                url: "/User/Login",
                data: JSON.stringify(obj),
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=utf-8",
                success: function (data, status, options) {
                    if (data == null) {
                        alert("密码或者用户名错误，请重新输入！")
                    }
                    else {
                        alert("登陆成功！");
                    }
                }
            });
        }
        else {
            alert("验证码错误！");
        }
    });
});