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
    PL_OrderEnum : {
        normal : -1,
        up : 0,
        down : 1
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
            if (item.data() == ProductList.PL_OrderEnum.up || item.data() == ProductList.PL_OrderEnum.down)
                orderStr = "0x" + item.data() + i;
        });
        var parArr = window.location.pathname.split("/");
        var CID = parArr[3];
        //ajaz获取数据，更新内容
        $.ajax({
            url: "/Product/ListByCategory/" + CID + "/" + PIndex + "/" + whereStr + "/" + orderStr,
            data: '{}',
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
                $("#plDiv").empty();
                ProductList.buildProductList(data);
            }
        });
        //$.getJSON("/Product/ListByCategory/" + CID + "/" + PIndex + "/" + whereStr + "/" + orderStr, null, function (data,textStatus,jqXHR) {
        //    $("#plDiv").empty();
        //    ProductList.buildProductList(data);
        //});
    },
    buildProductList: function (data) {

    }
};