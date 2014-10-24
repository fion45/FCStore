var IndexJS = {
    SearchText : "",
    ClickSearchBtn: function (ev) {
        var pIndex = $(ev.target).attr("data-PIndex");
        IndexJS.SearchText = $("#searchTB").val();
        var tmpData = {
            searchText: IndexJS.SearchText,
            pageIndex: pIndex
        };
        $.myAjax({
            historyTag: false,
            refreshTag: true,
            loadEle: $("#searchResult"),
            url: "/Home/SearchResource/",
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                $("#searchResult").empty();
                $("#pageUl").empty();
                $.each(data.content.Items, function (i, n) {
                    $("#searchResult").append(IndexJS.BuildResourceItem(n));
                });
                $("#pageUl").append(IndexJS.BuildPage(data.content.PageIndex, data.content.PageCount));
            }
        });
    },
    BuildResourceItem: function (item) {
        return $.CreateString("<li>{0}</li><li><a href='{1}'>{2}</a></li><li>{3}</li><li>{4}</li>", [item.ResourceName, item.MagnetLink, "磁链", "下载", "播放"])
    },
    BuildPage: function (pIndex,count) {
        var result = "<li><a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + 1 + "'>|<</a></li>" +
            "<li><a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + (pIndex - 1) + "'><<</a></li>" +
            "{0}" +
            "<li><a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + (pIndex + 1) + "'>>></a></li>" +
            "<li><a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + count + "'>>|</a></li>";
        var tmpStr = "";
        var i = pIndex - 2;
        for (; i < pIndex; i++) {
            tmpStr += "<li>" + (i >= 1 ? "<a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + i + "'>" + i + "</a>" : "") + "</li>";
        }
        tmpStr += "<li class='sel'><a>" + pIndex + "</a></li>";
        i = pIndex + 1;
        for (; i < pIndex + 3; i++) {
            tmpStr += "<li>" + (i <= count ? "<a onclick='IndexJS.ClickSearchBtn({target:this});' data-PIndex='" + i + "'>" + i + "</a>" : "") + "</li>";
        }
        return $.CreateString(result, [tmpStr]);
    }
}