$(function(){
	$("body").on("keypress",function(ev){
			switch(ev.keyCode) {
		    	case 45: {
		    		//关闭编辑
		    		RG.HideEditor();
		    		break;
		    	}
		    	case 61: {
		    		//打开编辑
		    		//TODO:从服务器获取当前url匹配的rgx表达式，自动加载内容
		    		
		    		RG.ShowEditor();
		    		break;
		    	}
		  }
	});
});

var RG = {
	EditEle : null,
	ShowEditor : function() {
		if(RG.EditEle == null) {
			RG.EditEle = $(
				"<div id=RG_FIX>" +
					"<ul>" +
						"<li><input type='button' id='RG_ExtentBtn' value='展开' /></li>" +
						"<li><input type='button' id='RG_SaveBtn' value='保存' /></li>" +
						"<li><input type='button' id='RG_SendBtn' value='发送' /></li>" +
						"<li><input type='button' id='RG_LoginBtn' value='登陆' /></li>" +
						"<li class='RG_p1'>标题:<div id='RG_tiledrag' class='RG_dragBtn' data-forele='#RG_tileTA' data-radiogn='tileGroup'>+</div></li>" +
						"<li class='RG_p1'>正则:" +
							"<label><input type='radio' checked='checked' name='tileGroup' value='0' />Html</label>" +
							"<label><input type='radio' name='tileGroup' value='1' />Text</label>" +
						"</li>" +
						"<li class='RG_p1'><input id='RG_tilergxTB' type='text' /></li>" +
						"<li class='RG_p1'><textarea id='RG_tileTA'></textarea></li>" +
						"<li class='RG_p1'>内容:<div id='RG_contentdrag' class='RG_dragBtn' data-forele='#RG_contentTA' data-radiogn='contentGroup'>+</div></li>" +
						"<li class='RG_p1'>正则:" +
							"<label><input type='radio' checked='checked' name='contentGroup' value='0' />Html</label>" +
							"<label><input type='radio' name='contentGroup' value='1' />Text</label>" +
						"</li>" +
						"<li class='RG_p1'><input id='RG_contentrgxTB' type='text' /></li>" +
						"<li class='RG_p1'><textarea id='RG_contentTA'></textarea></li>" +
						"<li class='RG_p1'>价钱:<div id='RG_pricedrag' class='RG_dragBtn' data-forele='#RG_priceTB' data-radiogn='priceGroup'>+</div></li>" +
						"<li class='RG_p1'>正则:" +
							"<label><input type='radio' name='priceGroup' value='0' />Html</label>" +
							"<label><input type='radio' checked='checked' name='priceGroup' value='1' />Text</label>" +
						"</li>" +
						"<li class='RG_p1'><input id='RG_pricergxTB' type='text' /></li>" +
						"<li class='RG_p1'><input id='RG_priceTB' type='text'></textarea></li>" +
						"<li class='RG_p1'>图片:<div id='RG_imgdrag' class='RG_dragBtn'>+</div></li>" +
					"</ul>" +
				"</div>");
			RG.EditEle.appendTo($("body"));
			RG.EditEle.on("mouseenter",function(ev){
				RG.EditEle.animate({"-moz-opacity":1,
					"filter":"alpha(opacity=100)",
					"opacity":1});
			}).on("mouseleave",function(ev){
				RG.EditEle.animate({"-moz-opacity":0.1,
					"filter":"alpha(opacity=10)",
					"opacity":0.1});
			});
			$("#RG_ExtentBtn").on("click",function(ev) {
				if($("#RG_ExtentBtn").attr("value") == "展开") {
					RG.EditEle.css({"width" : "100%"});
					$("#RG_ExtentBtn").attr("value","收起");
				}
				else {
					RG.EditEle.css({"width" : "250px"});
					$("#RG_ExtentBtn").attr("value","展开");
				}
			});
			$("#RG_SendBtn").on("click",function(ev) {
				//生成数据并发送
				
			});
			$(".RG_dragBtn").on("mousedown",function(ev){
				RG.HideEditor();
				RG.DragBtnEle = $(ev.currentTarget);
				$("body").on("mouseover",RG.MouseOverInBody).on("mouseup",RG.MouseUpInBody);
				return false;
			});
		}
		else {
			if($("#RG_FIX").length == 0)
				RG.EditEle.appendTo($("body"));
			RG.EditEle.show();
		}
	},
	HideEditor : function() {
		if(RG.EditEle != null) {
			RG.EditEle.hide();
		}
	},
	LabelEles : function(eleArr) {
		$.each(eleArr,function(i,n){
			$(n).css({
				width:$(n).width() - 5,
				height:$(n).height() - 5,
				border:"solid 5px gray"
			});
		});
	},
	UnlabelEle : function(eleArr) {
		$.each(eleArr,function(i,n){
			$(n).removeAttr("style");
		});
	},
	DragBtnEle : null,
	LabeledEles : null,
	MouseOverInBody : function(ev) {
		if(RG.LabeledEles != null) {
			RG.UnlabelEle(RG.LabeledEles);
		}
		RG.LabeledEles = $(ev.target);
		var parArr = RG.LabeledEles.parents("div,span");
		$.merge(RG.LabeledEles,parArr);
		RG.LabelEles(RG.LabeledEles);
	},
	MouseUpInBody : function(ev) {
		var radioVal = $("input[name=" + RG.DragBtnEle.attr("data-radiogn") + "]:checked").val();
		var labeledFirstEle = RG.LabeledEles.first();
		$(RG.DragBtnEle.attr("data-forele")).val(radioVal == 0 ? labeledFirstEle.html() : labeledFirstEle.text());
		$("body").off("mouseover",RG.MouseOverInBody).off("mouseup",RG.MouseUpInBody);
		RG.ShowEditor();
		window.setTimeout(function(){
			if(RG.LabeledEles != null) {
				RG.UnlabelEle(RG.LabeledEles);
			}
		},3000);
		
		//生成Rgx表达式
		
	}
};