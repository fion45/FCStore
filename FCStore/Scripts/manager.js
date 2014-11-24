$(function () {
    $(".contentTabel tbody td").live("dblclick", function (ev) {
        var target = $(ev.currentTarget);
        var tmpTR = target.parentsUntil("tbody").last();
        if (target.data("editTag") === false || tmpTR.hasClass("tr_del"))
            return;
        Manager.CreateEditEle(target, target.text(), target);
        target.data("editTag", false);
    });
    $(".contentTabel .checkAll").checkAll(".contentTabel .checkItem");
});

var Manager = {
    uploadTag: false,
    EditReturn: function (trEle) {
        var tdArr = trEle.children("td");
        $.each(tdArr, function (i, n) {
            var ele = $(n);
            var childEle = ele.children();
            if (childEle != null) {
                childEle.focusout();
            }
            ele.data("editTag", true);
        });
    },
    OnRefreshBtnClick: function () {
        window.location.reload();
    },
    OnDelBtnClick: function () {
        //选择的行
        var checkedCB = $(".contentTabel .checkItem:checked");
        $.each(checkedCB, function (i, n) {
            var ele = $(n);
            var checkedTR = ele.parentsUntil("tbody").last();
            checkedTR.removeClass("tr_edit");
            if (checkedTR.hasClass("tr_add")) {
                checkedTR.remove();
            }
            else {
                Manager.EditReturn(checkedTR);
                checkedTR.addClass("tr_del");
            }
        });
    },
    OnAddBtnClick: function () {
        var eles = $(".contentTabel thead td");
        var tbody = $(".contentTabel tbody");
        var trEle = $("<tr></tr>");
        $.each(eles, function (i, n) {
            var ele;
            var tdEle;
            if (i != eles.length - 1) {
                var par = $(n);
                if (par.hasClass("IDTD")) {
                    tdEle = $("<td class=\'IDTD\'></td>");
                }
                else if (par.hasClass("TextTD")) {
                    tdEle = $("<td class=\'TextTD\'></td>");
                }
                else if (par.hasClass("MultiTextTD")) {
                    tdEle = $("<td class=\'MultiTextTD\'></td>");
                }
                else if (par.hasClass("SelectionTD")) {
                    tdEle = $("<td class=\'SelectionTD\'></td>");
                }
                else if (par.hasClass("BoolTagTD")) {
                    tdEle = $("<td class=\'BoolTagTD\'></td>");
                }
                else if (par.hasClass("ImgTD")) {
                    tdEle = $("<td class=\'ImgTD\'></td>");
                }
                else {
                    tdEle = $("<td clss\'checkTD\'>" +
								"<input class=\'checkItem\' type=\'checkbox\'>" +
							"</td>");
                }
                Manager.CreateEditEle(par, "", tdEle);
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
    OnSaveBtnClick: function () {
        Manager.SaveFun();
    },
    SaveFun: function () {
        var ulArr = $(".contentTabel .uploadify");
        if (ulArr.length > 0) {
            if (!Manager.uploadTag) {
                $.each(ulArr, function (i, n) {
                    $(n).uploadify("upload", "*");
                });
            }
            return;
        }
        var trEles = $(".contentTabel tbody tr");
        $.each(trEles, function (i, n) {
            Manager.EditReturn($(n));
        });
        var PerpertyArr = [];
        var columTDArr = $(".contentTabel thead td");
        $.each(columTDArr, function (i, n) {
            if (i != 0 && i < columTDArr.length - 1) {
                PerpertyArr.push($(n).text());
            }
        });

        //获得addArr
        var addArr = [];
        var addTRArr = $(".contentTabel tr[class*='tr_add']");
        $.each(addTRArr, function (i, n) {
            var trEle = $(n);
            var obj = {};
            var tdArr = trEle.children();
            $.each(PerpertyArr, function (j, m) {
                var tmpTD = $(tdArr[j + 1]);
                obj[m] = tmpTD.attr("data-content");
            });
            addArr.push(obj);
        });

        //获得editArr
        var editArr = [];
        var editTRArr = $(".contentTabel tr[class*='tr_edit']");
        $.each(editTRArr, function (i, n) {
            var trEle = $(n);
            var obj = {};
            var tdArr = trEle.children();
            $.each(PerpertyArr, function (j, m) {
                var tmpTD = $(tdArr[j + 1]);
                obj[m] = tmpTD.attr("data-content");
            });
            editArr.push(obj);
        });

        //获得delArr
        var delArr = [];
        var delTRArr = $(".contentTabel tr[class*='tr_del']");
        $.each(delTRArr, function (i, n) {
            var trEle = $(n);
            var obj = {};
            var tdArr = trEle.children();
            $.each(PerpertyArr, function (j, m) {
                var tmpTD = $(tdArr[j + 1]);
                obj[m] = tmpTD.attr("data-content");
            });
            delArr.push(obj);
        });

        var tmpData = {
            AddArr: addArr,
            EditArr: editArr,
            DelArr: delArr
        };
        var ActionName = $(".contentTabel").attr("data-action");
        //保存
        $.myAjax({
            historyTag: false,
            loadEle: $("#ManagerMain .right"),
            url: "/Manager/" + ActionName,
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                window.location.reload();
            }
        });
    },
    CreateEditEle: function (target, value, par) {
        var tmpTR = target.parentsUntil("tbody").last();
        var ele = $("<div></div>");
        par.empty();
        if (target.hasClass("IDTD")) {

        }
        else if (target.hasClass("TextTD")) {
            ele = $("<input type='text' value='" + value + "' />");
            ele.on("focusout", function (ev1) {
                var eleTarget = $(ev1.target);
                var tdEle = eleTarget.parent();
                tdEle.html(eleTarget.val());
                tdEle.attr("data-content", eleTarget.val());
                tdEle.data("editTag", true);
                if (!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
                    tmpTR.addClass("tr_edit");
            });
            ele.width(target.width() - 6);
            par.append(ele);
        }
        else if (target.hasClass("MultiTextTD")) {
            ele = $("<textarea>" + value + "</textarea>");
            ele.on("focusout", function (ev1) {
                var eleTarget = $(ev1.target);
                var tdEle = eleTarget.parent();
                tdEle.html(eleTarget.val());
                tdEle.attr("data-content", eleTarget.val());
                tdEle.data("editTag", true);
                if (!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
                    tmpTR.addClass("tr_edit");
            });
            ele.width(target.width() - 6);
            par.append(ele);
        }
        else if (target.hasClass("SelectionTD")) {
            ele = $("<select></select>");
            ele.width(target.width() - 6);
            par.append(ele);
        }
        else if (target.hasClass("BoolTagTD")) {
            ele = $("<input type='checkbox' />");
            ele.width(target.width() - 6);
            par.append(ele);
        }
        else if (target.hasClass("ImgTD")) {
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
                height: 30,
                width: ele.width(),
                buttonText: '文件上传',
                auto: false,
                swf: '/Scripts/uploadify/uploadify.swf',
                uploader: '/Manager/Upload',
                fileTypeDesc: 'Image Files',
                fileTypeExts: '*.jpg;*.bmp;*.png;*.gif',
                formData: { toPath: htd.attr("data-toPath") },
                onSelect: function (file) {
                    $("#file_upload" + rIndex).css({
                        "height": "0px",
                        "overflow": "hidden"
                    });
                    if (!tmpTR.hasClass("tr_del") && !tmpTR.hasClass("tr_add"))
                        tmpTR.addClass("tr_edit");
                },
                onUploadSuccess: function (file, data, response) {
                    var obj = $.parseJSON(data);
                    par.empty();
                    par.attr("data-content", obj.imgSrc);
                    par.append($("<img src='" + obj.imgSrc + "' />"));
                    Manager.SaveFun();
                },
                onCancel: function (file) {
                    $("#file_upload" + rIndex).css({
                        "height": "30px",
                        "overflow": "inherit"
                    });
                }
            });
        }
    },
    GetParamTag: function (parIndex) {
        var pathName = window.location.pathname;
        var parArr = pathName.split('/');
        var tmpTag = -1;
        if (parArr.length >= parIndex + 1)
            tmpTag = parseInt(parArr[parIndex]);
        return tmpTag;
    }
};

var ProductManager = {
    selArr: [],
    notSelArr: [],
    selTag: false,
    treeObj: null,
    forProduct: null,
    ShowColumSetting: function (ev) {
        var target = $(ev.target);
        if (!target.hasClass("productCB")) {
            ProductManager.forProduct = $(ev.currentTarget);
            if ($("#CSDlg").length > 0) {
                $("#CSDlg").dialog("open");
            }
            else {
                var ele = $(
		    		"<ul id=CSDlg>" +
						"<li class='title'>横跨行数：</li>" +
						"<li class='content'><input id='CRTB' type='text' value='1' /></li>" +
						"<li class='title'>横跨列数：</li>" +
						"<li class='content'><input id='CCTB' type='text' value='1' /></li>" +
						"<li class='title'>显示方式：</li>" +
						"<li class='content'><input id='RTTB' type='text' value='0' /></li>" +
					"</ul>");
                ele.appendTo($(window.document.body));
                $("#CSDlg").dialog({
                    autoOpen: false,
                    modal: true,
                    title: '设置',
                    buttons: {
                        "确定": function () {
                            ProductManager.forProduct.attr({
                                "data-cr": $("#CRTB").val(),
                                "data-cc": $("#CCTB").val(),
                                "data-rt": $("#RTTB").val()
                            });
                            $(this).dialog("close");
                        }
                    }
                });
            }
            $("#CRTB").val(ProductManager.forProduct.attr("data-cr"));
            $("#CCTB").val(ProductManager.forProduct.attr("data-cc"));
            $("#RTTB").val(ProductManager.forProduct.attr("data-rt"));
        }
    },
    updateSelProducts: function () {
        $("#selDiv .item").remove();
        //初始化已选产品列表
        var tmpTag = Manager.GetParamTag(3);
        var GetProductsUrl;
        switch (tmpTag) {
            case 0: {
                //select products for column
                GetProductsUrl = "/Product/GetSelectProductInColum/" + Manager.GetParamTag(4);
                $("#selDiv").on("dblclick", ".item", ProductManager.ShowColumSetting);
                break;
            }
        }
        $.myAjax({
            historyTag: false,
            loadEle: $("#selDiv"),
            url: GetProductsUrl,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content != null) {
                    $.each(data.content, function (i, n) {
                        ProductManager.BuildProductItemWithCB(n).appendTo($("#selDiv"));
                    });
                }
            }
        });
    },

    saveSelProducts: function () {
        //保存已选产品
        var SetProductsUrl;
        var tmpData = null;
        var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case 0: {
                //select products for column
                SetProductsUrl = "/Product/SetSelectProductInColum";
                var par = [];
                $.each($("#selDiv .item"), function (i, n) {
                    var item = $(n);
                    var CrossRow = item.attr("data-cr") ? item.attr("data-cr") : 1;
                    var CrossColum = item.attr("data-cc") ? item.attr("data-cc") : 1;
                    var RenderType = item.attr("data-rt") ? item.attr("data-rt") : 0;
                    par.push({
                        RCPID: 0,
                        ColumnID: Manager.GetParamTag(4),
                        ProductID: item.attr("data-pid"),
                        CrossRow: CrossRow,
                        CrossColum: CrossColum,
                        RenderType: RenderType
                    });
                });
                tmpData = {
                    id: Manager.GetParamTag(4),
                    Par: par
                }
                break;
            }
        }

        $.myAjax({
            historyTag: false,
            loadEle: $("#selDiv"),
            url: SetProductsUrl,
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {

            }
        });
    },
    updateArr: function () {
        var tmpArr = null;
        if (ProductManager.selTag) {
            tmpArr = ProductManager.treeObj.getCheckedNodes(true);
            ProductManager.notSelArr = [];
            ProductManager.selArr = [];
            $.each(tmpArr, function (i, n) {
                if (!n.isParent)
                    ProductManager.selArr.push(n.CID);
            });
        }
        else {
            tmpArr = ProductManager.treeObj.getCheckedNodes(false);
            ProductManager.selArr = [];
            ProductManager.notSelArr = [];
            $.each(tmpArr, function (i, n) {
                if (!n.isParent)
                    ProductManager.notSelArr.push(n.CID);
            });
        }
    },
    updateProductsLST: function (insertTag) {
        var pathName = window.location.pathname;
        var result = pathName.split('/');
        var href = "";
        for (var i = 1; i < 3; i++) {
            href += "/" + result[i];
        }
        var tmpData = {
            Tag: result[3],
            Par: result[4],
            BeginIndex: insertTag ? $("#plDiv")[0].childElementCount : 0,
            GetCount: 50,
            OrderStr: ProductManager.GetOrderStr(),
            WhereStr: ProductManager.GetWhereStr()
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
                        ProductManager.BuildProductItem(n).appendTo($("#plDiv"));
                    });
                }
                $("#plDiv").myScrollDown({}, "Goon");
            }
        });
        return false;
    },
    BuildProductItem: function (item) {
        var htmlStr =
            "<div class='item {10}' data-pid='{0}'>" +
        		"<div id='DelItemBtn'>删除</div>" +
        		"<div id='ShowItemBtn'>{11}</div>" +
                "<div class='img'>" +
                    "<img src='{1}' />" +
                "</div>" +
                "<div class='title' title='{2}'>{2}</div>" +
                "<div class='p60'>" +
                    "市价：￥<label class='marketPrice'>{3}<label>" +
                "</div>" +
                "<div class='p40'>" +
                    "折扣：<label class='discount'>{4}</label>" +
                "</div>" +
                "<div class='p60'>" +
                    "现价：￥<label class='price'>{5}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "已售：<label class='sale'>{6}</label>" +
                "</div>" +
                "<div class='p60'>" +
                    "存货：<label class='stock'>{7}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "浏览：<label class='pvcount'>{8}</label>" +
                "</div>" +
            	"<div class='p1'>" +
                    "创建时间：<label class='date'>{9}</label>" +
                	"<a class='detailA' href='/Product/EditDetail/{0}'>详细</a>" +
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
            item.Date,
            item.ShowTag ? "show" : "hide",
            item.ShowTag ? "隐藏" : "显示"
        ]);
        return $(htmlStr);
    },
    BuildProductItemWithCB: function (item) {
        var tmpTag = Manager.GetParamTag(3);
        var tmpData = "";
        switch (tmpTag) {
            case 0: {
                var columnID = parseInt(Manager.GetParamTag(4));
                if (item.REProColLST != null) {
                    $.each(item.REProColLST, function (i, n) {
                        if (n.ColumnID == columnID) {
                            tmpData = "data-cr='" + n.CrossRow + "' data-cc='" + n.CrossColum + "' data-rt='" + n.RenderType + "' ";
                            return false;
                        }
                    });
                }
                break;
            }
        }
        var htmlStr =
            "<div class='item' title='描述：{2}，市价：{3}，现价：{5}，已售：{6}，折扣：{4}，库存：{7}，浏览：{8}，创建时间：{9}，'" +
            " data-pid='" + item.PID + "' " + tmpData + ">" +
                "<div class='img'>" +
                    "<img src='{1}' />" +
                "</div>" +
            	"<a class='detailA' href='/Product/EditDetail/{0}'>详细</a>" +
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
        var pItem = $(htmlStr);
        var CBCtrl = $("<input class='productCB' type='checkbox'/>");
        pItem.append(CBCtrl);
        return pItem;
    },
    GetOrderStr: function () {
        var result = "";
        var orderTagArr = $("#orderDiv .orderTag");
        $.each(orderTagArr, function (i, n) {
            var item = $(n);
            if (item.hasClass("ASC")) {
                result += item.attr("data-tag") + ",ASC;";
            }
            else if (item.hasClass("DESC")) {
                result += item.attr("data-tag") + ",DESC;";
            }
        });
        result = result.substring(0, result.length - 1);
        return result;
    },
    GetWhereStr: function () {
        var result = $("#whereInput").val();
        var tmpStr = "";
        if (ProductManager.selTag) {
            if (ProductManager.selArr.length > 0) {
                tmpStr = "CID IN (" + ProductManager.selArr.join(",") + ")";
            }
            else {
                tmpStr = "CID == -1";
            }
        }
        else {
            if (ProductManager.notSelArr.length > 0) {
                tmpStr = "CID NOT IN (" + ProductManager.notSelArr.join(",") + ")";
            }
        }
        if (result == "") {
            result = tmpStr;
        }
        else if (tmpStr != "") {
            result += " AND " + tmpStr;
        }
        return result;
    },
    OnOrderTagClick: function (obj) {
        var orderTag = $(obj);
        if (orderTag.hasClass("ASC")) {
            orderTag.removeClass("ASC");
            orderTag.addClass("DESC");
        }
        else if (orderTag.hasClass("DESC")) {
            orderTag.removeClass("DESC");
        }
        else {
            orderTag.addClass("ASC");
        }
    },
    OnSelItemCBChange: function (ev) {
        var cTag = false;
        $.each($("#selDiv .productCB"), function (i, n) {
            if ($(n).prop("checked")) {
                cTag = true;
                return false;
            }
        });
        if (cTag) {
            if (!$("#upBtn").hasClass("sbtn1")) {
                $("#PSMain .btnDiv .gray").removeClass("sbtngray").addClass("sbtn1");
                $("#upBtn").on("click", ProductManager.OnUpBtnClick);
                $("#downBtn").on("click", ProductManager.OnDownBtnClick);
                $("#delBtn").on("click", ProductManager.OnDelSelBtnClick);
            }
        }
        else {
            $("#PSMain .btnDiv .gray").removeClass("sbtn1").addClass("sbtngray");
            $("#upBtn").off("click");
            $("#downBtn").off("click");
            $("#delBtn").off("click");
        }
    },
    AutoCalculateDiscount : function(ev) {
    	var target = $(ev.currentTarget);
    	var parent = target.parentsUntil("#selDiv").last();
    	var price = parseFloat(parent.find(".price input").val());
    	var marketPrice = parseFloat(parent.find(".marketPrice input").val());
    	parent.find(".discount input").val(parseInt(price / marketPrice * 100));
    },
    PM_UploadTag : false,
    OnProductItemClick: function (ev) {
        var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case -1: {
                var target = $(ev.currentTarget);
                target.toggleClass("sel");
                var par = $("#selDiv");
                par.empty();
                target = target.clone();
                target.appendTo(par);
                var tmpC = target.find(".title");
                var tmpStr = $.trim(tmpC.text());
                tmpC.empty();
                tmpC.append($("<textarea>" + tmpStr + "</textarea>"));
                tmpC = target.find(".marketPrice");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".discount");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".price");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".sale");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".stock");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".pvcount");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".date");
                var tmpStr = tmpC.text();
                tmpC.empty();
                tmpC.append($("<input type='textbox' value='" + tmpStr + "' />"));
                tmpC = target.find(".img");
                //		    	tmpC.css("position","relative");
                var tmpUploader = $("<div id='file_upload'></div>");
                tmpC.append(tmpUploader);
                var tmpToPath = tmpC.children("img").prop("src");
                var FIndex = tmpToPath.indexOf("//") + 2;
                tmpToPath = tmpToPath.substring(tmpToPath.indexOf("/", FIndex), tmpToPath.lastIndexOf("/") + 1);
                tmpUploader.uploadify({
                    height: 25,
                    width: 70,
                    buttonText: '替换',
                    auto: false,
                    swf: '/Scripts/uploadify/uploadify.swf',
                    uploader: '/Manager/Upload',
                    fileTypeDesc: 'Image Files',
                    fileTypeExts: '*.jpg;*.bmp;*.png;*.gif',
                    formData: { toPath: tmpToPath },
                    onUploadSuccess: function (file, data, response) {
                        var obj = $.parseJSON(data);
                        if (obj.Success)
                            tmpC.children("img").prop("src", obj.imgSrc);
                        ProductManager.SaveProductInfo(obj.imgSrc);
                    },
	                onSelect: function (file) {
	                	ProductManager.PM_UploadTag = true;
	                },
	                onCancel: function (file) {
	                	ProductManager.PM_UploadTag = false;
	                }
                });
                break;
            }
            case 0: {
                var target = $(ev.currentTarget);
                ProductManager.BuildProductItemWithCB({
                    PID: target.attr('data-pid'),
                    ImgPathArr: [target.find('img').attr('src')],
                    Title: $.trim(target.find('.title').text()),
                    MarketPrice: target.find('.marketPrice').text(),
                    Discount: target.find('.discount').text(),
                    Price: target.find('.price').text(),
                    Sale: target.find('.sale').text(),
                    Stock: target.find('.stock').text(),
                    PVCount: target.find('.pvcount').text(),
                    Date: target.find('.date').text(),
                    REProColLST: [{ ColumnID: Manager.GetParamTag(4), CrossRow: 1, CrossColum: 1, RenderType: 0 }]
                }).appendTo($("#selDiv"));
                break;
            }
        }
    },
    OnSelItemClick: function (ev) {
        var target = $(ev.target);
        var curTarget = $(ev.currentTarget);
        var CBCtrl = curTarget.children(".productCB");
        if (!target.hasClass("productCB")) {
            CBCtrl.click();
        }
    },
    OnUpBtnClick: function (ev) {
        var selItems = $("#selDiv .item:has(.productCB:checked)");
        var frontItem = null;
        var lastItem = null;
        var moveItems = [];
        $.each(selItems, function (i, n) {
            var tmpItem = $(n);
            if (frontItem == null) {
                var prevItem = tmpItem.prev();
                if (prevItem.length > 0 && (lastItem == null || prevItem.attr("data-pid") != lastItem.attr("data-pid")))
                    frontItem = prevItem;
                else
                    lastItem = tmpItem;
            }
            if (frontItem != null) {
                moveItems.push(tmpItem);
            }
        });
        if (frontItem != null) {
            frontItem.before(moveItems);
        }
    },
    OnDownBtnClick: function (ev) {
        var selItems = $.ReverseArr($("#selDiv .item:has(.productCB:checked)"));
        var backItem = null;
        var lastItem = null;
        var moveItems = [];
        $.each(selItems, function (i, n) {
            var tmpItem = $(n);
            if (backItem == null) {
                var nextItem = tmpItem.next();
                if (nextItem.length > 0 && (lastItem == null || nextItem.attr("data-pid") != lastItem.attr("data-pid")))
                    backItem = nextItem;
                else
                    lastItem = tmpItem;
            }
            if (backItem != null) {
                moveItems.unshift(tmpItem);
            }
        });
        if (backItem != null) {
            backItem.after(moveItems);
        }
    },
    OnDelSelBtnClick: function (ev) {
        var selItems = $("#selDiv .item:has(.productCB:checked)");
        selItems.remove();
    },
    OnRefreshSelBtnClick: function (ev) {
        ProductManager.updateSelProducts();
    },
    OnSaveSelBtnClick: function (ev) {
        ProductManager.saveSelProducts();
    },
    OnBackBtnClick: function (ev) {
        //    	window.location.href = "/Manager/BannerManager/"
        window.history.back();
    },
    SaveProductInfo : function(imgUrl) {
    	var item = $("#selDiv .item");
        var tmpData = {
        	Product : {
	            PID: item.attr("data-pid"),
	            CID: 0,
	            BID: 0,
	            ProductTags: [],
	            Title: item.find(".title").children().val(),
	            Chose: "",
	            Price: item.find(".price").children().val(),
	            MarketPrice: item.find(".marketPrice").children().val(),
	            Discount: item.find(".discount").children().val(),
	            Stock: item.find(".stock").children().val(),
	            Sale: item.find(".sale").children().val(),
	            ImgPath: "",
	            PVCount: item.find(".pvcount").children().val(),
	            Descript: "",
	            Date: item.find(".date").children().val(),
	            Tag: 0,
	            REProColLST: []
        	},
        	ImgPath : imgUrl
        };
        $.myAjax({
            historyTag: false,
            loadEle: $("#selDiv"),
            url: "/Product/SetSelectProductInfo",
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                    var item = $("#plDiv .item[data-pid=" + tmpData.PID + "]");
                    item.find(".title").text(tmpData.Title);
                    item.find(".price").text(tmpData.Price);
                    item.find(".marketPrice").text(tmpData.MarketPrice);
                    item.find(".discount").text(tmpData.Discount);
                    item.find(".stock").text(tmpData.Stock);
                    item.find(".sale").text(tmpData.Sale);
                    item.find(".pvcount").text(tmpData.PVCount);
                    item.find(".date").text(tmpData.Date);
                }
            }
        });
    },
    OnSaveBtnClick: function (ev) {
        //select products for column
        if(ProductManager.PM_UploadTag) {
        	$("#file_upload").uploadify("upload", "*");
        }
        else {
        	ProductManager.SaveProductInfo("");
        }
    },
    onBuildEvaluationBtnClick : function(ev) {
    	var target = $(ev.currentTarget);
    	if(!target.hasClass("sbtngray")) {
    		$("#buildDiv").empty();
    		var arrResult = [];
    		//模拟生成评价
    		$.each($("#saleLog .content"),function(i,n){
    			var ele = $(n);
    			var inputEle = ele.children("input");
    			var oldVal = parseInt(inputEle.attr("data-val"));
    			var newVal = parseInt(inputEle.val());
    			arrResult.push(0);
				if(newVal > oldVal) {
					//增加评价
					var tmpBDT = ele.attr("data-bdt");
					var tmpEDT = ele.attr("data-edt");
				    var time1 = Date.parse(tmpBDT);
				    var time2 = Date.parse(tmpEDT);
				    var secondCount = (Math.abs(time2 - time1)) / 1000;  
					for(var i=0;i<newVal - oldVal;i++) {
						var item = GenerateEvaluation(tmpBDT,secondCount);
						$("<div class='item' data-issham='1' >" + 
						    "<div class='headDiv'><img class='headImg' /></div>" +
						    "<div class='starDiv'>" +
							    "<div class='unDiv'>" + item.IDLabel + "：</div>" +
		                        "<div class='starArea'></div>" +
		                    "</div>" +
		                    "<div class='lbDiv'>" +
						        "<div class='description'>" + item.Description + "</div>" +
		                    "</div>" +
		                    "<div class='dataDiv'>" + item.DataTime + "</div>" +
		                    "<div class='pullupDiv'></div>" +
		                    "<div class='editDiv'>" +
	                            "<input class='delBtn sbtn2' type='button' value='删除'' />" +
	                        	"<input class='editBtn sbtn1' type='button' value='编辑' />" +
                    		"</div>" +
					    "</div>") .appendTo($("#buildDiv"));
					}
				}
				else if(newVal <= oldVal) {
					//减少虚假的评价
					arrResult[i] = oldVal - newVal;
				}
    		});
    		$("#evaluation").data("reduce",arrResult);
    	}
    	target.attr("class","sbtngray");
    },
    onDelEvaluationBtnClick : function(ev) {
    	var target = $(ev.currentTarget);
    	var itemEle = target.parentsUntil(".item").parent();
    	if(target.attr("data-id")) {
    		$.myAjax({
	            historyTag: false,
	            loadEle: itemEle,
	            url: "/Product/DelShamEvaluation/" + target.attr("data-id"),
	            dataType: "json",
	            type: "GET",
	            contentType: "application/json;charset=utf-8",
	            success: function (data, status, options) {
	                if (data.content == "OK") {
    					itemEle.remove();
	                }
	            }
	        });
		}
		else {
    		itemEle.remove();
		}
    },
    onEditEvaluationBtnClick : function(ev) {
    	var target = $(ev.currentTarget);
    	var itemEle = target.parentsUntil(".item").parent();
    	var desEle = itemEle.find(".description");
    	var cbEle = itemEle.find(".showCB");
    	if(target.attr("value") == "编辑") {
	    	var text = desEle.text();
	    	desEle.empty();
	    	desEle.append($("<textarea>" + text + "</textarea>"));
	    	target.attr("value","保存").attr("class","editBtn sbtn3");
	    	cbEle.attr("disabled","");
    	}
    	else {
    		if(target.attr("data-id")) {
    			var tmpData = {};
    			tmpData.EID = target.attr("data-id");
    			tmpData.IsSham = parseBool(target.attr("data-issham"));
    			tmpData.Description = tmpData.IsSham ? "" : desEle.find("textarea").val();
    			tmpData.IsShow = tmpData.IsSham ? true : target.find(".showCB").attr("checked");
	    		$.myAjax({
		            historyTag: false,
		            loadEle: itemEle,
		            url: "/Product/EditEvaluation/",
            		data: JSON.stringify(tmpData),
		            dataType: "json",
		            type: "POST",
		            contentType: "application/json;charset=utf-8",
		            success: function (data, status, options) {
		                if (data.content == "OK") {
	    					itemEle.remove();
		                }
		            }
		        });
    		}
    		else {
	    		desEle.html(desEle.children("textarea").val());
		    	target.attr("value","编辑").attr("class","editBtn sbtn1");
    		}
    	}
    },
    OnDetailDelBtnClick: function (ev) {
        var selItems = $("#selDiv .item:has(.brandCB:checked)");
        selItems.remove();
    },
	onDetailBackBtnClick : function(ev) {
		window.history.back();
	},
	onDetailRefreshBtnClick : function(ev) {
		location.reload();
	},
	onUploadFileCompleteForAddItem : function() {
		var Product = {};
		Product.PID = 0;
		Product.Title = $("#productTitle textarea").val();
		Product.EvaluationStarCount = $("#productBrand .evaluate").attr("data-val");
		Product.Price = $("#productPrice .price input").val();
		Product.MarketPrice = $("#productPrice .marketPrice input").val();
		Product.Sale = $("#productPrice .sale input").val();
		Product.Chose = $("#productChoose textarea").val();
		Product.Descript = $("#descriptContent textarea").data("xhe").getSource();
		var tmpStr = "";
		$.each($("#preview img"),function(i,n){
			var img = $(n);
			if(img.attr("src")) {
				tmpStr += img.attr("src") + ";";
			}
		});
		Product.ImgPath = tmpStr.substring(0,tmpStr.length - 1);
		
		var ShamOrderDataArr = [];
		$.each($("#buildDiv .item"),function(i,n){
			var item = $(n);
			var ShamOrderData = {};
			ShamOrderData.ProductID = Product.PID;
			ShamOrderData.IDLabel = item.find(".unDiv").text();
			ShamOrderData.IDLabel = ShamOrderData.IDLabel.substring(0,ShamOrderData.IDLabel.length - 1);
			if(item.find(".description text").length > 0) {
				ShamOrderData.Description = item.find(".description text").val();
			}
			else {
				ShamOrderData.Description = item.find(".description").text();
			}
			ShamOrderData.DateTime = item.find(".dataDiv").text();
			ShamOrderDataArr.push(ShamOrderData);
		});
		var tmpData = {
			Product : Product,
			ShamOrderDataArr : ShamOrderDataArr
		};
		$.myAjax({
            historyTag: false,
            loadEle: $("#PD_View"),
            url: "/Product/SaveAddDetail/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
					location.reload();
                }
            }
        });
	},
	onUploadFileComplete : function() {
		var Product = {};
		Product.PID = Manager.GetParamTag(3);
		Product.Title = $("#productTitle textarea").val();
		Product.EvaluationStarCount = $("#productBrand .evaluate").attr("data-val");
		Product.Price = $("#productPrice .price input").val();
		Product.MarketPrice = $("#productPrice .marketPrice input").val();
		Product.Sale = $("#productPrice .sale input").val();
		Product.Chose = $("#productChoose textarea").val();
		Product.Descript = $("#descriptContent textarea").data("xhe").getSource();
		var tmpStr = "";
		$.each($("#preview img"),function(i,n){
			var img = $(n);
			if(img.attr("src")) {
				tmpStr += img.attr("src") + ";";
			}
		});
		Product.ImgPath = tmpStr.substring(0,tmpStr.length - 1);
		
		var ShamOrderDataArr = [];
		$.each($("#buildDiv .item"),function(i,n){
			var item = $(n);
			var ShamOrderData = {};
			ShamOrderData.ProductID = Product.PID;
			ShamOrderData.IDLabel = item.find(".unDiv").text();
			ShamOrderData.IDLabel = ShamOrderData.IDLabel.substring(0,ShamOrderData.IDLabel.length - 1);
			if(item.find(".description text").length > 0) {
				ShamOrderData.Description = item.find(".description text").val();
			}
			else {
				ShamOrderData.Description = item.find(".description").text();
			}
			ShamOrderData.DateTime = item.find(".dataDiv").text();
			ShamOrderDataArr.push(ShamOrderData);
		});
		var tmpData = {
			Product : Product,
			ShamOrderDataArr : ShamOrderDataArr
		};
		$.myAjax({
            historyTag: false,
            loadEle: $("#PD_View"),
            url: "/Product/SaveEditDetail/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
					location.reload();
                }
            }
        });
	},
	onDetailPreviewBtnClick : function(ev) {
		
	},
	onDetailStarClick : function(ev) {
		$("#productBrand .evaluate .star").removeClass("half").removeClass("full");
		var target = $(ev.currentTarget);
		var offset = target.offset();
		var width = target.width();
		var prevArr = target.prevAll();
		prevArr.addClass("full");
		var tmpVal = prevArr.length * 2;
		
		if(ev.clientX < offset.left + width / 2) {
			tmpVal += 1;
			target.addClass("half");
		}
		else {
			tmpVal += 2;
			target.addClass("full");
		}
		$("#productBrand .evaluate").attr("data-val",tmpVal);
	},
	clickchktag : false,
	onCategoryChkClick : function(ev) {
		if(!ProductManager.clickchktag) {
			ProductManager.clickchktag = true;
			$(".categoryLST").on("mouseleave",ProductManager.onCategoryMouseLeave);
		}
	},
	onCategoryMouseLeave : function(ev) {
		$("#plDiv").empty();
		ProductManager.updateProductsLST(false);
		$(".categoryLST").off("mouseleave",ProductManager.onCategoryMouseLeave);
		ProductManager.clickchktag = false;
	},
	OnAddProductBtnClick : function(ev) {
		window.location = "/Product/AddItemDetail/";
	},
	OnShowProductBtnClick : function(ev) {
		var tmpData = {
			PIDArr : [],
			ShowTag : 1
		};
		$.each($("#plDiv .sel"),function(i,n){
			tmpData.PIDArr.push($(n).attr("data-pid"));
		});
		$.myAjax({
            historyTag: false,
            loadEle: $("#plDiv"),
            url: "/Product/ShowProductsByPIDArr/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                	$("#plDiv .sel").removeClass("hide");
                	$("#plDiv .sel #ShowItemBtn").text("隐藏");
                }
            }
        });
	},
	OnHideProductBtnClick : function(ev) {
		var tmpData = {
			PIDArr : [],
			ShowTag : 0
		};
		$.each($("#plDiv .sel"),function(i,n){
			tmpData.PIDArr.push($(n).attr("data-pid"));
		});
		$.myAjax({
            historyTag: false,
            loadEle: $("#plDiv"),
            url: "/Product/ShowProductsByPIDArr/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                	$("#plDiv .sel").addClass("hide");
                	$("#plDiv .sel #ShowItemBtn").text("显示");
                }
            }
        });
	},
	OnShowProductItemBtnClick : function(ev) {
		var target = $(ev.currentTarget);
		var par = target.parent();
		var ShowTag = target.text() == "隐藏" ? 0 : 1;
		var tmpData = {
			PIDArr : [par.attr("data-pid")],
			ShowTag : ShowTag
		};
		$.myAjax({
            historyTag: false,
            loadEle: par,
            url: "/Product/ShowProductsByPIDArr/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                	if(ShowTag == 1) {
                		par.removeClass("hide");
                		target.text("隐藏");
                	}
                	else {
                		par.addClass("hide");
                		target.text("显示");
                	}
                }
            }
        });
        return false;
	},
	OnDelProductItemBtnClick : function(ev) {
		var target = $(ev.currentTarget);
		var par = target.parent();
		var tmpData = {
			PIDArr : [par.attr("data-pid")]
		};
		$.myAjax({
            historyTag: false,
            loadEle: par,
            url: "/Product/DelProductsByPIDArr/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                	$("#plDiv .sel").remove();
                }
            }
        });
        return false;
	},
	OnDelProductBtnClick : function(ev) {
		var tmpData = {
			PIDArr : []
		};
		$.each($("#plDiv .sel"),function(i,n){
			tmpData.PIDArr.push($(n).attr("data-pid"));
		});
		$.myAjax({
            historyTag: false,
            loadEle: $("#plDiv"),
            url: "/Product/DelProductsByPIDArr/",
    		data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content == "OK") {
                	$("#plDiv .sel").remove();
                }
            }
        });
	},
	OnImportProductBtnClick : function(ev) {
		//导入商品XML文件
		
	},
	OnExportProductBtnClick : function(ev) {
		//导出商品XML文件
		var PIDArrStr = "";
		$.each($("#plDiv .sel"),function(i,n){
			PIDArrStr += $(n).attr("data-pid") + ",";
		});
		PIDArrStr = PIDArrStr.substring(0,PIDArrStr.length - 1);
		var newWindow = OpenWindowWithPost("/Product/BuildProductsXML/",null,["PIDArrStr"],[PIDArrStr]);
		delete newWindow;
	}
};

