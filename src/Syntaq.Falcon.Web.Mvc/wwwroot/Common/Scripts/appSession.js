var app = app || {};
(function () {
  abp.services.app.session.getCurrentLoginInformations({ async: true }).done(function (result) {
    app.session = result;
  });
})();
