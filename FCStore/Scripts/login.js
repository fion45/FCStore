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
                        alert("密码或者用户名错误，请重新输入！");
                        //刷新验证码
                        $("#ccImg").prop("src","/Home/GetValidateCode");
                    }
                    else {
                    	$("#loginDiv").remove();
                    	$("#resultDiv").show();
                    	$("#resultDiv .userName").text($.cookie('UserInfo').UserName);
                    }
                }
            });
        }
        else {
            alert("验证码错误！");
        }
    });
    $("#ccImg").click(function(ev){
    	$(this).prop("src","/Home/GetValidateCode");
    });
});