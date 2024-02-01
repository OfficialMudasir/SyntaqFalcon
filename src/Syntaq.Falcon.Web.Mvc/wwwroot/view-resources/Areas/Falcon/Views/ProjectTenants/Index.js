﻿(function () {
    $(function () {
        var _$projectTenantsTable = $('#ProjectTenantsTable');
        var _projectTenantsService = abp.services.app.projectTenants;

        var $selectedDate = {
            startDate: null,
            endDate: null,
        };

        $('.date-picker').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('MM/DD/YYYY'));
        });

        $('.startDate')
            .daterangepicker(
                {
                    autoUpdateInput: false,
                    singleDatePicker: true,
                    locale: abp.localization.currentLanguage.name,
                    format: 'L',
                },
                (date) => {
                    $selectedDate.startDate = date;
                }
            )
            .on('cancel.daterangepicker', function (ev, picker) {
                $(this).val('');
                $selectedDate.startDate = null;
            });

        $('.endDate')
            .daterangepicker(
                {
                    autoUpdateInput: false,
                    singleDatePicker: true,
                    locale: abp.localization.currentLanguage.name,
                    format: 'L',
                },
                (date) => {
                    $selectedDate.endDate = date;
                }
            )
            .on('cancel.daterangepicker', function (ev, picker) {
                $(this).val('');
                $selectedDate.endDate = null;
            });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.ProjectTenants.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectTenants.Edit'),
            delete: abp.auth.hasPermission('Pages.ProjectTenants.Delete'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTenants/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTenants/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectTenantModal',
        });

        var _viewProjectTenantModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTenants/ViewprojectTenantModal',
            modalClass: 'ViewProjectTenantModal',
        });

        var getDateFilter = function (element) {
            if ($selectedDate.startDate == null) {
                return null;
            }
            return $selectedDate.startDate.format('YYYY-MM-DDT00:00:00Z');
        };

        var getMaxDateFilter = function (element) {
            if ($selectedDate.endDate == null) {
                return null;
            }
            return $selectedDate.endDate.format('YYYY-MM-DDT23:59:59Z');
        };

        var dataTable = _$projectTenantsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _projectTenantsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectTenantsTableFilter').val(),
                        minSubscriberTenantIdFilter: $('#MinSubscriberTenantIdFilterId').val(),
                        maxSubscriberTenantIdFilter: $('#MaxSubscriberTenantIdFilterId').val(),
                        projectIdFilter: $('#ProjectIdFilterId').val(),
                        enabledFilter: $('#EnabledFilterId').val(),
                        projectEnvironmentNameFilter: $('#ProjectEnvironmentNameFilterId').val(),
                    };
                },
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0,
                },
                {
                    width: 120,
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('View'),
                                iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _viewProjectTenantModal.open({ id: data.record.projectTenant.id });
                                },
                            },
                            {
                                text: app.localize('Edit'),
                                iconStyle: 'far fa-edit mr-2',
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.projectTenant.id });
                                },
                            },
                            {
                                text: app.localize('Delete'),
                                iconStyle: 'far fa-trash-alt mr-2',
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectTenant(data.record.projectTenant);
                                },
                            },
                        ],
                    },
                },
                {
                    targets: 2,
                    data: 'projectTenant.subscriberTenantId',
                    name: 'subscriberTenantId',
                },
                {
                    targets: 3,
                    data: 'projectTenant.projectId',
                    name: 'projectId',
                },
                {
                    targets: 4,
                    data: 'projectTenant.enabled',
                    name: 'enabled',
                    render: function (enabled) {
                        if (enabled) {
                            return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    },
                },
                {
                    targets: 5,
                    data: 'projectEnvironmentName',
                    name: 'projectEnvironmentFk.name',
                },
            ],
        });

        function getProjectTenants() {
            dataTable.ajax.reload();
        }

        function deleteProjectTenant(projectTenant) {
            abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                if (isConfirmed) {
                    _projectTenantsService
                        .delete({
                            id: projectTenant.id,
                        })
                        .done(function () {
                            getProjectTenants(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                }
            });
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

        $('#CreateNewProjectTenantButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _projectTenantsService
                .getProjectTenantsToExcel({
                    filter: $('#ProjectTenantsTableFilter').val(),
                    minSubscriberTenantIdFilter: $('#MinSubscriberTenantIdFilterId').val(),
                    maxSubscriberTenantIdFilter: $('#MaxSubscriberTenantIdFilterId').val(),
                    projectIdFilter: $('#ProjectIdFilterId').val(),
                    enabledFilter: $('#EnabledFilterId').val(),
                    projectEnvironmentNameFilter: $('#ProjectEnvironmentNameFilterId').val(),
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditProjectTenantModalSaved', function () {
            getProjectTenants();
        });

        $('#GetProjectTenantsButton').click(function (e) {
            e.preventDefault();
            getProjectTenants();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getProjectTenants();
            }
        });
    });
})();
