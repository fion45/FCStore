jQuery.fn.extend({
	elementHaveClassName : function(className) {
		return (' ' + $(this).prop('class').toLowerCase() + ' ').indexOf(' ' + className.toLowerCase() + ' ') != -1;
	},
	showLoading : function(srcStr) {
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
	},
	hideLoading : function() {
		var ele = $(this);
		if(ele.data["loadingEle"] != null) {
			ele.data["loadingEle"].hide();
		}
	},
	mySpinner : function (config) {
		var _self = $(this);
	    _self.config = {
	        upEle: null,
	        downEle: null,
	        step: 1,
	        minVal: null,
	        maxVal : null,
	        UCB : null,
	        DCB : null
	    };
	    $.extend(_self.config, config);
	    if(_self.config.upEle == null) {
	    	_self.config.upEle = _self.next();
	    }
	    if(_self.config.downEle == null) {
	    	_self.config.downEle = _self.prev();
	    }
	    if (_self.config.upEle != null) {
	        _self.config.upEle.click(function (ev) {
	        	var target = $(ev.currentTarget);
	        	var tmpTB = target.prev("input");
	            var tmpI = parseInt(tmpTB.val());
	            if (_self.config.maxVal != null) {
	                tmpI = Math.min(tmpI + _self.config.step, _self.config.maxVal);
	            }
	            else {
	                tmpI += _self.config.step;
	            }
	            tmpTB.val(tmpI);
	            if(_self.config.UCB != null) {
	            	_self.config.UCB(_self,tmpI);
	            }
	        });
	    }
	    if (_self.config.downEle != null) {
	        _self.config.downEle.click(function (ev) {
	        	var target = $(ev.currentTarget);
	        	var tmpTB = target.next("input");
	            var tmpI = parseInt(tmpTB.val());
	            if (_self.config.minVal != null) {
	                tmpI = Math.max(tmpI - _self.config.step, _self.config.minVal);
	            }
	            else {
	                tmpI -= _self.config.step;
	            }
	            tmpTB.val(tmpI);
	            if(_self.config.DCB != null) {
	            	_self.config.DCB(_self,tmpI);
	            }
	        });
	    }
	    _self.keypress(function (ev) {
	        if (ev.charCode < 48 || ev.charCode > 57)
	            return false;
	        else if (_self.val() == "" && ev.charCode == 48) {
	            return false;
	        }
	    });
	},
	checkAll : function(ele) {
	    var allEle = $(this);
	    allEle.bind("change",function(ev){
	    	var checked = allEle.prop("checked");
	    	$(ele).prop("checked",checked);
	    });
	},
	areaSelector : function (config) {
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
	},
	myEditArea : function() {
		this.area = $(this);
		this.eleArr = this.area.find("[data-meopt]");
		$.each(this.eleArr,function(i,n){
			var item = $(n);
			var par = item.parent();
			var pos = item.position();
			var option = {
				opt : item.attr("data-meopt"),
				input : item.attr("data-meit"),
				buildInputCB : item.attr("data-mebi")
			};
			var tmpOption = {};
			$.extend(true, tmpOption, $.myEditArea.ele.defaults, option);
			item.data("settings",tmpOption);
			var CEDiv =  $("<div class=MECEDiv ></div>");
			//添加修改按钮
			if(tmpOption.opt | jQuery.EMEOpt.edit == jQuery.EMEOpt.edit) {
				var editEle = $("<span class=MEEditBtn ></span>");
				CEDiv.append(editEle);
			}
			//添加删除按钮
			if(tmpOption.opt | jQuery.EMEOpt.del == jQuery.EMEOpt.del) {
				var delEle = $("<span class=MEDelBtn ></span>");
				CEDiv.append(delEle);
			}
			CEDiv.appendTo(par);
			CEDiv.css({
				left : pos.left,
				top : pos.top,
				position : "absolute"
			});
		});
	},
	myMovement : function(config) {
	    this.config = {
	    	inEle : null,
	    	width : null,
	    	height : null,
	    	removeTag : true,
	    	speed : "normal",
	    	easing : "swing",
	    	beginCB : null,
	    	endAnimationCB : function() {
//	    		var _self = $(this);
	    		_self.cloneEle.remove('MoveItem');
	    		_self.cloneEle.appendTo(_self.config.inEle);
	    		if(_self.config.endCB != null) {
	    			_self.config.endCB();
	    		}
	    	},
	    	endCB : null
	    };
	    $.extend(this.config, config);
	    var _self = this;
	    if(_self.config.beginCB != null) {
	    	_self.config.beginCB();	
	    }
		var pos = _self.offset();
		var toPos = _self.config.inEle.offset();
		_self.cloneEle = _self.clone(false);
		var tmpW = _self.config.width == null ? _self.width() : _self.config.width;
		var tmpH = _self.config.height == null ? _self.height() : _self.config.height;
		var body = $(window.document.body);
		if(_self.config.removeTag) {
			_self.remove();
		}
		_self.cloneEle.appendTo(body);
		_self.cloneEle.addClass("MoveItem");
		_self.cloneEle.css({
			top : pos.top,
			left: pos.left
		});
		_self.cloneEle.animate({
			top : toPos.top,
			left : toPos.left,
			height : tmpH,
			width : tmpW
		},"normal",_self.config.endAnimationCB);
	}
});
jQuery.extend({
	EMEOpt : {
		edit : 1,
		del : 2,
		add : 4,
		change : 3,
		all : 7
	},
	EMEInput : {
		text : 1,
		radio : 2,
		checkbox : 3,
		select : 4,
		custom : 5
	}
});
jQuery.extend({
	myEditArea : {
		init : function() {
			$(window.document).myEditArea();
		},
		ele : {
			defaults : {
				opt : jQuery.EMEOpt.change,
				input : jQuery.EMEInput.text,
				buildInputCB : null
			}
		}
	},
	myAjaxCache : [],
	myAjax : function(setting) {
		jQuery.extend({
			loadEle : null,
			historyTag : true,
			success : null,
			refreshTag : true,
			url : "/"
		},setting);
		if(setting.refreshTag || typeof($.myAjaxCache[setting.url]) == "undefined") {
			var loadEle = setting.loadEle;
			if(loadEle != null)
				loadEle.showLoading();
				var funPtr = setting.success;
				jQuery.extend(setting,{
					success : [
						funPtr,
						function(data,status,options){
							$.myAjaxCache[setting.url] = data;
							loadEle.hideLoading();
						}
					]
				});
			if (setting.historyTag && history && history.pushState) {
		    	history.pushState(null, document.title, setting.url);
		    }
			$.ajax(setting);
		}
		else {
			setting.success($.myAjaxCache[setting.url],null,null);
		}
	},
	selectOne : function(eleStr,className,childCN) {
		if(!className)
			className = "sel";
		ele = $(eleStr);
		if(childCN != null) {
			var par = ele.parent(); 
			par.on("click",childCN,function(ev){
				ele = par.children(childCN);
				ele.removeClass(className);
				$(ev.currentTarget).addClass(className);
			});
		}
		else {
			ele = $(eleStr);
			ele.on("click",function(ev){
				ele.removeClass(className);
				$(ev.currentTarget).addClass(className);
			});
		}
	}
});

