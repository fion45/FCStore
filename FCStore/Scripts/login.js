var LoginPageView = {
	login : {
		OnLoginBtnClick : function() {
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
	                    if (!data.successTag) {
	                    	if(data.errCode == -2) {
	                    		alert("验证码错误，请重新输入");
				                //刷新验证码
				        		LoginPageView.register.OnRefreshCCode();
	                    	}
	                    	else {
	                    		alert("密码或者用户名错误，请重新输入！");
	                    		$("#UIDTB").val("");
	                    		$("#PSWTB").val("");
				                //刷新验证码
				        		LoginPageView.register.OnRefreshCCode();
	                    	}
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
                //重新输入验证码
	            $("#checkCodeTB").val("")
	        }
		},
		OnCheckCodeImgClick : function() {
    		$("#ccImg").prop("src","/Home/GetValidateCode");
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
	        obj.checkCode = $("#CheckCodeTB").val();
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
                        //刷新验证码
                		LoginPageView.register.OnRefreshCCode();
                    }
                    else {
                    	//注册成功
                    }
                }
            });
		},
		OnResetBtnClick : function() {
			$("#UserNameTB").val("");
			$("#EmailTB").val("");
			$("#PSWTB").val("");
			$("#EPSWTB").val("");
			$("#CheckCodeTB").val("");
    		$("#ccImg").prop("src","/Home/GetValidateCode");
		},
		OnRefreshCCode : function() {
			$("#CheckCodeTB").val("");
    		$("#ccImg").prop("src","/Home/GetValidateCode");
		}
	}
}