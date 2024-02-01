(function ($) {
    app.modals.CreateOrEditFileModal = function () {

        var _filesService = abp.services.app.files;

        var _modalManager;
        var _$fileInformationForm = null;

		        var _recordLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Files/RecordLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Files/_RecordLookupTableModal.js',
            modalClass: 'RecordLookupTableModal'
        });        var _recordMatterLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Files/RecordMatterLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Files/_RecordMatterLookupTableModal.js',
            modalClass: 'RecordMatterLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$fileInformationForm = _modalManager.getModal().find('form[name=FileInformationsForm]');
            _$fileInformationForm.validate();
        };

		          $('#OpenRecordLookupTableButton').click(function () {

            var file = _$fileInformationForm.serializeFormToObject();

            _recordLookupTableModal.open({ id: file.recordId, displayName: file.recordRecordName }, function (data) {
                _$fileInformationForm.find('input[name=recordRecordName]').val(data.displayName); 
                _$fileInformationForm.find('input[name=recordId]').val(data.id); 
            });
        });
		
		$('#ClearRecordRecordNameButton').click(function () {
                _$fileInformationForm.find('input[name=recordRecordName]').val(''); 
                _$fileInformationForm.find('input[name=recordId]').val(''); 
        });
		
        $('#OpenRecordMatterLookupTableButton').click(function () {

            var file = _$fileInformationForm.serializeFormToObject();

            _recordMatterLookupTableModal.open({ id: file.recordMatterId, displayName: file.recordMatterRecordMatterName }, function (data) {
                _$fileInformationForm.find('input[name=recordMatterRecordMatterName]').val(data.displayName); 
                _$fileInformationForm.find('input[name=recordMatterId]').val(data.id); 
            });
        });
		
		$('#ClearRecordMatterRecordMatterNameButton').click(function () {
                _$fileInformationForm.find('input[name=recordMatterRecordMatterName]').val(''); 
                _$fileInformationForm.find('input[name=recordMatterId]').val(''); 
        });
		


        this.save = function () {
            if (!_$fileInformationForm.valid()) {
                return;
            }

            var file = _$fileInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _filesService.createOrEdit(
				file
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditFileModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);