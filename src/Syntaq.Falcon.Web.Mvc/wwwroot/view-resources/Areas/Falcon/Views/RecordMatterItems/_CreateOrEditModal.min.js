(function ($) {
    app.modals.CreateOrEditRecordMatterItemModal = function () {

        var _recordMatterItemsService = abp.services.app.recordMatterItems;

        var _modalManager;
        var _$recordMatterItemInformationForm = null;

		var _recordMatterLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItems/RecordMatterLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItems/_RecordMatterLookupTableModal.js',
            modalClass: 'RecordMatterLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$recordMatterItemInformationForm = _modalManager.getModal().find('form[name=RecordMatterItemInformationsForm]');
            _$recordMatterItemInformationForm.validate();
        };

		          $('#OpenRecordMatterLookupTableButton').click(function () {

            var recordMatterItem = _$recordMatterItemInformationForm.serializeFormToObject();

            _recordMatterLookupTableModal.open({ id: recordMatterItem.recordMatterId, displayName: recordMatterItem.recordMatterTenantId }, function (data) {
                _$recordMatterItemInformationForm.find('input[name=recordMatterTenantId]').val(data.displayName); 
                _$recordMatterItemInformationForm.find('input[name=recordMatterId]').val(data.id); 
            });
        });
		
		$('#ClearRecordMatterTenantIdButton').click(function () {
                _$recordMatterItemInformationForm.find('input[name=recordMatterTenantId]').val(''); 
                _$recordMatterItemInformationForm.find('input[name=recordMatterId]').val(''); 
        });
		


        this.save = function () {
            if (!_$recordMatterItemInformationForm.valid()) {
                return;
            }

            var recordMatterItem = _$recordMatterItemInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _recordMatterItemsService.createOrEdit(
				recordMatterItem
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditRecordMatterItemModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);