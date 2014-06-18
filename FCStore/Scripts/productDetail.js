$(function () {
    $(".bigShow").jqueryzoom({
        xzoom: 400,
        yzoom: 400,
        offset: 10,
        position: "absolute",
        preload: 1,
        lens: 1
    });
    $("#spec-list").jdMarquee({
        deriction: "left",
        width: 318,
        height: 82,
        step: 2,
        speed: 4,
        delay: 10,
        control: true,
        _front: "#spec-right",
        _back: "#spec-left"
    });
    $("#spec-list img").bind("mouseover", function () {
        var src = $(this).attr("src");
        var jqimg = $(this).attr("jqimg");
        $("#spec-n1 img").eq(0).attr({
            src: src.replace("\/n5\/", "\/n1\/"),
            jqimg: jqimg.replace("\/n5\/", "\/n0\/")
        });
        $("#spec-n1 a").attr({ href: jqimg.replace("\/n5\/", "\/n0\/") });
        $(this).css({
            "border-color": "#50A6BD",
            "border-right": "2px solid #50A6BD",
            "border-style": "solid",
            "border-width": "2px 2px 2px 2px",
            "padding": "2px 1px 2px 0px",
            "border-left": "3px solid #50A6BD"

        });
    }).bind("mouseout", function () {
        $(this).css({
            "border": "1px solid #ccc",

            "padding": "3px 2px 3px 2px"
        });
    });
})