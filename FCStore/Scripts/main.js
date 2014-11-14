$(function () {
	//类别分类选择器
    (new SidebarFollow()).init({
        element: $('#Categorys'),
        distanceToTop: 10
    });
    $("#Categorys .item:gt(0)").bind("mouseenter", function (ev) {
        var item = $(ev.currentTarget);
        item.addClass("hover");
        var subDiv = item.next(".subCategory");
        subDiv.show();
        var height = subDiv.outerHeight();
        var os = subDiv.offset();
        var pos = subDiv.position();
        var oY = os.top + height - $(document).scrollTop() - $(window).height();
        if (oY > 0) {
            subDiv.animate({ "top": pos.top - oY }, "fast");
        }
    }).bind("mouseleave", function (ev) {
        var item = $(ev.currentTarget);
        var subDiv = item.next(".subCategory");
        if ($(ev.toElement)[0] != subDiv[0] && $(ev.toElement).parents('.subCategory').length == 0) {
            subDiv.hide();
            subDiv.css({ top: -1 });
            item.removeClass("hover");
        }
        else
            subDiv.one("mouseleave", function (ev) {
                $(this).hide();
                $(this).css({ top: -1 });
                subDiv.unbind();
                item.removeClass("hover");
            });
    });
    
    //顶层滚动
    (new SidebarFollow()).init({
        element: $('#TopBtn'),
        distanceToTop: "90%",
        afterFollowCB : function(ele) {
        	if(!MainLayout.topBtnTag && $(document).scrollTop() != 0)
        		ele.show();
        },
        afterStopCB : function(ele) {
        	if(!MainLayout.topBtnTag)
        		ele.hide();
        }
    });
    (new SidebarFollow()).init({
        element: $('#Contacts'),
        distanceToTop: 10
    });
    
    $("#haiiskefu .kfleft").click(function(){
		var i=$("#haiiskefu").css("right");
		if (i=='0px'){
			$('#haiiskefu').animate({right:-80}, 200);
		} else {
			$('#haiiskefu').animate({right:0}, 200);
		}
	});
	
	$("#orderBtn").on("click",MainLayout.onOrderBtnClick);
	$("#onlineBtn").on("click",MainLayout.onOnlineBtnClick);
});

