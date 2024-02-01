(function ($) {
    app.modals.RecordLookupTableModal = function () {

        var _modalManager;

        var _recordMattersService = abp.services.app.recordMatters;
        var _$recordTable = $('#RecordTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };
        var dataTable = _$recordTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMattersService.getAllRecordForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordTableFilter').val()
                    };
                }
            },
            columnDefs: [
                //{
                //    targets: 0,
                //    width: 120,
                //    data: null,
                //    orderable: false,
                //    autoWidth: false,
                //    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                //},
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 0,
                    data: "displayName",
                    render: function (data, type, row) {
                        data += '<span class=\"text-center\"><input id="selectbtn" class="btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x" type="button" width="25px" value="' + app.localize('Select') + '" /></span>';
                        return data;
                    }
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