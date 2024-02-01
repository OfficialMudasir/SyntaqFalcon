﻿(function ($) {
    app.modals.ImportRulesModal = function () {
        var _modalManager;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            $('#ImportRulesForm input[name=file]').change(function () {
                $('#ImportRulesForm').submit();
            });

            $('#ImportRulesForm').ajaxForm({
                beforeSubmit: function (formData, jqForm, options) {
                    var $fileInput = $('#ImportRulesForm input[name=file]');
                    var files = $fileInput.get()[0].files;

                    if (!files.length) {
                        return false;
                    }

                    var file = files[0];

                    //File type check
                    var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
                    if ('|json|'.indexOf(type) === -1) {
                        abp.message.warn(app.localize('Rules_Upload_Warn_FileType'));
                        return false;
                    }

                    //File size check
                    if (file.size > 2621440) //2.5MB
                    {
                        abp.message.warn(app.localize('Rules_Upload_Warn_SizeLimit', '2.5'));
                        return false;
                    }

                    var mimeType = _.filter(formData, { name: 'file' })[0].value.type;

                    formData.push({ name: 'FileType', value: mimeType });
                    formData.push({ name: 'FileName', value: 'FormRules' });
                    formData.push({ name: 'FileToken', value: app.guid() });

                    return true;
                }, success: function (response) {
                    if (response.success) {
                        abp.notify.success(app.localize('RulesUpdated'));
                        _modalManager.close();
                        location.reload(false);
                    } else {
                        abp.message.error(app.localize('RulesUpdateFailed'));
                    }
                }
            });
        };
    };
})(jQuery);