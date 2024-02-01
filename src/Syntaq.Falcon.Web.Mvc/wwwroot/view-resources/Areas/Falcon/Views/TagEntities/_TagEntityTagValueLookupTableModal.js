(function ($) {
    app.modals.TagValueLookupTableModal = function () {

        var _modalManager;

        var _tagEntitiesService = abp.services.app.tagEntities;
        var _$tagValueTable = $('#TagValueTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$tagValueTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tagEntitiesService.getAllTagValueForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#TagValueTableFilter').val()
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

        $('#TagValueTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getTagValue() {
            dataTable.ajax.reload();
        }

        $('#GetTagValueButton').click(function (e) {
            e.preventDefault();
            getTagValue();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getTagValue();
            }
        });

    };
})(jQuery);

