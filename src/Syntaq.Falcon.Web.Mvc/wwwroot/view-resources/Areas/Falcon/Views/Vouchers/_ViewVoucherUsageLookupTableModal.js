﻿(function ($) {
    app.modals.ViewVoucherUsageLookupTableModal = function () {

        var _modalManager;

        var _vouchersService = abp.services.app.vouchers;
        var _voucherUsageTable = $('#VoucherUsageTable');


        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var dataTable = _voucherUsageTable.DataTable({ 
            paging: true,
            serverSide: true,
            processing: true,
            //createdRow: function (row, data, dataIndex) {

            //    $(row).find("[name='ViewVoucherUsageLink']").on("click", function () {
            //        alert("view usage link clicked");
            //        _VoucherUsageModal.open({ entityid: data.id, entityname: data.name, entitytype: 'VoucherTemplate' });
            //    });

            //    $(row).find("[name='EditVoucherLink']").on("click", function () {
            //        //alert("edit voucher clicked");
            //        _createOrEditModal.open({ id: data.voucher.id });
            //    });

            //    //           $(row).find("[name='CopyVoucherLink']").on("click", function () {
            //    //               alert("copy voucher clicked");
            //    //               //var clipboard = new ClipboardJS("[name='CopyVoucherLink']");

            //    //               //For Debugging Copy
            //    ////clipboard.on('success', function (e) {
            //    ////    console.log(e);
            //    ////});
            //    ////clipboard.on('error', function (e) {
            //    ////    console.log(e);
            //    ////});
            //    //           });

            //    $(row).find("[name='DeleteVoucherLink']").on("click", function () {
            //        alert("delete voucher clicked!");
            //        //deleteVoucher(data);
            //    });
            //},

            listAction: {
                ajaxFunction: _vouchersService.getVoucherUsagesForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#vUsage_VoucherID').val()
                        //dateRedeemedFilter: formatDateRedeemed($('#dateRedeemed'))
                    };
                }
            },
            columnDefs: [
                //{
                //    autoWidth: false,
                //    orderable: false,
                //    targets: 0,
                //    data: "displayName",
                //    render: function (data, type, row) {
                //        data += '<span class=\"text-center\"><input id="selectbtn" class="btn btn-sm btn-secondary pull-right m-btn m-btn--custom m-btn--label-accent m-btn--bolder   m-btn--outline-2x" type="button" width="25px" value="' + app.localize('Select') + '" /></span>';
                //        return data;
                //    }
                //}
                {
                    targets: 0,
                    data: "dateRedeemed",
                    name: "dateRedeemed",
                    id: "dateRedeemed",
                    render: function (dateRedeemed) {
                        dt = new Date(dateRedeemed);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        return dt;
                    }
                },
                {
                    targets: 1,
                    data: "entityName",
                    name: "entityName"
                },
                {
                    targets: 2,
                    data: "entityType",
                    name: "entityType",
                },
                {
                    targets: 3,
                    data: "userName",
                    name: "userName"
                },
                {
                    targets: 4,
                    data: "userEmail",
                    name: "userEmail"
                }
            ]
        });


        // DO WE NEED A SELECT BUTTON?
        $('#VoucherUsageTable tbody').on('click', '[id*=selectbtn]', function () {
            var data = dataTable.row($(this).parents('tr')).data();
            _modalManager.setResult(data);
            _modalManager.close();
        });

        function getVoucherUsage() {
            dataTable.ajax.reload();
        }

        $('#SearchVoucherUsageBtn').click(function (e) {
            e.preventDefault();
            getVoucherUsage();
        });

        //$('#SelectButton').click(function (e) {
        //    e.preventDefault();
        //});

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getVoucherUsage();
            }
        });

    };
})(jQuery);