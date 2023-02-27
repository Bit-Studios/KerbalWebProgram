$(function() {
	//The passed argument has to be at least a empty object or a object with your desired options
	$("body").overlayScrollbars({

        className            : "os-theme-dark",
        resize               : "none",
        sizeAutoCapable      : true,
        clipAlways           : true,
        normalizeRTL         : true,
        paddingAbsolute      : false,
        autoUpdate           : null,
        autoUpdateInterval   : 33, 
        updateOnLoad         : ["img"],
        nativeScrollbarsOverlaid : {
            showNativeScrollbars   : false,
            initialize             : true 
        },
        overflowBehavior : {
            x : "scroll",
            y : "scroll"
        },
        scrollbars : {
            visibility       : "auto",
            autoHide         : "scroll",
            autoHideDelay    : 800,
            dragScrolling    : true,
            clickScrolling   : false,
            touchSupport     : true,
            snapHandle       : false
        },
        textarea : {
            dynWidth       : false,
            dynHeight      : false,
            inheritedAttrs : ["style", "class"]
        },
        callbacks : {
            onInitialized               : null,
            onInitializationWithdrawn   : null,
            onDestroyed                 : null,
            onScrollStart               : null,
            onScroll                    : null,
            onScrollStop                : null,
            onOverflowChanged           : null,
            onOverflowAmountChanged     : null,
            onDirectionChanged          : null,
            onContentSizeChanged        : null,
            onHostSizeChanged           : null,
            onUpdated                   : null
        }

     });
});