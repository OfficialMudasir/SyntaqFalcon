(function ($) {
    app.modals.FoldersLookupTableModal = function () {

        var _modalManager;

        var _foldersService = abp.services.app.folders;
        var _$folderTable = $('#FoldersTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var dataTable = _$folderTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _foldersService.getAllFoldersForLookupTable,
                inputFilter: function () {
                    return {
                        id: "00000000-0000-0000-0000-000000000000",
                        type: "R",
                        filter: $('#FoldersTableFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 0,
                    data: "displayName",
                    render: function (data, type, row) {

                        data += "<span class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></span>";
                        return data;

                    }
                }
                //,
                //{
                //    width: 120,
                //    targets: 1,
                //    data: null,
                //    orderable: false,
                //    autoWidth: false,
                //    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                //}
            ]
        });

        $('#FoldersTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getFolders() {
            dataTable.ajax.reload();
        }

        $('#GetFoldersButton').click(function (e) {
            e.preventDefault();
            getFolders();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getFolders();
            }
        });

    };
})(jQuery);

