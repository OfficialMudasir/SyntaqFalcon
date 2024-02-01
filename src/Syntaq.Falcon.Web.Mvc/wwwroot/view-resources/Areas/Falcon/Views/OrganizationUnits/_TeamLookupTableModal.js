﻿(function ($) {
    app.modals.TeamLookupTableModal = function () {

        var _modalManager;

        var _organizationUnitService = abp.services.app.organizationUnit;
        var _$teamTable = $('#TeamTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };


        var dataTable = _$teamTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _organizationUnitService.getOrganizationUnits,
                inputFilter: function () {
                    return {
                        filter: $('#TeamTableFilter').val()
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

        $('#TeamTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getTeam() {
            dataTable.ajax.reload();
        }

        $('#GetTeamButton').click(function (e) {
            e.preventDefault();
            getTeam();
        });

        $('#SelectButton').click(function (e) {
            e.preventDefault();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getTeam();
            }
        });

    };
})(jQuery);