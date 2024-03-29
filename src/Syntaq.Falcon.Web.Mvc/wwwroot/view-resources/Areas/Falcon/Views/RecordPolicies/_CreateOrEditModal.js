﻿(function ($) {
    app.modals.CreateOrEditRecordPolicyModal = function () {

        var _recordPoliciesService = abp.services.app.recordPolicies;
        var _recordPolicyActionsService = abp.services.app.recordPolicyActions;


        var _modalManager;
        var _$recordPolicyInformationForm = null;

        var _$ruleTable = $('#RecordDeleteRuleTable');

        var _createOrEditRulesModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/recordPolicyActions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordPolicyActions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditrecordPolicyActionModal'
        });

        var dataTable = _$ruleTable.DataTable({
            paging: false,
            serverSide: true,
            processing: true,
            "searching": true,
            ordering: false,
            createdRow: function (row, data, dataIndex) {
                $(row).find("#editRulesButton").on("click", function () {
                    _createOrEditRulesModal.open({ id: data.recordPolicyAction.id, RecordPolicyId: data.recordPolicyAction.recordPolicyId, AppliedTenantId: data.recordPolicyAction.appliedTenantId });
                });
            },
            listAction: {
                ajaxFunction: _recordPolicyActionsService.getAllByRecordId,
                inputFilter: function () {
                    var id = $('#RecordPolicyId').val() === '' ? '00000000-0000-0000-0000-000000000000' : $('#RecordPolicyId').val();
                    return {
                        recordPolicyId: id
                    };

                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "recordPolicyAction.name",
                    name: "name"
                },
                {
                    targets: 1,
                    data: "recordPolicyAction.expireDays",
                    name: "expireDays"
                },
                {
                    targets: 2,
                    data: "recordPolicyAction.type",
                    name: "type",
                    render: function (type) {
                        return app.localize('Enum_RecordPolicyActionType_' + type);
                    }
                },
                {
                    targets: 3,
                    data: "recordPolicyAction.recordStatus",
                    name: "recordStatus",
                    render: function (recordStatus) {
                        return app.localize('Enum_RecordStatusType_' + recordStatus);
                    }
                },
                {
                    targets: 4,
                    data: "recordPolicyAction.active",
                    name: "active",
                    render: function (active) {
                        if (active) {
                            return '<div class="text-center"><i class="fa fa-check kt--font-success" title="True"></i></div>';
                        }
                        return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                    }

                },
                {
                    responsivePriority: 1,
                    targets: 5,
                    data: null,
                    render: function (data, type, row) {
                        data = "<div class='pull-right'><button id='editRulesButton'  style='pointer:default' class='btn btn-secondary btn-sm' type='button'>" + app.localize('Edit') + "</button ></div>";
                        return data;
                    }

                }
            ]
        });


        abp.event.on('app.createOrEditRecordPolicyActionModalSaved', function () {
            getRules();
        });


        function getRules() {
            dataTable.ajax.reload();
        }


        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });



            _$recordPolicyInformationForm = _modalManager.getModal().find('#kt_wizard_form_step_2_form');
            _$recordPolicyInformationForm.validate();

            $("#create-rule").on('click', function (e) {
                e.preventDefault();
                if ($('#RecordPolicyId').val() !== "") {
                    _createOrEditRulesModal.open({ id: null, RecordPolicyId: $('#RecordPolicyId').val(), AppliedTenantId: $('#AppliedTenantId').val() }, function (data) {

                    });
                }
            });

        };

        this.save = function () {

            if (!_$recordPolicyInformationForm.valid()) {
                return;
            }
            var recordPolicy = _$recordPolicyInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _recordPoliciesService.createOrEdit(
                recordPolicy
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditRecordPolicyModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });

        };

 

    };
})(jQuery);