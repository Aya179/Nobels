
;(function ( $, window, document, undefined ) {

	"use strict";
	// Create the defaults once
	var pluginName = "equalWidthChildren",
		defaults = {
			fudge: 0,
			gutter: 0,
			children: null
		};

	// The actual plugin constructor
	function Plugin ( element, options ) {
		this.element = element;
		this.settings = $.extend( {}, defaults, options );
		this._defaults = defaults;
		this._name = pluginName;
		this.init();
	}

	// Avoid Plugin.prototype conflicts
	$.extend(Plugin.prototype, {
		init: function () {
			var $element = $(this.element);
			console.log($element.width());
			var $children = (this.settings.children === null) ? $element.children() : this.settings.children;
			var len = $children.length;
			var width = $element.width()/len - this.settings.fudge;
			console.log(width);
			if (this.settings.gutter > 0) {
				width -= ((len - 1) * this.settings.gutter)/len;
				$children.not(':last-child').css({
					marginRight: this.settings.gutter
				});
			}
			$children.css('width', width);
		}
	});

	// A really lightweight plugin wrapper around the constructor,
	// preventing against multiple instantiations
	$.fn[ pluginName ] = function ( options ) {
		return this.each(function() {
				if ( !$.data( this, "plugin_" + pluginName ) ) {
						$.data( this, "plugin_" + pluginName, new Plugin( this, options ) );
				}
		});
	};

})( jQuery, window, document );