var BuildPullHeightDiv = function(par) {
	par.append("<div class=\"pullupDiv\"></div>");
};

var PriceFormat = function(tmpF,tag) {
	var tmpStr = "" + tmpF;
	var charIndex = tmpStr.indexOf(".");
	if(charIndex != -1) {
		tmpStr += "00";
		tmpStr = tmpStr.substr(0,charIndex + 3);
	}
	else {
		tmpStr += ".00";
	}
	return tag ? "￥" + tmpStr : tmpStr;
};

var GetDateTimeStr = function(dt,formatStr){
	var date = dt;
	var timeValues = function(){};
	timeValues.prototype = {
		year:function(){
			if(formatStr.indexOf('yyyy')>=0)
			{
				return date.getFullYear();
			}
		    else
		    {
		     	return date.getFullYear().toString().substr(2);
		    }
		},
	    elseTime:function(val,formatVal){
	    	return formatVal>=0?(val<10?'0'+val:val):(val);
	   	},
	   	month:function(){
	    	return this.elseTime(date.getMonth ()+1,formatStr.indexOf('MM'));
	   	},
	   	day:function(){
	    	return this.elseTime(date.getDate(),formatStr.indexOf ('dd'));
	   	},
	   	hour:function(){
	    	return this.elseTime(date.getHours(),formatStr.indexOf ('hh'));
	   	},
	   	minute:function(){
	    	return this.elseTime(date.getMinutes (),formatStr.indexOf('mm'));
	   	},
	   	second:function(){
	    	return this.elseTime(date.getSeconds(),formatStr.indexOf ('ss'));
	   	}
	}
	var tV = new timeValues();
	var replaceStr = {
   		year:['yyyy','yy'],
   		month:['MM','M'],
   		day:['dd','d'],
   		hour:['hh','h'],
   		minute:['mm','m'],
   		second:['ss','s']
	};
	for(var key in replaceStr)
	{
   		formatStr = formatStr.replace(replaceStr[key][0],eval ('tV.'+key+'()'));
   		formatStr = formatStr.replace(replaceStr[key][1],eval ('tV.'+key+'()'));
	}
	return formatStr;
}

//jQuery.extend(jQuery.validator.messages, {
//	required: "必填字段",
//	remote: "请修正该字段",
//	email: "请输入正确格式的电子邮件",
//	url: "请输入合法的网址",
//	date: "请输入合法的日期",
//	dateISO: "请输入合法的日期 (ISO).",
//	number: "请输入合法的数字",
//	digits: "只能输入整数",
//	creditcard: "请输入合法的信用卡号",
//	equalTo: "请再次输入相同的值",
//	accept: "请输入拥有合法后缀名的字符串",
//	maxlength: jQuery.validator.format("请输入一个 长度最多是 {0} 的字符串"),
//	minlength: jQuery.validator.format("请输入一个 长度最少是 {0} 的字符串"),
//	rangelength: jQuery.validator.format("请输入 一个长度介于 {0} 和 {1} 之间的字符串"),
//	range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
//	max: jQuery.validator.format("请输入一个最大为{0} 的值"),
//	min: jQuery.validator.format("请输入一个最小为{0} 的值")
//});