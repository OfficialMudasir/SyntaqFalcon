(function ($) {
    app.modals.TemplateLookupTableModal = function () {

        var _modalManager;

        var _userAcceptanceTypesService = abp.services.app.userAcceptanceTypes;
        var _$templateTable = $('#TemplateTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$templateTable.DataTable({
            paging: true,
            serverSide: true,
            autoWidth: true,
            processing: true,
            listAction: {
                ajaxFunction: _userAcceptanceTypesService.getAllTemplateForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#TemplateTableFilter').val()
                    };
                }
            },
            columnDefs: [
                //{
                //    targets: 0,
                //    data: null,
                //    orderable: false,
                //    autoWidth: true,
                //    defaultContent: "<div><input id='selectbtn' class='btn btn-success' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                //},
                {
                    autoWidth: true,
                    orderable: false,
                    targets: 0,
                    //data: "displayName",
                    render: function (data, type, row) {
                        return "<input id='selectbtn' class='btn btn-sm btn-success mr-4' type='button' width='25px' value='" + app.localize('Select') + "' /> <span class=' '>" + row.displayName + "</span>"
                    }
                }
            ]
        });

        $('#TemplateTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getTemplate() {
            dataTable.ajax.reload();
        }

        $('#GetTemplateButton').click(function (e) {
            e.preventDefault();
            getTemplate();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getTemplate();
            }
        });

    };
})(jQuery);