var MainLayout = {
	onSetHomeBtnClick : function(obj) {
		var vrl = window.location.href;
		try{ 
			obj.style.behavior='url(#default#homepage)';
			obj.setHomePage(vrl); 
		} 
		catch(e){ 
			if(window.netscape) { 
				try { 
					netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect"); 
				} 
				catch (e) { 
					alert("此操作被浏览器拒绝！\n请在浏览器地址栏输入“about:config”并回车\n然后将 [signed.applets.codebase_principal_support]的值设置为'true',双击即可。"); 
				} 
				var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch); 
				prefs.setCharPref('browser.startup.homepage',vrl); 
			}
		}
	},
	onAddFavoriteBtnClick : function() {
		var sURL = window.location.href;
		var sTitle = document.title;
		try 
		{ 
			window.external.addFavorite(sURL, sTitle); 
		} 
		catch (e) 
		{ 
			try 
			{ 
				window.sidebar.addPanel(sTitle, sURL, ""); 
			} 
			catch (e) 
			{ 
				alert("加入收藏失败，请使用Ctrl+D进行添加"); 
			} 
		}
	},
	topBtnTag : false,
	onTopBtnClick : function(ele) {
		MainLayout.topBtnTag = true;
		$(ele).hide();
        //滚动到产品顶部
        $("html,body").animate({
        	scrollTop:0
    	},"fast","linear",function() {
    		MainLayout.topBtnTag = false;
    	});
	},
	enterPLInFavorit : function() {
		var target = $("#plInFavorit");
    	var ICount = target.children(".MoveItem").length;
    	var tmpW = ICount * 95;
    	target.animate({
    		width : tmpW
    	},"normal","linear",function(){
		    $("#plInFavorit").one("mouseleave",MainLayout.leavePLInFavorit);
    	});
	},
	leavePLInFavorit : function() {
		var target = $("#plInFavorit");
		target.animate({
    		top : 30,
    		width : 40
    	},"normal","linear",function(){
    		$("#plInFavorit").one("mouseenter",MainLayout.enterPLInFavorit);
    	});
	},
	enterPLInCar : function() {
		var target = $("#plInCar");
    	var ICount = target.children(".MoveItem").length;
    	var tmpH = ICount * 100;
    	var tmpT = 10;
    	target.animate({
    		top : tmpT - (tmpH - 25),
    		height : tmpH
    	},"normal","linear",function(){
		    $("#plInCar").one("mouseleave",MainLayout.leavePLInCar);
    	});
	},
	leavePLInCar : function() {
		var target = $("#plInCar");
		target.animate({
    		top : 10,
    		height : 25
    	},"normal","linear",function(){
    		$("#plInCar").one("mouseenter",MainLayout.enterPLInCar);
    	});
	},
	FoldPLInCar : function() {
		$("#plInCar").animate({
    		top : 10,
    		height : 25
    	},"normal","linear");
		$("#plInCar").one("mouseenter",MainLayout.enterPLInCar);
	},
	onDeleteCarItem : function(obj) {
		//删除购物车栏目，并且刷新cookie状态
		var delBtn = $(obj);
		var delItem = delBtn.parent(".MoveItem");
		var delIndex = delItem.prevAll().length;
		delItem.animate({
			"margin-left" : "-100%"
		},"normal","linear",function(){
			delItem.remove();
			var target = $("#plInCar");
			target.animate({
	    		top : parseInt(target.css("top")) + 77,
	    		height : parseInt(target.css("height")) - 77
	    	},"fast","linear");
		});
		$.myAjax({
        	historyTag : false,
        	loadEle : null,
            url: "/OrderPacket/Delete/" + delIndex,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	
            }
		});
		return false;
	},
	BrandListHeight: $("#Brands .content").height(),
	ExtentTag: false,
	MarginTop: 0,
	BCPerPage: Math.floor($(document.body).width() / 121),
	HRBC: Math.floor($(document.body).width() / 121),
    BInterval : null,
	onExtentClick : function(obj) {
	    if (MainLayout.ExtentTag) {
	        MainLayout.ShrinkBrandList(obj);
	    }
	    else {
	        MainLayout.ExtendBrandList(obj);
	    }

	},
	StartBrandMove: function () {
	    MainLayout.MarginTop = 0;
	    $("#Brands .content").css({
	        marginTop: MainLayout.MarginTop
	    });
	    MainLayout.HRBC = MainLayout.BCPerPage * 2;
	    MainLayout.BInterval = setInterval(function () {
	        MainLayout.MarginTop -= 60;
	        $("#Brands .content").animate({
	            marginTop: MainLayout.MarginTop
	        }, "fast", "linear");
	        $("#Brands img:lt(" + (MainLayout.BCPerPage + MainLayout.HRBC) + ")").trigger("sporty");
	        MainLayout.HRBC += MainLayout.BCPerPage;
	        if ((MainLayout.MarginTop - 60) * -1 >= MainLayout.BrandListHeight) {
	            MainLayout.HRBC = MainLayout.BCPerPage;
	            MainLayout.MarginTop = 60;
	        }
	    }, 4000);
	},
	StopBrandMove : function() {
	    clearInterval(MainLayout.BInterval);
	    $("#Brands .content").css({
	        marginTop: 0
	    });
	},
    ShrinkBrandList: function (obj) {
        var target = $(obj);
	    var contentDiv = $("#Brands .content");
	    contentDiv.animate({
	        height: 58
	    }, "slow", "linear", function () {
	        contentDiv.height(MainLayout.BrandListHeight);
	        $("#Brands").css({
	            overflow: "hidden"
	        });
	        MainLayout.ExtentTag = false;
	        target.toggleClass("downBtn");
	    });
	    MainLayout.StartBrandMove();
	},
    ExtendBrandList: function (obj) {
        MainLayout.StopBrandMove();
	    var contentDiv = $("#Brands .content");
	    var target = $(obj);
	    $("#Brands img").trigger("sporty");
	    contentDiv.css({
	        height: 58
	    });
	    $("#Brands").css({
	        overflow: "visible"
	    });
	    contentDiv.animate({
	        height: MainLayout.BrandListHeight
	    }, "slow", "linear", function () {
	        MainLayout.ExtentTag = true;
	        target.toggleClass("downBtn");
	    });
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
			UserName : userInfo.nickname,
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
            		$("#loginDiv").hide();
            		$("#userInfoDiv").show();
            		$("#userInfoDiv .userHead").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
            		$("#userInfoDiv .userName").html($.cookie("UserInfo").UserName);
            	}
            	else {
            		alert("已有同名用户，请重新修改提交。");
            	}
            }
		});
	},
	onROUCloseBtn : function() {
		var userInfo = $("#associateUserDlg").data("userInfo");
		$("#associateUserDlg").dialog( "close" );
		$("#loginDiv").hide();
		$("#userInfoDiv").show();
		$("#userInfoDiv .userHead").prop("src",userInfo.smallHead);
		$("#userInfoDiv .userName").html(userInfo.userName);
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
			figureurl_qq_1 : userInfo.smallHead,
			figureurl_qq_2 : userInfo.lagerHead,
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
            		$("#loginDiv").hide();
            		$("#userInfoDiv").show();
            		$("#userInfoDiv .userHead").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
            		$("#userInfoDiv .userName").html($.cookie("UserInfo").UserName);
            	}
            	else {
            		alert("用户名或者密码错误，请重新输入");
            	}
            }
		});
	},
	onExitBtnClick : function() {
		if(QC.Login.check()) {
			QC.Login.signOut();
		}
		if(WB2.checkLogin()) {
			WB2.logout(function(){
				window.location.href = "/User/Exit";
			});
			return;
		}
		window.location.href = "/User/Exit";
	},
	WBLoginSuccess : function(o) {
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
						MainLayout.CreateAssociateDlg(userInfo);
	            	}
	            	else {
	            		//关联了
	            		$("#loginDiv").hide();
	            		$("#userInfoDiv").show();
	            		$("#userInfoDiv .userHead").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
	            		$("#userInfoDiv .userName").html($.cookie("UserInfo").UserName);
	            	}
	            }
			});
		}
	},
	QQLoginSuccess : function(reqData, opts) {
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
		            		//未关联，要求输入ID和密码
							MainLayout.CreateAssociateDlg(userInfo);
		            	}
		            	else {
		            		//关联了
		            		$("#loginDiv").hide();
		            		$("#userInfoDiv").show();
		            		$("#userInfoDiv .userHead").prop("src","/picture/user/" + GetUIDHex($.cookie("UserInfo").UID) + "_40_40.jpg");
		            		$("#userInfoDiv .userName").html($.cookie("UserInfo").UserName);
		            	}
		            }
				});
	    	});
    	}
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
							"<input onclick='MainLayout.onRNUOKBtn()' class='sbtn2' type='button' value='确定' />" +
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
							"<input onclick='MainLayout.onROUOKBtn()' class='sbtn2' type='button' value='确定' />" +
						"</div>" +
						"<div class='pullupDiv' ></div>" +
					"</div>" +
					"<div class='ui-icon ui-icon-closethick closeBtn' onclick='MainLayout.onROUCloseBtn()'></div>" +
				"</div>" +
			"</div>");
		tmpEle.appendTo($("#Main"));
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
	},
	OrderDlg : null,
	createOrderPage : function() {
		if(MainLayout.OrderDlg == null) {
			var ele = $(
				"<div id='orderPage'>" +
					"<ul>" +
						"<li class='title'>国家:</li>" +
						"<li class='content'>" +
							"<div id='OPCountrySel' class='input'></div>" +
						"</li>" +
						"<li class='title'>商品品牌：</li>" +
						"<li class='content'><input id='OPBrandTB' class='input' placeholder='精确商品品牌（可填）' type='text' /></li>" +
						"<li class='title'>商品名字：</li>" +
						"<li class='content'><input id='OPNameTB' class='input' placeholder='详细商品名字（可填）' type='text' /></li>" +
						"<li class='title'>商品描述：</li>" +
						"<li class='content clearLeft'><textarea id='OPDescriptionTB' class='input' placeholder='介绍商品颜色，商品尺寸，商品形状，商品用途。越详细越提供更符合客人的需求（可填）等'></textarea></li>" +
						"<li class='title p1'>可接受的价格范围(人民币￥)</li>" +
						"<li class='minPriceRange priceRange'><input id='OPMinTB' placeholder='最低阶（可填）' type='text' /></li>" +
						"<li class='line'>-</li>" +
						"<li class='priceRange'><input id='OPMaxTB' placeholder='最高价（可填）' type='text' /></li>" +
						"<li class='title'>备注：</li>" +
						"<li class='content clearLeft'><textarea id='OPRemarkTB' class='input' placeholder='例如需求是否急切，需求量等（可填）'></textarea></li>" +
					"</ul>" +
				"</div>");
			ele.appendTo($("body:first"));
			MainLayout.OrderDlg = ele.dialog({
	      		autoOpen: true,
		      	width: 600,
		      	height: 530,
	     	 	modal: true,
	  			buttons: {
	    			"下单": function() {
	    				var SaveCustomOrder = function() {
	    					tmpData.UID = $.cookie("UserInfo").UID;
		    				$.myAjax({
					        	historyTag : false,
					        	loadEle : ele.parent(),
					            url: "/CustomOrder/Save/",
					            data: JSON.stringify(tmpData),
					            dataType: "json",
					            type: "POST",
					            contentType: "application/json;charset=utf-8",
					            success: function (data,status,options) {
					            	if(data.content) {
					            		ele.find("input").val("");
					            		ele.find("textarea").val("");
					            	}
					            }
		    				});
		    				alert("下单成功，请等待联系。");
		    				MainLayout.OrderDlg.dialog("close");
	    				};
	    				var tmpData = {
	    					CountryName : ele.find("#OPCountrySel input").val(),
	    					BrandName : ele.find("#OPBrandTB").val(),
	    					ProductName : ele.find("#OPNameTB").val(),
	    					Description : ele.find("#OPDescriptionTB").val(),
	    					MinPrice : ele.find("#OPMinTB").val(),
	    					MaxPrice : ele.find("#OPMaxTB").val(),
	    					Remark : ele.find("#OPRemarkTB").val()
	    				};
	    				//判断是否注册
	    				if(typeof $.cookie("UserInfo") == 'undefined') {
	    					//未登陆
	    					alert("请先登陆");
	    					//Ajax登陆
	    					AjaxLogin.ShowAjaxLoginDlg(SaveCustomOrder);
	    				}
	    				else {
	    					SaveCustomOrder();
	    				}
	    			}
	  			}
		    });
		    $("#OPCountrySel").myCombobox({
				placeholder : "可以多个国家如：美国，英国（可填）",
				loadUrl : "/CustomOrder/GetEnableCountry/"
		    });
		}
		else {
			MainLayout.OrderDlg.dialog( "open" );
		}
	},
	onOrderBtnClick : function(ev) {
		MainLayout.createOrderPage();
	},
	onOnlineBtnClick : function(ev) {
		
	}
};

