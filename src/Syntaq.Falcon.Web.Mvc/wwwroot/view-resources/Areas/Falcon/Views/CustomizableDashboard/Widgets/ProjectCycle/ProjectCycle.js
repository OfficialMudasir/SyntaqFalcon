$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var initProjectCycle = function (data) {
		$('#projectsCycleTableContainer').empty();
		$('#projectsCycleTableContainer').append(`
							<table id="projectsCycleTableId" class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                <thead>
                                    <tr>
                                        <th class="all">Project</th>
                                        <th>Status</th>
                                        <th>Cycle</th>
                                    </tr>
                                </thead>
                            </table>
								`);
		data.length === 0 ? $('#projectsCycleTableContainer').empty() : '';
		$('#projectsCycleTableId').DataTable({
			paging: false,
			data: data ? data : null,
			ajax: null,
			info: false,
			lengthChange: false,
			//scrollY: "200px",

			language: {
				emptyTable: "No data available in table",
			},
			columnDefs: [
				{
					//className: 'control responsive',
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						data = `<a class="OnClickLink" href="/Falcon/Projects/ViewProject/${row.projectId}"><div style="text-overflow:ellipsis; overflow: hidden;">${row.projectName}</div>`;
						data += `</a>`
						return data;
					},
					targets: 0
				},
				{
					targets: 1,
					defaultContent: '',
					render: function (data, type, row) {

						var badgeclass = '';

						if (row.status === 'Completed') {
							badgeclass = 'success';
						} else if (row.status === 'InProgress') {
							badgeclass = 'info';
							row.status = 'In Progress';
						} else {
							badgeclass = 'warning';
						}
						var data = `<span class="label badge badge-${badgeclass} badge-inline">${row.status}</span>`
						return data;
					},
				},
				{
					targets: 2,
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						//var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
						//var tmoptions = { hour: 'numeric', minute: 'numeric' };
						//var date = new Date(row.lastModificationTime);
						var create = moment(new Date(row.creationTime));
						var complete = moment(new Date(row.lastModificationTime));
						//var ago = moment(new Date(row.lastModificationTime)).fromNow();
						var ago = create.fromNow(true);
						if (row.status === 'Completed') {
							ago = create.from(complete, true)
						}
						//date = date.toLocaleTimeString('en-US', tmoptions) + ', ' + date.toLocaleDateString('en-GB', dtoptions);
						return `<b>${ago}</b>`;
					},
				}
			]
		});
	};

	var getProjectCycleData = function (id) {
		_tenantDashboardService.getDashboardProjectData({ tabType: 'C', environmentId: id }).done(function (result) {
			initProjectCycle(result.projectRecentDocumentsList)
		});
	};

	getProjectCycleData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#PcEnvContent").append(`<a class='dropdown-item pc-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#PcEnvContent").append(`<a class='dropdown-item pc-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".pc-environment-selector").click(function () {
					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectCycleData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

	$("#refreshProjectsCycleButton").click(function () {
		getProjectCycleData();
	});
});