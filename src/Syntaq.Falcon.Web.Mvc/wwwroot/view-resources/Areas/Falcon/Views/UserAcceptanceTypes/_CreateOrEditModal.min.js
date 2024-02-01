(function ($) {
    app.modals.CreateOrEditUserAcceptanceTypeModal = function () {

        var _userAcceptanceTypesService = abp.services.app.userAcceptanceTypes;

        var _modalManager;
        var _$userAcceptanceTypeInformationForm = null;

		        var _templateLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptanceTypes/TemplateLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptanceTypes/_TemplateLookupTableModal.js',
            modalClass: 'TemplateLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$userAcceptanceTypeInformationForm = _modalManager.getModal().find('form[name=UserAcceptanceTypeInformationsForm]');
            _$userAcceptanceTypeInformationForm.validate();
        };

		          $('#OpenTemplateLookupTableButton').click(function () {

            var userAcceptanceType = _$userAcceptanceTypeInformationForm.serializeFormToObject();

            _templateLookupTableModal.open({ id: userAcceptanceType.templateId, displayName: userAcceptanceType.templateName }, function (data) {
                _$userAcceptanceTypeInformationForm.find('input[name=templateName]').val(data.displayName); 
                _$userAcceptanceTypeInformationForm.find('input[name=templateId]').val(data.id); 
            });
        });
		
		$('#ClearTemplateNameButton').click(function () {
                _$userAcceptanceTypeInformationForm.find('input[name=templateName]').val(''); 
                _$userAcceptanceTypeInformationForm.find('input[name=templateId]').val(''); 
        });
		


        this.save = function () {
            if (!_$userAcceptanceTypeInformationForm.valid()) {
                return;
            }
            if ($(".activeAcceptanceTypeSign").length < 2 && !$('#UserAcceptanceType_Active').is(':checked') && abp.setting.getBoolean('App.UserManagement.IsUserAcceptanceRequired')) {
                alert(app.localize('AtLeastOneActiveUserAcceptanceTypeWarning'));
                return;
            }
            var userAcceptanceType = _$userAcceptanceTypeInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _userAcceptanceTypesService.createOrEdit(
				userAcceptanceType
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditUserAcceptanceTypeModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);