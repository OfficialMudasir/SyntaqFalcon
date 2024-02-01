﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var _$Container = $('.DocumentOnAllContainer');

	function initDocumentOnAll(count) {
		_$Container.find("#DocumentOnAllCount").text(count);
	}

	var getDocumentOnAllData = function (id) {
		_tenantDashboardService
			.getDocumentsStatusCountForWidget({ statusType: 'A', environmentId: id }).done(function (result) {
				initDocumentOnAll(result);
			});
	};

	getDocumentOnAllData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#DoaEnvContent").append(`<a class='dropdown-item doa-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#DoaEnvContent").append(`<a class='dropdown-item doa-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".doa-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getDocumentOnAllData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

});