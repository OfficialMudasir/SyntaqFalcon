(function ($) {

    app.modals.UsersLookupTableModal = function () {

        var _modalManager;
        var _userService = abp.services.app.user;
        var _$formsFolderTable = $('#UsersTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var dataTable = _$formsFolderTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _userService.getUsers,
                inputFilter: function () {
                    return {
                        filter: $('#UsersTableFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 0,
                    data: "name"
                },
                {
                    width: 120,
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-secondary m-btn m-btn--custom m-btn--label-accent m-btn--bolder dropdown-toggle' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                }
            ]
        });

        $('#UsersTable tbody').on('click', '[id*=selectbtn]', function () {
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