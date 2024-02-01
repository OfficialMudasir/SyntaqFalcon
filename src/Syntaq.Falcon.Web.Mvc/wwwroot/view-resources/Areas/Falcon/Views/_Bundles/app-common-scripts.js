(function () {
  app.modals.LookupModal = function () {
    var _modalManager;
    var _dataTable;
    var _$table;
    var _$filterInput;

    var _options = {
      serviceMethod: null, //Required
      title: app.localize('SelectAnItem'),
      loadOnStartup: true,
      showFilter: true,
      filterText: '',
      excludeCurrentUser: false,
      pageSize: app.consts.grid.defaultPageSize,
      canSelect: function (item) {
        /* This method can return boolean or a promise which returns boolean.
         * A false value is used to prevent selection.
         */
        return true;
      },
    };

    function refreshTable() {
      _dataTable.ajax.reload();
    }

    function selectItem(item) {
      var boolOrPromise = _options.canSelect(item);
      if (!boolOrPromise) {
        return;
      }

      if (boolOrPromise === true) {
        _modalManager.setResult(item);
        _modalManager.close();
        return;
      }

      //assume as promise
      boolOrPromise.then(function (result) {
        if (result) {
          _modalManager.setResult(item);
          _modalManager.close();
        }
      });
    }

    this.init = function (modalManager) {
      _modalManager = modalManager;
      _options = $.extend(_options, _modalManager.getOptions().lookupOptions);
      _$table = _modalManager.getModal().find('.lookup-modal-table');

      _dataTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        processing: true,
        lengthChange: false,
        pageLength: _options.pageSize,
        deferLoading: _options.loadOnStartup ? null : 0,
        listAction: {
          ajaxFunction: _options.serviceMethod,
          inputFilter: function () {
            return $.extend(
              {
                filter: _$filterInput.val(),
                excludeCurrentUser: _options.excludeCurrentUser,
              },
              _modalManager.getArgs().extraFilters
            );
          },
        },
        columnDefs: [
          {
            targets: 0,
            data: null,
            orderable: false,
            defaultContent: '',
            className: 'text-center',
            rowAction: {
              element: $('<button/>')
                    .addClass('btn btn-icon btn-bg-light btn-active-color-primary btn-sm')
                .attr('title', app.localize('Select'))
                .append($('<i/>').addClass('la la-chevron-circle-right'))
                .click(function () {
                  var record = $(this).data();
                  selectItem(record);
                }),
            },
          },
          {
            targets: 1,
            data: 'name',
          },
        ],
      });

      _modalManager
        .getModal()
        .find('.lookup-filter-button')
        .click(function (e) {
          e.preventDefault();
          refreshTable();
        });

      _modalManager
        .getModal()
        .find('.modal-body')
        .keydown(function (e) {
          if (e.which === 13) {
            e.preventDefault();
            refreshTable();
          }
        });

      _$filterInput = _modalManager.getModal().find('.lookup-filter-text');
      _$filterInput.val(_options.filterText);
    };
  };

  app.modals.LookupModal.create = function (lookupOptions) {
    return new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Common/LookupModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Common/Modals/_LookupModal.js',
      modalClass: 'LookupModal',
      lookupOptions: lookupOptions,
    });
  };
})();

(function () {
  app.modals.EntityTypeHistoryModal = function () {
    var _modalManager;
    var _dataTable;
    var _$table;
    var _args;

    var _auditLogService = abp.services.app.auditLog;

    var _options = {
      serviceMethod: null, //Required
      title: app.localize('SelectAnItem'),
      loadOnStartup: true,
      showFilter: true,
      filterText: '',
      pageSize: app.consts.grid.defaultPageSize,
      canSelect: function (item) {
        /* This method can return boolean or a promise which returns boolean.
         * A false value is used to prevent selection.
         */
        return true;
      },
    };

    function refreshTable() {
      _dataTable.ajax.reload();
    }

    function showEntityChangeDetails(entityChange) {
      var entityChangeDetailModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Falcon/AuditLogs/EntityChangeDetailModal',
        modalClass: 'EntityChangeDetailModal',
      });

      entityChangeDetailModal.open({ entityChangeListDto: entityChange });
    }

    this.init = function (modalManager) {
      _modalManager = modalManager;

      _options = $.extend(_options, _modalManager.getOptions().lookupOptions);
      _$table = _modalManager.getModal().find('.entity-type-history-table');
      _args = _modalManager.getArgs();

      _dataTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        processing: true,
        lengthChange: false,
        pageLength: _options.pageSize,
        deferLoading: _options.loadOnStartup ? null : 0,
        listAction: {
          ajaxFunction: _auditLogService.getEntityTypeChanges,
          inputFilter: function () {
            return {
              entityTypeFullName: _args.entityTypeFullName,
              entityId: _args.entityId,
            };
          },
        },
        columnDefs: [
          {
            targets: 0,
            data: null,
            orderable: false,
            defaultContent: '',
            rowAction: {
              element: $('<div/>')
                .addClass('text-center')
                .append(
                  $('<button/>')
                    .addClass('btn btn-outline-primary btn-sm btn-icon')
                    .attr('title', app.localize('EntityChangeDetail'))
                    .append($('<i/>').addClass('la la-search'))
                )
                .click(function () {
                  showEntityChangeDetails($(this).data());
                }),
            },
          },
          {
            targets: 1,
            data: 'changeTypeName',
            orderable: false,
            render: function (changeTypeName) {
              return app.localize(changeTypeName);
            },
          },
          {
            targets: 2,
            data: 'userName',
          },
          {
            targets: 3,
            data: 'changeTime',
            render: function (changeTime) {
              return moment(changeTime).format('L LT');
            },
          },
        ],
      });

      refreshTable();
    };
  };

  app.modals.EntityTypeHistoryModal.create = function () {
    return new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Common/EntityTypeHistoryModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Common/Modals/_EntityTypeHistoryModal.js',
      modalClass: 'EntityTypeHistoryModal',
    });
  };
})();
