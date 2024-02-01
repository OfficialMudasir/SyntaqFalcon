(function ($) {
	
	app.modals.CreateOrEditAppModal = function () {

		var _appsService = abp.services.app.apps;

		var _modalManager;
		var _$appInformationForm = null;

		
		this.init = function (modalManager) {
			_modalManager = modalManager;

			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});

			_$appInformationForm = _modalManager.getModal().find('form[name=AppInformationsForm]');
			_$appInformationForm.validate();
		};

  
		this.save = function () {
			if (!_$appInformationForm.valid()) {
				return;
			}

			var app = _$appInformationForm.serializeFormToObject();
			
			_modalManager.setBusy(true);
			_appsService.createOrEdit(
				app
			).done(function () {
				abp.notify.info(abp.localization.localize('SavedSuccessfully'));
				_modalManager.close();
				abp.event.trigger('app.createOrEditAppModalSaved');
			}).always(function () {
			   _modalManager.setBusy(false);
			});
		};

	};
})(jQuery);