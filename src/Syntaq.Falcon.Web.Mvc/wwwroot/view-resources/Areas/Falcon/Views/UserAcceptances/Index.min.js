(function () {
    $(function () {

        var _$userAcceptancesTable = $('#UserAcceptancesTable');
        var _userAcceptancesService = abp.services.app.userAcceptances;

        $('.date-picker').datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.UserAcceptances.Create'),
            edit: abp.auth.hasPermission('Pages.UserAcceptances.Edit'),
            'delete': abp.auth.hasPermission('Pages.UserAcceptances.Delete')
        };

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var dataTable = _$userAcceptancesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(2),td:nth-child(3),td:nth-child(4),td:nth-child(5),td:nth-child(6)").on("click", function () {
                    ;
                    $(row).find(':checkbox').each(function () {
                        this.checked = !this.checked;
                    });

                });
            },
            listAction: {
                ajaxFunction: _userAcceptancesService.getAllExcludeContributors,
                inputFilter: function () {
                    return {
                        filter: $('#UserAcceptancesTableFilter').val(),
                        tenantNameFilter: $('#tenantNameFilter').val(),
                        userNameFilter: $('#userNameFilter').val(),
                        userFirstNameFilter: $('#userFirstNameFilter').val(),
                        userSurnameFilter: $('#userSurnameFilter').val(),
                        userEmailFilter: $('#userEmailFilter').val(),
                        userAcceptanceTypeNameFilter: $('#userAcceptanceTypeNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    className: 'selectedChk',
                    targets: 0,
                    orderable: false,
                    render: function (data, type, row) {
                        data = "<div class='text-center'><input type='checkbox'  name='selectedChk'/> <input type='hidden' name='Id' novalidate value='" + row.userAcceptance.id + "'/></div>"

                        //data += '<label for="toggle" class="form-check form-check-custom form-check-solid col-lg-12"><input id="Id" type="checkbox" name="toggle" class="form-check-input" value="'+ row.userAcceptance.id +'" checked="checked"><span class="form-check-label"> </span></label>';

                        return data;
                    }
                },
                {
                    targets: 1,
                    render: function (data, type, row) {
                        return row.tenantName;
                    }
                },
                {
                    targets: 2,
                    render: function (data, type, row) {
                        return row.userName;
                    }
                },
                {
                    targets: 3,
                    render: function (data, type, row) {
                        return row.userFirstName + ' ' + row.userSurname;
                    }
                },
                {
                    targets: 4,
                    data: "userEmailAddress",
                    name: "userFk.emailAddress"
                },
                {
                    targets: 5,
                    data: "userAcceptanceTypeName",
                    name: "userAcceptanceTypeFk.name"
                },
                {
                    targets: 6,
                    data: "userAcceptance.creationTime",
                    render: function (time) {

                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }
                }
            ]
        });


        function getUserAcceptances() {
            dataTable.ajax.reload();
        }

        $('#chkToggle').click(function () {
            var checked = this.checked;
            $("[name='selectedChk']").attr('checked', checked);
        });


        $('#deleteSelectedAcceptedUsers').click(function () {

            var checkedLength = $('input[name="selectedChk"]:checked').length;
            if (checkedLength < 1) {
                alert(app.localize("NoneSelected"));
                return;
            } else {
                delSelectedAcceptedUsersAcceptances(checkedLength);
            }
        });

        function delSelectedAcceptedUsersAcceptances(checkedLength) {

            abp.message.confirm(
                app.localize("ClearedUsersMustReacceptActivePolicies"),
                '',
                function (isConfirmed) {
                    if (isConfirmed) {
                        var data = []; cnt = 0;
                        _$userAcceptancesTable.DataTable().cells(".selectedChk").every(function (index) {
                            var _$cellCheckBox = $(this.node()).children().children('input[name="selectedChk"]');
                            if (_$cellCheckBox.is(":checked")) {
                                var Id = _$cellCheckBox.siblings('input[name="Id"]').val();
                                data[cnt] = Id; cnt++;
                            }
                        });

                        _userAcceptancesService.deleteSelected(data).done(function () {
                            abp.notify.success(app.localize('SuccessfullyCleared'));
                            dataTable.ajax.reload();
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

        var _deleteFilteredUserAcceptanceModal = new app.ModalManager({
            viewUrl: abp.appPath + "Falcon/UserAcceptances/DeleteUserAcceptanceModal",
            scriptUrl: abp.appPath + "view-resources/Areas/Falcon/Views/UserAcceptances/_DeleteUserAcceptanceModal.js",
            modalClass: "DeleteUserAcceptanceModal"
        });

        $('#DeleteFilteredUserAcceptance').click(function () {
            _deleteFilteredUserAcceptanceModal.open();
        });

        $('#ClearAllUserAcceptances').click(function () {
            clearAllUserAcceptances();
        });

        $('#ExportToExcelButton').click(function () {
            _userAcceptancesService
                .getUserAcceptancesToExcel({
                    filter: $('#UserAcceptancesTableFilter').val(),
                    acceptedFilter: $('#AcceptedFilterId').val(),
                    userAcceptanceTypeNameFilter: $('#UserAcceptanceTypeNameFilterId').val(),
                    userNameFilter: $('#UserNameFilterId').val(),
                    recordMatterContributorNameFilter: $('#RecordMatterContributorNameFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditUserAcceptanceModalSaved', function () {
            getUserAcceptances();
        });

        $('#GetUserAcceptancesButton').click(function (e) {
            e.preventDefault();
            getUserAcceptances();
        });

        $('#clear').click(function (e) {
            e.preventDefault();
            $('#UserAcceptancesTableFilter').clear();
            getUserAcceptances();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getUserAcceptances();
            }
        });

    });
})();