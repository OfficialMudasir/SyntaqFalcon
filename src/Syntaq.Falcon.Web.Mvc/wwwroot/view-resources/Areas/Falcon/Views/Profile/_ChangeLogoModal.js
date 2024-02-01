﻿(function ($) {
    app.modals.ChangeLogoPictureModal = function () {

        var _modalManager;
        var $jcropApi = null;
        var uploadedLogoFileToken = null;

        var _profileService = abp.services.app.profile;

        this.init = function (modalManager) {
            _modalManager = modalManager;
 
        ///////////////////////////////////////////////////////////////////////////////////////////////////////

            $('#ChangeLogoPictureModalForm input[name=LogoPicture]').change(function () {
                $('#ChangeLogoPictureModalForm').submit();
            });

            $('#ChangeLogoPictureModalForm').ajaxForm({
                beforeSubmit: function (formData, jqForm, options) {
                    var $fileInput = $('#ChangeLogoPictureModalForm input[name=LogoPicture]');
                    var files = $fileInput.get()[0].files;

                    if (!files.length) {
                        return false;
                    }

                    var file = files[0];

                    //File type check
                    var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
                    if ('|jpg|jpeg|png|gif|'.indexOf(type) === -1) {
                        abp.message.warn(app.localize('ProfilePicture_Warn_FileType'));
                        return false;
                    }

                    //File size check
                    if (file.size > 5242880) //5MB
                    {
                        abp.message.warn(app.localize('LogoPicture_Warn_SizeLimit', app.maxProfilPictureBytesUserFriendlyValue));
                        return false;
                    }

                    var mimeType = _.filter(formData, { name: 'LogoPicture' })[0].value.type;

                    formData.push({ name: 'FileType', value: mimeType });
                    formData.push({ name: 'FileName', value: 'LogoPicture' });
                    formData.push({ name: 'FileToken', value: app.guid() });

                    return true;
                },
                success: function (response) {
                    if (response.success) {
                        var $LogoPictureResize = $('#LogoPictureResize');

                        var LogoFilePath = abp.appPath + 'File/DownloadTempFile?fileToken=' + response.result.fileToken + '&fileName=' + response.result.fileName + '&fileType=' + response.result.fileType + '&v=' + new Date().valueOf();
                        uploadedLogoFileToken = response.result.fileToken;

                        if ($jcropApi) {
                            $jcropApi.destroy();
                        }

                        $LogoPictureResize.show();
                        $LogoPictureResize.attr('src', LogoFilePath);
                        $LogoPictureResize.attr('originalWidth', response.result.width);
                        $LogoPictureResize.attr('originalHeight', response.result.height);

                        $LogoPictureResize.Jcrop({
                            setSelect: [0, 0, 100, 100],
                            aspectRatio: 1,
                            boxWidth: 400,
                            boxHeight: 400
                        }, function () {
                            $jcropApi = this;
                        });

                    } else {
                        abp.message.error(response.error.message);
                    }
                }
            });


            $("#saveLogo").click(function () {

                if (!uploadedLogoFileToken) {
                    return;
                }

                var resizeParams = {};
                if ($jcropApi) {
                    resizeParams = $jcropApi.getSelection();
                }

                var containerWidth = $jcropApi.getContainerSize()[0];
                var containerHeight = $jcropApi.getContainerSize()[1];

                var originalWidth = containerWidth;
                var originalHeight = containerHeight;

                if ($('#LogoPictureResize')) {
                    originalWidth = parseInt($('#LogoPictureResize').attr("originalWidth"));
                    originalHeight = parseInt($('#LogoPictureResize').attr("originalHeight"));
                }

                var widthRatio = originalWidth / containerWidth;
                var heightRatio = originalHeight / containerHeight;

                _profileService.updateLogoPicture({
                    fileToken: uploadedLogoFileToken,
                    x: parseInt(resizeParams.x * widthRatio),
                    y: parseInt(resizeParams.y * heightRatio),
                    width: parseInt(resizeParams.w * widthRatio),
                    height: parseInt(resizeParams.h * heightRatio)
                }).done(function () {
                    $jcropApi.destroy();
                    $jcropApi = null;
                    $('.header-Logo-picture').attr('src', app.getUserLogoPicturePath());
                    _modalManager.close();
                });

            });

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
        };

        this.save = function () {

            if (!uploadedLogoFileToken) {
                return;
            }

            var resizeParams = {};
            if ($jcropApi) {
                resizeParams = $jcropApi.getSelection();
            }

            var containerWidth = $jcropApi.getContainerSize()[0];
            var containerHeight = $jcropApi.getContainerSize()[1];

            var originalWidth = containerWidth;
            var originalHeight = containerHeight;

            if ($('#LogoPictureResize')) {
                originalWidth = parseInt($('#LogoPictureResize').attr("originalWidth"));
                originalHeight = parseInt($('#LogoPictureResize').attr("originalHeight"));
            }

            var widthRatio = originalWidth / containerWidth;
            var heightRatio = originalHeight / containerHeight;

            _profileService.updateLogoPicture({
                fileToken: uploadedLogoFileToken,
                x: parseInt(resizeParams.x * widthRatio),
                y: parseInt(resizeParams.y * heightRatio),
                width: parseInt(resizeParams.w * widthRatio),
                height: parseInt(resizeParams.h * heightRatio)
            }).done(function () {
                $jcropApi.destroy();
                $jcropApi = null;
                $('.header-Logo-picture').attr('src', app.getUserLogoPicturePath());
                _modalManager.close();
            });

        };

    };
})(jQuery);