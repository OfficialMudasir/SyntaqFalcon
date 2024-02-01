$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var _$Container = $('.DocumentOnPublishContainer');

	function initDocumentOnPublish(count) {
		_$Container.find("#DocumentOnPublishCount").text(count);
	}

	var getDocumentOnPublishData = function (id) {
		_tenantDashboardService
			.getDocumentsStatusCountForWidget({ statusType: 'P', environmentId: id }).done(function (result) {
				initDocumentOnPublish(result);
			});
	};

	getDocumentOnPublishData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#DopEnvContent").append(`<a class='dropdown-item dop-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#DonEnvContent").append(`<a class='dropdown-item dop-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".dop-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getDocumentOnPublishData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();
});