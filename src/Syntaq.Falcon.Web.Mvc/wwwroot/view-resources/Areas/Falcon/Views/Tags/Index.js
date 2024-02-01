(function () {
    $(function () {

        var _$tagsTable = $('#TagsTable');
        var _tagsService = abp.services.app.tags;
        var _tagEntityTypesService = abp.services.app.tagEntityTypes;
        var _tagValuesService = abp.services.app.tagValues;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Tags.Create'),
            edit: abp.auth.hasPermission('Pages.Tags.Edit'),
            'delete': abp.auth.hasPermission('Pages.Tags.Delete'),
            // TO DO: hasPermission ==>> editValue
            editValue: abp.auth.hasPermission('Pages.TagValues.Edit'),
            editEntity: abp.auth.hasPermission('Pages.TagEntityTypes.Edit'),
            'deleteEntity': abp.auth.hasPermission('Pages.TagEntityTypes.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Tags/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Tags/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditTagModal'
        });



        // add edit value modal here
        var _valueCreateOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagValues/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagValues/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditTagValueModal'
        });

        // edit entity
        var _entityCreateOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntityTypes/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntityTypes/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditTagEntityTypeModal'
        });


        var _viewTagModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Tags/ViewtagModal',
            modalClass: 'ViewTagModal'
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

        var dataTable = _$tagsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            createdRow: function (row, data, dataIndex) {

                $(row).find("td:nth-child(0)").on("click", function () {
                    debugger;
                    alert('');
                    //_createOrEditModal.open({ id: data.tag.id });
                });
            },
            listAction: {
                ajaxFunction: _tagsService.getAll,
                inputFilter: function () {
                    return {
                        filter: $('#TagsTableFilter').val(),
                        nameFilter: $('#NameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0
                },
                {
                    width: 120,
                    targets: 2,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {


                        text: app.localize('Actions'),

                        items: [

                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.tag.id });
                                }
                            },
                            /* {
                                 text: app.localize('EditEntity'),
                                 iconStyle: 'far fa-edit mr-2',
                                 visible: function () {
                                     return _permissions.editEntity;
                                 },
                                 action: function (data) {
                                     _entityCreateOrEditModal.open({ id: data.record.tag.id, name: data.record.tag.name });
                                 }
                             },
                             {
                                 text: app.localize('DeleteEntity'),
                                 iconStyle: 'far fa-edit mr-2',
                                 visible: function () {
                                     return _permissions.deleteEntity;
                                 },
                                 action: function (data) {
                                     deleteTagEntityType(data.record.tag);
                                 }
                             },*/

                            {
                                text: app.localize('EditValue'),
                                visible: function () {
                                    return _permissions.editValue;
                                },
                                action: function (data) {
                                    _valueCreateOrEditModal.open({ id: data.record.tag.id, name: data.record.tag.name });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteTag(data.record.tag);
                                }
                            }]
                    }
                },
                {
                    targets: 1,
                    data: "tag.name",
                    name: "name",
                    render: function (data, type, row) {

                        return `<img class="stq-primary-icon me-2" title="App" src="/common/images/primaryicons/tags.png"></i> ${data}`;
                    }
                },
                /* {
                 targets: 3,
                 defaultContent: 'Project',

             },*/
            ]
        });

        function getTags() {
            dataTable.ajax.reload();
        }

        function deleteTagEntityType(tag) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagEntityTypesService.delete({
                            id: tag.id
                        }).done(function () {
                            /*getTagEntityTypes(true);*/
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }


        function deleteTag(tag) {

            $.ajax({
                type: "POST",
                url: "/Tags/PassTagHasValue",
                data: { "id": tag.id },
                success: function (response) {
                    abp.notify.success(app.localize('SuccessfullyDeleted'));

                },
                failure: function () {
                    abp.notify.success(app.localize('Error'));
                }
            });

            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagsService.delete({
                            id: tag.id
                        }).done(function () {
                            getTags(true);
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

        $('#CreateNewTagButton').click(function () {
            _createOrEditModal.open();
        });



        abp.event.on('app.createOrEditTagModalSaved', function () {
            getTags();
        });

        $('#GetTagsButton').click(function (e) {
            e.preventDefault();
            getTags();
        });

        $('#clearbtn').click(function () {
            $('#TagsTableFilter').val('');
            getTags();
        });
        $("#TagsTableFilter").keyup(function (event) {
            // On enter 
            if (event.keyCode === 13) {
                getTags();
            }
        });
    });
})();
