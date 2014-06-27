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
		var item = $("<div id='evaluationInput' class='item' >" +
				"<div class='headDiv'><img class='headImg' /></div>" +
				"<div class='inputDiv'><textarea id='eInput' class='eInput' ></textarea></div>" +
				"<div class='btnDiv'><p>" +
				"<div class='starArea' id='evaluater'><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='star full'></div><div class='floatLeft'><div class='spinnerLeft'></div><input id='evaluationSpinner' class='spinner' type='text' value='10' /><div class='spinnerRight'></div>分</div></div>" +
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
	    item.find("#eInput").on("input propertychange", function () {
	    	var tmpStr = item.find("#eInput").val();
			var tmpLen = ProductDetail.InputMaxNum - tmpStr.length;
			if(tmpLen < 0) {
				item.find("#eInput").val(tmpStr.substring(0,ProductDetail.InputMaxNum));
				tmpLen = 0;
			}
			item.find("#ewcLB").text(tmpLen);
	    });
	    item.find("#subEvaluationBtn").on("click", function (ev) {
	        var PID = $("#PIDLB").text();
	        ProductDetail.submitEvaluation(PID, OID);
	    });
	},
	CreateEvaluation: function (panel, evaluation) {
	    var item = $("<div class='item' >" +
				"<div class='headDiv'><img class='headImg' /></div>" +
				"<div class='starDiv'>" +
					"<div class='unDiv'>" + evaluation.IDLabel + "：</div>" +
                    "<div class='starArea'></div>" +
                "</div>" +
                "<div class='lbDiv'>" +
				    "<div class='description'>" + evaluation.Description + "</div>" +
                "</div>" +
                "<div class='dataDiv'>" +
                evaluation.DataTime +
                "</div>" +
                "<div class='pullupDiv'></div>" +
			"</div>");
	    item.appendTo(panel);
	    var fsc = Math.floor(evaluation.StarCount / 2);
	    var tmpStr = "";
	    for (i = 0; i < 5; i++)
	    {
	        if (i < fsc) {
	            tmpStr += "<div class='star full'></div>";
	        }
	        else if(i == fsc && evaluation.StarCount % 2 > 0) {
	            tmpStr += "<div class='star half'></div>";
	        }
	        else {
	            tmpStr += "<div class='star empty'></div>";
	        }
	    }
	    tmpStr += "<span>" + evaluation.StarCount + "分</span>";
	    item.find(".starArea").append($(tmpStr));
	},
    getEvaluationByPID : function(panel,PID) {
    	$.myAjax({
        	historyTag : false,
        	loadEle : $("#evaluation"),
            url: "/Evaluation/getEvaluationByPID/" + PID,
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
            		var tmpEle = $("<div id='yourEvaluationArea'></div>");
            		tmpEle.appendTo(panel);
	            	if(myEvaluation != null) {
	            		//已评价
	            		ProductDetail.CreateEvaluation(tmpEle,myEvaluation);
	            	}
	            	else {
	            		ProductDetail.CreateInput(tmpEle,data.custom.buyedOID);
	            	}
            	}
            	var evaluationArr = data.content;
        		$("<div class=title><span>大家的评价：</span></div>").appendTo(panel);
        		tmpEle = $("<div id='ourEvaluationArea'></div>");
        		tmpEle.appendTo(panel);
            	if(evaluationArr.length > 0) {
	            	$.each(evaluationArr,function(i,n){
	            		if(myEvaluation == null || myEvaluation.EID != n.EID)
	            			ProductDetail.CreateEvaluation(tmpEle,n);
	            	});
            	}
            	else {
            		//没有评价
            		$("<div class='hne' ><label>还没有收到评价喔，亲</label></div>").appendTo(tmpEle);
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
        	historyTag : false,
        	loadEle : $("#evaluation"),
            url: "/Order/getSaleLogByPID/" + PID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
    		traditional: true,
            success: function (data,status,options) {
            	var chart = $('#chartDiv').highcharts();
            	if(chart) {
            		chart.series[0].setData(data.content.CountArr);
            	}
            	else {
					var minY = 0;
					var maxY = 10;
					$.each(data.content.CountArr,function(i,n){
						maxY = Math.max(maxY,n);
					});
					maxY += Math.ceil(maxY / 10) * 10;
	            	var yAxis = {
			    	    lineWidth: 1,
			            title: {
			                text: '最近一个月售出的数量'
			            },
	            		min: minY,
	            		max: maxY
	            	};
				    $("#chartDiv").highcharts({
				    	title: {
				    		text : ''
				    	},
				    	xAxis : {
				    		type: "category",
				    		categories: data.content.DTStrArr
				    	},
				    	yAxis: yAxis,
				        series: [{
				        	data: data.content.CountArr
				        }],
				        legend: {
				            enabled : false
				        },
				        tooltip: {
							formatter: function() {
								return "<div>在\"" + this.x + "\"内已卖出" + this.y + "件商品</div>";
							}  
						}
					});
            	}
            }
		});
    },
    submitEvaluation: function (PID,OID) {
        var item = $("#eInput");
        var evaluation = {
            Description: $("#eInput").val(),
            StarCount: $("#evaluationSpinner").val(),
            PID : PID,
            OID : OID
        };
        $.myAjax({
            historyTag: false,
            loadEle: $("#evaluation .item:first"),
            url: "/Evaluation/submitEvaluation/",
            data: JSON.stringify(evaluation),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            traditional: true,
            success: function (data, status, options) {
				//添加成功
            	var panel = $("#evaluationInput");
            	panel.empty();
            	ProductDetail.CreateEvaluation(panel,data.content);
            }
        });
    }
};