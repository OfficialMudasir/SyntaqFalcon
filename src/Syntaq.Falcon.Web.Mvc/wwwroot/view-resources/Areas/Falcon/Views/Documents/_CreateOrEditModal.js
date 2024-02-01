(function ($) {
    app.modals.CreateOrEditDocumentModal = function () {

        var _documentsService = abp.services.app.documents;

        var _modalManager;
        var _$documentInformationForm = null;

		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$documentInformationForm = _modalManager.getModal().find('form[name=DocumentInformationsForm]');
            _$documentInformationForm.validate();
        };

        this.save = function () {
            if (!_$documentInformationForm.valid()) {
                return;
            }

            var document = _$documentInformationForm.serializeFormToObject();
			
                _modalManager.setBusy(true);
                _documentsService.createOrEdit(
				document
                ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditDocumentModalSaved');
                }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);