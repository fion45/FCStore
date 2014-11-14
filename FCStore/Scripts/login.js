var LoginPageView = {
	WBLoginSuccess : function(o) {
		//登录成功
    	//判断该微薄号是否已注册
		if(WB2.checkLogin()) {
			var tmpData = {
				wbId : o.idstr
			};
			$.myAjax({
			    historyTag : false,
        		refreshTag : true,
    			loadEle : $(window.document.body),
	            url: "/User/LoginByWB/",
	            data: JSON.stringify(tmpData),
	            dataType: "json",
	            type: "POST",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	if(data.content == null) {
	            		//未关联，要求输入ID和密码
						var userInfo = {
							openId : "",
							accessToken : "",
							wbId : o.idstr,
							userName : o.screen_name,
							sex : o.gender == "m",
							smallHead : o.profile_image_url,
							lagerHead : o.avatar_large
						};
						LoginPageView.login.CreateAssociateDlg(userInfo);
	            	}
	            	else {
	            		//关联了
				    	$("#loginDiv").remove();
				        $("#resultDiv").show();
				        $("#resultDiv .userName").html($.cookie("UserInfo").UserName);
				        $("#resultDiv .userPhoto").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
                		LoginPageView.login.OnFiveSecondsTicket();
	            	}
	            }
			});
		}
	},
	QQLoginSuccess : function(reqData, opts, par) {
    	//登录成功
    	//判断该QQ号是否已注册
    	if(QC.Login.check()){
	    	QC.Login.getMe(function(openId, accessToken){
	    		var tmpData = {
	    			openID : openId,
	    			accessToken : accessToken
	    		};
	    		//openId和accessToken保存到本地
	    		$.myAjax({
	    		    historyTag : false,
	        		refreshTag : true,
	    			loadEle : $(window.document.body),
		            url: "/User/LoginByQQ/",
		            data: JSON.stringify(tmpData),
		            dataType: "json",
		            type: "POST",
		            contentType: "application/json;charset=utf-8",
		            success: function (data,status,options) {
		            	if(data.content == null) {
		            		//未关联，要求输入ID和密码
							var userInfo = {
								openId : openId,
								accessToken : accessToken,
								wbId : "",
								userName : QC.String.escHTML(reqData.nickname),
								sex : reqData.gender == "男",
								smallHead : reqData.figureurl_qq_1,
								lagerHead : reqData.figureurl_qq_2
							};
							LoginPageView.login.CreateAssociateDlg(userInfo);
		            	}
		            	else {
		            		//关联了
					    	$("#loginDiv").remove();
					        $("#resultDiv").show();
					        $("#resultDiv .userName").html($.cookie("UserInfo").UserName);
					        $("#resultDiv .userPhoto").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
	                		LoginPageView.login.OnFiveSecondsTicket();
		            	}
		            }
				});
	    	});
    	}
	},
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
	            historyTag: false,
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
		},
		onRNUOKBtn : function() {
			var tmpPar = $("#tabs-nu"); 
			var tmpUN = tmpPar.find(".UserNameTB").val();
			var tmpPSW = tmpPar.find(".PSWTB").val();
			var userInfo = $("#associateUserDlg").data("userInfo");
			var tmpData = {
				NTag : true,
				LoginID : tmpUN,
				PSW : tmpPSW,
				UserName : userInfo.userName,
				sex : userInfo.sex,
				smallHead : userInfo.smallHead,
				lagerHead : userInfo.lagerHead,
				openId : userInfo.openId,
				accessToken : userInfo.accessToken,
				wbId : userInfo.wbId
			};
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#associateUserDlg"),
	            url: "/User/RelativeUser",
	            data: JSON.stringify(tmpData),
	            dataType: "json",
	            type: "POST",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	if(data.content != null) {
	            		//注册新用户成功
	            		$("#associateUserDlg").dialog( "close" );
				    	$("#loginDiv").remove();
				        $("#resultDiv").show();
				        $("#resultDiv .userName").html($.cookie("UserInfo").UserName);
				        $("#resultDiv .userPhoto").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
                		LoginPageView.login.OnFiveSecondsTicket();
	            	}
	            	else {
	            		alert("已有同名用户，请重新修改提交。");
	            	}
	            }
			});
		},
		onROUOKBtn : function() {
			var tmpPar = $("#tabs-ou");
			var tmpUN = tmpPar.find(".UserNameTB").val();
			var tmpPSW = tmpPar.find(".PSWTB").val();
			var userInfo = $("#associateUserDlg").data("userInfo");
			var tmpData = {
				NTag : false,
				LoginID : tmpUN,
				PSW : tmpPSW,
				UserName : userInfo.userName,
				sex : userInfo.sex,
				smallHead : userInfo.smallHead,
				lagerHead : userInfo.lagerHead,
				openId : userInfo.openId,
				accessToken : userInfo.accessToken,
				wbId : userInfo.wbId
			};
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#associateUserDlg"),
	            url: "/User/RelativeUser",
	            data: JSON.stringify(tmpData),
	            dataType: "json",
	            type: "POST",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	if(data.content != null) {
	            		//关联用户成功
	            		$("#associateUserDlg").dialog( "close" );
				    	$("#loginDiv").remove();
				        $("#resultDiv").show();
				        $("#resultDiv .userName").html($.cookie("UserInfo").UserName);
				        $("#resultDiv .userPhoto").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
                		LoginPageView.login.OnFiveSecondsTicket();
	            	}
	            	else {
	            		alert("用户名或者密码错误，请重新输入");
	            	}
	            }
			});
		},
		onROUCloseBtn : function() {
			var userInfo = $("#associateUserDlg").data("userInfo");
			var tmpData = {
				UserName : userInfo.userName,
				sex : userInfo.sex,
				smallHead : userInfo.smallHead,
				lagerHead : userInfo.lagerHead,
				openId : userInfo.openId,
				accessToken : userInfo.accessToken,
				wbId : userInfo.wbId
			};
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#associateUserDlg"),
	            url: "/User/JustLogin",
	            data: JSON.stringify(tmpData),
	            dataType: "json",
	            type: "POST",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            }
			});
			$("#associateUserDlg").dialog( "close" );
	    	$("#loginDiv").remove();
	        $("#resultDiv").show();
	        $("#resultDiv .userName").html(userInfo.userName);
	        $("#resultDiv .userPhoto").prop("src",userInfo.smallHead);
    		LoginPageView.login.OnFiveSecondsTicket();
		},
		CreateAssociateDlg : function(userInfo) {
			var tmpEle = $("<div id='associateUserDlg' >" +
					"<div id='auTabs'>" +
						"<ul>" +
							"<li><a href='#tabs-nu' >新用户</a></li>" +
							"<li><a href='#tabs-ou' >已有用户</a></li>" +
						"</ul>" +
						"<div id='tabs-nu'>" +
							"<div class='description'>QQ账号登陆成功,需关联新用户</div>" +
							"<div class='title' >新用户名：</div>" +
							"<div class='content' >" +
								"<input class='UserNameTB' type='text' />" +
							"</div>" +
							"<div class='title' >登陆密码：</div>" +
							"<div class='content' >" +
								"<input class='PSWTB' type='password' />" +
							"</div>" +
							"<div class='title' >密码确认：</div>" +
							"<div class='content' >" +
								"<input class='EnsurePSWTB' type='password' />" +
							"</div>" +
							"<div class='btn'>" +
								"<input onclick='LoginPageView.login.onRNUOKBtn()' class='sbtn2' type='button' value='确定' />" +
							"</div>" +
							"<div class='pullupDiv' ></div>" +
						"</div>" +
						"<div id='tabs-ou'>" +
							"<div class='description'>QQ账号登陆成功,需关联老用户</div>" +
							"<div class='title' >老用户名：</div>" +
							"<div class='content' >" +
								"<input class='UserNameTB' type='text' />" +
							"</div>" +
							"<div class='title' >登陆密码：</div>" +
							"<div class='content' >" +
								"<input class='PSWTB' type='password' />" +
							"</div>" +
							"<div class='btn'>" +
								"<input onclick='LoginPageView.login.onROUOKBtn()' class='sbtn2' type='button' value='确定' />" +
							"</div>" +
							"<div class='pullupDiv' ></div>" +
						"</div>" +
						"<div class='ui-icon ui-icon-closethick closeBtn' onclick='LoginPageView.login.onROUCloseBtn()'></div>" +
					"</div>" +
				"</div>");
			tmpEle.appendTo($(document.body));
		    $( "#auTabs" ).tabs({
		      event: "mouseover"
		    });
			tmpEle.dialog({
		  		dialogClass: "justOverlayer",
				width: 350,
		  		resizable: false,
		      	modal: true
			});
			tmpEle.data("userInfo",userInfo);
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
	            historyTag: false,
            	refreshTag : true,
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
                    		$("#checkCodeTB").val("");
                    	}
                        else if(data.errCode == -2) {
                        	alert("用户名已被注册，请重新输入");
                        	$("#UserNameTB").val("");
                        	$("#PSWTB").val("");
                        	$("#EPSWTB").val("");
                        }
                        else if(data.errCode == -3) {
                        	alert("邮箱已被注册，请重新输入");
                        	$("#EmailTB").val("");
                        	$("#PSWTB").val("");
                        	$("#EPSWTB").val("");
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

var AjaxLogin = {
	loginDlg : null,
	loginSuccessCB : null,
	OnThreeSecondsTicket : function(successTag) {
		//关闭当前Ajax登陆框
	    var tmpI = parseInt($("#resultDiv .ticket").text());
	    if (tmpI == 0) {
			AjaxLogin.loginDlg.hide();
			if(successTag)
				AjaxLogin.loginSuccessCB();
	    }
	    else {
	        setTimeout(AjaxLogin.OnThreeSecondsTicket, 1000);
	        $("#resultDiv .ticket").text(tmpI - 1);
	    }
	},
	OnLoginBtnClick : function(ev) {
		//ajax登陆
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
            historyTag: false,
        	refreshTag : true,
            loadEle: $("#loginDiv"),
            url: "/User/Login",
            data: JSON.stringify(obj),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                var tmpForm = $("#loginer");
                if (!data.successTag) {
                	if(data.errCode == -3) {
                		//多次登陆异常,登陆异常
	                    tmpForm.empty();
		        		var errEle = $("<div id='abnormal'>" +
	                        "<div>登陆异常，错误次数过多，请等待一段时间重试。</div>" +
	                        "<div>" +
	                            "等待时间：" +
	                            "<label id='waitTime'>" +
	                                data.LOCK +
	                            "</label>" +
	                        "</div>" +
	                        "<div class='bottom'><label>在<span class='ticket'>3</span>秒后自动</label>" +
	                        "<a id=returnBtn href='#'>返回</a>" +
	                    "</div>");
	                    errEle.appendTo(tmpForm);
	                    AjaxLogin.OnThreeSecondsTicket(false);
	                	errEle.find("#returnBtn").on("click",function(ev){
	                		AjaxLogin.loginDlg.hide();
	            		});
                	}
                	else {
                		if(data.errCode == -2) {
                			alert("验证码错误，请重新输入");
	                	}
	                	else {
	                		alert("密码或者用户名错误，请重新输入！");
	                		$("#UIDTB").val("");
	                		$("#PSWTB").val("");
	                	}
		        		$("#UIDTB").val("");
        				$("#PSWTB").val("");
	                	if(data.custom == -1){
			        		LoginPageView.register.OnRefreshCCode();	//刷新验证码
//		        			$.validator.unobtrusive.parse($("#loginer"));	//没效
	                	}
                	}
                }
                else {
                	//登陆成功，自动返回
                	tmpForm.empty();
                	var successEle = $("<div id='resultDiv'>" +
	                		"<label class='blb'>欢迎<img class='userPhoto' onerror='javascript:this.src = '/picture/user/NoPicture_40_40.png';' src='/picture/user/" + GetUIDHex(data.content.UID) + "_40_40.jpg')' /><a class='userName' href='/User/Details'>" + data.content.UserName + "</a>光临Right Go网</label>" +
	            			"<label class='blb welcome'>Welcome Welcome</label>" +
	            			"<div class='bottom'><label>在<span class='ticket'>30</span>秒后自动</label>" +
	            			"<a id=returnBtn href='#'>返回</a>" +
				        "</div>");
	                successEle.appendTo(tmpForm);
                	AjaxLogin.OnThreeSecondsTicket(true);
                	successEle.find("#returnBtn").on("click",function(ev){
                		AjaxLogin.loginDlg.hide();
						AjaxLogin.loginSuccessCB();
            		});
                }
            }
        });
	},
	ShowAjaxLoginDlg : function(LoginSuccessCB) {
		AjaxLogin.loginSuccessCB = LoginSuccessCB;
		if(AjaxLogin.loginDlg == null) {
			AjaxLogin.loginDlg = $(
				"<div id='loginOverlayout'>" +
					"<div id='centerDiv'>" +
						"<div id='ajaxloginDiv'>" +
				            "<form id='loginer' class='ele'>" +
				                "<div class='ltTitle'>用户登陆</div>" +
				                "<div class='content UIDDIV'>" +
				                    "<input id='UIDTB' name='UIDTB' class='enterTB' type='text' data-val='true' data-val-required='必填字段' />" +
				                    "<div class='inBox'>用户名</div>" +
				                "</div>" +
				                "<div class='content'>" +
				                    "<input id='PSWTB' name='PSWTB' class='enterTB' type='password' data-val='true' data-val-required='必填字段' data-val-length='密码为6-16位字母或数字' data-val-length-min='6' data-val-length-max='16' />" +
				                    "<div class='inBox'>密&#12288;码</div>" +
				                "</div>" +
				                "<div class='content ccDiv'>" +
//			                        "<input id='checkCodeTB' name='checkCodeTB' class='enterTB' type='text' data-val='true' data-val-required='必填字段' data-val-length='' data-val-length-min='4' data-val-length-max='4' />" +
//			                        "<div class='inBox'>验证码</div>" +
//			                        "<img class='inBoxIMG' onclick='LoginPageView.login.OnCheckCodeImgClick()' id='ccImg' src='/Home/GetValidateCode' />" +
				                "</div>" +
				                "<div class='content'>" +
				                    "<input id='loginBtn' type='submit' value='登陆' />" +
				                "</div>" +
				                "<div class='pullupDiv'></div>" +
				            "</form>" +
				            "<div class='other'>" +
				                "<span id='qqLoginBtn'></span>" +
				                "<span id='wb_connect_btn'></span>" +
				            "</div>" +
				            "<div class='forgot'>" +
				                "<a>忘记密码</a>" +
				            "</div>" +
				            "<div class='registerDiv'>" +
				                "<a id='registerBtn' href='/Home/Register'>立即注册账号</a>" +
				            "</div>" +
				        "</div>" +
			        "</div>" +
		        "</div>");
	        AjaxLogin.loginDlg.appendTo($("body"));
		 	var tmpForm = $("#loginer");
	        $.validator.unobtrusive.parse(tmpForm);
		    var validator = tmpForm.validate();
		    $.extend(validator.settings, {
		        submitHandler: function () {
		            AjaxLogin.OnLoginBtnClick();
		        }
		    });
		}
		else {
			AjaxLogin.loginDlg.show();
		}
	}
};
