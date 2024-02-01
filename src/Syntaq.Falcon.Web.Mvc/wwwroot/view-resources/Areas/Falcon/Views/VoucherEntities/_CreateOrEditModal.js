(function ($) {
    app.modals.CreateOrEditVoucherEntityModal = function () {

        var _voucherEntitiesService = abp.services.app.voucherEntities;
        var _modalManager;
        var _$voucherEntityInformationForm = null;
            var _voucherLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherEntities/VoucherLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/VoucherEntities/_VoucherLookupTableModal.js',
            modalClass: 'VoucherLookupTableModal'
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$voucherEntityInformationForm = _modalManager.getModal().find('form[name=VoucherEntityInformationsForm]');
            _$voucherEntityInformationForm.validate();
        };

        $('#OpenVoucherLookupTableButton').click(function () {

            var voucherEntity = _$voucherEntityInformationForm.serializeFormToObject();

            _voucherLookupTableModal.open({ id: voucherEntity.voucherId, displayName: voucherEntity.voucherTenantId }, function (data) {
                _$voucherEntityInformationForm.find('input[name=voucherTenantId]').val(data.displayName); 
                _$voucherEntityInformationForm.find('input[name=voucherId]').val(data.id); 
            });
        });
		
		$('#ClearVoucherTenantIdButton').click(function () {
                _$voucherEntityInformationForm.find('input[name=voucherTenantId]').val(''); 
                _$voucherEntityInformationForm.find('input[name=voucherId]').val(''); 
        });
		
        this.save = function () {
            if (!_$voucherEntityInformationForm.valid()) {
                return;
            }

            var voucherEntity = _$voucherEntityInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _voucherEntitiesService.createOrEdit(
				voucherEntity
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditVoucherEntityModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };

})(jQuery);