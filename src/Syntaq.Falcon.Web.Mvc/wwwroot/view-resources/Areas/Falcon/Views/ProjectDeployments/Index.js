﻿
(function () {
    $(function () {

        var _$projectDeploymentsTable = $('#ProjectDeploymentsTable');
        var _projectDeploymentsService = abp.services.app.projectDeployments;

        var _$projectsTable = $('#ProjectTemplatesTable');
        var _$sharedProjectsTable = $('#SharedProjectTemplatesTable');
        var _projectsService = abp.services.app.projects;
        var _aclService = abp.services.app.aCLs;

        var _createOrEditProjectTemplateModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTemplates/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTemplates/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectTemplatesModal'
        });

        var _createOrEditProjectTemplateTagsModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTemplates/CreateOrEditTagsModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTemplates/_CreateOrEditTagsModal.js',
            modalClass: 'CreateOrEditProjectTemplatesTagModal'
        });

        var _viewProjectTemplateModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTemplates/ViewProjectTemplateModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTemplates/_ViewProjectTemplateModal.js',
            modalClass: 'ViewProjectTemplateModal'
        });

        var _projectDeploymentTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectDeployments/ProjectDeploymentTableModal',
            scriptUrl:
                abp.appPath +
                'view-resources/Areas/Falcon/Views/ProjectDeployments/_ProjectDeploymentTableModal.js',
            modalClass: 'ProjectReleaseLookupTableModal',
        });

        var _ProjectDeploymentprojectReleaseLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectDeployments/ProjectReleaseLookupTableModal',
            scriptUrl:
                abp.appPath +
                'view-resources/Areas/Falcon/Views/ProjectDeployments/_ProjectDeploymentTableModal.js',
            modalClass: 'ProjectReleaseLookupTableModal',
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.ProjectTemplates.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectTemplates.Edit'),
            delete: abp.auth.hasPermission('Pages.ProjectTemplates.Delete')
        };

        var _manageACLModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
            modalClass: 'ManageACLModal'
        });

        var _manageTenantACLModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageTenantACLModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageTenantACLModal.js',
            modalClass: 'ManageACLModal'
        });

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var dataTable = _$projectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3)").on("click", function () {
                    //_createOrEditProjectTemplateModal.open({ projectTemplateId: data.id })
                    window.location.href = '/Falcon/ProjectTemplates/createoredit?Id=' + data.id;
                });
            },
            listAction: {
                ajaxFunction: _projectsService.getAllProjectTemplates,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectTemplatesTableFilter').val(),
                        nameFilter: null,
                        descriptionFilter: null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 3,
                    data: null,
                    orderable: false,
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
                                    //_createOrEditProjectTemplateModal.open({ projectTemplateId: data.record.id })
                                    window.location.href = '/Falcon/ProjectTemplates/createoredit?Id=' + data.record.id;
                                }
                            },
                            {
                                text: app.localize('Export'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    exportProject(data.record.id);
                                }
                            },
                            {
                                text: 'Share',
                                action: function (data) {
                                    _manageACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Project' });
                                }
                            },
                            {
                                text: app.localize('Share to Tenant'),
                                visible: function () {
                                    return true;
                                },
                                action: function (data) {
                                    _manageTenantACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Project' });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectTemplate(data.record);
                                }
                            }]
                    }
                },
                {
                    targets: 0,
                    data: null,
                    name: "name",
                    overflow: 'hidden',
                    render: function (data, type, row) {

                        if (data.shared === true) {
                            var sharedSpan = '<span class="label badge badge-' + 'info' + ' badge-inline">' + app.localize('Share') + '</span>';
                            data = `<div  ><img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/project.png"></i> ${data.name} ${sharedSpan}</div><div> ${data.description}</div>`;
                        }
                        else {
                            data = `<img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/project${data.type}.png"></i> ${data.name} 
                                        <div class="text-width" style = "text-overflow:ellipsis; overflow: hidden;">${data.description}</div>`;
                        }

                        return data;
                    }
                },
                {
                    targets: 1,
                    data: null,
                    name: "enabled",
                    render: function (data, type, row) {
                        return data.enabled;
                    }

                },
                {
                    targets: 2,
                    data: null,
                    name: "lastUpdated",
                    width: '22em',
                    render: function (data, type, row) {
                        var dt = new Date(row.creationTime);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        var creationTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        return creationTime;
                    }

                }
            ]
        });

        var sharedDataTable = _$sharedProjectsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _projectsService.getAllSharedProjectTemplates,
                inputFilter: function () {
                    return {
                        filter: null,
                        nameFilter: null,
                        descriptionFilter: null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 3,
                    data: null,
                    orderable: false,
                    width: 100,
                    defaultContent: '',
                    rowAction: {
                        text: app.localize('Actions'),
                        items: [
                            {
                                text: app.localize('Accept'),
                                visible: function (data) {
                                    return _permissions.delete
                                },
                                action: function (data) {
                                    acceptSharedProjectTemplate(data.record);
                                }
                            },

                            {
                                text: app.localize('Tags'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditProjectTemplateTagsModal.open({ projectTemplateId: data.record.id })
                                }
                            },

                            {
                                text: 'Share',
                                action: function (data) {
                                    _manageACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Project' });
                                }
                            },
                            {
                                text: app.localize('Remove'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    removeSharedProjectTemplate(data.record);
                                }
                            }]
                    }
                },
                {
                    targets: 0,
                    data: null,
                    name: "name",
                    render: function (data, type, row) {
                        return `<div><img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/project.png"></i> ${data.name}</div><div style="overflow-y: hidden; max-height: 10em"><div style="pre-wrap; width: 40vw ">${data.description}</div></div>`;
                    }

                },
                {
                    targets: 1,
                    data: null,
                    name: "sharedby",
                    render: function (data, type, row) {
                        return data.tenancyName;
                    }

                },
                {
                    targets: 2,
                    data: null,
                    width: 150,
                    name: "accepted",
                    render: function (data, type, row) {
                        return data.accepted;
                    }

                }
            ]
        });

        function exportProject(projectId) {
            window.open('/Falcon/ProjectTemplates/export?projectid=' + projectId);
        }

        function getProjectTemplates() {
            dataTable.ajax.reload();
            sharedDataTable.ajax.reload();
        }

        function deleteProjectTemplate(projectTemplate) {
            abp.message.confirm(
                `Delete Project Template: ${projectTemplate.name}`,
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectsService.deleteProjectTemplate({
                            id: projectTemplate.id
                        }).done(function () {
                            getProjectTemplates();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

        function acceptSharedProjectTemplate(projectTemplate) {
            abp.message.confirm(
                `Accept this shared Project Template : ${projectTemplate.name}`,
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {

                        var acl = {};
                        acl.Type = "Project";
                        acl.Role = "V";
                        acl.EntityId = projectTemplate.id;

                        _aclService.acceptSharedByTenant(acl).done(function () {
                            abp.notify.success(app.localize('SuccessfullyAccepted'));
                        });
                    }
                }
            );
        }

        function removeSharedProjectTemplate(projectTemplate) {

            abp.message.confirm(
                `Remove this Shared Project Template : ${projectTemplate.name}`,
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {


                        var acl = {};
                        acl.Type = "Project";
                        acl.Role = "V";
                        acl.EntityId = projectTemplate.id;

                        _aclService.removeSharedByTenant(acl).done(function () {
                            sharedDataTable.ajax.reload();
                            abp.notify.success(app.localize('SuccessfullyRemoved'));
                        });
                    }
                }
            );
        }

        $("#CreateNewProjectTemplateButton").click(function () {
            _createOrEditProjectTemplateModal.open({ Id: null });
        });

        $("#ImportProjectTemplateButton").click(function () {

            var _importProjectTemplateModal = new app.ModalManager({
                viewUrl: abp.appPath + 'Falcon/ProjectTemplates/ImportModal',
                scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTemplates/_ImportModal.js',
                modalClass: 'ImportProjectTemplatesModal'
            });

            _importProjectTemplateModal.open();
        });

        abp.event.on('app.createOrEditProjectTemplatesModalSaved', function () {
            getProjectTemplates();
        });

        abp.event.on('app.grantAccessSuccess', function () {
            getProjectTemplates();
        });

        abp.event.on('app.revokeAccessSuccess', function () {
            getProjectTemplates();
        });

        $('#GetProjectsButton').click(function (e) {
            e.preventDefault();
            getProjectTemplates();
        });
        $('#clearbtn').click(function () {
            $('#ProjectTemplatesTableFilter').val('');
            getProjectTemplates();
        });
        var _manageNotificationModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Notifications/ManageRecipientsModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Notifications/_ManageRecipientsModal.js',
            modalClass: 'ManageNotificationRecipients'
        });
        // Notification 
        $('#closeNotificationButton').click(function (e) {
            e.preventDefault();
            $("#setLiveToast").removeClass("show");
        });

        $('#notifyButton').click(function (e) {
            e.preventDefault();
            $("#setLiveToast").removeClass("show");
            _manageNotificationModal.open({ defaultMessage: $("#SetAliveHeaderMessage").text(), entityNames: $('#savedTemplateName').text() });
        });
        //$(".bootstrap-tagsinput").find("input").change( function () {
        $(document).on('click', function (e) {
            var container = $(".toast.show");
            if (!$(e.target).closest(container).length) {
                container.removeClass("show");
            }
        });

        //Start Project Deployment Table
        var pdTable = _$projectDeploymentsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3)").on("click", function () {

                    _projectDeploymentTableModal.open(
                        { id: null, displayName: '' }
                    );
                });
            },
            listAction: {
                ajaxFunction: _projectDeploymentsService.getAllByProject,
                inputFilter: function () {
                    return {
                        filter: $('#ProjectDeploymentsTableFilter').val(),
                        commentsFilter: $('#CommentsFilterId').val(),
                        actionTypeFilter: $('#ActionTypeFilterId').val(),
                        projectReleaseNameFilter: $('#ProjectReleaseNameFilterId').val(),
                    };
                },
            },
            columnDefs: [
                {
                    width: 100,
                    targets: 2,
                    data: null,
                    orderable: false,

                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            {
                                text: app.localize('Deployments'),
                                //iconStyle: 'fas fa-boxes me-2',
                                action: function (data) {

                                    _projectDeploymentTableModal.open(
                                        { id: null, displayName: '' }
                                    );

                                },
                            },
                            {
                                text: app.localize('Delete'),
                                //iconStyle: 'far fa-trash-alt me-2',
                                visible: function () {
                                    return false;
                                },
                                action: function (data) {
                                    deleteProjectDeployment(data.record.projectDeployment);
                                },
                            },
                        ],
                    },
                },
                {
                    targets: 1,

                    data: null,
                    name: "lastUpdated",
                    width: '22em',
                    render: function (data, type, row) {

                        var dt = new Date(row.createdDateTime);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        var creationTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        return creationTime;
                    }
                },
                //{
                //    targets: 1,
                //    width: 100,
                //    data: 'projectDeployment.actionType',
                //    name: 'actionType',
                //    render: function (actionType) {
                //        return app.localize('Enum_ProjectDeploymentActionType_' + actionType);
                //    },
                //},
                {
                    targets: 0,
                    width: 250,
                    // data: 'projectReleaseName',
                    //name: 'projectReleaseFk.name',
                    //width: '42em',
                    render: function (data, type, row) {

                        data = `<img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/project1.png"></i> ${row.projectName}<div>${row.projectDeployment.comments}</div>`;
                        return data;
                    }

                },
            ],
        });

        //function getProjectDeployments() {
        //    pdTable.ajax.reload();
        //}

        //function deleteProjectDeployment(projectDeployment) {


        //    abp.message.confirm(
        //        '',
        //        app.localize('AreYouSure'),
        //        function (isConfirmed) {
        //            if (isConfirmed) {
        //                _projectDeploymentsService.delete({
        //                    id: projectDeployment.id
        //                }).done(function () {
        //                    debugger;
        //                    getProjectDeployments(true);
        //                    abp.notify.success(app.localize('SuccessfullyDeleted'));
        //                });
        //            }
        //        }
        //    );
        //}


    });
})();