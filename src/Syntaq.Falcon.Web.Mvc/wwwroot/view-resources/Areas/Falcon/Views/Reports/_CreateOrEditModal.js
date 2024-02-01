//(function ($) {
//	app.modals.CreateOrEditReportModal = function () {

//		var _reportsService = abp.services.app.reports;

//		var _modalManager;
//		var _$reportInformationForm = null;

//				var _formLookupTableModal = new app.ModalManager({
//			viewUrl: abp.appPath + 'Falcon/Reports/FormLookupTableModal',
//			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Reports/_FormLookupTableModal.js',
//			modalClass: 'FormLookupTableModal'
//		});        var _userLookupTableModal = new app.ModalManager({
//			viewUrl: abp.appPath + 'Falcon/Reports/UserLookupTableModal',
//			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Reports/_UserLookupTableModal.js',
//			modalClass: 'UserLookupTableModal'
//		});        var _recordLookupTableModal = new app.ModalManager({
//			viewUrl: abp.appPath + 'Falcon/Reports/RecordLookupTableModal',
//			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Reports/_RecordLookupTableModal.js',
//			modalClass: 'RecordLookupTableModal'
//		});

//		this.init = function (modalManager) {
//			_modalManager = modalManager;

//			var modal = _modalManager.getModal();
//			modal.find('.date-picker').datetimepicker({
//				locale: abp.localization.currentLanguage.name,
//				format: 'L'
//			});

//			_$reportInformationForm = _modalManager.getModal().find('form[name=ReportInformationsForm]');
//			_$reportInformationForm.validate();
//		};

//				  $('#OpenFormLookupTableButton').click(function () {

//			var report = _$reportInformationForm.serializeFormToObject();

//			_formLookupTableModal.open({ id: report.formId, displayName: report.formName }, function (data) {
//				_$reportInformationForm.find('input[name=formName]').val(data.displayName); 
//				_$reportInformationForm.find('input[name=formId]').val(data.id); 
//			});
//		});
		
//		$('#ClearFormNameButton').click(function () {
//				_$reportInformationForm.find('input[name=formName]').val(''); 
//				_$reportInformationForm.find('input[name=formId]').val(''); 
//		});
		
//		$('#OpenUserLookupTableButton').click(function () {

//			var report = _$reportInformationForm.serializeFormToObject();

//			_userLookupTableModal.open({ id: report.userId, displayName: report.userName }, function (data) {
//				_$reportInformationForm.find('input[name=userName]').val(data.displayName); 
//				_$reportInformationForm.find('input[name=userId]').val(data.id); 
//			});
//		});
		
//		$('#ClearUserNameButton').click(function () {
//				_$reportInformationForm.find('input[name=userName]').val(''); 
//				_$reportInformationForm.find('input[name=userId]').val(''); 
//		});
		
//		$('#OpenRecordLookupTableButton').click(function () {

//			var report = _$reportInformationForm.serializeFormToObject();

//			_recordLookupTableModal.open({ id: report.recordId, displayName: report.recordRecordName }, function (data) {
//				_$reportInformationForm.find('input[name=recordRecordName]').val(data.displayName); 
//				_$reportInformationForm.find('input[name=recordId]').val(data.id); 
//			});
//		});
		
//		$('#ClearRecordRecordNameButton').click(function () {
//				_$reportInformationForm.find('input[name=recordRecordName]').val(''); 
//				_$reportInformationForm.find('input[name=recordId]').val(''); 
//		});
		


//		this.save = function () {
//			if (!_$reportInformationForm.valid()) {
//				return;
//			}

//			var report = _$reportInformationForm.serializeFormToObject();
			
//			 _modalManager.setBusy(true);
//			 _reportsService.createOrEdit(
//				report
//			 ).done(function () {
//			   abp.notify.info(app.localize('SavedSuccessfully'));
//			   _modalManager.close();
//			   abp.event.trigger('app.createOrEditReportModalSaved');
//			 }).always(function () {
//			   _modalManager.setBusy(false);
//			});
//		};
//	};
//})(jQuery);