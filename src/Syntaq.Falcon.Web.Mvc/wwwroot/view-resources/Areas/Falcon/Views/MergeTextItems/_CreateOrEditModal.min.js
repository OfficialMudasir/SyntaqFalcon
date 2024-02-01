(function ($) {
    app.modals.CreateOrEditMergeTextItemModal = function () {

        var _mergeTextItemsService = abp.services.app.mergeTextItems;

        var _modalManager;
        var _$mergeTextItemInformationForm = null;

		        var _MergeTextItemmergeTextItemValueLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/MergeTextItems/MergeTextItemValueLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/MergeTextItems/_MergeTextItemMergeTextItemValueLookupTableModal.js',
            modalClass: 'MergeTextItemValueLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$mergeTextItemInformationForm = _modalManager.getModal().find('form[name=MergeTextItemInformationsForm]');
            _$mergeTextItemInformationForm.validate();
        };

		          $('#OpenMergeTextItemValueLookupTableButton').click(function () {

            var mergeTextItem = _$mergeTextItemInformationForm.serializeFormToObject();

            _MergeTextItemmergeTextItemValueLookupTableModal.open({ id: mergeTextItem.mergeTextItemValueId, displayName: mergeTextItem.mergeTextItemValueKey }, function (data) {
                _$mergeTextItemInformationForm.find('input[name=mergeTextItemValueKey]').val(data.displayName); 
                _$mergeTextItemInformationForm.find('input[name=mergeTextItemValueId]').val(data.id); 
            });
        });
		
		$('#ClearMergeTextItemValueKeyButton').click(function () {
                _$mergeTextItemInformationForm.find('input[name=mergeTextItemValueKey]').val(''); 
                _$mergeTextItemInformationForm.find('input[name=mergeTextItemValueId]').val(''); 
        });
		


        this.save = function () {
            if (!_$mergeTextItemInformationForm.valid()) {
                return;
            }
            if ($('#MergeTextItem_MergeTextItemValueId').prop('required') && $('#MergeTextItem_MergeTextItemValueId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('MergeTextItemValue')));
                return;
            }

            var mergeTextItem = _$mergeTextItemInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _mergeTextItemsService.createOrEdit(
				mergeTextItem
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditMergeTextItemModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);