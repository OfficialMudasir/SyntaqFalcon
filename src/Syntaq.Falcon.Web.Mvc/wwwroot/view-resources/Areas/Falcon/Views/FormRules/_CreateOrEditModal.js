(function ($) {
    app.modals.CreateOrEditFormRuleModal = function () {

        var _formRulesService = abp.services.app.formRules;

        var _modalManager;
        var _$formRuleInformationForm = null;

		        var _formLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/FormRules/FormLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormRules/_FormLookupTableModal.js',
            modalClass: 'FormLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$formRuleInformationForm = _modalManager.getModal().find('form[name=FormRuleInformationsForm]');
            _$formRuleInformationForm.validate();
        };

		          $('#OpenFormLookupTableButton').click(function () {

            var formRule = _$formRuleInformationForm.serializeFormToObject();

            _formLookupTableModal.open({ id: formRule.formId, displayName: formRule.formName }, function (data) {
                _$formRuleInformationForm.find('input[name=formName]').val(data.displayName); 
                _$formRuleInformationForm.find('input[name=formId]').val(data.id); 
            });
        });
		
		$('#ClearFormNameButton').click(function () {
                _$formRuleInformationForm.find('input[name=formName]').val(''); 
                _$formRuleInformationForm.find('input[name=formId]').val(''); 
        });
		


        this.save = function () {
            if (!_$formRuleInformationForm.valid()) {
                return;
            }

            var formRule = _$formRuleInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _formRulesService.createOrEdit(
				formRule
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditFormRuleModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);