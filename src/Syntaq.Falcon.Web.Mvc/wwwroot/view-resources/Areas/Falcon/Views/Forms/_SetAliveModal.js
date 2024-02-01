(function ($) {
	app.modals.SetAliveModal = function (data) {
		var _formsService = abp.services.app.forms;

		var _modalManager;

		this.init = function (modalManager) {
			_modalManager = modalManager;
			var modal = _modalManager.getModal();
		};

		$('#setAlive').click(function () {
			var formVersion = (parseInt($('[name="FormVersion"]').val()) );
			//formVersion = (parseInt(formVersion) + 1);
			var FormId = $('[name="FormId"]').val();
			var FormOriginalId = $('[name="FormOriginalId"]').val();
			var FormVersionName = $('#FormVersionName').val();
			var FormVersionDes = $('#VersionDescription').val();

			_formsService.setCurrent({
				originalId: FormOriginalId,
				version: formVersion,
				VersionDes: FormVersionDes,
				VersionName: FormVersionName
			}).done(function () {
				_modalManager.close(); 
				$('#SetAliveHeaderMessage').text("Form: " + $('[name="Name"]').val() + " V. " + formVersion + ' is successfully set as live.');
				$('#setLiveFormName').text($('[name="Name"]').val());
				$("#setLiveToast").addClass("show");
				abp.event.trigger('app.SetAliveModalSetLive');
			});
		});
		
	};
})(jQuery);