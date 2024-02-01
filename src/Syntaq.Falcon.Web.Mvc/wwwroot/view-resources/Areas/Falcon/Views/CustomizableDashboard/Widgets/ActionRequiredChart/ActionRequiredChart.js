﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	function initDashboardActionStatus(contributorsList, actionNames) {
		function generateChartData(contributorsList, actionNames) {

			var returnData = { labels: [], series: [] };
			actionNames.forEach((action, i) => {
				returnData.labels.push(action);
				returnData.series.push({ value: 0, className: `my-custom-class-${i}` });
			});
			contributorsList.forEach((contributor) => {
				var index = contributor.actionCode;
				returnData.series[index].value += 1;
			})

			return returnData;
		}

		var data = generateChartData(contributorsList, actionNames);

		var options = {
			donut: true,
			donutWidth: 17,
			donutSolid: true,
			startAngle: 0,
			showLabel: true,
			labelInterpolationFnc: function (value, idx) {
				if ($('#actionStatusLables').children().length >= data.series.length) {
					$('#actionStatusLables').empty();
				}
				var labelS = "<div class='pie-chart-label'><svg width='20' height='20'><circle cx='10' cy='10' r='10' class='my-custom-class-" + (idx) + "' /></svg >";
				labelS += value + " : " + data.series[idx].value + "</div>"
				$('#actionStatusLables').append(labelS);
				return '';
			}
		};
		new Chartist.Pie('#actionStatusChart', data, options);//, responsiveOptions

		$("#actionStatusCount").text(contributorsList.length);
	}

	var getActionStatusChartData = function (id) {
		_tenantDashboardService.getDashboardProjectData({ tabType: 'C', environmentId: id }).done(function (result) {

			initDashboardActionStatus(result.projectContributorsList, result.projectActionNames);
		});
	};

	$(".nav-item").click(() => {
		setTimeout(function () {
			if ($(".ActionRequiredChartContainer").is(':visible')) {
				getActionStatusChartData();
			}
		}, 500);

	});

	$(".refreshWaitingContributorButton").click(function () {
		if ($(".ActionRequiredChartContainer").is(':visible')) {
			getActionStatusChartData();
		}
	});

	getActionStatusChartData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#ArEnvContent").append(`<a class='dropdown-item ar-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#ArEnvContent").append(`<a class='dropdown-item ar-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".ar-environment-selector").click(function () {
 
					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getActionStatusChartData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

});