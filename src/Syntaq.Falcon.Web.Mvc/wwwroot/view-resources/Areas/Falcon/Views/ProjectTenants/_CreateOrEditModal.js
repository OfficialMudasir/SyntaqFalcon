﻿(function ($) {
    app.modals.CreateOrEditProjectTenantModal = function () {
 
        var _projectTenantsService = abp.services.app.projectTenants;

        var _modalManager;
        var _$projectTenantInformationForm = null;

        var _ProjectTenantprojectEnvironmentLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/ProjectTenants/ProjectEnvironmentLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectTenants/_ProjectTenantProjectEnvironmentLookupTableModal.js',
            modalClass: 'ProjectEnvironmentLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').daterangepicker({
                singleDatePicker: true,
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$projectTenantInformationForm = _modalManager.getModal().find('form[name=ProjectTenantInformationsForm]');
            _$projectTenantInformationForm.validate();
        };

        $('#OpenProjectEnvironmentLookupTableButton').click(function () {

            var projectTenant = _$projectTenantInformationForm.serializeFormToObject();

            _ProjectTenantprojectEnvironmentLookupTableModal.open({ id: projectTenant.projectEnvironmentId, displayName: projectTenant.projectEnvironmentName }, function (data) {
                _$projectTenantInformationForm.find('input[name=projectEnvironmentName]').val(data.displayName);
                _$projectTenantInformationForm.find('input[name=projectEnvironmentId]').val(data.id);
            });
        });

        $('#ClearProjectEnvironmentNameButton').click(function () {
            _$projectTenantInformationForm.find('input[name=projectEnvironmentName]').val('');
            _$projectTenantInformationForm.find('input[name=projectEnvironmentId]').val('');
        });

        this.save = function () {

            if (!_$projectTenantInformationForm.valid()) {
                return;
            }
            if ($('#ProjectTenant_ProjectEnvironmentId').prop('required') && $('#ProjectTenant_ProjectEnvironmentId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectEnvironment')));
                return;
            }

            var projectTenant = _$projectTenantInformationForm.serializeFormToObject();

            if (projectTenant.projectEnvironmentId == '') {
                abp.message.error('Project Environment must be selected');
                return;
            }

            _modalManager.setBusy(true);
            _projectTenantsService.createOrEdit(
                projectTenant
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditProjectTenantModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };

    };
})(jQuery);