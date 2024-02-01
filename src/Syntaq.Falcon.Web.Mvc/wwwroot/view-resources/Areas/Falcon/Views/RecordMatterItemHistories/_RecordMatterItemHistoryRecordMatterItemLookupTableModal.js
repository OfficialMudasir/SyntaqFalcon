(function ($) {
    app.modals.RecordMatterItemLookupTableModal = function () {

        var _modalManager;

        var _recordMatterItemHistoriesService = abp.services.app.recordMatterItemHistories;
        var _$recordMatterItemTable = $('#RecordMatterItemTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordMatterItemTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterItemHistoriesService.getAllRecordMatterItemForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordMatterItemTableFilter').val()
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

        $('#RecordMatterItemTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getRecordMatterItem() {
            dataTable.ajax.reload();
        }

        $('#GetRecordMatterItemButton').click(function (e) {
            e.preventDefault();
            getRecordMatterItem();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordMatterItem();
            }
        });

    };
})(jQuery);

