﻿(function () {
    $(function () {
        var _$archievedProjectsTable = $('#ArchivedProjectsTable');
        var _$projectsTable = $('#ProjectsTable');
        var _$shareProjectsTable = $('#ShareProjectsTable');

        var _projectsService = abp.services.app.projects;
        var _entityTypeFullName = 'Syntaq.Falcon.Projects.Project';

        var _RecordMatterContributorCommentModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/RecordMatterContributorCommentModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_RecordMatterContributorCommentModal.js',
            modalClass: 'RecordMatterContributorCommentModal'
        });

        var _startModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/StartProjectModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/_StartProjectModal.js',
            modalClass: 'StartProjectModal'
        });

        var _editModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/CreateOrEdit',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/CreateOrEdit.js',
            modalClass: 'CreateOrEditModal'
        });

        var _manageACLModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
            modalClass: 'ManageACLModal'
        });


        var _permissions = {
            create: abp.auth.hasPermission('Pages.Projects.Create'),
            edit: abp.auth.hasPermission('Pages.Projects.Edit'),
            'delete': abp.auth.hasPermission('Pages.Projects.Delete')
        };


        var _viewProjectModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/ViewprojectModal',
            modalClass: 'ViewProjectModal'
        });

        var _createProjectModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/CreateprojectModal',
            modalClass: 'CreateProjectModal'
        });


        $("#StartNewProjectButton").click(function (e) {
            //console.log(e);
            e.preventDefault();
            _startModal.open();
        });

        var _entityTypeHistoryModal = app.modals.EntityTypeHistoryModal.create();
        function entityHistoryIsEnabled() {
            return abp.auth.hasPermission('Pages.Administration.AuditLogs') &&
                abp.custom.EntityHistory &&
                abp.custom.EntityHistory.IsEnabled &&
                _.filter(abp.custom.EntityHistory.EnabledEntities, entityType => entityType === _entityTypeFullName).length === 1;
        }

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }
        $(window).bind('resize', function () {
            /*the line below was causing the page to keep loading.
            $('#tableData').dataTable().fnAdjustColumnSizing();
            Below is a workaround. The above should automatically work.*/
            $($.fn.dataTable.tables(true)).css('width', '100%');
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
        });

        //when nav tab change active panel, make all table resize
        $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            $($.fn.dataTable.tables(true)).css('width', '100%');
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc();
            // $($.fn.dataTable.tables(true)).DataTable().responsive.recalc();
        });

        var dataTable = _$projectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,

            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3)").on("click", function () {
                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject/' + data.project.id;
                    window.open(openProject, "_self");
                });
            },


            listAction: {
                ajaxFunction: _projectsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val(),
                        statusFilter: $('#StatusFilterId').val(),
                        typeFilter: $('#TypeFilterId').val(),
                        recordRecordNameFilter: $('#RecordRecordNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "project.name",
                    name: "name",
                    overflow: "hidden",
                    render: function (data, type, row) {

                        var badgeclass = 'warning';

                        if (row.project.status === 1) {
                            badgeclass = 'info';
                        }

                        if (row.project.status === 2) {
                            badgeclass = 'success';
                        }

                        data = `<div><div style="text-overflow:ellipsis; overflow: hidden;"><img class="stq-primary-icon me-2"   title="App" src="/common/images/primaryicons/project.png"></i> ${row.project.name}</div>`;
                        data += `</a>`

                        return data;
                    }

                },
                {
                    targets: 1,
                    data: "project.status",
                    name: "status",
                    width: '12em',
                    //orderable: false,
                    className: '',
                    render: function (status) {
                        var badgeclass = 'warning';

                        if (status === 1) {
                            badgeclass = 'info';
                        }

                        if (status === 2) {
                            badgeclass = 'success';
                        }
                        return '<span class="label badge badge-' + badgeclass + ' badge-inline" style="width:10em;">' + app.localize('Enum_ProjectStatus_' + status) + '</span>';
                    }

                },
                {
                    targets: 2,
                    data: "project.lastModificationTime",
                    name: "lastModificationTime",
                    width: '15em',
                    render: function (time) {

                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }

                }, {
                    responsivePriority: 1,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    width: '6em',
                    rowAction: {
                        //cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('Open'),
                                action: function (data) {
                                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject/' + data.record.project.id;
                                    window.open(openProject);
                                }
                            },
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    // window.location = "/Falcon/Projects/CreateOrEdit/" + data.record.project.id;
                                    _editModal.open({ id: data.record.project.id });
                                }
                            },
                            //{
                            //    text: 'Share',
                            //    action: function (data) {
                            //        _manageACLModal.open({ entityid: data.record.project.id, entityname: data.record.project.name, entitytype: 'Project' });
                            //    }
                            //},
                            {
                                text: app.localize('History'),
                                visible: function () {
                                    return entityHistoryIsEnabled();
                                },
                                action: function (data) {
                                    _entityTypeHistoryModal.open({
                                        entityTypeFullName: _entityTypeFullName,
                                        entityId: data.record.project.id
                                    });
                                }
                            },
                            {
                                text: app.localize('Archive'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    archiveProject(data.record.project);
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProject(data.record.project);
                                }
                            }]
                    }
                }
            ]
        });

        var archivedDataTable = _$archievedProjectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            fixedColumns: true,
            listAction: {
                ajaxFunction: _projectsService.getArchivedProject,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val(),
                        statusFilter: $('#StatusFilterId').val(),
                        typeFilter: $('#TypeFilterId').val(),
                        recordRecordNameFilter: $('#RecordRecordNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    // responsivePriority: 1,
                    targets: 0,
                    data: "project.name",
                    name: "name",
                    overflow: "hidden",
                    render: function (data, type, row) {
                        var badgeclass = 'warning';

                        if (row.project.status === 1) {
                            badgeclass = 'info';
                        }

                        if (row.project.status === 2) {
                            badgeclass = 'success';
                        }
                        data = `<a class="text-black" href="/Falcon/Projects/ViewProject/${row.project.id}">   <div   style="text-overflow:ellipsis; overflow: hidden;"> <img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/project.png"></i> ${row.project.name}</div>`;
                        data += `</a>`
                        return data;
                    }

                },
                {
                    targets: 1,
                    data: "project.status",
                    name: "status",
                    width: '12em',
                    //orderable: false,
                    className: '',
                    render: function (status) {
                        var badgeclass = 'warning';

                        if (status === 1) {
                            badgeclass = 'info';
                        }

                        if (status === 2) {
                            badgeclass = 'success';
                        }
                        return '<span class="label badge badge-' + badgeclass + ' badge-inline" style="width:10em;">' + app.localize('Enum_ProjectStatus_' + status) + '</span>';
                    }

                },
                {
                    targets: 2,
                    data: "project.lastModificationTime",
                    name: "lastModificationTime",
                    width: '15em',
                    render: function (time) {
                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }

                },
                {
                    responsivePriority: 1,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    width: '6em',
                    rowAction: {
                        //cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('Restore'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    restoreProject(data.record.project);
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProject(data.record.project);
                                }
                            }]
                    }
                },

            ]
        });

        var shareProjectsTable = _$shareProjectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,



            listAction: {
                ajaxFunction: _projectsService.getSharedProjects,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val(),
                        statusFilter: $('#StatusFilterId').val(),
                        typeFilter: $('#TypeFilterId').val(),
                        recordRecordNameFilter: $('#RecordRecordNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    // responsivePriority: 1,
                    targets: 0,
                    data: null,
                    name: "name",
                    overflow: 'hidden',
                    render: function (data, type, row) {
                        var badgeclass = 'warning';
                        data = `<a class="OnClickLink text-black" href="/Falcon/forms/load?AccessToken=${row.accessToken}&RecordMatterId=${row.recordMatterId}"><div style="text-overflow:ellipsis; overflow: hidden;"><img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/project.png"></i> ${row.projectName} - ${row.projectStepName}</div></a>`;
                        return data;
                    }
                },
                {
                    targets: 1,
                    data: "null",
                    name: "status",
                    width: '12em',
                    className: '',
                    render: function (data, type, row) {
                        var badgeclass = 'warning';
                        if (row.status === 0) {
                            badgeclass = 'info';
                        }
                        if (row.status === 3) {
                            badgeclass = 'success';
                        }
                        return '<span class="label badge badge-' + badgeclass + ' badge-inline" style="width:10em;">' + app.localize('Enum_RecordMatterContributorStatus_' + row.status) + ' ' + app.localize('Enum_ProjectStepRole_' + row.role); + '</span>';
                    }
                },
                {
                    targets: 2,
                    data: "lastModificationTime",
                    name: "lastModificationTime",
                    width: '15em',
                    render: function (time) {
                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }

                }, {
                    responsivePriority: 1,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    width: '6em',
                    rowAction: {
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('Open'),
                                action: function (data) {
                                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/load?AccessToken=' + data.record.accessToken + '&RecordMatterId=' + data.record.recordMatterId;
                                    window.open(openProject);
                                }
                            },
                            {
                                text: app.localize('Decline'),
                                action: function (data) {
                                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/load?AccessToken=' + data.record.accessToken + '&RecordMatterId=' + data.record.recordMatterId + '&finalised=' + 2;
                                    window.open(openProject);
                                }
                            }
                        ]
                    }
                }
            ]
        });

        function getProjects() {
            archivedDataTable.ajax.reload();
            shareProjectsTable.ajax.reload();
            dataTable.ajax.reload();
        }


        function archiveProject(project) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectsService.archiveProject({
                            id: project.id
                        }).done(function () {
                            getProjects(true);
                            abp.notify.success(app.localize('SuccessfullyArchived'));
                        });
                    }
                }
            );
        }

        function restoreProject(project) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectsService.restoreProject({
                            id: project.id
                        }).done(function () {
                            getProjects(true);
                            abp.notify.success('SuccessfullyRestored');
                        });
                    }
                }
            );
        }

        function deleteProject(project) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectsService.delete({
                            id: project.id
                        }).done(function () {
                            getProjects(true);
                            abp.notify.success('SuccessfullyDeleted');
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

        $('#clearbtn').click(function () {
            $('#ProjectsTableFilter').val('');
            getProjects();
        });



        $('#ExportToExcelButton').click(function () {
            _projectsService
                .getProjectsToExcel({
                    filter: $('#ProjectsTableFilter').val(),
                    nameFilter: $('#NameFilterId').val(),
                    descriptionFilter: $('#DescriptionFilterId').val(),
                    statusFilter: $('#StatusFilterId').val(),
                    typeFilter: $('#TypeFilterId').val(),
                    recordRecordNameFilter: $('#RecordRecordNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditProjectModalSaved', function () {
            getProjects();
        });

        $('#GetProjectsButton').click(function (e) {
            e.preventDefault();
            getProjects();
        });
        $("#ProjectsTableFilter").keyup(function (event) {
            // On enter 
            if (event.keyCode === 13) {
                getProjects();
            }
        });
    });
})();