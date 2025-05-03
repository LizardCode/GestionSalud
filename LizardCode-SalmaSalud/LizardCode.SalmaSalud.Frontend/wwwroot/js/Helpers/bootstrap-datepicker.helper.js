(function ($) {

    $.fn.DatepickerEx = function (settings) {

        var $selector = this;

        var defaultSettings = {
            format: 'dd/mm/yyyy',
            weekStart: 0,
            todayBtn: false, // linked | true | false
            language: 'es',
            autoclose: true,
            clearBtn: false,
            todayHighlight: false,
            toggleActive: true,
            templates: {
                leftArrow: '<i class="fas fa-chevron-left"></i>',
                rightArrow: '<i class="fas fa-chevron-right"></i>'
            }            
        };


        function init() {

            if (typeof settings === 'object')
                $.extend(defaultSettings, settings);

            $selector
                .datepicker(defaultSettings)
                .on('show', function (e, settings) {
                    if ($.isFunction(defaultSettings.onShow))
                        defaultSettings.onShow(e, settings);
                })
                .on('hide', function (e, settings) {
                    e.stopPropagation();
                    if ($.isFunction(defaultSettings.onHide))
                        defaultSettings.onHide(e, settings);
                })
                .on('clearDate', function (e, settings) {
                    if ($.isFunction(defaultSettings.onClearDate))
                        defaultSettings.onClearDate(e, settings);
                })
                .on('changeDate', function (e, settings) {
                    if ($.isFunction(defaultSettings.onChangeDate))
                        defaultSettings.onChangeDate(e, settings);
                })
                .on('changeMonth', function (e, settings) {
                    if ($.isFunction(defaultSettings.onChangeMonth))
                        defaultSettings.onChangeMonth(e, settings);
                })
                .on('changeYear', function (e, settings) {
                    if ($.isFunction(defaultSettings.onChangeYear))
                        defaultSettings.onChangeYear(e, settings);
                })
                .on('changeDecade', function (e, settings) {
                    if ($.isFunction(defaultSettings.onChangeDecade))
                        defaultSettings.onChangeDecade(e, settings);
                })
                .on('changeCentury', function (e, settings) {
                    if ($.isFunction(defaultSettings.onChangeCentury))
                        defaultSettings.onChangeCentury(e, settings);
                })
                .each(function () {
                    var $dp = $(this);
                    var $addon = $(this).next('span.input-group-addon');
                    if ($addon.length == 1)
                        $addon.on('click', function () {
                            $dp.datepicker('show');
                        });
                });

            $selector.mask("99/99/9999", { placeholder: '_' });
        }

        var api = {
            self: function () {
                return $selector;
            },

            show: function () {
                $selector.datepicker('show');
            },

            hide: function () {
                $selector.datepicker('hide');
            },

            update: function () {
                var args = Array.prototype.slice.call(arguments);
                $selector.datepicker.apply($selector, ['update'].concat(args));
            },

            date: function (inputDate) {
                if (inputDate === undefined)
                    return $selector.datepicker('getDate');
                else
                    $selector.datepicker('setDate', inputDate);
            }
        };


        init();

        return api;
    };

}(jQuery));

//# sourceUrl=bootstrap-datepicker.helper.js