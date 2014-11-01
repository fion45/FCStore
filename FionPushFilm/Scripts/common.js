﻿jQuery.fn.extend({
	elementHaveClassName : function(className) {
		return (' ' + $(this).prop('class').toLowerCase() + ' ').indexOf(' ' + className.toLowerCase() + ' ') != -1;
	},
	showLoading : function(srcStr) {
		var ele = $(this);
		if(ele.data("loadingEle") != null) {
			var loadingEle = ele.data("loadingEle");
			if(srcStr != null)
				loadingEle.children("img").prop("src",srcStr);
			loadingEle.show();
		}
		else {
			if(ele.css("position") != "absolute")
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
			ele.data("loadingEle",loadingEle);
		}
	},
	hideLoading : function() {
		var ele = $(this);
		if(ele.data("loadingEle") != null) {
			ele.data("loadingEle").hide();
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
	checkAll : function(ele,childCN) {
	    var allEle = $(this);
	    if(childCN != null) {
	    	allEle.on("change",function(ev){
		    	var checked = allEle.prop("checked");
		    	$(childCN).prop("checked",checked);
		    	$(childCN).trigger("change");
		    });
	    }
	    else {
		    allEle.on("change",function(ev){
		    	var checked = allEle.prop("checked");
		    	$(ele).prop("checked",checked);
		    	$(childCN).trigger("change");
		    });
	    }
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
	            $.each(data.custom.ProvinceArr, function (i, n) {
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
	                $.each(data.custom.CityArr, function (i, n) {
	                    citySelector.append("<option value=\"" + n.CID + "\"" + ((n.CID == CID) ? " selected=\"selected\"" : "") + ">" + n.CName + "</option>");
	                });
	        		countySelector.empty();
	                $.each(data.custom.TownArr, function (i, n) {
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
	},
	myScrollDown : function(config,funName) {
		if(funName != null) {
			this.data("MYSCROLLDOWN")[funName]();
			return;
		}
		this.config = {
	    	downDis : 50,
	    	doingCB : null,
	    	doingPar : null
	    };
	    $.extend(this.config, config);
	    var _self = this;
	    _self.data("MYSCROLLDOWN",this);
	    _self.Activate = function(ev) {
	    	if(_self.scrollTop() + _self.config.downDis + _self.height() >= _self.prop("scrollHeight")) {
	    		_self.off("scroll");
	    		if(_self.config.doingCB(_self.config.doingPar))
	    			_self.on("scroll", _self.Activate);
	    	}
	    };
	    _self.Goon = function() {
	    	_self.on("scroll", _self.Activate);
	    }
	    _self.on("scroll", _self.Activate);
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
		var tmpSetting = {
			loadEle : null,
			historyTag : false,
			success : null,
			refreshTag : true,
			url : "/"
		};
		jQuery.extend(tmpSetting,setting);
		if(tmpSetting.refreshTag || typeof($.myAjaxCache[tmpSetting.url]) == "undefined") {
			var loadEle = tmpSetting.loadEle;
			if(loadEle != null)
				loadEle.showLoading();
				var funPtr = tmpSetting.success;
				jQuery.extend(tmpSetting,{
					success : [
						funPtr,
						function(data,status,options){
							$.myAjaxCache[tmpSetting.url] = data;
							loadEle.hideLoading();
						}
					]
				});
			if (tmpSetting.historyTag && history && history.pushState) {
		    	history.pushState(null, document.title, tmpSetting.url);
		    }
			$.ajax(tmpSetting);
		}
		else {
			tmpSetting.success($.myAjaxCache[tmpSetting.url],null,null);
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
			ele.data("eles",ele);
			ele.on("click",function(ev){
				var tmpEle = $(ev.currentTarget);
				tmpEle.data("eles").removeClass(className);
				tmpEle.addClass(className);
			});
		}
	},
	CreateString : function(str,replaces){
        var re = /\{(\d+)\}/g;
        var temp = str.replace(re,function($0,$1,$2){
            return replaces[$1];
        });
        return temp;
    },
    ReverseArr : function(arr) {
    	var result = [];
    	for(var i = arr.length - 1; i >= 0; i--) {
    		result.push(arr[i]);
    	}
    	return result;
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

var AddSeconds = function(temp,addSeconds) {
	if(temp.getDate != null){
		temp = this.GetDateTimeStr(temp,'yyyy-M-d h:m:s');
	}
    var dayCountArray=[31,28,31,30,31,30,31,31,30,31,30,31];
    var tempArray= temp.split(':');
    var tempS=parseInt(tempArray[2]);
    var tempM=parseInt(tempArray[1]);
    tempArray=tempArray[0].split(' ');
    var tempH=parseInt(tempArray[1]);
    tempArray=tempArray[0].split('-');
    var tempD=parseInt(tempArray[2]);
    var tempMM=parseInt(tempArray[1]);
    var tempY=parseInt(tempArray[0]);
    //判断是不是闰年
    if((tempY % 4 == 0 && tempY % 100 != 0) || tempY % 400 == 0)
    {
    	dayCountArray[1] = 29;
    }
    tempS+=addSeconds;
    var addMinute=Math.floor(tempS/60);
    tempS%=60;
    if(tempS<0)
    {
        tempS+=60;
    }
    tempS = '0' + tempS;
    tempS = tempS.substring(tempS.length - 2, tempS.length);
    tempM+=addMinute;
    var addHour=Math.floor(tempM/60);
    tempM%=60;
    if(tempM<0)
    {
        tempM+=60;
    }
    tempM = '0' + tempM;
    tempM = tempM.substring(tempM.length - 2, tempM.length);
    tempH+=addHour;
    var addDay=Math.floor(tempH/24);
    tempH%=24;
    if(tempH<0)
    {
        tempH+=24;
    }
    tempH = '0' + tempH;
    tempH = tempH.substring(tempH.length - 2, tempH.length);
    tempD+=addDay;
    var addMM = 0;
    var tag = 1;
    if(tempD <= 0){
    	tag = -1;
    	addMM = -1;
    }
    while(tempD * tag > 0){
    	var tmpAI = (tempMM + addMM - 1) % 12;
    	if(tmpAI < 0){
    		tmpAI += 12;
    	}
    	tempD -= tag * dayCountArray[tmpAI];
    	addMM += tag * 1;
    }
    if(tag > 0){
    	tempD += tag * dayCountArray[(tempMM + addMM - 1 - tag * 1) % 12];
    }
    addMM -= tag * 1;
    if(tempD<0)
    {
        tempD+=dayCountArray[tempMM-1];
    }
    tempD = '0' + tempD;
    tempD = tempD.substring(tempD.length - 2, tempD.length);
    tempMM+=addMM;
    var addY = 0;
    if(tempMM == 0){
    	addY = -1;
    	tempMM = 12;
    }
    addY+=Math.floor(tempMM/12);
    tempMM%=12;
    if(tempMM<=0)
    {
        tempMM+=12;
        addY-=1;
    }
    tempMM = '0' + tempMM;
    tempMM = tempMM.substring(tempMM.length - 2, tempMM.length);
    tempY+=addY;
    return tempY+'-'+tempMM+'-'+tempD+' '+tempH+':'+tempM+':'+tempS;
}

function GetUIDHex(num) {
	var tmpStr = '00000000' + num.toString(16);
	return tmpStr.substring(tmpStr.length - 8,tmpStr.length);
}