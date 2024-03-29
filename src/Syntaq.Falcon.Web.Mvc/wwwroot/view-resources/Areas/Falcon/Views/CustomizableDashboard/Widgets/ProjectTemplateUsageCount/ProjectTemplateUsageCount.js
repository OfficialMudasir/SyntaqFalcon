﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;
	var localname = "Project Template Usage Count";
	var initProjectTemplateUsageCount = function (data) {
		$('#projectTemplateUsageCountTableContainer').empty();
		$('#projectTemplateUsageCountTableContainer').append(`
							<table id="projectTemplateUsageCountTableId" class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                <thead>
                                    <tr>
                                        <th class="all">${app.localize('ProjectTemplate')}
											<i class="fas fa-question-circle" data-bs-toggle="tooltip" data-placement="right" title="${app.localize('Usage Count By User')}"></i>
										</th>
                                        <th>${app.localize('TotalUsageCount')}</th>
                                    </tr>		
                                </thead>
                            </table>
								`);
		//data.length === 0 ? $('#projectTemplateUsageCountTableContainer').empty() : '';
		$('#projectTemplateUsageCountTableId').DataTable({
			paging: false,
			data: data ? data : null,
			ajax: null,
			info: false,
			lengthChange: false,
			//scrollY: "200px",

			language: {
				emptyTable: app.localize('NoDataAvailableInTable'),
			},
			columnDefs: [
				{
					//className: 'control responsive',
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						data = `<div id="usageCountByUserId_${row.projectTemplateId}" style="text-overflow:ellipsis; overflow: hidden;">${row.name}</div>`;
						return data;
					},
					targets: 0
				},
				{
					targets: 1,
					defaultContent: '',
					render: function (data, type, row) {
						var count = `<b>${row.totalCount}</b>`
						//var data = `<span class="label kt-badge kt-badge--${badgeclass} kt-badge--inline" style="margin-left: 1em;">${row.status}</span>`
						return count;
					},
				}
			]
		});
	};

	var projectTemplateUserCount = function (data) {
		$('#projectTemplateUsageCountTableContainer').empty();
		$('#projectTemplateUsageCountTableContainer').append(`
							<table id="projectTemplateUsageCountTableId" class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                <thead>
                                    <tr>
                                        <th class="all">${app.localize('User')}</th>
                                        <th>${app.localize('UsageCount')}</th>
                                    </tr>		
                                </thead>
                            </table>
								`);
		//data.length === 0 ? $('#projectTemplateUsageCountTableContainer').empty() : '';
		$('#projectTemplateUsageCountTableId').DataTable({
			paging: false,
			data: data ? data : null,
			ajax: null,
			info: false,
			lengthChange: false,
			//scrollY: "200px",

			language: {
				emptyTable: app.localize('NoDataAvailableInTable'),
			},
			columnDefs: [
				{
					//className: 'control responsive',
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						data = `<div class="UserName" style="text-overflow:ellipsis; overflow: hidden;">${row.creatorUserName}</div>`;
						return data;
					},
					targets: 0
				},
				{
					targets: 1,
					defaultContent: '',
					render: function (data, type, row) {
						var count = `<b>${row.countByUser}</b>`
						return count;
					},
				}
			]
		});
	};

	var getProjectTemplateUsageCountData = function (id) {
		_tenantDashboardService.getDashboardProjectData({ tabType: 'C', environmentId: id }).done(function (result) {
			initProjectTemplateUsageCount(result.projectTemplatesCountList)
		});
	};

	getProjectTemplateUsageCountData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#PtuEnvContent").append(`<a class='dropdown-item ptu-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#PtuEnvContent").append(`<a class='dropdown-item ptu-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".ptu-environment-selector").click(function () {

					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectTemplateUsageCountData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();


	$("#refreshProjectTemplateUsageCountButton").click(function () {
		$("#ProjectTemplateUsageCountWidgetTitle").text(localname);
		$("#refreshProjectTemplateUsageCountButton").text(app.localize('Refresh'));
		$('#envlablId').text('All');
		getProjectTemplateUsageCountData();
	});


	$(document).on('click', 'body', function (e) {
		var targetDivId = e.target.id;
		if (targetDivId.includes("usageCountByUserId_")) {
			var targetDivText = $("#" + targetDivId).text();
			var targetTemplateId = targetDivId.substring(19);
			console.log("id: " + targetTemplateId);
			$("#ProjectTemplateUsageCountWidgetTitle").text(targetDivText);
			$("#refreshProjectTemplateUsageCountButton").text(app.localize('Back'));
			_tenantDashboardService.getDashboardProjectData({ tabType: 'C', projectTemplateId: targetTemplateId }).done(function (result) {
				projectTemplateUserCount(result.projectTemplatesCountByUserList)
			});
		}
	});
});