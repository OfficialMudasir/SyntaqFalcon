﻿/// <reference path="../folders/_folderslookuptablemodal.js" />
/// <reference path="../folders/_folderslookuptablemodal.js" />
//(function ($) {

 
   // app.modals.UploadFormRulesSchemaViewModel = function () {
		//var _modalManager;
	//	var _filesService = abp.services.app.files;

	//	this.init = function (modalManager) {

           // _modalManager = modalManager;

			$('#btnsubmit').click(function () {
				$('#ImportProjectTemplate').submit();
			});

			//$('#btnformfolder').click(function () { 
			//	var _formFolderModal = new app.ModalManager({
			//		viewUrl: abp.appPath + 'Falcon/Folders/FoldersLookupTableModal',
			//		scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Folders/_FoldersLookupTableModal.js',
			//		modalClass: 'FoldersLookupTableModal'
			//	});
			//	_formFolderModal.open(); 
			//});

            $('#ImportProjectTemplate').ajaxForm({
				beforeSubmit: function (formData, jqForm, options) {

					abp.ui.setBusy($('.modal-content'));

                    var $fileInput = $('#ImportProjectTemplate input[name=file]');
					var files = $fileInput.get()[0].files;

					if (!files.length) {
						return false;
					}

					var file = files[0];

					//File type check
					var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
					if ('|x-zip-compressed|'.indexOf(type) === -1) {
						abp.message.warn(app.localize('WarnFileType'));
						return false;
					}

					//File size check
					if (file.size > 104857600) //100MB
					{
						abp.message.warn(app.localize('ProjectTemplateWarnSizeLimit', '100Mb'));
						return false;
					}

					var mimeType = _.filter(formData, { name: 'file' })[0].value.type;

					formData.push({ name: 'FileType', value: mimeType });
					//formData.push({ name: 'FileName', value: 'FormSchema' });
					//formData.push({ name: 'FileToken', value: app.guid() });

					return true;
				}, success: function (response) {
					$('.modal').modal('hide');
					abp.ui.clearBusy();
					location.reload();
					if (response.success) {
						abp.notify.success(app.localize('Project Template SchemaUpdated'));
						_modalManager.close();
					} else {
						abp.message.error(app.localize('Project Template Failed'));
					}

				}
			});
	//	};
	//};
//})(jQuery);