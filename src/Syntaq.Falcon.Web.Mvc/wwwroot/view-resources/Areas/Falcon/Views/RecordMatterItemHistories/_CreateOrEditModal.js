﻿(function ($) {
    app.modals.CreateOrEditRecordMatterItemHistoryModal = function () {

        var _recordMatterItemHistoriesService = abp.services.app.recordMatterItemHistories;

        var _modalManager;
        var _$recordMatterItemHistoryInformationForm = null;

		        var _RecordMatterItemHistoryrecordMatterItemLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItemHistories/RecordMatterItemLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItemHistories/_RecordMatterItemHistoryRecordMatterItemLookupTableModal.js',
            modalClass: 'RecordMatterItemLookupTableModal'
        });        var _RecordMatterItemHistoryformLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItemHistories/FormLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItemHistories/_RecordMatterItemHistoryFormLookupTableModal.js',
            modalClass: 'FormLookupTableModal'
        });        var _RecordMatterItemHistorysubmissionLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItemHistories/SubmissionLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItemHistories/_RecordMatterItemHistorySubmissionLookupTableModal.js',
            modalClass: 'SubmissionLookupTableModal'
        });
		
		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$recordMatterItemHistoryInformationForm = _modalManager.getModal().find('form[name=RecordMatterItemHistoryInformationsForm]');
            _$recordMatterItemHistoryInformationForm.validate();
        };

		          $('#OpenRecordMatterItemLookupTableButton').click(function () {

            var recordMatterItemHistory = _$recordMatterItemHistoryInformationForm.serializeFormToObject();

            _RecordMatterItemHistoryrecordMatterItemLookupTableModal.open({ id: recordMatterItemHistory.recordMatterItemId, displayName: recordMatterItemHistory.recordMatterItemDocumentName }, function (data) {
                _$recordMatterItemHistoryInformationForm.find('input[name=recordMatterItemDocumentName]').val(data.displayName); 
                _$recordMatterItemHistoryInformationForm.find('input[name=recordMatterItemId]').val(data.id); 
            });
        });
		
		$('#ClearRecordMatterItemDocumentNameButton').click(function () {
                _$recordMatterItemHistoryInformationForm.find('input[name=recordMatterItemDocumentName]').val(''); 
                _$recordMatterItemHistoryInformationForm.find('input[name=recordMatterItemId]').val(''); 
        });
		
        $('#OpenFormLookupTableButton').click(function () {

            var recordMatterItemHistory = _$recordMatterItemHistoryInformationForm.serializeFormToObject();

            _RecordMatterItemHistoryformLookupTableModal.open({ id: recordMatterItemHistory.formId, displayName: recordMatterItemHistory.formName }, function (data) {
                _$recordMatterItemHistoryInformationForm.find('input[name=formName]').val(data.displayName); 
                _$recordMatterItemHistoryInformationForm.find('input[name=formId]').val(data.id); 
            });
        });
		
		$('#ClearFormNameButton').click(function () {
                _$recordMatterItemHistoryInformationForm.find('input[name=formName]').val(''); 
                _$recordMatterItemHistoryInformationForm.find('input[name=formId]').val(''); 
        });
		
        $('#OpenSubmissionLookupTableButton').click(function () {

            var recordMatterItemHistory = _$recordMatterItemHistoryInformationForm.serializeFormToObject();

            _RecordMatterItemHistorysubmissionLookupTableModal.open({ id: recordMatterItemHistory.submissionId, displayName: recordMatterItemHistory.submissionSubmissionStatus }, function (data) {
                _$recordMatterItemHistoryInformationForm.find('input[name=submissionSubmissionStatus]').val(data.displayName); 
                _$recordMatterItemHistoryInformationForm.find('input[name=submissionId]').val(data.id); 
            });
        });
		
		$('#ClearSubmissionSubmissionStatusButton').click(function () {
                _$recordMatterItemHistoryInformationForm.find('input[name=submissionSubmissionStatus]').val(''); 
                _$recordMatterItemHistoryInformationForm.find('input[name=submissionId]').val(''); 
        });
		


        this.save = function () {
            if (!_$recordMatterItemHistoryInformationForm.valid()) {
                return;
            }
            if ($('#RecordMatterItemHistory_RecordMatterItemId').prop('required') && $('#RecordMatterItemHistory_RecordMatterItemId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('RecordMatterItem')));
                return;
            }
            if ($('#RecordMatterItemHistory_FormId').prop('required') && $('#RecordMatterItemHistory_FormId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Form')));
                return;
            }
            if ($('#RecordMatterItemHistory_SubmissionId').prop('required') && $('#RecordMatterItemHistory_SubmissionId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Submission')));
                return;
            }

            

            var recordMatterItemHistory = _$recordMatterItemHistoryInformationForm.serializeFormToObject();
            
            
            
			
			 _modalManager.setBusy(true);
			 _recordMatterItemHistoriesService.createOrEdit(
				recordMatterItemHistory
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditRecordMatterItemHistoryModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
        
        
    };
})(jQuery);