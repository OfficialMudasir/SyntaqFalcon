/// <reference path="../users/_userslookuptablemodal.js" />
(function () {
    $(function () {

        abp.ui.setBusy($("body"));

        var _$appsTable = $('#AppsTable');
        var _appsService = abp.services.app.apps;
        var _appJobsService = abp.services.app.appJobs;

        // Saving AppData

        _$appDataForm = $('form[name=AppDataForm]');
        _$appDataForm.validate();

        var _$btnSaveAppdata = $('#btn-SaveAppData');
        _$btnSaveAppdata.click(function () {

            if (!_$appDataForm.valid()) {
                return;
            }

            var app = _$appDataForm.serializeFormToObject();

            //this.setBusy(true);
            _appsService.createOrEdit(
                app
            ).done(function () {
                abp.notify.info(abp.localization.localize('SavedSuccessfully'));
                //_modalManager.close();
                abp.event.trigger('app.createOrEditAppModalSaved');
                $('#appJobModal').modal('hide');
            }).always(function () {
                // _modalManager.setBusy(false);
            });

        });

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Apps.Create'),
            edit: abp.auth.hasPermission('Pages.Apps.Edit'),
            'delete': abp.auth.hasPermission('Pages.Apps.Delete')
        };

        var _uploadFormRulesSchemaModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Forms/UploadFormRulesSchemaModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_UploadFormRulesSchemaModal.js',
            modalClass: 'UploadFormRulesSchemaViewModel'
        });


        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Apps/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Apps/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditAppModal'
        });

        var _createOrEditJobModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AppJobs/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AppJobs/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditAppJobModal'
        });

        var _manageACLModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
            modalClass: 'ManageACLModal'
        });

        var _viewAppModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Apps/ViewappModal',
            modalClass: 'ViewAppModal'
        });

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() === null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        };

        var dataTable = _$appsTable.DataTable({
            paging: true,
            serverSide: true,
            autoWidth: false,
            //processing: true,
            initComplete: function (settings, json) {
                abp.ui.clearBusy($("body"));
            },
            createdRow: function (row, data, dataIndex) {
                $(row).find("[name='RunAppLink']").on("click", function () {
                    runApp(data.app);
                });

                $(row).find("[name='ShareAppLink']").on("click", function () {
                    _manageACLModal.open({ entityid: data.app.id, entityname: data.app.name, entitytype: 'App' });
                });

                $(row).find("[name='CopyAppIdLink']").on("click", function () {


                    var clipboard = new ClipboardJS("[name='CopyAppIdLink']");


                });

                $(row).find("[name='EditAppLink']").on("click", function () {
                    selectedApp = { Id: data.app.id, Name: data.app.name };

                    $('#SelectedAppJobRightTitle').text(data);

                    appjobs.load();

                    _appsService.getAppForEdit({
                        id: data.app.id
                    }).done(function (data) {
                        $('#AppId').val(data.app.id);
                        $('#AppName').val(data.app.name);
                        $('#AppDescription').val(data.app.description);
                        $('#AppData').val(data.app.data);
                    });
                });

                $(row).find("[name='DeleteAppLink']").on("click", function () {
                    deleteApp(data.app);
                });

                $(row).find("[name='UploadFormRulesSchemaLink']").on("click", function () {
                    _uploadFormRulesSchemaModal.open({ Id: data.app.id });
                });

            },
            listAction: {
                ajaxFunction: _appsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#AppsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val(),
                        descriptionFilter: $('#DescriptionFilterId').val()
                    };
                }
            },
            columnDefs: [

                {
                    targets: 0,
                    data: "app.name",
                    render: function (data, type, row) {
                        data = '<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/app2.png"  ></i>';
                        data += row.app.name;
                        data += '<div style = "word-wrap: break-word;" >' + row.app.description + '</div>';
                        return data;
                    }

                },
                //{
                //targets: 1,
                //data: "app.description",
                //},
                {
                    targets: 1,
                    data: "lastModified",
                    orderable: false,
                    width: '230px',
                    render: function (data, type, row) {
                        var dt = new Date(row.app.lastModified);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                        return '<span  >' + dt + '</span>';
                    }
                },
                {
                    targets: 2,
                    responsivePriority: 1,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    width: '7em',
                    rowAction: {
                        text: app.localize('Actions'),
                        class: 'btn btn-secondary',
                        items: [
                            {
                                text: 'Edit App',
                                visible: function (data, type, row) {
                                    return true;
                                    // Below code somehow makes it in-visible
                                    // return data.record.app.userACLPermission === "V" || data.record.app.userACLPermission === "E" || data.record.app.userACLPermission === "O" ? true : false; 
                                },
                                action: function (data, type, row) {

                                    // TODO REFACTOR

                                    selectedApp = { Id: data.record.app.id, Name: data.record.app.name };

                                    appjobs.load();

                                    $('#appJobModal').modal('show');

                                    _appsService.getAppForEdit({
                                        id: data.record.app.id
                                    }).done(function (data) {

                                        $('#AppId').val(data.app.id);
                                        $('#AppName').val(data.app.name);
                                        $('#AppDescription').val(data.app.description);
                                        $('#AppData').val(data.app.data);
                                    });

                                }
                            },
                            {
                                text: 'Run App',
                                visible: function (data, type, row) {
                                    return true;
                                    // Below code somehow makes it in-visible
                                    // return data.record.app.userACLPermission === "V" || data.record.app.userACLPermission === "E" || data.record.app.userACLPermission === "O" ? true : false; 
                                },
                                action: function (data, type, row) {
                                    runApp(data.record.app);
                                }
                            },
                            {
                                text: 'Share',
                                visible: function (data, type, row) {
                                    return data.record.app.userACLPermission === "O" ? true : false;
                                },
                                action: function (data, type, row) {
                                    _manageACLModal.open({ entityid: data.record.app.id, entityname: data.record.app.name, entitytype: 'App' });
                                }
                            },
                            {
                                text: 'Copy App ID',
                                visible: function (data, type, row) {
                                    return data.record.app.userACLPermission === "E" || data.record.app.userACLPermission === "O" ? true : false;
                                },
                                action: function (data, type, row) {
                                    copytext(data.record.app.id);
                                }
                            },
                            {
                                text: 'Import Rules Schema',
                                visible: function (data, type, row) {
                                    return data.record.app.userACLPermission === "E" || data.record.app.userACLPermission === "O" ? true : false;
                                },
                                action: function (data, type, row) {
                                    _uploadFormRulesSchemaModal.open({ Id: data.record.app.id });
                                }
                            },
                            {
                                text: 'Delete',
                                visible: function (data, type, row) {
                                    return data.record.app.userACLPermission === "E" || data.record.app.userACLPermission === "O" ? true : false;
                                },
                                action: function (data, type, row) {
                                    deleteApp(data.record.app);
                                }
                            }
                        ]
                    }
                }
            ]
        });

        var selectedApp = {
            Id: null,
            Name: null,
            Data: null,
            set: function (app) {
                if (!app) {
                    //organizationTree.selectedOu.id = null;
                    //organizationTree.selectedOu.displayName = null;
                    //organizationTree.selectedOu.code = null;
                } else {
                    //organizationTree.selectedOu.id = ouInTree.id;
                    //organizationTree.selectedOu.displayName = ouInTree.original.displayName;
                    //organizationTree.selectedOu.code = ouInTree.original.code;
                }
                //appjobs.load();
            }
        };

        // TODO REFACTOR
        function setAppsRowClick() {
            $('#AppsTable').on('click', 'tbody tr td', function (event) {
                if (event.target.nodeName == "TD" || event.target.nodeName == "STRONG" || event.target.nodeName == "IMG" || event.target.nodeName == "SPAN" || event.target.nodeName == "DIV") {
                    var data = dataTable.row(this).data();
                    selectedApp = { Id: data.app.id, Name: data.app.name };

                    appjobs.load();

                    $('#appJobModal').modal('show');

                    _appsService.getAppForEdit({
                        id: data.app.id
                    }).done(function (data) {
                        $('#AppId').val(data.app.id);
                        $('#AppName').val(data.app.name);
                        $('#AppDescription').val(data.app.description);
                        $('#AppData').val(data.app.data);
                    });

                }
            })
        }

        setAppsRowClick();

        function setAppJobsRowClick() {
            $('#JobsTable').off('click.jobsRowClick');
            $('#JobsTable').on('click.jobsRowClick', 'tbody tr', function (event) { /*});*/
                //$('#JobsTable').on('click', 'tbody tr', function (event) {
                if (event.target.nodeName == "TD" || event.target.nodeName == "STRONG") {
                    var data = appjobs.dataTable.row(this).data();
                    //selectedApp = { Id: data.app.id, Name: data.app.name };
                    $('#appJobModal').modal('hide');
                    _createOrEditJobModal.open({ id: data.appJob.id });

                }
            })
        }
        //setAppJobsRowClick();

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

            abp.notify.success("", "Copied App Id", {
                "positionClass": "toast-bottom-right"
            })

        };

        function copyToClipboard(text, el) {
            var copyTest = document.queryCommandSupported('copy');
            var elOriginalText = el.attr('data-original-title');

            if (copyTest === true) {

                var copyTextArea = document.createElement("textarea");

                $('#textarea-content').append(copyTextArea);

                $(copyTextArea).text(text);
                copyTextArea.select();

                try {

                    var successful = document.execCommand('copy');
                    var msg = successful ? 'Copied!' : 'Whoops, not copied!';

                    el.tooltip();
                    el.attr('data-original-title', msg).tooltip('option', 'show');
                    //el.attr('data-original-title', msg).tooltip('show');

                } catch (err) {
                    console.log('Oops, unable to copy');
                }

                //$('#textarea-content').remove(copyTextArea);

                $(copyTextArea).remove();

                el.attr('data-original-title', elOriginalText);
            } else {
                // Fallback if browser doesn't support .execCommand('copy')
                window.prompt("Copy to clipboard: Ctrl+C or Command+C, Enter", data);
            }
        }

        function getApps() {
            dataTable.ajax.reload(function () { setAppsRowClick(); });
        }

        function runApp(app) {
            _appsService.run({
                //"App": {
                //    "Id": app.id,
                //    "DataURL": "",
                //    "data": ""
                //}	
                "Id": app.id,
                "DataURL": "",
                "data": "" //  example { "Chairman_Casting_cho": "is not", "Chairman_Casting_cho_MText": "is not" }
            }).done(function () {
                //getApps();
                abp.notify.success('App Run. Check Submission response for the status of this run.');
            });
        }

        function deleteApp(app) {
            abp.message.confirm(
                '',
                '',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _appsService.delete({
                            id: app.id
                        }).done(function () {
                            getApps();
                            abp.notify.success(abp.localization.localize('SuccessfullyDeleted'));
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

        $('#CreateNewAppButton').click(function () {
            $('#appJobModal').modal('hide');
            _createOrEditModal.open();
        });

        $('#AddJobToAppButton').click(function () {
            $('#appJobModal').modal('hide');
            _createOrEditJobModal.open({ id: '00000000-0000-0000-0000-000000000000', Appid: selectedApp.Id });
        });

        abp.event.on('app.createOrEditAppModalSaved', function () {
            getApps();
        });

        abp.event.on('app.createOrEditAppJobModal', function () {
            appjobs.dataTable.ajax.reload(function () { setAppJobsRowClick() });
        });

        $('#GetAppsButton').click(function (e) {
            e.preventDefault();
            getApps();
        });

        $('#clearbtn').click(function () {
            $('#AppsTableFilter').val('');
            getApps();
        });

        $("#AppsTableFilter").keyup(function (event) {
            // On enter 
            if (event.keyCode === 13) {
                getApps();
            }
        });

        /*        $(document).keypress(function (e) {
                    if (e.which === 13) {
                        getApps();
                    }
                });*/

        var appjobs = {
            $table: $('#AppJobsTable'),
            $emptyInfo: $('#AppJobsEmptyInfo'),
            $AddJobToAppButton: $('#AddJobToAppButton'),
            $selectedAppJobRightTitle: $('#SelectedAppJobRightTitle'),
            dataTable: null,

            showTable: function () {
                appjobs.$emptyInfo.hide();
                appjobs.$table.show();
                appjobs.$AddJobToAppButton.show();
                appjobs.$selectedAppJobRightTitle.text(selectedApp.Name).show();
            },

            hideTable: function () {
                appjobs.$selectedAppJobRightTitle.hide();
                appjobs.$AddJobToAppButton.hide();
                appjobs.$table.hide();
                appjobs.$emptyInfo.show();
            },

            load: function () {
                if (!selectedApp.Id) {
                    appjobs.hideTable();
                    return;
                }

                appjobs.showTable();
                this.dataTable.ajax.reload(function () { setAppJobsRowClick() });
            },

            add: function (users) {
                var Id = selectedApp.Id;
                if (!Id) {
                    return;
                }
            },

            remove: function (job) {
            },

            init: function () {
                this.dataTable = appjobs.$table.find("#JobsTable").DataTable({
                    paging: true,
                    serverSide: true,
                    autoWidth: false,
                    //processing: true,
                    deferLoading: 0, //prevents table for ajax request on initialize
                    createdRow: function (row, data, dataIndex) {
                        $(row).find("[name='EditAppJobLink']").on("click", function () {
                            $('#appJobModal').modal('hide');
                            _createOrEditJobModal.open({ id: data.appJob.id });
                        });
                        $(row).find("[name='DeleteAppJobLink']").on("click", function () {
                            deleteAppJob(data.appJob);
                        });
                    },
                    listAction: {
                        ajaxFunction: _appJobsService.getJobsByAppId,
                        inputFilter: function () {
                            return { id: selectedApp.Id };
                        }
                    },
                    columnDefs: [
                        {
                            targets: 0,
                            data: "appJob.name",
                            width: "400px",
                            render: function (data, type, row) {
                                data = '<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/cog.png"></i>' + data;
                                return data;
                            }
                        },
                        {
                            targets: 1,
                            width: "80px",
                            mRender: function (data, type, row) {

                                data = '<div class="btn-group"><button class="btn btn-sm btn-secondary btn-active-light-primary dropdown-toggle" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions</button>'
                                data += '<ul class="dropdown-menu" style = "" >'
                                data += '   <li>'
                                data += '       <a class="dropdown-item" href="javascript:;" name="EditAppJobLink">Edit</a>'
                                data += '   </li>'
                                data += '   <li>'
                                data += '       <a class="dropdown-item" href="javascript:;" name="DeleteAppJobLink">Delete</a>'
                                data += '   </li>'
                                data += '</ul>'
                                data += '</div > ';

                                return data;
                            }
                        }
                    ]
                });

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

                function getAppJobs() {
                    appjobs.dataTable.ajax.reload(function () { setAppJobsRowClick() });
                }

                $('#AddUserToOuButton').click(function (e) {
                    e.preventDefault();
                    appjobs.openAddModal();
                });

                appjobs.hideTable();
            }
        };
        appjobs.init();
    });
})();