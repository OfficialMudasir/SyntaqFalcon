﻿$(function () {
    var _tenantDashboardService = abp.services.app.tenantDashboard;

	var _$Container = $('.DocumentOnNewContainer');

    function initDocumentOnNew(count) {
		_$Container.find("#DocumentOnNewCount").text(count);
    }

	var getDocumentOnNewData = function (id) {
		_tenantDashboardService
			.getDocumentsStatusCountForWidget({ statusType: 'N', environmentId: id }).done(function (result) {
				initDocumentOnNew(result);
			});
	};

	getDocumentOnNewData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#DonEnvContent").append(`<a class='dropdown-item don-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#DonEnvContent").append(`<a class='dropdown-item don-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".don-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getDocumentOnNewData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

});