var ProductList = {
    onByCategoryOrderClick : function(obj) {
    	var PIndex = 1;
    	var target = $(obj);
    	var parent = target.parent();
    	var child = parent.children('.asc,.desc');
    	if($.inArray(obj,child) != -1) {
    		if(child.elementHaveClassName('asc')) {
    			child.removeClass('asc').addClass('desc');
    		}
    		else {
    			child.removeClass('desc').addClass('asc');
    		}
    	}
    	else {
    		child.prop('class','orderTag');
    		target.addClass('desc');
    	}
    	ProductList.getProductListByCategory(PIndex);
    },
    onByCategoryPageClick : function (PA) {
        var parArr = window.location.pathname.split("/");
        var PIndex = parArr.length > 4 ? parseInt(parArr[4]) : 1;
        if(PA == -1){
        	PIndex += 1;
        }
        else if(PA == -2) {
        	PIndex -= 1;
        }
        else {
        	PIndex = PA;
        }
        if(PIndex < 1)
        	return;
        ProductList.getProductListByCategory(PIndex);
        //滚动到产品顶部
        $("html,body").animate({
        	scrollTop:0
    	},1000,"linear");
    },
    getProductListByCategory: function (PIndex) {
        var whereStr = "";
        //获得品牌过滤
        $.each($("#plBrands .brandCB"), function (i, n) {
            var item = $(n);
            if (item.prop("checked"))
                whereStr += "0x" + item.val();
        });
        var orderStr = "";
        //获得排序
        $.each($("#plTool .orderTag"), function (i, n) {
            var item = $(n);
            if (item.elementHaveClassName('asc'))
                orderStr = "0x1" + i;
            else if (item.elementHaveClassName('desc'))
                orderStr = "0x0" + i;
        });
        var parArr = window.location.pathname.split("/");
        var CID = parArr[3];
        //ajaz获取数据，更新内容
        $.myAjax({
        	historyTag : true,
        	loadEle : $("#Center"),
            url: "/Product/ListByCategory/" + CID + "/" + PIndex + "/" + orderStr + "/" + whereStr,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	//更新产品列表
            	var parent = $("#plDiv");
               	parent.empty();
                $.each(data.content.Products,function(i,n) {
					parent.append(ProductList.buildProductView(n));
				});
				BuildPullHeightDiv(parent);
		        var PCount = data.content.PageCount;
		        PIndex = data.content.PageIndex;
				$(".page .PCLabel").text(PCount);
				//更新Page
		        var CID = parseInt(data.content.Category.CID);
		        var par = $(".page .pIndexItem");
				$("#GoPIInput").val(PIndex);
		        par.empty();
		        //重新构造页
		        var fpItem = $("<a class=\"pItem fpItem " + (PIndex == 1 ? "gray" : "") + (PIndex == 1 ? "\"" : "\" href=\"" + "/Product/ListByCategory/" + CID + "/" + (PIndex - 1) + "\"") + ">上一页</a>");
		        fpItem.appendTo(par);
		        if(PIndex != 1) {
			        fpItem.bind('click',function(ev){
			        	ProductList.onByCategoryPageClick(-2);
			        	return false;
			        });
		        }
		        var html = "";
		        if (PIndex >= 1)
		        {
		            html += "<a class=\"pItem " + (PIndex == 1 ? "in" : "") + "\" href=\"/Product/ListByCategory/" + CID + "/1\" onclick=\"ProductList.onByCategoryPageClick(1); return false;\">1</a>";
		        }
		        if (PIndex >= 2)
		        {
		            html += "<a class=\"pItem " + (PIndex == 2 ? "in" : "") + "\" href=\"/Product/ListByCategory/" + CID + "/2\" onclick=\"ProductList.onByCategoryPageClick(2); return false;\">2</a>";
		        }
		        if (PIndex - (PCount - PIndex > 2 ? 0 : PCount - PIndex) - 3 > 2)
		        {
		            html += "<label>...</label>";
		        }
		        var tmpC = 5;
		        var tmpPI = 1;
		        if (PIndex <= 2)
		        {
		            tmpC = Math.min(5 - PIndex, PCount - PIndex);
		            tmpPI = PIndex + 1;
		        }
		        else
		        {
		            tmpPI = Math.max(Math.min(PIndex - 2, PCount - 4), 3);
		            tmpC = Math.min(5, PCount - tmpPI + 1);
		        }
		        for (i = tmpPI; i < tmpPI + tmpC; i++)
		        {
		            html += "<a class=\"pItem " + (PIndex == i ? "in" : "")  + "\" href=\"/Product/ListByCategory/" + CID + "/" + i + "\" onclick=\"ProductList.onByCategoryPageClick(" + i + "); return false;\">" + i + "</a>";
		        }
		        par.append($(html));
		        var lpItem = $("<a class=\"pItem lpItem " + (PIndex == PCount ? "gray" : "") + (PIndex == PCount ? "\"" : "\" href=\"" + "/Product/ListByCategory/" + CID + "/" + (PIndex + 1) + "\"") + ">下一页</a>");
		        lpItem.appendTo(par);
		        if(PIndex != PCount) {
			        lpItem.bind("click",function(ev){
			        	ProductList.onByCategoryPageClick(-1);
			        	return false;
			        });
		        }
            }
        });
    },
    onByBrandOrderClick : function(obj) {
    	var PIndex = 1;
    	var target = $(obj);
    	var parent = target.parent();
    	var child = parent.children('.asc,.desc');
    	if($.inArray(obj,child) != -1) {
    		if(child.elementHaveClassName('asc')) {
    			child.removeClass('asc').addClass('desc');
    		}
    		else {
    			child.removeClass('desc').addClass('asc');
    		}
    	}
    	else {
    		child.prop('class','orderTag');
    		target.addClass('desc');
    	}
    	ProductList.getProductListByBrand(PIndex);
    },
    onByBrandPageClick : function (PA) {
        var parArr = window.location.pathname.split("/");
        var PIndex = parArr.length > 4 ? parseInt(parArr[4]) : 1;
        if(PA == -1){
        	PIndex += 1;
        }
        else if(PA == -2) {
        	PIndex -= 1;
        }
        else {
        	PIndex = PA;
        }
        if(PIndex < 1)
        	return;
        ProductList.getProductListByBrand(PIndex);
        //滚动到产品顶部
        $("html,body").animate({
        	scrollTop:0
    	},1000,"linear");
    },
    getProductListByBrand: function (PIndex) {
        var orderStr = "";
        //获得排序
        $.each($("#plTool .orderTag"), function (i, n) {
            var item = $(n);
            if (item.elementHaveClassName('asc'))
                orderStr = "0x1" + i;
            else if (item.elementHaveClassName('desc'))
                orderStr = "0x0" + i;
        });
        var parArr = window.location.pathname.split("/");
        var BID = parArr[3];
        //ajaz获取数据，更新内容
        $.myAjax({
        	historyTag : true,
        	loadEle : $("#Center"),
            url: "/Product/ListByBrand/" + BID + "/" + PIndex + "/" + orderStr,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	//更新产品列表
            	var parent = $("#plDiv");
               	parent.empty();
                $.each(data.content.Products,function(i,n) {
					parent.append(ProductList.buildProductView(n));
				});
				BuildPullHeightDiv(parent);
				//更新Page
		        var parArr = window.location.pathname.split("/");
		        var par = $(".page .pIndexItem");
		        var PCount = data.content.PageCount;
		        PIndex = data.content.PageIndex;
		        $(".page .PCLabel").text(PCount);
		        var BID = data.content.Brand.BID;
				$("#GoPIInput").val(PIndex);
		        par.empty();
		        //重新构造页
		        var fpItem = $("<a class=\"pItem fpItem " + (PIndex == 1 ? "gray" : "") + (PIndex == 1 ? "\"" : "\" href=\"" + "/Product/ListByBrand/" + BID + "/" + (PIndex - 1) + "\"") + ">上一页</a>");
		        fpItem.appendTo(par);
		        if(PIndex != 1) {
			        fpItem.bind('click',function(ev){
			        	ProductList.onByBrandPageClick(-2);
			        	return false;
			        });
		        }
		        var html = "";
		        if (PIndex >= 1)
		        {
		            html += "<a class=\"pItem " + (PIndex == 1 ? "in" : "") + "\" href=\"/Product/ListByBrand/" + BID + "/1\" onclick=\"ProductList.onByBrandPageClick(1); return false;\">1</a>";
		        }
		        if (PIndex >= 2)
		        {
		            html += "<a class=\"pItem " + (PIndex == 2 ? "in" : "") + "\" href=\"/Product/ListByBrand/" + BID + "/2\" onclick=\"ProductList.onByBrandPageClick(2); return false;\">2</a>";
		        }
		        if (PIndex - (PCount - PIndex > 2 ? 0 : PCount - PIndex) - 3 > 2)
		        {
		            html += "<label>...</label>";
		        }
		        var tmpC = 5;
		        var tmpPI = 1;
		        if (PIndex <= 2)
		        {
		            tmpC = Math.min(5 - PIndex, PCount - PIndex);
		            tmpPI = PIndex + 1;
		        }
		        else
		        {
		            tmpPI = Math.max(Math.min(PIndex - 2, PCount - 4), 3);
		            tmpC = Math.min(5, PCount - tmpPI + 1);
		        }
		        for (i = tmpPI; i < tmpPI + tmpC; i++)
		        {
		            html += "<a class=\"pItem " + (PIndex == i ? "in" : "")  + "\" href=\"/Product/ListByBrand/" + BID + "/" + i + "\" onclick=\"ProductList.onByBrandPageClick(" + i + "); return false;\">" + i + "</a>";
		        }
		        par.append($(html));
		        var lpItem = $("<a class=\"pItem lpItem " + (PIndex == PCount ? "gray" : "") + (PIndex == PCount ? "\"" : "\" href=\"" + "/Product/ListByBrand/" + BID + "/" + (PIndex + 1) + "\"") + ">下一页</a>");
		        lpItem.appendTo(par);
		        if(PIndex != PCount) {
			        lpItem.bind("click",function(ev){
			        	ProductList.onByBrandPageClick(-1);
			        	return false;
			        });
		        }
            }
        });
    },
    buildProductView : function(product) {
    	var item = $(
    	'<div class=\"item\">' +
            '<a href="/Product/Detail/' + product.PID + '">' +
                '<div class="img">' +
                    '<img src="' + product.ImgPathArr[0] + '" />' +
                '</div>' +
                '<div class="title">' + product.Title + '</div>' +
                '<div class="marketPrice">' +
                    '市场价：<label>￥' + PriceFormat(product.MarketPrice) + '</label>' +
                '</div>' +
                '<div class="price">' +
                    '代购价：<label>￥' + PriceFormat(product.Price) + '</label>' +
                '</div>' +
            '</a>' +
        '</div>');
        return item;
    },
    onFavoriteClick : function() {
        window.location = "/Keep/List";
    },
    onSBMoreBtnClick : function(ele) {
    	//显示更多的Brand
    	if($(ele).hasClass("upTag")) {
	    	$("#plBrands").parent().animate({
	    		height : 102
	    	},"fast","linear");
	    	$(ele).removeClass("upTag");
    	}
    	else {
	    	$("#plBrands").parent().animate({
	    		height : $("#plBrands").height()
	    	},"fast","linear");
	    	$(ele).addClass("upTag");
    	}
    }
};

