$(function(){
	$("#ManagerMain .right table tbody td").on("dblclick",function(ev){
		var target = $(ev.currentTarget);
		if(target.data("editTag") === false)
			return;
		var ele;
		if(target.hasClass("textTB")) {
			ele = $("<input type='text' value='" + target.text() + "' />");
			ele.on("focusout",function(ev1){
				var eleTarget = $(ev1.target);
				var tdEle = eleTarget.parent();
				tdEle.html(eleTarget.val());
				tdEle.data("editTag",true);
			});
		}
		else if(target.hasClass("selTB")) {
			ele = $("<select></select>");
		}
		else if(target.hasClass("imgTB")) {
			ele = $("<div>" +
					"</div>");
			ele.append(target.children());
		}
		else if(target.hasClass("boolTB")) {
			ele = $("<input type='checkbox' />")
		}
		else {
			return;
		}
		target.empty();
		target.append(ele);
		target.data("editTag",false);
	});
	$("#ManagerMain .right table .checkAll").checkAll($("#ManagerMain .right table .checkItem "));
});
