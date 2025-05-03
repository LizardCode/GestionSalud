var LoginView = new (function () {

    var self = this;
    var btSubmit = $('form button.btSubmit');
    var resetingPassword = false;

    this.init = function () {
        obtenerDatosBrowser();

        buildControls();
        bindEvents();

    }

    this.ajaxFormBegin = function (context) {
        obtenerDatosBrowser();

        console.log('ajaxFormBegin', context);
        submitLoader("Aguarde...", true);
    }

    this.ajaxFormSuccess = function (data) {
        console.log('ajaxFormSuccess');


        $('.g-recaptcha').hide();

        if (!$.isPlainObject(data)) {
            console.log('Data: ', data);
            alertError('Se produjo un error grave', false);
            submitLoader("Ingresar", false);
        }
        else {
            if (data.status === undefined || data.status === null ||
                data.detail === undefined || data.detail === null) {
                console.log('Data: ', data);
                alertError('Se produjo un error grave', false);
                submitLoader("Ingresar", false);
            }
            else {
                if (data.status == enums.AjaxStatus.OK)
                    location.href = data.detail;
                else {
                    switch (data.detail.code) {
                        case 0:
                            console.error(data.detail.message);
                            alertError('Se produjo un error grave', false);
                            break;

                        case 1: // Login failed
                            alertError(data.detail.message, 2000);
                            $('#Pass').val('');
                            $('#User').val('').focus();
                            break;

                        case 2: // Contraseña vencida
                            $('form').attr('action', data.detail.message)
                            alertInfo('Contraseña vencida');
                            $('#RepeatPass').val('');
                            $('#NewPass').val('').focus();
                            requestNewPassword();
                            return;

                        case 3: // Usuario inexistente
                            alertInfo(data.detail.message);
                            $('#Pass').val('');
                            $('#User').val('').focus();
                            break;

                        case 4: // Contraseña debil
                            $('#RepeatPass').val('');
                            $('#NewPass').val('').focus();
                            alertInfo(data.detail.message, 5000);
                            break;

                        case 5: // No coinciden las contraseñas
                            $('#RepeatPass').val('');
                            $('#NewPass').val('').focus();
                            alertInfo(data.detail.message);
                            break;

                        case 11: // Selección de Empresa
                            alertInfo('Seleccione una Empresa');
                            requestEmpresa();
                            return;

                        case 12: // Captcha
                            alertError(data.detail.message, false);
                            $('#Pass').val('');
                            $('#User').val('').focus();
                            requestCaptcha();
                            submitLoader(null, false);
                            return;

                        default:
                        //case 7: // Mas de un resultado
                        //case 8: // Clave incorrecta
                            $('#Pass').val('');
                            $('#User').val('').focus();
                            setTimeout(function () {
                                location.href = RootPath + '/Login';
                            }, 3000);
                            alertError(data.detail.message, 2000);
                            break;
                    }

                    submitLoader(null, false);
                }
            }
        }

    }

    this.ajaxFormFailure = function (context) {
        console.error(context);
        alertError('Se produjo un error grave', false);
        submitLoader("Ingresar", false);
    }


    function buildControls() {

        $("#global-loader").fadeOut("slow");
        $('#User').focus();

        feather.replace();

        $('#IdEmpresa').Select2Ex();

    }

    function bindEvents() {

        $('form').on('submit', function (e) {

            var user = $('#User').val().trim();
            var pass = $('#Pass').val().trim();

            if (user == '') {
                e.preventDefault();
                $('#User').val('').focus();
                return false;
            }

            if (pass == '') {
                e.preventDefault();
                $('#Pass').val('').focus();
                return false;
            }

            if (user == '' && pass == '')
                return false;

            if (resetingPassword) {

                var newPass = $('#NewPass').val().trim();
                var reePass = $('#RepeatPass').val().trim();

                if (newPass == '') {
                    e.preventDefault();
                    $('#NewPass').val('').focus();
                    return false;
                }
                else if (reePass == '') {
                    e.preventDefault();
                    $('#RepeatPass').val('').focus();
                    return false;
                }

            }

            if ($('#empresa-field').is(':visible')) {
                var empresa = $('#IdEmpresa').select2('data');

                if (empresa == null) {
                    $('#IdEmpresa').focus();
                    alertInfo('Seleccione una Empresa');
                    return false;
                }
            }

        });

    }


    function requestNewPassword() {

        resetingPassword = true;

        $('#User').prop('readonly', true);
        $('#User').parents('.field-wrapper').addClass('disabled');
        $('#Pass').parents('.field-wrapper').addClass('hidden');
        $('#NewPass').parents('.field-wrapper').removeClass('hidden');
        $('#RepeatPass').parents('.field-wrapper').removeClass('hidden');

        btSubmit
            .prop('disabled', false)
            .text('Actualizar contraseña');

        $('#NewPass').focus();
    }

    function requestEmpresa() {

        Ajax.ToSelect({
            action: "/Login/GetEmpresasUsuarios",
            params: {
                user: $('#User').val()
            },
            selector: "#IdEmpresa",
            valueProperty: 'idEmpresa',
            textProperty: 'razonSocial'
        });

        $('#User').prop('readonly', true);
        $('#Pass').prop('readonly', true);
        $('#empresa-field').removeClass('hidden');

        btSubmit
            .prop('disabled', false)
            .text('Seleccionar Empresa');
    }

    function requestCaptcha() {

        $('.g-recaptcha').show();
        grecaptcha.reset();
    }

    function alertInfo(message, timeout) {

        Utils.alertInfo(message, {
            layout: 'topCenter',
            animation: {
                open: 'animated bounceInDown',
                close: 'animated bounceOutUp'
            },
            timeout: timeout | 2000
        });

    }

    function alertError(message, timeout) {

        Utils.alert(message, 'error', {
            layout: 'topCenter',
            animation: {
                open: 'animated bounceInDown',
                close: 'animated bounceOutUp'
            },
            closeWith: timeout ? ['click'] : ['button'],
            timeout: timeout
        });

    }

    function submitLoader(text, flag) {

        if (flag) {
            btSubmit
                .data('normal-text', btSubmit.text())
                .prop('disabled', true)
                .html('<i class="fas fa-cog fa-spin"></i> ' + text);
        }
        else {

            if (text === null)
                text = btSubmit.data('normal-text');

            btSubmit
                .prop('disabled', false)
                .html(text)
                .find('i')
                .remove();
        }

    }

});