var CartPage = {
	onDeleteBtnClick : function() {
		var OID = $("#CartDiv").attr("data");
		if(OID != -1)
		{
			if($("#CartDiv .order .checkItem:checked").length == 0) {
				alert("请选择至少一项商品");
				return;
			}
			var tmpStr = OID + ",";
			$.each($("#CartDiv .order .checkItem:checked"),function(i,n){
				tmpStr += $(n).val() + ",";
			});
			tmpStr = tmpStr.slice(0,tmpStr.length - 1);
			//删除该商品
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#CartDiv .addresses .scrollDiv"),
	        	url: "/Order/DeletePacket/" + tmpStr,
	            data:  null,
	            dataType: "json",
	            type: "GET",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	if(data.content == "OK") {
	            		$.each($("#CartDiv .order .checkItem:checked").parentsUntil("ul"),function(i,n){
							$(n).animate({
								height:0
							}, "normal", "linear",function(){
								$(n).remove();
			            	});
						});
	            	}
	            }
			});
		}
	},
	onKeepBtnClick : function() {
		if($("#CartDiv .order .checkItem:checked").length == 0) {
			alert("请选择至少一项商品");
			return;
		}
		var tmpStr = "";
		var MIArr = [];
		$.each($("#CartDiv .order .checkItem:checked"),function(i,n){
			tmpStr += $(n).attr("data") + ",";
			var obj = {
				ele : $(n).parent().next(),
				html : $(n).parent().next().next().children().html()
			}
			MIArr.push(obj);
		});
		//收藏该商品
		$.myAjax({
        	historyTag : false,
        	loadEle : $("#CartDiv .addresses .scrollDiv"),
        	url: "/Keep/Add/" + tmpStr,
            data:  null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	if(data.content == "OK") {
            		$.each(MIArr,function(i,n){
            			var viewItem = n.ele;
            			var tmpOS = viewItem.offset();
	            		var tmpW = viewItem.width();
	            		var tmpH = viewItem.height();
	            		var tmpItem = viewItem.clone(false,false).addClass('MoveItem');
	            		tmpItem.appendTo($("body:first"));
	            		tmpItem.css({
	            			top:tmpOS.top,
	            			left:tmpOS.left,
	            			width:tmpW,
	            			height:tmpH,
	            			padding:viewItem.css("padding")
	            		});
	            		var favorite = $("#Favorite");
	            		tmpOS = favorite.offset();
	            		tmpW = favorite.width();
	            		tmpH = favorite.height();
	        			tmpItem.animate({
		            			top:tmpOS.top - tmpH + 180,
		            			left:tmpOS.left + 130,
		            			width:88,
		            			height:88
		            		},{
		            		queue:false,
		            		duration:1000,
		            		complete : function() {
		            			var moveContent = $("<label>" + n.html + "</label>");
		            			tmpItem.removeAttr("style");
		            			tmpItem.append(moveContent);
		            			//收藏夹增加内容
		            			tmpItem.appendTo($("#plInFavorit"));
		            		}
		            	});
            		});
            	}
            }
		});
	},
	onAreaChangeCB : function() {
		var tmpStr = $("#AreaSelector .province  option:selected").text() + " " + $("#AreaSelector .city  option:selected").text() + " " + $("#AreaSelector .county  option:selected").text() + " ";
		$("#addressTA").val(tmpStr);
	},
	onAddressItemClick : function(ev) {
		var target = $(ev.currentTarget);
		if(target.prev().length > 0) {
			//不是第一个,ajax修改默认地址
			var AID = target.attr("data");
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#CartDiv .addresses .scrollDiv"),
	        	url: "/User/SelectDefaultAddress/" + AID,
	            data: null,
	            dataType: "json",
	            type: "GET",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	var tmpOF = target.offset(); 
	            	var top = tmpOF.top; 
	            	var left = tmpOF.left;
	            	var scrollDiv = target.parentsUntil(".addresses").last();
	            	var ml = scrollDiv.offset().left;
	            	var iItem = target.parent().children().first();
	            	target.appendTo($("body:first"));
	            	target.css({position:"absolute",top:top + "px",left:left + "px"});
	            	target.animate({
	            		left : ml
	            	}, "normal", "linear",function(){
	            		scrollDiv.animate({
	            			scrollLeft:0
	            		});
	            		target.insertBefore(iItem);
	            		target.removeAttr("style");
	            	});
	            }
			});
		}
	},
	onPostTypeItemClick : function(ev) {
		var target = $(ev.currentTarget);
		var tmpData = parseInt(target.attr("data"));
		$("#CartDiv .postType .item").removeClass("sel");
		switch(tmpData) {
			case 0 : {
				//直邮
				$("#CartDiv .postType .item:eq(0)").addClass("sel");
				$("#CartDiv .order .li41 input:eq(0)").attr("checked",'checked');
				$("#CartDiv .order .withPostPay").show();
				$("#CartDiv .order .withoutPostPay").hide();
				break;
			}
			case 1 : {
				//转邮
				$("#CartDiv .postType .item:eq(1)").addClass("sel");
				$("#CartDiv .order .li41 input:eq(1)").attr("checked",'checked');
				$("#CartDiv .order .withoutPostPay").show();
				$("#CartDiv .order .withPostPay").hide();
				break;
			}
		}
	},
	onAddAddressBtnClick : function() {
		$("#addAddressDlg").dialog();
		$("#addAddressDlg").show();
	},
	onAAEnsure : function() {
		var tmpStr = $("#AreaSelector .province  option:selected").text() + " " + $("#AreaSelector .city  option:selected").text() + " " + $("#AreaSelector .county  option:selected").text() + " ";
		var areaStr = $("#addressTA").val()
		if(tmpStr == areaStr) {
			alert("请输入详细的地址。");
			$("#addressTA").focus();
		}
		else {
			//添加联系地址
			var address = {
				AddID : 0,
				Contacts : $("#contactsTB").val(),
				TownID : $("#AreaSelector .county").val(),
				AddressName : $("#addressTA").val(),
				Phone : $("#phoneTB").val(),
				PostCode : $("#postTB").val()
			};
			$.myAjax({
	        	historyTag : false,
	        	loadEle : $("#addAddressDlg"),
	            url: "/User/AddAddress/",
	            data: JSON.stringify(address),
	            dataType: "json",
	            type: "POST",
	            contentType: "application/json;charset=utf-8",
	            success: function (data,status,options) {
	            	var item = $("<div data=\"" + data.content.AddID + "\" class=\"addressItem sel\">" +
	                                "<div class=\"name\">" +
	                                    data.content.Contacts + "收" +
	                                "</div>" +
	                                "<div class=\"addressStr\">" +
	                                    data.content.AddressName +
	                                "</div>" +
	                                "<div class=\"tip\">" +
	                                    "收货地址" +
	                                "</div>"+
	                            "</div>");
	                var selItem = $("#CartDiv .addresses .content .sel");
	                if(selItem.length == 0) {
	                	selItem = $("#CartDiv .addresses .content .addressItem");
	                }
	                if(selItem.length > 0) {
	                	item.insertBefore(selItem);
	                	selItem.removeClass("sel");
	                }
	                else {
	                	item.appendTo($("#CartDiv .addresses .content"));
	                }
	                $("#CartDiv .addresses .content").width(293 * $("#CartDiv .addresses .content").children().length);
	            }
			});
			$("#addAddressDlg").dialog("close");
		}
	},
	onAACancel : function() {
		//关闭
		$("#addAddressDlg").dialog("close");
	},
	onGOONBtnClick : function() {
		location.href = "/Home/Index";
	},
	onCancelBtnClick : function() {
		$.myAjax({
        	historyTag : false,
        	loadEle : $("#CartDiv"),
            url: "/Order/CancelOrder/" + $("#CartDiv").attr("data"),
            data: null,
            dataType: "json",
            type: "Get",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	location.href = "/Home/Index";
            }
		});
	},
	onSubmitBtnClick : function() {
		if($("#CartDiv .addresses .sel").length == 0) {
			//没有收货地址
			alert("必须选择收货地址");
			return;
		}
		var obj = {
			OrderID : $("#CartDiv").attr("data"),
			SendType : $("#CartDiv .postType .sel").attr("data"),
			Packets : []
		};
		$.each($("#CartDiv .order .item"),function(i,n){
			var tmpItem = $(n);
			var packet = {
				PacketID : tmpItem.find(".li0 input:first").val(),
				Count : tmpItem.find(".li4 .spinner:first").val()
			}
			obj.Packets.push(packet);
		});
		
		$.myAjax({
        	historyTag : false,
        	loadEle : $("#CartDiv"),
            url: "/Order/SubmitOrder/",
            data: JSON.stringify(obj),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
    		traditional: true,
            success: function (data,status,options) {
            	if(data.content == "OK") {
            		//下一页
            		$("#CartDiv")[0].submit();
            	}
            	else {
            		alert("Error");
            	}
            }
		});
	},
	Calculate : function() {
		var itemArr = $("#CartDiv .order .item");
		var total = 0;
		$.each(itemArr,function(i,n){
			var item = $(n);
			var count = parseInt(item.find(".spinner").val());
			var univalence = parseInt(item.find(".li3").text().substring(1));
			var discount = parseInt(item.find(".li5").text().substring(1));
			var cal = count * (univalence - discount);
			total += cal;
			item.find(".li6").text(PriceFormat(cal,true));
		});
		var footer = $("#CartDiv .order .footer .li31");
		footer.children(".price").text(PriceFormat(total,true));
		footer.children(".postPrice:eq(0)").text(PriceFormat(total * 0.01,true));
		footer.children(".postPrice:eq(1)").text(PriceFormat(0,true));
		footer.children(".amount:eq(0)").text(PriceFormat(total * 1.01,true));
		footer.children(".amount:eq(1)").text(PriceFormat(total,true));
	}
};

