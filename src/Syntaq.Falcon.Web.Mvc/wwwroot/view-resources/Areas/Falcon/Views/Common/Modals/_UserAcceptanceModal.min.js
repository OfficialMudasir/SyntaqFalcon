(function ($) {
    app.modals.UserAcceptanceNotifyModal = function () {

        var _userAcceptancesService = abp.services.app.userAcceptances;
        var _userAcceptancesTypeService = abp.services.app.userAcceptanceTypes;


        var _modalManager;
        var _$userAcceptanceInformationFormArray = [];
        var countOfActiveUserAcceptanceTypes = 0;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            _modalManager.getModal().find(".modal-content").css("width", "400px");

            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });
            countOfActiveUserAcceptanceTypes = parseInt($("input[name=countOfActiveUseAcceptanceTypes]").val());
            for (var index = 0; index < countOfActiveUserAcceptanceTypes; index++) {
                var temp = _modalManager.getModal().find('form[name=UserAcceptanceInformationsForm' + index + ']');
                temp.validate();
                _$userAcceptanceInformationFormArray.push(temp);
            }
        };

        var _userAcceptanceDocumentModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptances/UserAcceptanceDocumentModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptances/_UserAcceptanceDocumentModal.js',
            modalClass: 'UserAcceptanceDocumentModal'
        });
		
        $("#userAcceptanceChecked").change(function () {
            if (this.checked) {
                $("#userAcceptanceButton").removeAttr('disabled');
            } else {
                $("#userAcceptanceButton").attr('disabled', "disabled");
            }
        });


        $("#userAcceptanceButton").click(function () {

            if ($("#userAcceptanceChecked").prop("checked") == false) {
                $("#userAcceptanceButton").attr('disabled', "disabled");
                alert(app.localize('ConsentUncheckedWarning'));
                return;
            }

            var formObjectArray = [];
            for (var j = 0; j < countOfActiveUserAcceptanceTypes; j++) {
                formObjectArray.push(_$userAcceptanceInformationFormArray[j].serializeFormToObject());
            }

            _modalManager.setBusy(true);
            for (var j = 0; j < countOfActiveUserAcceptanceTypes; j++) {
                if (j < countOfActiveUserAcceptanceTypes - 1) {
                    _userAcceptancesService.createOrEdit(
                        formObjectArray[j]
                    );
                } else {
                    _userAcceptancesService.createOrEdit(
                        formObjectArray[j]
                    ).done(function () {
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
                }

            }
        });

        $('u[id*="userAcceptanceDocument"]').click(function () {
            _userAcceptancesTypeService.getUserAcceptanceTypeForView({
                id: $("input." + this.id).val()
            }).done(function (data) {
                _userAcceptanceDocumentModal.open({
                    id: data.userAcceptanceType.templateId
                });
            });
        });
    };
})(jQuery);