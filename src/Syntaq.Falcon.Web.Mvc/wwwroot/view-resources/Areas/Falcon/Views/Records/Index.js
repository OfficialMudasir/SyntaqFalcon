(function () {
	$(function () {
		abp.ui.setBusy($("body"));

		var _foldersService = abp.services.app.folders;
		var _recordsService = abp.services.app.records;
		var _recordMattersService = abp.services.app.recordMatters;
		//var _aCLService = abp.services.app.aCLs;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Records.Create'),
			edit: abp.auth.hasPermission('Pages.Records.Edit'),
			'delete': abp.auth.hasPermission('Pages.Records.Delete')
		};

		var _folderPermissions = {
			edit: abp.auth.hasPermission('Pages.Folders.Edit'),
			'delete': abp.auth.hasPermission('Pages.Folders.Delete')
		};

		var _createOrEditRecordModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Records/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Records/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditRecordModal'
		});

		var _viewRecordsJSONModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Records/ViewRecordsJSONModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Records/_ViewRecordsJSONModal.js',
			modalClass: 'ViewRecordsJSONModal'
		});

		var _createOrEditRecordMatterModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/RecordMatters/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatters/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditRecordMatterModal'
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

		var _viewFilesModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Files/ViewFilesModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Files/_ViewFilesModal.js',
			modalClass: 'ViewFilesModal'
		});

		var _createEmbedModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Embedding/CreateModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Embedding/_CreateModal.js',
			modalClass: 'CreateEmbedModal'
		});
		var _viewNewFormForRecordModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Records/NewFormForRecord',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Records/_NewFormForRecord.js',
			modalClass: 'NewFormForRecord'
		});

		var _viewNewAppForRecordModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Records/NewAppForRecord',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Records/_NewAppForRecord.js',
			modalClass: 'NewAppForRecord'
		});


		var recordsFolders = {
			$table: null,

			/*@* 10443 Record Archving issue Resolved *@*/
			//$showAdvancedFiltersSpan: null,
			//$hideAdvancedFiltersSpan: null,
			//$advacedAuditFiltersArea: null,
			dataTable: null,
			setViewObjects: function () {
				this.$table = $('#RecordsTable');
				/*	@* 10443 Record Archving issue Resolved *@*/

				//this.$showAdvancedFiltersSpan = $('#ShowAdvancedFiltersSpan');
				//this.$hideAdvancedFiltersSpan = $('#HideAdvancedFiltersSpan');
				//this.$advacedAuditFiltersArea = $('#AdvacedAuditFiltersArea');
			},
			setEvents: function () {
				$('.dataTable').on('click', 'tbody tr td:nth-child(1)', function (event) {
					if (event.target.nodeName == "TD" || event.target.nodeName == "STRONG" || event.target.nodeName == "IMG" || event.target.nodeName == "SPAN") {
						var data = recordsFolders.dataTable.row(this).data();
						if (data.type === 'Folder') {
							$('#RecordNavigationButton').trigger('click', data.id.toString());
						}
					}
				});

				$('#RefreshRecordsButton').click(function () {
					var Id = $("span[name*='Current']").next("input").val();
					loadPartialView(Id);
				});
			},
			selectedRecord: {
				id: null,
				displayName: null,
				lastModified: null,
				Comments: 'Comments XYZ',

				set: function (ID, DisplayName, LastModified, Comments) {
					if (ID === null) {
						recordsFolders.selectedRecord.id = null;
						recordsFolders.selectedRecord.displayName = null;
						recordsFolders.selectedRecord.lastModified = null;
						recordsFolders.selectedRecord.Comments = null;
					} else {
						recordsFolders.selectedRecord.id = ID;
						recordsFolders.selectedRecord.displayName = DisplayName;
						recordsFolders.selectedRecord.lastModified = LastModified;
						recordsFolders.selectedRecord.Comments = Comments;
					}
				}
			},
			init: function (Id) {
				this.dataTable = recordsFolders.$table.DataTable({
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
								$(this).find('td:nth-child(1)').attr('data-droptype', 'Record');
							}
						});
						$(row).find("[name='ShareFolderLink']").on("click", function () {
							_manageACLModal.open({ entityid: data.id, entityname: data.name, entitytype: 'Folder' });
						});
						$(row).find("[name='FilesViewLink']").on("click", function () {
							var MatNum = $(this).attr('data-identifier');
							var MatItemId = $(this).attr('data-matitemid');
							_viewFilesModal.open({ recordId: data.id, recordMatterId: data.recordMatters[MatNum].id, recordMatterItemGroupId: MatItemId, recordMatterName: data.recordMatters[MatNum].recordMatterName });
						});
						$(row).find("[name='EditFolderLink']").on("click", function () {
							_createOrEditFolderModal.open({ id: data.id, parentid: "00000000-0000-0000-0000-000000000000", orgid: null });
						});
						$(row).find("[name='DeleteFolderLink']").on("click", function () {
							deleteFolder(data);
						});
						$(row).find("[name='ShareRecordLink']").on("click", function () {
							_manageACLModal.open({ entityid: data.id, entityname: data.name, entitytype: 'Record' });
						});

						$(row).find("[name='RecordJSONLink']").on("click", function () {
							_viewRecordsJSONModal.open({ id: data.id });
						});
						$(row).find("[name='DeleteRecordLink']").on("click", function () {
							deleteRecord(data);
						});
						$(row).find("[name='EditRecordMatterLink']").on("click", function () {
							var MatNum = $(this).attr('data-identifier');
							_createOrEditRecordMatterModal.open({ id: data.recordMatters[MatNum].id });
						});
						$(row).find("[name='DeleteRecordMatterLink']").on("click", function () {
							var MatNum = $(this).attr('data-identifier');
							deleteRecordMatter(data.recordMatters[MatNum]);
						});

						$(row).find("[name='NewFormForRecordLink']").on("click", function () {
							_viewNewFormForRecordModal.open({ id: data.id });
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
									_recordsService.move({
										draggableId: draggableid,
										draggableType: type,
										id: droppableid,
										folderType: 'R'
									}).done(function (data) {
										if (data === true) {
											getRecords();
											abp.notify.success(app.localize('SuccessfullyMoved' + type + ''));
										} else {
											getRecords();
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

						abp.ui.clearBusy($("body"));

					},
					listAction: {
						ajaxFunction: _recordsService.getAll,
						inputFilter: function () {
							return {
								filter: $('#RecordsTableFilter').val(),
								recordNameFilter: $('#RecordNameFilterId').val(),
								id: Id.toString(),/*'B2C927B1-85D7-49C3-DFAC-08D600A6F5AF'$('').val()*/
								type: 'R',
								isArchived: $('#RecordIsArchivedFilter').prop('checked')
							};
						}
					},
					columnDefs: [
						{
							responsivePriority: 1,
							targets: 2,
							data: null,
							orderable: false,
							defaultContent: '',
							rowAction: {
								text: app.localize('Actions'),
								items: [
									//Folder actions
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
									//Record actions
									{
										text: 'Share',
										visible: function (data) {
											if (data.record.type !== 'Record') return false;
											return data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_manageACLModal.open({ entityid: data.record.id, entityname: data.record.name, entitytype: 'Record' });
										}
									},
									{
										text: 'New Form',
										visible: function (data) {
											if (data.record.type !== 'Record') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_viewNewFormForRecordModal.open({ id: data.record.id });
										}
									},
									{
										text: 'Run App',
										visible: function (data) {
											if (data.record.type !== 'Record') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_viewNewAppForRecordModal.open({ id: data.record.id });
										}
									},
									{
										text: 'JSON',
										visible: function (data) {
											if (data.record.type !== 'Record') return false;
											return data.record.userACLPermission === "V" || data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											_viewRecordsJSONModal.open({ id: data.record.id });
										}
									},
									{
										text: 'Delete',
										visible: function (data) {
											if (data.record.type !== 'Record') return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											deleteRecord(data.record);
										}
									},
									{
										text: 'Archive',
										visible: function (data) {
											debugger;
											if (data.record.type !== 'Record' || data.record.isArchived === true) return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											archiveRecord(data.record);
										}
									},
									{
										text: 'Un-Archive',
										visible: function (data) {
											debugger;
											if (data.record.type !== 'Record' || data.record.isArchived === false) return false;
											return data.record.userACLPermission === "E" || data.record.userACLPermission === "O" ? true : false;
										},
										action: function (data) {
											unArchiveRecord(data.record);
										}
									}
								]
							}
						},
						{

							//width: 35,
							targets: 0,
							width: "150px",
							data: "name",
							render: function (data, type, row) {
								if (row.type === 'Folder') {
									debugger;
									// onClick="$('#RecordNavigationButton').trigger('click', row.id);"

									var id = row.id;

									data = '<span href="javascript:;"   >';
									data += '<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/folder.png">';
									data += '<span> ' + row.name + '</span>';
									data += '</span>';
								} else {
									data = '';// '<i class="text-info fa fa-archive fa-2x fa-lg m-1" style="font-size: 2.2em;"></i>';
									data += '<div style="display:inline-block; height:100%;top:6px;"><span class=" large" onClick="(function () { $(\'#LoadRecordMattersButton\').trigger(\'click\', [\'' + row.id.toString() + '\', \'' + row.name + '\',  \'' + row.lastModified + '\', \'' + row.comments + '\']) })();"  > <img class="stq-primary-icon me-2" title="Form" src="/common/images/primaryicons/record.png"></i> <span >' + row.name + ' ' + '</span></span></div>';

									var dt = new Date(row.lastModified);
									var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
									var tmoptions = { hour: 'numeric', minute: 'numeric' };
									data += `<div>${dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions)}</div>`;

								}
								return data;
							}
						},
						{
							targets: 1,
							data: null,
							orderable: false,
							render: function (data, type, row) {

								if (row.type === 'Folder') {
									data = '';
								} else {
									data = '<div class="mt-2 mb-6">';
									if (row.recordMatters !== null) {
										var rmc = 0;
										row.recordMatters.forEach(function (recordmatter) {
											//data += '<span style="width: 110px;"><i class="text-info fas fa-book fa-lg"></i> </span>';
											var dt = new Date(recordmatter.lastModified);
											var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
											dt = dt.toLocaleDateString('en-GB', options);

											data += '<span style="width: 350px;">';

											if (recordmatter.formId !== null) {
												data += '<a class="OnClickLink me-1" href="/Falcon/forms/load?OriginalId=' + recordmatter.formId + '&RecordMatterId=' + recordmatter.id + '&version=live"><img class="stq-primary-icon-lg me-2" title="Form" src="/common/images/primaryicons/form.png"></i></a>';
											} else {
												data += '<span class="me-1" href="/Falcon/forms/load?OriginalId=' + recordmatter.formId + '&RecordMatterId=' + recordmatter.id + '&version=live"><img class="stq-primary-icon mx-2" title="Form" src="/common/images/primaryicons/cog.png"></i></span>';
											}

											data += '<span >' + recordmatter.recordMatterName + '</span>';

											data += '<span class="pull-right ">';
											data += '<span class="me-2">' + dt + '</span> ';
											//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="EditRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-edit"></i> Edit</a> | ' : '';
											data += row.userACLPermission === "E" || row.userACLPermission === "O" ? ' | <a class="OnClickLink ms-2" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-trash-alt text-danger"></i> </a>' : '';
											data += '</span>';

											data += '<table class="mb-2 table table-row-dashed table-hover bg-transparent" style="width:100%">';

											data += '<tr>';
											data += '<td class="pt-1 pb-1 pl-0 pr-0" style="white-space: pre-wrap; position: relative;">';
											if (recordmatter.recordMatterItems !== null) {
												recordmatter.recordMatterItems.forEach(function (recordmatteritem) {

													dt = new Date(recordmatteritem.lastModified);
													var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
													var tmoptions = { hour: 'numeric', minute: 'numeric' };
													dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

													if (recordmatteritem.document === true) {

														if (recordmatteritem.formId === '00000000-0000-0000-0000-000000000000' || recordmatteritem.formId === null) {
															// data += '<img src="https://img.icons8.com/color/settings.png" title="Document build using App" height=32/>';
														}
														else {
															if (recordmatteritem.lockOnBuild === true) {
																data += '<i class="fa fa-lock" style="font-size: 1.8em;" title="Form has been submitted and cannot be submitted again."  /></i>';
															}
															else {
																//data += '<a class="OnClickLink" href="/Falcon/forms/load?OriginalId=' + recordmatteritem.formId + '&RecordMatterId=' + recordmatter.id + '&RecordMatterItemId=' + recordmatteritem.id + '&version=live"><span class="fa-stack fa-1x text-primary stq-v-center"><i class="far fa-window-maximize fa-stack-2x"></i><i class="fa fa-list fa-stack-1x" style="margin-top:1px"></i></span></a>';
															}
														}

														if (recordmatteritem.allowPdf) {
															data += '<a class="" href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=pdf"><img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/pdf.svg"></i></a>';
														}
														if (recordmatteritem.allowWord) {
															data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=docx"><img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/doc.svg"></i></a>';
														}
														if (recordmatteritem.allowHTML) {
															data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=html"><img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/html.png"></i></a>';
														}
														if (recordmatter.hasFiles) {
															data += '<a class="OnClickLink" name="FilesViewLink" data-matitemid="' + recordmatteritem.groupId + '" data-identifier="' + rmc + '"><img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/download.png"></a>';
														}
														if (recordmatteritem.documentName === "" || recordmatteritem.documentName === null) {
															var now = new Date();
															var then = new Date(recordmatteritem.creationTime);
															var difference = Math.round((new Date() - new Date(recordmatteritem.creationTime)) / 60000);
															data += difference < 5 ? 'Document still assembling' : 'Document Name not set';
														} else {
															data += '<span class="me-2">' + recordmatteritem.documentName + '</span>';
														}
													} else {

														data += '<a class="OnClickLink stq-v-center" title="Open in Form"  href="/Falcon/forms/load?OriginalId=' + recordmatteritem.formId + '&RecordMatterId=' + recordmatter.id + '&RecordMatterItemId=' + recordmatteritem.id + '&version=live"></a>';
														if (recordmatter.hasFiles) {
															//data += '<a class="OnClickLink stq-v-center" name="FilesViewLink"  data-matitemid="' + recordmatteritem.groupId + '" data-identifier="' + rmc + '"><i class="kt-font-info fas fa-download m-1" title="Download Files" style="font-size: 1.8em;"></i></a>';
															data += '<a class="OnClickLink stq-v-center" name="FilesViewLink"  data-matitemid="' + recordmatteritem.groupId + '" data-identifier="' + rmc + '"><img class="stq-primary-icon me-2" title="Download Files" src="/common/images/primaryicons/download.png"></a>';
														}

														var frmname = recordmatteritem.documentName;
														if (frmname === null) {
															frmname = 'Open in Form';
														}

														//	data += '<a class="OnClickLink stq-v-center" title="Open in Form" href="/Falcon/forms/load?OriginalId=' + recordmatteritem.formId + '&RecordMatterId=' + recordmatter.id + '&RecordMatterItemId=' + recordmatteritem.id + '&version=live">' + frmname + ' </a>';
													}

													//data += '<span class="pull-right mt-2 pr-0">' + dt + '</span>';

												});

												data += '</td>';
												data += '</tr>';
											}

											data += '</table>';
											rmc = rmc + 1;
										});
										data += '</hr>';
										data += '</div>';
									}
								}
								return data;
							}
						}//,
						//{
						//	targets: 3,
						//	data: "lastModified",
						//	render: function (data, type, row) {
						//		if (row.type === 'Folder') {
						//			data = '';
						//		} else {
						//			var dt = new Date(row.lastModified);
						//			var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
						//			var tmoptions = { hour: 'numeric', minute: 'numeric' };
						//			data = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
						//		}
						//		return data;
						//	}
						//}

					]
				}
				);
			}
		};

		function loadPartialView(Id) {
			var FolderId = Id !== null ? Id.toString() : '00000000-0000-0000-0000-000000000000';
			if (Id !== null) {
				abp.ui.setBusy($("body"));
			}
			$.ajax({
				url: '/Falcon/Records/RecordsPartial',
				type: 'POST',
				data: { Id: FolderId },
				dataType: 'html'
			}).done(function (partialViewResult) {

				$("#RecordFolderPortlet").html(partialViewResult);
				recordsFolders.setViewObjects();
				recordsFolders.init(FolderId);
				recordsFolders.dataTable.ajax.reload(function () { abp.ui.clearBusy($("body")); });
				recordsFolders.setEvents();

				$('#GetRecordsButton').click(function (e) {
					getRecords();
				});

				$("#RecordsTableFilter").keyup(function (event) {
					// On enter 
					if (event.keyCode === 13) {
						getRecords();
					}
				});

				$('#clearbtn').click(function () {
					$('#RecordsTableFilter').val('');
					getRecords();
				});

				/*@* 10443 Record Archving issue Resolved *@*/

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

			});
		}

		loadPartialView(null);

		function getRecords() {
			abp.ui.setBusy($("body"));
			recordsFolders.dataTable.ajax.reload(/*function () { RemoveSortObject(); }*/);
		}

		$(_manageACLModal).on('hidden.bs.modal', function () {
			recordsFolders.dataTable.ajax.reload();
		});

		function deleteRecord(data) {
			abp.message.confirm(
				`Delete Record: ${data.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordsService.delete({
							id: data.id
						}).done(function (result) {
							//getRecords();
							//abp.notify.success(app.localize('SuccessfullyDeletedRecord'));
							if (result.success == true) {
								getRecords();
								abp.notify.success(app.localize('SuccessfullyDeletedRecord'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteRecord'));
							}
						});
					}
				}
			);
		}

		function archiveRecord(data) {

			abp.message.confirm(
				`Archive Record: ${data.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordsService.archive({
							id: data.id
						}).done(function (result) {
							//getRecords();
							//abp.notify.success(app.localize('SuccessfullyDeletedRecord'));
							if (result.success == true) {
								getRecords();
								abp.notify.success(app.localize('SuccessfullyArchivedRecord'));
							} else {
								abp.notify.warn(app.localize('FailedToArchiveRecord'));
							}
						});
					}
				}
			);
		}


		function unArchiveRecord(data) {

			abp.message.confirm(
				`Archive Record: ${data.name}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordsService.unArchive({
							id: data.id
						}).done(function (result) {
							//getRecords();
							//abp.notify.success(app.localize('SuccessfullyDeletedRecord'));
							if (result.success == true) {
								getRecords();
								abp.notify.success(app.localize('SuccessfullyArchivedRecord'));
							} else {
								abp.notify.warn(app.localize('FailedToArchiveRecord'));
							}
						});
					}
				}
			);
		}

		function deleteRecordMatter(data) {
			abp.message.confirm(
				`Delete Matter: ${data.recordMatterName}`,
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordMattersService.delete({
							id: data.id
						}).done(function () {
							getRecords();
							abp.notify.success(app.localize('SuccessfullyDeletedRecordMatter'));
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
							//getRecords();
							//abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							if (result.success == true) {
								getRecords();
								abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteFolder'));
							}
						});
					}
				}
			);
		}

		//$('#ShowAdvancedFiltersSpan').click(function () {
		//	recordsFolders.$showAdvancedFiltersSpan.hide();
		//	recordsFolders.$hideAdvancedFiltersSpan.show();
		//	recordsFolders.$advacedAuditFiltersArea.slideDown();
		//});

		//$('#HideAdvancedFiltersSpan').click(function () {
		//	recordsFolders.$hideAdvancedFiltersSpan.hide();
		//	recordsFolders.$showAdvancedFiltersSpan.show();
		//	recordsFolders.$advacedAuditFiltersArea.slideUp();
		//});

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

		$('#RecordNavigationButton').click(function (event, Id) {
			loadPartialView(Id);
		});

		$('#CreateNewRecordButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditRecordModal.open({ Id: null, FolderId: Id/*, orgid: null */ });
		});

		$('#CreateNewFolderButton').click(function () {
			var Id = $("span[name*='Current']").next("input").val();
			_createOrEditFolderModal.open({ Id: null, parentid: Id, type: "R" });
		});

		$('#CreateEmbedCodeButton').click(function () {
			_createEmbedModal.open({ originalId: null, formId: null, type: "Record" });
		});

		$('#CreateNewRecordMatterButton').click(function (Id) {
			_createOrEditRecordMatterModal.open({ Id: null, recordId: recordsFolders.selectedRecord.id });
		});

		abp.event.on('app.createOrEditRecordModalSaved', function () {
			getRecords();
		});

		abp.event.on('app.createOrEditRecordMatterModalSaved', function () {
			getRecords();
		});

		abp.event.on('app.createOrEditFolderModalSaved', function () {
			getRecords();
		});

		/*		$(document).keypress(function (e) {
					if (e.which === 13) {
						getRecords();
					}
				});*/


	});
})();