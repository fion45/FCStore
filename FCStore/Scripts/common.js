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

jQuery.extend({
	myAjax : function(setting) {
		var loadEle = setting.loadEle;
		loadEle.showLoading();
		if (history && history.pushState) {
	    	history.pushState(null, document.title, setting.url);
	    }
		
		var funPtr = setting.success;
		jQuery.extend(setting,{
			success : [
				funPtr,
				function(){
					loadEle.hideLoading();
				}
			]
		});
		$.ajax(setting);
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