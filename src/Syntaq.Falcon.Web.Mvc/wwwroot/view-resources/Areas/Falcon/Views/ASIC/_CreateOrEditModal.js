(function ($) {
    app.modals.CreateOrEditAsicModal = function () {

        var _asicService = abp.services.app.asic;

        var _modalManager;
        var _$asicInformationForm = null;

		
		
		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$asicInformationForm = _modalManager.getModal().find('form[name=AsicInformationsForm]');
            _$asicInformationForm.validate();
        };

		  

        this.save = function () {
            if (!_$asicInformationForm.valid()) {
                return;
            }

            

            var asic = _$asicInformationForm.serializeFormToObject();
            
            
            
			
			 _modalManager.setBusy(true);
			 _asicService.createOrEdit(
				asic
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditAsicModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
        
        
    };
})(jQuery);