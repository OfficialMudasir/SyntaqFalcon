﻿(function ($) {
    app.modals.CreateOrEditProjectReleaseModal = function () {

        var _projectReleasesService = abp.services.app.projectReleases;
        var _$projectTenantsTable = $('#ProjectTenantsModalTable');
        var _projectTenantsService = abp.services.app.projectTenants;

        var _modalManager;
        var _$projectReleaseInformationForm = null;

        var _ProjectReleaseprojectEnvironmentLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectReleases/ProjectEnvironmentLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectReleases/_ProjectReleaseProjectEnvironmentLookupTableModal.js',
            modalClass: 'ProjectEnvironmentLookupTableModal'
        });

        $('#addProjectTenantButton').click(function () {
            addProjectTenant2();
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').daterangepicker({
                singleDatePicker: true,
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$projectReleaseInformationForm = _modalManager.getModal().find('form[name=ProjectReleaseInformationsForm]');
            _$projectReleaseInformationForm.validate();

            _$projectReleaseTenantForm = _modalManager.getModal().find('form[name=ProjectTenantsForm]');

        };
 
        var datatableProjectTenants = _$projectTenantsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _projectTenantsService.getAll,
                inputFilter: function () {
                    return {
                        filter: null,
                        minSubscriberTenantIdFilter: null,
                        maxSubscriberTenantIdFilter: null,
                        projectIdFilter: $('#ProjectRelease_ProjectId').val(),
                        enabledFilter: 1,
                        projectEnvironmentNameFilter: $('input[name=projectEnvironmentName]').val() == '' ? '00000000-0000-0000-0000-000000000000' : $('input[name=projectEnvironmentName]').val() ,
                    };
                }
            },
            columnDefs: [
                {
                    width: 120,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    targets: 3,
                    name: "action",
                    render: function (enabled) {
                        data = '';
                        if ($('#id').val() == '' || $('#releaseIdToClone').val() != '') {
                            data = `<div class="btn btn-sm btn-secondary pull-right" onclick="$(this).closest('tr').remove()" >Remove</div>`
                        }
                        return data;
                    }
                },
                {
                    targets: 0,
                    data: "projectTenant.subscriberTenantName",
                    name: "subscriberTenantName",
                    render: function (data, type, row) {
                        return `<input type="hidden" name="subscriberTenantId" value="${row.projectTenant.subscriberTenantId}" /> ${data}`;
                    }
                },
                {
                    targets: 1,
                    data: "projectTenant.enabled",
                    name: "enabled",
                    render: function (enabled, type, row) {
                        if (enabled) {
                            return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    }

                },
                {
                    targets: 2,
                    data: "projectEnvironmentName",
                    name: "projectEnvironmentFk.name"
                }
            ]
        });

        $('#OpenProjectEnvironmentLookupTableButton').click(function () {

            var projectRelease = _$projectReleaseInformationForm.serializeFormToObject();

            _ProjectReleaseprojectEnvironmentLookupTableModal.open({ id: projectRelease.projectEnvironmentId, displayName: projectRelease.projectEnvironmentName }, function (data) {
                _$projectReleaseInformationForm.find('input[name=projectEnvironmentName]').val(data.displayName);
                _$projectReleaseInformationForm.find('input[name=projectEnvironmentId]').val(data.id);

                // Load the tenant subscribers here
                datatableProjectTenants.ajax.reload();

            });
        });

        $('#ClearProjectEnvironmentNameButton').click(function () {
            _$projectReleaseInformationForm.find('input[name=projectEnvironmentName]').val('');
            _$projectReleaseInformationForm.find('input[name=projectEnvironmentId]').val('');
        });

        $('.deploy-button').click(function () {
           // this.save(true);

            if (!_$projectReleaseInformationForm.valid()) {
                return;
            }
            if ($('#ProjectRelease_ProjectEnvironmentId').prop('required') && $('#ProjectRelease_ProjectEnvironmentId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectEnvironment')));
                return;
            }

            var projectRelease = _$projectReleaseInformationForm.serializeFormToObject();
            if (projectRelease.projectEnvironmentId == '') {
                abp.message.error('Project Environment must be selected');
                return;
            }

            projectRelease.ProjectTenants = [];
            var input = document.getElementsByName('subscriberTenantId');
            $.each(input, function (key, value) {
                projectRelease.ProjectTenants.push(value.value);
            });

            var notes = quill.root.innerHTML;
            projectRelease.notes = notes;
  
            projectRelease.deployToSubscribers = true;
 
            _modalManager.setBusy(true);
            _projectReleasesService.createOrEdit(
                projectRelease
            ).done(function () {
                abp.event.trigger('app.createOrEditProjectReleaseModalSaved');
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });

        });

        $('.promote-button').click(function () {

            if (!_$projectReleaseInformationForm.valid()) {
                return;
            }
            if ($('#ProjectRelease_ProjectEnvironmentId').prop('required') && $('#ProjectRelease_ProjectEnvironmentId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectEnvironment')));
                return;
            }

            var projectRelease = _$projectReleaseInformationForm.serializeFormToObject();
            if (projectRelease.projectEnvironmentId == '') {
                abp.message.error('Project Environment must be selected');
                return;
            }

            projectRelease.ProjectTenants = [];
            var input = document.getElementsByName('subscriberTenantId');
            $.each(input, function (key, value) {
                projectRelease.ProjectTenants.push(value.value);
            });

            var notes = quill.root.innerHTML;
            projectRelease.notes = notes;

            projectRelease.deployToSubscribers = true;

            _modalManager.setBusy(true);
            _projectReleasesService.createOrEdit(
                projectRelease
            ).done(function () {
                abp.event.trigger('app.createOrEditProjectReleaseModalSaved');
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });

        });

        this.save = function (deploy) {
   
            if (!_$projectReleaseInformationForm.valid()) {
                return;
            }
            if ($('#ProjectRelease_ProjectEnvironmentId').prop('required') && $('#ProjectRelease_ProjectEnvironmentId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectEnvironment')));
                return;
            }

            var projectRelease = _$projectReleaseInformationForm.serializeFormToObject();
            if (projectRelease.projectEnvironmentId == '') {
                abp.message.error('Project Environment must be selected');
                return;
            }

            projectRelease.ProjectTenants = [];
            var input = document.getElementsByName('subscriberTenantId');
            $.each(input, function (key, value) {
                projectRelease.ProjectTenants.push(value.value);
            });

            var notes = quill.root.innerHTML;
            projectRelease.notes = notes;

            if (deploy === undefined) deploy = false;
            projectRelease.deployToSubscribers = deploy;

            _modalManager.setBusy(true);
            _projectReleasesService.createOrEdit(
                projectRelease
            ).done(function () {
                abp.event.trigger('app.createOrEditProjectReleaseModalSaved');
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };

        var _projectDeploymentsService = abp.services.app.projectDeployments;
        var _$projectReleaseDeploymentsTableModal = $('#ProjectReleaseDeploymentsTable');

        var releaseid = $('#id').val();

        var dataTableReleaseDeployments = _$projectReleaseDeploymentsTableModal.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
               
                    $(row).find("select").on("change", function () {
 
                    event.stopPropagation();
                    event.stopImmediatePropagation();

                    _modalManager.setBusy(true);
                    var enabled = $(this).val();

                    _projectDeploymentsService.setEnabled(
                        data.projectDeployment.id, enabled
                    ).done(function () {
                        abp.notify.info(app.localize('Saved'));
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });

                });
            },
            listAction: {
                ajaxFunction: _projectDeploymentsService.getAllForRelease,
                inputFilter: function () {
                    return {
                        projectId: null,
                        releaseId: releaseid,
                        filter: null,
                        commentsFilter: null,
                        actionTypeFilter: null,
                        projectReleaseNameFilter: null
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "tenantName",
                    name: "tenantName",
                    render: function (tenantName, type, row) {
                        debugger;
                        return '<span style=" ">' + tenantName + '</span>';
                    }
                },
                {
                    //STQ Modified
                    targets: 1,
                    data: 'projectDeployment.creationTime',
                    name: 'creationTime',
                    //render: function (creationTime) {
                    //    return moment(creationTime).format('YYYY-MM-DD HH:mm:ss');
                    //},
                    //render: function (data, type, row) {
                    //    var dt = new Date(row.projectDeployment.creationTime);
                    //    var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                    //    var tmoptions = { hour: 'numeric', minute: 'numeric' };
                    //    var creationTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    //    return creationTime;
                    //}
                    render: function (data, type, row) {
                        var dt = new Date(row.projectDeployment.creationTime);
                        var dtoptions = { weekday: 'short', day: 'numeric', month: 'long', year: 'numeric' };
                        var creationTime = dt.toLocaleDateString('en-GB', dtoptions);
                        return creationTime;
                    }
                },
                {
                    targets: 2,
                    data: "projectDeployment.actionType",
                    name: "actionType",
                    render: function (actionType) {
                        return app.localize('Enum_ProjectDeploymentActionType_' + actionType);
                    }

                },

                {
                    targets: 3,
                    data: "projectDeployment.enabled",
                    name: "enabled",
                    render: function (enabled) {

                        //var data = app.localize('Enum_ProjectDeploymentActionType_' + actionType); 
                        var data = `<select style="width:120px" class="form-select m-input m-input--square" name="actionType" name="ProjectDeployment_Enabled"    aria-invalid="false">
                                <option ${enabled ? 'selected="selected"' : ''} value="true">Enabled</option>
                                <option ${!enabled ? 'selected="selected"' : ''}value="false">Disabled</option>
                            </select>`;

                        return data;
                    }

                },
                {
                    targets: 4,
                    orderable: false,
                    data: "projectDeployment.comments",
                    name: "projectDeployment.comments"
                }
            ]
        });


        $('#addSubscriptionAndDeployButton').click(function () {
            addSubscriptionAndDeploy();
        });

        function addSubscriptionAndDeploy() {

            // 1. Create a Tenant Subscription
            //public int SubscriberTenantId { get; set; }
            //public string SubscriberTenantName { get; set; }
            //public Guid ProjectId { get; set; }
            //public bool Enabled { get; set; }
            //public Guid ? ProjectEnvironmentId { get; set; }

            // 2. Create a Deployment
            //public string Comments { get; set; }
            //public ProjectDeploymentEnums.ProjectDeploymentActionType ActionType { get; set; }
            //public Guid ? ProjectReleaseId { get; set; }

            var releaseid = $('#id').val();

            if (releaseid == '') {
                abp.message.error('Release must be created first');
                return;
            }

            if ($('#ProjectTenantName').val() == '') {
                abp.message.error('Tenant name must be provided');
                return;
            }


            if ($('#ProjectRelease_ProjectEnvironmentId').val() == '') {
                abp.message.error('Project Environment must be selected');
                return;
            }

            var projectTenant = {
                SubscriberTenantName: $('#ProjectTenantName').val(),
                ProjectId: $('#ProjectRelease_ProjectId').val(),
                Enabled: true,
                ProjectEnvironmentId: $('#ProjectRelease_ProjectEnvironmentId').val(),
            };


            _projectTenantsService.createOrEdit(
                projectTenant
            ).done(function () {
                datatableProjectTenants.ajax.reload();

                // Create a Deployment for this new Subscription

                var projectDeployment = {
                    ActionType: 0,
                    ProjectReleaseId: releaseid,
                    ReleaseIdToClone: $('#releaseIdToClone').val(),
                    TenantName: $('#ProjectTenantName').val()
                };

                _projectDeploymentsService.createOrEdit(
                    projectDeployment
                ).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditProjectDeploymentModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });

            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
 
        $('#ProjectReleaseInformationsForm').on("keyup keypress keydown", function (e) {

            var code = e.keyCode || e.which;

            //alert(code);
            if (code == 13) {
                debugger;
                /*alert('2');*/
                e.preventDefault();
                return false;
            }
        });

    };


})(jQuery);