jQuery.fn.elementHaveClassName = function(className) {
	return $(this).prop('class').toLowerCase().indexOf(className.toLowerCase()) != -1;
};

jQuery.extend({
	myAjax : function(setting) {
		var loadEle = setting.loadEle;
		alert("ele 创建加载");
		if (history && history.pushState) {
	    	history.pushState(null, document.title, setting.url);
	    }
		
		var funPtr = setting.success;
		jQuery.extend(setting,{
			success : [
				funPtr,
				function(){
					alert("ele 移除加载");
				}
			]
		});
		$.ajax(setting);
	}
});

var BuildPullHeightDiv = function(par) {
	par.append("<div class=\"pullupDiv\"></div>");
}