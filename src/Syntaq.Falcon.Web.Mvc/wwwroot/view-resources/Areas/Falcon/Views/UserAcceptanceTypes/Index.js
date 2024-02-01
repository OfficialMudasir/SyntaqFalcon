(function () {
    $(function () {

        var _$userAcceptanceTypesTable = $('#UserAcceptanceTypesTable');
        var _userAcceptanceTypesService = abp.services.app.userAcceptanceTypes;

        $('.date-picker').datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.UserAcceptanceTypes.Create'),
            edit: abp.auth.hasPermission('Pages.UserAcceptanceTypes.Edit'),
            'delete': abp.auth.hasPermission('Pages.UserAcceptanceTypes.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptanceTypes/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptanceTypes/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditUserAcceptanceTypeModal'
        });





        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var dataTable = _$userAcceptanceTypesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {

                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4),td:nth-child(5),td:nth-child(6)").on("click", function () {
                    _createOrEditModal.open({ id: data.userAcceptanceType.id });
                });


            },
            listAction: {
                ajaxFunction: _userAcceptanceTypesService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#UserAcceptanceTypesTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        activeFilter: $('#ActiveFilterId').val(),
                        templateNameFilter: $('#TemplateNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    width: 120,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.userAcceptanceType.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteUserAcceptanceType(data.record.userAcceptanceType);
                                }
                            }]
                    }
                },
                {
                    targets: 0,
                    data: "userAcceptanceType.name",
                    name: "name"
                },
                {
                    targets: 1,
                    data: "userAcceptanceType.active",
                    name: "active",
                    render: function (active) {
                        if (active) {
                            return '<div class="activeAcceptanceTypeSign"><i class="fa fa-check-circle text-success" title="True"></i></div>';
                        }
                        return '<div class=""><i class="fa fa-times-circle" title="False"></i></div>';
                    }

                },
                {
                    targets: 2,
                    data: "templateName",
                    name: "templateFk.name"
                }
            ]
        });


        function getUserAcceptanceTypes() {
            dataTable.ajax.reload();
        }

        function deleteUserAcceptanceType(userAcceptanceType) {
            abp.message.confirm(
                app.localize('AreYouSure'),
                '',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userAcceptanceTypesService.delete({
                            id: userAcceptanceType.id
                        }).done(function () {
                            getUserAcceptanceTypes(true);
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

        $('#CreateNewUserAcceptanceTypeButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _userAcceptanceTypesService
                .getUserAcceptanceTypesToExcel({
                    filter: $('#UserAcceptanceTypesTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    activeFilter: $('#ActiveFilterId').val(),
                    templateNameFilter: $('#TemplateNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditUserAcceptanceTypeModalSaved', function () {
            getUserAcceptanceTypes();
        });

        $('#GetUserAcceptanceTypesButton').click(function (e) {
            e.preventDefault();
            getUserAcceptanceTypes();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getUserAcceptanceTypes();
            }
        });

    });
})();  