$("#Categorys .TCItem").bind("mouseenter", function (ev) {
    var item = $(this);
    var subDiv = item.next(".subCategory");
    subDiv.show();
    var height = subDiv.outerHeight();
    var os = subDiv.offset();
    var pos = subDiv.position();
    var oY = os.top + height - $(window).height();
    if (oY > 0) {
        subDiv.animate({ "top": pos.top - oY }, 500);
    }
}).bind("mouseleave", function (ev) {
    var item = $(this);
    var subDiv = item.next(".subCategory");
    if ($(ev.toElement)[0] != subDiv[0] && $(ev.toElement).parents('.subCategory').length == 0) {
        subDiv.hide();
        subDiv.css({ top: -1 });
    }
    else
        subDiv.one("mouseleave", function (ev) {
            $(this).hide();
            $(this).css({ top: -1 });
            subDiv.unbind();
        });
});

var ProductList = {
    onOrderClick : function(obj) {
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
    	ProductList.getProductList(PIndex);
    },
    getProductList: function (PIndex) {
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
        	loadEle : $("#plTool"),
            url: "/Product/ListByCategory/" + CID + "/" + PIndex + "/" + orderStr + "/" + whereStr,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	//更新产品列表
            	var parent = $("#plDiv");
               	parent.empty();
                $.each(data.Products,function(i,n) {
					parent.append(ProductList.buildProductView(n));
				});
				BuildPullHeightDiv(parent);
				//更新Page
				//
            }
        });
    },
    buildProductView : function(product) {
    	var item = $(
    	'<div class=\"item\">' +
            '<a href="Product/Detail/' + product.PID + '">' +
                '<div class="img">' +
                    '<img src="' + product.ImgPathArr[0] + '" />' +
                '</div>' +
                '<div class="title">' + product.Title + '</div>' +
                '<div class="marketPrice">' +
                    '市场价：<label>￥' + product.MarketPrice + '</label>' +
                '</div>' +
                '<div class="price">' +
                    '代购价：<label>￥' + product.Price + '</label>' +
                '</div>' +
            '</a>' +
        '</div>');
        return item;
    }
};