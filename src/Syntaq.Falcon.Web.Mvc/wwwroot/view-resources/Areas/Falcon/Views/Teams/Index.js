//(function () {
//    $(function () {

//        //var _$teamsTable = $('#TeamsTable');
//        var _organizationUnitService = abp.services.app.organizationUnit;
//        var _teamsService = abp.services.app.teams;
//        var _formsService = abp.services.app.forms;

//        $('.date-picker').datetimepicker({
//            locale: abp.localization.currentLanguage.name,
//            format: 'L'
//        });

//        var _permissions = {
//            create: abp.auth.hasPermission('Pages.Teams.Create'),
//            edit: abp.auth.hasPermission('Pages.Teams.Edit'),
//            'delete': abp.auth.hasPermission('Pages.Teams.Delete')
//        };

//        var _createOrEditModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/Teams/CreateOrEditModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Teams/_CreateOrEditModal.js',
//            modalClass: 'CreateOrEditTeamModal'
//        });

//        var _createOrEditFormsModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/Forms/CreateOrEditModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_CreateOrEditModal.js',
//            modalClass: 'CreateOrEditFormModal'
//        });

//        //var _AssignOrRemoveRolesModal = new app.ModalManager({
//        //    viewUrl: abp.appPath + 'Falcon/Teams/AssignOrRemoveRolesModal',
//        //    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Teams/_AssignOrRemoveRolesModal.js',
//        //    modalClass: 'AssignOrRemoveRolesModal'
//        //});

//        var _assignOrRemoveRolesModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/Teams/AssignOrRemoveRolesModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Teams/_AssignOrRemoveRolesModal.js',
//            modalClass: 'AssignOrRemoveRolesModal'
//        });

//        var teamsMembers = {
//            $table: null,
//            $emptyInfo: null,
//            dataTable: null,

//            setViewObjects: function () {
//                this.$table = $('#TeamsMembersTable');
//                this.$emptyInfo = $('#TeamMembersEmptyInfo');
//            },

//            showTable: function () {
//                teamsMembers.$emptyInfo.hide();
//                teamsMembers.$table.show();
//                //members.$addUserToOuButton.show();
//                //members.$selectedOuRightTitle.text(': ' + organizationTree.selectedOu.displayName).show();
//            },

//            hideTable: function () {
//                //members.$selectedOuRightTitle.hide();
//                //members.$addUserToOuButton.hide();
//                teamsMembers.$table.hide();
//                teamsMembers.$emptyInfo.show();
//            },

//            load: function () {
//                //if (!organizationTree.selectedOu.id) {
//                //    members.hideTable();
//                //    return;
//                //}

//                this.showTable();
//                this.dataTable.ajax.reload();
//            },

//            init: function () {
//                this.dataTable = teamsMembers.$table.find("#MembersTable").DataTable({
//                    paging: true,
//                    serverSide: true,
//                    processing: true,
//                    deferLoading: 0, //prevents table for ajax request on initialize
//                    listAction: {
//                        ajaxFunction: _organizationUnitService.getOrganizationUnitUsers,
//                        inputFilter: function () {
//                            return { id: $('#TeamSelectionCombo').val() }
//                        }
//                    },
//                    columnDefs: [
//                        {
//                            targets: 0,
//                            data: null,
//                            orderable: false,
//                            autoWidth: false,
//                            defaultContent: '',
//                            rowAction: {
//                                cssClass: 'btn btn-brand dropdown-toggle',
//                                text:  app.localize('Actions') + ' <span class="caret"></span>',
//                                items: [
//                                    {
//                                        text: app.localize('ManageRoles'),
//                                        action: function (data) {
//                                            _assignOrRemoveRolesModal.open({ userid: data.record.id, orgid: data.record.organizationUnitId });
//                                        }
//                                    },
//                                    {
//                                        text: app.localize('Edit'),
//                                        visible: function () {
//                                            return _permissions.edit;
//                                        },
//                                        action: function (data) {
//                                            _createOrEditModal.open({ id: data.record.form.id });
//                                        }
//                                    },
//                                    {
//                                        text: app.localize('Delete'),
//                                        visible: function () {
//                                            return _permissions.delete;
//                                        },
//                                        action: function (data) {
//                                            deleteForm(data.record.form);
//                                        }
//                                    }]
//                            }
//                        },
//                        {
//                            targets: 1,
//                            data: "userName"
//                        },
//                        {
//                            targets: 2,
//                            data: "addedTime",
//                            render: function (addedTime) {
//                                return moment(addedTime).format('L');
//                            }
//                        }
//                    ]
//                });


