﻿(function () {
    $(function () {

        var _$asicTable = $('#AsicTable');
        var _asicService = abp.services.app.asic;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Asic.Create'),
            edit: abp.auth.hasPermission('Pages.Asic.Edit'),
            'delete': abp.auth.hasPermission('Pages.Asic.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Asic/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Asic/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditAsicModal'
        });


        var _viewAsicModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Asic/ViewasicModal',
            modalClass: 'ViewAsicModal'
        });

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z");
        }

        var getMaxDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT23:59:59Z");
        }
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
                getAsic();
            });
        var dataTable = _$asicTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _asicService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#AsicTableFilter').val(),
                        startDateFilter: _selectedDateRangeAuditLog.startDate,
                        endDateFilter: _selectedDateRangeAuditLog.endDate,
                        hTTPRequestsFilter: $('#HTTPRequestsFilterId').val(),
                        requestMethodFilter: $('#RequestMethodFilterId').val(),
                        responseFilter: $('#ResponseFilterId').val(),
                        tenantNameFilter: $('#TenantNameFilter').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "asic.requestId",
                    name: "requestId"
                },
                {
                    targets: 1,
                    data: "userName",
                    name: "User"
                },
                {
                    targets: 2,
                    data: "companyName",
                    name: "companyName"
                },
                {
                    targets: 3,
                    data: "asic.status",
                    name: "Status"
                },
                {
                    targets: 4,
                    data: "tenantName",
                    name: "tenantName"
                },
                {
                    targets: 5,
                    data: "asic.lastModificationTime",
                    name: "creationTime",
                    render: function (time) {
                        var dt = new Date(time);
                        var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                        var tmoptions = { hour: 'numeric', minute: 'numeric' };
                        return dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                    }
                },
            ]
        });

        function finaliseAsicRequest(asic) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                
                        _asicService.finaliseRequest(
                            asic.id
                        ).done(function () {
                            getAsic(true);
                            abp.notify.success(app.localize('SuccessfullyFinalised'));
                        });
                    }
                }
            );




        }


        function getAsic() {
            dataTable.ajax.reload();
        }

        function deleteAsic(asic) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _asicService.delete({
                            id: asic.id
                        }).done(function () {
                            getAsic(true);
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

        $('#CreateNewAsicButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportToExcelButton').click(function () {
            _asicService
                .getAsicToExcel({
                    filter: $('#AsicTableFilter').val(),
                    hTTPRequestsFilter: $('#HTTPRequestsFilterId').val(),
                    requestMethodFilter: $('#RequestMethodFilterId').val(),
                    responseFilter: $('#ResponseFilterId').val()
                })
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditAsicModalSaved', function () {
            getAsic();
        });

        $('#GetAsicButton').click(function (e) {
            e.preventDefault();
            getAsic();
        });
        $('#clearbtn').click(function () {
            $('#AsicTableFilter').val('');
            getAsic();
        });
         $("#AsicTableFilter").keyup(function (event) {
            // On enter 
            if (event.keyCode === 13) {
                getAsic();
            }
        });
    });
})();
