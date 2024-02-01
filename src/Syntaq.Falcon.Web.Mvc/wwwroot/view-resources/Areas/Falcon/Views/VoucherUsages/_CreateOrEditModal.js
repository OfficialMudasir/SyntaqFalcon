(function ($) {
    app.modals.CreateOrEditVoucherUsageModal = function () {

        var _voucherUsagesService = abp.services.app.voucherUsages;

        var _modalManager;
        var _$voucherUsageInformationForm = null;

		var _userLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherUsages/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/VoucherUsages/_UserLookupTableModal.js',
            modalClass: 'UserLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$voucherUsageInformationForm = _modalManager.getModal().find('form[name=VoucherUsageInformationsForm]');
            _$voucherUsageInformationForm.validate();
        };

		          $('#OpenUserLookupTableButton').click(function () {

            var voucherUsage = _$voucherUsageInformationForm.serializeFormToObject();

            _userLookupTableModal.open({ id: voucherUsage.userId, displayName: voucherUsage.userName }, function (data) {
                _$voucherUsageInformationForm.find('input[name=userName]').val(data.displayName); 
                _$voucherUsageInformationForm.find('input[name=userId]').val(data.id); 
            });
        });
		
		$('#ClearUserNameButton').click(function () {
                _$voucherUsageInformationForm.find('input[name=userName]').val(''); 
                _$voucherUsageInformationForm.find('input[name=userId]').val(''); 
        });
		


        this.save = function () {
            if (!_$voucherUsageInformationForm.valid()) {
                return;
            }

            var voucherUsage = _$voucherUsageInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _voucherUsagesService.createOrEdit(
				voucherUsage
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditVoucherUsageModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);