//                //$('#AddUserToOuButton').click(function (e) {
//                //    e.preventDefault();
//                //    members.openAddModal();
//                //});

//                teamsMembers.hideTable();
//            }
//        }

//        var teamsForms = {
//            $table: null,
//            $emptyInfo: null,
//            dataTable: null,

//            setViewObjects: function () {
//                this.$table = $('#TeamFormsTable');
//                this.$emptyInfo = $('#TeamFormsEmptyInfo');
//            },

//            showTable: function () {
//                teamsForms.$emptyInfo.hide();
//                teamsForms.$table.show();
//                //members.$addUserToOuButton.show();
//                //members.$selectedOuRightTitle.text(': ' + organizationTree.selectedOu.displayName).show();
//            },

//            hideTable: function () {
//                //members.$selectedOuRightTitle.hide();
//                //members.$addUserToOuButton.hide();
//                teamsForms.$table.hide();
//                teamsForms.$emptyInfo.show();
//            },

//            load: function () {
//                //if (!organizationTree.selectedOu.id) {
//                //    members.hideTable();
//                //    return;
//                //}

//                teamsForms.showTable();
//                this.dataTable.ajax.reload();
//            },

//            openAddModal: function () {
//                //var ouId = organizationTree.selectedOu.id;
//                //if (!ouId) {
//                //    return;
//                //}

//                _createOrEditFormsModal.open(
//                    //{
//                    //    title: app.localize('SelectAUser')
//                    //}
//                        //,
//                //    organizationUnitId: ouId
//                //}, function (selectedItems) {
//                //    members.add(selectedItems);
//                //    }
//                );
//            },

//            init: function () {
//                this.dataTable = teamsForms.$table.find("#FormsTable").DataTable({
//                    paging: true,
//                    serverSide: true,
//                    processing: true,
//                    deferLoading: 0, //prevents table for ajax request on initialize
//                    listAction: {
//                        ajaxFunction: _formsService.getAll,
//                        inputFilter: function () {
//                            return {
//                                filter: $('#FormsTableFilter').val(),
//                                organizationUnitId: $('#TeamSelectionCombo').val(),
//                                //organizationUnitId: 3,
//                                descriptionFilter: $('#DescriptionFilterId').val(),
//                                docPDFFilter: $('#DocPDFFilterId').val(),
//                                docUserCanSaveFilter: $('#DocUserCanSaveFilterId').val(),
//                                docWordFilter: $('#DocWordFilterId').val(),
//                                docWordPaidFilter: $('#DocWordPaidFilterId').val(),
//                                nameFilter: $('#NameFilterId').val(),
//                                minPaymentAmountFilter: $('#MinPaymentAmountFilterId').val(),
//                                maxPaymentAmountFilter: $('#MaxPaymentAmountFilterId').val(),
//                                paymentCurrFilter: $('#PaymentCurrFilterId').val(),
//                                paymentEnabledFilter: $('#PaymentEnabledFilterId').val(),
//                                formsFolderNameFilter: $('#FormsFolderNameFilterId').val()
//                            };
//                        }
//                    },
//                    columnDefs: [
//                        {
//                            width: 120,
//                            targets: 0,
//                            data: null,
//                            orderable: false,
//                            autoWidth: false,
//                            defaultContent: '',
//                            rowAction: {
//                                cssClass: 'btn btn-brand dropdown-toggle',
//                                text:  app.localize('Actions') + ' <span class="caret"></span>',
//                                items: [
//                                    {
//                                        text: app.localize('View'),
//                                        action: function (data) {
//                                            _viewFormModal.open({ data: data.record });
//                                        }
//                                    },
//                                    {
//                                        text: app.localize('Edit'),
//                                        visible: function () {
//                                            return _permissions.edit;
//                                        },
//                                        action: function (data) {
//                                            _createOrEditModal.open({ id: data.record.form.id });
//                                        }
//                                    },
//                                    {
//                                        text: app.localize('Delete'),
//                                        visible: function () {
//                                            return _permissions.delete;
//                                        },
//                                        action: function (data) {
//                                            deleteForm(data.record.form);
//                                        }
//                                    }]
//                            }
//                        },
//                        {
//                            targets: 1,
//                            data: "form.description"
//                        },
//                        {
//                            targets: 2,
//                            data: "form.docPDF",
//                            render: function (docPDF) {
//                                if (docPDF) {
//                                    return '<div class="text-center"><i class="fa fa-check-circle m--font-success" title="True"></i></div>';
//                                }
//                                return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
//                            }

