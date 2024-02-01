(function ($) {
	app.modals.CreateVersionModal = function (data) {

		var _formsService = abp.services.app.forms;

		var _modalManager;

		this.init = function (modalManager) {
			_modalManager = modalManager;
			var modal = _modalManager.getModal();
		};

		this.save = function () {
			var formVersion = (parseInt($('[name="FormVersion"]').val()) + 1);
			//formVersion = (parseInt(formVersion) + 1);
			var FormId = $('[name="FormId"]').val();
			var FormOriginalId = $('[name="FormOriginalId"]').val();
			var FormVersionName = $('#FormVersionName').val();

			_modalManager.setBusy(true);
			_formsService.createFormVersion({
				Id: FormId,
				OriginalId: FormOriginalId,
				VersionName: FormVersionName
			}).done(function (result) {
				window.location = '/Falcon/forms/build?OriginalId=' + FormOriginalId + '&version=' + result + '';
				///Falcon/forms/build?OriginalId=' + row.originalId + '&version=live" name="BuildForm">
				abp.notify.info(app.localize('Form Version Saved'));
				_modalManager.close();
			}).always(function () {
				_modalManager.setBusy(false);
			});



			//var mergeText = _$mergeTextInformationForm.serializeFormToObject();

			//_modalManager.setBusy(true);
			//_mergeTextsService.createOrEdit(
			//	mergeText
			//).done(function () {
			//	abp.notify.info(app.localize('SavedSuccessfully'));
			//	_modalManager.close();
			//	abp.event.trigger('app.createOrEditMergeTextModalSaved');
			//}).always(function () {
			//	_modalManager.setBusy(false);
			//});
		};
	};
})(jQuery);