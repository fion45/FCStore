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