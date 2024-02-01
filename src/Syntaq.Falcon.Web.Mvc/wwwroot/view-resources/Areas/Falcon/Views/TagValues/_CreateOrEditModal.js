﻿(function ($) {
    app.modals.CreateOrEditTagValueModal = function () {

        var _tagValuesService = abp.services.app.tagValues;

        var _modalManager;
        var _table = $('#TagValuesTable');
        var _permissions = {
            edit: abp.auth.hasPermission('Pages.TagValues.Edit'),
            'delete': abp.auth.hasPermission('Pages.TagValues.Delete')
        };
  
        var modal;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });
            _$tagValueInformationForm = _modalManager.getModal().find('form[name=TagValueInformationsForm]');
        };

        var dataTable = _table.DataTable({
                paging: false,
                serverSide: true,
                processing: true,
                autoWidth: false,
                listAction: {
                    ajaxFunction: _tagValuesService.getAll,
                    inputFilter: function () {
                        return {
                            id: _modalManager.getArgs().id,
                        };
                    }
                },
                columnDefs: [
                    {
                        targets: 0,
                        orderable: false,
                        data: "tagValue.value",
                    },
                    {
                        targets: 1,
                        data: null,
 
                        orderable: false,
                        defaultContent: '<input type="button" id = "editTagValue" class="btn btn-secondary btn-sm" value="Edit"/><input type="button" id = "deleteTagValue" class="btn btn-sm btn-danger pull-right" value="Delete"/>',
                    }
                ]
        });

        $('#TagValuesTable tbody').on('click', '#editTagValue', function () {
            var row = $(this).closest('tr');
            var data = dataTable.row(row).data();
            if (_permissions.edit) {
                createOrEditValue(data.tagValue.id, data.tagValue.value);
            }
        });

        $('#TagValuesTable tbody').on('click', '#deleteTagValue', function () {
            var row = $(this).closest('tr');
            var data = dataTable.row(row).data();
            if (_permissions.delete) {
                deleteTagValue(data.tagValue);
            }
        });

        function deleteTagValue(tagValue) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagValuesService.delete({
                            id: tagValue.id
                        }).done(function () {
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                            reloadValues();
                        });
                    }
                }
            );
        }

        $('#addTagValues').click(function () {
            createOrEditValue();
        });

        $('#modealFooterCloseButton').click(function () {
            reloadValues();
        });

        function reloadValues() {
            dataTable.ajax.reload();
        }


        function createOrEditValue(id, value) {
            $('#TagValue_Id').val(id);
            $('#TagValue_Value').val(value);
            $('#TagValue_TagId').val(_modalManager.getArgs().id);
            
            $("#edit-values-area").show("fast");
        }   

        this.save = function () {
            if (!_$tagValueInformationForm.valid()) {
                return;
            }
            var tagValue = _$tagValueInformationForm.serializeFormToObject();
            _modalManager.setBusy(true);
            _tagValuesService.createOrEdit(
                tagValue
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                $("#edit-values-area").hide("fast");
                reloadValues();
            }).always(function () {
                _modalManager.setBusy(false);

            });
        };
    };
})(jQuery);