﻿(function ($) {
    app.modals.RecordPolicyLookupTableModal = function () {

        var _modalManager;

        var _recordPolicyActionsService = abp.services.app.recordPolicyActions;
        var _$recordPolicyTable = $('#RecordPolicyTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$recordPolicyTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordPolicyActionsService.getAllRecordPolicyForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#RecordPolicyTableFilter').val()
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

        $('#RecordPolicyTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getRecordPolicy() {
            dataTable.ajax.reload();
        }

        $('#GetRecordPolicyButton').click(function (e) {
            e.preventDefault();
            getRecordPolicy();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordPolicy();
            }
        });

    };
})(jQuery);
