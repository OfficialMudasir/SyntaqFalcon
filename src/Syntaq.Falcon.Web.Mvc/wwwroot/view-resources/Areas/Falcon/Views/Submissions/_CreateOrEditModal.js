(function ($) {
	app.modals.CreateOrEditSubmissionModal = function () {

		var _submissionsService = abp.services.app.submissions;

		var _modalManager;
		var _$submissionInformationForm = null;

				var _SubmissionrecordLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/RecordLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_SubmissionRecordLookupTableModal.js',
			modalClass: 'RecordLookupTableModal'
		});        var _SubmissionrecordMatterLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/RecordMatterLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_SubmissionRecordMatterLookupTableModal.js',
			modalClass: 'RecordMatterLookupTableModal'
		});        var _SubmissionuserLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/UserLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_SubmissionUserLookupTableModal.js',
			modalClass: 'UserLookupTableModal'
		});        var _SubmissionappJobLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/AppJobLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_SubmissionAppJobLookupTableModal.js',
			modalClass: 'AppJobLookupTableModal'
		});

		this.init = function (modalManager) {
			_modalManager = modalManager;

			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});

			_$submissionInformationForm = _modalManager.getModal().find('form[name=SubmissionInformationsForm]');
			_$submissionInformationForm.validate();
		};

				  $('#OpenRecordLookupTableButton').click(function () {

			var submission = _$submissionInformationForm.serializeFormToObject();

			_SubmissionrecordLookupTableModal.open({ id: submission.recordId, displayName: submission.recordRecordName }, function (data) {
				_$submissionInformationForm.find('input[name=recordRecordName]').val(data.displayName); 
				_$submissionInformationForm.find('input[name=recordId]').val(data.id); 
			});
		});
		
		$('#ClearRecordRecordNameButton').click(function () {
				_$submissionInformationForm.find('input[name=recordRecordName]').val(''); 
				_$submissionInformationForm.find('input[name=recordId]').val(''); 
		});
		
		$('#OpenRecordMatterLookupTableButton').click(function () {

			var submission = _$submissionInformationForm.serializeFormToObject();

			_SubmissionrecordMatterLookupTableModal.open({ id: submission.recordMatterId, displayName: submission.recordMatterRecordMatterName }, function (data) {
				_$submissionInformationForm.find('input[name=recordMatterRecordMatterName]').val(data.displayName); 
				_$submissionInformationForm.find('input[name=recordMatterId]').val(data.id); 
			});
		});
		
		$('#ClearRecordMatterRecordMatterNameButton').click(function () {
				_$submissionInformationForm.find('input[name=recordMatterRecordMatterName]').val(''); 
				_$submissionInformationForm.find('input[name=recordMatterId]').val(''); 
		});
		
		$('#OpenUserLookupTableButton').click(function () {

			var submission = _$submissionInformationForm.serializeFormToObject();

			_SubmissionuserLookupTableModal.open({ id: submission.userId, displayName: submission.userName }, function (data) {
				_$submissionInformationForm.find('input[name=userName]').val(data.displayName); 
				_$submissionInformationForm.find('input[name=userId]').val(data.id); 
			});
		});
		
		$('#ClearUserNameButton').click(function () {
				_$submissionInformationForm.find('input[name=userName]').val(''); 
				_$submissionInformationForm.find('input[name=userId]').val(''); 
		});
		
		$('#OpenAppJobLookupTableButton').click(function () {

			var submission = _$submissionInformationForm.serializeFormToObject();

			_SubmissionappJobLookupTableModal.open({ id: submission.appJobId, displayName: submission.appJobName }, function (data) {
				_$submissionInformationForm.find('input[name=appJobName]').val(data.displayName); 
				_$submissionInformationForm.find('input[name=appJobId]').val(data.id); 
			});
		});
		
		$('#ClearAppJobNameButton').click(function () {
				_$submissionInformationForm.find('input[name=appJobName]').val(''); 
				_$submissionInformationForm.find('input[name=appJobId]').val(''); 
		});
		


		this.save = function () {
			if (!_$submissionInformationForm.valid()) {
				return;
			}
			if ($('#Submission_RecordId').prop('required') && $('#Submission_RecordId').val() == '') {
				abp.message.error(app.localize('{0}IsRequired', app.localize('Record')));
				return;
			}
			if ($('#Submission_RecordMatterId').prop('required') && $('#Submission_RecordMatterId').val() == '') {
				abp.message.error(app.localize('{0}IsRequired', app.localize('RecordMatter')));
				return;
			}
			if ($('#Submission_UserId').prop('required') && $('#Submission_UserId').val() == '') {
				abp.message.error(app.localize('{0}IsRequired', app.localize('User')));
				return;
			}
			if ($('#Submission_AppJobId').prop('required') && $('#Submission_AppJobId').val() == '') {
				abp.message.error(app.localize('{0}IsRequired', app.localize('AppJob')));
				return;
			}

			var submission = _$submissionInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _submissionsService.createOrEdit(
				submission
			 ).done(function () {
			   abp.notify.info(app.localize('SavedSuccessfully'));
			   _modalManager.close();
			   abp.event.trigger('app.createOrEditSubmissionModalSaved');
			 }).always(function () {
			   _modalManager.setBusy(false);
			});
		};
	};
})(jQuery);