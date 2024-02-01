﻿(function ($) {
  app.modals.CreateOrEditProjectDeploymentModal = function () {
    var _projectDeploymentsService = abp.services.app.projectDeployments;

    var _modalManager;
    var _$projectDeploymentInformationForm = null;

    var _ProjectDeploymentprojectReleaseLookupTableModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/ProjectDeployments/ProjectReleaseLookupTableModal',
      scriptUrl:
        abp.appPath +
        'view-resources/Areas/Falcon/Views/ProjectDeployments/_ProjectDeploymentProjectReleaseLookupTableModal.js',
      modalClass: 'ProjectReleaseLookupTableModal',
    });

    this.init = function (modalManager) {
      _modalManager = modalManager;

      var modal = _modalManager.getModal();
      modal.find('.date-picker').daterangepicker({
        singleDatePicker: true,
        locale: abp.localization.currentLanguage.name,
        format: 'L',
      });

      _$projectDeploymentInformationForm = _modalManager
        .getModal()
        .find('form[name=ProjectDeploymentInformationsForm]');
      _$projectDeploymentInformationForm.validate();
    };

    $('#OpenProjectReleaseLookupTableButton').click(function () {
      var projectDeployment = _$projectDeploymentInformationForm.serializeFormToObject();

      _ProjectDeploymentprojectReleaseLookupTableModal.open(
        { id: projectDeployment.projectReleaseId, displayName: projectDeployment.projectReleaseName },
        function (data) {
          _$projectDeploymentInformationForm.find('input[name=projectReleaseName]').val(data.displayName);
          _$projectDeploymentInformationForm.find('input[name=projectReleaseId]').val(data.id);
        }
      );
    });

    $('#ClearProjectReleaseNameButton').click(function () {
      _$projectDeploymentInformationForm.find('input[name=projectReleaseName]').val('');
      _$projectDeploymentInformationForm.find('input[name=projectReleaseId]').val('');
    });

    this.save = function () {
      if (!_$projectDeploymentInformationForm.valid()) {
        return;
      }
      if (
        $('#ProjectDeployment_ProjectReleaseId').prop('required') &&
        $('#ProjectDeployment_ProjectReleaseId').val() == ''
      ) {
        abp.message.error(app.localize('{0}IsRequired', app.localize('ProjectRelease')));
        return;
      }

      var projectDeployment = _$projectDeploymentInformationForm.serializeFormToObject();

      _modalManager.setBusy(true);
      _projectDeploymentsService
        .createOrEdit(projectDeployment)
        .done(function () {
          abp.notify.info(app.localize('SavedSuccessfully'));
            _modalManager.close();
            abp.event.trigger('app.createOrEditProjectDeploymentModalSaved');
        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})(jQuery);
