(function () {
    $(function () {

        var _$vouchersTable = $('#VouchersTable');
        var _vouchersService = abp.services.app.vouchers;

        $('.date-picker').datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Vouchers.Create'),
            edit: abp.auth.hasPermission('Pages.Vouchers.Edit'),
            'delete': abp.auth.hasPermission('Pages.Vouchers.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Vouchers/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Vouchers/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditVoucherModal'
        });

        //var _viewVoucherModal = new app.ModalManager({
        //         viewUrl: abp.appPath + 'Falcon/Vouchers/ViewvoucherModal',
        //          modalClass: 'ViewVoucherModal'
        //     });

        var _VoucherUsageModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Vouchers/ViewVoucherUsageLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Vouchers/_ViewVoucherUsageLookupTableModal.js',
            modalClass: 'ViewVoucherUsageLookupTableModal'
        });

        var getDateFilter = function (element) {
            if (element) {
                return element;
            }
            return null;

            //if (element.data("DateTimePicker") == undefined) { return; }
            //if (element.data("DateTimePicker").date() == null) {
            //    return null;
            //}
            //return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var dataTable = _$vouchersTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {

                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4),td:nth-child(5),td:nth-child(6)").on("click", function () {
                    _createOrEditModal.open({ id: data.voucher.id });
                });

                $(row).find("[name='ViewVoucherUsageLink']").on("click", function (event) {
                    _VoucherUsageModal.open({ id: data.voucher.id });
                    event.stopPropagation();
                });

                $(row).find("[name='EditVoucherLink']").on("click", function (event) {
                    _createOrEditModal.open({ id: data.voucher.id });
                    event.stopPropagation();
                });
                $(row).find("[name='DeleteVoucherLink']").on("click", function (event) {
                    deleteVoucher({ id: data.voucher.id });
                    event.stopPropagation();
                });


            },
            listAction: {
                ajaxFunction: _vouchersService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#VouchersTableFilter').val(),
                        keyFilter: $('#KeyFilterId').val(),
                        minExpiryFilter: $('#MinExpiryFilterId').val(),
                        maxExpiryFilter: $('#MaxExpiryFilterId').val(),
                        minNoOfUsesFilter: $('#MinNoOfUsesFilterId').val(),
                        maxNoOfUsesFilter: $('#MaxNoOfUsesFilterId').val(),
                        discountTypeFilter: $('#DiscountTypeFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    responsivePriority: 1,
                    targets: 6,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('View Usage'),
                                action: function (data) {
                                    _VoucherUsageModal.open({ id: data.record.voucher.id });
                                }
                            },
                            {
                                text: app.localize('Edit'),
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.voucher.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                action: function (data) {
                                    deleteVoucher({ id: data.record.voucher.id });
                                }
                            }
                        ]
                    }
                },
                {
                    targets: 0,
                    data: "voucher.key",
                    name: "key"
                },
                {
                    targets: 1,
                    data: "voucher.value",
                    name: "value"
                },
                {
                    targets: 2,
                    data: "voucher.expiry",
                    name: "expiry",
                    render: function (expiry) {
                        if (expiry) {
                            return moment(expiry).format('L');
                        }
                        return "";
                    }
                },
                {
                    targets: 3,
                    data: "voucher.noOfUses",
                    name: "noOfUses"
                },
                {
                    targets: 4,
                    data: "voucher.description",
                    name: "description"
                },
                {
                    targets: 5,
                    data: "voucher.discountType",
                    name: "discountType"
                }
            ]
        });

        function getVouchers() {
            dataTable.ajax.reload();
        }

        function deleteVoucher(voucher) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _vouchersService.delete({
                            id: voucher.id
                        }).done(function () {
                            getVouchers(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        $('#CreateNewVoucherButton').click(function () {
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditVoucherModalSaved', function () {
            getVouchers();
        });

        $('#GetVouchersButton').click(function (e) {
            e.preventDefault();
            getVouchers();
        });

        $('#clearbtn').click(function () {
            $('#VouchersTableFilter').val('');
            getVouchers();
        });
        $("#VouchersTableFilter").keyup(function (event) {
            // On enter 
            if (event.keyCode === 13) {
                getVouchers();
            }
        });
    });
})();