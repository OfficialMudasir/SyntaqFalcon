(function ($) {
  app.modals.ProjectEnvironmentLookupTableModal = function () {
    var _modalManager;

    var _projectTenantsService = abp.services.app.projectTenants;
    var _$projectEnvironmentTable = $('#ProjectEnvironmentTable');

    this.init = function (modalManager) {
      _modalManager = modalManager;
    };

    var dataTable = _$projectEnvironmentTable.DataTable({
      paging: true,
      serverSide: true,
      processing: true,
      listAction: {
        ajaxFunction: _projectTenantsService.getAllProjectEnvironmentForLookupTable,
        inputFilter: function () {
          return {
            filter: $('#ProjectEnvironmentTableFilter').val(),
          };
        },
      },
      columnDefs: [
        {
          targets: 1,
          data: null,
          orderable: false,
          autoWidth: false,
          defaultContent:
            "<div class=\"text-center\"><input id='selectbtn' class='btn btn-secondary' type='button' width='25px' value='" +
            app.localize('Select') +
            "' /></div>",
        },
        {
          autoWidth: false,
          orderable: false,
          targets: 0,
          data: 'displayName',
        },
      ],
    });

    $('#ProjectEnvironmentTable tbody').on('click', '[id*=selectbtn]', function () {
      var data = dataTable.row($(this).parents('tr')).data();
      _modalManager.setResult(data);
      _modalManager.close();
    });

    function getProjectEnvironment() {
      dataTable.ajax.reload();
    }

    $('#GetProjectEnvironmentButton').click(function (e) {
      e.preventDefault();
      getProjectEnvironment();
    });

    $('#SelectButton').click(function (e) {
      e.preventDefault();
    });

    $(document).keypress(function (e) {
      if (e.which === 13 && e.target.tagName.toLocaleLowerCase() != 'textarea') {
        getProjectEnvironment();
      }
    });
  };
})(jQuery);
