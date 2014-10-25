(function ($) {
    $.fn.tappyTap = function (options) {

        var singleTapCallback = options.onSingleTap || function () { };
        var doubleTapCallback = options.onDoubleTap || function () { };
        var tripleTapCallback = options.onTrippleTap || function () { };

        var delayMs = options.delayMs || 400;

        var self = $(this);

        var tapCount = 0;
        var currentTimeout;

        var currentGesture = {
            tapCount: 0,
            timeout: null,
            end: function() {
                this.tapCount = 0;
            }
        };

        self.on('click', function () {
            currentGesture.tapCount++;
            clearTimeout(currentGesture.timeout);

            if (currentGesture.tapCount < 3) {
                currentGesture.timeout = setTimeout(onStoppedTapping, delayMs);
            } else {
                onStoppedTapping();
            }
        });

        var onStoppedTapping = function () {
            switch (currentGesture.tapCount) {
                case 1:
                    singleTapCallback();
                    break;
                case 2:
                    doubleTapCallback();
                    break;
                case 3:
                    tripleTapCallback();
                    break;

                default:
            }

            currentGesture.end();
        };
    };
})(jQuery);