(function ($) {
    app.modals.FormsFolderLookupTableModal = function () {

        var _modalManager;

        var _formsService = abp.services.app.forms;
        var _$formsFolderTable = $('#FormsFolderTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$formsFolderTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _formsService.getAllFormsFolderForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#FormsFolderTableFilter').val()
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

        $('#FormsFolderTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getFormsFolder() {
            dataTable.ajax.reload();
        }

        $('#GetFormsFolderButton').click(function (e) {
            e.preventDefault();
            getFormsFolder();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getFormsFolder();
            }
        });

    };
})(jQuery);

