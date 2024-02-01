$(function () {
    var _tenantDashboardService = abp.services.app.tenantDashboard;
    var _selectedDateRange = {
        startDate: moment().startOf('month'),
        endDate: moment().endOf('day')
    };
    
    var _$container = $('.SubmissionLimitContainer');

    var initDashboardSubmissionLimit = function (submissionLimit, currentSubmissions, submissionUsagePercent) {
        _$container.find("#submissionLimit").text(submissionLimit);
        _$container.find('#submissionPercentage').css('width', '' + submissionUsagePercent + '%');
        _$container.find("#currentOfLimit").text(`${currentSubmissions} of ${submissionLimit}`);
        _$container.find(".counterup").text(submissionUsagePercent);
        _$container.find(".counterup").counterUp();
    };

    var getSubmissionLimitData = function () {
        _tenantDashboardService
            .getSubmissionLimit({
                startDate: _selectedDateRange.startDate.format("YYYY-MM-DDT00:00:00Z"),
                endDate: _selectedDateRange.endDate.format("YYYY-MM-DDT23:59:59.999Z"),
            })
            .done(function (result) {
                initDashboardSubmissionLimit(result.submissionLimit, result.currentSubmissions, result.submissionUsagePercent);
            });
    };

    getSubmissionLimitData();
});