(function () {
    $(function () {

        var _$formFeedbacksTable = $('#FormFeedbacksTable');
        var _formFeedbacksService = abp.services.app.formFeedbacks;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.FormFeedbacks.Create'),
            edit: abp.auth.hasPermission('Pages.FormFeedbacks.Edit'),
            'delete': abp.auth.hasPermission('Pages.FormFeedbacks.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/FormFeedbacks/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormFeedbacks/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditFormFeedbackModal'
        });


        var _viewFormFeedbackModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/FormFeedbacks/ViewformFeedbackModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormFeedbacks/_ViewFormFeedbackModal.js',
            modalClass: 'ViewFormFeedbackModal'
        });




        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var dataTable = _$formFeedbacksTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,

            createdRow: function (row, data, dataIndex) {

                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4),td:nth-child(5),td:nth-child(6)").on("click", function () {
                    _viewFormFeedbackModal.open({ id: data.formFeedback.id });
                });


            },

            listAction: {
                ajaxFunction: _formFeedbacksService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#FormFeedbacksTableFilter').val(),
                        formNameFilter: $('#FormNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    width: 120,
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
                                    _viewFormFeedbackModal.open({ id: data.record.formFeedback.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteFormFeedback(data.record.formFeedback);
                                }
                            }]
                    }
                },
                {
                    targets: 0,
                    data: "formName",
                    name: "formFk.name"
                },
                {
                    targets: 1,
                    data: "userName",
                    name: "userName"
                },
                {
                    targets: 2,
                    orderable: false,
                    data: "email",
                    name: "email"
                },
                {
                    targets: 3,
                    orderable: true,
                    data: "rating",
                    name: "rating"
                },
                {
                    targets: 4,
                    orderable: false,
                    data: "comment",
                    name: "comment"
                },
                {
                    targets: 5,
                    data: null,
                    orderable: true,
                    name: "creationTime",
                    render: function (data, type, row) {
                        var dt = new Date(row.formFeedback.creationTime);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        var date = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        return date;
                    }
                }
            ]
        });

        function getFormFeedbacks() {
            dataTable.ajax.reload();
        }

        function deleteFormFeedback(formFeedback) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _formFeedbacksService.delete({
                            id: formFeedback.id
                        }).done(function () {
                            getFormFeedbacks(true);
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

        $('#CreateNewFormFeedbackButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _formFeedbacksService
                .getFormFeedbacksToExcel({
                    filter: $('#FormFeedbacksTableFilter').val(),
                    formNameFilter: $('#FormNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditFormFeedbackModalSaved', function () {
            getFormFeedbacks();
        });

        $('#GetFormFeedbacksButton').click(function (e) {
            e.preventDefault();
            getFormFeedbacks();
        });

        $('#clearbtn').click(function () {
            $('#FormFeedbacksTableFilter').val('');
            getFormFeedbacks();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getFormFeedbacks();
            }
        });



    });
})();