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
						"<li><input type='button' id='RG_ResetBtn' value='重置' /></li>" +
						"<li><input type='button' id='RG_SendBtn' value='发送' /></li>" +
						"<li class='RG_p1'>标题:<div id='RG_tiledrag' class='RG_dragBtn'>+</div></li>" +
						"<li class='RG_p1'><textarea id='RG_tileTA'></textarea></li>" +
						"<li class='RG_p1'>内容:<div id='RG_contentdrag' class='RG_dragBtn'>+</div></li>" +
						"<li class='RG_p1'><textarea id='RG_contentTA'></textarea></li>" +
						"<li class='RG_p1'>价钱:<div id='RG_pricedrag' class='RG_dragBtn'>+</div></li>" +
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
				RG.EditEle.animate({"-moz-opacity":0.4,
					"filter":"alpha(opacity=40)",
					"opacity":0.4});
			});
			$("#RG_ExtentBtn").on("click",function(ev) {
				if($("#RG_ExtentBtn").attr("value") == "展开") {
					RG.EditEle.attr("value","收起").css({"width" : "100%"});
				}
				else {
					RG.EditEle.attr("value","展开").css({"width" : "250px"});
				}
			});
			$("#RG_ResetBtn").on("click",function(ev) {
				
			});
			$("#RG_SendBtn").on("click",function(ev) {
				//生成数据并发送
				
			});
			$(".RG_dragBtn").on("mousedown",function(ev){
				
			}).on("mouseup",function(ev){
				
			});
		}
		else {
			RG.EditEle.show();
		}
	},
	HideEditor : function() {
		if(RG.EditEle != null) {
			RG.EditEle.hide();
		}
	}
};