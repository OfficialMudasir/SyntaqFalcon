(function ($) {
    app.modals.CreateOrEditMergeTextItemValueModal = function () {

        var _mergeTextItemValuesService = abp.services.app.mergeTextItemValues;

        var _modalManager;
        var _$mergeTextItemValueInformationForm = null;

		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$mergeTextItemValueInformationForm = _modalManager.getModal().find('form[name=MergeTextItemValueInformationsForm]');
            _$mergeTextItemValueInformationForm.validate();
        };

		  

        this.save = function () {
            if (!_$mergeTextItemValueInformationForm.valid()) {
                return;
            }

            var mergeTextItemValue = _$mergeTextItemValueInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _mergeTextItemValuesService.createOrEdit(
				mergeTextItemValue
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditMergeTextItemValueModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);