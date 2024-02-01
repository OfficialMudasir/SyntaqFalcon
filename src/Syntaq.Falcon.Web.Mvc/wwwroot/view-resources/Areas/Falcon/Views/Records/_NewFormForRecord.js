(function ($) {
    app.modals.NewFormForRecord = function () {
        var _modalManager;
        var _$newFormForRecordForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$newFormForRecordForm = _modalManager.getModal().find('form[name=NewFormForRecordForm]');
            _$newFormForRecordForm.validate();
        };

        this.save = function () {

            if (!_$newFormForRecordForm.valid()) {
                return;
            }
            var form = _$newFormForRecordForm.serializeFormToObject();

            window.location = '/Falcon/forms/load?OriginalId=' + form.formid + '&RecordId=' + form.rid + '&load=record';
        };
    };
})(jQuery);