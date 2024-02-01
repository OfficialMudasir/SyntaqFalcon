(function ($) {
    app.modals.AppLookupTableModal = function () {

        var _modalManager;

        var _appJobsService = abp.services.app.appJobs;
        var _$appTable = $('#AppTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$appTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _appJobsService.getAllAppForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#AppTableFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 1,
                    data: "displayName"
                }
            ]
        });

        $('#AppTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getApp() {
            dataTable.ajax.reload();
        }

        $('#GetAppButton').click(function (e) {
            e.preventDefault();
            getApp();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getApp();
            }
        });

    };
})(jQuery);

