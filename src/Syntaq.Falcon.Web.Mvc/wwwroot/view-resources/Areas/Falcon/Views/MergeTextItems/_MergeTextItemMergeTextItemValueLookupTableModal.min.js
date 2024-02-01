(function ($) {
    app.modals.MergeTextItemValueLookupTableModal = function () {

        var _modalManager;

        var _mergeTextItemsService = abp.services.app.mergeTextItems;
        var _$mergeTextItemValueTable = $('#MergeTextItemValueTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$mergeTextItemValueTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _mergeTextItemsService.getAllMergeTextItemValueForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#MergeTextItemValueTableFilter').val()
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

        $('#MergeTextItemValueTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getMergeTextItemValue() {
            dataTable.ajax.reload();
        }

        $('#GetMergeTextItemValueButton').click(function (e) {
            e.preventDefault();
            getMergeTextItemValue();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getMergeTextItemValue();
            }
        });

    };
})(jQuery);

