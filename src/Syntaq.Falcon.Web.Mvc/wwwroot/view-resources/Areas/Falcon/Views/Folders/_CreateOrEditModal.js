(function ($) {
    app.modals.CreateOrEditFolderModal = function () {

        var _foldersService = abp.services.app.folders;

        var _modalManager;
        var _$folderInformationForm = null;

		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$folderInformationForm = _modalManager.getModal().find('form[name=FolderInformationsForm]');
            _$folderInformationForm.validate();
        };

		  

        this.save = function () {
            if (!_$folderInformationForm.valid()) {
                return;
            }

            var folder = _$folderInformationForm.serializeFormToObject();
			
            _modalManager.setBusy(true);
            _foldersService.createOrEdit(
                folder
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditFolderModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);