﻿$(function () {
    var _tenantDashboardService = abp.services.app.tenantDashboard;

    var _selectedDateRange = {
        startDate: moment().startOf('month'),
        endDate: moment().endOf('day')
    };


    var _$container = $('.NewUsersContainer');

    var initDashboardTodaySubmissions = function (todaySubmissions) {
        _$container.find("#newUsers").text(todaySubmissions);
    };

    var getNewUsersData = function () {
        _tenantDashboardService
            .getNewUsersForWidget({
                startDate: _selectedDateRange.startDate.format("YYYY-MM-DDT00:00:00Z"),
                endDate: _selectedDateRange.endDate.format("YYYY-MM-DDT23:59:59.999Z"),
            })
            .done(function (result) {
                initDashboardTodaySubmissions(result.newUsers);
            });
    };

    getNewUsersData();
});