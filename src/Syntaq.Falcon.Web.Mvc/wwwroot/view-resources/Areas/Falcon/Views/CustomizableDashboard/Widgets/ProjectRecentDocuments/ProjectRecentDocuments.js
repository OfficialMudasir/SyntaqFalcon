﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var initDashboardRecentDocuments = function (data) {
		$('#recentDocumentsTableContainer').empty();
		$('#recentDocumentsTableContainer').append(`
							<table id="recentDocumentsTableId" class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column">
                                <thead>
                                    <tr>
                                        <th class="all">Project</th>
                                        <th>Steps</th>
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
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						var data = `<a class="OnClickLink" href="/Falcon/forms/load?OriginalId=${row.formId}&RecordMatterId=${row.recordMatterId}&RecordMatterItemId=${row.recordMatterItemId}&ProjectId=${row.projectId}&version=live">${row.documentName}</a>`;
						if (row.type === 'I') {
							data = `<a class="OnClickLink" href="/Falcon/forms/load?AccessToken=${row.accessToken}&RecordMatterId=${row.recordMatterId}&RecordMatterItemId=${row.recordMatterItemId}&ProjectId=${row.projectId}&version=live">${row.documentName}</a>`;
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
	};

	var getProjectRecentDocumentsData = function (id) {
		_tenantDashboardService.getDashboardProjectData({ tabType: 'C', environmentId: id }).done(function (result) {
			initDashboardRecentDocuments(result.projectRecentDocumentsList)
		});
	};

	getProjectRecentDocumentsData();


	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#PrdEnvContent").append(`<a class='dropdown-item prd-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#PrdEnvContent").append(`<a class='dropdown-item prd-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".prd-environment-selector").click(function () {
		 
					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectRecentDocumentsData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

	$("#refreshProjectRecentDocumentsButton").click(function () {
		getProjectRecentDocumentsData();
	});
});