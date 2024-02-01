﻿(function ($) {
  app.modals.CreateOrEditProjectEnvironmentModal = function () {
    var _projectEnvironmentsService = abp.services.app.projectEnvironments;

    var _modalManager;
    var _$projectEnvironmentInformationForm = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;

      var modal = _modalManager.getModal();
      modal.find('.date-picker').daterangepicker({
        singleDatePicker: true,
        locale: abp.localization.currentLanguage.name,
        format: 'L',
      });

      _$projectEnvironmentInformationForm = _modalManager
        .getModal()
        .find('form[name=ProjectEnvironmentInformationsForm]');
      _$projectEnvironmentInformationForm.validate();
    };

    this.save = function () {
      if (!_$projectEnvironmentInformationForm.valid()) {
        return;
      }

      var projectEnvironment = _$projectEnvironmentInformationForm.serializeFormToObject();

      _modalManager.setBusy(true);
      _projectEnvironmentsService
        .createOrEdit(projectEnvironment)
        .done(function () {
          abp.notify.info(app.localize('SavedSuccessfully'));
          _modalManager.close();
          abp.event.trigger('app.createOrEditProjectEnvironmentModalSaved');
        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})(jQuery);
