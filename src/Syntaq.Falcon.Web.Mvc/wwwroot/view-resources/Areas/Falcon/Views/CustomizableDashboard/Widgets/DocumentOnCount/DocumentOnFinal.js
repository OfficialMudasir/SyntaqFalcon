$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var _$Container = $('.DocumentOnFinalContainer');

	function initDocumentOnFinal(count) {
		_$Container.find("#DocumentOnFinalCount").text(count);
	}

	var getDocumentOnFinalData = function (id) {
		_tenantDashboardService
			.getDocumentsStatusCountForWidget({ statusType: 'F', environmentId: id }).done(function (result) {
				initDocumentOnFinal(result);
			});
	};

	getDocumentOnFinalData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#DofEnvContent").append(`<a class='dropdown-item dof-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#DofEnvContent").append(`<a class='dropdown-item dof-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".dof-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getDocumentOnFinalData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();
});