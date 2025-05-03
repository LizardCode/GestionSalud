/// <reference path="../../lib/accounting/accounting.min.js" />
/// <reference path="../../lib/autonumeric/autonumeric.min.js" />
/// <reference path="../../lib/cleave/cleave.min.js" />
/// <reference path="../../lib/cleave/cleave.jquery.js" />
/// <reference path="../../lib/cleave/cleave-phone.ar.js" />
/// <reference path="../../lib/jquery-alphanum/jquery.alphanum.js" />
/// <reference path="../Shared/enums.js" />

var ConstraintsMaskEx = new (function () {

    var _instance = this;

    this.init = function (settings) {
        $('div').find(`[data-constraint='mask']`).each(function (i, obj) {
            var divObj = $(obj);

            $(obj).find('input[type=text]').each(function (i, objInput) {
                var obj = $(objInput)[0];
                var type = $(divObj).data('constraint-type');
                var blocks = $(divObj).data('constraint-blocks');
                var datePattern = $(divObj).data('constraint-datepattern');
                var delimiters = $(divObj).data('constraint-delimiters');
                var numericOnly = $(divObj).data('constraint-numericonly');
                var uppercase = $(divObj).data('constraint-uppercase');


                switch (type) {
                    case enums.MaskConstraintType.Custom:
                        new Cleave(obj, {
                            delimiter: delimiters || "",
                            blocks: eval(blocks) || [],
                            uppercase: eval(uppercase),
                            numericOnly: eval(numericOnly)
                        });
                        break;

                    case enums.MaskConstraintType.String:
                        new Cleave(obj, {
                            blocks: eval(blocks) || [],
                            uppercase: eval(uppercase)
                        });
                        break;

                    case enums.MaskConstraintType.Number:
                        new Cleave(obj, {
                            numeral: true
                        });
                        break;

                    case enums.MaskConstraintType.Date:
                        new Cleave(obj, {
                            date: true,
                            delimiter: "/",
                            datePattern: eval(datePattern)
                        });
                        break;
                }
            });
        });

        $(`input[type=text][data-constraint='mask']:not(.repeater-control)`).each(function (i, obj) {
            var obj = $(obj)[0];
            var type = $(obj).data('constraint-type');
            var blocks = $(obj).data('constraint-blocks');
            var datePattern = $(obj).data('constraint-datepattern');
            var delimiters = $(obj).data('constraint-delimiters');
            var numericOnly = $(obj).data('constraint-numericonly');
            var uppercase = $(obj).data('constraint-uppercase');

            switch (type) {
                case enums.MaskConstraintType.Custom:
                    new Cleave(obj, {
                        delimiter: delimiters || "",
                        blocks: blocks || [],
                        uppercase: uppercase || false,
                        numericOnly: numericOnly || false
                    });
                    break;

                case enums.MaskConstraintType.String:
                    new Cleave(obj, {
                        blocks: blocks || [],
                        uppercase: uppercase || false
                    });
                    break;

                case enums.MaskConstraintType.Number:
                    new Cleave(obj, {
                        numeral: true
                    });
                    break;

                case enums.MaskConstraintType.Date:
                    new Cleave(obj, {
                        date: true,
                        delimiter: "/",
                        datePattern: eval(datePattern)
                    });
                    break;
            }
        });

        $(`input[type=text][data-constraint='autonumeric']:not(.repeater-control)`).each(function (i, obj) {
            var obj = $(obj)[0];
            var type = $(obj).data('constraint-type');

            var currencySymbol = $(obj).data('constraint-currencysymbol');
            var decimalCharacter = $(obj).data('constraint-decimalcharacter');
            var digitGroupSeparator = $(obj).data('constraint-digitgroupseparator');
            var decimalPlaces = $(obj).data('constraint-decimalplaces');
            var maximumValue = $(obj).data('constraint-maximumvalue');
            var minimumValue = $(obj).data('constraint-minimumvalue');

            switch (type) {
                case enums.AutoNumericConstraintType.Numeric:
                    new AutoNumeric(obj, {
                        unformatOnSubmit: true,
                        decimalCharacter: decimalCharacter,
                        emptyInputBehavior: "press",
                        digitGroupSeparator: digitGroupSeparator,
                        decimalPlaces: decimalPlaces,
                        decimalCharacterAlternative: ".",
                        maximumValue: maximumValue,
                        minimumValue: minimumValue
                    });
                    break;

                case enums.AutoNumericConstraintType.Currency:
                    new AutoNumeric(obj, {
                        unformatOnSubmit: true,
                        currencySymbol: currencySymbol,
                        emptyInputBehavior: "press",
                        decimalCharacter: decimalCharacter,
                        digitGroupSeparator: digitGroupSeparator,
                        decimalPlaces: decimalPlaces,
                        decimalCharacterAlternative: ".",
                        maximumValue: maximumValue,
                        minimumValue: minimumValue
                    });
                    break;

                case enums.AutoNumericConstraintType.Percentage:
                    new AutoNumeric(obj, { unformatOnSubmit: true, decimalCharacter: decimalCharacter, emptyInputBehavior: "press", decimalPlaces: decimalPlaces, digitGroupSeparator: "." });
                    break;
            }

        });
    }
});