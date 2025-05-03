(function ($) {

    $.fn.LoaderEx = function (settings) {

        var $target = this;
        var $overlay;
        var $loader;

        var defaultSettings = {
            message: 'Cargando...',
            top: 'calc(50% - 102px)',
            left: 'calc(50% - 189px)'
        };

        $.extend(defaultSettings, settings);

        if ($target.data('logo-loader') === true) {
            $overlay = $target.find('div.loader-overlay');
            $loader = $target.find('div.loader')
            if (settings !== undefined && settings.message !== undefined)
                $loader.find('span').text(defaultSettings.message);
        }
        else {
            $overlay = $('<div />')
                .addClass('loader-overlay');

            $loader = $('<div />')
                .css('top', defaultSettings.top)
                .css('left', defaultSettings.left)
                .addClass('loader')
                .append(
                    $('<div />')
                        .addClass('img')
                )
                .append(
                    $('<span />')
                        .text(defaultSettings.message.trim())
                );

            var targetHeight = $target.innerHeight();

            if (targetHeight < 205) {
                var height = 205 + (targetHeight > 0 ? targetHeight : 60);
                $target.css('min-height', height)
            }

            $target
                .css('position', ($target.css('position') == 'absolute' ? 'absolute' : 'relative'))
                .append($overlay)
                .append($loader)
                .data('logo-loader', true);
        }

        return {
            show: function () {
                $overlay.fadeIn();
                $loader.fadeIn();
            },

            showOverlay: function () {
                $overlay.show();
            },

            hide: function (doneCallback) {
                $overlay.fadeOut(500, function () {
                    if (doneCallback)
                        doneCallback();
                });
                $loader.fadeOut();
            },

            isVisible: function () {
                return $overlay.is(':visible') || $loader.is(':visible');
            },

            message: function (message) {
                $loader.find('span.msg').text(message);
            },

            destroy: function () {
                $target.data('logo-loader', '');
                $overlay.remove();
                $loader.remove();
            }
        };
    };

}(jQuery));