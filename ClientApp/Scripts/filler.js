(function($) {
    $.fn.filler = function () {
        $(this).css('height', ($(window).height() - 40) + 'px');
        $(this).css('width', ($(window).width() - 40) + 'px');
    };
})(jQuery);
