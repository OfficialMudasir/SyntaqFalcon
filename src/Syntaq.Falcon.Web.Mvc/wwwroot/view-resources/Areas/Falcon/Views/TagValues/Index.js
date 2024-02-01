(function () {
    $(function () {

        var _$tagValuesTable = $('#TagValuesTable');
        var _tagValuesService = abp.services.app.tagValues;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.TagValues.Create'),
            edit: abp.auth.hasPermission('Pages.TagValues.Edit'),
            'delete': abp.auth.hasPermission('Pages.TagValues.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
                    viewUrl: abp.appPath + 'Falcon/TagValues/CreateOrEditModal',
                    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagValues/_CreateOrEditModal.js',
                    modalClass: 'CreateOrEditTagValueModal'
                });
                   

		 var _viewTagValueModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagValues/ViewtagValueModal',
            modalClass: 'ViewTagValueModal'
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

        var dataTable = _$tagValuesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tagValuesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#TagValuesTableFilter').val(),
					valueFilter: $('#ValueFilterId').val(),
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
                                    _viewTagValueModal.open({ id: data.record.tagValue.id });
                                }
                        },
						{
                            text: app.localize('AddTagValue'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.tagValue.id });                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteTagValue(data.record.tagValue);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "tagValue.value",
						 name: "value"   
					},
					{
						targets: 3,
						 data: "tagName" ,
						 name: "tagFk.name" 
					}
            ]
        });

        function getTagValues() {
            dataTable.ajax.reload();
        }

        function deleteTagValue(tagValue) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tagValuesService.delete({
                            id: tagValue.id
                        }).done(function () {
                            getTagValues(true);
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

        $('#CreateNewTagValueButton').click(function () {
            _createOrEditModal.open();
        });        

		

        abp.event.on('app.createOrEditTagValueModalSaved', function () {
            getTagValues();
        });

		$('#GetTagValuesButton').click(function (e) {
            e.preventDefault();
            getTagValues();
        });

        $('#clearbtn').click(function () {
            $('#TagValuesTableFilter').val('');
            getTagValues();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getTagValues();
		  }
		});
		
		
		
    });
})();
