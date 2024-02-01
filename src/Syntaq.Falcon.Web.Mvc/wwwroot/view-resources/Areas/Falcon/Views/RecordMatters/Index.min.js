(function () {
	$(function () {

		var _$recordMattersTable = $('#RecordMattersTable');
		var _recordMattersService = abp.services.app.recordMatters;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.RecordMatters.Create'),
			edit: abp.auth.hasPermission('Pages.RecordMatters.Edit'),
			'delete': abp.auth.hasPermission('Pages.RecordMatters.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/RecordMatters/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatters/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditRecordMatterModal'
		});


		var dataTable = _$recordMattersTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _recordMattersService.getAll,
				inputFilter: function () {
					return {
					filter: $('#RecordMattersTableFilter').val(),
					minDataFilter: $('#MinDataFilterId').val(),
					maxDataFilter: $('#MaxDataFilterId').val(),
					accessTokenFilter: $('#AccessTokenFilterId').val(),
					recordRecordNameFilter: $('#RecordRecordNameFilterId').val()
					};
				}
			},
			columnDefs: [
				{
					width: 120,
					targets: 0,
					data: null,
					orderable: false,
					autoWidth: false,
					defaultContent: '',
					rowAction: {
						cssClass: 'btn btn-secondary',
						text: app.localize('Actions'),
						items: [
						{
							text: app.localize('Edit'),
							visible: function () {
								return _permissions.edit;
							},
							action: function (data) {
								_createOrEditModal.open({ id: data.record.recordMatter.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteRecordMatter(data.record.recordMatter);
							}
						}]
					}
				},
					{
						targets: 1,
						 data: "recordMatter.data"   
					},
					{
						targets: 2,
						 data: "recordMatter.accessToken"   
					},
					{
						targets: 3,
						 data: "recordRecordName" 
					}
			]
		});


		function getRecordMatters() {
			dataTable.ajax.reload();
		}

		function deleteRecordMatter(recordMatter) {
			abp.message.confirm(
				'',
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordMattersService.delete({
							id: recordMatter.id
						}).done(function () {
							getRecordMatters(true);
							abp.notify.success(app.localize('SuccessfullyDeleted'));
						});
					}
				}
			);
		}

		$('#ShowAdvancedFiltersSpan').click(function () {
			$('#ShowAdvancedFiltersSpan').hide();
			$('#HideAdvancedFiltersSpan').show();
			$('#AdvacedAuditFiltersArea').slideDown();
		});

		$('#HideAdvancedFiltersSpan').click(function () {
			$('#HideAdvancedFiltersSpan').hide();
			$('#ShowAdvancedFiltersSpan').show();
			$('#AdvacedAuditFiltersArea').slideUp();
		});

		$('#CreateNewRecordMatterButton').click(function () {
			_createOrEditModal.open();
		});
		
		abp.event.on('app.createOrEditRecordMatterModalSaved', function () {
			getRecordMatters();
		});

		$('#GetRecordMattersButton').click(function (e) {
			e.preventDefault();
			getRecordMatters();
		});

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getRecordMatters();
		  }
		});

	});
})();