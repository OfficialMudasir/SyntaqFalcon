(function ($) {
	app.modals.ViewFilesModal = function () {
		var _$filesTable = $('#FilesTable');
		var _filesService = abp.services.app.files;

		var dataTable = _$filesTable.DataTable({
			paging: true,
			serverSide: true,
			deferLoading: 0,
			listAction: {
				ajaxFunction: _filesService.getFiles,
				inputFilter: function () {
					return {
						recordId: JSRecordId,
                        recordMatterId: JSRecordMatterId,
                        recordMatterItemGroupId: JSRecordMatterItemGroupId
					};
				}
			},
			columnDefs: [
				{
					targets: 0,
					data: "file.fileName"
				},
				{
					targets: 1,
					data: "file.size"
				},
				{
					targets: 2,
					data: null,
                    render: function (data, type, row) {
                        data = '<a class="OnClickLink" href="' + abp.appPath + 'Falcon/Files/DownloadFile?RecordId=' + row.file.recordId + '&RecordMatterId=' + row.file.recordMatterId + '&RecordMatterItemGroupId=' + JSRecordMatterItemGroupId + '&Filename=' + row.file.fileName + '" name="ExportRuleLink"><i class="fas fa-file-download"></i> Download</a>';
						return data;
					}
				}
			]
		});

		function getFiles() {
			dataTable.ajax.reload();
		}

		$(document).keypress(function (e) {
			if (e.which === 13) {
				getFiles();
			}
		});

		getFiles();
	};
})(jQuery);