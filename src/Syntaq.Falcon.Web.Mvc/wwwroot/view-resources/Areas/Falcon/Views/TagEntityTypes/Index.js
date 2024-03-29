﻿(function () {
    $(function () {

        var _$tagEntityTypesTable = $('#TagEntityTypesTable');
        var _tagEntityTypesService = abp.services.app.tagEntityTypes;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.TagEntityTypes.Create'),
            edit: abp.auth.hasPermission('Pages.TagEntityTypes.Edit'),
            'delete': abp.auth.hasPermission('Pages.TagEntityTypes.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
                    viewUrl: abp.appPath + 'Falcon/TagEntityTypes/CreateOrEditModal',
                    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntityTypes/_CreateOrEditModal.js',
                    modalClass: 'CreateOrEditTagEntityTypeModal'
                });
                   

		 var _viewTagEntityTypeModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntityTypes/ViewtagEntityTypeModal',
            modalClass: 'ViewTagEntityTypeModal'
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

        var dataTable = _$tagEntityTypesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tagEntityTypesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#TagEntityTypesTableFilter').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					tagNameFilter: $('#TagNameFilterId').val()
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
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text:  app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
						{
                                text: app.localize('View'),
                                iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _viewTagEntityTypeModal.open({ id: data.record.tagEntityType.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.tagEntityType.id });                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteTagEntityType(data.record.tagEntityType);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "tagEntityType.entityType",
						 name: "entityType"   ,
						render: function (entityType) {
							return app.localize('Enum_EntityType_' + entityType);
						}
			
					},
					{
						targets: 3,
						 data: "tagName" ,
						 name: "tagFk.name" 
					}
            ]
        });

        function getTagEntityTypes() {
            dataTable.ajax.reload();
        }

        function deleteTagEntityType(tagEntityType) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagEntityTypesService.delete({
                            id: tagEntityType.id
                        }).done(function () {
                            getTagEntityTypes(true);
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

        $('#CreateNewTagEntityTypeButton').click(function () {
            _createOrEditModal.open();
        });        

		

        abp.event.on('app.createOrEditTagEntityTypeModalSaved', function () {
            getTagEntityTypes();
        });

		$('#GetTagEntityTypesButton').click(function (e) {
            e.preventDefault();
            getTagEntityTypes();
        });

        $('#clearbtn').click(function () {
            $('#TagEntityTypesTableFilter').val('');
            getTagEntityTypes();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getTagEntityTypes();
		  }
		});
		
		
		
    });
})();
