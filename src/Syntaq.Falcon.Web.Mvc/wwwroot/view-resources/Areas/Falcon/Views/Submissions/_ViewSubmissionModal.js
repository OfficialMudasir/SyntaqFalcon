﻿(function () {
	$(function () {
		DisplayPayments();
		SubmissionStatusIcon();

		$('body').on('show.bs.modal', '.modal', function () {
			DisplayPayments();
			SubmissionStatusIcon();
		});

		function DisplayPayments() {
			var RequiresPayment = $('#RequiresPayment').val();
			if (RequiresPayment == 0) {
				$('#PaymentCard').hide();
			} else {
				$('#PaymentCard').show();
				var _Currency = CurrencySymbol($('#PaymentCurrency').val());
				$("[name='Currency']").html(_Currency);
			}
		}

		function SubmissionStatusIcon() {
			if ($('#SubmissionStatus').val() == "Error") {
				$('#Submission_Success').hide();
				$('#Submission_Exception').show();
			} else if ($('#SubmissionStatus').val() == "Complete") {
				$('#Submission_Success').show();
				$('#Submission_Exception').hide();
			} else {
				$('#Submission_Success').hide();
				$('#Submission_Exception').hide();
			}
		}

		function CurrencySymbol(Currency) {
			switch (Currency) {
				case "USD":
				default:
					return "US$";
				case "AUD":
					return "A$";
				case "BRL":
					return "R$";
				case "CAD":
					return "C$";
				case "CHF":
					return "CHF";
				case "CZK":
					return "Kč";
				case "DKK":
					return "kr";
				case "EUR":
					return "€";
				case "GBP":
					return "£";
				case "HKD":
					return "HK$";
				case "HUF":
					return "Ft";
				case "ILS":
					return "₪";
				case "JPY":
					return "¥";
				case "MXN":
					return "Mex$";
				case "MYR":
					return "RM";
				case "NOK":
					return "kr";
				case "NZD":
					return "NZ$";
				case "PHP":
					return "₱";
				case "PLN":
					return "zł";
				case "SEK":
					return "kr";
				case "SGD":
					return "S$";
				case "THB":
					return "฿";
				case "TWD":
					return "NT$";
			}
		}
	});
})();