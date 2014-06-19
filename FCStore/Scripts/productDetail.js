$(function () {
    $(".jqzoom").jqueryzoom({
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
    $("#productSaleInput .spinner").mySpinner({
        upEle: $("#productSaleInput .spinnerRight"),
        downEle: $("#productSaleInput .spinnerLeft"),
        minVal: 1
    });

    $("#productTabs").tabs({
        select: function (event, ui) {
            var panel = ui.panel;
            var PID = parseInt($("#PIDLB").html());
            switch (ui.index) {
                case 0: {
                    break;
                }
                case 1: {
                    //显示评价
                    ProductDetail.getEvaluationByPID(panel, PID);
                    break;
                }
                case 2: {
                    //显示销售记录
                    ProductDetail.getSaleLogByPID(panel, PID);
                    break;
                }
            }
        }
    });
})

var ProductDetail = {
	CreateInput : function(panel,OID) {
		$("<div class='item' >" +
				"<div class='headDiv'><img class='headImg' /></div>" +
				"<div class='inputDiv'><textarea class='eInput' ></textarea></div>" +
				"<div class='btnDiv'><p>" +
				"<div id='evaluater'><div class='star empty'></div><div class='star empty'></div><div class='star empty'></div><div class='star empty'></div><div class='star empty'></div></div>" +
				"<span><span class='coutTxt'>还能输入</span><strong class='maxNum'>140</strong><span>个字</span></span><input class='sbtn1' type='button' value='提交' />" +
				"</p></div>" +
				"<div class='pullupDiv'></div>" +
			"</div>").appendTo(panel);
	},
	CreateEvaluation : function(panel,evaluation) {
		$("<div class='item' >" +
				"<div><img class='headImg' /></div>" +
				"<div><label class='eLabel' ></label></div>" +
				"<div class='pullupDiv'></div>" +
			"</div>").appendTo(panel);
	},
    getEvaluationByPID : function(panel,PID) {
    	$.myAjax({
        	historyTag : false,
        	loadEle : $("#evaluation"),
            url: "/Product/getEvaluationByPID/" + PID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
    		traditional: true,
            success: function (data,status,options) {
            	var myEvaluation = null;
            	if(data.custom != null && data.custom.buyedOID != -1) {
            		myEvaluation = data.custom.myEvaluation;
            		$("<div class='title'>您的评价：</div>").appendTo(panel);
	            	if(myEvaluation != null) {
	            		//已评价
	            		ProductDetail.CreateEvaluation(panel,myEvaluation);
	            	}
	            	else {
	            		ProductDetail.CreateInput(panel,data.custom.buyedOID);
	            	}
            	}
            	var evaluationArr = data.content;
            	$.each(evaluationArr,function(i,n){
            		ProductDetail.CreateEvaluation(panel,n);
            	});
            }
		});
    },
    getSaleLogByPID : function(panel,PID) {
    	
    }
};