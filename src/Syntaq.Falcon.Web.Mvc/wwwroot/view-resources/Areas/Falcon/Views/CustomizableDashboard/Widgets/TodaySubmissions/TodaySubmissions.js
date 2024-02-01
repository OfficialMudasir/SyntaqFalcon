$(function () {
    var _tenantDashboardService = abp.services.app.tenantDashboard;

    var _$container = $('.TodaySubmissionsContainer');

    var initDashboardTodaySubmissions = function (YesterdaysSubmissions, TodaysSubmissions) {
        _$container.find(".todaysSubmissions").text(TodaysSubmissions);
        var ChangeSymbol = YesterdaysSubmissions > TodaysSubmissions ? '-' : '+';
        var changePercent = 0;
        if (YesterdaysSubmissions == 0 && TodaysSubmissions == 0) {
            ChangeSymbol = '';
        }
        else {
            if (YesterdaysSubmissions == 0) {
                YesterdaysSubmissions = 1;
                changePercent = Math.round((Math.abs(TodaysSubmissions) / 1) * 100);
            }
            else {
                changePercent = Math.round((Math.abs(YesterdaysSubmissions - TodaysSubmissions) / YesterdaysSubmissions) * 100);
            }
        }

        _$container.find('#todaySubmissionPercentage').css('width', '' + changePercent + '%');
        _$container.find(".submissionChangePercent").text(ChangeSymbol + changePercent);
        _$container.find(".counterup").counterUp();
    };

    var getTodaySubmissionsData = function () {
        _tenantDashboardService
            .getTodaySubmissions({
                yesterdaysStartDate: moment().subtract(1, "days").format("YYYY-MM-DDT00:00:00Z"),
                yesterdaysEndDate: moment().subtract(1, "days").format("YYYY-MM-DDT23:59:59.999Z"),
                todaysStartDate: moment().format("YYYY-MM-DDT00:00:00Z"),
                todaysEndDate: moment().format("YYYY-MM-DDT23:59:59.999Z")
            })
            .done(function (result) {
                initDashboardTodaySubmissions(result.yesterdaysSubmissions, result.todaysSubmissions);
            });
    };

    getTodaySubmissionsData();
});