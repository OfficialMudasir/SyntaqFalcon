(function () {
	$(function () {

		var _$appJobsTable = $('#AppJobsTable');
		var _appJobsService = abp.services.app.appJobs;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.AppJobs.Create'),
			edit: abp.auth.hasPermission('Pages.AppJobs.Edit'),
			'delete': abp.auth.hasPermission('Pages.AppJobs.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/AppJobs/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AppJobs/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditAppJobModal'
		});

		 var _viewAppJobModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/AppJobs/ViewappJobModal',
			modalClass: 'ViewAppJobModal'
		});

		var getDateFilter = function (element) {
			if (element.data("DateTimePicker").date() == null) {
				return null;
			}
			return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
		}

		var dataTable = _$appJobsTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _appJobsService.getAll,
				inputFilter: function () {
					return {
						filter: $('#AppJobsTableFilter').val(),
						nameFilter: $('#NameFilterId').val(),
						appNameFilter: $('#AppNameFilterId').val()
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
						cssClass: 'btn btn-secondary ',
						text:   app.localize('Actions')   ,
						items: [
						{
								text: app.localize('View'),
								action: function (data) {
									_viewAppJobModal.open({ data: data.record });
								}
						},
						{
							text: app.localize('Edit'),
							visible: function () {
								return _permissions.edit;
							},
							action: function (data) {
								_createOrEditModal.open({ id: data.record.appJob.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteAppJob(data.record.appJob);
							}
						}]
					}
				},
					{
						targets: 1,
						data: "appJob.name"   
					},
					{
						targets: 2,
						data: "appName" 
					}
			]
		});


		function getAppJobs() {
			dataTable.ajax.reload();
		}

		function deleteAppJob(appJob) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_appJobsService.delete({
							id: appJob.id
						}).done(function () {
							getAppJobs(true);
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

		$('#CreateNewAppJobButton').click(function () {
			_createOrEditModal.open();
		});

		$('#ExportToExcelButton').click(function () {
			_appJobsService
				.getAppJobsToExcel({
				filter : $('#AppJobsTableFilter').val(),
					nameFilter: $('#NameFilterId').val(),
					appNameFilter: $('#AppNameFilterId').val()
				})
				.done(function (result) {
					app.downloadTempFile(result);
				});
		});

		abp.event.on('app.createOrEditAppJobModalSaved', function () {
			getAppJobs();
		});

		$('#GetAppJobsButton').click(function (e) {
			e.preventDefault();
			getAppJobs();
		});

		$(document).keypress(function(e) {
			if(e.which === 13) {
				getAppJobs();
			}
		});

	});
})();