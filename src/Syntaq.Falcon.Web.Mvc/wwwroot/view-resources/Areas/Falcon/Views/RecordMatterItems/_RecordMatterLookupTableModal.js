(function ($) {
    app.modals.RecordMatterLookupTableModal = function () {


        var _modalManager;

        var _recordMatterItemsService = abp.services.app.recordMatterItems;
        var _$recordMatterTable = $('#RecordMatterTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordMatterTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterItemsService.getAllRecordMatterForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordMatterTableFilter').val()
                    };
                }
            },
            columnDefs: [
                //{
                //    targets: 0,
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
                        data += "<span class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></span>";
                        return data;

                    }
                }
            ]
        });

        $('#RecordMatterTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getRecordMatter() {
            dataTable.ajax.reload();
        }

        $('#GetRecordMatterButton').click(function (e) {
            e.preventDefault();
            getRecordMatter();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordMatter();
            }
        });

    };
})(jQuery);

