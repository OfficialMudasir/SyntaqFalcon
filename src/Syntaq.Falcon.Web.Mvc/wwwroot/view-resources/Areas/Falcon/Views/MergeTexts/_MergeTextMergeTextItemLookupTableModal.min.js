(function ($) {
    app.modals.MergeTextItemLookupTableModal = function () {

        var _modalManager;

        var _mergeTextsService = abp.services.app.mergeTexts;
        var _$mergeTextItemTable = $('#MergeTextItemTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$mergeTextItemTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _mergeTextsService.getAllMergeTextItemForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#MergeTextItemTableFilter').val()
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

        $('#MergeTextItemTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getMergeTextItem() {
            dataTable.ajax.reload();
        }

        $('#GetMergeTextItemButton').click(function (e) {
            e.preventDefault();
            getMergeTextItem();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getMergeTextItem();
            }
        });

    };
})(jQuery);

