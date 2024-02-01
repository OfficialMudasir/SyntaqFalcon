(function ($) {
    app.modals.CreateOrEditrecordPolicyActionModal = function () {
        var _recordPolicyActionsService = abp.services.app.recordPolicyActions;

        var _modalManager;
        var _$RecordPolicyActionInformationForm = null;

        //hide and show relative rule block according to the rule tyle
        $("#ExpireRuleTypeId").change(function () {
            var optionValue = $(this).find("option:selected").val();

            if (optionValue == 0) {
                $("#archieveRules").hide();
                //  $("#softHardDeleteRules").show();
                $("#archieveRules").val("");
            } else if (optionValue == 1) {
                $("#archieveRules").hide();
                // $("#softHardDeleteRules").show();
                $("#archieveRules").val("");
            } else {
                $("#archieveRules").show();
                //  $("#softHardDeleteRules").hide();
                //  $("#softHardDeleteRules").val("");
            }
        }).change();

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$RecordPolicyActionInformationForm = _modalManager.getModal().find('form[name=RecordPolicyActionInformationsForm]');
            _$RecordPolicyActionInformationForm.validate();
        };

        //save recordstatus type
        //applied tenant Id:
        this.save = function () {
            if (!_$RecordPolicyActionInformationForm.valid()) {
                return;
            }
            //check the rule has input value or not
            var RecordPolicyAction = _$RecordPolicyActionInformationForm.serializeFormToObject();

            //console.log($('#ExpireRuleTypeId').val());
            if ($('#ExpireRuleTypeId').val() == 0) {
                RecordPolicyAction.recordStatus = 3;  //archived
            }
            else if ($('#ExpireRuleTypeId').val() == 1) {
                RecordPolicyAction.recordStatus = 4;  //harddelete
            }
            else {
                RecordPolicyAction.recordStatus = $('#RecordStatusType').val();
            }

            _modalManager.setBusy(true);
            _recordPolicyActionsService.createOrEdit(
                RecordPolicyAction
            ).done(function () {
                //abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditRecordPolicyActionModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };


    };
})(jQuery);