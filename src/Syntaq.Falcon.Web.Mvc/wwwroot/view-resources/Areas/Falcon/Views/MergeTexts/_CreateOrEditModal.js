(function ($) {
	app.modals.CreateOrEditMergeTextModal = function () {

		var _mergeTextsService = abp.services.app.mergeTexts;

		var _modalManager;
		var _$mergeTextInformationForm = null;

				var _MergeTextmergeTextItemLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/MergeTexts/MergeTextItemLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/MergeTexts/_MergeTextMergeTextItemLookupTableModal.js',
			modalClass: 'MergeTextItemLookupTableModal'
		});

		this.init = function (modalManager) {
			_modalManager = modalManager;

			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});

			_$mergeTextInformationForm = _modalManager.getModal().find('form[name=MergeTextInformationsForm]');
			_$mergeTextInformationForm.validate();
		};

				  $('#OpenMergeTextItemLookupTableButton').click(function () {

			var mergeText = _$mergeTextInformationForm.serializeFormToObject();

			_MergeTextmergeTextItemLookupTableModal.open({ id: mergeText.mergeTextItemId, displayName: mergeText.mergeTextItemName }, function (data) {
				_$mergeTextInformationForm.find('input[name=mergeTextItemName]').val(data.displayName); 
				_$mergeTextInformationForm.find('input[name=mergeTextItemId]').val(data.id); 
			});
		});
		
		$('#ClearMergeTextItemNameButton').click(function () {
				_$mergeTextInformationForm.find('input[name=mergeTextItemName]').val(''); 
				_$mergeTextInformationForm.find('input[name=mergeTextItemId]').val(''); 
		});
		


		this.save = function () {
			if (!_$mergeTextInformationForm.valid()) {
				return;
			}
			if ($('#MergeText_MergeTextItemId').prop('required') && $('#MergeText_MergeTextItemId').val() == '') {
				abp.message.error(app.localize('{0}IsRequired', app.localize('MergeTextItem')));
				return;
			}

			var mergeText = _$mergeTextInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _mergeTextsService.createOrEdit(
				mergeText
			 ).done(function () {
			   abp.notify.info(app.localize('SavedSuccessfully'));
			   _modalManager.close();
			   abp.event.trigger('app.createOrEditMergeTextModalSaved');
			 }).always(function () {
			   _modalManager.setBusy(false);
			});
		};
	};
})(jQuery);