(function ($) {

    $.fn.Select2Ex = function (settings) {

        var isInit = false;
        var $select = this;
        var $data;

        var defaultSettings = {
            language: 'es',
            width: ''
        };

        var defaultAjaxSettings = {
            placeholder: 'Buscar...',
            minimumInputLength: 3,
            maximumInputLength: 30,
            delay: 500,
            ajax: {
                type: 'POST',
                dataType: 'json',
                data: function (term, page) {
                    return {
                        q: term
                    };
                },
                processResults: function (data, params) {
                    return { results: data.detail }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    if (xhr.status == 401)
                        BaseLayoutView.expiredSession();
                    else if (xhr.responseJSON != null && xhr.responseJSON.desc != undefined)
                        Utils.alertError(xhr.responseJSON.desc);
                    else
                        Utils.alertError(thrownError.message);
                }
            }
        };

        if (this.length > 1) {
            this.each(function (i, e) {
                $(e).Select2Ex(settings);
            });

            return;
        }

        if (settings === undefined) {
            $data = $select.data('select2');

            if ($data === undefined)
                initialize();
        }
        else if (typeof settings === 'object') {
            isInit = true;
            initialize();
        }
        
        function initialize() {
            if (settings !== undefined && settings.highlightSearchTerms === true)
                defaultSettings.templateResult = highlightSearchTerms;

            if (settings !== undefined && settings.placeholder !== undefined)
                defaultSettings.placeholder = settings.placeholder;

            $.extend(defaultSettings, settings);

            if ($select.is('input')) {
                if (settings.url === undefined)
                    throw 'No se especificó el parámetro "url".';
                else
                    defaultAjaxSettings.ajax.url = settings.url;

                if (settings.minimumInputLength !== undefined)
                    defaultAjaxSettings.minimumInputLength = settings.minimumInputLength;

                if (settings.type !== undefined)
                    defaultAjaxSettings.ajax.type = settings.type;

                if (settings.dataType !== undefined)
                    defaultAjaxSettings.ajax.dataType = settings.dataType;

                if (settings.formatResult !== undefined)
                    defaultAjaxSettings.formatResult = settings.formatResult;

                if (settings.data !== undefined)
                    defaultAjaxSettings.ajax.data = settings.data;

                if (settings.map !== undefined)
                    defaultAjaxSettings.ajax.processResults = function (data, params) {
                        return {
                            results: $.map(data, settings.map)
                        }
                    };

                $.extend(defaultSettings, defaultAjaxSettings);
            }

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
                    $select.append($('<option />').val("").text("..."));
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

        return (isInit ? this : api);
    };

}(jQuery));