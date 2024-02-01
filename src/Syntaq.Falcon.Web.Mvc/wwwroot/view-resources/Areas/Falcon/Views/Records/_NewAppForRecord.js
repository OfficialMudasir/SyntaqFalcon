(function ($) {
    app.modals.NewAppForRecord = function () {
        var _modalManager;
        var _$newFormForRecordForm = null;
        var _appsService = abp.services.app.apps;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$newFormForRecordForm = _modalManager.getModal().find('form[name=NewFormForRecordForm]');
            _$newFormForRecordForm.validate();
        };
        console.log('helo');

        this.save = function () {
            if (!_$newFormForRecordForm.valid()) {
                return;
            }
            var form = _$newFormForRecordForm.serializeFormToObject();

            _appsService.run({
                "Id": form.appid,
                "DataURL": "",
                "data": JSONObj  //  example { "Chairman_Casting_cho": "is not", "Chairman_Casting_cho_MText": "is not" }
            }).done(function () {
                //getApps();
                abp.notify.success('App Run. Check Submission response for the status of this run.');
                _modalManager.close();
            });
        };
    };
})(jQuery);