﻿(function ($) {
    app.modals.CreateOrEditTagEntityTypeModal = function () {

        var _tagEntityTypesService = abp.services.app.tagEntityTypes;

        var _modalManager;
        var _$tagEntityTypeInformationForm = null;

        var _TagEntityTypetagLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntityTypes/TagLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntityTypes/_TagEntityTypeTagLookupTableModal.js',
            modalClass: 'TagLookupTableModal'
        });



        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$tagEntityTypeInformationForm = _modalManager.getModal().find('form[name=TagEntityTypeInformationsForm]');
            _$tagEntityTypeInformationForm.validate();
        };

        $('#OpenTagLookupTableButton').click(function () {

            var tagEntityType = _$tagEntityTypeInformationForm.serializeFormToObject();

            _TagEntityTypetagLookupTableModal.open({ id: tagEntityType.tagId, displayName: tagEntityType.tagName }, function (data) {
                _$tagEntityTypeInformationForm.find('input[name=tagName]').val(data.displayName);
                _$tagEntityTypeInformationForm.find('input[name=tagId]').val(data.id);
            });
        });

        $('#ClearTagNameButton').click(function () {
            _$tagEntityTypeInformationForm.find('input[name=tagName]').val('');
            _$tagEntityTypeInformationForm.find('input[name=tagId]').val('');
        });



        this.save = function () {
            if (!_$tagEntityTypeInformationForm.valid()) {
                return;
            }
            if ($('#TagEntityType_TagId').prop('required') && $('#TagEntityType_TagId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Tag')));
                return;
            }



            var tagEntityType = _$tagEntityTypeInformationForm.serializeFormToObject();




            _modalManager.setBusy(true);
            _tagEntityTypesService.createOrEdit(
                tagEntityType
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditTagEntityTypeModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };


    };
})(jQuery);