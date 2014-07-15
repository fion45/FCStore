var LoginPageView = {
	login : {
		OnLoginBtnClick : function() {
			var obj = {};
	        obj.userID = $("#UIDTB").val();
	        obj.PSW = $("#PSWTB").val();
	        obj.checkCode = '';
	        if($("#checkCodeTB").length != 0) {
	        	obj.checkCode = $("#checkCodeTB").val();
	        	if (obj.checkCode.length != 4) {
	        		alert("验证码错误！");
	                //重新输入验证码
		            $("#checkCodeTB").val("");
		            return;
	        	}
	        }
            $.myAjax({
            	refreshTag : true,
                loadEle: $("#loginDiv"),
                url: "/User/Login",
                data: JSON.stringify(obj),
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=utf-8",
                success: function (data, status, options) {
                    if (!data.successTag) {
                    	if(data.errCode == -2) {
                    		alert("验证码错误，请重新输入");
                    	}
                    	else {
                    		alert("密码或者用户名错误，请重新输入！");
                    		$("#UIDTB").val("");
                    		$("#PSWTB").val("");
                    	}
                    	if(data.custom == -1)
			        		LoginPageView.register.OnRefreshCCode();	//刷新验证码
			        	else if(data.custom == -2)
			        		window.location.reload();
                    }
                    else {
                    	$("#loginDiv").remove();
                    	$("#resultDiv").show();
                    	$("#resultDiv .userName").text($.cookie('UserInfo').UserName);
                    	LoginPageView.login.OnFiveSecondsTicket();
                    }
                }
            });
		},
		OnCheckCodeImgClick : function() {
    		$("#ccImg").prop("src","/Home/GetValidateCode");
		},
		OnFiveSecondsTicket: function () {
		    var tmpI = parseInt($("#resultDiv .ticket").text());
		    if (tmpI == 0) {
		        window.location = $("#resultDiv .returnUrl").text();
		    }
		    else {
		        setTimeout(LoginPageView.login.OnFiveSecondsTicket, 1000);
		        $("#resultDiv .ticket").text(tmpI - 1);
		    }
		}
	},
	register : {
		OnRegisterBtnClick : function() {
			if($("#PSWTB").val() != $("#EPSWTB").val()) {
				alert("密码不一致，请重新输入");
				$("#PSWTB").val("");
				$("#EPSWTB").val("");
			}
			var obj = {};
	        obj.userName = $("#UserNameTB").val();
	        obj.email = $("#EmailTB").val();
	        obj.psw = $("#PSWTB").val();
	        obj.checkCode = $("#checkCodeTB").val();
	        var hArr = window.location.href.split("/");
	        var tmpUrlStr = hArr[hArr.length - 1];
	        tmpUrlStr = tmpUrlStr == "Register" ? "" : tmpUrlStr;
			$.myAjax({
                loadEle: $("#regCDiv"),
                url: "/User/Register",
                data: JSON.stringify(obj),
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=utf-8",
                success: function (data, status, options) {
                    if (!data.successTag) {
                    	if(data.errCode == -1) {
                    		alert("验证码错误");
                    	}
                        else if(data.errCode == -2) {
                        	alert("用户名已被注册，请重新输入");
                        	$("#UserNameTB").val("");
                        }
                        else if(data.errCode == -3) {
                        	alert("邮箱已被注册，请重新输入");
                        	$("#EmailTB").val("");
                        }
                        else if(data.errCode == -4) {
                        	alert("输入信息未完整");
                        }
                		LoginPageView.register.OnRefreshCCode();
                    }
                    else {
                        //注册成功
                        window.location = "/Home/Login/" + tmpUrlStr;
                    }
                }
            });
		},
		OnResetBtnClick : function() {
			$("#UserNameTB").val("");
			$("#EmailTB").val("");
			$("#PSWTB").val("");
			$("#EPSWTB").val("");
			$("#checkCodeTB").val("");
    		$("#ccImg").prop("src","/Home/GetValidateCode");
		},
		OnRefreshCCode : function() {
			if($("#ccImg").length == 0) {
				var tmpIMG = $(
					"<input id='checkCodeTB' name='checkCodeTB' class='enterTB' type='text' data-val='true' data-val-required='必填字段' data-val-length='' data-val-length-min='4' data-val-length-max='4' />" +
                    "<div class='inBox'>验证码</div>" +
					"<img class='inBoxIMG' onclick='LoginPageView.login.OnCheckCodeImgClick()' id='ccImg' src='/Home/GetValidateCode' />");
				tmpIMG.appendTo($("#loginer .ccDiv"));
				$.validator.unobtrusive.parse(document);
			}
			$("#checkCodeTB").val("");
    		$("#ccImg").prop("src","/Home/GetValidateCode");
		}
	}
}

