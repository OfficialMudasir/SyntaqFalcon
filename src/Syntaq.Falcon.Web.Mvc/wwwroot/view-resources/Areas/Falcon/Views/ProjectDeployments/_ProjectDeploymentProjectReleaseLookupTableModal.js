(function ($) {
  app.modals.ProjectReleaseLookupTableModal = function () {
    var _modalManager;

    var _projectDeploymentsService = abp.services.app.projectDeployments;
    var _$projectReleaseTable = $('#ProjectReleaseTable');

    this.init = function (modalManager) {
      _modalManager = modalManager;
    };

    var dataTable = _$projectReleaseTable.DataTable({
      paging: true,
      serverSide: true,
      processing: true,
      listAction: {
        ajaxFunction: _projectDeploymentsService.getAllProjectReleaseForLookupTable,
        inputFilter: function () {
          return {
            filter: $('#ProjectReleaseTableFilter').val(),
          };
        },
      },
      columnDefs: [
        {
          targets: 0,
          data: null,
          orderable: false,
          autoWidth: false,
          defaultContent:
            "<div class=\"text-center\"><input id='selectbtn' class='btn btn-success' type='button' width='25px' value='" +
            app.localize('Select') +
            "' /></div>",
        },
        {
          autoWidth: false,
          orderable: false,
          targets: 1,
          data: 'displayName',
        },
      ],
    });

    $('#ProjectReleaseTable tbody').on('click', '[id*=selectbtn]', function () {
      var data = dataTable.row($(this).parents('tr')).data();
      _modalManager.setResult(data);
      _modalManager.close();
    });

    function getProjectRelease() {
      dataTable.ajax.reload();
    }

    $('#GetProjectReleaseButton').click(function (e) {
      e.preventDefault();
      getProjectRelease();
    });

    $('#SelectButton').click(function (e) {
      e.preventDefault();
    });

    $(document).keypress(function (e) {
      if (e.which === 13 && e.target.tagName.toLocaleLowerCase() != 'textarea') {
        getProjectRelease();
      }
    });
  };
})(jQuery);
