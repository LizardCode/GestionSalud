(function ($) {

    $.fn.Select2Ex = function (settings) {

        var isInit = false;
        var $select = this;
        var $data;

        var defaultSettings = {
            language: 'es',
        };

        if (this.length > 1) {
            this.each(function (i, e) {
                $(e).Select2Ex(settings);
            });

            return;
        }

        $data = $select.data('select2');

        if ($data === undefined)
            initialize();


        function initialize() {
            if (settings !== undefined && settings.highlightSearchTerms === true)
                defaultSettings.templateResult = highlightSearchTerms;

            $.extend(defaultSettings, settings);

            $select
                .select2(defaultSettings)
                .on('select2-close', function (e) {
                    if ($.isFunction(defaultSettings.onDropdownClose))
                        defaultSettings.onDropdownClose(e, defaultSettings);
                });
        }

        function highlightSearchTerms(item) {
            if (item.loading)
                return item.text;

            var term = $select
                .data('select2')
                .$dropdown
                .find('input[type="search"]')
                .val();

            var $span = $('<span />');
            var output = item.text;

            if (term.length > 0) {
                var match = item.text.match(new RegExp(term, 'gi'));

                for (var i in match)
                    output = output.replace(new RegExp(match[i], 'g'), '<b>' + match[i] + '</b>');
            }

            return $span.html(output);
        }


        var api = {
            show: function () {
                $data.$container.show();
            },

            hide: function () {
                $data.$container.hide();
            },

            enabled: function (flag) {
                $select.prop('disabled', !flag);
            },

            selectedItem: function () {
                return $select.select2('data')[0];
            },

            val: function () {
                return $select.select2('val');
            },

            source: function (array) {

                if (array === undefined || array === null || !Array.isArray(array))
                    return;

                var isSelect = ($select.prop('tagName') === 'SELECT');

                if (isSelect) {

                    $select.empty();

                    if (array.length === 0)
                        return;

                    for (var i in array) {

                        var item = array[i];

                        $select.append(
                            $('<option />').val(item.id).text(item.text)
                        );

                    }

                    $select.select2();
                }
                else {

                    $select.select2({ data: array });

                }
            }
        };

        return api;
    };

}(jQuery));