function obtenerDatosBrowser() {
    var platform, resolution = '';
    var device = (/android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(navigator.userAgent.toLowerCase()));

    var mobileChk = (/mobile/i.test(navigator.userAgent.toLowerCase()));
    if (device && window.screen.width < 481) platform = 'Mobile';
    else if (device && window.screen.width > 481 && window.screen.width < 1025) platform = 'Tablet';
    else platform = 'Desktop';

    resolution = window.screen.width + ' X ' + window.screen.height;
    var nAgt = navigator.userAgent;
    var os = '';
    var clientStrings = [
        { s: 'Windows 3.11', r: /Win16/ },
        { s: 'Windows 95', r: /(Windows 95|Win95|Windows_95)/ },
        { s: 'Windows ME', r: /(Win 9x 4.90|Windows ME)/ },
        { s: 'Windows 98', r: /(Windows 98|Win98)/ },
        { s: 'Windows CE', r: /Windows CE/ },
        { s: 'Windows 2000', r: /(Windows NT 5.0|Windows 2000)/ },
        { s: 'Windows XP', r: /(Windows NT 5.1|Windows XP)/ },
        { s: 'Windows Server 2003', r: /Windows NT 5.2/ },
        { s: 'Windows Vista', r: /Windows NT 6.0/ },
        { s: 'Windows 7', r: /(Windows 7|Windows NT 6.1)/ },
        { s: 'Windows 8', r: /(Windows 8|Windows NT 6.2)/ },
        { s: 'Windows 8.1', r: /(Windows 8.1|Windows NT 6.3)/ },
        { s: 'Windows 10', r: /(Windows 10|Windows NT 10.0)/ },
        { s: 'Windows NT 4.0', r: /(Windows NT 4.0|WinNT4.0|WinNT|Windows NT)/ },
        { s: 'Android', r: /Android/ },
        { s: 'Open BSD', r: /OpenBSD/ },
        { s: 'Sun OS', r: /SunOS/ },
        { s: 'Linux', r: /(Linux|X11)/ },
        { s: 'iOS', r: /(iPhone|iPad|iPod)/ },
        { s: 'Mac OS X', r: /Mac OS X/ },
        { s: 'Mac OS', r: /(MacPPC|MacIntel|Mac_PowerPC|Macintosh)/ },
        { s: 'QNX', r: /QNX/ },
        { s: 'UNIX', r: /UNIX/ },
        { s: 'BeOS', r: /BeOS/ },
        { s: 'OS/2', r: /OS\/2/ },
        { s: 'Search Bot', r: /(nuhk|Googlebot|Yammybot|Openbot|Slurp|MSNBot|Ask Jeeves\/Teoma|ia_archiver)/ }
    ];

    for (var id in clientStrings) {
        var cs = clientStrings[id];
        if (cs.r.test(nAgt)) {
            os = cs.s;
            break;
        }
    }

    var version = '' + parseFloat(navigator.appVersion);
    var browser = navigator.appName;

    // Opera
    if ((verOffset = nAgt.indexOf('Opera')) != -1) {
        browser = 'Opera';
        version = nAgt.substring(verOffset + 6);
        if ((verOffset = nAgt.indexOf('Version')) != -1) {
            version = nAgt.substring(verOffset + 8);
        }
    }
    // MSIE
    else if ((verOffset = nAgt.indexOf('MSIE')) != -1) {
        browser = 'Microsoft Internet Explorer';
        version = nAgt.substring(verOffset + 5);
    }
    // Chrome
    else if ((verOffset = nAgt.indexOf('Chrome')) != -1) {
        browser = 'Chrome';
        version = nAgt.substring(verOffset + 7);
    }
    // Safari
    else if ((verOffset = nAgt.indexOf('Safari')) != -1) {
        browser = 'Safari';
        version = nAgt.substring(verOffset + 7);
        if ((verOffset = nAgt.indexOf('Version')) != -1) {
            version = nAgt.substring(verOffset + 8);
        }
    }
    // Firefox
    else if ((verOffset = nAgt.indexOf('Firefox')) != -1) {
        browser = 'Firefox';
        version = nAgt.substring(verOffset + 8);
    }
    // MSIE 11+
    else if (nAgt.indexOf('Trident/') != -1) {
        browser = 'Microsoft Internet Explorer';
        version = nAgt.substring(nAgt.indexOf('rv:') + 3);
    }
    // Other browsers
    else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
        browser = nAgt.substring(nameOffset, verOffset);
        version = nAgt.substring(verOffset + 1);
        if (browser.toLowerCase() == browser.toUpperCase()) {
            browser = navigator.appName;
        }
    }

    // trim the version string
    if ((ix = version.indexOf(';')) != -1) version = version.substring(0, ix);
    if ((ix = version.indexOf(' ')) != -1) version = version.substring(0, ix);
    if ((ix = version.indexOf(')')) != -1) version = version.substring(0, ix);

    var rObject = {
        platform: platform,
        os: os,
        browser: browser,
        version: version,
        resolution: resolution
    };
    //console.log(rObject);

    $('#Platform').val(platform);
    $('#OS').val(os);
    $('#Browser').val(browser);
    $('#Version').val(version);
    $('#Resolucion').val(resolution);

    return rObject;
}

$(function () {

    LoginView.init();

});