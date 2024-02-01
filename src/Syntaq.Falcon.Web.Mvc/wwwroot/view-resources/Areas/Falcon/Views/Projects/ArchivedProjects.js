(function () {
    $(function () {
        console.log("hello");
        var _$archievedProjectsTable = $('#ArchivedProjectsTable');
        var _$projectsTable = $('#ProjectsTable');
        var _projectsService = abp.services.app.projects;
        var _entityTypeFullName = 'Syntaq.Falcon.Projects.Project';

        //var _StartProjectModal = new app.ModalManager({
        //    viewUrl: abp.appPath + 'Falcon/Projects/StartProjectModal',
        //    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/_StartProjectModal.js',
        //    modalClass: 'StartProjectModal'
        //});


        var _startModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/StartProjectModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/_StartProjectModal.js',
            modalClass: 'StartProjectModal'
        });


        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

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

        var dataTable = _$projectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
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
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        //cssClass: 'btn btn-brand dropdown-toggle',
                        text:  app.localize('Actions') + ' <i class="fas fa-angle-down"></i>',
                        items: [
                            {
                                text: app.localize('Open'),
                                action: function (data) {
                                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject/' + data.record.project.id;
                                    window.open(openProject);
                                }
                            },
                            //{
                            //    text: app.localize('Edit'),
                            //    visible: function () {
                            //        return _permissions.edit;
                            //    },
                            //    action: function (data) {
                            //        window.location = "/Falcon/Projects/CreateOrEdit/" + data.record.project.id;
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
                },

                {
                    targets: 1,
                    data: "project.name",
                    name: "name",

                    render: function (data, type, row) {
                        var badgeclass = 'warning';

                        if (row.project.status === 1) {
                            badgeclass = 'info';
                        }

                        if (row.project.status === 2) {
                            badgeclass = 'success';
                        }

                        data = `<a href="/Falcon/Projects/ViewProject/${row.project.id}"><div class="h6 text-dark" > <span>${row.project.name}</span></div>`;
                        data += `<div style="overflow-y: auto; max-height: 5em;"><div style="white-space: pre-wrap;">${row.project.description}</div></div></a>`

                        return data;
                    }

                },
                //{
                //    targets: 2,
                //    data: "project.description",
                //    name: "description"
                //},
                {
                    targets: 2,
                    data: "project.status",
                    name: "status",
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
                        return '<span class="label kt-badge kt-badge--' + badgeclass + ' kt-badge--inline">' + app.localize('Enum_ProjectStatus_' + status) + '</span>';
                    }

                },
                {
                    targets: 3,
                    data: "project.lastModificationTime",
                    name: "lastModificationTime",
                    render: function (time) {
                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }

                }
                //, {
                //    targets: 2,
                //    data: "project.type",
                //    name: "type",
                //    render: function (type) {
                //        return app.localize('Enum_ProjectType_' + type);
                //    }

                //}
                //,{
                //    targets: 3,
                //    data: "recordRecordName",
                //    name: "recordFk.recordName"
                //}


            ]
        });


        var archivedDataTable = _$archievedProjectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
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
                    targets: 0,
                    data: "project.name",
                    name: "name",

                    render: function (data, type, row) {
                        var badgeclass = 'warning';

                        if (row.project.status === 1) {
                            badgeclass = 'info';
                        }

                        if (row.project.status === 2) {
                            badgeclass = 'success';
                        }

                        data = `<a href="/Falcon/Projects/ViewProject/${row.project.id}"><div class="h6 text-dark" > <span>${row.project.name}</span></div>`;
                        data += `<div style="overflow-y: auto; max-height: 5em;"><div style="white-space: pre-wrap;">${row.project.description}</div></div></a>`

                        return data;
                    }

                },
                //{
                //    targets: 2,
                //    data: "project.description",
                //    name: "description"
                //},
                {
                    targets: 1,
                    data: "project.status",
                    name: "status",
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
                        return '<span class="label kt-badge kt-badge--' + badgeclass + ' kt-badge--inline">' + app.localize('Enum_ProjectStatus_' + status) + '</span>';
                    }

                },
                {
                    targets: 2,
                    data: "project.lastModificationTime",
                    name: "lastModificationTime",
                    render: function (time) {
                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }

                },
                {
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        //cssClass: 'btn btn-brand dropdown-toggle',
                        text:  app.localize('Actions') + ' <i class="fas fa-angle-down"></i>',
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
                }

            ]
        });

        function getProjects() {
            archivedDataTable.ajax.reload();
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

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getProjects();
            }
        });
        $('#ProjectsTableFilter').keyup(function (e) {
            if (e.which === 13) {
                getProjects();
            }
        });
    });
})();