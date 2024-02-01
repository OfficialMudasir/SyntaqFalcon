$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	function initProjectStatusChart(projectslist, statusNames, projectTotal) {
		function generateChartData(projectslist, statusNames) {
			var returnData = { labels: [], series: [] };
			statusNames.forEach((statu, i) => {
				returnData.labels.push(statu);
				returnData.series.push({ value: 0, className: `my-custom-class-${i}` });
			});
			projectslist.forEach((project, p) => {
				var index = project.status;
				returnData.series[index].value += 1;
			})
			return returnData;
		}

		var data = generateChartData(projectslist, statusNames);

		var options = {
			donut: true,
			donutWidth: 17,
			donutSolid: true,
			startAngle: 0,
			showLabel: true,
			labelInterpolationFnc: function (value, idx) {
				if ($('#projectStatusLables').children().length >= data.series.length) {
					$('#projectStatusLables').empty();
				}
				var labelS = "<div class='pie-chart-label'><svg width='20' height='20'><circle cx='10' cy='10' r='10' class='my-custom-class-" + (idx) + "' /></svg >";
				labelS += value + " : " + data.series[idx].value + "</div>"
				$('#projectStatusLables').append(labelS);
				return '';
			}
		};
		var drawChart = new Chartist.Pie('#projectStatusChart', data, options);//, responsiveOptions
		$("#projectStatusCount").text(projectTotal);
		drawChart.on('draw', function (data) {
		});

	}
	var getProjectStatusChartData = function (id) {
		_tenantDashboardService
			.getDashboardProjectData({ tabType: 'P', environmentId: id }).done(function (result) {
				initProjectStatusChart(result.projectsList, result.projectStatuNames, result.projectTotal)
			});
	};

	getProjectStatusChartData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#PscEnvContent").append(`<a class='dropdown-item psc-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#PscEnvContent").append(`<a class='dropdown-item psc-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".psc-environment-selector").click(function () {


					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectStatusChartData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

	$(".nav-item").click(() => {
		setTimeout(function () {
			if ($(".ProjectStatusChartContainer").is(':visible')) {
				getProjectStatusChartData();
			}
		}, 500);

	});
});