var BrandManager = {
    updateSelBrands: function () {
        $("#selDiv .item").remove();
        //初始化已选产品列表
        var tmpTag = Manager.GetParamTag(3);
        var GetBrandsUrl;
        switch (tmpTag) {
            case 0: {
                //select brands for column
                GetBrandsUrl = "/Brand/GetSelectBrandInColum/" + Manager.GetParamTag(4);
                break;
            }
            case 1: {
            	//select product's brand
            	GetBrandsUrl = "/Brand/GetProductBrand/" + Manager.GetParamTag(4);
            }
        }
        $.myAjax({
            historyTag: false,
            loadEle: $("#selDiv"),
            url: GetBrandsUrl,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content != null) {
                    $.each(data.content, function (i, n) {
                        BrandManager.BuildBrandItemWithCB(n).appendTo($("#selDiv"));
                    });
                }
            }
        });
    },
    GetBrandsInColumn: function (CID) {
        $.myAjax({
            historyTag: false,
            loadEle: $("#BSMain .right"),
            url: "/Brand/GetBrandsInColumn/" + CID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content != null) {
                	var tmpPar = $("#BSMain .right");
            		var tmpStr = "<div class='charDiv'>项目所属</div>"
                    $.each(data.content, function (i, n) {
                    	tmpStr += "<div class='brandItem' data-bid='" + n.BID + "' data-tag='" + n.Tag + "'>" +
	                        "<img src='/Brand/" + n.Tag + ".jpg' />" +
	                        "<div class='title'>" + n.Name2 + "</div>" +
	                        "<a href='Product/ListByBrand/" + n.BID + "'>详情</a>" +
	                    "</div>";
                    });
                    $(tmpStr).prependTo(tmpPar);
                }
            }
        });
    },
    GetProductBrand : function(PID) {
    	$.myAjax({
            historyTag: false,
            loadEle: $("#BSMain .right"),
            url: "/Brand/GetProductBrand/" + PID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content != null) {
                	var tmpPar = $("#BSMain .right");
            		var tmpStr = "<div class='charDiv'>旧值</div>"
                    $.each(data.content, function (i, n) {
                    	tmpStr += "<div class='brandItem' data-bid='" + n.BID + "' data-tag='" + n.Tag + "'>" +
	                        "<img src='/Brand/" + n.Tag + ".jpg' />" +
	                        "<div class='title'>" + n.Name2 + "</div>" +
	                        "<a href='Product/ListByBrand/" + n.BID + "'>详情</a>" +
	                    "</div>";
                    });
                    $(tmpStr).prependTo(tmpPar);
                }
            }
        });
    },
    BuildBrandItemWithCB: function (item) {
        var tmpTag = Manager.GetParamTag(3);
        var tmpData = "";
        switch (tmpTag) {
            case 0: {

                break;
            }
        }
        var htmlStr =
            "<div class='item' data-bid='" + item.BID + "' data-tag='{1}' >" +
                "<div class='img'>" +
                    "<img src='/Brand/{1}.jpg' />" +
                "</div>" +
            	"<a class='detailA' href='/Product/ListByBrand/{0}'>详细</a>" +
            "</div>";
        htmlStr = $.CreateString(htmlStr, [
            item.BID,
            item.Tag
        ]);
        var pItem = $(htmlStr);
        var CBCtrl = $("<input class='brandCB' type='checkbox'/>");
        pItem.append(CBCtrl);
        return pItem;
    },
    OnBrandItemClick: function (ev) {
		var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case -1: {
                break;
            }
            case 0: {
                var target = $(ev.currentTarget);
                BrandManager.BuildBrandItemWithCB({
                	BID : target.attr("data-bid"),
                	Tag : target.attr("data-tag")
                }).appendTo($("#selDiv"));
                break;
            }
            case 1: {
            	var target = $(ev.currentTarget);
            	$("#selDiv").empty();
                BrandManager.BuildBrandItemWithCB({
                	BID : target.attr("data-bid"),
                	Tag : target.attr("data-tag")
                }).appendTo($("#selDiv"));
            	break;
            }
        }
    },
    OnBackBtnClick: function (ev) {
        window.history.back();
    },
    saveSelBrands: function () {
        //保存已选品牌
        var SetBrandsUrl;
        var tmpData = null;
        var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case 0: {
                //select brands for column
                SetBrandsUrl = "/Brand/SetSelectBrandsInColum";
                var par = [];
                $.each($("#selDiv .item"), function (i, n) {
                    var item = $(n);
                    par.push({
                        RCBID: 0,
                        ColumnID: Manager.GetParamTag(4),
                        BrandID: item.attr("data-bid")
                    });
                });
                tmpData = {
                    id: Manager.GetParamTag(4),
                    Par: par
                }
                break;
            }
            case 1: {
            	SetBrandsUrl = "/Brand/SetProductBrand";
                var par = $("#selDiv .item").first().attr("data-bid");
                tmpData = {
                    PID: Manager.GetParamTag(4),
                    BID: par
                }
                break;
            }
        }

        $.myAjax({
            historyTag: false,
            loadEle: $("#selDiv"),
            url: SetBrandsUrl,
            data: JSON.stringify(tmpData),
            dataType: "json",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
				if (data.content == "OK") {
					
                }
            }
        });
    },
    OnSaveSelBtnClick: function (ev) {
        BrandManager.saveSelBrands();
    },
    OnRefreshSelBtnClick: function (ev) {
        BrandManager.updateSelBrands();
    },
    OnSelItemCBChange: function (ev) {
        var cTag = false;
        $.each($("#selDiv .brandCB"), function (i, n) {
            if ($(n).prop("checked")) {
                cTag = true;
                return false;
            }
        });
        if (cTag) {
            if (!$("#upBtn").hasClass("sbtn1")) {
                $("#BSMain .btnDiv .gray").removeClass("sbtngray").addClass("sbtn1");
                $("#upBtn").on("click", BrandManager.OnUpBtnClick);
                $("#downBtn").on("click", BrandManager.OnDownBtnClick);
                $("#delBtn").on("click", BrandManager.OnDelBtnClick);
            }
        }
        else {
            $("#BSMain .btnDiv .gray").removeClass("sbtn1").addClass("sbtngray");
            $("#upBtn").off("click");
            $("#downBtn").off("click");
            $("#delBtn").off("click");
        }
    },
    OnSelItemClick: function (ev) {
        var target = $(ev.target);
        var curTarget = $(ev.currentTarget);
        var CBCtrl = curTarget.children(".brandCB");
        if (!target.hasClass("brandCB")) {
            CBCtrl.click();
        }
    },
    OnUpBtnClick: function (ev) {
        var selItems = $("#selDiv .item:has(.brandCB:checked)");
        var frontItem = null;
        var lastItem = null;
        var moveItems = [];
        $.each(selItems, function (i, n) {
            var tmpItem = $(n);
            if (frontItem == null) {
                var prevItem = tmpItem.prev();
                if (prevItem.length > 0 && (lastItem == null || prevItem.attr("data-pid") != lastItem.attr("data-pid")))
                    frontItem = prevItem;
                else
                    lastItem = tmpItem;
            }
            if (frontItem != null) {
                moveItems.push(tmpItem);
            }
        });
        if (frontItem != null) {
            frontItem.before(moveItems);
        }
    },
    OnDownBtnClick: function (ev) {
        var selItems = $.ReverseArr($("#selDiv .item:has(.brandCB:checked)"));
        var backItem = null;
        var lastItem = null;
        var moveItems = [];
        $.each(selItems, function (i, n) {
            var tmpItem = $(n);
            if (backItem == null) {
                var nextItem = tmpItem.next();
                if (nextItem.length > 0 && (lastItem == null || nextItem.attr("data-pid") != lastItem.attr("data-pid")))
                    backItem = nextItem;
                else
                    lastItem = tmpItem;
            }
            if (backItem != null) {
                moveItems.unshift(tmpItem);
            }
        });
        if (backItem != null) {
            backItem.after(moveItems);
        }
    }
};
