﻿(function () {
    $(function () {
        var _recordMatterAuditsService = abp.services.app.recordMatterAudits;

        var _$recordMatterAuditInformationForm = $('form[name=RecordMatterAuditInformationsForm]');
        _$recordMatterAuditInformationForm.validate();

		        var _RecordMatterAudituserLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterAudits/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterAudits/_RecordMatterAuditUserLookupTableModal.js',
            modalClass: 'UserLookupTableModal'
        });        var _RecordMatterAuditrecordMatterLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterAudits/RecordMatterLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterAudits/_RecordMatterAuditRecordMatterLookupTableModal.js',
            modalClass: 'RecordMatterLookupTableModal'
        });
   
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});
      
	            $('#OpenUserLookupTableButton').click(function () {

            var recordMatterAudit = _$recordMatterAuditInformationForm.serializeFormToObject();

            _RecordMatterAudituserLookupTableModal.open({ id: recordMatterAudit.userId, displayName: recordMatterAudit.userName }, function (data) {
                _$recordMatterAuditInformationForm.find('input[name=userName]').val(data.displayName); 
                _$recordMatterAuditInformationForm.find('input[name=userId]').val(data.id); 
            });
        });
		
		$('#ClearUserNameButton').click(function () {
                _$recordMatterAuditInformationForm.find('input[name=userName]').val(''); 
                _$recordMatterAuditInformationForm.find('input[name=userId]').val(''); 
        });
		
        $('#OpenRecordMatterLookupTableButton').click(function () {

            var recordMatterAudit = _$recordMatterAuditInformationForm.serializeFormToObject();

            _RecordMatterAuditrecordMatterLookupTableModal.open({ id: recordMatterAudit.recordMatterId, displayName: recordMatterAudit.recordMatterRecordMatterName }, function (data) {
                _$recordMatterAuditInformationForm.find('input[name=recordMatterRecordMatterName]').val(data.displayName); 
                _$recordMatterAuditInformationForm.find('input[name=recordMatterId]').val(data.id); 
            });
        });
		
		$('#ClearRecordMatterRecordMatterNameButton').click(function () {
                _$recordMatterAuditInformationForm.find('input[name=recordMatterRecordMatterName]').val(''); 
                _$recordMatterAuditInformationForm.find('input[name=recordMatterId]').val(''); 
        });
		


        function save(successCallback) {
            if (!_$recordMatterAuditInformationForm.valid()) {
                return;
            }
            if ($('#RecordMatterAudit_UserId').prop('required') && $('#RecordMatterAudit_UserId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('User')));
                return;
            }
            if ($('#RecordMatterAudit_RecordMatterId').prop('required') && $('#RecordMatterAudit_RecordMatterId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('RecordMatter')));
                return;
            }

            var recordMatterAudit = _$recordMatterAuditInformationForm.serializeFormToObject();
			
			 abp.ui.setBusy();
			 _recordMatterAuditsService.createOrEdit(
				recordMatterAudit
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               abp.event.trigger('app.createOrEditRecordMatterAuditModalSaved');
               
               if(typeof(successCallback)==='function'){
                    successCallback();
               }
			 }).always(function () {
			    abp.ui.clearBusy();
			});
        };
        
        function clearForm(){
            _$recordMatterAuditInformationForm[0].reset();
        }
        
        $('#saveBtn').click(function(){
            save(function(){
                window.location="/Falcon/RecordMatterAudits";
            });
        });
        
        $('#saveAndNewBtn').click(function(){
            save(function(){
                if (!$('input[name=id]').val()) {//if it is create page
                   clearForm();
                }
            });
        });
    });
})();