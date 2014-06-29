$(function () {
    QC.Login({
        btnId: "qqLoginBtn"	//插入按钮的节点id
    },
    function (reqData, opts) {
    	//判断该QQ号是否已注册
    	if(QC.Login.check()){
	    	QC.Login.getMe(function(openId, accessToken){
	    		//openId和accessToken保存到本地
	    		$.myajax({
	    			loadEle : $(window.document.body),
		        	historyTag : false,
		        	loadEle : null,
		            url: "/User/LoginByQQ/" + openId + "/" + accessToken,
		            data: null,
		            dataType: "json",
		            type: "GET",
		            contentType: "application/json;charset=utf-8",
		            success: function (data,status,options) {
		            	
		            }
				});
	    	});
	        //登录成功 
	        //根据返回数据，更换按钮显示状态方法  
	        var dom = document.getElementById(opts['btnId']);
	        var tmpStr = 
	        	"<div class=UserHead >" +
	        		"<div class=img >" +
	        			"<img src=\"{figureurl}\" />" +
	        		"</div>" +
	        		"<div class=info >" +
	        			"<div>RightGO网欢迎{nickname} | <a href=\"javascript:QC.Login.signOut();\">退出</a></div>" +
	        		"</div>" +
	    		"</div>";
	        dom && (dom.innerHTML = QC.String.format(tmpStr, {
	            figureurl: reqData.figureurl,
	            nickname: QC.String.escHTML(reqData.nickname)
	        }));
	        $("#wb_connect_btn").hide();
	        $("#userArea .VADiv").hide();
    	}
    }, function (opts) {
    	//注销成功  
        alert("已退出登录");
        $("#wb_connect_btn").show();
        $("#userArea .VADiv").show();
    });
    
    WB2.anyWhere(function (W) {
        W.widget.connectButton({
            id: "wb_connect_btn",
            type: '3,2',
            callback: {
                login: function (o) {
                    alert(o.screen_name)
                },
                logout: function () {
                    alert('logout');
                }
            }
        });
    });
	
	//类别分类选择器
    (new SidebarFollow()).init({
        element: $('#Categorys'),
        distanceToTop: 15
    });
    $("#Categorys .TCItem").bind("mouseenter", function (ev) {
        var item = $(this);
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
        var item = $(this);
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
        distanceToTop: 1,
        afterFollowCB : function(ele) {
        	if(!MainLayout.topBtnTag && $(document).scrollTop() != 0)
        		ele.show();
        },
        afterStopCB : function(ele) {
        	if(!MainLayout.topBtnTag)
        		ele.hide();
        }
    });
    
    //收藏夹
    (new SidebarFollow()).init({
        element: $('#Favorite'),
        distanceToTop: $('#Favorite').position().top
    });
    $("#plInFavorit").one("mouseenter",MainLayout.enterPLInFavorit);
    
    //购物车
    (new SidebarFollow()).init({
        element: $('#Cart'),
        distanceToTop: $('#Cart').position().top
    });
    $("#plInCar").one("mouseenter",MainLayout.enterPLInCar);
    
    //QQ在线客服
    dealy('qq_icon',3);						//2秒后显示QQ图标，默认为2秒，可自行设置
	settop('qq_icon','cs_online',150);		//设置在线客服的高度，默认150，可自行设置
	var span_q = getbyClass('cs_online','qq_num');
	setqq(span_q,['86945494','460927737','86945494','460927737','86945494']);		//填写5个QQ号码
	click_fn('qq_icon','cs_online');
    
	//品牌滚动功能
    setTimeout(function () {
        $("#Brands img:lt(" + (MainLayout.BCPerPage + MainLayout.HRBC) + ")").trigger("sporty");
        MainLayout.HRBC += MainLayout.BCPerPage;
    }, 200);
    MainLayout.StartBrandMove();
    $(".brandA").mouseenter(function () {
        var item = $(this);
        item.children(".name").hide();
        item.children(".icon").show();
    }).mouseleave(function () {
        var item = $(this);
        item.children(".name").show();
        item.children(".icon").hide();
    });
    $("#Brands img").lazyload({
        placeholder: "/Content/themes/image/BrandBlank.jpg",
        event: "sporty"
    });
    
//    $.myEditArea.init();
    
   	
});

var MainLayout = {
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
	}
};

var ProductList = {
	onBuyBtnClick : function() {
		var buyCount = $("#buyCount").val();
		var PID = $("#PIDLB").text();
		//购买按钮
		$.myAjax({
        	historyTag : false,
        	loadEle : $("#Center"),
            url: "/Product/Buy/" + PID + "/" + buyCount,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	if(data.successTag) {
            		//购买成功,添加到收藏夹里
            		var viewItem = $("#PD_View .top");
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
            		},"slow","linear",function(){
            			tmpItem.empty();
            			var moveContent = $("<div class=\"countShow\" style=\"display:" + (buyCount > 1 ? "normal" : "none") + "\">" + buyCount + "</div><div class=\"deleteBtn\" onclick=\"MainLayout.onDeleteCarItem(this)\">X</div><img src=" + $("#productImage").prop("src") + " /><label>" + $("#productTitle").html() + "</label>");
            			tmpItem.removeAttr("style");
            			tmpItem.append(moveContent);
            			//购物车增加内容
            			tmpItem.appendTo($("#plInCar"));
            		});
            	}
            }
		});
	},
	onKeepBtnClick : function() {
		//收藏按钮
		var PID = $("#PIDLB").text();
		$.myAjax({
        	historyTag : false,
        	loadEle : $("#Center"),
            url: "/Keep/Add/" + PID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	if(data.successTag) {
            		//添加到收藏夹里
            		$("#keepBtn").removeClass("btn1");
            		$("#keepBtn").addClass("btn13gray");
            		var viewItem = $("#PD_View .top");
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
            		},"slow","linear",function(){
            			tmpItem.empty();
            			var moveContent = $("<img src=" + $("#productImage").prop("src") + " /><label>" + $("#productTitle").html() + "</label>");
            			tmpItem.removeAttr("style");
            			tmpItem.append(moveContent);
            			//收藏夹增加内容
            			tmpItem.appendTo($("#plInFavorit"));
            		});
            	}
            }
		});
	},
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
    onSBMoreBtnClick : function() {
    	//显示更多的Brand
    	if($("#plBrands").data("tag")) {
	    	$("#plBrands").parent().animate({
	    		height : 102
	    	},"fast","linear");
	    	$("#plBrands").data("tag",false);
    	}
    	else {
	    	$("#plBrands").parent().animate({
	    		height : $("#plBrands").height()
	    	},"fast","linear");
	    	$("#plBrands").data("tag",true);
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