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
        //ѡ�����
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

        //���addArr
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

        //���editArr
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

        //���delArr
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
        //����
        $.myAjax({
            historyTag: false,
            loadEle: null,
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
                buttonText: '�ļ��ϴ�',
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
						"<li class='title'>���������</li>" +
						"<li class='content'><input id='CRTB' type='text' value='1' /></li>" +
						"<li class='title'>���������</li>" +
						"<li class='content'><input id='CCTB' type='text' value='1' /></li>" +
						"<li class='title'>��ʾ��ʽ��</li>" +
						"<li class='content'><input id='RTTB' type='text' value='0' /></li>" +
					"</ul>");
                ele.appendTo($(window.document.body));
                $("#CSDlg").dialog({
                    autoOpen: false,
                    modal: true,
                    title: '����',
                    buttons: {
                        "ȷ��": function () {
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
        //��ʼ����ѡ��Ʒ�б�
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
        //������ѡ��Ʒ
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
            "<div class='item' data-pid='{0}'>" +
                "<div class='img'>" +
                    "<img src='{1}' />" +
                "</div>" +
                "<div class='title' title='{2}'>{2}</div>" +
                "<div class='marketPrice p60'>" +
                    "�мۣ���<label class='marketPrice'>{3}<label>" +
                "</div>" +
                "<div class='p40'>" +
                    "�ۿۣ�<label class='discount'>{4}</label>" +
                "</div>" +
                "<div class='p60'>" +
                    "�ּۣ���<label class='price'>{5}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "���ۣ�<label class='sale'>{6}</label>" +
                "</div>" +
                "<div class='p60'>" +
                    "�����<label class='stock'>{7}</label>" +
                "</div>" +
                "<div class='p40'>" +
                    "�����<label class='pvcount'>{8}</label>" +
                "</div>" +
            	"<div class='p1'>" +
                    "����ʱ�䣺<label class='date'>{9}</label>" +
                	"<a class='detailA' href='/Product/Detail/{0}'>��ϸ</a>" +
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
    BuildProductItemWithCB: function (item) {
        var tmpTag = Manager.GetParamTag(3);
        var tmpData = "";
        switch (tmpTag) {
            case 0: {
                var columnID = parseInt(Manager.GetParamTag(4));
                $.each(item.REProColLST, function (i, n) {
                    if (n.ColumnID == columnID) {
                        tmpData = "data-cr='" + n.CrossRow + "' data-cc='" + n.CrossColum + "' data-rt='" + n.RenderType + "' ";
                        return false;
                    }
                });
                break;
            }
        }
        var htmlStr =
            "<div class='item' title='������{2}���мۣ�{3}���ּۣ�{5}�����ۣ�{6}���ۿۣ�{4}����棺{7}�������{8}������ʱ�䣺{9}��'" +
            " data-pid='" + item.PID + "' " + tmpData + ">" +
                "<div class='img'>" +
                    "<img src='{1}' />" +
                "</div>" +
            	"<a class='detailA' href='/Product/Detail/{0}'>��ϸ</a>" +
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
    OnProductItemClick: function (ev) {
        var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case -1: {
                var target = $(ev.currentTarget);
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
                    height: 30,
                    width: 120,
                    buttonText: '�ļ��ϴ�',
                    auto: true,
                    swf: '/Scripts/uploadify/uploadify.swf',
                    uploader: '/Manager/Upload',
                    fileTypeDesc: 'Image Files',
                    fileTypeExts: '*.jpg;*.bmp;*.png;*.gif',
                    formData: { toPath: tmpToPath },
                    onUploadSuccess: function (file, data, response) {
                        var obj = $.parseJSON(data);
                        if (obj.Success)
                            tmpC.children("img").prop("src", obj.imgSrc);
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
    OnSaveBtnClick: function (ev) {
        //select products for column
        var item = $("#selDiv .item");
        var tmpData = {
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
    }
};

var BrandManager = {
    updateSelBrands: function () {
        $("#selDiv .item").remove();
        //��ʼ����ѡ��Ʒ�б�
        var tmpTag = Manager.GetParamTag(3);
        var GetBrandsUrl;
        switch (tmpTag) {
            case 0: {
                //select brands for column
                GetBrandsUrl = "/Brand/GetSelectBrandInColum/" + Manager.GetParamTag(4);
                break;
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
    GetBrandsInColumn: function () {
        $.myAjax({
            historyTag: false,
            loadEle: $("#BSMain .right"),
            url: "/Brand/GetBrandsInColumn/" + Manager.GetParamTag(4),
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
                if (data.content != null) {
                    $.each(data.content, function (i, n) {

                    });
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
            "<div class='item' data-bid='" + item.BID + "'>" +
                "<div class='img'>" +
                    "<img src='/Brand/{1}.jpg' />" +
                "</div>" +
            	"<a class='detailA' href='/Product/ListByBrand/{0}'>��ϸ</a>" +
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

    },
    OnBackBtnClick: function (ev) {
        window.history.back();
    },
    saveSelBrands: function () {
        //������ѡƷ��
        var SetBrandsUrl;
        var tmpData = null;
        var tmpTag = Manager.GetParamTag(3);
        switch (tmpTag) {
            case 0: {
                //select brands for column
                SetProductsUrl = "/Brand/SetSelectBrandsInColum";
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
    },
    OnDelBtnClick: function (ev) {
        var selItems = $("#selDiv .item:has(.brandCB:checked)");
        selItems.remove();
    }
};
