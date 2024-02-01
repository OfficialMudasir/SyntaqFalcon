(function ($) {
    app.modals.TagLookupTableModal = function () {

        var _modalManager;

        var _tagEntityTypesService = abp.services.app.tagEntityTypes;
        var _$tagTable = $('#TagTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$tagTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tagEntityTypesService.getAllTagForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#TagTableFilter').val()
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

        $('#TagTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getTag() {
            dataTable.ajax.reload();
        }

        $('#GetTagButton').click(function (e) {
            e.preventDefault();
            getTag();
        });

        $('#tag_clearbtn').click(function () {
            $('#TagTableFilter').val('');
            getTag();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getTag();
            }
        });

    };
})(jQuery);

