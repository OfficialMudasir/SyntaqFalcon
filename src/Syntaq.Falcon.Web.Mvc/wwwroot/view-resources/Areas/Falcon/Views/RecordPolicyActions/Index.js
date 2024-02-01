﻿(function () {
    $(function () {

        var _$RecordPolicyActionsTable = $('#RecordPolicyActionsTable');
        var _recordPolicyActionsService = abp.services.app.recordPolicyActions;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordPolicyActions.Create'),
            edit: abp.auth.hasPermission('Pages.RecordPolicyActions.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordPolicyActions.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordPolicyActions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordPolicyActions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditrecordPolicyActionModal'
        });


        var _viewRecordPolicyActionModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordPolicyActions/ViewRecordPolicyActionModal',
            modalClass: 'ViewRecordPolicyActionModal'
        });




        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var getMaxDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT23:59:59Z");
        }

        var dataTable = _$RecordPolicyActionsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4),td:nth-child(5),td:nth-child(6)").on("click", function () {
                    _createOrEditModal.open({ id: data.recordPolicyAction.id });
                });
            },
            listAction: {
                ajaxFunction: _recordPolicyActionsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#RecordPolicyActionsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        minAppliedTenantIdFilter: $('#MinAppliedTenantIdFilterId').val(),
                        maxAppliedTenantIdFilter: $('#MaxAppliedTenantIdFilterId').val(),
                        minExpireDaysFilter: $('#MinExpireDaysFilterId').val(),
                        maxExpireDaysFilter: $('#MaxExpireDaysFilterId').val(),
                        activeFilter: $('#ActiveFilterId').val(),
                        minTypeFilter: $('#MinTypeFilterId').val(),
                        maxTypeFilter: $('#MaxTypeFilterId').val(),
                        minRecordStatusFilter: $('#MinRecordStatusFilterId').val(),
                        maxRecordStatusFilter: $('#MaxRecordStatusFilterId').val(),
                        recordPolicyNameFilter: $('#RecordPolicyNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 6,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('View'),
                                action: function (data) {
                                    _viewRecordPolicyActionModal.open({ id: data.record.recordPolicyAction.id });
                                }
                            },
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.recordPolicyAction.id });
                                }
                            }
                            //{
                            //                      text: app.localize('Delete'),
                            //                      iconStyle: 'far fa-trash-alt mr-2',
                            //                      visible: function () {
                            //                          return _permissions.delete;
                            //                      },
                            //                      action: function (data) {
                            //                          deleteRecordPolicyAction(data.record.RecordPolicyAction);
                            //                      }
                            //                  }]
                        ]
                    }
                },
                {
                    targets: 0,
                    data: "recordPolicyAction.name",
                    name: "name"
                },
                {
                    targets: 1,
                    orderable: false,
                    data: "appliedTenantName",
                    name: "appliedTenant"
                },
                {
                    targets: 2,
                    data: "recordPolicyAction.expireDays",
                    name: "expireDays"
                },
                {
                    targets: 3,
                    data: "recordPolicyAction.type",
                    name: "type",
                    render: function (type) {
                        return app.localize('Enum_RecordPolicyActionType_' + type);
                    }
                },
                {
                    targets: 4,
                    data: "recordPolicyAction.recordStatus",
                    name: "recordStatus",
                    render: function (recordStatus) {
                        return app.localize('Enum_RecordStatusType_' + recordStatus);
                    }
                },
                {
                    targets: 5,
                    data: "recordPolicyAction.active",
                    name: "active",
                    render: function (active) {
                        if (active) {
                            return '<div class="text-center"><i class="fa fa-check kt--font-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    }

                }
            ]
        });

        function getRecordPolicyActions() {
            dataTable.ajax.reload();
        }

        function deleteRecordPolicyAction(RecordPolicyAction) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _RecordPolicyActionsService.delete({
                            id: RecordPolicyAction.id
                        }).done(function () {
                            getRecordPolicyActions(true);
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

        $('#CreateNewRecordPolicyActionButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _RecordPolicyActionsService
                .getRecordPolicyActionsToExcel({
                    filter: $('#RecordPolicyActionsTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    minAppliedTenantIdFilter: $('#MinAppliedTenantIdFilterId').val(),
                    maxAppliedTenantIdFilter: $('#MaxAppliedTenantIdFilterId').val(),
                    minExpireDaysFilter: $('#MinExpireDaysFilterId').val(),
                    maxExpireDaysFilter: $('#MaxExpireDaysFilterId').val(),
                    activeFilter: $('#ActiveFilterId').val(),
                    minTypeFilter: $('#MinTypeFilterId').val(),
                    maxTypeFilter: $('#MaxTypeFilterId').val(),
                    minRecordStatusFilter: $('#MinRecordStatusFilterId').val(),
                    maxRecordStatusFilter: $('#MaxRecordStatusFilterId').val(),
                    recordPolicyNameFilter: $('#RecordPolicyNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditRecordPolicyActionModalSaved', function () {
            getRecordPolicyActions();
        });

        $('#GetRecordPolicyActionsButton').click(function (e) {
            e.preventDefault();
            getRecordPolicyActions();
        });

        $('#clearbtn').click(function () {
            $('#RecordPolicyActionsTableFilter').val('');
            getRecordPolicyActions();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordPolicyActions();
            }
        });



    });
})();