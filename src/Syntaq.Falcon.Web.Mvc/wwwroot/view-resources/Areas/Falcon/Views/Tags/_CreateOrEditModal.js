(function ($) {
    app.modals.CreateOrEditTagModal = function () {

        var _tagsService = abp.services.app.tags;

        var _modalManager;
        var _$tagInformationForm = null;

		
		
		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });
            _$tagInformationForm = _modalManager.getModal().find('form[name=TagInformationsForm]');
            _$tagInformationForm.validate();
        };

		  

        this.save = function () {
            if (!_$tagInformationForm.valid()) {
                return;
            }

            

            var tag = _$tagInformationForm.serializeFormToObject();
            
            
            
			
			 _modalManager.setBusy(true);
			 _tagsService.createOrEdit(
				tag
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditTagModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
        
        
    };
})(jQuery);