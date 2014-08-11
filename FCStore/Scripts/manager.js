$(function(){
	$(".contentTabel tbody td").live("dblclick",function(ev){
		var target = $(ev.currentTarget);
		var tmpTR = target.parentsUntil("tbody").last();
		if(target.data("editTag") === false || tmpTR.hasClass("tr_del"))
			return;
		Manager.CreateEditEle(target,target.text(),target);
		target.data("editTag",false);
	});
	$(".contentTabel .checkAll").checkAll(".contentTabel .checkItem");
});

var Manager = {
	uploadTag : false,
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
		var checkedCB = $(".contentTabel .checkItem:checked");
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
		var eles = $(".contentTabel thead td");
		var tbody = $(".contentTabel tbody");
		var trEle = $("<tr></tr>");
		$.each(eles,function(i,n){
			var ele;
			var tdEle;
			if(i != eles.length - 1) {
				var par = $(n);
				if(par.hasClass("IDTD")) {
					tdEle = $("<td class=\'IDTD\'></td>");
				}
				else if(par.hasClass("TextTD")) {
					tdEle = $("<td class=\'TextTD\'></td>");
				}
				else if(par.hasClass("MultiTextTD")) {
					tdEle = $("<td class=\'MultiTextTD\'></td>");
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
				Manager.CreateEditEle(par,"",tdEle);
			}
			else {
				ele = $("<div class=\'icon\' ></div>");
				tdEle = $("<td class=\'tagTD\' ></td>");
				ele.appendTo(tdEle);
			}
			tdEle.appendTo(trEle);
		});
		trEle.addClass("tr_add");
		trEle.appendTo(tbody);
	},
	OnSaveBtnClick : function() {
		Manager.SaveFun();
	},
	SaveFun : function() {
		var ulArr = $(".contentTabel .uploadify");
		if(ulArr.length > 0) {
			if(!Manager.uploadTag) {
				$.each(ulArr,function(i,n){
					$(n).uploadify("upload","*");
				});
			}
			return;
		}
		var trEles = $(".contentTabel tbody tr");
		$.each(trEles,function(i,n){
			Manager.EditReturn($(n));
		});
		var PerpertyArr = [];
		var columTDArr = $(".contentTabel thead td");
		$.each(columTDArr,function(i,n){
			if(i != 0 && i < columTDArr.length - 1) {
				PerpertyArr.push($(n).text());
			}
		});
		
		//获得addArr
		var addArr = [];
		var addTRArr = $(".contentTabel tr[class*='tr_add']");
		$.each(addTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.attr("data-content");
			});
			addArr.push(obj);
		});
		
		//获得editArr
		var editArr = [];
		var editTRArr = $(".contentTabel tr[class*='tr_edit']");
		$.each(editTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.attr("data-content");
			});
			editArr.push(obj);
		});
		
		//获得delArr
		var delArr = [];
		var delTRArr = $(".contentTabel tr[class*='tr_del']");
		$.each(delTRArr,function(i,n){
			var trEle = $(n);
			var obj = {};
			var tdArr = trEle.children();
			$.each(PerpertyArr,function(j,m){
				var tmpTD = $(tdArr[j + 1]);
				obj[m] = tmpTD.attr("data-content");
			});
			delArr.push(obj);
		});
		
		var tmpData = {
			AddArr : addArr,
			EditArr : editArr,
			DelArr : delArr
		};
		var ActionName = $(".contentTabel").attr("data-action");
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
				window.location.reload();
            }
		});
	},
	CreateEditEle : function(target,value,par) {
		var tmpTR = target.parentsUntil("tbody").last();
		var ele = $("<div></div>");
		par.empty();
		if(target.hasClass("IDTD")) {
			
		}
		else if(target.hasClass("TextTD")) {
			ele = $("<input type='text' value='" + value + "' />");
			ele.on("focusout",function(ev1){
				var eleTarget = $(ev1.target);
				var tdEle = eleTarget.parent();
				tdEle.html(eleTarget.val());
				tdEle.attr("data-content",eleTarget.val());
				tdEle.data("editTag",true);
				if(!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
					tmpTR.addClass("tr_edit");
			});
			ele.width(target.width() - 6);
			par.append(ele);
		}
		else if(target.hasClass("MultiTextTD")) {
			ele = $("<textarea>" + value + "</textarea>");
			ele.on("focusout",function(ev1){
				var eleTarget = $(ev1.target);
				var tdEle = eleTarget.parent();
				tdEle.html(eleTarget.val());
				tdEle.attr("data-content",eleTarget.val());
				tdEle.data("editTag",true);
				if(!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
					tmpTR.addClass("tr_edit");
			});
			ele.width(target.width() - 6);
			par.append(ele);
		}
		else if(target.hasClass("SelectionTD")) {
			ele = $("<select></select>");
			ele.width(target.width() - 6);
			par.append(ele);
		}
		else if(target.hasClass("BoolTagTD")) {
			ele = $("<input type='checkbox' />");
			ele.width(target.width() - 6);
			par.append(ele);
		}
		else if(target.hasClass("ImgTD")) {
			var cellIndex = par[0].cellIndex;
			var htd = $(".contentTabel thead td:eq(" + cellIndex + ")");
			var trEle = par.parent();
			var rIndex = trEle[0].rowIndex;
			ele = $("<div>" +
						"<div id='file_upload" + rIndex + "' name='file_upload'></div>" +
					"</div>");
			ele.width(target.width() - 6);
			par.append(ele);
			$('#file_upload' + rIndex).uploadify({
	        	height : 30,
	        	width : ele.width(),
                buttonText : '文件上传',
                auto : false,
	            swf : '/Scripts/uploadify/uploadify.swf',
	            uploader : '/Manager/Upload',
	            fileTypeDesc : 'Image Files',
	            fileTypeExts : '*.jpg;*.bmp;*.png;*.gif',
	            formData : {toPath:htd.attr("data-toPath")},
	            onSelect : function(file) {
	            	$("#file_upload" + rIndex).css({
	            		"height" : "0px",
	            		"overflow" : "hidden"
	            	});
					if(!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
						tmpTR.addClass("tr_edit");
	            },
	            onUploadSuccess : function(file, data, response) {
	            	var obj = $.parseJSON(data);
	            	par.empty();
	            	par.attr("data-content",obj.imgSrc);
	            	par.append($("<img src='" + obj.imgSrc + "' />"));
					Manager.SaveFun();
	            },
	            onCancel : function(file) {
	            	$("#file_upload" + rIndex).css({
	            		"height" : "30px",
	            		"overflow" : "inherit"
	            	});
	            }
	        });
		}
	}
}
