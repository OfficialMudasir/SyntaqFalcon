(function ($) {
    app.modals.VoucherLookupTableModal = function () {

        var _modalManager;

        var _voucherEntitiesService = abp.services.app.voucherEntities;
        var _$voucherTable = $('#VoucherTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$voucherTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _voucherEntitiesService.getAllVoucherForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#VoucherTableFilter').val()
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

        $('#VoucherTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getVoucher() {
            dataTable.ajax.reload();
        }

        $('#GetVoucherButton').click(function (e) {
            e.preventDefault();
            getVoucher();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getVoucher();
            }
        });

    };
})(jQuery);

