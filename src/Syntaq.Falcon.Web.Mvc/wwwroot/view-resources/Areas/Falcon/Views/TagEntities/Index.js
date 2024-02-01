﻿(function () {
    $(function () {

        var _$tagEntitiesTable = $('#TagEntitiesTable');
        var _tagEntitiesService = abp.services.app.tagEntities;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.TagEntities.Create'),
            edit: abp.auth.hasPermission('Pages.TagEntities.Edit'),
            'delete': abp.auth.hasPermission('Pages.TagEntities.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
                    viewUrl: abp.appPath + 'Falcon/TagEntities/CreateOrEditModal',
                    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntities/_CreateOrEditModal.js',
                    modalClass: 'CreateOrEditTagEntityModal'
                });
                   

		 var _viewTagEntityModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntities/ViewtagEntityModal',
            modalClass: 'ViewTagEntityModal'
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

        var dataTable = _$tagEntitiesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tagEntitiesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#TagEntitiesTableFilter').val(),
					entityIdFilter: $('#EntityIdFilterId').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					tagValueValueFilter: $('#TagValueValueFilterId').val()
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
                                    _viewTagEntityModal.open({ id: data.record.tagEntity.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.tagEntity.id });                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteTagEntity(data.record.tagEntity);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "tagEntity.entityId",
						 name: "entityId"   
					},
					{
						targets: 3,
						 data: "tagEntity.entityType",
						 name: "entityType"   ,
						render: function (entityType) {
							return app.localize('Enum_EntityType_' + entityType);
						}
			
					},
					{
						targets: 4,
						 data: "tagValueValue" ,
						 name: "tagValueFk.value" 
					}
            ]
        });

        function getTagEntities() {
            dataTable.ajax.reload();
        }

        function deleteTagEntity(tagEntity) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagEntitiesService.delete({
                            id: tagEntity.id
                        }).done(function () {
                            getTagEntities(true);
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

        $('#CreateNewTagEntityButton').click(function () {
            _createOrEditModal.open();
        });        

		

        abp.event.on('app.createOrEditTagEntityModalSaved', function () {
            getTagEntities();
        });

		$('#GetTagEntitiesButton').click(function (e) {
            e.preventDefault();
            getTagEntities();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getTagEntities();
		  }
		});
		
		
		
    });
})();