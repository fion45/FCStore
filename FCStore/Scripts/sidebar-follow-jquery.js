/**
 * @author: mg12 [http://www.neoease.com/]
 * @update: 2012/12/05
 */

//只能设置成absolute，而fixed有Bug

SidebarFollow = function() {

	this.config = {
		element: null, // 处理的节点
		distanceToTop: 0, // 节点上边到页面顶部的距离
		afterFollowCB : null,
		afterStopCB : null
	};

	this.cache = {
		originalToTop: 0, // 原本到页面顶部的距离
//		prevElement: null, // 上一个节点
//		parentToTop: 0, // 父节点的上边到顶部距离
//		placeholder: jQuery('<div>') // 占位节点
		parTopTag: false,
		fixedTag : false,
		fixedTop : null
	}
};

SidebarFollow.prototype = {
	init: function(config) {
		this.config = config || this.config;
		var _self = this;
		var element = jQuery(_self.config.element);

		// 如果没有找到节点, 不进行处理
		if(element.length <= 0) {
			return;
		}

		// 获取上一个节点
//		var prevElement = element.prev();
//		while(prevElement.is(':hidden')) {
//			prevElement = prevElement.prev();
//			if(prevElement.length <= 0) {
//				break;
//			}
//		}
//		_self.cache.prevElement = prevElement;

		// 计算父节点的上边到顶部距离
//		var parent = element.parent();
//		var parentToTop = parent.offset().top;
//		var parentBorderTop = parseInt(parent.css('border-top-width'));
//		var parentPaddingTop = parseInt(parent.css('padding-top'));
//		_self.cache.parentToTop = parentToTop + parentBorderTop + parentPaddingTop;

		// 滚动屏幕
		jQuery(window).scroll(function() {
			_self._scrollScreen({element:element, _self:_self});
		});

		// 改变屏幕尺寸
		jQuery(window).resize(function() {
			_self._scrollScreen({element:element, _self:_self});
		});
		
		var bodyToTop = parseInt(jQuery('body').css('top'), 10);
		_self.cache.parTopTag = !isNaN(bodyToTop);
		
		if(element.css('position') == "fixed") {
			_self.cache.fixedTop = parseInt(element.css('top'));
		}
		_self._scrollScreen({
			_self : _self,
			element : element
		});
	},

	/**
	 * 修改节点位置
	 */
	_scrollScreen: function(args) {
		var _self = args._self;
		var element = args.element;
//		var prevElement = _self.cache.prevElement;
		prevElement = null;
		
		// 获得到顶部的距离
		var toTop = _self.config.distanceToTop;
		var tmpPer = _self.config.distanceToTop + "";
		if(tmpPer.indexOf("%") > 0) {
			toTop = document.documentElement.clientHeight * parseInt(tmpPer) / 100;
		}

		// 如果 body 有 top 属性, 消除这些位移
		if(_self.cache.parTopTag) {
			var bodyToTop = parseInt(jQuery('body').css('top'), 10);
			toTop += bodyToTop;
			// 获得到顶部的绝对距离
		}
		if(!_self.cache.fixedTag || _self.cache.fixedTop == null) {
			_self.cache.elementToTop = parseInt(element.css('top')) - toTop;
		}
		else {
			_self.cache.elementToTop = parseInt(element.css('top')) - toTop;
			element.css('top',_self.cache.fixedTop - jQuery(document).scrollTop());
		}
		

		// 如果存在上一个节点, 获得到上一个节点的距离; 否则计算到父节点顶部的距离
//		var referenceToTop = 0;
//		if(prevElement && prevElement.length === 1) {
//			referenceToTop = prevElement.offset().top + prevElement.outerHeight();
//		} else {
//			referenceToTop = _self.cache.parentToTop - toTop;
//		}

		// 当节点进入跟随区域, 跟随滚动
		if(jQuery(document).scrollTop() - _self.cache.elementToTop >= 1) {
			// 添加占位节点
			var elementHeight = element.outerHeight();
			// 记录原位置
			_self.cache.originalToTop = _self.cache.elementToTop;
			// 修改样式
			if(_self.cache.oldStyle == null) {
				_self.cache.oldStyle = element.attr("style");
				if(_self.cache.oldStyle == null) {
					_self.cache.oldStyle = {};
				}
			}
			element.attr("style","top:" + toTop + "px;position:fixed;");
			_self.cache.fixedTop = toTop;
			_self.cache.fixedTag = true;
			if(_self.config.afterFollowCB != null) {
				_self.config.afterFollowCB(element);
			}
		// 否则回到原位
		} else if(_self.cache.originalToTop >= _self.cache.elementToTop) {
			if(_self.cache.oldStyle) {
				element.attr("style",_self.cache.oldStyle);
			}
			else {
				element.removeAttr("style");	
			}
			_self.cache.fixedTop = null;
			_self.cache.fixedTag = false;
			if(_self.config.afterStopCB != null) {
				_self.config.afterStopCB(element);
			}
		}
	}
};