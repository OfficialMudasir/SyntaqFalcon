(function () {
	$(function () {
		var _appUserNotificationHelper = new app.UserNotificationHelper();
		var _tenantDashboardService = abp.services.app.tenantDashboard;
		//var _$container = $("#kt_content");
		var _$dateRangePicker = $(".dashboard-report-range");
		//var _$refreshButton = $("button[name='RefreshButton']");
		//var salesSummaryDatePeriod = {
		//	daily: 1,
		//	weekly: 2,
		//	monthly: 3
		//};

		var _selectedDateRange = {
			//startDate: moment().add(-7, 'days').startOf('day'),
			//endDate: moment().endOf("day")
			startDate: moment().startOf('month'),
			endDate: moment().endOf('day')
		};

		var showSelectedDate = function () {
			if (_$dateRangePicker.attr("data-display-range") !== "0") {
				_$dateRangePicker.find("span:eq(0)").html(_selectedDateRange.startDate.format("LL") +
					" - " +
					_selectedDateRange.endDate.format("LL"));
			}
		};

		var _viewSubmissionModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/ViewSubmissionModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_ViewSubmissionModal.js',
			modalClass: 'ViewSubmissionModal'
		});

		function initDashboardSubCountStats(SubmissionLimit, CurrentSubmissions, SubmissionUsagePercent) {
			$(".submissionLimit").text(SubmissionLimit);
			$(".currentSubmissions").text(CurrentSubmissions);
			$(".submissionUsagePercent").text(SubmissionUsagePercent);
			$('.progress-bar').css('width', '' + SubmissionUsagePercent +'%');
		};

		function initDashboardTodaySubCountStats(YesterdaysSubmissions, TodaysSubmissions) {
			$(".todaysSubmissions").text(TodaysSubmissions);
			var ChangeSymbol = YesterdaysSubmissions > TodaysSubmissions ? '-' : '+';
			$(".submissionChangePercent").text(ChangeSymbol + Math.round((Math.abs(YesterdaysSubmissions - TodaysSubmissions) / YesterdaysSubmissions) * 100));
		};

		function initDashboardNewUserCountStats(NewUsers) {
			$("#newUsers").text(NewUsers);
		}

		function initDashboardRecentSubmissions(RecentSubmissions) {
			$(".kt-list-timeline__items").empty();
			var colorArray = ['brand', 'warning', 'success', 'danger', 'info'];
			for (i = 0; i < RecentSubmissions.length; i++) {
				var color = colorArray[Math.floor(Math.random() * colorArray.length)];
				var statusColor;
				switch (RecentSubmissions[i].status) {
					case "Started":
						statusColor = "info";
						break;
					case "Submitted":
						statusColor = "success";
						break;
					case "Awaiting Payment":
						statusColor = "warning";
						break;
					case "Assembling":
						statusColor = "info";
						break;
					case "Error":
						statusColor = "danger";
						break;
					case "Complete":
						statusColor = "success";
						break;
					default:
						statusColor = "warning";
						RecentSubmissions[i].status = "Unknown";
				};
				$(".kt-list-timeline__items").append("<div class=\"kt-list-timeline__item\">"
					+ "<span class=\"kt-list-timeline__badge kt-list-timeline__badge--" + color + "\"></span>"
					+ "<span class=\"kt-list-timeline__text\">" + RecentSubmissions[i].display + " "
					+ "<span class=\"kt-badge kt-badge--" + statusColor + " kt-badge--inline kt-badge--pill\">"
					+ RecentSubmissions[i].status
					+ "</span></span>"
					+ "<span class=\"kt-list-timeline__time\" style=\"width: 225px;\">" + moment(new Date(RecentSubmissions[i].time)).fromNow()/*.timeAgo*/
					+ "<a class=\"OnClickLink\" name=\"ViewSubmissionLink\" onclick=\"$('#SubmissionModalButton').trigger('click', '" + RecentSubmissions[i].id +"');\" data-id=\"" + RecentSubmissions[i].id +"\"> <i class=\"fas fa-search\"></i> View Details</a ></span>"
				+"</div>");
			}
		}

		var getDashboardData = function () {
			_tenantDashboardService.getDashboardData({
				startDate: _selectedDateRange.startDate.format("YYYY-MM-DDT00:00:00Z"),
				endDate: _selectedDateRange.endDate.format("YYYY-MM-DDT23:59:59.999Z"),
				yesterdaysStartDate: moment().subtract(1, "days").format("YYYY-MM-DDT00:00:00Z"),
				yesterdaysEndDate: moment().subtract(1, "days").format("YYYY-MM-DDT23:59:59.999Z"),
				todaysStartDate: moment().format("YYYY-MM-DDT00:00:00Z"),
				todaysEndDate: moment().format("YYYY-MM-DDT23:59:59.999Z")
					//.endOf('day')
			}).done(function (result) {
				initDashboardSubCountStats(result.submissionLimit, result.currentSubmissions, result.submissionUsagePercent);
				initDashboardTodaySubCountStats(result.yesterdaysSubmissions, result.todaysSubmissions);
				initDashboardNewUserCountStats(result.newUsers);
				initDashboardRecentSubmissions(result.recentSubmissions);
				//profitShare(result.profitShares);
				//$(".counterup").counterUp();
			});
		};

		var getDashboardProjectData = function () {
			_tenantDashboardService.getDashboardProjectData({tabType:'P'}).done(function (result) {
				initDashboardProjects(result.projectTotal, result.projectStatuNames, result.projectsList);
				initDashboardDocumentStatus(result.projectsList, result.projectStatuNames);
			});
		}

		var getDashboardContributorData = function () {
			_tenantDashboardService.getDashboardProjectData({ tabType: 'C' }).done(function (result) {
				initDashboardContributor(result.projectContributorsList);
				initDashboardRecentDocuments(result.projectRecentDocumentsList)
				initDashboardActionStatus(result.projectContributorsList, result.projectActionNames);
			});
		}

		function initDashboardProjects(projectTotal, projectStatuNames, projectsList) {
			var styleWeith = parseInt(100 / (projectStatuNames.length + 1));
			function createTemplate(stylew, statuName, count, percentage, border) {
				var template = `<div class="col-auto .mr-auto ${border}" style="width: ${stylew}%;">
									<div class="kt-widget24">
										<div class="kt-widget24__details">
											<div class="kt-widget24__info style="width: 50%;"">
												<h4 class="kt-widget24__title">${statuName}</h4>
												<h1 style="float: right;">${count}</h1>
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
			$("#projectStatusAppendedId").empty();
			$("#projectStatusAppendedId").append(initialT);
			projectStatuNames.forEach((name, index) => {
				var statucount = projectsList.filter(p => p.status == index).length;
				var percenT = ((statucount / projectTotal) * 100).toFixed(2);
				var getTemplate = createTemplate(styleWeith, name, statucount, percenT, "border-left");
				$("#projectStatusAppendedId").append(getTemplate);
			})
		}

		function initDashboardContributor(data) {
			$('#contributorTableContainer').empty();
			$('#contributorTableContainer').append(`
                                <table id="projectContributorTableId"
                                       class="display table table-striped table-bordered table-hover dt-responsive">
                                    <thead>
                                        <tr>
                                            <th class="all">Organization</th>
                                            <th>Name</th>
                                            <th>Role</th>
                                            <th>Email Address</th>
                                            <th class="all">Project Name</th>
                                            <th class="all">Project Step Name</th>
                                            <th class="all">Step Status</th>
                                            <th class="all">Step Action</th>
                                            <th class="all">Share Time</th>
                                        </tr>
                                    </thead>
                                </table>
								`);
			//data.length === 0 ? $('#projectDashbordContributorId').empty() : '';
			$('#projectContributorTableId').DataTable({
				paging: true,
				data: data ? data : null,
				ajax: null,
				info: false,
				lengthChange: false,
				iDisplayLength: 5,
				language: {
					emptyTable: "No data available in table",
				},
				columnDefs: [
					{
						//className: 'control responsive',
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							return row.organization;
						},
						targets: 0
					},
					{
						targets: 1,
						defaultContent: '',
						render: function (data, type, row) {
							return row.name;
						},
					},
					{
						targets: 2,
						defaultContent: '',
						render: function (data, type, row) {
							return row.role;
						},
					},
					{
						targets: 3,
						defaultContent: '',
						render: function (data, type, row) {
							return row.email;
						},
					},
					{
						targets: 4,
						defaultContent: '',
						render: function (data, type, row) {
							return row.projectName;
						}
					},
					{
						targets: 5,
						defaultContent: '',
						render: function (data, type, row) {
							var data = `<a class="OnClickLink" href="/Falcon/forms/load?AccessToken=${row.accessToken}&RecordMatterId=${row.recordMatterId}">${row.projectStepName}</a>`;
							return data;
						},
					},
					{
						targets: 6,
						defaultContent: '',
						render: function (data, type, row) {

							var badgeclass = 'warning';

							if (row.statusCode === 1) {
								badgeclass = 'info';
							}

							if (row.statusCode === 2) {
								badgeclass = 'success';
							}
							var data = `<span class="label kt-badge kt-badge--${badgeclass} kt-badge--inline" style="margin-left: 1em;">${row.status}</span>`
							return data;
						},
					},
					{
						targets: 7,
						defaultContent: '',
						render: function (data, type, row) {
							return row.action;
						},
					},
					{
						targets: 8,
						defaultContent: '',
						render: function (data, type, row) {
							return moment(row.createdTime).format('ll');
						},
					}
				]
			});
		}

		function initDashboardDocumentStatus(projectslist, statusNames) {
			function generateChartData(projectslist, statusNames) {
				var returnData = { labels: [], series: [] };
				statusNames.forEach((statu,i) => {
					returnData.labels.push(statu);
					returnData.series.push({ value: 0, className: `my-custom-class-${i}` });
				});
				projectslist.forEach((project,p) => {
					var index = project.status;
					returnData.series[index].value += 1;
				})
				return returnData;
			}

			var data = generateChartData(projectslist, statusNames);

			var options = {
				donut: true,
				donutWidth: 60,
				height:200,
				donutSolid: true,
				startAngle: 0,
				showLabel: true,
				labelInterpolationFnc: function (value, idx) {
					if ($('#projectStatusLables').children().length >= data.series.length) {
						$('#projectStatusLables').empty();
					}
					var labelS = "<div class='pie-chart-label'><svg width='30' height='30'><circle cx='15' cy='15' r='15' class='my-custom-class-" + (idx) + "' /></svg ><b>";
					labelS += value + " : " + data.series[idx].value + "</b></div>"
					$('#projectStatusLables').append(labelS);
					return '';
				}
			};

			var drawChart = new Chartist.Pie('#projectStatusChart', data, options);//, responsiveOptions

			drawChart.on('draw', function (context) {
			});
		}

		function initDashboardActionStatus(contributorsList, actionNames) {

			//$("#actionStatusChartContainer").empty();
			//if (contributorsList.length == 0) {
			//	$("#actionStatusChartContainer").append(``);
			//	return;
			//} else {
			//	$("#actionStatusChartContainer").append(`<div id="actionStatusChart"></div>
			//											<div id="actionStatusLables"></div>`);
			//}
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
				donutWidth: 60,
				height: 200,
				donutSolid: true,
				startAngle: 0,
				showLabel: true,
				labelInterpolationFnc: function (value, idx) {
					if ($('#actionStatusLables').children().length >= data.series.length) {
						$('#actionStatusLables').empty();
					}
					var labelS = "<div class='pie-chart-label'><svg width='30' height='30'><circle cx='15' cy='15' r='15' class='my-custom-class-" + (idx) + "' /></svg ><b>";
					labelS += value + " : " + data.series[idx].value + "</b></div>"
					$('#actionStatusLables').append(labelS);
					return '';
				}
			};
			new Chartist.Pie('#actionStatusChart', data, options);//, responsiveOptions
		}

		function initDashboardRecentDocuments(data) {
			$('#recentDocumentsTableContainer').empty();
			$('#recentDocumentsTableContainer').append(`
							<table id="recentDocumentsTableId" class="display table table-striped table-bordered table-hover dt-responsive">
                                <thead>
                                    <tr>
                                        <th class="all">Project</th>
                                        <th>Document</th>
                                        <th>Last Modifed</th>
                                    </tr>
                                </thead>
                            </table>
								`);
			data.length === 0 ? $('#recentDocumentsTableContainer').empty() : '';
			$('#recentDocumentsTableId').DataTable({
				paging: false,
				data: data ? data : null,
				ajax: null,
				info: false,
				lengthChange: false,
				language: {
					emptyTable: "No data available in table",
				},
				columnDefs: [
					{
						//className: 'control responsive',
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							return row.projectName;
						},
						targets: 0
					},
					{
						targets: 1,
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							var data = `<a class="OnClickLink" href="/Falcon/forms/load?OriginalId=${row.formId}&RecordMatterId=${row.recordMatterId}&RecordMatterItemId=${row.recordMatterItemId ?? ''}">${row.documentName}</a>`;
							if (row.type === 'I') {
								data = `<a class="OnClickLink" href="/Falcon/forms/load?AccessToken=${row.accessToken}&RecordMatterId=${row.recordMatterId}&RecordMatterItemId=${row.recordMatterItemId ?? ''}">${row.documentName}</a>`;
							}
							return data;
						},
					},
					{
						targets: 2,
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
							var tmoptions = { hour: 'numeric', minute: 'numeric' };
							var date = new Date(row.lastModificationTime);
							var ago = moment(new Date(row.lastModificationTime)).fromNow();
							date = date.toLocaleTimeString('en-US', tmoptions) + ', ' + date.toLocaleDateString('en-GB', dtoptions);
							return `<b>${ago}</b>`;
						},
					}
				]
			});
		}
		//initRegionalStats();
		//initMemberActivity();
		//getDashboardData();

		var refreshAllData = function () {
			showSelectedDate();
			getDashboardData();
		};

		_$dateRangePicker.daterangepicker(
			$.extend(true, app.createDateRangePickerOptions(), _selectedDateRange), function (start, end, label) {
				_selectedDateRange.startDate = start;
				_selectedDateRange.endDate = end;
				refreshAllData();
			});

		$("button[name='RefreshButton']").click(function () {
			refreshAllData();
		});

		$('#SubmissionModalButton').click(function (event, Id) {
			_viewSubmissionModal.open({ id: Id });
		});

		$('#project-dashboard-tab').click(function () {
			setTimeout(function () { getDashboardProjectData(); }, 500);
		});

		$('#contributor-dashboard-tab').click(function () {
			setTimeout(function () { getDashboardContributorData(); }, 500);
		});

		refreshAllData();
	});
})();