//                        },
//                        {
//                            targets: 3,
//                            data: "form.docUserCanSave",
//                            render: function (docUserCanSave) {
//                                if (docUserCanSave) {
//                                    return '<div class="text-center"><i class="fa fa-check-circle m--font-success" title="True"></i></div>';
//                                }
//                                return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
//                            }

//                        },
//                        {
//                            targets: 4,
//                            data: "form.docWord",
//                            render: function (docWord) {
//                                if (docWord) {
//                                    return '<div class="text-center"><i class="fa fa-check-circle m--font-success" title="True"></i></div>';
//                                }
//                                return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
//                            }

//                        },
//                        {
//                            targets: 5,
//                            data: "form.docWordPaid",
//                            render: function (docWordPaid) {
//                                if (docWordPaid) {
//                                    return '<div class="text-center"><i class="fa fa-check-circle m--font-success" title="True"></i></div>';
//                                }
//                                return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
//                            }

//                        },
//                        {
//                            targets: 6,
//                            data: "form.name"
//                        },
//                        {
//                            targets: 7,
//                            data: "form.paymentAmount"
//                        },
//                        {
//                            targets: 8,
//                            data: "form.paymentCurr"
//                        },
//                        {
//                            targets: 9,
//                            data: "form.paymentEnabled",
//                            render: function (paymentEnabled) {
//                                if (paymentEnabled) {
//                                    return '<div class="text-center"><i class="fa fa-check-circle m--font-success" title="True"></i></div>';
//                                }
//                                return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
//                            }

//                        },
//                        {
//                            targets: 10,
//                            data: "formsFolderName"
//                        }
//                    ]
//                });

//                $('#CreateNewFormButton').click(function (e) {
//                    e.preventDefault();
//                    teamsForms.openAddModal();
//                });

//                //teamsForms.hideTable();
//            }
//        }

//        function getTeamForms() {
//            teamsForms.showTable();
//            teamsForms.dataTable.ajax.reload();
//        }

//        function getTeamMembers() {
//            teamsMembers.showTable();
//            teamsMembers.dataTable.ajax.reload();
//        }

//        $(_createOrEditFormsModal).on('hidden.bs.modal', function () {
//            teamsForms.dataTable.ajax.reload();
//        })

//        function ShowFilters(set) {
//            $('#' + set +'ShowAdvancedFiltersSpan').hide();
//            $('#' + set +'HideAdvancedFiltersSpan').show();
//            $('#' + set +'AdvacedAuditFiltersArea').slideDown();
//        }

