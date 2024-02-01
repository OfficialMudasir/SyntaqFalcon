(function ($) {
	app.modals.SetAliveModal = function (data) {

		var _templatesService = abp.services.app.templates;

		var _modalManager;

		this.init = function (modalManager) {
			_modalManager = modalManager;
			var modal = _modalManager.getModal();
		};

		$('#setAlive').click(function () {
			var id = $('[name ="OriginalId"]').val();
			var v = $('[name="Version"]').val();
			var des = $('#VersionDescription').val();
			var cv = $('[ name="CurrentVersion"]').val();

			_templatesService.setCurrent({
				originalId: id, version: v, VersionDes: des, CurrentVersion: cv
			}).done(function () {
				_modalManager.close();
				$('#setLiveTemplateName').text($('[name="Name"]').val());
				$('#SetAliveHeaderMessage').text("Document template: " + $('[name="Name"]').val() + " V. " + v + ' is successfully set as live.');
				$("#setLiveToast").addClass("show");
				abp.event.trigger('app.SetAliveModalSetLive');
			});
		});

	};
})(jQuery);