(function ($) {
	app.modals.CreateOrEditFormModal = function () {

		var _formsService = abp.services.app.forms;

		var _modalManager;
		var _$formInformationForm = null;

		//create or Edit Version Modal 
		var _createVersionModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Forms/CreateVersionModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_CreateVersionModal.js',
			modalClass: 'CreateVersionModal'
		});

		var _setAliveModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Forms/SetAliveModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_SetAliveModal.js',
			modalClass: 'SetAliveModal'
		});

		this.init = function (modalManager) {
			_modalManager = modalManager;

			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});

			_$formInformationForm = _modalManager.getModal().find('form[name=FormInformationsForm]');
			_$formInformationForm.validate();

			$('[name=btn-setlive]').click(function (event) {

				var Id2 = $(this).data('id');
				var Id = $(this).data('originalid');
				var Version = $(this).data('version');
				var versionname = $(this).data('versionname');


				_formsService.setCurrent({
					originalId: Id, version: Version, versionname: versionname
				}).done(function () {
					_modalManager.close();
					abp.event.trigger('app.createOrEditFormModalSetLive');
				});
			});

		};

		$('#SetLiveFormButton').click(function (event, OriginalId, Version, VersionName) {
			_setAliveModal.open({ id: $('#id').val(), "OriginalId": OriginalId, "Version": Version, "Name": $('#Form_Name').val(), "VersionName": VersionName });
			_modalManager.close();

			//_formsService.setCurrent({
			//	originalId: OriginalId,
			//	version: Version
			//}).done(function () {
			//	_modalManager.close();
			//	abp.event.trigger('app.createOrEditFormModalSetLive');
			//});
		});

		$('#ToggleFormButton').click(function (event, Id, Toggle) {
			_formsService.toggleForm(
				Id,
				Toggle
			).done(function () {
				_modalManager.close();
				abp.event.trigger('app.createOrEditFormModalToggled');
			});
		});

		$('#DeleteFormButton').click(function (event, Id) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_formsService.deleteIndividual({
							id: Id
						}).done(function () {
							_modalManager.close();
							abp.event.trigger('app.createOrEditFormModalDeleted');
						});
					}
				}
			);
		});

		this.save = function () {
			if (!_$formInformationForm.valid()) {
				return;
			}

			var form = _$formInformationForm.serializeFormToObject();

			_modalManager.setBusy(true);
			_formsService.createOrEdit(
				form
			).done(function () {
				abp.notify.info(app.localize('SavedSuccessfully'));
				_modalManager.close();
				abp.event.trigger('app.createOrEditFormModalSaved');
			}).always(function () {
				_modalManager.setBusy(false);
			});
		};
	};
})(jQuery);