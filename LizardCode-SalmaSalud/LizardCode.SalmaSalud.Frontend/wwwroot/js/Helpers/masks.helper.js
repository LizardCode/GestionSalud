/// <reference path="../../lib/accounting/accounting.min.js" />
/// <reference path="../../lib/autonumeric/autonumeric.min.js" />
/// <reference path="../../lib/cleave/cleave.min.js" />
/// <reference path="../../lib/cleave/cleave.jquery.js" />
/// <reference path="../../lib/cleave/cleave-phone.ar.js" />
/// <reference path="../../lib/jquery-alphanum/jquery.alphanum.js" />

(function ($) {

    $.fn.Mask = function (settings) {

        this.each(function (i, e) {

            // TODO: Agregar todas las máscaras restantes

            if ($(e).hasClass('cuit')) {

                new Cleave(e, {
                    blocks: [2, 8, 1],
                    delimiter: "-",
                    numericOnly: true
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