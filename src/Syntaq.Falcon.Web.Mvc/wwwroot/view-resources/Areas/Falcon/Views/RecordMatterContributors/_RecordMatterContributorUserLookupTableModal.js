(function ($) {
    app.modals.UserLookupTableModal = function () {

        var _modalManager;

        var _recordMatterContributorsService = abp.services.app.recordMatterContributors;
        var _$userTable = $('#UserTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var dataTable = _$userTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,

            drawCallback: function () {
                jQuery('.dataTables_paginate > .pagination').find('li').addClass('page-item');
                jQuery('.dataTables_paginate > .pagination > li').find('a').addClass('page-link');
                jQuery('.bottom').addClass('row');
                jQuery('.dataTables_info').addClass('col-12');
                jQuery('.dataTables_length').addClass('col-2');
                jQuery('.dataTables_paginate').addClass('col-9');
            },

            listAction: {
                ajaxFunction: _recordMatterContributorsService.getAllUserForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#UserTableFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 3,
                    data: null,
                    orderable: false,
                    // autoWidth: false,
                    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 0,
                    data: "displayName"
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 1,
                    data: "surname"
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 2,
                    data: "email"
                }
            ]
        });

        $('#UserTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getUser() {
            dataTable.ajax.reload();
        }

        $('#GetUserButton').click(function (e) {
            e.preventDefault();
            getUser();
        });

        $('#clearbtn').click(function () {
            $('#UserTableFilter').val('');
            getUser();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getUser();
            }
        });

    };
})(jQuery);

