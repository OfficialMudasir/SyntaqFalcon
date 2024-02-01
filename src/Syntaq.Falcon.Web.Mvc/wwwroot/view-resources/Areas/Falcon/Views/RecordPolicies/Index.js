(function () {
    $(function () {

        var _$recordPoliciesTable = $('#RecordPoliciesTable');
        var _recordPoliciesService = abp.services.app.recordPolicies;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordPolicies.Create'),
            edit: abp.auth.hasPermission('Pages.RecordPolicies.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordPolicies.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordPolicies/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordPolicies/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRecordPolicyModal'
        });


        var _viewRecordPolicyModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordPolicies/ViewrecordPolicyModal',
            modalClass: 'ViewRecordPolicyModal'
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

        var dataTable = _$recordPoliciesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,

            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2)").on("click", function () {
                    _createOrEditModal.open({ id: data.recordPolicy.id });
                });
            },

            listAction: {
                ajaxFunction: _recordPoliciesService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#RecordPoliciesTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        activeFilter: $('#ActiveFilterId').val(),
                        minAppliedTenantIdFilter: $('#MinAppliedTenantIdFilterId').val(),
                        maxAppliedTenantIdFilter: $('#MaxAppliedTenantIdFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 2,
                    data: null,
                    orderable: false,
                    autoWidth: true,
                    defaultContent: '',
                    rowAction: {
                        text:
                            ' <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
                            app.localize('Actions') +
                            '</span> <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('View'),
                                action: function (data) {
                                    _viewRecordPolicyModal.open({ id: data.record.recordPolicy.id });
                                }
                            },
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.recordPolicy.id });
                                }
                            }
                            //{
                            //                      text: app.localize('Delete'),
                            //                      iconStyle: 'far fa-trash-alt mr-2',
                            //                      visible: function () {
                            //                          return _permissions.delete;
                            //                      },
                            //                      action: function (data) {
                            //                          deleteRecordPolicy(data.record.recordPolicy);
                            //                      }
                            //                      }]
                        ]
                    }
                },
                {
                    targets: 0,
                    data: "recordPolicy.name",
                    name: "name"
                },
                {
                    targets: 1,
                    data: "appliedTenantName",
                    name: "apliedTenant"
                }
            ]
        });

        function getRecordPolicies() {
            dataTable.ajax.reload();
        }

        function deleteRecordPolicy(recordPolicy) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _recordPoliciesService.delete({
                            id: recordPolicy.id
                        }).done(function () {
                            getRecordPolicies(true);
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

        $('#CreateNewRecordPolicyButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _recordPoliciesService
                .getRecordPoliciesToExcel({
                    filter: $('#RecordPoliciesTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    activeFilter: $('#ActiveFilterId').val(),
                    minAppliedTenantIdFilter: $('#MinAppliedTenantIdFilterId').val(),
                    maxAppliedTenantIdFilter: $('#MaxAppliedTenantIdFilterId').val(),
                    descriptionFilter: $('#DescriptionFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditRecordPolicyModalSaved', function () {
            getRecordPolicies();
        });


        //abp.event.on('app.createOrEditRecordPolicyActionModalSaved', function () {
        //    getRecordPolicies();
        //});
        $('#clearbtn').click(function () {
            $('#RecordPoliciesTableFilter').val('');
            getForms();
        });


        $('#GetRecordPoliciesButton').click(function (e) {
            e.preventDefault();
            getRecordPolicies();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getRecordPolicies();
            }
        });



    });
})();
