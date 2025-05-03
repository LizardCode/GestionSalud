var LoginView = new (function () {

    var self = this;
    var btSubmit = $('form button.btSubmit');
    var resetingPassword = false;

    this.init = function () {

        buildControls();
        bindEvents();

    }


    this.ajaxFormBegin = function (context) {
        console.log('ajaxFormBegin', context);
        submitLoader("Aguarde...", true);
    }

    this.ajaxFormSuccess = function (data) {
        console.log('ajaxFormSuccess');

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

$(function () {

    LoginView.init();

});