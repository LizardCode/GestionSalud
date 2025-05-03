(function ($) {

    $.fn.DropDownGroup = function (settings) {

        var $dropdown = this;
        var $data;

        var defaultSettings = {};

        if (this.length > 1) {
            this.each(function (i, e) {
                $(e).DropDownGroup(settings);
            });

            return;
        }

        if ($dropdown.prop('tagName') == 'INPUT') {
            if ($dropdown.parent().hasClass('dropdown-group-list'))
                $dropdown = $dropdown.parent();
            else
                return;
        }
        else if ($dropdown.prop('tagName') == 'DIV' && !$dropdown.hasClass('dropdown-group-list'))
            return;

        $data = $dropdown.data('dropdown-group-list');

        if ($data === undefined)
            initialize();

        function initialize() {

            $.extend(defaultSettings, settings);

            var controls = getControls($dropdown);

            controls.items.on('click', function (e) {
                e.preventDefault();

                if (controls.dropdownField.val() == $(this).data('value'))
                    return;

                controls.dropdownField.val($(this).data('value')).trigger('change');
                controls.btnText.text($(this).text());
            });

            controls.items.first().trigger('click');
            $dropdown.data('dropdown-group-list', 'initialized');
        }

        function getControls(element) {
            var dropdownField = element.find('.input-group-prepend > input[type=hidden]');
            var inputField = element.find('> input[type=text]');
            var btn = element.find('.input-group-prepend > button.dropdown-toggle');
            var btnText = element.find('.input-group-prepend > button.dropdown-toggle > span');
            var menu = element.find('.input-group-prepend > .dropdown-menu');
            var items = menu.find('> a.dropdown-item');

            return {
                dropdownField: dropdownField,
                inputField: inputField,
                btn: btn,
                btnText: btnText,
                menu: menu,
                items: items
            };
        }

        var api = {
            val: function (dropdownValue, inputValue) {

                var controls = getControls($dropdown);

                if (dropdownValue === undefined && inputValue === undefined)
                    return {
                        value: controls.dropdownField.val(),
                        text: controls.btnText.text(),
                        input: controls.inputField.val()
                    };

                if (dropdownValue !== undefined)
                    controls.items
                        .filter('[data-value=' + dropdownValue + ']')
                        .trigger('click');

                if (inputValue !== undefined)
                    controls.inputField.val(inputValue);
            },

            readonly: function (flag) {

                var controls = getControls($dropdown);

                if (flag) {
                    controls.btn.attr('readonly', true);
                    controls.inputField.prop('readonly', true);
                }
                else {
                    controls.btn.attr('readonly', null);
                    controls.inputField.attr('readonly', null);
                }

                return api;
            }
        };

        return api;
    }

}(jQuery));