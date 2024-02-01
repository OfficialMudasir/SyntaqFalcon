(function () {
    $(function () {

        var _$projectEnvironmentsTable = $('#ProjectEnvironmentsTable');
        var _projectEnvironmentsService = abp.services.app.projectEnvironments;

        var $selectedDate = {
            startDate: null,
            endDate: null,
        }

        $('.date-picker').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('MM/DD/YYYY'));
        });

        $('.startDate').daterangepicker({
            autoUpdateInput: false,
            singleDatePicker: true,
            locale: abp.localization.currentLanguage.name,
            format: 'L',
        }, (date) => {
            $selectedDate.startDate = date;
        }).on('cancel.daterangepicker', function (ev, picker) {
            $(this).val("");
            $selectedDate.startDate = null;
        });

        $('.endDate').daterangepicker({
            autoUpdateInput: false,
            singleDatePicker: true,
            locale: abp.localization.currentLanguage.name,
            format: 'L',
        }, (date) => {
            $selectedDate.endDate = date;
        }).on('cancel.daterangepicker', function (ev, picker) {
            $(this).val("");
            $selectedDate.endDate = null;
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.ProjectEnvironments.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectEnvironments.Edit'),
            'delete': abp.auth.hasPermission('Pages.ProjectEnvironments.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectEnvironments/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectEnvironments/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectEnvironmentModal'
        });


        var _viewProjectEnvironmentModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectEnvironments/ViewprojectEnvironmentModal',
            modalClass: 'ViewProjectEnvironmentModal'
        });




        var getDateFilter = function (element) {
            if ($selectedDate.startDate == null) {
                return null;
            }
            return $selectedDate.startDate.format("YYYY-MM-DDT00:00:00Z");
        }

        var getMaxDateFilter = function (element) {
            if ($selectedDate.endDate == null) {
                return null;
            }
            return $selectedDate.endDate.format("YYYY-MM-DDT23:59:59Z");
        }

        var dataTable = _$projectEnvironmentsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3)").on("click", function () {
                    _createOrEditModal.open({ id: data.projectEnvironment.id });
                });
            },
            listAction: {
                ajaxFunction: _projectEnvironmentsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectEnvironmentsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val(),
                        environmentTypeFilter: $('#EnvironmentTypeFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    responsivePriority: 1,
                    width: 120,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            //{
                            //                          text: app.localize('View'),

                            //                          action: function (data) {
                            //                              _viewProjectEnvironmentModal.open({ id: data.record.projectEnvironment.id });
                            //                          }
                            //                  },
                            {
                                text: app.localize('Edit'),

                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.projectEnvironment.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),

                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectEnvironment(data.record.projectEnvironment);
                                }
                            }]
                    }
                },
                {
                    targets: 0,
                    data: "projectEnvironment.name",
                    name: "name",
                    render: function (data, type, row) {
                        data = `<img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/environment.png"></i> ${data}`;
                        return data;
                    }
                },
                {
                    targets: 1,
                    data: "projectEnvironment.description",
                    name: "description"
                },
                {
                    targets: 2,
                    data: "projectEnvironment.environmentType",
                    name: "environmentType",
                    render: function (environmentType) {
                        return app.localize('Enum_ProjectEnvironmentType_' + environmentType);
                    }

                }
            ]
        });

        function getProjectEnvironments() {
            dataTable.ajax.reload();
        }

        function deleteProjectEnvironment(projectEnvironment) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectEnvironmentsService.delete({
                            id: projectEnvironment.id
                        }).done(function () {
                            getProjectEnvironments(true);
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

        $('#CreateNewProjectEnvironmentButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _projectEnvironmentsService
                .getProjectEnvironmentsToExcel({
                    filter: $('#ProjectEnvironmentsTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    descriptionFilter: $('#DescriptionFilterId').val(),
                    environmentTypeFilter: $('#EnvironmentTypeFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditProjectEnvironmentModalSaved', function () {
            getProjectEnvironments();
        });

        $('#GetProjectEnvironmentsButton').click(function (e) {
            e.preventDefault();
            getProjectEnvironments();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getProjectEnvironments();
            }
        });

        $('#clearbtn').click(function () {
            $('#ProjectEnvironmentsTableFilter').val('');
            getProjectEnvironments();

        });
    });
})();
