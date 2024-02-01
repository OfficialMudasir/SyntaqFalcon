﻿(function () {
    $(function () {

        var _$entityVersionHistoriesTable = $('#EntityVersionHistoriesTable');
        var _entityVersionHistoriesService = abp.services.app.entityVersionHistories;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.EntityVersionHistories.Create'),
            edit: abp.auth.hasPermission('Pages.EntityVersionHistories.Edit'),
            'delete': abp.auth.hasPermission('Pages.EntityVersionHistories.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/EntityVersionHistories/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/EntityVersionHistories/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditEntityVersionHistoryModal'
        });


        var _viewEntityVersionHistoryModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/EntityVersionHistories/ViewentityVersionHistoryModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/EntityVersionHistories/_ViewentityVersionHistoryModal.js',
            modalClass: 'ViewEntityVersionHistoryModal'
        });

        //var _viewSfaAllVersionHistoryModal = new app.ModalManager({
        //    viewUrl: abp.appPath + 'Falcon/SfaAllVersionHistories/ViewsfaAllVersionHistoryModal',
        //    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/SfaAllVersionHistories/_ViewsfaAllVersionHistoryModal.js',
        //    modalClass: 'ViewSfaAllVersionHistoryModal'
        //});


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

        function showAuditLogDetails(auditLog) {

            _viewEntityVersionHistoryModal.open({ id: auditLog.entityVersionHistory.id });
        }

        var dataTable = _$entityVersionHistoriesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td").on("click", function () {
                    showAuditLogDetails(data);
                });
            },
            listAction: {
                ajaxFunction: _entityVersionHistoriesService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#EntityVersionHistoriesTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        versionNameFilter: $('#VersionNameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val(),
                        userNameFilter: $('#UserNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        element: $('<div/>')
                            .addClass('text-center')
                            .append(
                                $('<button/>')
                                    .addClass('btn btn-secondary btn-icon btn-sm')
                                    .attr('title', app.localize('EntityChangeDetail'))
                                    .append($('<i/>').addClass('fa fa-search'))
                            )
                            .click(function () {

                            }),
                    },
                },
                {
                    targets: 1,
                    data: "entityVersionHistory.name",
                    name: "name"
                },
                {
                    targets: 2,
                    data: "entityVersionHistory.version",
                    name: "version"
                },
                {
                    targets: 3,
                    data: "entityVersionHistory.previousVersion",
                    name: "previousVersion"
                },
                {
                    targets: 4,
                    data: "entityVersionHistory.type",
                    name: "type"
                },
                {
                    targets: 5,
                    data: "userName",
                    name: "userFk.name"
                },
                {
                    targets: 6,
                    orderable: false,
                    data: "entityVersionHistory.creationTime",
                    render: function (data, type, row) {

                        var dt = new Date(row.entityVersionHistory.creationTime);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        data = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

                        return data;
                    }
                }
            ]
        });

        function getEntityVersionHistories() {
            dataTable.ajax.reload();
        }

        function deleteEntityVersionHistory(entityVersionHistory) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _entityVersionHistoriesService.delete({
                            id: entityVersionHistory.id
                        }).done(function () {
                            getEntityVersionHistories(true);
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

        $('#CreateNewEntityVersionHistoryButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _entityVersionHistoriesService
                .getEntityVersionHistoriesToExcel({
                    filter: $('#EntityVersionHistoriesTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    versionNameFilter: $('#VersionNameFilterId').val(),
                    descriptionFilter: $('#DescriptionFilterId').val(),
                    userNameFilter: $('#UserNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditEntityVersionHistoryModalSaved', function () {
            getEntityVersionHistories();
        });

        $('#GetEntityVersionHistoriesButton').click(function (e) {
            e.preventDefault();
            getEntityVersionHistories();
        });

        $('#clearbtn').click(function () {
            $('#EntityVersionHistoriesTableFilter').val('');
            getEntityVersionHistories();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getEntityVersionHistories();
            }
        });



    });
})();
