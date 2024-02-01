(function () {
	$(function () {
		abp.ui.setBusy($("body"));

		var _foldersService = abp.services.app.folders;
		var _templatesService = abp.services.app.templates;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Templates.Create'),
			edit: abp.auth.hasPermission('Pages.Templates.Edit'),
			'delete': abp.auth.hasPermission('Pages.Templates.Delete')
		};

		var _folderPermissions = {
			edit: abp.auth.hasPermission('Pages.Folders.Edit'),
			'delete': abp.auth.hasPermission('Pages.Folders.Delete')
		};

		var _createOrEditTemplateModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Templates/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Templates/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditTemplateModal'
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

		var _manageNotificationModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Notifications/ManageRecipientsModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Notifications/_ManageRecipientsModal.js',
			modalClass: 'ManageNotificationRecipients'
		});

		function copytext(str) {
			const el = document.createElement('textarea');
			el.value = str;
			el.setAttribute('readonly', '');
			el.style.position = 'absolute';
			el.style.left = '-9999px';
			document.body.appendChild(el);
			el.select();
			document.execCommand('copy');
			document.body.removeChild(el);

		};

		var templatesFolders = {
			$table: null,
			$showAdvancedFiltersSpan: null,
			$hideAdvancedFiltersSpan: null,
			$advacedAuditFiltersArea: null,
			dataTable: null,

			setViewObjects: function () {
				this.$table = $('#TemplatesTable');
				this.$showAdvancedFiltersSpan = $('#ShowAdvancedFiltersSpan');
				this.$hideAdvancedFiltersSpan = $('#HideAdvancedFiltersSpan');
				this.$advacedAuditFiltersArea = $('#AdvacedAuditFiltersArea');
			},

			showTable: function () {
				templatesFolders.$table.show();
			},

			hideTable: function () {
				templatesFolders.$table.hide();
			},

			setEvents: function () {
				$('.dataTable').on('click', 'tbody tr td', function (event) {
					if (event.target.nodeName === "TD" || event.target.nodeName == "SPAN" || event.target.nodeName == "IMG") {
						var data = templatesFolders.dataTable.row(this).data();
						if (data.type === 'Folder') {
							$('#TemplateNavigationButton').trigger('click', data.id.toString());
						} else {
							if (data.userACLPermission === "E" || data.userACLPermission === "O") {
								_createOrEditTemplateModal.open({ id: data.id, originalId: data.originalId, folderId: "00000000-0000-0000-0000-000000000000" });
							}
						}
					}
				});

				$('#RefresTemplatesButton').click(function () {
					var Id = $("span[name*='Current']").next("input").val();
					loadPartialView(Id);
				});
			},

			load: function () {
				this.setViewObjects();
				this.init();
				this.showTable();
				this.dataTable.ajax.reload();
			},

			init: function (Id) {
				this.dataTable = templatesFolders.$table.DataTable({
					paging: true,
					serverSide: true,
					pageLength: 25,
					//processing: true,
					deferLoading: 0,
					createdRow: function (row, data, dataIndex) {
						$(row).each(function (i) {

							$(this).find('td:nth-child(1)').addClass('draggable');
							$(this).find('td:nth-child(1)').attr('data-id', data.id);

							if (data.type === 'Folder') {
								$(this).addClass('droppable');
								$(this).find('td:nth-child(1)').attr('data-droptype', 'Folder');
							} else {
								$(this).find('td:nth-child(1)').attr('data-droptype', 'Template');
							}
						});

						$(row).find("[name='ShareTemplateLink']").on("click", function () {
							_manageACLModal.open({ entityid: data.id, entityname: data.name, entitytype: 'DocumentTemplate' });
						});

						$(row).find("[name='EditTemplateLink']").on("click", function () {
							if (data.type === 'Folder') {
								_createOrEditFolderModal.open({ id: data.id, parentid: "00000000-0000-0000-0000-000000000000", orgid: null });
							} else {
								_createOrEditTemplateModal.open({ id: data.id, originalId: data.originalId, folderId: "00000000-0000-0000-0000-000000000000" });
							}
						});

						$(row).find("[name='CopyLiveTemplateLink']").on("click", function () {
							//var clipboard = new ClipboardJS("[name='CopyLiveTemplateLink']");
							var url = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + "/Falcon/Templates/gettemplate?OriginalId=" + data.originalId + "&version=live"
							copytext(url);
						});

						$(row).find("[name='CopyLiveTemplateIdLink']").on("click", function () {
							//var clipboard = new ClipboardJS("[name='CopyLiveTemplateIdLink']");
							copytext(data.originalId);
						});


						$(row).find("[name='DeleteTemplateLink']").on("click", function () {
							if (data.type === 'Folder') {
								deleteFolder(data);
							} else {
								deleteTemplate(data);
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
								droppableid = droppableid == undefined ? "00000000-0000-0000-0000-000000000000" : droppableid;
								if (draggableid != droppableid) {
									_templatesService.move({
										draggableId: draggableid,
										draggableType: type,
										id: droppableid,
										folderType: 'T'
									}).done(function (data) {
										if (data === true) {
											getTemplates(true);
											abp.notify.success(app.localize('SuccessfullyMoved' + type + ''));
										} else {
											getTemplates(true);
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
						ajaxFunction: _templatesService.getAll,
						inputFilter: function () {
							return {
								filter: $('#TemplatesTableFilter').val(),
								nameFilter: $('#NameFilterId').val(),
								minVersionFilter: $('#MinVersionFilterId').val(),
								maxVersionFilter: $('#MaxVersionFilterId').val(),
								minCurrentVersionFilter: $('#MinCurrentVersionFilterId').val(),
								maxCurrentVersionFilter: $('#MaxCurrentVersionFilterId').val(),
								id: Id.toString(),
								type: 'T'
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
									//Template actions
									{
										text: app.localize('Share'),
										visible: function (data) {
											if (data.record.type !== 'Template') return false;
											return data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_manageACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'DocumentTemplate' });

										}
									},
									{
										text: app.localize('Edit'),
										visible: function (data) {
											if (data.record.type !== 'Template') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_createOrEditTemplateModal.open({ id: data.record.id, originalId: data.record.originalId, folderId: "00000000-0000-0000-0000-000000000000" });
										}
									},
									{
										text: app.localize('Copy Doc ID'),
										visible: function (data) {
											if (data.record.type !== 'Template') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											abp.notify.success('', 'Copied Document ID', { 'positionClass': 'toast-bottom-right' });
											copytext(data.record.originalId);
										}
									},
									{
										text: app.localize('Copy Link URL'),
										visible: function (data) {
											if (data.record.type !== 'Template') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											abp.notify.success('', 'Copied Document URL', { 'positionClass': 'toast-bottom-right' });
											var url = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + "/Falcon/Templates/gettemplate?OriginalId=" + data.record.originalId + "&version=live"
											copytext(url);
										}
									},
									{
										text: app.localize('Delete'),
										visible: function (data) {
											if (data.record.type !== 'Template') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											deleteTemplate(data.record);
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
									data = '<img class="stq-primary-icon me-2" title="folder" src="/common/images/primaryicons/folder.png"></i> ';
									data += '<span class="font-weight-bold">' + row.name + ' </span>';
								} else {
									data = '<img class="stq-primary-icon me-2" title="doc" src="/common/images/primaryicons/doc.svg"></i> ';
									data += '<span class="font-weight-bold">' + row.name + ' </span>';
									data += '<div class="font-weight-bold">' + row.comments + ' </div>';
								}
								return data;
							}
						},
						{
							targets: 1,
							data: "version",
							orderable: false,
							render: function (data, type, row) {
								var currentVersion = "";
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
			}
		}

		function getTemplates() {
			templatesFolders.dataTable.ajax.reload();
		}

		function deleteTemplate(data) {
			abp.message.confirm(
				`Delete Template: ${data.name} with all versions`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_templatesService.deleteAll({
							id: data.originalId
						}).done(function (result) {
							//getTemplates(true);
							//abp.notify.success(app.localize('SuccessfullyDeletedTemplate'));
							if (result.success == true) {
								getTemplates(true);
								abp.notify.success(app.localize('SuccessfullyDeletedTemplate'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteTemplate'));
							}
						});
					}
				}
			);
		}

		function deleteFolder(data) {
			abp.message.confirm(
				`Delete Folder: ${data.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_foldersService.delete({
							id: data.id
						}).done(function (result) {
							//getTemplates(true);
							//abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							if (result.success == true) {
								getTemplates(true);
								abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteFolder'));
							}
						});
					}
				}
			);
		}

		function loadPartialView(Id) {
			debugger;
			var FolderId = Id !== null ? Id.toString() : '00000000-0000-0000-0000-000000000000';
			if (Id !== null) {
				abp.ui.setBusy($("body"));
			}

			$.ajax({
				url: '/Falcon/Templates/TemplatesPartial',
				type: 'POST',
				data: { Id: FolderId },
				dataType: 'html'
			}).done(function (partialViewResult) {
				$("#TemplateFolderPortlet").html(partialViewResult);
				templatesFolders.setViewObjects();
				templatesFolders.init(FolderId);
				templatesFolders.dataTable.ajax.reload(function () {
					abp.ui.clearBusy($("body"));
					templatesFolders.setEvents();
				});

				$('#GetTemplatesButton').click(function (e) {
					debugger;
					e.preventDefault();
					getTemplates();
				});

				$('#clearbtn').click(function () {
					$('#TemplatesTableFilter').val('');
					getTemplates();
				});

				$("#TemplatesTableFilter").keyup(function (event) {
					// On enter 
					if (event.keyCode === 13) {
						getTemplates();
					}
				});
			});
		}

		loadPartialView(null);

		$('#ShowAdvancedFiltersSpan').click(function () {
			templatesFolders.$showAdvancedFiltersSpan.hide();
			templatesFolders.$hideAdvancedFiltersSpan.show();
			templatesFolders.$advacedAuditFiltersArea.slideDown();
		});

		$('#HideAdvancedFiltersSpan').click(function () {
			templatesFolders.$hideAdvancedFiltersSpan.hide();
			templatesFolders.$showAdvancedFiltersSpan.show();
			templatesFolders.$advacedAuditFiltersArea.slideUp();
		});

		$('#TemplateNavigationButton').click(function (event, Id) {
			loadPartialView(Id);
		});

		$('#CreateNewTemplateButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditTemplateModal.open({ Id: null, OriginalId: null, FolderId: Id });
		});

		$('#CreateNewFolderButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditFolderModal.open({ Id: null, parentid: Id, type: "T" });
		});

		abp.event.on('app.createOrEditTemplateModalSaved', function () {
			getTemplates();
		});

		abp.event.on('app.createOrEditFolderModalSaved', function () {
			getTemplates();
		});

		abp.event.on('app.createOrEditTemplateModalDeleted', function () {
			getTemplates(true);
			abp.notify.success(app.localize('SuccessfullyDeletedTemplateVersion'));
		});

		abp.event.on('app.SetAliveModalSetLive', function () {
			getTemplates();
		});

		$('#GetTemplatesButton').click(function (e) {
			debugger;
			e.preventDefault();
			console.log("sdkfjksdjfkjbsdflkjbdskjlfb");
			getTemplates();
		});
		$("#TemplatesTableFilter").keyup(function (event) {
			event.preventDefault();
			// On enter 
			if (event.keyCode === 13) {
				getTemplates();
			}
		});
		// Notification 
		$('#closeNotificationButton').click(function (e) {
			e.preventDefault();
			$("#setLiveToast").removeClass("show");
		});

		$('#notifyButton').click(function (e) {
			e.preventDefault();
			$("#setLiveToast").removeClass("show");
			_manageNotificationModal.open({ defaultMessage: $("#SetAliveHeaderMessage").text(), entityNames: $('#setLiveTemplateName').text() });
		});
		//$(".bootstrap-tagsinput").find("input").change( function () {
		$(document).on('click', function (e) {
			var container = $(".toast.show");
			if (!$(e.target).closest(container).length) {
				container.removeClass("show");
			}
		});
/*
		$(document).keypress(function (e) {
			if (e.which === 13) {
				getTemplates();
			}
		});*/

	});
})();