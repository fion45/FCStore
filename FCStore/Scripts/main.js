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
    getProductList: function (ev) {
        var target = $(ev.target);
        var whereStr = "";
        //获得品牌过滤
        $.each($("#plBrands .brandCB"), function (i, n) {
            var item = $(n);
            if (item.checked)
                whereStr += "0x" + item.val();
        });
        var orderStr = "";
        //获得排序
        $.each($("#plTool .orderTag"), function (i, n) {
            var item = $(n);
            if (item.data() == PL_OrderEnum.up || item.data() == PL_OrderEnum.down)
                orderStr = "0x" + item.data() + i;
        });
        //ajaz获取数据，更新内容

    }
};