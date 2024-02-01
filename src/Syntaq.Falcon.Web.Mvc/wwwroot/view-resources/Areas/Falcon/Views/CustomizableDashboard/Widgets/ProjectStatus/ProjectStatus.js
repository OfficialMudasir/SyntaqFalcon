$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	function initDashboardProjects(projectTotal, archivedTotal, projectStatuNames, projectsList) {
		var styleWeith = parseInt(100 / (projectStatuNames.length + 2));
		function createTemplate(stylew, statuName, count, percentage, border) {
			var template = `<div class="col-auto .mr-auto ${border}" style="width: ${stylew}%;">
									<div class="kt-widget24">
										<div class="kt-widget24__details">
											<div class="kt-widget24__info style="width: 50%;"">
												<h4 class="kt-widget24__title" id="statusFilter" style='color: #5867dd'>${statuName}</h4>
												<h1 class="kt-widget24__title">${count}</h1>
												</div>
											</div>
								<div class="kt-widget24__action float-right">
									<span class="kt-widget24__number">
									<span class="font-weight-bold">${percentage}%</span></span>
								</div>
								</div>
								</div>`;
			return template;
		}
		var initialT = createTemplate(styleWeith, "All Projects", projectTotal, 100, "");
		var archivedProjects = createTemplate(styleWeith, "Archived", archivedTotal, ((archivedTotal / projectTotal) * 100).toFixed(2), "border-left");

		$("#projectStatusAppendedId").empty();
		$("#projectStatusAppendedId").append(initialT);
		$("#projectStatusAppendedId").append(archivedProjects);

		projectStatuNames.forEach((name, index) => {
			var statucount = projectsList.filter(p => p.status == index).length;
			var percenT = ((statucount / projectTotal) * 100).toFixed(2);
			var getTemplate = createTemplate(styleWeith, name, statucount, percenT, "border-left");
			$("#projectStatusAppendedId").append(getTemplate);
		})
	}

	var getProjectStatusData = function (id) {
		_tenantDashboardService
			.getDashboardProjectData({ tabType: 'P', environmentId: id }).done(function (result) {
				initDashboardProjects(result.projectTotal, result.archivedTotal, result.projectStatuNames, result.projectsList);
			});
	};

	getProjectStatusData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#PsEnvContent").append(`<a class='dropdown-item ps-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#PsEnvContent").append(`<a class='dropdown-item ps-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".ps-environment-selector").click(function () {

					debugger;
					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectStatusData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

	$(document).on('click', '#statusFilter', function (e) {

		var filter = e.target.innerText;
		if (filter.includes("Complete")) {
			window.open('/Falcon/Projects?statusFilter=2');
		} else if (filter.includes("New")) {
			window.open('/Falcon/Projects?statusFilter=0');
		} else if (filter.includes("Progress")) {
			window.open('/Falcon/Projects?statusFilter=1');
		} else {
			window.open('/Falcon/Projects');
		}
	});
});