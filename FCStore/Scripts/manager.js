$(function(){
	$("#contentTabel tbody td").live("dblclick",function(ev){
		var target = $(ev.currentTarget);
		var tmpTR = target.parentsUntil("tbody").last();
		if(target.data("editTag") === false || tmpTR.hasClass("tr_del"))
			return;
		var ele = Manager.CreateEditEle(target,target.text());
		target.empty();
		target.append(ele);
		target.data("editTag",false);
	});
	$("#contentTabel .checkAll").checkAll("#contentTabel .checkItem");
});

var Manager = {
	EditReturn : function(trEle) {
		var tdArr = trEle.children("td");
		$.each(tdArr,function(i,n){
			var ele = $(n);
			var childEle = ele.children(); 
			if(childEle != null) {
				childEle.focusout();
			}
			ele.data("editTag",true);
		});
	},
	OnRefreshBtnClick : function() {
		window.location.reload();
	},
	OnDelBtnClick : function() {
		//选择的行
		var checkedCB = $("#contentTabel .checkItem:checked");
		$.each(checkedCB,function(i,n){
			var ele = $(n);
			var checkedTR = ele.parentsUntil("tbody").last();
			checkedTR.removeClass("tr_edit");
			if(checkedTR.hasClass("tr_add")) {
				checkedTR.remove();
			}
			else {
				Manager.EditReturn(checkedTR);
				checkedTR.addClass("tr_del");
			}
		});
	},
	OnAddBtnClick : function() {
		var eles = $("#contentTabel thead td");
		var tbody = $("#contentTabel tbody");
		var trEle = $("<tr></tr>");
		$.each(eles,function(i,n){
			var ele;
			var tdEle;
			if(i != eles.length - 1) {
				var par = $(n);
				ele = Manager.CreateEditEle(par,"");
				if(par.hasClass("IDTD")) {
					tdEle = $("<td class=\'IDTD\'></td>");
				}
				else if(par.hasClass("TextTD")) {
					tdEle = $("<td class=\'TextTD\'></td>");
				}
				else if(par.hasClass("SelectionTD")) {
					tdEle = $("<td class=\'SelectionTD\'></td>");
				}
				else if(par.hasClass("BoolTagTD")) {
					tdEle = $("<td class=\'BoolTagTD\'></td>");
				}
				else if(par.hasClass("ImgTD")) {
					tdEle = $("<td class=\'ImgTD\'></td>");
				}
				else {
					tdEle = $("<td clss\'checkTD\'>" +
								"<input class=\'checkItem\' type=\'checkbox\'>" +
							"</td>");
				}
			}
			else {
				ele = $("<div class=\'icon\' ></div>");
				tdEle = $("<td class=\'tagTD\' ></td>");
			}
			ele.appendTo(tdEle);
			tdEle.appendTo(trEle);
		});
		trEle.addClass("tr_add");
		trEle.appendTo(tbody);
	},
	OnSaveBtnClick : function() {
		var trEles = $("#contentTabel tbody tr");
		$.each(trEles,function(i,n){
			Manager.EditReturn($(n));
		});
		var PerpertyArr = [];
		var columTDArr = $("#contentTabel thead td");
		$.each(columTDArr,function(i,n){
			if(i != 0 && i < columTDArr.length - 1) {
				PerpertyArr.push($(n).text());
			}
		});
		
		//获得addArr
		var addArr = [];
		var addTRArr = $("#contentTabel tr[class*='tr_add']");
		$.each(addTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.text();
			});
			addArr.push(obj);
		});
		
		//获得editArr
		var editArr = [];
		var editTRArr = $("#contentTabel tr[class*='tr_edit']");
		$.each(editTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.text();
			});
			editArr.push(obj);
		});
		
		//获得delArr
		var delArr = [];
		var delTRArr = $("#contentTabel tr[class*='tr_del']");
		$.each(delTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.text();
			});
			delArr.push(obj);
		});
		
		var tmpData = {
			AddArr : addArr,
			EditArr : editArr,
			DelArr : delArr
		};
		var ActionName = $("#contentTabel").attr("data-action");
		//保存
		$.myAjax({
        	historyTag : false,
        	loadEle : null,
            url: "/Manager/" + ActionName,
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data,status,options) {
            	
            }
		});
	},
	CreateEditEle : function(target,value) {
		var tmpTR = target.parentsUntil("tbody").last();
		var ele = $("<div></div>");
		if(target.hasClass("IDTD")) {
			
		}
		else if(target.hasClass("TextTD")) {
			ele = $("<input type='text' value='" + value + "' />");
			ele.on("focusout",function(ev1){
				var eleTarget = $(ev1.target);
				var tdEle = eleTarget.parent();
				tdEle.html(eleTarget.val());
				tdEle.data("editTag",true);
				if(!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
					tmpTR.addClass("tr_edit");
			});
		}
		else if(target.hasClass("SelectionTD")) {
			ele = $("<select></select>");
		}
		else if(target.hasClass("BoolTagTD")) {
			ele = $("<input type='checkbox' />");
		}
		else if(target.hasClass("ImgTD")) {
			ele = $("<div>" +
					"</div>");
			ele.append(target.children());
		}
		ele.width(target.width() - 6);
		return ele;
	}
}
