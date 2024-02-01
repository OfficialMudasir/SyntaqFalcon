//(function () {
//	$(function () {
//		var _formsService = abp.services.app.forms;
//		//var _recordsService = abp.services.app.records;
//		var _recordMattersService = abp.services.app.recordMatters;

//		//var _documentsService = abp.services.app.documents;
//		//var _appsService = abp.services.app.apps;
//		//var _appJobsService = abp.services.app.appJobs;

//		var _recordId = JSONRMObj.RecordId !== null ? JSONRMObj.RecordId : uuidv4();
//		var _recordMatterId = JSONRMObj.Id !== null ? JSONRMObj.Id : uuidv4();
//		var _recordMatterItemId = JSONRMIObj.Id !== null ? JSONRMIObj.Id : uuidv4();
//		var _submissionId = JSONRMIObj.SubmissionId !== null ? JSONRMIObj.SubmissionId : uuidv4();
//		abp.ui.setBusy('body');

//		// Add to loader script
//		function LoopThroughQueryString(JSONData) {

//			//loops through queryString
//			var queryString = unescape(location.search);

//			//if no querystring (homepage etc.) then exit
//			if (!queryString) {
//				return {};
//			}

//			//remove the ?
//			queryString = queryString.substring(1);

//			//split querystring into key/value pairs
//			var pairs = queryString.split("&");

//			//load the pairs into a collection
//			for (var i = 0; i < pairs.length; i++) {
//				//split into key/value pair by splitting on =
//				var keyValuePair = pairs[i].split("=");

//				if (keyValuePair.length > 1) {
//					JSONData[keyValuePair[0]] = keyValuePair[1];
//				}
//			}
//			return JSONData;
//		}

//		_formsService.getSchema({
//			id: JSONFormObj.Id
//		},'Load').done(function (result) {
//			loadform(result);            
//		}).always(function () {
//			abp.ui.clearBusy('body');
//		});
	  
//		var loadform = function (form) {

//			if (form === '' || !form) {
//				form = '{"type": "form", "display": "form", "components": [{"key": "notesHelpEditor", "htmlcontent": "This form hasn\'t been designed yet! Open this form in the form builder and add some fields first.", "type": "helpnotes", "input": false, "tableView": false, "label": "Notes/Help editor"}]}';
//			}

//			formJSON = JSON.parse(form);
//			formJSON = UpdateSchema(formJSON);

//			Formio.icons = 'fontawesome';
//			Formio.createForm(document.getElementById('formio'), formJSON, {}).then(function (form)
//			{
//				// load the submited data at the beginning if it does have.
//				if (JSONRMObj.Data) {
//					form.data = JSON.parse(JSONRMObj.Data);
//				}

//				Form = form;
//				//
//				form.on('render', function () {
//					Form.redraw();
//				});
 
//				form.on('sectionBuildRows', function () {
//					if (typeof InitCustomFormScript === "function") {
//						InitCustomFormScript();
//					}
//				});

//				form.on('initialized', function () { 
//					if (typeof InitCustomFormScript === "function") { 
//						InitCustomFormScript();
//					}
//					abp.ui.clearBusy('body');
//				});

//				form.on('change', function (event) {
//					if (event.changed !== 'undefined') {
//						onFormChange(event, form);
//					}                    
//				});

//				// Prevent the submission from going to the form.io server.a
//				form.nosubmit = true;

//				// TODO update Syntaq embed script
//				if (JSONRMObj.Id) {
//					/*var jsondata = */_recordMattersService.getRecordMatterJsonData({
//						id: JSONRMObj.Id
//					}).done(function (result) {

//						// Initialise any functions in custom script
//						onFormChange(null, form);
//						var JSONResult = JSON.parse(result);

//						if (typeof InitFormData === "function") {
//							JSONResult = InitFormData(JSONResult);
//						}

