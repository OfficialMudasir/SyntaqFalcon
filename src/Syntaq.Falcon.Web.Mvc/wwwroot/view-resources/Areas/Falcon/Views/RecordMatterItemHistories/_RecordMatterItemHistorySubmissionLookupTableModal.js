(function ($) {
    app.modals.SubmissionLookupTableModal = function () {

        var _modalManager;

        var _recordMatterItemHistoriesService = abp.services.app.recordMatterItemHistories;
        var _$submissionTable = $('#SubmissionTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$submissionTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterItemHistoriesService.getAllSubmissionForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#SubmissionTableFilter').val()
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

        $('#SubmissionTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getSubmission() {
            dataTable.ajax.reload();
        }

        $('#GetSubmissionButton').click(function (e) {
            e.preventDefault();
            getSubmission();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getSubmission();
            }
        });

    };
})(jQuery);

