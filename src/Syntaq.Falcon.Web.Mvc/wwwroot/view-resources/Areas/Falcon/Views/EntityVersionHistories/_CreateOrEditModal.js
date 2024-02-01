(function ($) {
    app.modals.CreateOrEditEntityVersionHistoryModal = function () {

        var _entityVersionHistoriesService = abp.services.app.entityVersionHistories;

        var _modalManager;
        var _$entityVersionHistoryInformationForm = null;

		        var _EntityVersionHistoryuserLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/EntityVersionHistories/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/EntityVersionHistories/_EntityVersionHistoryUserLookupTableModal.js',
            modalClass: 'UserLookupTableModal'
        });
		
		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$entityVersionHistoryInformationForm = _modalManager.getModal().find('form[name=EntityVersionHistoryInformationsForm]');
            _$entityVersionHistoryInformationForm.validate();
        };

		          $('#OpenUserLookupTableButton').click(function () {

            var entityVersionHistory = _$entityVersionHistoryInformationForm.serializeFormToObject();

            _EntityVersionHistoryuserLookupTableModal.open({ id: entityVersionHistory.userId, displayName: entityVersionHistory.userName }, function (data) {
                _$entityVersionHistoryInformationForm.find('input[name=userName]').val(data.displayName); 
                _$entityVersionHistoryInformationForm.find('input[name=userId]').val(data.id); 
            });
        });
		
		$('#ClearUserNameButton').click(function () {
                _$entityVersionHistoryInformationForm.find('input[name=userName]').val(''); 
                _$entityVersionHistoryInformationForm.find('input[name=userId]').val(''); 
        });
		


        this.save = function () {
            if (!_$entityVersionHistoryInformationForm.valid()) {
                return;
            }
            if ($('#EntityVersionHistory_UserId').prop('required') && $('#EntityVersionHistory_UserId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('User')));
                return;
            }

            

            var entityVersionHistory = _$entityVersionHistoryInformationForm.serializeFormToObject();
            
            
            
			
			 _modalManager.setBusy(true);
			 _entityVersionHistoriesService.createOrEdit(
				entityVersionHistory
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditEntityVersionHistoryModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
        
        
    };
})(jQuery);