//						JSONResult = LoopThroughQueryString(JSONResult);
//						if (NewFormForRecord) {
//							if (NewFormForRecord.Data) {
//								var recordjsondata = JSON.parse(NewFormForRecord.Data);
//								JSONResult = insertNewFormForRecord(JSONResult, recordjsondata);
//							}
//						}
//						form.submission = {
//							data: JSONResult
//						};
//						Form.redraw();
//						if (typeof AfterFormDataLoad === "function") {
//							AfterFormDataLoad();
//						}

//					});
//				}   
 
//				// Triggered when click the save button.
//				form.on('save', function (submission) {

//					if (typeof BeforeFormSave === 'function') {
//						BeforeFormSave();
//					}

//					var FormId = JSON.stringify(JSONFormObj.Id);
//					var Data = JSON.stringify(submission);
//					var input = '{ "Id":' + FormId + ', "RecordId":"' + _recordID + '", "RecordMatterId":"' + _recordMatterID + '", "RecordMatterItemId":"' + _recordMatterItemID + '", "Submission": ' + Data + '}';

//					_formsService.save(
//						input
//					).done(function (result) {
//						var json = JSON.parse(result);
//						_recordID = json.RId;
//						_recordMatterID = json.RMId;
//						_recordMatterItemID = json.RMIId;
//						_submissionId = json.SubmissionId;

//						if (_recordId !== 0) {
//							abp.notify.info(app.localize('Form saved'));
//						} else {
//							abp.notify.warn(app.localize('Form not saved'));
//						}

//						if (typeof AfterFormSave === 'function') {
//							AfterFormSave();
//						}
//						abp.ui.clearBusy('body');

//					}).always(function () {
//						abp.ui.clearBusy('body');
//					});
//				});
 

//				// Triggered when they click the submit button.
//				form.on('submit', function (submission) {

//					abp.ui.setBusy('body');

//					var Redirect;

//					if (typeof BeforeFormSubmit === 'function') {
//						submission = BeforeFormSubmit(submission);
//					}

//					if (typeof submission.data["sfasystemredirect"] !== "undefined") {
//						if (submission.data["sfasystemredirect"] !== "") {
//							Redirect = submission.data["sfasystemredirect"];
//						}                            
//					}
						
//					var FormId = JSON.stringify(JSONFormObj.Id);              
//					submission.data.RecordId = _recordId;
//					submission.data.RecordMatterId = _recordMatterId;
//					submission.data.RecordMatterItemId = _recordMatterItemId;
//					submission.data.SubmissionId = _submissionId;

//					var Data = JSON.stringify(submission);

//					var input = '{ "id":' + FormId + ', "RecordId": "' + _recordId + '", "RecordMatterId": "' + _recordMatterId + '", "RecordMatterItemId": "' + _recordMatterItemId + '", "SubmissionId":"' + _submissionId + '",  "submission":' + Data + '}';

//					_formsService.run(
//						input
//					).done(function (result) {
//						if (result === true) {
//							abp.notify.info(app.localize('Form Submitted'));
//							if (Redirect) {
//								window.location.replace(Redirect.toString());
//							}
//						} else {
//							abp.notify.warn(app.localize('Form Submission failed'));
//						}
							
//						abp.event.trigger('app.createOrEditRecordModalSaved');

//						if (typeof AfterFormSubmit === 'function') {
//							AfterFormSubmit();
//						}
//						abp.ui.clearBusy('body');
//					}).always(function () {
//						abp.ui.clearBusy('body');
//						$('[type=submit]').removeAttr("disabled");
//						$('[type=submit]').find('i').remove();
//					}); 
//				});
//			});
//		};
//	});
//})();


//function uuidv4() {
//	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
//		var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
//		return v.toString(16);
//	});
//}

//function insertNewFormForRecord(jsondata, recordjsondata) {
//	delete recordjsondata['OriginalId'];
//	delete recordjsondata['RecordMatterId'];
//	delete recordjsondata['RecordMatterItemId'];
//	delete recordjsondata['version'];
//	for (const key in jsondata) {
//		recordjsondata[key] = jsondata[key];
//	}
//	return recordjsondata;
//}