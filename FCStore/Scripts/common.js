jQuery.fn.elementHaveClassName = function(className) {
	return $(this).prop('class').toLowerCase().indexOf(className.toLowerCase()) != -1;
};

jQuery.fn.showLoading = function(srcStr) {
	var ele = $(this);
	if(ele.data["loadingEle"] != null) {
		var loadingEle = ele.data["loadingEle"];
		if(srcStr != null)
			loadingEle.children("img").prop("src",srcStr);
		loadingEle.show();
	}
	else {
		ele.css("position","relative");
		if(srcStr == null) {
			srcStr = "/Content/themes/image/loading.gif"
		}
		var tmpH = ele.height();
		var tmpW = ele.width();
		var loadingEle = $(
				"<div class=\"loadingEle\" >" +
					"<img src=\"" + srcStr + "\" >" +
				"</div>");
		ele.append(loadingEle);
		ele.data["loadingEle"] = loadingEle;
	}
};

jQuery.fn.hideLoading = function() {
	var ele = $(this);
	if(ele.data["loadingEle"] != null) {
		ele.data["loadingEle"].hide();
	}
};

jQuery.fn.mySpinner = function (config) {
    this.config = {
        upEle: null,
        downEle: null,
        step: 1,
        minVal: null,
        maxVal : null
    };
    $.extend(this.config, config);
    var _self = this;
    if (this.config.upEle != null) {
        this.config.upEle.click(function () {
            var tmpI = parseInt(_self.val());
            if (_self.config.maxVal != null) {
                tmpI = Math.min(tmpI + _self.config.step, _self.config.maxVal);
            }
            else {
                tmpI += _self.config.step;
            }
            _self.val(tmpI);
        });
    }
    if (this.config.downEle != null) {
        this.config.downEle.click(function () {
            var tmpI = parseInt(_self.val());
            if (_self.config.minVal != null) {
                tmpI = Math.max(tmpI - _self.config.step, _self.config.minVal);
            }
            else {
                tmpI -= _self.config.step;
            }
            _self.val(tmpI);
        });
    }
    _self.keypress(function (ev) {
        if (ev.charCode < 48 || ev.charCode > 57)
            return false;
        else if (_self.val() == "" && ev.charCode == 48) {
            return false;
        }
    });
};

jQuery.fn.checkAll = function(ele) {
    var allEle = $(this);
    allEle.bind("change",function(ev){
    	var checked = allEle.prop("checked");
    	$(ele).prop("checked",checked);
    });
};

jQuery.fn.areaSelector = function (config) {
    this.config = {
    	PID : null,
    	CID : null,
    	TID : null,
        changeCB : null
    };
    $.extend(this.config, config);
    var _self = this;
    var countrySelector = _self.children(".province");
    if (countrySelector.length == 0) {
    	_self.append($("<select class=\"country\"><option>中国</option></select>"));
    }
    var provinceSelector = _self.children(".province");
    if (provinceSelector.length == 0) {
        provinceSelector = $("<select class=\"province\"></select>");
        _self.append(provinceSelector);
    }
    var citySelector = _self.children(".city");
    if (citySelector.length == 0) {
        citySelector = $("<select class=\"city\"></select>");
        _self.append(citySelector);
    }
    var countySelector = _self.children(".county");
    if (countySelector.length == 0) {
        countySelector = $("<select class=\"county\"></select>");
        _self.append(countySelector);
    }
    $.myAjax({
        historyTag: false,
        loadEle: _self,
        url: "/Common/GetProvinceArr",
        data: null,
        dataType: "json",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (data, status, options) {
        	provinceSelector.empty();
            $.each(data.ProvinceArr, function (i, n) {
                provinceSelector.append("<option value=\"" + n.PID + "\"" + (((_self.config.PID == n.PID) || (_self.config.PID == null && i == 0)) ? " selected=\"selected\"" : "") + ">" + n.PName + "</option>");
            });
            changeFun(provinceSelector.val(), _self.config.CID);
        }
    });
    var changeFun = function (PID, CID) {
        $.myAjax({
        	historyTag : false,
        	loadEle : _self,
        	url: "/Common/GetZoneList/" + PID + "/" + CID,
            data: null,
            dataType: "json",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (data, status, options) {
        		citySelector.empty();
                $.each(data.CityArr, function (i, n) {
                    citySelector.append("<option value=\"" + n.CID + "\"" + ((n.CID == CID) ? " selected=\"selected\"" : "") + ">" + n.CName + "</option>");
                });
        		countySelector.empty();
                $.each(data.TownArr, function (i, n) {
                    countySelector.append("<option value=\"" + n.TID + "\"" + (((_self.config.TID == n.TID) || (_self.config.TID == null && i == 0)) ? " selected=\"selected\"" : "") + ">" + n.TName + "</option>");
                });
                if(_self.config.changeCB != null) {
                	_self.config.changeCB();
                }
            }
		}); 
    };
    provinceSelector.on("change", function () {
        changeFun(provinceSelector.val(), -1);
    });
    citySelector.on("change", function () {
        changeFun(provinceSelector.val(), citySelector.val());
    });
    countySelector.on("change", function () {
        if(_self.config.changeCB != null) {
        	_self.config.changeCB();
        }
    });
};


jQuery.extend({
	myAjax : function(setting) {
		jQuery.extend({
			loadEle : null,
			historyTag : true,
			success : null,
			url : "/"
		},setting);
		var loadEle = setting.loadEle;
		if(loadEle != null)
			loadEle.showLoading();
			var funPtr = setting.success;
			jQuery.extend(setting,{
				success : [
					funPtr,
					function(){
						loadEle.hideLoading();
					}
				]
			});
		if (setting.historyTag && history && history.pushState) {
	    	history.pushState(null, document.title, setting.url);
	    }
		$.ajax(setting);
	},
	selectOne : function(eleStr,className,childCN) {
		if(!className)
			className = "sel";
		ele = $(eleStr);
		if(childCN != null) {
			ele.parent().on("click",childCN,function(ev){
				ele = $(eleStr);
				ele.removeClass(className);
				$(ev.currentTarget).addClass(className);
			});
		}
		else {
			ele.on("click",function(ev){
				ele = $(eleStr);
				ele.removeClass(className);
				$(ev.currentTarget).addClass(className);
			});
		}
	}
});

var BuildPullHeightDiv = function(par) {
	par.append("<div class=\"pullupDiv\"></div>");
};

var PriceFormat = function(tmpF) {
	var tmpStr = "" + tmpF;
	var charIndex = tmpStr.indexOf(".");
	if(charIndex != -1) {
		tmpStr += "00";
		tmpStr = tmpStr.substr(0,charIndex + 3);
	}
	else {
		tmpStr += ".00";
	}
	return tmpStr;
};