﻿(function () {
	$(function () {
		var _$recordsTable = $('#RecordsTable');
		//var _foldersService = abp.services.app.folders;
		var _recordsService = abp.services.app.records;
		//var _recordMattersService = abp.services.app.recordMatters;
		var AuthToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6ImFkbWluIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJCMkRER0ZTVFVBMlNaU1NXN05QUUxaQjVKUUxCMkdBUiIsInJvbGUiOiJBZG1pbiIsImp0aSI6IjZiZTk0MmFiLWVlODAtNDllYS1hMzVjLTVjYTE5MGY3MWJmYiIsImlhdCI6MTU1NzcyMjA5MSwidG9rZW5fdmFsaWRpdHlfa2V5IjoiYzI1YWIxMWUtYTIwYi00YjhmLThlODYtOTNlZmE0ODAwMGI3IiwidXNlcl9pZGVudGlmaWVyIjoiMSIsIm5iZiI6MTU1Nzg3NDE1MiwiZXhwIjoxNTU3OTYwNTUyLCJpc3MiOiJGYWxjb24iLCJhdWQiOlsiRmFsY29uIiwiRmFsY29uIl19.DVS3RWouFBgMQyiXWcu5iOUszxIULjUd6rNfBShRXl8';
		var BaseUrl = 'http://localhost:62114';
		var FormId = '00000000-0000-0000-0000-000000000000';
		var dataTable = _$recordsTable.DataTable({
			paging: true,
			serverSide: true,
			//deferLoading: 0,
			listAction: {
				ajaxFunction: _recordsService.getAll,
				inputFilter: function () {
					return {
						filter: $('#RecordsTableFilter').val(),
						recordNameFilter: $('#RecordNameFilterId').val(),
						id: FormId.toString(),/*'B2C927B1-85D7-49C3-DFAC-08D600A6F5AF'$('').val()*/
						type: 'R'
					};
				}
			},
			columnDefs: [
				{
					targets: 0,
					width: "200px",
					data: "name",
					render: function (data, type, row) {
						if (row.type === 'Folder') {
							data = '<img src="/Common/Images/Entities/folder.png" width=32  />';

							data += '<strong> ' + row.name + '</strong>';
						} else {
							var dt = new Date(row.lastModified);
							var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
							var tmoptions = { hour: 'numeric', minute: 'numeric' };
							dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

							data = '<img src="/Common/Images/Entities/binders-folder.png" width=45 height=45 style="position:relative; top:-5px;"/>';
							data += /*'<span style="display:inline-block; height:45; position:relative; top:6px;"><a class=" large" onClick="(function () { $(\'#LoadRecordMattersButton\').trigger(\'click\', [\'' + row.id.toString() + '\', \'' + row.name + '\',  \'' + row.lastModified + '\', \'' + row.comments + '\']) })();"  >*/'<strong>' + row.name + '</strong>'/* + '</a>'*/;
							data += '<br>' + dt + '</span>';


							data += '<div class="ml-1">';
							//data += row.userACLPermission === "O" ? '<a class="OnClickLink" name="ShareRecordLink"><i class="fa fa-share-alt"></i> Share</a> | ' : '';
							//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="EditRecordLink"><i class="fa fa-edit"></i> Edit</a> | ' : '';
							//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="MoveRecordLink"><i class="fa fa-folder"></i> Move</a> | ' : '';
							//data += row.userACLPermission === "V" || row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="RecordJSONLink"><strong style="letter-spacing: 2px;">{i}</strong> JSON</a>' : '';
							data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="DeleteRecordLink"><i class="fa fa-times text-danger"></i> Delete</a>' : '';
							data += '</div>';
						}
						return data;
					}
				},
				{
					targets: 1,
					data: null,
					render: function (data, type, row) {
						if (row.type === 'Folder') {
							data = '<span class="pull-right mr-5">';
							//data += row.userACLPermission === "O" ? '<a class="OnClickLink" name="ShareFolderLink"><i class="fa fa-share-alt"></i> Share</a> | ' : '';
							//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="EditFolderLink"><i class="fa fa-edit"></i> Edit</a> | ' : '';
							//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="DeleteFolderLink"><i class="fa fa-times text-danger"></i> Delete</a>' : '';
							data += '</span>';
						} else {
							data = '<div>';

							if (row.recordMatters !== null) {
								var rmc = 0;
								row.recordMatters.forEach(function (recordmatter) {
									data += '<span style="width: 110px;"><img src="/Common/Images/Entities/moleskine.png" width=38 height=38 /> </span>';

									var dt = new Date(recordmatter.creationTime);
									var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
									dt = dt.toLocaleDateString('en-GB', options);

									data += '<span style="width: 350px;"><strong>' + recordmatter.recordMatterName + '</strong><span class="ml-3"> </span></span>';

									data += '<span class="pull-right mr-5">';
									//data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="EditRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-edit"></i> Edit</a> | ' : '';
									data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete</a>' : '';
									data += '</span>';
									data += '<table class="ml-3 mb-2 m--bg-fill-light" style="width:95%">';

									if (recordmatter.recordMatterItems !== null) {
										recordmatter.recordMatterItems.forEach(function (recordmatteritem) {

											data += '<tr>';
											dt = new Date(recordmatteritem.creationTime);
											var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
											var tmoptions = { hour: 'numeric', minute: 'numeric' };
											dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

											data += '<td class="ml-3 pt-1 pb-1 pl-1">';

											if (recordmatteritem.document == true) {
												if (recordmatteritem.formId != '00000000-0000-0000-0000-000000000000') {
													data += '<a class="OnClickLink" href="/Falcon/forms/load?OriginalId=' + recordmatteritem.formId + '&RecordMatterId=' + recordmatter.id + '&RecordMatterItemId=' + recordmatteritem.id + '&version=live"><img src="/Common/Images/Entities/form.png" height=34/></a>';
												}

												data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=pdf"><img src="/Common/Images/Entities/pdf.png" height=28/></a>';
                                                data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=docx"><img src="/Common/Images/Entities/word.png" height=28/></a>';
												data += '<a href="/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritem.id + '&version=1&format=html"><img src="/Common/Images/Entities/48/000000/html-filetype.png" height=28/><strong>';
												if (recordmatteritem.documentName == "" || recordmatteritem.documentName == null) {
													var now = new Date();
													var then = new Date(recordmatteritem.creationTime);
													var difference = Math.round((new Date() - new Date(recordmatteritem.creationTime)) / 60000);
													data += difference < 5 ? 'Document still assembling' : 'Document Name not set';
												} else {
													data += recordmatteritem.documentName;
												}
											} else {
												data += '<a class="OnClickLink" href="/Falcon/forms/load?OriginalId=' + recordmatteritem.formId + '&RecordMatterId=' + recordmatter.id + '&RecordMatterItemId=' + recordmatteritem.id + '&version=live"><img src="/Common/Images/Entities/form.png" height=34/> Open in Form</a>';

												if (recordmatter.hasFiles) {
													data += '<a class="OnClickLink" name="FilesViewLink" data-identifier="' + rmc + '"><img src="https://img.icons8.com/color/48/000000/open-document.png" height=30> Files view</a>';
												}
											}

											data += '</td>';
											data += '<td class="pt-1 pb-1 w-25">' + dt + '</td>';

											data += '</tr>';
										});
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
				}
			]
		});

		function getFormRules() {
			dataTable.ajax.reload();
		}

		function loadPartialView(Id) {
			var FolderId = Id !== null ? Id.toString() : '00000000-0000-0000-0000-000000000000';
			recordsFolders.init(FolderId);
			recordsFolders.dataTable.ajax.reload(function () { abp.ui.clearBusy($("body")); });
		}

		loadPartialView(null);

		function getRecords() {
			recordsFolders.dataTable.ajax.reload();
		}

		function deleteRecord(data) {
			abp.message.confirm(
				'',
				app.localize('AreYouSure'),
				function (isConfirmed) {
					if (isConfirmed) {
						_recordsService.delete({
							id: data.id
						}).done(function () {
							getRecords();
							abp.notify.success(app.localize('SuccessfullyDeletedRecord'));
						});
					}
				}
			);
		}

		function deleteRecordMatter(data) {
			abp.message.confirm(
				'',
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
	});
})();