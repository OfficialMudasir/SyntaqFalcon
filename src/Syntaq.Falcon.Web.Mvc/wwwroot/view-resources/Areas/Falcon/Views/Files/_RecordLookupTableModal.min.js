(function ($) {
    app.modals.RecordLookupTableModal = function () {

        var _modalManager;

        var _filesService = abp.services.app.files;
        var _$recordTable = $('#RecordTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _filesService.getAllRecordForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordTableFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-success' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 1,
                    data: "displayName"
                }
            ]
        });

        $('#RecordTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getRecord() {
            dataTable.ajax.reload();
        }

        $('#GetRecordButton').click(function (e) {
            e.preventDefault();
            getRecord();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecord();
            }
        });

    };
})(jQuery);

