/// <reference path="../../lib/accounting/accounting.min.js" />
/// <reference path="../../lib/autonumeric/autonumeric.min.js" />
/// <reference path="../../lib/cleave/cleave.min.js" />
/// <reference path="../../lib/imask/imask.min.js" />
/// <reference path="../../lib/jquery-alphanum/jquery.alphanum.js" />

(function ($) {

    $.fn.Mask = function (settings) {

        this.each(function (i, e) {

            // TODO: Agregar todas las máscaras restantes

            if ($(e).hasClass('cuit')) {
                IMask(e, {
                    mask: '00{-}00000000{-}0',
                    lazy: false
                });
            }
            else if ($(e).hasClass('phone')) {
                new Cleave(e, {
                    phone: true,
                    phoneRegionCode: 'AR',
                    delimiter: '-'
                });
            }

        });

    };

}(jQuery));