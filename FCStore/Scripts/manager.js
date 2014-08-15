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
};

var ProductSelect = {
	selArr : [],
    notSelArr : [],
    selTag : false,
    treeObj : null,
    updateArr : function () {
        var tmpArr = null;
        if (ProductSelect.selTag) {
            tmpArr = ProductSelect.treeObj.getCheckedNodes(true);
            ProductSelect.notSelArr = [];
            ProductSelect.selArr = [];
            $.each(tmpArr, function (i, n) {
                if (!n.isParent)
                    ProductSelect.selArr.push(n.CID);
            });
        }
        else {
            tmpArr = ProductSelect.treeObj.getCheckedNodes(false);
            ProductSelect.selArr = [];
            ProductSelect.notSelArr = [];
            $.each(tmpArr, function (i, n) {
                if (!n.isParent)
                    ProductSelect.notSelArr.push(n.CID);
            });
        }
    },
    updateProductsLST : function (insertTag) {
        var pathName = window.location.pathname;
        var result = pathName.split('/');
        var href = "";
        for (var i = 1; i < 3; i++)
        {
            href += "/" + result[i];
        }
        var tmpData = {
            Tag: result[3],
            Par: result[4],
            BeginIndex : insertTag ? $("#plDiv")[0].childElementCount : 0,
            GetCount : 50,
            OrderStr : ProductSelect.GetOrderStr(),
            WhereStr : ProductSelect.GetWhereStr()
        };
        $.myAjax({
            historyTag: false,
            loadEle: $("#plDiv"),
            url: href,
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (!insertTag) {
                    $("#plDiv").empty();
                }
                if (data.content != null) {
                    $.each(data.content, function (i, n) {
                        ProductSelect.BuildProductItem(n).appendTo($("#plDiv"));
                    });
                }
                $("#plDiv").myScrollDown({},"Goon");
            }
        });
        return false;
    },
    BuildProductItem : function (item) {
        var htmlStr =
            "<div class='item'>" +
                "<div class='img'>" +
                    "<img src='{1}' />" +
                "</div>" +
                "<div class='title' title='{2}'>{2}</div>" +
                "<div class='marketPrice p60'>" +
                    "市价：<label>￥{3}<label>" +
                "</div>" +
                "<div class='p40'>" +
                    "折扣：<label>{4}</label>" +
                "</div>" +
                "<div class='price p60'>" +
                    "现价：<label>￥{5}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "已售：<label>{6}</label>" +
                "</div>" + 
                "<div class='p60'>" +
                    "存货：<label>{7}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "浏览：<label>{8}</label>" +
                "</div>" +
            	"<div class='p1'>" +
                    "创建时间：<label>{9}</label>" +
                	"<a class='detailA' href='/Product/Detail/{0}'>详细</a>" +
                "</div>" +
            "</div>";
        htmlStr = $.CreateString(htmlStr, [
            item.PID,
            item.ImgPathArr[0],
            item.Title,
            item.MarketPrice,
            item.Discount,
            item.Price,
            item.Sale,
            item.Stock,
            item.PVCount,
            item.Date
        ]);
        return $(htmlStr);
    },
    BuildProductItemWithCB : function(item) {
    	var pItem = ProductSelect.BuildProductItem(item);
        var CBCtrl = $("<input class='productCB' type='checkbox' data-pid='" + item.PID + "' />");
        pItem.append(CBCtrl);
//        CBCtrl.attr("onpropertychange", "ProductSelect.OnSelItemCBChange");
//        CBCtrl.attr("oninput", "ProductSelect.OnSelItemCBChange");
        return pItem;
    },
    GetOrderStr : function () {
        var result = "";
        var orderTagArr = $("#orderDiv .orderTag");
        $.each(orderTagArr,function(i,n){
        	var item = $(n);
        	if(item.hasClass("ASC")) {
        		result += item.attr("data-tag") + ",ASC;";
        	}
        	else if(item.hasClass("DESC")) {
        		result += item.attr("data-tag") + ",DESC;";
        	}
        });
        result = result.substring(0,result.length - 1);
        return result;
    },
    GetWhereStr : function () {
    	var result = $("#whereInput").val();
    	var tmpStr = "";
        if (ProductSelect.selTag) {
            if (ProductSelect.selArr.length > 0) {
                tmpStr = "CID IN (" + ProductSelect.selArr.join(",") + ")";
            }
            else {
                tmpStr = "CID == -1";
            }
        }
        else {
            if (ProductSelect.notSelArr.length > 0) {
                tmpStr = "CID NOT IN (" + ProductSelect.notSelArr.join(",") + ")";
            }
        }
        if(result == "") {
    		result = tmpStr;
        }
        else if(tmpStr != "") {
    		result += " AND " + tmpStr;
        }
        return result;
    },
    OnOrderTagClick : function(obj) {
    	var orderTag = $(obj);
    	if(orderTag.hasClass("ASC")) {
    		orderTag.removeClass("ASC");
    		orderTag.addClass("DESC");
    	}
    	else if(orderTag.hasClass("DESC")) {
    		orderTag.removeClass("DESC");
    	}
    	else {
    		orderTag.addClass("ASC");
    	}
    },
    OnSelItemCBChange : function(ev) {
    	var cTag = false;
    	$.each($("#selDiv .productCB"),function(i,n){
    		if($(n).prop("checked")){
    			cTag = true;
    			return false;
    		}
    	});
    	if (cTag) {
    		if(!$("#upBtn").hasClass("sbtn1")) {
	            $("#PSMain .btnDiv .gray").removeClass("sbtngray").addClass("sbtn1");
	            $("#upBtn").on("click",ProductSelect.OnUpBtnClick);
	            $("#downBtn").on("click",ProductSelect.OnDownBtnClick);
	            $("#delBtn").on("click",ProductSelect.OnDelBtnClick);
    		}
        }
        else {
            $("#PSMain .btnDiv .gray").removeClass("sbtn1").addClass("sbtngray");
            $("#upBtn").off("click");
            $("#downBtn").off("click");
            $("#delBtn").off("click");
        }
    },
    OnSelItemClick : function(ev) {
    	var target = $(ev.target);
        var curTarget = $(ev.currentTarget);
        var CBCtrl = curTarget.children(".productCB");
        if(!target.hasClass("productCB")) {
//	        var tag = CBCtrl.prop("checked");
//	        CBCtrl.prop("checked", !tag);
//	        CBCtrl.trigger("change");
        	CBCtrl.click();
        }
    },
    OnUpBtnClick : function(ev) {
    	
    },
    OnDownBtnClick : function(ev) {
    	
    },
    OnDelBtnClick : function(ev) {
    	
    },
    OnRefreshBtnClick : function(ev) {
    	
    },
    OnSaveBtnClick : function(ev) {
    	
    }
};
