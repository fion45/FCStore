$("#Categorys .TCItem").bind("mouseenter", function (ev) {
    var item = $(this);
    item.next(".subCategory").show();
}).bind("mouseleave", function (ev) {
    var item = $(this);
    var subDiv = item.next(".subCategory");
    if ($(ev.toElement)[0] != subDiv[0] && $(ev.toElement).parents('.subCategory').length == 0)
        subDiv.hide();
    else
        subDiv.one("mouseleave", function (ev) {
            $(this).hide();
            subDiv.unbind();
        });
});