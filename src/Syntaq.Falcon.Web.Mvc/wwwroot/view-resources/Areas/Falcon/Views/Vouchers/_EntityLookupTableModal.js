﻿(function ($) {
    app.modals.EntityLookupTableModal = function () {

        var _modalManager;

        //var _recordMattersService = abp.services.app.recordMatters;
        var _vouchersService = abp.services.app.vouchers;
        //var _$recordTable = $('#RecordTable');
        var _$EntityTable = $('#EntityTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var dataTable = _$EntityTable.DataTable({  //gets to here, opens picker modal with no data
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _vouchersService.getAllEntitysForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#EntityTableFilter').val()
                    };
                }
            },
            columnDefs: [
                //{
                //    targets: 0,
                //    width: 120,
                //    data: null,
                //    orderable: false,
                //    autoWidth: false,
                //    defaultContent: "<div class=\"text-center\"><input id='selectbtn' class='btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x' type='button' width='25px' value='" + app.localize('Select') + "' /></div>"
                //},
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 0,
                    data: "displayName",
                    render: function (data, type, row) {
                        data += '<span class=\"text-center\"><input id="selectbtn" class="btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x" type="button" width="25px" value="' + app.localize('Select') + '" /></span>';
                        return data;
                    }
                }
            ]
        });

        $('#EntityTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getEntity() {
            dataTable.ajax.reload();
        }

        $('#GetEntityButton').click(function (e) {
            e.preventDefault();
            getEntity();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getEntity();
            }
        });

    };
})(jQuery);