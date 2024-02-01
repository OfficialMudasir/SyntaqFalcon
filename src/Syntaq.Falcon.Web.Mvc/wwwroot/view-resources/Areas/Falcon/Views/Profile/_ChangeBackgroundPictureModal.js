(function ($) {
	app.modals.ChangeProfileBackgroundPictureModal = function () {

		var _modalManager;
		var $jcropApi = null;
		var uploadedFileToken = null;

		var _profileService = abp.services.app.profile;

		this.init = function (modalManager) {
			_modalManager = modalManager;
			$('#ChangeProfileBackgroundPictureModalForm input[name=ProfileBackgroundPicture]').change(function () {
				$('#ChangeProfileBackgroundPictureModalForm').submit();
			});
			$('#ChangeProfileBackgroundPictureModalForm').ajaxForm({
				beforeSubmit: function (formData, jqForm, options) {
					var $fileInput = $('#ChangeProfileBackgroundPictureModalForm input[name=ProfileBackgroundPicture]');
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
						abp.message.warn(app.localize('ProfilePicture_Warn_SizeLimit', app.maxProfilPictureBytesUserFriendlyValue));
						return false;
					}

					var mimeType = _.filter(formData, { name: 'ProfileBackgroundPicture' })[0].value.type;

					formData.push({ name: 'FileType', value: mimeType });
					formData.push({ name: 'FileName', value: 'ProfileBackgroundPicture' });
					formData.push({ name: 'FileToken', value: app.guid() });

					return true;
				},
				success: function (response) {
					if (response.success) {
						var $profileBackgroundPictureResize = $('#ProfileBackgroundPictureResize');

						var profileFilePath = abp.appPath + 'File/DownloadTempFile?fileToken=' + response.result.fileToken + '&fileName=' + response.result.fileName + '&fileType=' + response.result.fileType + '&v=' + new Date().valueOf();
						uploadedFileToken = response.result.fileToken;

						if ($jcropApi) {
							$jcropApi.destroy();
						}

						$profileBackgroundPictureResize.show();
						$profileBackgroundPictureResize.attr('src', profileFilePath);
						$profileBackgroundPictureResize.attr('originalWidth', response.result.width);
						$profileBackgroundPictureResize.attr('originalHeight', response.result.height);

						$profileBackgroundPictureResize.Jcrop({
							minSize: [305, 100], // min crop size
							maxSize: [610, 200], // max crop size
							aspectRatio: 305 / 100, //keep aspect ratio
							setSelect: [0, 0, 305, 100], 
							boxWidth: 500,
							boxHeight: 500
						}, function () {
							$jcropApi = this;
						});
					} else {
						abp.message.error(response.error.message);
					}
				}
			});
			$('#ProfileBackgroundPictureResize').hide();
		};
		this.save = function () {
			if (!uploadedFileToken) {
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

			if ($('#ProfileBackgroundPictureResize')) {
				originalWidth = parseInt($('#ProfileBackgroundPictureResize').attr("originalWidth"));
				originalHeight = parseInt($('#ProfileBackgroundPictureResize').attr("originalHeight"));
			}

			var widthRatio = originalWidth / containerWidth;
			var heightRatio = originalHeight / containerHeight;

			_profileService.updateProfileBackgroundPicture({
				fileToken: uploadedFileToken,
				x: parseInt(resizeParams.x * widthRatio),
				y: parseInt(resizeParams.y * heightRatio),
				width: parseInt(resizeParams.w * widthRatio),
				height: parseInt(resizeParams.h * heightRatio)
			}).done(function () {
				$jcropApi.destroy();
				$jcropApi = null;
				//$('.header-profile-picture').attr('src', app.getUserProfilePicturePath());
				_modalManager.close();
			});
		};
	};
})(jQuery);