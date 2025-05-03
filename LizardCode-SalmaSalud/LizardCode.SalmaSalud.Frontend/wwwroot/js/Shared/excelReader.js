var ExcelReaderView = new function () {

    this.init = function () {

        $('#FileExcel').val('');
        $('#ERV_lblFileExcel').text('').hide();
        $('.ERV_btOkProcesarExcel').prop('disabled', true);

        $('#FileExcel').unbind().on('change', async function (e) {

            var fE = document.getElementById('FileExcel');

            $('#ERV_lblFileExcel').text(fE.files.item(0).name).show();
            $('.ERV_btOkProcesarExcel').prop('disabled', false);
        });

        $('.ERV_btOkProcesarExcel').unbind().click(function () {
            $('.ERV_frmProcesarExcel').submit();
        });
    }

    this.ajaxReadBegin = function () { }

    this.ajaxReadSuccess = function (response) { }

    this.ajaxReadFailure = function (jqXHR, textStatus, errorThrown) { }
};

$(function () {
    ExcelReaderView.init();
});

//# sourceURL=excelReader.js