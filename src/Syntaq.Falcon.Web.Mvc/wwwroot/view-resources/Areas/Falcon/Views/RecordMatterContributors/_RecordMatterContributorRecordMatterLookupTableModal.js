(function ($) {
    app.modals.RecordMatterLookupTableModal = function () {

        var _modalManager;

        var _recordMatterContributorsService = abp.services.app.recordMatterContributors;
        var _$recordMatterTable = $('#RecordMatterTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordMatterTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterContributorsService.getAllRecordMatterForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordMatterTableFilter').val()
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

