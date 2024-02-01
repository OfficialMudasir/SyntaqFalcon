(function ($) {
    app.modals.CreateOrEditTagEntitiesModal = function () {

        var _tagEntitiesService = abp.services.app.tagEntities;

        var _modalManager;
        var _$tagEntityForm = null;

        var _modalManager;


        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$tagValuesForEntityForm = _modalManager.getModal().find('form[name=TagValuesForEntityForm]');
            _$tagValuesForEntityForm.validate();
        };


        this.save = function () {

            //if (!_$tagEntityForm.valid()) {
            //    return;
            //}
            if ($('#TagEntity_TagValueId').prop('required') && $('#TagEntity_TagValueId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('TagValue')));
                return;
            }

            var tagEntity = _$tagEntityInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);

            _tagEntitiesService.createOrEdit(
                tagEntity
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditTagEntityModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });

        };

    };
})(jQuery);