(function ($) {
    app.modals.CreateOrEditUserAcceptanceModal = function () {

        var _userAcceptancesService = abp.services.app.userAcceptances;

        var _modalManager;
        var _$userAcceptanceInformationForm = null;

		        var _userAcceptanceTypeLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptances/UserAcceptanceTypeLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptances/_UserAcceptanceTypeLookupTableModal.js',
            modalClass: 'UserAcceptanceTypeLookupTableModal'
        });        var _userLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptances/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptances/_UserLookupTableModal.js',
            modalClass: 'UserLookupTableModal'
        });        var _recordMatterContributorLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptances/RecordMatterContributorLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptances/_RecordMatterContributorLookupTableModal.js',
            modalClass: 'RecordMatterContributorLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$userAcceptanceInformationForm = _modalManager.getModal().find('form[name=UserAcceptanceInformationsForm]');
            _$userAcceptanceInformationForm.validate();
        };

		          $('#OpenUserAcceptanceTypeLookupTableButton').click(function () {

            var userAcceptance = _$userAcceptanceInformationForm.serializeFormToObject();

            _userAcceptanceTypeLookupTableModal.open({ id: userAcceptance.userAcceptanceTypeId, displayName: userAcceptance.userAcceptanceTypeName }, function (data) {
                _$userAcceptanceInformationForm.find('input[name=userAcceptanceTypeName]').val(data.displayName); 
                _$userAcceptanceInformationForm.find('input[name=userAcceptanceTypeId]').val(data.id); 
            });
        });
		
		$('#ClearUserAcceptanceTypeNameButton').click(function () {
                _$userAcceptanceInformationForm.find('input[name=userAcceptanceTypeName]').val(''); 
                _$userAcceptanceInformationForm.find('input[name=userAcceptanceTypeId]').val(''); 
        });
		
        $('#OpenUserLookupTableButton').click(function () {

            var userAcceptance = _$userAcceptanceInformationForm.serializeFormToObject();

            _userLookupTableModal.open({ id: userAcceptance.userId, displayName: userAcceptance.userName }, function (data) {
                _$userAcceptanceInformationForm.find('input[name=userName]').val(data.displayName); 
                _$userAcceptanceInformationForm.find('input[name=userId]').val(data.id); 
            });
        });
		
		$('#ClearUserNameButton').click(function () {
                _$userAcceptanceInformationForm.find('input[name=userName]').val(''); 
                _$userAcceptanceInformationForm.find('input[name=userId]').val(''); 
        });
		
        $('#OpenRecordMatterContributorLookupTableButton').click(function () {

            var userAcceptance = _$userAcceptanceInformationForm.serializeFormToObject();

            _recordMatterContributorLookupTableModal.open({ id: userAcceptance.recordMatterContributorId, displayName: userAcceptance.recordMatterContributorName }, function (data) {
                _$userAcceptanceInformationForm.find('input[name=recordMatterContributorName]').val(data.displayName); 
                _$userAcceptanceInformationForm.find('input[name=recordMatterContributorId]').val(data.id); 
            });
        });
		
		$('#ClearRecordMatterContributorNameButton').click(function () {
                _$userAcceptanceInformationForm.find('input[name=recordMatterContributorName]').val(''); 
                _$userAcceptanceInformationForm.find('input[name=recordMatterContributorId]').val(''); 
        });
		


        this.save = function () {
            if (!_$userAcceptanceInformationForm.valid()) {
                return;
            }

            var userAcceptance = _$userAcceptanceInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _userAcceptancesService.createOrEdit(
				userAcceptance
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditUserAcceptanceModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);