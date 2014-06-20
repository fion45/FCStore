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
            var panel = $(ui.panel);
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
	InputMaxNum : 140,
	SetEvaluatorScore : function(spinner,value) {
		spinner.val(value);
		var tmpV = Math.floor(value / 2);
		$("#evaluater .star:lt(" + tmpV + ")").prop("className","star full");
		if(value % 2 > 0) {			
			$("#evaluater .star:eq(" + tmpV + ")").prop("className","star half");
			tmpV += 1;
		}
		$("#evaluater .star:gt(" + (tmpV - 1) + ")").prop("className","star empty");
	},
	CreateInput : function(panel,OID) {
		var item = $("<div class='item' >" +
				"<div class='headDiv'><img class='headImg' /></div>" +
				"<div class='inputDiv'><textarea id='eInput' class='eInput' ></textarea></div>" +
				"<div class='btnDiv'><p>" +
				"<div id='evaluater'><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='floatLeft'><div class='spinnerLeft'></div><input id='evaluationSpinner' class='spinner' type='text' value='10' /><div class='spinnerRight'></div>分</div></div>" +
				"<span><span class='coutTxt'>还能输入</span><strong id='ewcLB' class='maxNum'>" + ProductDetail.InputMaxNum + "</strong><span>个字</span></span><input id='subEvaluationBtn' class='sbtn1' type='button' value='提交' />" +
				"</p></div>" +
				"<div class='pullupDiv'></div>" +
			"</div>");
		item.appendTo(panel);
		item.find("#evaluationSpinner").mySpinner({
	        minVal: 1,
	        maxVal: 10,
	        UCB: ProductDetail.SetEvaluatorScore,
	        DCB: ProductDetail.SetEvaluatorScore
	    });
	    item.find("#evaluater .star").on("click",function(ev){
	    	var target = $(ev.target);
	    	var ox = ev.offsetX;
	    	var tmpVal = target.prevAll(".star").length * 2;
	    	if(ox > target.width() / 2) {
	    		tmpVal += 2;
	    	}
	    	else {
	    		tmpVal += 1;
	    	}
	    	ProductDetail.SetEvaluatorScore($("#evaluationSpinner"),tmpVal);
	    });
	    item.find("#eInput").on("change",function(){
	    	var tmpStr = item.find("#eInput").val();
			var tmpLen = ProductDetail.InputMaxNum - tmpStr.length;
			if(tmpLen < 0) {
				item.find("#eInput").val(tmpStr.substring(0,ProductDetail.InputMaxNum));
				tmpLen = 0;
			}
			item.find("#ewcLB").text(tmpLen);
	    });
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
        		panel.empty();
            	var myEvaluation = null;
            	if(data.custom != null && data.custom.buyedOID != -1) {
            		myEvaluation = data.custom.myEvaluation;
            		$("<div class='title'><span>您的评价：</span></div>").appendTo(panel);
	            	if(myEvaluation != null) {
	            		//已评价
	            		ProductDetail.CreateEvaluation(panel,myEvaluation);
	            	}
	            	else {
	            		ProductDetail.CreateInput(panel,data.custom.buyedOID);
	            	}
            	}
            	var evaluationArr = data.content;
        		$("<div class=title><span>大家的评价：</span></div>").appendTo(panel);
            	if(evaluationArr.length > 0) {
	            	$.each(evaluationArr,function(i,n){
	            		ProductDetail.CreateEvaluation(panel,n);
	            	});
            	}
            	else {
            		//没有评价
            		$("<div class='hne' ><label>还没有收到评价喔，亲</label></div>").appendTo(panel);
            	}
            }
		});
    },
    CreateSaleLog : function(panel,order) {
    	$("<div class='item' >" +
    			"<div></div>" +
    			"<div></div>" +
    			"<div>购买时间：" + order.OrderDate + "收货时间：" + order.CompleteDate + "</div>" +
			"</div>").appendTo(panel);
    },
    getSaleLogByPID : function(panel,PID) {
    	$.myAjax({
        	historyTag : true,
        	loadEle : $("#evaluation"),
            url: "/Product/getSaleLogByPID/" + PID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
    		traditional: true,
            success: function (data,status,options) {
            	$(data.content,function(i,n){
            		ProductDetail.CreateSaleLog(panel,n);
            	});
            }
		});
    }
};