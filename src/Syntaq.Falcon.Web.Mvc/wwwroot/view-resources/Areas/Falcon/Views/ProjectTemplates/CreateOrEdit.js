﻿(function () {

    var _projectsService = abp.services.app.projects;

    var _$projectTenantsTable = $('#ProjectTenantsTable');
    var _projectTenantsService = abp.services.app.projectTenants;

    var _ProjectReleaseLookupTableModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Falcon/ProjectTemplates/ProjectReleaseLookupTableModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTemplates/_ProjectReleaseLookupTableModal.js',
        modalClass: 'ProjectReleaseLookupTableModal'
    });

    $("#btn-releases-open").click(function () {
        var projectTemplateId = $('#Id').val();
        var projectId = $('#ProjectId').val();
        _ProjectReleaseLookupTableModal.open({ projectTemplateId: projectTemplateId, projectId: projectId, modalClass: 'modal-xl' });
    });

    var _createOrEditProjectTenantModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Falcon/ProjectTenants/CreateOrEditModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTenants/_CreateOrEditModal.js',
        modalClass: 'CreateOrEditProjectTenantModal modal-xl',
    });

    $('#AddProjectTenantButton').click(function () {
        if ($('#Id').val() == '') {
            abp.message.info('You need to save the Project first before adding Subscribers.');
        }
        else {
            _createOrEditProjectTenantModal.open({ id: null, projectId: $('#ProjectId').val() });
        }
    });

    // ACL----
    var _userService = abp.services.app.user;
    var _organizationUnitService = abp.services.app.organizationUnit;
    var _assignees;
    // ACL----

    // INIT
    this.init = async function () {

        if ($('#Id').val() === '') {
            $('#Id').val(uuidv4());
            $('#ProjectId').val($('#Id').val());
        }

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectReleases/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectReleases/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditProjectReleaseModal',
        });

        $("#PublishProjectTemplateButton").click(function () {
            var projectId = $('#ProjectId').val();
            _createOrEditModal.open({ id: null, projectId });
        });

        // ACL
        var _localSource = [];
        var users = await _userService.getUsers({ filter: null, permission: null, role: null, onlyLockedUsers: false });

        $(users.items).each(function () {
            var user = { value: "" + this.userName + "", type: "User", id: this.id };
            _localSource.push(user);
        });

        var teams = await _organizationUnitService.getOrganizationUnits();

        $(teams.items).each(function () {
            var team = { value: "" + this.displayName + "", type: "Team", id: this.id };
            _localSource.push(team);
        });

        _assignees = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            local: _localSource
        });
        _assignees.initialize();

        var elt = $('.tagsinput-typeahead');
        elt.tagsinput({
            itemValue: 'value',
            itemText: 'value',
            typeaheadjs: {
                name: 'assignees',
                displayKey: 'value',
                source: _assignees.ttAdapter()
            }
        });

        $(elt).each(function (index) {
            var ctrl = this;
            var value = $(this).attr('data-value');
            if (value) {
                var obj = JSON.parse(value);
                jQuery.each(obj, function () {
                    $(ctrl).tagsinput('add', this);
                });
            }
        });

        $('[name=ProjectTemplateName]').change(() => {
            if ($('[name=ProjectTemplateName]').val() === "") {
                $('[name=ProjectTemplateName]')[0].style.borderColor = 'red';
            } else {
                $('[name=ProjectTemplateName]')[0].style.borderColor = '';
            }
        });

        $('#kt_wizard_form_step_2_form').change(() => {
            $('[name="Steps[][StepName]"]').each(function (index) {
                if (($('[name="Steps[][StepName]"]').length - 1) !== index && $(this).val() == "") {
                    $(this)[0].style.borderColor = 'red';
                } else {
                    $(this)[0].style.borderColor = '';
                }
            });

            $('[name="Steps[][FormId]"]').each(function (index) {
                if (($('[name="Steps[][FormId]"]').length - 1) !== index && $(this).val() == "") {
                    $(this)[0].style.borderColor = 'red';
                } else {
                    $(this)[0].style.borderColor = '';
                }
            });
        });

        var counter = 1
        $('#btn-add-ProjectStep').on('click', function (wizard) {

            var index = $('.step').length;
            var step_label = `step_label_${index}`;

            var clone = $('#kt_wizard_form_step_2_formblock').clone(true);

            clone.prop('id', `step_${index}`);

            var input = clone.find('.step_name');
            input.attr('data-step', index);

            clone.find('[name="Steps[][Order]"]').each(function () {
                $(this).val(counter);
                $(this).attr('placeholder', counter);
                counter++;
            });

            $('#steps_container').append(clone);
            $('#steps_nav').append(`<a id="${step_label}" href="#" class="step-label list-group-item p-2 list-group-item-action" onclick="$('.list-group a').removeClass('active');$(this).addClass('active'); $('.step').addClass('d-none');$('#step_${index}').removeClass('d-none');"> Step ${index} </a>`);

            $(`#${step_label}`).click();

        });

        $('.step_name').on('keyup', function (e) {
            var label = $(this).val(); var index = $(this).data('step'); $('#step_label_' + index).text(label);
        });

        $('#saveProjectTemplateButton, #saveProjectTemplateModalButton').click(() => {

            var setpsJSON = $('#kt_wizard_form_step_2_form').serializeJSON({ useIntKeysAsArrayIndex: false });
            //var tags = $('#kt_wizard_form_step_3_tags').serializeArray();

            var stepsError = '';
            setpsJSON.Steps.forEach(obj => {
                if (obj.FormId === '') {
                    stepsError += obj.StepName + ', ';
                }
            });

            if (stepsError !== '') {
                stepsError = stepsError.slice(0, -2);
                var errMsg = stepsError + ' does not have a selected Form';
                if (setpsJSON.length > 1) {
                    errMsg = stepsError + ' do not have a selected Form';
                }
                abp.message.error(errMsg);
                return;
            }

            abp.ui.block(); //Block the whole page

            _projectsService.createOrEditProjectTemplate({
                id: $('#Id').val(),
                name: $('[name=ProjectTemplateName]').val(),
                description: $('[name=TemplateDescription]').val(),
                stepsSchema: setpsJSON.Steps,
                enabled: true, // no longer used
                assignees: null, // $('#Assignees').tagsinput('items').length == 0 ? null : $('#Assignees').tagsinput('items'),
                tags: null // no longerr used
            }).done(function () {
                //_modalManager.close();
                $('#projectSettingsModal').modal('hide');
                $('#savedTemplateName').text($('[name=ProjectTemplateName]').val());
                $('#SetAliveHeaderMessage').text("Project template: " + $('[name=ProjectTemplateName]').val() + ' is successfully saved.');
                $("#setLiveToast").addClass("show");
                abp.event.trigger('app.createOrEditProjectTemplatesModalSaved');
                abp.notify.info(abp.localization.localize('SavedSuccessfully'));
            }).always(function () {
                abp.ui.unblock();
            });

        });

        function uuidv4() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        $('#newVersionButton').click(() => {

            var setpsJSON = $('#kt_wizard_form_step_2_form').serializeJSON({ useIntKeysAsArrayIndex: false });
            var tags = $('#kt_wizard_form_step_3_tags').serializeArray();

            abp.ui.block(); //Block the whole page

            var newid = uuidv4();

            _projectsService.createProjectVersion({
                id: newid,
                projectId: $('#ProjectId').val(),
                version: $('[name=ProjectVersion]').val(),
                name: $('[name=ProjectTemplateName]').val(),
                description: $('[name=TemplateDescription]').val(),
                stepsSchema: setpsJSON.Steps,
                enabled: Boolean($('[name=TemplateEnable]').is(':checked')),
                assignees: null, //$('#Assignees').tagsinput('items').length == 0 ? null : $('#Assignees').tagsinput('items'),
                tags: null //tags
            }).done(function () {
                //_modalManager.close();
                abp.event.trigger('app.createOrEditProjectTemplatesModalSaved');
                window.location.href = '/Falcon/ProjectTemplates/createoredit?Id=' + newid;

            }).always(function () {
                abp.ui.unblock();
            });

        });

        // START RELEASES
        var _permissions = {
            create: abp.auth.hasPermission('Pages.ProjectReleases.Create'),
            edit: abp.auth.hasPermission('Pages.ProjectReleases.Edit'),
            delete: abp.auth.hasPermission('Pages.ProjectReleases.Delete'),
        };

        var pid = $('#ProjectId').val();
        if (pid === '') pid = uuidv4(); // random pid to return no subs

        // START TENANTS
        var dataTableTenants = _$projectTenantsTable.DataTable({
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
                        projectIdFilter: $('#ProjectId').val(),
                        enabledFilter: null,
                        projectEnvironmentNameFilter: null,
                    };
                },
            },
            columnDefs: [
                {
                    width: 120,
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    createdRow: function (row, data, dataIndex) {

                        $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3)").on("click", function () {
                            debugger;
                            _createOrEditProjectTenantModal.open({ id: data.record.projectTenant.id, projectId: $('#ProjectId').val() });
                        });
                    },
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
                            //{
                            //    text: app.localize('View'),
                            //    iconStyle: 'far fa-eye mr-2',
                            //    action: function (data) {
                            //        _viewProjectTenantModal.open({ id: data.record.projectTenant.id });
                            //    },
                            //},
                            {
                                text: app.localize('Edit'),
                                //iconStyle: 'far fa-edit mr-2',
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditProjectTenantModal.open({ id: data.record.projectTenant.id, projectId: $('#ProjectId').val() });
                                },
                            },
                            {
                                text: app.localize('Delete'),
                                //iconStyle: 'far fa-trash-alt mr-2',
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteProjectTenant(data.record.projectTenant);
                                },
                            },
                        ],
                    },
                },
                {
                    targets: 0,
                    data: 'projectTenant.subscriberTenantName',
                    name: 'Tenant Name',
                    render: function (data, type, row) {
                        return `<div><img class="stq-primary-icon me-2" title="Project" src="/common/images/primaryicons/organization.png"></i> ${row.projectTenant.subscriberTenantName}</div><div style="overflow-y: hidden; max-height: 10em"></div>`;
                    }
                },
                {
                    targets: 1,
                    data: 'projectTenant.enabled',
                    name: 'enabled',
                    render: function (enabled) {
                        if (enabled) {
                            return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    },
                },
                {
                    targets: 2,
                    data: 'projectEnvironmentName',
                    name: 'projectEnvironmentFk.name',
                },
            ],
        });

        function deleteProjectTenant(projectTenant) {
            abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
                if (isConfirmed) {
                    _projectTenantsService
                        .delete({
                            id: projectTenant.id,
                        })
                        .done(function () {
                            dataTableTenants.ajax.reload();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                }
            });
        }

        abp.event.on('app.createOrEditProjectTenantModalSaved', function () {
            dataTableTenants.ajax.reload();
        });

        // END TENANTS

    }

    this.init();

})(jQuery);