(function ($) {
 
    app.modals.ProjectReleaseLookupTableModal = function () {

        var _permissions = {
            create: abp.auth.hasPermission('Pages.ProjectDeployments.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectDeployments.Edit'),
            'delete': abp.auth.hasPermission('Pages.ProjectDeployments.Delete')
        };

        var _projectDeploymentsService = abp.services.app.projectDeployments;
        var _$projectDeploymentsTableModal = $('#ProjectDeploymentsTableModal'); 


        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectDeployments/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectDeployments/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectDeploymentModal'
        });

        var _createOrEditTagEntitiesModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntities/CreateOrEditTagEntitiesModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntities/_CreateOrEditTagEntitiesModal.min.js',
            modalClass: 'CreateOrEditTagEntitiesModal'
        });

        var _manageACLModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
            modalClass: 'ManageACLModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        //this.setType = function () {
        //    if (!_$projectDeploymentInformationForm.valid()) {
        //        return;
        //    }
        //    if ($('#ProjectDeployment_ProjectReleaseId').prop('required') && $('#ProjectDeployment_ProjectReleaseId').val() == '') {
        //        abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectRelease')));
        //        return;
        //    }

        //    var projectDeployment = _$projectDeploymentInformationForm.serializeFormToObject();

        //    _modalManager.setBusy(true);
        //    _projectDeploymentsService.setType(
        //        projectDeployment
        //    ).done(function () {
        //        abp.notify.info(app.localize('SavedSuccessfully'));
        //        _modalManager.close();
        //        abp.event.trigger('app.createOrEditProjectDeploymentModalSaved');
        //    }).always(function () {
        //        _modalManager.setBusy(false);
        //    });
        //};


 
        var dataTable = _$projectDeploymentsTableModal.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("select").on("change", function () {
 
                    event.stopPropagation();
                    event.stopImmediatePropagation();

                    _modalManager.setBusy(true);

                    var selectedOption = $(this).find('option:selected');
                    var selectedValue = selectedOption.val();

                    //var type = $(this).val();

                    _projectDeploymentsService.setType(
                        data.projectDeployment.id, selectedValue
                    ).done(function () {
                        abp.notify.info(app.localize('Saved'));
                        getProjectDeployments();
                    }).always(function () {
                        _modalManager.setBusy(false);
                        getProjectDeployments();
                    });

                });
            },
            listAction: {
                ajaxFunction: _projectDeploymentsService.getAll,
                inputFilter: function () {
                    return {
                        projectId: $('#ProjectTemplateIdFilter').val(),
                        environmentId: $('#ProjectEnvironmentIdFilter').val(),
                        filter: $('#ProjectDeploymentsTableFilter').val(),
                        commentsFilter: $('#CommentsFilterId').val(),
                        actionTypeFilter: $('#ActionTypeFilterId').val(),
                        projectReleaseNameFilter: $('#ProjectReleaseNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "projectReleaseName",
                    name: "projectReleaseFk.name",
                    render: function (data, type, row) {

                        var data = `<div style="width:200px ">${row.projectReleaseName}</div>`

                        return data;
                    }
                },
                {
                    targets: 1,
                    //orderable: false,
                    name: "ProjectReleaseFk.VersionMajor",
                    render: function (data, type, row) {
 
                        return '<span>' + row.projectDeployment.projectRelease.versionMajor + '</span>' + '<span>.' + row.projectDeployment.projectRelease.versionMinor + '</span>' + '<span>.' + row.projectDeployment.projectRelease.versionRevision + '</span>';
                    }
                },
                {
                    targets: 4,
        
                    autoWidth: false,
                    name: "CreationTime",
                    render: function (data, type, row) {
           
                        var dt = new Date(row.projectDeployment.projectRelease.creationTime);
                        var dtoptions = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        // var creationTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        var creationTime =   dt.toLocaleDateString('en-GB', dtoptions);

                        return '<span style="white-space: nowrap;">' + creationTime + '</span>';
                    
 
                    }
                },
                {
                    targets: 2,
                    data: "projectDeployment.actionType",
                    name: "actionType",
                    width : "33%",
                    render: function (actionType) {

                        //var data = app.localize('Enum_ProjectDeploymentActionType_' + actionType);
                        debugger;
                        
                        var data = `<select style="width:100px" class="form-select m-input m-input--square" name="actionType" name="ProjectDeployment_ActionType" data-val="true" data-val-required="The ActionType field is required." aria-invalid="false">                

                                ${actionType == 0 ? '<option selected="selected" value="1" >New</option>' : ''}

                                <option ${actionType == 1 ? 'selected="selected"' : '' } value="1" class="">Active</option>
                                <option ${actionType == 2 ? 'selected="selected"' : ''} value="2" class="${actionType == 0 || actionType == 3 ? 'd-none' : '' }">InActive</option>
                                <option ${actionType == 3 ? 'selected="selected"' : ''} value="3" class="${actionType == 1 || actionType == 2 ? 'd-none' : '' }">Deferred</option>
                            </select>`;

                        return data;
                    }

                },
                {
                    targets: 3,
                    data: 'projectDeployment.enabled',
                    name: 'enabled',
                    render: function (enabled, type, row) {
                  
                        if (enabled) {
                            return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    },
                },
                {
         
                    targets: 5,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text:   app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            //{
                            //    text: app.localize('View'),
                            //    iconStyle: 'far fa-eye mr-2',
                            //    action: function (data) {
                            //        _viewProjectDeploymentModal.open({ id: data.record.projectDeployment.id });
                            //    }
                            //},
                            {
                                text: app.localize('Edit'),
                                //iconStyle: 'far fa-edit mr-2',
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    debugger;
                                    _createOrEditModal.open({ id: data.record.projectDeployment.id });
                                }
                            },
                            {
                                text: app.localize('Tags'),
                                //iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _createOrEditTagEntitiesModal.open({ id: data.record.projectDeployment.id });
                                },
                            },
                            {
                                text: 'Share',
                                action: function (data) {
                                    _manageACLModal.open({ entityid: data.record.projectDeployment.id, entityname: data.record.name, entitytype: 'Project', simplemode : true });
                                }
                            },
                            {
                                text: app.localize('Delete'),
   /*                             iconStyle: 'far fa-trash-alt mr-2',*/
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectDeployment(data.record.projectDeployment);
                                }
                            }]
                    }
                }
            ]
        });
 
        abp.event.on('app.createOrEditProjectDeploymentModalSaved', function () {
            getProjectDeployments();
        });
 
        function getProjectDeployments() {
            dataTable.ajax.reload();
        }

        function deleteProjectDeployment(projectDeployment) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _projectDeploymentsService.delete({
                            id: projectDeployment.id
                        }).done(function () {
                            dataTable.ajax.reload();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }
 
    };
})(jQuery);
