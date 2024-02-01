﻿(function () {
    $(function () {
        var _$projectReleasesTable = $('#ProjectReleasesTable');
        var _projectReleasesService = abp.services.app.projectReleases;

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
            create: abp.auth.hasPermission('Pages.ProjectReleases.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectReleases.Edit'),
            delete: abp.auth.hasPermission('Pages.ProjectReleases.Delete'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectReleases/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectReleases/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectReleaseModal',
        });

        var _viewProjectReleaseModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectReleases/ViewprojectReleaseModal',
            modalClass: 'ViewProjectReleaseModal',
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

        var dataTable = _$projectReleasesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _projectReleasesService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectReleasesTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        notesFilter: $('#NotesFilterId').val(),
                        projectIdFilter: $('#ProjectIdFilterId').val(),
                        requiredFilter: $('#RequiredFilterId').val(),
                        minVersionMajorFilter: $('#MinVersionMajorFilterId').val(),
                        maxVersionMajorFilter: $('#MaxVersionMajorFilterId').val(),
                        minVersionMinorFilter: $('#MinVersionMinorFilterId').val(),
                        maxVersionMinorFilter: $('#MaxVersionMinorFilterId').val(),
                        minVersionRevisionFilter: $('#MinVersionRevisionFilterId').val(),
                        maxVersionRevisionFilter: $('#MaxVersionRevisionFilterId').val(),
                        releaseTypeFilter: $('#ReleaseTypeFilterId').val(),
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
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('View'),
                                iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _viewProjectReleaseModal.open({ id: data.record.projectRelease.id });
                                },
                            },
                            {
                                text: app.localize('Edit'),
                                iconStyle: 'far fa-edit mr-2',
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.projectRelease.id });
                                },
                            },
                            {
                                text: app.localize('Delete'),
                                iconStyle: 'far fa-trash-alt mr-2',
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectRelease(data.record.projectRelease);
                                },
                            },
                        ],
                    },
                },
                {
                    targets: 2,
                    data: 'projectRelease.name',
                    name: 'name',
                },
                {
                    targets: 3,
                    data: 'projectRelease.notes',
                    name: 'notes',
                },
                {
                    targets: 4,
                    data: 'projectRelease.projectId',
                    name: 'projectId',
                },
                {
                    targets: 5,
                    data: 'projectRelease.required',
                    name: 'required',
                    render: function (required) {
                        if (required) {
                            return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    },
                },
                {
                    targets: 6,
                    data: 'projectRelease.versionMajor',
                    name: 'versionMajor',
                },
                {
                    targets: 7,
                    data: 'projectRelease.versionMinor',
                    name: 'versionMinor',
                },
                {
                    targets: 8,
                    data: 'projectRelease.versionRevision',
                    name: 'versionRevision',
                },
                {
                    targets: 9,
                    data: 'projectRelease.releaseType',
                    name: 'releaseType',
                    render: function (releaseType) {
                        return app.localize('Enum_ProjectReleaseType_' + releaseType);
                    },
                },
                {
                    targets: 10,
                    data: 'projectEnvironmentName',
                    name: 'projectEnvironmentFk.name',
                },
            ],
        });

        function getProjectReleases() {
            dataTable.ajax.reload();
        }

        function deleteProjectRelease(projectRelease) {
            abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                if (isConfirmed) {
                    _projectReleasesService
                        .delete({
                            id: projectRelease.id,
                        })
                        .done(function () {
                            getProjectReleases(true);
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

        $('#CreateNewProjectReleaseButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _projectReleasesService
                .getProjectReleasesToExcel({
                    filter: $('#ProjectReleasesTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    notesFilter: $('#NotesFilterId').val(),
                    projectIdFilter: $('#ProjectIdFilterId').val(),
                    requiredFilter: $('#RequiredFilterId').val(),
                    minVersionMajorFilter: $('#MinVersionMajorFilterId').val(),
                    maxVersionMajorFilter: $('#MaxVersionMajorFilterId').val(),
                    minVersionMinorFilter: $('#MinVersionMinorFilterId').val(),
                    maxVersionMinorFilter: $('#MaxVersionMinorFilterId').val(),
                    minVersionRevisionFilter: $('#MinVersionRevisionFilterId').val(),
                    maxVersionRevisionFilter: $('#MaxVersionRevisionFilterId').val(),
                    releaseTypeFilter: $('#ReleaseTypeFilterId').val(),
                    projectEnvironmentNameFilter: $('#ProjectEnvironmentNameFilterId').val(),
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditProjectReleaseModalSaved', function () {
            getProjectReleases();
        });

        $('#GetProjectReleasesButton').click(function (e) {
            e.preventDefault();
            getProjectReleases();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getProjectReleases();
            }
        });
    });
})();