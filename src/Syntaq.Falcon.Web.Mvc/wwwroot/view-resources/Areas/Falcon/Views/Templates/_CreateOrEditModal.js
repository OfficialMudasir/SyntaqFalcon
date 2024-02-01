(function ($) {
	app.modals.CreateOrEditTemplateModal = function () {

		var _templatesService = abp.services.app.templates;

		var _modalManager;
		var _$templateInformationForm = null;

		var _setAliveModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Templates/SetAliveModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Templates/_SetAliveModal.js',
			modalClass: 'SetAliveModal'
		});

		this.init = function (modalManager) {
			_modalManager = modalManager;
			_$templateInformationForm = _modalManager.getModal().find('form[name=TemplateInformationsForm]');
			_$templateInformationForm.validate();

			var clipboard = new ClipboardJS('.CopyTemplateLink');

			//For Debugging Copy
			clipboard.on('success', function (e) {
			    //console.log(e);
				abp.notify.success('', 'Copied Document URL', { 'positionClass': 'toast-bottom-right' });
			});
			clipboard.on('error', function (e) {
			    //console.log(e);
				abp.notify.warn('', 'Failed To Copy Document URL', { 'positionClass': 'toast-bottom-right' });
			});

			_modalManager.close(function (event) {
				if ($('#IsDirty').val() == "true") {
					event.preventDefault();
					event.stopImmediatePropagation();
					abp.message.confirm(
						'You have unsaved changes. Do you still want to cancel?',
						app.localize('AreYouSure'),
						function (isConfirmed) {
							if (isConfirmed) {
								$('#IsDirty').val(false);
								_modalManager.close();
							} else {
								return false;
							}
						}
					);
				}
			});
		};

		$("[name='TemplateInformationsForm']").change(function () {
			$("#IsDirty").val(true);
		});
 

		$(".modal-body").find("[name^='UploadDocumenttemplateModalForm']").change(function () {
			$("#IsDirty").val(true);
			var form = $(this).closest('form').submit();
		});

		var formNumber;

		$("[name^='UploadDocumenttemplateModalForm']").ajaxForm({
			beforeSubmit: function (formData, jqForm, options) {
				var action = jqForm[0].name;
				formNumber = action.substring(32);
				var files = jqForm[0][1].files;

				if (!files.length) {
					return false;
				}

				var file = files[0];
				var ext = file.name.slice(file.name.lastIndexOf('.'));
				var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
				if ('|msword|vnd.openxmlformats-officedocument.wordprocessingml.document|'.indexOf(type) === -1 && (ext.toLowerCase().indexOf('.docx') < 0 || ext.toLowerCase().indexOf('.doc'))) {
					abp.message.warn(app.localize('DocumentTemplate_Warn_FileType'));
					return false;
				}

				//File size check
				if (file.size > 10485760) //10MB
				{
					abp.message.warn(app.localize('DocumentTemplate_Warn_SizeLimit', app.maxProfilPictureBytesUserFriendlyValue));
					return false;
				}
				try {
					if (formData[1].value.name !== undefined) {
						$('input[id=Template_DocumentName]')[0].value = formData[1].value.name;
					}
				}
				catch (e) { }

				return true;
			},
			success: function (response) {
				if (response.success) {
					$("#Template_Version").val(formNumber);
					$("[name='HasFile']").val('True');
					//abp.message.success(app.localize('DocumentTemplate_Success_Upload'));
				} else {
					abp.message.error(response.error.message);
				}
			}
		});

		$('#SetLiveTemplateButton').click(function (event, Id, Version) {
			var currentVersion = $('#Template_CurrentVersion').val();
			var name = $('#Template_Name').val();

			_setAliveModal.open({ "OriginalId": Id, "Version": Version, currentV: currentVersion, name: name});
			_modalManager.close();
 
		});

		$('#DeleteTemplateButton').click(function (event, Id) {
			abp.message.confirm(
				'',
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_templatesService.deleteIndividual({
							id: Id
						}).done(function (result) {
							//_modalManager.close();
							//abp.event.trigger('app.createOrEditTemplateModalDeleted');
							if (result.success == true) {
								_modalManager.close();
								abp.event.trigger('app.createOrEditTemplateModalDeleted');
							}
						});
					}
				}
			);
		});

		this.save = function () {
			if (!_$templateInformationForm.valid()) {
				return;
			}

			var template = _$templateInformationForm.serializeFormToObject();

			//if (template.HasFile === "False") {
			//    abp.message.warn(app.localize('DocumentTemplate_Warn_NoFile'));
			//    return;
			// }
			//else {
			var latestV = parseInt($('#latestVersionNumber').attr("latestversionnumber"));
			if (latestV != 0
				&& template.HasFile
				&& template.version === "0") {
				swal("Do you want to set this new template as live?", {
					buttons: ["No", "Yes"],
				}).then((value) => {
					$('#SetToLive').val(false);
					if (value) {
						$('#SetToLive').val(true);
					}
					save();
				})
			} else {
				save()
			}

			function save() {
				_modalManager.setBusy(true);
				_templatesService.createOrEdit(
					template
				).done(function () {
					$('#IsDirty').val(false);
					abp.notify.info(app.localize('SavedSuccessfully'));
					if ($('#SetToLive').val() === "true") {
						SetToLive(template.originalId);
						console.log(template);
					}
					_modalManager.close();
					abp.event.trigger('app.createOrEditTemplateModalSaved');
				}).always(function () {
					_modalManager.setBusy(false);
				});

			}

			function SetToLive(Id) {
				var v = parseInt($('#latestVersionNumber').attr("latestversionnumber"));
				var currentVersion = $('#Template_CurrentVersion').val();
				var name = $('#Template_Name').val();
				_setAliveModal.open({ originalId: Id, version: v + 1, currentV: currentVersion, name: name});


				//_templatesService.setCurrent({
				//	originalId: Id, version: v + 1
				//});
			}

		};
	};
})(jQuery);