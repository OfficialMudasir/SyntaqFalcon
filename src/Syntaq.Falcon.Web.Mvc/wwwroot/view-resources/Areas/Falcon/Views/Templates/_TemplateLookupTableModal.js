(function ($) {
    app.modals.TemplateLookupTableModal = function () {

        var _modalManager;

        //var _recordMattersService = abp.services.app.recordMatters;
        var _$templateTable = $('#TemplateTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var documents = $('#kt_wizard_form_step_1_form').serializeJSON({ useIntKeysAsArrayIndex: false });

        //$(documents).each(function (objkey, objvalue) {
        //    console.log("Object is " + objvalue);
        //    $(objvalue).each(function (key, value) {
        //        console.log("Key is " + key);
        //        console.log("Value is " + value);
        //    });
        //});

        var DocArr = [];
        var i;
        for (i = 0; i < documents.Document.length; i++) {
            $(documents.Document[i]).each(function (objkey, objvalue) {
                var InnerArr = [];
                InnerArr['DocumentId'] = objvalue.DocumentId;
                InnerArr['DocumentName'] = objvalue.DocumentName;
                DocArr.push(InnerArr);
                //DocArr.push(new Array("DocumentId:" + objvalue.DocumentId, "DocumentName:" + objvalue.DocumentName));
                //$(objvalue).each(function (key, value) {
                //    console.log("Key is " + key);
                //    console.log("Value is " + value);
                //});
            });
        }

        //var dataSet = [
        //    ["1", "Doc A"],
        //    ["2", "Doc B"]
        //];

        var dataTable = _$templateTable.DataTable({
            data: DocArr,
            columnDefs: [
                {
                    orderable: false,
                    targets: 0,
                    data: null,
                    render: function (data, type, row) {
                        data = `<div class="text-dark text-width" style = "text-overflow:ellipsis; overflow: hidden; width: 250px;">${data.DocumentName}</div>`;
                        return data;
                    }
                },
                {
                    width: 120,
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: "<div class=\"text-center\"><input id='selectpdfbtn' class='btn btn-secondary me-3' type='button' width='25px' value='" + app.localize('Pdf') + "' /><input id='selectbtn' class='btn btn-secondary' type='button' width='25px' value='" + app.localize('Word') + "' /></div>"
                }
            ]
        });

        $('#TemplateTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            data.DocumentName = data.DocumentName + ".docx";
            data.Format = ".docx";
            _modalManager.setResult(data);
            _modalManager.close();
        });

        $('#TemplateTable tbody').on('click', '[id*=selectpdfbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            data.DocumentName = data.DocumentName + ".pdf";
            data.Format = ".pdf";
            _modalManager.setResult(data);
            _modalManager.close();
        });


        //$('#RecordTable tbody').on('click', '[id*=selectbtn]', function () {
        //    var data = dataTable.row($(this).parents('tr')).data();
        //    _modalManager.setResult(data);
        //    _modalManager.close();
        //});

        //function getRecord() {
        //    dataTable.ajax.reload();
        //}

        //$('#GetRecordButton').click(function (e) {
        //    e.preventDefault();
        //    getRecord();
        //});

        //$('#SelectButton').click(function (e) {
        //    e.preventDefault();
        //});

        //$(document).keypress(function (e) {
        //    if (e.which === 13) {
        //        getRecord();
        //    }
        //});

    };
})(jQuery);