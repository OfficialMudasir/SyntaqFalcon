﻿(function ($) {
    app.modals.CreateOrEditFormFeedbackModal = function () {

        var _formFeedbacksService = abp.services.app.formFeedbacks;

        var _modalManager;
        var _$formFeedbackInformationForm = null;

		        var _FormFeedbackformLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/FormFeedbacks/FormLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormFeedbacks/_FormFeedbackFormLookupTableModal.js',
            modalClass: 'FormLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$formFeedbackInformationForm = _modalManager.getModal().find('form[name=FormFeedbackInformationsForm]');
            _$formFeedbackInformationForm.validate();
        };

		          $('#OpenFormLookupTableButton').click(function () {

            var formFeedback = _$formFeedbackInformationForm.serializeFormToObject();

            _FormFeedbackformLookupTableModal.open({ id: formFeedback.formId, displayName: formFeedback.formName }, function (data) {
                _$formFeedbackInformationForm.find('input[name=formName]').val(data.displayName); 
                _$formFeedbackInformationForm.find('input[name=formId]').val(data.id); 
            });
        });
		
		$('#ClearFormNameButton').click(function () {
                _$formFeedbackInformationForm.find('input[name=formName]').val(''); 
                _$formFeedbackInformationForm.find('input[name=formId]').val(''); 
        });
		


        this.save = function () {
            if (!_$formFeedbackInformationForm.valid()) {
                return;
            }
            if ($('#FormFeedback_FormId').prop('required') && $('#FormFeedback_FormId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Form')));
                return;
            }

            var formFeedback = _$formFeedbackInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _formFeedbacksService.createOrEdit(
				formFeedback
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditFormFeedbackModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);