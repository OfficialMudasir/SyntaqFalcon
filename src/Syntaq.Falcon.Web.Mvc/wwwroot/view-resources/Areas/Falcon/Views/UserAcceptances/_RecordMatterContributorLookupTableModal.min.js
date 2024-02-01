(function ($) {
    app.modals.RecordMatterContributorLookupTableModal = function () {

        var _modalManager;

        var _userAcceptancesService = abp.services.app.userAcceptances;
        var _$recordMatterContributorTable = $('#RecordMatterContributorTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordMatterContributorTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _userAcceptancesService.getAllRecordMatterContributorForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordMatterContributorTableFilter').val()
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

        $('#RecordMatterContributorTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getRecordMatterContributor() {
            dataTable.ajax.reload();
        }

        $('#GetRecordMatterContributorButton').click(function (e) {
            e.preventDefault();
            getRecordMatterContributor();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordMatterContributor();
            }
        });

    };
})(jQuery);