//        function HideFilters(set) {
//            $('#' + set +'HideAdvancedFiltersSpan').hide();
//            $('#' + set +'ShowAdvancedFiltersSpan').show();
//            $('#' + set +'AdvacedAuditFiltersArea').slideUp();
//        }

//        $('#TeamSelectionCombo').change(function (e) {
//            //e.preventDefault();
//            //var test = $(this).val();
//            var TeamId = parseInt($(this).val());
//            if ($(this).val() !== "") {
//                //alert("I have a value of " + $(this).val() + "!");
//                $.ajax({
//                    url: '/Falcon/Teams/TeamsPartial',
//                    type: 'POST',
//                    data: { Id: TeamId },
//                    dataType: 'html'
//                }).done(function (partialViewResult) {
//                    $(".m-content").html(partialViewResult);
//                    teamsForms.setViewObjects();
//                    teamsMembers.setViewObjects();
//                    teamsForms.init();
//                    teamsMembers.init();
//                    getTeamForms();
//                    getTeamMembers();
//                });
//            }
//        });

//        $('#FormsShowAdvancedFiltersSpan').click(function () {
//            ShowFilters('Forms');
//        });

//        $('#FormsHideAdvancedFiltersSpan').click(function () {
//            HideFilters('Forms');
//        });

//        $('#CreateNewTeamButton').click(function (e) {
//            e.preventDefault();
//            _createOrEditModal.open();
//        });

//        abp.event.on('app.createOrEditTeamModalSaved', function () {
//            //getTeams();
//            window.location.reload();
//        });

//     //   var dataTable = _$teamsTable.DataTable({
//     //       paging: true,
//     //       serverSide: true,
//     //       processing: true,
//     //       listAction: {
//     //           ajaxFunction: _teamsService.getAll,
//     //           inputFilter: function () {
//     //               return {
//					//filter: $('#TeamsTableFilter').val(),
//					//teamNameFilter: $('#TeamNameFilterId').val()
//     //               };
//     //           }
//     //       },
//     //       columnDefs: [
//     //           {
//     //               width: 120,
//     //               targets: 0,
//     //               data: null,
//     //               orderable: false,
//     //               autoWidth: false,
//     //               defaultContent: '',
//     //               rowAction: {
//     //                   cssClass: 'btn btn-brand dropdown-toggle',
//     //                   text:  app.localize('Actions') + ' <span class="caret"></span>',
//     //                   items: [
//					//	{
//     //                       text: app.localize('Edit'),
//     //                       visible: function () {
//     //                           return _permissions.edit;
//     //                       },
//     //                       action: function (data) {
//     //                           _createOrEditModal.open({ id: data.record.team.id });
//     //                       }
//     //                   }, 
//					//	{
//     //                       text: app.localize('Delete'),
//     //                       visible: function () {
//     //                           return _permissions.delete;
//     //                       },
//     //                       action: function (data) {
//     //                           deleteTeam(data.record.team);
//     //                       }
//     //                   }]
//     //               }
//     //           },
//					//{
//					//	targets: 1,
//     //                       //data: "team.teamName"
//     //                       data: "team.displayName" 
//					//}
//     //       ]
//     //   });


//        //function getTeams() {
//        //    dataTable.ajax.reload();
//        //}

//     //   function deleteTeam(team) {
//     //       abp.message.confirm(
//     //           '',
//     //           function (isConfirmed) {
//     //               if (isConfirmed) {
//     //                   _teamsService.delete({
//     //                       id: team.id
//     //                   }).done(function () {
//     //                       getTeams(true);
//     //                       abp.notify.success(app.localize('SuccessfullyDeleted'));
//     //                   });
//     //               }
//     //           }
//     //       );
//     //   }		



//		//$('#GetTeamsButton').click(function (e) {
//  //          e.preventDefault();
//  //          getTeams();
//  //      });

//		//$(document).keypress(function(e) {
//		//  if(e.which === 13) {
//		//	getTeams();
//		//  }
//		//});

//    });
//})();