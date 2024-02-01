﻿(function ($) {
	app.modals.FormSettingsModal = function () {
		var _formsService = abp.services.app.forms;
		var _paymentsService = abp.services.app.payments;
		var _mergeTextsService = abp.services.app.mergeTexts;

		var _modalManager;
		var _$formGeneralForm = null;
		var _$formButtonSetting = null;
		var _$formPaymentSetting = null;

		function loadPaymentsPartialView(type) {
			$.ajax({
				url: '/Falcon/Payments/PaymentsPartial',
				type: 'POST',
				data: { Type: type, OriginalId: JSONObj.OriginalId, Version: JSONObj.Version },
				dataType: 'html'
			}).done(function (partialViewResult) {
				$("#FormPaymentTab").html(partialViewResult);
			});
		}

		function loadMText() {
			 _mergeTextsService.getMergeTextForView(
				"Form",
				JSONObj.Id
			).done(function (result) {
				$('#MergeTextId').val(result.mergeText.id);
			});
		};

		this.init = function (modalManager) {
			_modalManager = modalManager;

			_$formGeneralForm = _modalManager.getModal().find('form[name=FormGeneralInformations]');
			_$formGeneralForm.validate();

			_$formButtonSetting = _modalManager.getModal().find('form[name=FormButtonSetting]');
			_$formButtonSetting.validate();

			_$formMtextSetting = _modalManager.getModal().find('form[name=FormMtextSetting]');
			_$formMtextSetting.validate();

			loadPaymentsPartialView("Form");

			loadMText();
		};

		function cloneAppendTemplate() {
			var clone = $('#MTextKeyValueTemplate').clone();
			clone.removeAttr("id");
			$('#MTextKeyValueFormblock').append(clone);
		}

		$('#AddItemButton').click(function () {
			event.preventDefault();
			cloneAppendTemplate();
			$(this).blur();
		})

		$('#AddListButton').click(function () {
			event.preventDefault();
			_mergeTextsService.createMergeTextItem({
				//Id
				Name: "Temp",
				MergeTextId: $('#MergeTextId').val(),
				MergeTextEntityType: "Form",
				MergeTextEntityKey: JSONObj.Id,
			}).done(function () {
				abp.notify.info(app.localize('Merge Text List Created'));
			})
			$(this).blur();
		})

		$('#SaveListButton').click(async function () {
			event.preventDefault();
			if ($('#MergeTextId').val() === "") {
				await _mergeTextsService.createMergeText({
					MergeTextEntityType: "Form",
					MergeTextEntityKey: JSONObj.Id
				}).done(function (result) {
					$('#MergeTextId').val(result.id);
				});
			}
			var MTextForm = _$formMtextSetting.serializeFormToObject();
			var MTextList = $('#MTextKeyValueFormblock').serializeJSON({ useIntKeysAsArrayIndex: false });
			_mergeTextsService.createOrEditMergeTextItem(
				{
					Id: MTextForm.MergeTextListId,
					//MergeTextEntityType: "Form",
					//MergeTextEntityKey: JSONObj.Id,
					
					Name: MTextForm.MtextListName,
					MergeTextId: $('#MergeTextId').val(),
					MTextList: MTextList.MTextList
				}
			).done(function () {
				abp.notify.info(app.localize('Merge Text Saved'));
			})
			$(this).blur();
		})

		this.save = function () {
			if (!_$formGeneralForm.valid()) {
				return;
			}

			var form = _$formGeneralForm.serializeFormToObject();
			_formsService.toggleForm(JSONObj.Id, form.formToggle);
			var buttons = _$formButtonSetting.serializeFormToObject();
			var jsonSchema = JSON.parse($('#json').text());
			jsonSchema.buttons = buttons;
			jsonSchema.autoSaving = form.hasOwnProperty("autoSaving") ? Boolean(form.autoSaving) : false;
			jsonSchema.stickyMenu = form.hasOwnProperty("stickyMenu") ? Boolean(form.stickyMenu) : false;

			jsonSchema.feedbackForm = form.hasOwnProperty("feedbackForm") ? form.feedbackForm: null;
			
			if (form.formType != 'form') {
				jsonSchema.isBuildTicks = form.hasOwnProperty("isBuildTicks") ? Boolean(form.isBuildTicks) : false;
			}
			_$formPaymentSetting = _modalManager.getModal().find('form[name=FormPaymentSetting]');
			_$formPaymentSetting.validate();
			var payment = _$formPaymentSetting.serializeFormToObject();
			//_paymentsService.updatePaymentSettings(
			//	{
			//		EntityType: "Form",
			//		EntityId: JSONObj.Id,
			//		HasPaymentConfigured: true,
			//		IsPaymentEnabled: payment.enablePayment,
			//		PaymentAmount: payment.paymentValue,
			//		PaymentCurrency: payment.paymentCurrency,
			//		PaymentProcess: payment.paymentProcess,
			//		PaymentProvider: "Stripe"
			//	}
			//);

			_modalManager.setBusy(true);
			_formsService.createOrEdit(
				{
					id: JSONObj.Id,
					OriginalId: JSONObj.OriginalId,
					Name: form.formName,
					Description: form.description,
					StickyMenu: form.StickyMenu,
                    LockOnBuild: form.LockOnBuild,
                    LockToTenant: form.LockToTenant,
                    RequireAuth: form.RequireAuth,
                    PaymentEnabled: payment.enablePayment,
					PaymentAmount: payment.paymentValue,
					PaymentCurrency: payment.paymentCurrency,
					PaymentProcess: payment.paymentProcess,
					//Version: JSONObj.Version,
					VersionName: form.versionName,
					//CurrentVersion: JSONObj.CurrentVersion,
					IsIndex: form.isIndex,
					Schema: JSON.stringify(jsonSchema)
				}
			).done(function () {
				abp.notify.info(app.localize('Form General Setting Saved'));
				abp.event.trigger('app.FormSettingModalSaved');
				window.location = '/Falcon/forms/build?OriginalId=' + JSONObj.OriginalId + '&version=' + JSONObj.Version + '';
			}).always(function () {
				_modalManager.setBusy(false);
			});
		};
	};
})(jQuery);