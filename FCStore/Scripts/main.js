$(function () {
    (new SidebarFollow()).init({
        element: $('#Categorys'),
        distanceToTop: 15
    });
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
    (new SidebarFollow()).init({
        element: $('#Favorite'),
        distanceToTop: $('#Favorite').position().top
    });
    (new SidebarFollow()).init({
        element: $('#Cart'),
        distanceToTop: $('#Cart').position().top
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
    $("#productSaleInput .spinner").mySpinner({
        upEle: $("#productSaleInput .spinnerRight"),
        downEle: $("#productSaleInput .spinnerLeft"),
        minVal : 1
    });
    $(".brandA").mouseenter(function () {
        var item = $(this);
        item.children(".name").hide();
        item.children(".icon").show();
    }).mouseleave(function () {
        var item = $(this);
        item.children(".name").show();
        item.children(".icon").hide();
    });

    $(".choseItemPar .choseItem").click(function () {
        var item = $(this);
        var par = item.parent();
        var selItem = par.children(".sel").removeClass("sel");
        item.addClass("sel");
    });

    $("#tabs").tabs();
    $("#plInCar").one("mouseenter",MainLayout.enterPLInCar);
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
	enterPLInCar : function() {
		var target = $("#plInCar");
    	var ICount = target.children(".MoveItem").length;
    	var tmpH = ICount * 77;
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
    }
};