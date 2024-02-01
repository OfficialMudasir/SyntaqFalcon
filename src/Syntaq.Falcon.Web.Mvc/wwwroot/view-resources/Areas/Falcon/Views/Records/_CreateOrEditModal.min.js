(function ($) {
	app.modals.CreateOrEditRecordModal = function () {

		var _recordsService = abp.services.app.records;

		var _modalManager;
		var _$recordInformationForm = null;

		this.init = function (modalManager) {
			_modalManager = modalManager;

			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});

			_$recordInformationForm = _modalManager.getModal().find('form[name=RecordInformationsForm]');
			_$recordInformationForm.validate();
		};

		this.save = function () {
			if (!_$recordInformationForm.valid()) {
				return;
			}

			var record = _$recordInformationForm.serializeFormToObject();

			var rec = JSON.stringify(record);

			 _modalManager.setBusy(true);
			 _recordsService.createOrEdit(
				record
			 ).done(function () {
			   abp.notify.info(app.localize('SavedSuccessfully'));
				 _modalManager.close();
				 abp.event.trigger('app.createOrEditRecordModalSaved');
			 }).always(function () {
			   _modalManager.setBusy(false);
			});
		};
	};
})(jQuery);