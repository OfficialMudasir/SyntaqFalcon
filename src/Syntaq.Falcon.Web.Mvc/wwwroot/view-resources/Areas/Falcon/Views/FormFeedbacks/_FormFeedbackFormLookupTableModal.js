(function ($) {
    app.modals.FormLookupTableModal = function () {

        var _modalManager;

        var _formFeedbacksService = abp.services.app.formFeedbacks;
        var _$formTable = $('#FormTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$formTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _formFeedbacksService.getAllFormForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#FormTableFilter').val()
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

        $('#FormTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getForm() {
            dataTable.ajax.reload();
        }

        $('#GetFormButton').click(function (e) {
            e.preventDefault();
            getForm();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getForm();
            }
        });

    };
})(jQuery);

