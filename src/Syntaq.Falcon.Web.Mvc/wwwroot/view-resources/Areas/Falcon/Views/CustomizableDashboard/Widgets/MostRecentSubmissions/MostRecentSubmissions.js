(function () {
	$(function () {
		var _tenantDashboardService = abp.services.app.tenantDashboard;

		var _viewSubmissionModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/ViewSubmissionModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_ViewSubmissionModal.js',
			modalClass: 'ViewSubmissionModal'
		});
		$('#SubmissionModalButton').click(function (event, Id) {
			_viewSubmissionModal.open({ id: Id });
		});
		var initDashboardMostRecentSubmissions = function (RecentSubmissions) {

			$("#MostRecentSubTableContainer").empty();
			$('#MostRecentSubTableContainer').append(`
                                <table id="MostRecentSubmissionTableId"
                                      class="table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer dtr-column table-hover dt-responsive">
                                    <thead>
                                        <tr>
                                            <th>Name</th>
                                            <th>Record Name</th>
                                            <th class="all">Submission Status</th>
                                            <th class="all">Total Time</th>
                                            <th class="all">Action</th>
                                        </tr>
                                    </thead>
                                </table>
								`);
			$('#MostRecentSubmissionTableId').DataTable({
				paging: true,
				data: RecentSubmissions,
				ajax: null,
				info: false,
				lengthChange: false,
				iDisplayLength: 5,
				order: [[6, "desc"]],
				language: {
					emptyTable: "No data available in table",
				},
				columnDefs: [
					{
						//className: 'control responsive',
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							return color;
						},
						targets: 0
					},
					{
						//className: 'control responsive',
						orderable: false,
						defaultContent: '',
						render: function (data, type, row) {
							return row.display;
						},
						targets: 1
					},
					{
						targets: 2,
						defaultContent: '',
						render: function (data, type, row) {
							return row.status;
						},
					},
					{
						targets: 3,
						defaultContent: '',
						render: function (data, type, row) {
							return moment(new Date(row.time)).fromNow();
						},
					},
					{
						targets: 4,
						defaultContent: '',
						render: function (data, type, row) {
							//data = `<a class="OnClickLink" href="/Falcon/Projects/ViewProject/${row.projectId}"><div style="text-overflow:ellipsis; overflow: hidden;">${row.projectName}</div>`;

							data = `<a class=\"OnClickLink\" name=\"ViewSubmissionLink\" onclick=\"$('#SubmissionModalButton').trigger('click', '" + ${row.id} + "');\" data-id=\"" + ${row.id} + "\"> <i class=\"fas fa-search\"></i> View Details`;
							data += `</a>`

							return data;
						}
					},
				]

			});
		};

		var getMostRecentSubmissionsData = function () {
			debugger;
			_tenantDashboardService
				.getRecentSubmissions({
					todaysStartDate: moment().format("YYYY-MM-DDT00:00:00Z"),
					todaysEndDate: moment().format("YYYY-MM-DDT23:59:59.999Z")
				})
				.done(function (result) {
					initDashboardMostRecentSubmissions(result);
				});
		};
		getMostRecentSubmissionsData();

	});
})();


