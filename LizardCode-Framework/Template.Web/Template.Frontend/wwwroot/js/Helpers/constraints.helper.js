/// <reference path="../../lib/accounting/accounting.min.js" />
/// <reference path="../../lib/autonumeric/autonumeric.min.js" />
/// <reference path="../../lib/cleave/cleave.min.js" />
/// <reference path="../../lib/imask/imask.min.js" />
/// <reference path="../../lib/jquery-alphanum/jquery.alphanum.js" />

var ConstraintsMaskEx = new (function () {

    var _instance = this;

    this.init = function (settings) {
        $('div').find(`[data-constraint='mask']`).each(function (i, obj) {
            var divObj = $(obj);

            $(obj).find('input[type=text]').each(function (i, objInput) {
                var obj = $(objInput)[0];
                var mask = $(divObj).data('constraint-mask');
                var type = $(divObj).data('constraint-type');

                switch (type) {
                    case enums.MaskConstraintType.RegExp:
                    case enums.MaskConstraintType.String:
                        IMask(obj, {
                            mask: mask,
                            lazy: false
                        });
                        break;

                    case enums.MaskConstraintType.Number:
                        IMask(obj, {
                            mask: Number,
                            normalizeZeros: false,
                            lazy: false
                        });
                        break;

                    case enums.MaskConstraintType.Date:
                        IMask(obj, {
                            mask: Date,
                            pattern: mask,
                            lazy: false,
                            autofix: true,
                            overwrite: true
                        });
                        break;
                }
            });
        });

        $(`input[type=text][data-constraint='mask']`).each(function (i, obj) {
            var obj = $(obj)[0];
            var mask = $(obj).data('constraint-mask');
            var type = $(obj).data('constraint-type');

            switch (type) {
                case enums.MaskConstraintType.RegExp:
                case enums.MaskConstraintType.String:
                    IMask(obj, {
                        mask: mask,
                        lazy: false
                    });
                    break;

                case enums.MaskConstraintType.Number:
                    IMask(obj, {
                        mask: Number,
                        lazy: false
                    });
                    break;

                case enums.MaskConstraintType.Date:
                    IMask(obj, {
                        mask: Date,
                        pattern: mask,
                        lazy: false,
                        autofix: true,
                        overwrite: true
                    });
                    break;
            }
        });
    }
});