var SubmitPage = {
	onSubmitBtnClick : function() {
		//根据所选支付方式，提交订单
		var payItem = $("#SubmitDiv .payType .content .sel");
		if(payItem.length == 0) {
			alert("请选择支付方式");
			return;
		}
		var payValue = parseInt(payItem.attr('data-payvalue'));
		switch(payValue) {
			case 0:{
				//支付宝支付
				window.open("/Order/Payment");
				break;
			}
			case 1:{
				//网银支付
				
				break;
			}
			case 2:{
				//农行支付
				
				break;
			}
		}
		
	}
};

var KeepPage = {
	Buy : function(IDArrStr,eles) {
		var MIArr = [];
		$.each(eles,function(i,n){
			var obj = {
				ele : $(n).children(".d1"),
				html : $(n).find(".d2 a").html()
			}
			MIArr.push(obj);
		});
		$.myAjax({
        	historyTag : false,
        	loadEle : null,
            url: "/Keep/Buy/" + IDArrStr,
            data: null,
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	if(data.content == "OK") {
            		$.each(MIArr,function(i,n){
            			var viewItem = n.ele;
	            		var tmpOS = viewItem.offset();
	            		var tmpW = viewItem.width();
	            		var tmpH = viewItem.height();
	            		var tmpItem = viewItem.clone(false,false).addClass('MoveItem');
	            		tmpItem.appendTo($("body:first"));
	            		tmpItem.css({
	            			top:tmpOS.top,
	            			left:tmpOS.left,
	            			width:tmpW,
	            			height:tmpH,
	            			padding:viewItem.css("padding")
	            		});
            			var cart = $("#Cart");
	            		tmpOS = cart.offset();
	            		tmpW = cart.width();
	            		tmpH = cart.height();
	        			tmpItem.animate({
		            			top:tmpOS.top - tmpH,
		            			left:tmpOS.left + 10,
		            			width:tmpW - 20,
		            			height:tmpH
		            		},{
		            		queue:false,
		            		duration:1000,
		            		complete : function() {
		            			var moveContent = $("<label>" + n.html + "</label>");
		            			tmpItem.removeAttr("style");
		            			tmpItem.append(moveContent);
		            			//购物车增加内容
		            			tmpItem.appendTo($("#plInCar"));
		            		}
		            	});
            		});
            	}
            }
		});
	},
	Delete : function(IDArrStr,eles) {
		$.myAjax({
        	historyTag : false,
        	loadEle : null,
            url: "/Keep/Delete/" + IDArrStr,
            data: null,
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	if(data.content == "OK") {
            		$.each(eles,function(i,n){
						$(n).animate({
							height:0
						}, "normal", "linear",function(){
							$(n).remove();
							$("#plInFavorit a[data='" + IDArrStr[i] + "']").remove();
		            	});
					});
            	}
            }
		});
	},
	onDeleteBtnClick : function(PID,obj) {
		var item = $(obj).parentsUntil("ul").last();
		KeepPage.Delete([PID],[item]);
	},
	onBuyBtnClick : function(PID,obj) {
		var item = $(obj).parentsUntil("ul").last();
		KeepPage.Buy([PID],[item]);
	},
	onAllDeleteBtnClick : function() {
		var tmpStr = "";
		var checkedEles = $("#favoriteList .checkItem:checked");
		if(checkedEles.length <= 0) {
			alert("请选择至少一项商品");
			return;
		}
		$.each(checkedEles,function(i,n){
			tmpStr += $(n).val() + ",";
		});
		var delItems = [];
		$.each(checkedEles,function(i,n){
			delItems.push($(n).parentsUntil("ul").last());
		});
		KeepPage.Delete(tmpStr,delItems);
	},
	onAllBuyBtnClick : function() {
		var tmpStr = "";
		var checkedEles = $("#favoriteList .checkItem:checked");
		if(checkedEles.length <= 0) {
			alert("请选择至少一项商品");
			return;
		}
		$.each(checkedEles,function(i,n){
			tmpStr += $(n).val() + ",";
		});
		var delItems = [];
		$.each(checkedEles,function(i,n){
			delItems.push($(n).parentsUntil("ul").last());
		});
		KeepPage.Buy(tmpStr,delItems);
	}
};