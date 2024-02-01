(function () {
	$(function () {
		var _$submissionsTable = $('#SubmissionsTable');
		var _submissionsService = abp.services.app.submissions;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Submissions.Create'),
			edit: abp.auth.hasPermission('Pages.Submissions.Edit'),
			'delete': abp.auth.hasPermission('Pages.Submissions.Delete')
		};

		//var _createOrEditModal = new app.ModalManager({
		//	viewUrl: abp.appPath + 'Falcon/Submissions/CreateOrEditModal',
		//	scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_CreateOrEditModal.js',
		//	modalClass: 'CreateOrEditSubmissionModal'
		//});

		var _viewModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Submissions/ViewSubmissionModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Submissions/_ViewSubmissionModal.js',
			modalClass: 'ViewSubmissionModal'
		});

		var _selectedDateRangeAuditLog = {
			startDate: moment().startOf('month'),
			endDate: moment().endOf('month'),
			locale: {
				format: 'DD/MM/YYYY'
			}
		};

		$('input.date-range-picker').daterangepicker(
			$.extend(true, app.createDateRangePickerOptions(), _selectedDateRangeAuditLog),
			function (start, end) {
				_selectedDateRangeAuditLog.startDate = start.format('YYYY-MM-DDT00:00:00Z');
				_selectedDateRangeAuditLog.endDate = end.format('YYYY-MM-DDT23:59:59.999Z');

				getSubmissions();
			});

		var dataTable = _$submissionsTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,


			createdRow: function (row, data, dataIndex) {

				$(row).find("td").on("click", function () {
					_viewModal.open({ id: data.submission.id });
				});

				var rowX = dataTable.row($(row).closest('tr'));
				$(row).find(".show_has_documents").on("click", function () {
					if ($('.fa-plus').length) {
						//var tr = $(row).closest('tr');
						//var rowX = dataTable.row(row);
						if (dataTable.row(row).child.isShown()) {
							$(row).find('.fa-plus').attr("hidden", false);
							$(row).find('.fa-minus').attr("hidden", true);
							//$(row).child.hide();
							rowX.child.hide();
							return false;
						} else {
							if (data.submission.hasDocuments) {
								$(row).find('.fa-plus').attr("hidden", true);
								$(row).find('.fa-minus').attr("hidden", false);
								//var rowX = dataTable.row(row);
								rowX.child(format(rowX.data())).show();
								return false;
							} else {
								return false;
							}

						}
					}
				});

				$(row).find("[name='ViewSubmissionLink']").on("click", function () {
					_viewModal.open({ id: data.submission.id });
				});

				//$(row).find("[name='DeleteSubmissionLink']").on("click", function () {
				//	deleteSubmission(data);
				//});
			},
			listAction: {
				ajaxFunction: _submissionsService.getAll,
				inputFilter: function () {
					return {
						filter: $('#SubmissionsTableFilter').val(),
						startDateFilter: _selectedDateRangeAuditLog.startDate,
						endDateFilter: _selectedDateRangeAuditLog.endDate,
						requiresPaymentFilter: $('#RequiresPaymentFilterId').val(),
						paymentStatusFilter: $('#PaymentStatusFilterId').val(),
						chargeIdFilter: $('#ChargeIdFilterId').val(),
						submissionStatusFilter: $('#SubmissionStatusFilterId').val(),
						typeFilter: $('#TypeFilterId').val(),
						formNameFilter: $('#FormNameFilterId').val(),
						recordNameFilter: $('#RecordNameFilterId').val(),
						recordMatterNameFilter: $('#RecordMatterNameFilterId').val(),
						userNameFilter: $('#UserNameFilterId').val(),
						//appJobNameFilter: $('#AppJobNameFilterId').val(),
						appNameFilter: $('#AppNameFilterId').val(),
						excludeOwnAccountFilter: $('#ExcludeOwnAccountToggleId').prop('checked')
					};
				}
			},
			columnDefs: [
				{
					targets: 0,
					data: null,
					orderable: false,
					defaultContent: '',
					rowAction: {
						element: $('<div/>')
							.addClass('text-center')
							.append(
								$('<button/>')
									.addClass('btn btn-secondary btn-icon btn-sm')
									.attr("aria-label", "Search")
									.append($('<i/>').addClass('fa fa-search'))
							)
							.click(function () {

							}),
					},
				},
				{
					targets: 1,
					render: function (data, type, row) {
						if (row.formName) {
							return ' <span>' + row.formName + '</span>';
						} else if (row.appName) {
							return ' ' + row.appName + '</span>';
						} else {
							return '';
						}
					}
				},
				{
					targets: 2,
					data: "userName"
				},
				{
					targets: 3,
					data: "userEmail"
				},
				{
					targets: 4,
					data: "recordName"
				},
				{
					targets: 5,
					data: "submission.date",
					render: function (data, type, row) {
						var dt = new Date(row.submission.date);
						var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
						var tmoptions = { hour: 'numeric', minute: 'numeric' };
						data = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
						return data;
					}
				},

				{
					targets: 6,
					data: "submission.submissionStatus"
				},
				{
					targets: 7,
					data: null,
					render: function (data, type, row) {
						if (row.submission.requiresPayment) {
							switch (row.submission.paymentStatus) {
								case "RequiredBFL":
								case "RequiredBA":
								case "RequiredAA":
								default:
									data = "Required";
									break;
								case "Deffered":
									data = "Deffered";
									break;
								case "Paid":
									data = "Paid";
									break;
							}
						} else {
							data = "Not Required";
						}
						return data;
					}
				},
				//{
				//	targets: 7,
				//	data: "submission.paymentAmount",
				//	render: function (data, type, row) {
				//		var Currency;
				//		if (row.submission.requiresPayment) {
				//			Currency = CurrencySymbol(row.submission.paymentCurrency);
				//			var Amount = row.submission.paymentAmount != null ? row.submission.paymentAmount.toFixed(2) : "0.00"
				//			data = Currency + Amount;
				//		} else {
				//			data = "US$0.00"
				//		}
				//		Currency = null;
				//		return data;
				//	}
				//},
				//{
				//	targets: 8,
				//	data: "submission.voucherAmount",
				//	render: function (data, type, row) {
				//		var Currency;
				//		if (row.submission.requiresPayment) {
				//			Currency = CurrencySymbol(row.submission.paymentCurrency);
				//			var Amount = row.submission.voucherAmount != null ? row.submission.voucherAmount.toFixed(2) : "0.00"
				//			data = Currency + Amount;
				//		} else {
				//			data = "US$0.00"
				//		}
				//		Currency = null;
				//		return data;
				//	}
				//},
				//{
				//	targets: 9,
				//	data: "submission.amountPaid",
				//	render: function (data, type, row) {
				//		var Currency;
				//		if (row.submission.requiresPayment) {
				//			Currency = CurrencySymbol(row.submission.paymentCurrency);
				//			var Amount = row.submission.amountPaid != null ? row.submission.amountPaid.toFixed(2) : "0.00"
				//			data = Currency + Amount;
				//		} else {
				//			data = "US$0.00"
				//		}
				//		Currency = null;
				//		return data;
				//	}
				//},

			]
		});

		function getSubmissions() {
			dataTable.ajax.reload();
		}

		//function deleteSubmission(data) {
		//	abp.message.confirm(
		//		'',
		//		function (isConfirmed) {
		//			if (isConfirmed) {
		//				_submissionsService.delete({
		//					id: data.submission.id
		//				}).done(function () {
		//					getSubmissions(true);
		//					abp.notify.success(app.localize('SuccessfullyDeleted'));
		//				});
		//			}
		//		}
		//	);
		//}

		function format(d) {
			// `d` is the original data object for the row

			var X = '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;"><tbody>';
			$.each(d.recordMatterItems, function (index, value) {
				X += '<tr><td class="ml-3 pt-1 pb-1 pl-1 w-65" height="35px;">';
				if (value.allowPdf == true) {
					X += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + value.id + '&version=1&format=pdf">' +
						'<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/pdf.svg">' +
						'</a>';
				}
				if (value.allowWord == true) {
					X += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + value.id + '&version=1&format=docx">' +
						'<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/doc.svg">' +
						'</a>';
				}
				if (value.allowHTML == true) {
					X += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + value.id + '&version=1&format=html">' +
						'<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/file.svg">' +
						'</a>';
				}
				var dt = new Date(value.creationTime);
				var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
				var tmoptions = { hour: 'numeric', minute: 'numeric' };
				var date = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
				X += '' + value.documentName + '</td><td class="pt-2 pb-2 w-35" style = "position: absolute; right: 5%; height: 35px;" >' + date + '</td ></tr >';
			});
			X += '</table>';
			return X;
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

		//$('#CreateNewSubmissionButton').click(function () {
		//	_createOrEditModal.open();
		//});

		$('#ExportToExcelButton').click(function () {
			_submissionsService
				.getSubmissionsToExcel({
					filter: $('#SubmissionsTableFilter').val(),
					startDateFilter: _selectedDateRangeAuditLog.startDate,
					endDateFilter: _selectedDateRangeAuditLog.endDate,
					requiresPaymentFilter: $('#RequiresPaymentFilterId').val(),
					paymentStatusFilter: $('#PaymentStatusFilterId').val(),
					chargeIdFilter: $('#ChargeIdFilterId').val(),
					submissionStatusFilter: $('#SubmissionStatusFilterId').val(),
					typeFilter: $('#TypeFilterId').val(),
					formNameFilter: $('#FormNameFilterId').val(),
					recordNameFilter: $('#RecordNameFilterId').val(),
					recordMatterNameFilter: $('#RecordMatterNameFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val(),
					appJobNameFilter: $('#AppJobNameFilterId').val(),
					excludeOwnAccountFilter: $('#ExcludeOwnAccountToggleId').prop('checked')
				}).done(function (result) {
					app.downloadTempFile(result);
				});
		});

		abp.event.on('app.createOrEditSubmissionModalSaved', function () {
			getSubmissions();
		});

		$('#GetSubmissionsButton').click(function (e) {
			e.preventDefault();
			getSubmissions();
		});

		$('#clearbtn').click(function () {
			$('#SubmissionsTableFilter').val('');
			getSubmissions();
		});

		$('#RequiresPaymentFilterId').click(function (e) {
			getSubmissions();
		});

		$('#PaymentStatusFilterId').change(function (e) {
			getSubmissions();
		});

		$('#SubmissionStatusFilterId').change(function (e) {
			getSubmissions();
		});

		$('#ChargeIdFilterId').change(function (e) {
			getSubmissions();
		});

		$('#TypeFilterId').change(function (e) {
			getSubmissions();
		});

		$('#FormNameFilterId').change(function (e) {
			getSubmissions();
		});

		$('#RecordNameFilterId').change(function (e) {
			getSubmissions();
		});

		$('#RecordMatterNameFilterId').change(function (e) {
			getSubmissions();
		});

		$('#UserNameFilterId').change(function (e) {
			getSubmissions();
		});

		$('#AppJobNameFilterId').change(function (e) {
			getSubmissions();
		});

		$('#ExcludeOwnAccountToggleId').change(function (e) {
			getSubmissions();
		});

		$("#SubmissionsTableFilter").keyup(function (event) {
			// On enter 
			if (event.keyCode === 13) {
				getSubmissions();
			}
		});
		$("button[name='RefreshButton']").click(function (e) {
			getSubmissions();
		});
	});
})();