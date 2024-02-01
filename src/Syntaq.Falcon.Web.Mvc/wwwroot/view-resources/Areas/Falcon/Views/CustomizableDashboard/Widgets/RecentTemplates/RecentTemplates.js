﻿$(function () {
	var _tenantDashboardService = abp.services.app.tenantDashboard;

	var initDashboardRecentDocuments = function (data) {
		$('#recentTemplateTableContainer').empty();
		$('#recentTemplateTableContainer').append(`
							<table id="recentTemplateTableId" class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                <thead>
                                    <tr>
										<th class="all">Project</th>
										<th>Steps</th>
										<th>Document</th>
										<th>Status</th>
                                        <th>Last Modifed</th>
                                    </tr>
                                </thead>
                            </table>
								`);
		//	data.length === 0 ? $('#recentTemplateTableContainer').empty() : '';
		$('#recentTemplateTableId').DataTable({
			paging: true,
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

						return data;

					},
				},
				{
					targets: 2,
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						data = '';

						if (row.allowedFormats.includes("W")) {
							data = '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + row.recordMatterId + '&version=1&format=docx"><img class="stq-primary-icon me-2" title="Download MS Word" src="/common/images/primaryicons/doc.svg"></a>';
						}

						if (row.allowedFormats.includes("P")) {
							data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + row.recordMatterId + '&version=1&format=pdf"><img class="stq-primary-icon me-2" title="Download PDF" src="/common/images/primaryicons/pdf.svg"></a>';
						}
						if (row.documentName === "" || row.documentName === null) {

							data += 'Document Name not set';
						} else {
							data += row.documentName;
						}

						return data;
					},
				},
				{
					targets: 3,
					orderable: false,
					defaultContent: '',
					render: function (data, type, row) {
						switch (row.status) {
							case 'Draft':
								data = `<span class="label badge badge-warning badge-inline pull-left  mr-2" style="min-width: 10em">Draft</span>`
								break;
							case 'Final':
								data = `<span class="label badge badge-success badge-inline pull-left mr-2" style="min-width: 10em">Final </span>`
								break;
							case 'New':
								data = `<span class="label badge badge-warning badge-inline pull-left mr-2" style="min-width: 10em">New</span>`
								break;
							case 'Publish':
								data = `<span class="label badge badge-info badge-inline pull-left mr-2" style="min-width: 10em">Shared</span>`
								break;
						}
						return data;
					},
				},
				{
					targets: 4,
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

			initDashboardRecentDocuments(result.projectRecentTemplatesList)
		});
	};

	getProjectRecentDocumentsData();

	var getEnvironmentData = function () {

		_tenantDashboardService
			.getProjectEnvironments().done(function (result) {
				$("#RtEnvContent").append(`<a class='dropdown-item rt-environment-selector' href='javascript:;' data-Id='' >All</a>`);
				$.each(result, function (k, v) {
					$("#RtEnvContent").append(`<a class='dropdown-item rt-environment-selector' href='javascript:;' data-Id='${this.id}' > ${this.name}</a>`);
				});

				$(".rt-environment-selector").click(function () {


					var lbl = $(this).closest('.dropdown').find('.envLbl');
					var id = $(this).closest('.dropdown').find('.envId');

					lbl.text(this.text);
					id.text($(this).attr('data-Id'));

					getProjectRecentDocumentsData($(this).attr('data-Id'));
				});

			});
	};
	getEnvironmentData();

	$("#refreshRecentTemplateButton").click(function () {

		getProjectRecentDocumentsData();
	});
});