﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	// <th class="all">Step Action</th>
	var initDashboardContributor = function (data) {
		$('#contributorTableContainer').empty();
		$('#contributorTableContainer').append(`
                                <table id="projectContributorTableId"
                                       class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                    <thead>
                                        <tr>
                                            <th class="all">Organisation</th>
                                            <th>Name</th>
                                            <th>Role</th>
                                            <th>Email Address</th>
                                            <th class="all">Project Name</th>
                                            <th class="all">Project Step Name</th>
                                            <th class="all">Status</th>
                                            <th class="all">Time Shared</th>
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
						//data = `<a class="OnClickLink" href="/Falcon/Projects/ViewProject/${row.projectId}"><div style="text-overflow:ellipsis; overflow: hidden;">${row.projectName}</div>`;
						//data += `</a>`

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
						var data = `<span class="label badge badge-${badgeclass} badge-inline">${row.status}</span>`
						return data;
					},
				},
				//{
				//	targets: 7,
				//	defaultContent: '',
				//	render: function (data, type, row) {
				//		return row.action;
				//	},
				//},
				{
					targets: 7,
					defaultContent: '',
					render: function (data, type, row) {
						return moment(row.createdTime).format('ll');
					},
				}
			]
		});
	};

	var getWaitingContributorData = function () {
		_tenantDashboardService.getDashboardProjectData({ tabType: 'C' })
			.done(function (result) {
				initDashboardContributor(result.projectContributorsList);
			});
	};

	getWaitingContributorData();

	$(".refreshWaitingContributorButton").click(function () {
		getWaitingContributorData();
	});
});