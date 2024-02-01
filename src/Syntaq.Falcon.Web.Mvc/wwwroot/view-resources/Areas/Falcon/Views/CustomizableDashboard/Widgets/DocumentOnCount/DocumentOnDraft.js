﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var _$Container = $('.DocumentOnDraftContainer');

	function initDocumentOnDraft(count) {
		_$Container.find("#DocumentOnDraftCount").text(count);
	}

	var getDocumentOnDraftData = function (id) {
		_tenantDashboardService
			.getDocumentsStatusCountForWidget({ statusType: 'D', environmentId: id }).done(function (result) {
				initDocumentOnDraft(result);
			});
	};

	getDocumentOnDraftData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#DodEnvContent").append(`<a class='dropdown-item dod-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#DodEnvContent").append(`<a class='dropdown-item dod-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".dod-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getDocumentOnDraftData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

});