(function ($) {
    app.modals.AppJobLookupTableModal = function () {

        var _modalManager;

        var _submissionsService = abp.services.app.submissions;
        var _$appJobTable = $('#AppJobTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$appJobTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _submissionsService.getAllAppJobForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#AppJobTableFilter').val()
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

        $('#AppJobTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getAppJob() {
            dataTable.ajax.reload();
        }

        $('#GetAppJobButton').click(function (e) {
            e.preventDefault();
            getAppJob();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getAppJob();
            }
        });

    };
})(jQuery);

