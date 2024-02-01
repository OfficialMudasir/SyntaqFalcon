

(function () {
	$(function () {

		abp.ui.setBusy($("body"));


		//var _$formsTable = $('#FormsTable');
		var _formsService = abp.services.app.forms;
		var _aclService = abp.services.app.aCLs;
		var _foldersService = abp.services.app.folders;
		var _userService = abp.services.app.user;
		var _organizationUnitService = abp.services.app.organizationUnit;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});
		var _permissions = {
			admin: abp.auth.hasPermission('Pages.Administration'),
			create: abp.auth.hasPermission('Pages.Forms.Create'),
			edit: abp.auth.hasPermission('Pages.Forms.Edit'),
			'delete': abp.auth.hasPermission('Pages.Forms.Delete')
		};
		var _folderPermissions = {
			edit: abp.auth.hasPermission('Pages.Folders.Edit'),
			'delete': abp.auth.hasPermission('Pages.Folders.Delete')
		};

		var _createOrEditFormModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Forms/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditFormModal'
		});

		var _createOrEditFolderModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Folders/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Folders/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditFolderModal'
		});

		var _manageACLModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
			modalClass: 'ManageACLModal'
		});

		var _manageTenantACLModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageTenantACLModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageTenantACLModal.js',
			modalClass: 'ManageACLModal'
		});

		var _createEmbedModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Embedding/CreateModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Embedding/_CreateModal.js',
			modalClass: 'CreateEmbedModal'
		});

		var _manageNotificationModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Notifications/ManageRecipientsModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Notifications/_ManageRecipientsModal.js',
			modalClass: 'ManageNotificationRecipients'
		});

		var formsFolders = {
			$table: null,
			$sharedTable: null,
			$showAdvancedFiltersSpan: null,
			$hideAdvancedFiltersSpan: null,
			$advacedAuditFiltersArea: null,
			dataTable: null,
			sharedDataTable: null,

			setViewObjects: function () {
				this.$table = $('#FormsTable');
				this.$sharedTable = $('#SharedFormsTable');
				this.$showAdvancedFiltersSpan = $('#ShowAdvancedFiltersSpan');
				this.$hideAdvancedFiltersSpan = $('#HideAdvancedFiltersSpan');
				this.$advacedAuditFiltersArea = $('#AdvacedAuditFiltersArea');
			},

			showTable: function () {
				formsFolders.$table.show();
			},

			hideTable: function () {
				formsFolders.$table.hide();
			},

			setEvents: function () {

				$('.dataTable').on('click', 'tbody tr td:nth-child(1)', function (event) {

					if (event.target.nodeName === "TD" || event.target.nodeName === "SPAN" || event.target.nodeName === "IMG") {
						var data = formsFolders.dataTable.row(this).data();
						if (data !== undefined && data.type === 'Folder') {
							$('#FormNavigationButton').trigger('click', data.id.toString());
						} else {


							var openFormUrl = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/load?OriginalId=' + data.originalId + '&version=live';
							window.open(openFormUrl);

						}
					}
				});

				$('#RefreshFormsButton').click(function () {
					var Id = $("span[name*='Current']").next("input").val();
					loadPartialView(Id);
				});
			},

			init: function (Id) {

				this.dataTable = formsFolders.$table.DataTable({
					paging: true,
					pageLength: 25,
					serverSide: true,
					deferLoading: 0,
					"autoWidth": false,
					"columnDefs": [
						{ "width": "5em", "targets": 0 },
						{ "width": "10em", "targets": 1 },
						{ "width": "70em", "targets": 2 },
						{ "width": "5em", "targets": 3 },
						{ "width": "10em", "targets": 4 }
					],
					createdRow: function (row, data, dataIndex) {
						$(row).each(function (i) {
							$(this).find('td:nth-child(1)').addClass('draggable');
							$(this).find('td:nth-child(1)').attr('data-id', data.id);
							if (data.type === 'Folder') {
								$(this).addClass('droppable');
								$(this).find('td:nth-child(1)').attr('data-droptype', 'Folder');
							} else {
								$(this).find('td:nth-child(1)').attr('data-droptype', 'Form');
							}
						});

						$(row).find("[name='ShareFolderLink']").on("click", function () {
							_manageACLModal.open({ entityid: data.id, entityname: data.name, entitytype: 'Folder' });
						});

						$(row).find("[name='EditFolderLink']").on("click", function () {
							_createOrEditFolderModal.open({ id: data.id, parentid: "00000000-0000-0000-0000-000000000000", orgid: null });
						});

						$(row).find("[name='DeleteFolderLink']").on("click", function () {
							deleteFolder(data);
						});

						$(row).find("[name='ShareFormLink']").on("click", function () {
							_manageACLModal.open({ entityid: data.originalId, entityname: data.name, entitytype: 'Form' });
						});
						$(row).find("[name='EmbedFormLink']").on("click", function () {
							_createEmbedModal.open({ originalId: data.originalId, formId: data.id, type: "Form" });
						});

						$(row).find("[name='EditFormLink']").on("click", function () {
							//_createOrEditFormModal.open({ id: data.id, originalId: data.originalId, folderId: "00000000-0000-0000-0000-000000000000" });
							_createOrEditFormModal.open({ originalId: data.originalId, version: "live", folderId: "00000000-0000-0000-0000-000000000000" });
						});

						$(row).find("[name='DeleteFormLink']").on("click", function () {
							if (data.type === 'Folder') {
								deleteFolder(data);
							} else {
								deleteForm(data);
							}
						});
					},
					drawCallback: function (settings) {
						$(".droppable").droppable({
							tolerance: "pointer",
							hoverClass: "ui-state-hover",
							drop: function (event, ui) {
								var draggableid = ui.draggable.attr('data-id');
								var type = ui.draggable.attr('data-droptype');
								var droppableid = $(this).find('td:nth-child(1)').attr('data-id');
								droppableid = droppableid === undefined ? "00000000-0000-0000-0000-000000000000" : droppableid;
								if (draggableid !== droppableid) {
									_formsService.move({
										draggableId: draggableid,
										draggableType: type,
										id: droppableid,
										folderType: 'F'
									}).done(function (data) {
										if (data === true) {
											getForms();
											abp.notify.success(app.localize('SuccessfullyMoved' + type + ''));
										} else {
											getForms();
											abp.notify.warn(app.localize('MoveFailed'));
										}
									});
								}
								event.preventDefault();
								event.stopPropagation();
							}
						});

						$(".draggable").draggable({
							helper: "clone",

							revert: function (is_valid_drop) {
								event.preventDefault();
								event.stopPropagation();
							}
						});

						$('.sorting_1').removeClass('sorting_1');
					},
					listAction: {
						ajaxFunction: _formsService.getAll,
						inputFilter: function () {
							return {
								filter: $('#FormsTableFilter').val(),
								id: Id.toString(),
								descriptionFilter: $('#DescriptionFilterId').val(),
								docPDFFilter: $('#DocPDFFilterId').val(),
								docUserCanSaveFilter: $('#DocUserCanSaveFilterId').val(),
								docWordFilter: $('#DocWordFilterId').val(),
								docWordPaidFilter: $('#DocWordPaidFilterId').val(),
								nameFilter: $('#NameFilterId').val(),
								minPaymentAmountFilter: $('#MinPaymentAmountFilterId').val(),
								maxPaymentAmountFilter: $('#MaxPaymentAmountFilterId').val(),
								paymentCurrFilter: $('#PaymentCurrFilterId').val(),
								paymentEnabledFilter: $('#PaymentEnabledFilterId').val(),
								type: 'F',
								formsFolderId: $('#SelectedFolderId').val()
							};
						}
					},
					columnDefs: [
						{
							responsivePriority: 1,
							targets: 3,
							data: null,
							orderable: false,
							defaultContent: '',
							rowAction: {
								text: app.localize('Actions'),
								items: [
									//folder actions
									{
										text: 'Share',
										visible: function (data) {
											if (data.record.type !== 'Folder') return false;

											return (data.record.userACLPermission === "O") ? true : false;
										},
										action: function (data) {
											_manageACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Folder' });
										}
									},
									{
										text: app.localize('Edit'),
										visible: function (data) {
											if (data.record.type !== 'Folder') return false;

											return _folderPermissions.edit && (data.record.userACLPermission === "E" || data.record.userACLPermission === "O") ? true : false;
										},
										action: function (data) {
											_createOrEditFolderModal.open({ id: data.record.id, parentid: "00000000-0000-0000-0000-000000000000", orgid: null });
										}
									},
									{
										text: app.localize('Delete'),
										visible: function (data) {
											if (data.record.type !== 'Folder') return false;

											return _folderPermissions.delete && (data.record.userACLPermission === "E" || data.record.userACLPermission === "O") ? true : false;
										},
										action: function (data) {
											deleteFolder(data.record);
										}
									},
									{
										text: 'View',
										visible: function (data) {
											if (data.record.type !== 'Folder') return false;
											return data.record.userACLPermission === "V" ? true : false;
										},
										action: function (data) {
											loadPartialView(data.record.id);
										}
									},
									//Form actions
									{
										text: app.localize('Open Form'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											var openFormUrl = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/load?OriginalId=' + data.record.originalId + '&version=live';
											window.open(openFormUrl);
										}
									},
									{
										text: app.localize('Build Form'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											var openFormUrl = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/build?OriginalId=' + data.record.originalId + '&version=live';
											window.open(openFormUrl);
										}
									},
									{
										text: app.localize('Edit'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_createOrEditFormModal.open({ originalId: data.record.originalId, formId: data.record.id, type: "Form" });
										}
									},
									{
										text: app.localize('Share to User or Team'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_manageACLModal.open({ entityid: data.record.originalId, entityname: data.record.name, entitytype: 'Form' });
										}
									},
									{
										text: app.localize('ShareToTenant'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_manageTenantACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Form' });
										}
									},
									{
										text: app.localize('Embed'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_createEmbedModal.open({ originalId: data.record.originalId, formId: data.record.id, type: "Form" });
										}
									},
									{
										text: app.localize('Delete'),
										visible: function (data) {
											if (data.record.type !== 'Form') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											deleteForm(data.record);
										}
									}
								]
							}
						},
						{
							targets: 0,
							data: "name",
							render: function (data, type, row) {
								if (row.type === 'Folder') {
									data = ' ';
									data += '<span><img class="stq-primary-icon me-2" title="folder" src="/common/images/primaryicons/folder.png"></i> ' + row.name + '</span>';
								} else {
									var sharedSpan = row.shared ? '<span class="label kt-badge kt-badge--' + 'info' + ' kt-badge--inline">' + app.localize('Share') + '</span>' : '<span class="label kt-badge kt-badge--' + 'info' + ' kt-badge--inline" style ="display: none">' + app.localize('Share') + '</span>';
									data = '<img class="stq-primary-icon-lg me-2" title="form" src="/common/images/primaryicons/form.png"></i>';
									data += row.name + ' ' + sharedSpan;
									data += `<div style="overflow-y: auto; max-height: 5em;"><div style="white-space: pre-wrap;">${row.description}</div></div>`
								}
								return data;
							}
						},
						{
							targets: 1,
							data: null,
							orderable: false,
							render: function (data, type, row) {
								var currentVersion = '';
								if (row.type === 'Folder') {
									currentVersion += '';
								} else {
									currentVersion += "V. " + row.currentVersion;
								}
								return currentVersion;
							}
						},
						{
							targets: 2,
							data: "lastModified",
							render: function (data, type, row) {
								if (row.type === 'Folder') {
									data = '';
								} else {
									var dt = new Date(row.lastModified);
									var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
									var tmoptions = { hour: 'numeric', minute: 'numeric' };
									data = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
								}
								return data;
							}
						}
					]
				});

				this.sharedDataTable = formsFolders.$sharedTable.DataTable({
					paging: true,
					pageLength: 25,
					serverSide: true,
					deferLoading: 0,
					"autoWidth": false,
					"columnDefs": [
						{ "width": "5em", "targets": 0 },
						{ "width": "10em", "targets": 1 },
						{ "width": "70em", "targets": 2 },
						{ "width": "5em", "targets": 3 },
						{ "width": "10em", "targets": 4 }
					],
					listAction: {
						ajaxFunction: _formsService.getAllShared,
						inputFilter: function () {
							return {
								filter: $('#FormsTableFilter').val(),
								id: Id.toString(),
								descriptionFilter: $('#DescriptionFilterId').val(),
								docPDFFilter: $('#DocPDFFilterId').val(),
								docUserCanSaveFilter: $('#DocUserCanSaveFilterId').val(),
								docWordFilter: $('#DocWordFilterId').val(),
								docWordPaidFilter: $('#DocWordPaidFilterId').val(),
								nameFilter: $('#NameFilterId').val(),
								minPaymentAmountFilter: $('#MinPaymentAmountFilterId').val(),
								maxPaymentAmountFilter: $('#MaxPaymentAmountFilterId').val(),
								paymentCurrFilter: $('#PaymentCurrFilterId').val(),
								paymentEnabledFilter: $('#PaymentEnabledFilterId').val(),
								type: 'F'
							};
						}
					},
					columnDefs: [
						{
							targets: 3,
							data: null,
							orderable: false,
							defaultContent: '',
							rowAction: {
								text: app.localize('Actions'),
								items: [
									//Form actions
									{
										text: app.localize('Open Form'),
										visible: function (data) {
											return true;
										},
										action: function (data) {
											var openFormUrl = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/forms/load?OriginalId=' + data.record.originalId + '&version=live';
											window.open(openFormUrl);
										}
									},
									{
										text: app.localize('Accept'),
										visible: function (data) {
											return _permissions.delete
										},
										action: function (data) {
											acceptSharedForm(data.record);
										}
									},
									{
										text: app.localize('Remove'),
										visible: function () {
											return _permissions.delete;
										},
										action: function (data) {

											removeSharedForm(data.record);
										}
									}]
							}
						},
						{
							targets: 0,
							data: null,
							name: "name",
							render: function (data, type, row) {



								var port = window.location.port !== null ? ":" + window.location.port : "";
								data = `<a href="${location.protocol}//${window.location.hostname}${port}/Falcon/forms/load?OriginalId=${data.originalId}&version=live"><img class="stq-primary-icon-lg me-2" title="form" src="/common/images/primaryicons/form.png"></i> <span class="text-black" >${data.name}</span><div style="overflow-y: auto; max-height: 5em;"><div style="white-space: pre-wrap;">${data.description}</div></span></a>`;
								return data;
							}

						},
						{
							targets: 1,
							data: null,
							name: "sharedby",
							render: function (data, type, row) {
								return data.tenancyName;
							}

						},
						{
							targets: 2,
							data: null,
							name: "accepted",
							render: function (data, type, row) {
								return data.accepted;
							}

						}
					]
				});

			}
		};

		function getForms() {
			formsFolders.dataTable.ajax.reload();
		}

		function deleteForm(data) {
			abp.message.confirm(
				`Delete Form: ${data.name}`,
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_formsService.deleteAll({
							id: data.originalId
						}).done(function () {
							getForms(true);
							abp.notify.success(app.localize('SuccessfullyDeletedForm'));
						});
					}
				}
			);
		}

		function deleteFolder(data) {
			abp.message.confirm(
				`Delete Folder: ${data.name}`,
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_foldersService.delete({
							id: data.id
						}).done(function (result) {
							if (result.success == true) {
								getForms(true);
								abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteFolder'));
							}
						});
					}
				}
			);
		}

		function acceptSharedForm(item) {
			abp.message.confirm(
				`Accept this shared Form : ${item.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {

						var acl = {};
						acl.Type = "Form";
						acl.Role = "V";
						acl.EntityId = item.id;

						_aclService.acceptSharedByTenant(acl).done(function () {
							abp.notify.success(app.localize('SuccessfullyAccepted'));
						});
					}
				}
			);
		}

		function removeSharedForm(item) {
			abp.message.confirm(
				`Remove this Shared Form : ${item.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {

						var acl = {};
						acl.Type = "Form";
						acl.Role = "V";
						acl.EntityId = item.id;

						_aclService.removeSharedByTenant(acl).done(function () {
							sharedDataTable.ajax.reload();
							abp.notify.success(app.localize('SuccessfullyRemoved'));
						});
					}
				}
			);
		}

		function loadPartialView(Id) {
			var FolderId = Id !== null ? Id.toString() : '00000000-0000-0000-0000-000000000000';
			if (Id !== null) {
				abp.ui.setBusy($("body"));
			}

			$.ajax({
				url: '/Falcon/Forms/FormsPartial',
				type: 'POST',
				data: { Id: FolderId },
				dataType: 'html'
			}).done(function (partialViewResult) {
				$("#FormFolderPortlet").html(partialViewResult);
				formsFolders.setViewObjects();
				formsFolders.init(FolderId);
				formsFolders.dataTable.ajax.reload(function () {
					abp.ui.clearBusy($("body"));
					formsFolders.setEvents();
				});

				$('#GetFormsButton').click(function (e) {
					e.preventDefault();
					getForms();
				});

				$('#clearbtn').click(function () {
					$('#FormsTableFilter').val('');
					getForms();
				});

				$('#FormsTableFilter').keyup(function (event) {
					debugger;
					// On enter
					if (event.keyCode === 13) {
						getForms();
					}
				});
				formsFolders.sharedDataTable.ajax.reload(function () {
					abp.ui.clearBusy($("body"));
				});

			});
		}

		loadPartialView(null);

		$('#ShowAdvancedFiltersSpan').click(function () {
			formsFolders.$showAdvancedFiltersSpan.hide();
			formsFolders.$hideAdvancedFiltersSpan.show();
			formsFolders.$advacedAuditFiltersArea.slideDown();
		});

		$('#HideAdvancedFiltersSpan').click(function () {
			formsFolders.$hideAdvancedFiltersSpan.hide();
			formsFolders.$showAdvancedFiltersSpan.show();
			formsFolders.$advacedAuditFiltersArea.slideUp();
		});

		$('#FormNavigationButton').click(function (event, Id) {

			loadPartialView(Id);
		});

		$('#CreateNewFormButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditFormModal.open({ Id: null, FolderId: Id });
		});

		$('#CreateNewFolderButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditFolderModal.open({ Id: null, parentid: Id, type: "F" });
		});

		$('#SetLiveFormButton').click(function (event, Id, Version) {
			_formsService.setCurrent({
				originalId: Id, version: Version
			}).done(function () {
				_modalManager.close();
				abp.event.trigger('app.createOrEditFormModalSetLive');
			});
		});

		abp.event.on('app.createOrEditFolderModalSaved', function () {
			getForms();
		});

		abp.event.on('app.createOrEditFormModalSaved', function () {
			getForms();
		});

		abp.event.on('app.createOrEditFormModalToggled', function () {
			getForms();
			abp.notify.success(app.localize('SuccessfullyToggledForm'));
		});

		abp.event.on('app.createOrEditFormModalDeleted', function () {
			getForms(true);
			abp.notify.success(app.localize('SuccessfullyDeletedFormVersion'));
		});
		abp.event.on('app.SetAliveModalSetLive', function () {
			getForms(true);
		});

		$('#GetFormsButton').click(function (e) {
			e.preventDefault();
			getForms();
		});
		abp.event.on('app.createOrEditFormModalSetLive', function () {
			getForms(true);
		});

		$('#closeNotificationButton').click(function (e) {
			e.preventDefault();
			$("#setLiveToast").removeClass("show");
		});

		$('#notifyButton').click(function (e) {
			e.preventDefault();
			$("#setLiveToast").removeClass("show");
			_manageNotificationModal.open({ defaultMessage: $("#SetAliveHeaderMessage").text(), entityNames: $('#setLiveFormName').text() });
		});
		//$(".bootstrap-tagsinput").find("input").change( function () {
		$(document).on('click', function (e) {
			var container = $(".toast.show");
			if (!$(e.target).closest(container).length) {
				container.removeClass("show");
			}
		});

		abp.event.on('app.grantAccessSuccess', function () {
			getForms();
		});

		abp.event.on('app.revokeAccessSuccess', function () {
			getForms();
		});

		/*		$(document).keypress(function (e) {
					if (e.which === 13) {
						getForms();
					}
				});*/

	});
})();