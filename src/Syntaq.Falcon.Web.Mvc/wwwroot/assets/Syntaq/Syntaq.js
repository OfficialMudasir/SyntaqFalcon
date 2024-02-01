// JavaScript source code
 
var parser = document.createElement('a');
parser.href = document.getElementById('syntaq-script').src;
var _SyntaqBaseURI = parser.protocol + "//" + parser.host;

loadjscssfile("https://js.stripe.com/v3/", "js");

loadjscssfile(_SyntaqBaseURI + "/assets/jquery-form/jquery.form.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/handlebars/handlebars.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/formio/dist/formio.full.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/toastr/build/toastr.min.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/spin.js/spin.js", "js");
loadjscssfile(_SyntaqBaseURI + "/AbpServiceProxies/GetAll?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/abp-web-resources/Abp/Framework/scripts/abp.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.jquery.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/AbpScripts/GetScripts?v=636936179744097465?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/sweetalert/dist/sweetalert.min.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/formio/dist/formio.full.css", "cssinhead");
loadjscssfile(_SyntaqBaseURI + "/assets/sweetalert/sweetalert.min.js", "js");

loadjscssfile(_SyntaqBaseURI + "/view-resources/Areas/Falcon/Views/_Bundles/datatables-all.min.js?v=" + Math.floor(Math.random() * 10) + "", "js");
 
var Form = { data: {} }; // main form object
var FormPages = {}; // array to track the validity of pages across a wizard

var body = document.getElementById('syntaq-content');

//--------------find user location
window.regionSyntaq = "Unknown";

function getUserPlace() {
    fetch("https://extreme-ip-lookup.com/json/?key=d0bHlIXB8WQ9aNyd1PSB")
        .then((res) => res.json())
        .then((response) => {
            window.regionSyntaq = response.countryCode;
        })
        .catch((data, status) => {
            window.regionSyntaq = "AU";
        });
}
//region just fetch once per user
function checkFlag() {
    if (window.regionSyntaq === "Unknown") {
        getUserPlace();
    }
}
checkFlag();
//----------------------find user location end

var Syntaq = {
    AuthToken: null,
    SetAuthToken: function (token) {
        localStorage.setItem('Syntaq.AuthToken', token);
    },
    GetAuthToken: function () {
        if (Syntaq.AuthToken !== null) {
            Syntaq.SetAuthToken(Syntaq.AuthToken);
        }
        var result = localStorage.getItem('Syntaq.AuthToken');

        // If token has expired logout and log back in
        if (result !== '' && result !== null) {
            var decodedJwt = Syntaq.Account.parseJwt(result);
            if (decodedJwt.exp !== null) {
                if (new Date(decodedJwt.exp * 1000) < Date.now()) {
                    localStorage.removeItem('Syntaq.AuthToken');
                    Syntaq.Account.Login();
                }
            }
        }

        return result;
    },
    SessionToken: null,
    HostBaseURI: '',
    TenantName: '',
    TenantId: null,
    isProject: false,
    CountryCode: 'NZ',
    Busy: {
        spinner: null,
        backgroundcolor: null,
        opts: {
            lines: 12, // The number of lines to draw
            length: 18, // The length of each line
            width: 10, // The line thickness
            radius: 40, // The radius of the inner circle
            scale: 0.85, // Scales overall size of the spinner
            corners: 0.5, // Corner roundness (0..1)
            color: '#ffffff', // CSS color or array of colors
            fadeColor: 'transparent', // CSS color or array of colors
            speed: 1.1, // Rounds per second
            rotate: 0, // The rotation offset
            animation: 'spinner-line-fade-more', // The CSS animation name for the lines
            direction: 1, // 1: clockwise, -1: counterclockwise
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            className: 'spinner', // The CSS class to assign to the spinner
            top: '50%', // Top position relative to parent
            left: '50%', // Left position relative to parent
            shadow: '0 0 1px transparent', // Box-shadow for the lines
            position: 'absolute' // Element positioning
        },
        setBusy: function () {
            var target = document.getElementsByTagName('body')[0];
            Syntaq.Busy.backgroundcolor = target.style.backgroundColor;
            target.style.backgroundColor = "#EEEEEE";
            Syntaq.Busy.spinner = new Spinner(Syntaq.Busy.opts).spin(target);
        },
        clearBusy: function () {
            var target = document.getElementsByTagName('body')[0];
            target.style.opacity = "1";
            target.style.backgroundColor = Syntaq.Busy.backgroundcolor;
            if (Syntaq.Busy.spinner !== undefined && Syntaq.Busy.spinner !== null) {
                Syntaq.Busy.spinner.stop();
            }
        }
    },
    Account: {
        HostEmailActivationURI: '',
        HostPasswordResetURI: '',
        HostPasswordResetReturnURI: '',
        HostRegistrationReturnURI: '',
        EmailActivationElementId: 'syntaq-content',
        LoginElementId: 'syntaq-content',
        LoginForm: null,
        PasswordElementId: 'syntaq-content',
        PasswordResetForm: null,
        RegistrationElementId: 'syntaq-content',
        RegistrationForm: null,
        ActivateEmail: function (callback) {
            var c = getParameterByName('c', window.location);
            var data = JSON.stringify({ c: c });

            var url = _SyntaqBaseURI + '/api/services/app/Account/ActivateEmail';

            jQuery.ajax({
                url: url,
                data: data,
                type: "POST",
                contentType: "application/json",
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                success: function () {
                    callback(response);
                },
                error: function (jqXHR, error, errorThrown) {
                    alert(errorThrown + error);
                },
                complete: function () {

                }
            });

        },
        Logout: function () {
            localStorage.removeItem('Syntaq.AuthToken');
        },
        Login: function (callback) {
            jQuery.ajax({
                url: _SyntaqBaseURI + '/Account/Login?view=/Views/Account/Login.min.cshtml', success: function (data) {
                    var formcontent = document.getElementById(Syntaq.Account.LoginElementId);
                    formcontent.innerHTML = data;
                    LoginForm = jQuery('.m-login__signin');
                    LoginForm.find('input').keypress(function (e) {
                        if (e.which === 13) {
                            //if (jQuery('.login-form').valid()) {
                            Syntaq.Account.submitLogin(callback);
                            //}
                            return false;
                        }
                    });
                    LoginForm.find('button').click(function (e) {
                        //   Syntaq.Account.submitLogin(callback);
                    });
                    LoginForm.find("#register-btn").attr('onclick', "Syntaq.Account.Registration();");
                    LoginForm.find('input[name=returnUrlHash]').val(location.hash);
                    jQuery('input[type=text]').first().focus();
                }
            });

        },
        submitLogin: function (callback) {

            var username = jQuery('[name=usernameOrEmailAddress]').val();
            var password = jQuery('[name=password]').val();
            var data = JSON.stringify({ UserNameOrEmailAddress: username, password: password, TenantName: Syntaq.TenantName });

            var url = _SyntaqBaseURI + '/api/TokenAuth/Authenticate';

            jQuery.ajax({
                url: url,
                data: data,
                type: "POST",
                contentType: "application/json",
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                success: function (response) {
                    if (response.result.userId !== null || response.result.userId !== undefined) {
                        Syntaq.userId = response.result.userId;
                        localStorage.setItem('Syntaq.AuthToken', response.result.accessToken);
                        localStorage.setItem('Syntaq.ExpireInSeconds', response.result.expireInSeconds);
                        localStorage.setItem('Syntaq.RefreshToken', response.result.refreshToken);
                        localStorage.setItem('Syntaq.RefreshTokenExpireInSeconds', response.result.refreshTokenExpireInSeconds);
                        localStorage.setItem('Syntaq.UserId', response.result.userId);
                        callback();
                    }
                },
                error: function (xhr, status, error) {
                    swal({
                        title: "Error",
                        text: 'Invalid Login Details'
                    });
                }
            });

        },
        Registration: function (callback) {
            jQuery.ajax({
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                url: _SyntaqBaseURI + '/Account/Register?view=/Views/Account/Register.min.cshtml',
                success: function (data) {
                    var formcontent = document.getElementById(Syntaq.Account.RegistrationElementId);
                    formcontent.innerHTML = data;
                    Syntaq.Account.RegistrationForm = jQuery('.register-form');
                    Syntaq.Account.RegistrationForm.find('input').keypress(function (e) {
                        if (e.which === 13) {
                            Syntaq.Account.submitRegistration(callback);
                            return false;
                        }
                    });
                    Syntaq.Account.RegistrationForm.find("#register-submit-btn").on("click", function () {
                        Syntaq.Account.submitRegistration(Syntaq.Account.Login());
                    });
                    Syntaq.Account.RegistrationForm.find('input[name=returnUrlHash]').val(location.hash);
                    jQuery('input[type=text]').first().focus();
                }
            });
        },
        submitRegistration: function (callback) {

            var data = JSON.stringify({
                name: document.getElementsByName('Name')[0].value,
                surname: document.getElementsByName('Surname')[0].value,
                userName: document.getElementsByName('UserName')[0].value,
                tenantName: Syntaq.TenantName,
                emailActivationReturnUrl: Syntaq.Account.HostEmailActivationURI,
                emailAddress: document.getElementsByName('EmailAddress')[0].value,
                password: document.getElementsByName('Password')[0].value
            });



            var url = _SyntaqBaseURI + '/api/services/app/Account/Register';

            jQuery.ajax({
                url: url,
                data: data,
                type: "POST",
                contentType: "application/json",
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                success: function () {
                    callback();
                },
                error: function (jqXHR, error, errorThrown) {
                    //alert(errorThrown + error);
                    //STQ Modified
                    alert(errorThrown + jqXHR.responseJSON.error.message);
                },
                complete: function () {
                    // callback();
                }
            });
        },
        forgotPassword: function (callback) {

            jQuery.ajax({
                url: _SyntaqBaseURI + '/Account/ForgotPasswordMin',

                success: function (data) {

                    var formcontent = document.getElementById(Syntaq.Account.PasswordElementId);
                    formcontent.innerHTML = data;
                    Syntaq.Account.PasswordResetForm = jQuery('.forget-form');

                    Syntaq.Account.PasswordResetForm.find('input').keypress(function (e) {
                        if (e.which === 13) {
                            Syntaq.Account.sendPasswordResetCode();
                            return false;
                        }
                    });

                    Syntaq.Account.PasswordResetForm.find("#passwordreset-submit-btn").on("click", function () {
                        Syntaq.Account.sendPasswordResetCode();
                    });

                    jQuery('input[type=text]').first().focus();

                }
            });
        },
        sendPasswordResetCode: function () {

            $("body").css("cursor", "progress");

            var data = JSON.stringify({
                EmailAddress: document.getElementsByName('EmailAddress')[0].value,
                PasswordResetReturnUrl: Syntaq.Account.HostPasswordResetURI
            });

            var url = _SyntaqBaseURI + '/api/services/app/Account/SendPasswordResetCode';
            jQuery.ajax({
                url: url,
                data: data,
                type: "POST",
                contentType: "application/json",
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                success: function () {
                    window.location.replace(Syntaq.Account.HostPasswordResetReturnURI);
                },
                error: function (jqXHR, error, errorThrown) {
                    alert(errorThrown + error);
                },
                complete: function () {
                    window.location.replace(Syntaq.Account.HostPasswordResetReturnURI);
                }
            });

        },
        resetPassword: function (callback) {

            var c = getParameterByName('c', window.location);

            jQuery.ajax({
                url: _SyntaqBaseURI + '/Account/ResetPassword?c=' + c + '&returnUrl=' + Syntaq.Account.HostPasswordResetReturnURI,
                beforeSend: function (request) {
                    request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                },
                success: function (data) {

                    var formcontent = document.getElementById(Syntaq.Account.PasswordElementId);
                    formcontent.innerHTML = data;
                    Syntaq.Account.PasswordResetForm = jQuery('.pass-reset-form');

                    $('[name="ReturnUrl"]').val(Syntaq.Account.HostPasswordResetReturnURI);
                    Syntaq.Account.PasswordResetForm.attr('action', _SyntaqBaseURI + '/Account/ResetPassword');

                    jQuery('input[type=text]').first().focus();

                }
            });

        },
        parseJwt: function (token) {
            var base64Url = token.split('.')[1];
            var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        }
    },
    Project: {
        Id: '',
        Name: '',
        ReleaseId: ''
    },
    Form: {
        SubmittingMessage: 'Form submitting...',
        SubmittedMessage: 'Form submitted',
        ErrorMessage: 'Error',
        SavingMessage: 'Form saving...',
        SavedMessage: 'Form saved',
        DeletedMessage: 'Deleted',
        ArchivedMessage: 'Archived',
        AnonAuthToken: '',
        Anonymous: false,
        ElementId: 'syntaq-content',
        FormId: null,
        FormName: 'Form name not set',
        Version: null,
        HasDocuments: false,
        FormData: null,
        FormType: 'default',
        PopName: null,
        PopKey: null,
        PopRequired: null,
        RecordId: null,
        LoadFromRecord: false,
        RecordMatterId: null,
        RecordMatterItemId: null,
        SubmissionId: null,
        ShowSubmissionDialog: true,
        IsPaid: false,
        Schema: '',
        ShowProgressTicks: true,
        ShowProgressElement: ' <i class="fa fa-check pull-right syntaq-ticks"></i>',
        ShowProgressClass: '',
        SummaryViews: [],
        SummaryView: '',
        ImportantViews: [],
        ImportantView: '',
        SubmitFunction: null,
        SaveFunction: null,
        ProjectId: null,
        ContinueSession: false,
        createForm: function () {  // Method which will display type of animal

            var anonAuthToken = getUrlParameter('AccessToken');
 
            if (anonAuthToken !== '') {
                Syntaq.Form.AnonAuthToken = anonAuthToken;
            }
            
            if (Syntaq.Form.Anonymous === false) {
                if (Syntaq.GetAuthToken() === null || Syntaq.GetAuthToken() === undefined) {
                    Syntaq.Account.Login(this.createForm);
                    return;
                }
            }

            jQuery('#' + Syntaq.Records.ElementId).html('');
            jQuery('#' + Syntaq.Account.ElementId).html('');
            jQuery('#syntaq-records-content_wrapper').remove();

            // START THE FORM SCHEMA XHR             
            var formId = getParameterByName('OriginalId');
            if (Syntaq.Form.FormId === null) {
                Syntaq.Form.FormId = formId;
            }

            if (Syntaq.Form.Version === null || Syntaq.Form.Version === '') {
                Syntaq.Form.Version = getParameterByName('version');
            }

            if (Syntaq.Form.Version === null || Syntaq.Form.Version === '') {
                Syntaq.Form.Version = 'live';
            }

            if (!Syntaq.Form.RecordMatterId) {
                var recordMatterId = getUrlParameter('RecordMatterId');
                Syntaq.Form.RecordMatterId = recordMatterId === '' ? uuidv4() : recordMatterId;
            }
            if (!Syntaq.Form.ProjectId) {
                var ProjectId = getUrlParameter('ProjectId');
                Syntaq.Form.ProjectId = ProjectId === '' ? uuidv4() : ProjectId;
            }

            var AccessToken = getParameterByName('AccessToken');
            var formurl = _SyntaqBaseURI + "/api/services/app/Forms/GetFormForView?ReleaseId=" + Syntaq.Project.ReleaseId + "&ProjectId=" + Syntaq.Project.Id + "&Id=" + Syntaq.Form.FormId + "&OriginalId=" + Syntaq.Form.FormId + "&version=" + Syntaq.Form.Version + "&AccessToken=" + AccessToken + "&RecordMatterId=" + Syntaq.Form.RecordMatterId;
            var recordMatterItemId = getParameterByName('RecordMatterItemId');
            if (Syntaq.Form.RecordMatterItemId === null) {
                Syntaq.Form.RecordMatterItemId = recordMatterItemId;
            }

            if (Syntaq.Form.RecordMatterItemId) {
                formurl += "&RecordMatterItemId=" + Syntaq.Form.RecordMatterItemId;
            }
  
            abp.ajax({
                type: "GET",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                headers: {
                    'wott': 'true'
                },
                contentType: 'application/json',
                url: formurl
            }).done(function (response) {


                var formOBJ = response.form;
                var projectOBJ = response.project;
                $('#FormName').text(formOBJ.name);

                if (projectOBJ !== undefined && projectOBJ !== null) {
                    Syntaq.Project.Id = projectOBJ.id;
                    Syntaq.Project.Name = projectOBJ.name;
                }
                Syntaq.Form.FormName = formOBJ.name;
                Syntaq.Form.FormId = formOBJ.id;
                Syntaq.Form.HasDocuments = response.hasDocuments;

                // Always set the Anonauth token with a value.
                // We will update it if we load an existing recordmatter
                //Syntaq.Form.AnonAuthToken = resultOBJ.result.anonAuthToken;

                // RULES SCRIPT
                if (formOBJ.rulesScript !== null) {
                    var formrulesscript = 'function UpdateSchema(form) {\n\n';
                    formrulesscript += '    formJSON = form;\n\n';
                    formrulesscript += '    instance = form;\n\n';
                    formrulesscript += '    // Need to inject / call the embedded Rules Schema function calls\n\n';

                    formrulesscript += 'if( typeof formJSON !== \'undefined\'){\n\n';

                    formrulesscript += formOBJ.rulesScript + '\n\n';

                    formrulesscript += '}\n\n';

                    formrulesscript += 'return formJSON;\n\n';
                    formrulesscript += '}';

                    loadjscssfile(formrulesscript, "script");
                }

                // CUSTOM SCRIPT
                var script = 'jQuery.fn.Repeat = function (key) {';
                script += '	var component = null;';
                script += '	component = getComponent(this[0], key);';
                script += '	return component;';
                script += '};';
                // test     $('#someid').attr('name', 'value');   $('input[name*="[key]"]').attr('title', key);
                script += 'jQuery.fn.getval = function (key) {';
                script += '	var result = null;';
                script += '	var parent = this.first();';
                script += '	var first = parent[0];';
                script += '	var rptname = \'\';';
                script += '	for (var itm in first) {';
                script += '		rptname = itm;';
                script += '	}';
                script += '	var data = parent[0][rptname].data;';
                script += '	result = data[key];';
                script += '	return result;';
                script += '};';

                script += 'jQuery.fn.setVal = function (key, value) {';
                script += '	var data = this[0].data;';
                script += '	var component = null;';
                script += '	component = getComponent(this[0], key);';
                script += '	if (component !== null && component !== undefined) {';
                script += '		component.setValue(value);';
                script += '	}';
                script += '	if (data !== undefined && data !== null) {';
                script += '		data[key] = value;';
                script += '	}';
                script += '};';

                script += 'function onFormChange(event, form) {';

                script += '	var row = form;';
                script += '	if (event) {';
                script += '		if (event.changed) {';
                script += '			row = event.changed.instance.parent;';
                script += '		}';
                script += '		var submission = { data: event.data };';
                script += '		var data = submission.data;';
                script += '	}\n\n';

                script += 'try {\n\n';
                script += formOBJ.script + "\n\n";


                // Ensure Calendar days are focusable

                // Assuming '.flatpickr-calendar' is a static parent container
                script += '$(".flatpickr-calendar").on("mouseover", ".flatpickr-day", function () {\n\n';
                script += '    $(this).focus();\n\n';
                script += '});\n\n';


                script += '}\n';
                script += 'catch (err) {\n';
                script += ' console.log(err.message);\n';
                script += '}\n\n';


                script += 'instance = event;';
                script += 'if( typeof formJSON !== \'undefined\'){';
                script += formOBJ.rulesScript + "\n\n";
                script += '};';

                script += 'if (typeof _InitRulesScript === "function") {';
                script += '	_InitRulesScript();';
                script += '}\n\n';
                script += '$("#syntaq-content").find("input").change(function(){localStorage["posStorage"]=$(window).scrollTop();});\n\n';
                script += '$("#syntaq-content").css("btn-wizard-nav-next","btn-wizard-nav-previous").click(function(){if(localStorage){localStorage.removeItem("posStorage");}});\n\n';
                script += '$("#syntaq-content").ready(function(){setTimeout(function(){if(localStorage){var posReader=localStorage["posStorage"];if(posReader>0){$(window).scrollTop(posReader);localStorage.removeItem("posStorage");return true;}}return false;},1);});\n\n';
                script += '};';

                // Comment
                loadjscssfile(script, "script");

                // onFormChange(null, null);

                //check the region is loaded or not
                if (window.regionSyntaq === "Unknown") {
                    // LOAD SCHEMA 
                    setTimeout(function () {
                        buildform(formOBJ.schema, formOBJ.readOnly);
                    }, 1000);
                } else {
                    // LOAD SCHEMA
                    buildform(formOBJ.schema, formOBJ.readOnly);
                }

                if (response.contributor !== null) {
                    Syntaq.isProject = true;
                    buildContributorContent(response.contributor, response.documents);


                    //if form is readonly, and not approval contributor role, hidden the nav bar button
                    if (formOBJ.readOnly && !(response.contributor.message.includes("Approve") && (response.contributor.recordMatterContributor.stepRole == 1))) {
                        $('.btn-wizard-nav-cancel').prop('disabled', true)
                        $('#contributor-btn-content .btn').prop('disabled', true)

                    }

                }

            }).fail(function () {

                if (abp.session.userId !== null) {
                    window.location.replace(_SyntaqBaseURI);
                }

            });

            function buildContributorDialog() {
                contributor_dialog.show();
            }

            var contributor_dialog;
            function buildContributorContent(contributor, documents) {

                //text-dark font-weight-bold mt-2 mb-2 me-2      
                var projectName = '<h1 class="h2 ps-4">' + Syntaq.Project.Name + '</h1>';  // Project Name in the header
                var btncontent = '<div id="contributor-content" class="list-inline kt-container kt-container--fluid pl-0 ml-0 mt-2" style="clear:both;"><div id="contributor-btn-content"></div></div>';
                var contributorActions = '<div class="list-inline-item pull-right"><span class="dropdown action-button"><button class="btn btn-primary" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> Actions <i class="fas fa-angle-down"></i></button><ul id="contributor-btn-options" class="dropdown-menu mw-100" style="position: absolute; inset: 0px 0px auto auto; margin: 0px; transform: translate3d(0px, 36px, 0px);"></ul></span></div>';
                var projDocuments = '<div class="list-inline-item pull-right mr-2"><span class="dropdown action-button" hidden><button class="btn btn-primary" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> Documents <i class="fas fa-angle-down"></i></button><ul id="project-documents" class="dropdown-menu project-documents mw-100 overflow-auto"></ul></span></div>';
                var btndraft = '<li><a href="#" type="button" id="btn-contributor-draft" data-title="Draft" data-status="3"  data-contributor-status="Draft" class="btn-wizard-nav-cancel btn-draft dropdown-item">' + app.localize('Draft') + '</a></li>';
                var btndone = '<li><a href="#" type="button" id="btn-done" data-title="Done" data-status="3" class="btn-wizard-nav-cancel btn-done dropdown-item">' + app.localize('Finalise') + '</a></li>';
                var btnpreview = '<li><a href="#" type="button" id="btn-contributor-preview" data-title="" data-status="" class="btn-wizard-nav-cancel btn-preview dropdown-item">' + app.localize('Preview') + '</a></li>';
                var btnsaveandclose = '<li><a href="#" type="button" id="btn-contributor-saveclose" data-title="" data-status="" class="btn-wizard-nav-cancel btn-saveclose dropdown-item">' + app.localize('Save and close') + '</a></li>';
                var btnclose = '<li><a href="#" type="button" id="btn-contributor-saveclose" data-title="" data-status="" class="btn-wizard-nav-cancel btn-saveclose dropdown-item">' + app.localize('Close') + '</a></li>';
                var btnpublish = '<li><a href="#" type="button" id="btn-contributor-publish" data-title="Publish" data-status="3"  class="btn-wizard-nav-cancel btn-publish dropdown-item" disabled style="opacity: 0;display:none;">' + '</a></li>'; //make this button not visable and clickable
                var btnfinalise = '<li><a href="#" type="button" id="btn-contributor-finalise" data-title="Finalise" data-status="3"  data-contributor-status="Final" class="btn-wizard-nav-cancel btn-finalise dropdown-item">' + app.localize('Finalise') + '</a></li>';
                var btnendorse = '<li><a href="#" type="button" id="btn-contributor-endorse" data-title="Submit Review" data-status="3" data-contributor-status="Approve" class="btn-wizard-nav-cancel btn-contributor dropdown-item">' + app.localize('Approve') + '</a></li>';
                var btnapprove = '<li><a href="#" type="button" id="btn-contributor-approve" data-title="Approve" data-status="3" data-contributor-status="Approve" class="btn-wizard-nav-cancel btn-contributor dropdown-item">' + app.localize('Approve') + '</a></li>';
                var btnreject = '<li><a href="#" type="button" id="btn-contributor-reject" data-title="Reject" data-status="1"  data-contributor-status="Reject" class="btn-wizard-nav-cancel btn-contributor dropdown-item">' + app.localize('Reject') + '</a></li>';
                var btncancel = '<li><a href="#" type="button" id="btn-contributor-cancel" data-title="Decline"  data-status="2"  data-contributor-status="Cancel" class="btn-wizard-nav-cancel btn-contributor dropdown-item" disabled style="opacity: 0; display:none;" role="none">' + '</a></li>'; //make this cancel button not visable and clickable, only clickable when automatically clicked in the shareproject page
                var btncomments = '<li><a href="#" type="button" id="btn-contributor-comments" data-title="Comments" class="btn-wizard-nav-comments btn-contributor-comments dropdown-item">' + app.localize('Comments') + '</a></li>';
                var btncomplete = '<li><a href="#" type="button" id="btn-contributor-complete" data-title="Complete" data-status="3" class="btn-wizard-nav-cancel btn-contributor dropdown-item">' + app.localize('Complete') + '</a></li>';

                switch (contributor.message) {
                    case "Draft":
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-warning badge-inline">' + contributor.message + '</span></div>'
                        break;
                    case "Final":
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-success badge-inline">' + contributor.message + '</span></div>'
                        break;
                    case "New":
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-danger badge-inline">' + contributor.message + '</span></div>'
                        break;
                    case "FinalUnlocked":
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-unified-dark badge-inline">' + 'Final Unlocked' + '</span></div>'
                        break;
                    case "Canceled Review":
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-info badge-inline">' + 'Cancelled Review' + '</span></div>'
                        break;
                    default:
                        var spanmessage = '<div bis_skin_checked="1" class = "ps-4" id="project-status-message"><span class="label badge badge-info badge-inline">' + contributor.message + '</span></div>'
                }

                var btninvite = '<li><a href="#" type="button" id="btn-invite" class="btn-wizard-nav-cancel dropdown-item" data-status="2">' + app.localize('Share') + '</a></li>';

                $('#syntaq-contributor-content').append(btncontent);
                $('#contributor-content').append(projectName);

                if (contributor.recordMatterContributor.stepRole === 3) {
                    if (contributor.requireApproval) {
                        $('#contributor-btn-content').append(contributorActions);
                        $('#contributor-btn-options').append(btnsaveandclose);
                        $('#contributor-btn-options').append(btnfinalise);
                        $('#contributor-btn-options').append(btninvite);
                        $('#contributor-btn-options').append(btnpreview);
                        $('#contributor-btn-options').append(btnpublish);
                    }
                    else {
                        $('#contributor-btn-content').append(contributorActions);
                        $('#contributor-btn-options').append(btnsaveandclose);
                        $('#contributor-btn-options').append(btndone);
                        $('#contributor-btn-options').append(btnpreview);
                    }

                    var _createOrEditModal = new app.ModalManager({
                        viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/CreateOrEditModal',
                        scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_CreateOrEditModal.js',
                        modalClass: 'CreateOrEditRecordMatterContributorModal'
                    });

                    $("#btn-invite").click(function () {
                        _createOrEditModal.open({ id: null, recordmatterId: Syntaq.Form.RecordMatterId, formID: Syntaq.Form.FormId, role: 2 });
                    });
                }

                if (contributor.recordMatterContributor.stepRole === 0) {
                    $('#contributor-btn-content').append(contributorActions);
                    $('#contributor-btn-options').append(btnsaveandclose);
                    $('#contributor-btn-options').append(btnendorse);
                    $('#contributor-btn-options').append(btnreject);
                    $('#contributor-btn-options').append(btnpreview);
                }

                if (contributor.recordMatterContributor.stepRole === 1) {
                    $('#contributor-btn-content').append(contributorActions);
                    $('#contributor-btn-options').append(btnclose);
                    $('#contributor-btn-options').append(btnendorse);
                    $('#contributor-btn-options').append(btnreject);
                    $('#contributor-btn-options').append(btnpreview);
                }

                //share role should not show now
                if (contributor.recordMatterContributor.stepRole === 2) {
                    $('#contributor-btn-content').append(contributorActions);
                    $('#contributor-btn-options').append(btnsaveandclose);
                    $('#contributor-btn-options').append(btncomplete);
                    $('#contributor-btn-options').append(btnreject);
                    $('#contributor-btn-options').append(btnpreview);
                }




                $('#contributor-btn-content').append(btncancel);
                $('#contributor-content').append(spanmessage);

                var authtoken = Syntaq.Form.AnonAuthToken;
                if (authtoken == '') {
                    authtoken = getParameterByName('AccessToken');
                }

                //test authoken is empty
                if (documents !== undefined && documents !== null) {

                    $('#contributor-btn-content').append(projDocuments);
                    documents.forEach(function (document, index) {
                        var documentelement = '<li><a class="mr-2" href="' + _SyntaqBaseURI + '/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + document.id + '&version=1&format=' + document.format + '&AccessToken=' + authtoken + '" target="_blank"><img height=25 src="' + _SyntaqBaseURI + '/Common/Images/Entities/' + document.format + '.png"/> ' + document.name + '</a></li>';
                        $('#project-documents').append(documentelement);

                    });
                    if ($('#project-documents li').length > 0) {
                        $('#project-documents').parent().removeAttr("hidden");
                    }
                }


                if (contributor.recordMatterContributor.stepRole === 0 || contributor.recordMatterContributor.stepRole === 1 || contributor.recordMatterContributor.stepRole === 3) {
                    $('#contributor-content').append('<hr   />');
                }

                $('body').append(Syntaq.Form.ContributorDialogTemplate);
                contributor_dialog = new Modal('#contributor-dialog', {
                    backdrop: true
                });

                var status = null;
                var contributorstatus = null;
                var contributortype = null;
                var contributormessage = '';
                $(".btn-contributor").click(function () {
                    status = $(this).data('status');
                    contributorstatus = $(this).data('contributor-status');

                    contributortype = $(this).data('title');
                    if (contributortype == "Submit Review") {
                        $('#btn-contributor-comment-save').text('Approve');
                    } else {
                        $('#btn-contributor-comment-save').text(contributortype);
                    }

                    switch (contributortype) {
                        case 'Complete':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success pull-right");
                            contributormessage = 'You successfully completed editing the document.';
                            break;
                        case 'Publish':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success pull-right");
                            contributormessage = 'You successfully published the document.';
                            break;
                        case 'Submit Review':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success pull-right");
                            contributormessage = 'You successfully reviewed the document.';
                            break;
                        case 'Reject':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-danger pull-right");
                            contributormessage = 'Review Rejected.';
                            break;
                        case 'Approve':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success pull-right");
                            contributormessage = 'You successfully approved the document.';
                            break;
                    }

                    contributor_dialog.show();
                });

                $(".btn-done").click(function () {

                    if (Form.pages) {
                        Form.setPage(Form.pages.length - 1);
                    }

                    // if (isFormValid) {
                    var datatitle = $(this).data('title');
                    var title = 'Confirm ' + datatitle;
                    var status = $(this).data('status');

                    swal({
                        title: "Are you sure?",
                        text: app.localize('Do you want to Finalise this Step?'),
                        //icon: "info",
                        buttons:
                        {
                            confirm: {
                                text: "Yes",
                                value: true,
                                visible: true,
                                className: "",
                                closeModal: true
                            },
                            cancel: {
                                text: "No",
                                value: false,
                                visible: true,
                                className: "",
                                closeModal: true,
                            }
                        }//,
                        // dangerMode: true,
                    })
                        .then((result) => {
                            if (result) {
                                setTimeout(function () {
                                    if (!Form.checkValidity(Form.data, true)) {
                                        return Promise.reject(Form.showErrors(null, true));

                                    }
                                    else {
                                        finaliseStep(title);
                                    }

                                }, 1500);
                            }
                        });
                });

                $(".btn-preview").click(function () {
                    Syntaq.Form.HasDocuments = true;
                    Syntaq.Form.ShowSubmissionDialog = true;
                    var submission = { "data": Form.data };
                    submission.data.sfasystemredirect = '';
                    submission.data.ContributorId = contributor.recordMatterContributor.id;
                    submission.data.ContributorStatus = "Draft";
                    Syntaq.Form.SubmitFunction(submission);
                });


                $(".btn-saveclose").click(function () {
                    var submission = Form.data;
                    Syntaq.Form.SaveFunction(submission, true);
                });

                function publishStep(title) {

                    var status = $(this).data('status');

                    swal({
                        title: "Are you sure?",
                        text: app.localize('Do you want to publish this Step?'),
                        // icon: "info",
                        buttons:
                        {
                            confirm: {
                                text: "Yes",
                                value: true,
                                visible: true,
                                className: "",
                                closeModal: true
                            },
                            cancel: {
                                text: "No",
                                value: false,
                                visible: true,
                                className: "",
                                closeModal: true,
                            }
                        },
                        dangerMode: false,
                    })
                        .then((result) => {
                            if (result) {
                                publishStep2();
                            }
                        });

                }

                function publishStep2() {
 
                    var data = JSON.stringify({ Id: Syntaq.Form.RecordMatterId, Publish: true });
                    var url = _SyntaqBaseURI + '/api/services/app/Projects/PublishStep';
                    Syntaq.Form.ShowSubmissionDialog = false;

                    abp.ajax({
                        type: "POST",
                        contentType: 'application/json',
                        url: url,
                        data: data,
                        beforeSend: function (request) {
                            request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                        },
                    }).done(function (data) {
                        $("#contributor-content button").attr("disabled", true);
                        Syntaq.Form.HasDocuments = false;
                        var submission = { "data": Form.data };
                        submission.data.ContributorId = contributor.recordMatterContributor.id;
                        submission.data.ContributorStatus = "Share";
                        submission.data.sfasystemredirect = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject?Id=' + ProjectId + '&RecordMatterId=' + Syntaq.Form.RecordMatterId;

                        Syntaq.Form.SubmitFunction(submission);

                    }).fail(function (data) {
                    });

                }

                $("#btn-contributor-publish").click(function () {
                    $('#btn-contributor-publish').disabled = false;

                    publishStep2();
                });

                function finaliseStep() {

                    //if (isFormValid) {
                    var data = JSON.stringify({ Id: Syntaq.Form.RecordMatterId, Finalise: true });
                    var url = _SyntaqBaseURI + '/api/services/app/Projects/FinaliseStep';
                    Syntaq.Form.ShowSubmissionDialog = false;

                    abp.ajax({
                        type: "POST",
                        contentType: 'application/json',
                        url: url,
                        data: data,
                        beforeSend: function (request) {
                            request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                        },
                    }).done(function (data) {
                        $("#contributor-content button").attr("disabled", true);
                        Syntaq.Form.HasDocuments = false;

                        if (Form.data) {
                            if (Form.data.ProjectSteps) {
                                $.each(Form.data.ProjectSteps, function (key, value) {
                                    if (value.stepId === Syntaq.Form.RecordMatterId) {
                                        value.status = "Final";

                                    }
                                });
                            }
                        }

                        var submission = { "data": Form.data };
                        submission.data.ContributorId = contributor.recordMatterContributor.id;
                        submission.data.ContributorStatus = "Final";
                        submission.data.sfasystemredirect = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject?Id=' + ProjectId + '&RecordMatterId=' + Syntaq.Form.RecordMatterId;
                        Syntaq.Form.SubmitFunction({ "data": submission.data });

                    }).fail(function (data) {

                    });
                    //  }


                }

                $("#btn-contributor-finalise").click(function () {

                    if (Form.pages) {
                        Form.setPage(Form.pages.length - 1);
                    }
                    // if (isFormValid) {
                    var datatitle = $(this).data('title');
                    var title = 'Confirm ' + datatitle;
                    var status = $(this).data('status');

                    swal({
                        title: "Are you sure?",
                        text: app.localize('Do you want to Finalise this Step?'),
                        //icon: "info",
                        buttons:
                        {
                            confirm: {
                                text: "Yes",
                                value: true,
                                visible: true,
                                className: "",
                                closeModal: true
                            },
                            cancel: {
                                text: "No",
                                value: false,
                                visible: true,
                                className: "",
                                closeModal: true,
                            }
                        }//,
                        // dangerMode: true,
                    })
                        .then((result) => {
                            if (result) {

                                setTimeout(function () {
                                    if (!Form.checkValidity(Form.data, true)) {
                                        return Promise.reject(Form.showErrors(null, true));

                                    }
                                    else {
                                        finaliseStep(title);
                                    }

                                }, 1500);

                            }
                        });
                });

                $('body').append(Syntaq.Form.ContributorCommentDialogTemplate);
                var contributor_dialog = new Modal('#contributor-comment-dialog', {
                    backdrop: true
                });

                $('#contibutor-dialog-comment').val(contributor.recordMatterContributor.comments);

                $(".btn-contributor-comments").click(function () {
                    contributor_dialog.show();
                });

                $("#btn-contributor-comment-save").click(function () {
                    
                    var accessToken = getUrlParameter('AccessToken'); // replaces AnonAuthToken
                    if (accessToken !== null && accessToken !== 'null' && accessToken !== '') {
                        Syntaq.Form.AnonAuthToken = accessToken;
                    }
                    else {
                        accessToken = Syntaq.Form.AnonAuthToken;
                    }

                    //var Data = JSON.stringify(submission);
                    //var input = JSON.stringify('{"AccessToken": "' + accessToken + '", "AnonAuthToken": "' + Syntaq.Form.AnonAuthToken + '",  "Id": "' + Syntaq.Form.FormId + '", "RecordId":"' + Syntaq.Form.RecordId + '", "RecordMatterId":"' + Syntaq.Form.RecordMatterId + '", "RecordMatterItemId":"' + Syntaq.Form.RecordMatterItemId + '", "Submission": ' + Data + '}');
                    //var AuthToken = getParameterByName('AccessToken');
                    //var data = { 'AccessToken': AuthToken, 'Comments': $('#contibutor-dialog-comment').val() };

                    var data = {"AccessToken": accessToken , "AnonAuthToken": Syntaq.Form.AnonAuthToken, "Comments": $('#contibutor-dialog-comment').val()};

                    toastr.success(Syntaq.Form.SubmittingMessage);

                    //MB MODIFIED
                    submitting = true;

                    abp.ajax({
                        type: "PUT",
                        contentType: 'application/json',
                        url: _SyntaqBaseURI + "/api/services/app/RecordMatterContributors/UpdateComments",
                        data: JSON.stringify(data)
                    }).done(function (data) {

 
                        data = JSON.stringify({ AccessToken: accessToken, Status: status, ContributorStatus: contributorstatus });
                        var url = _SyntaqBaseURI + '/api/services/app/RecordMatterContributors/Apply';


                        abp.ajax({
                            type: "POST",
                            contentType: 'application/json',
                            url: url,
                            data: data,
                            beforeSend: function (request) {
                                request.setRequestHeader('Abp.TenantId', Syntaq.TenantId);
                                request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                            },
                        }).done(function (data) {

                            //MB MODIFIED
                            submitting = false;

                            $("#contributor-content button").attr("disabled", true);

                            var submission = { "data": Form.data };

                            if (abp.session.userId !== null) {
                                submission.data.sfasystemredirect = '/Falcon/Dashboard';
                            }
                            else {
                                submission.data.sfasystemredirect = '';
                                swal({
                                    title: "Success",
                                    text: contributormessage,
                                    buttons:
                                    {
                                        confirm: {
                                            text: "Ok",
                                            value: true,
                                            visible: true,
                                            className: "",
                                            closeModal: true
                                        }
                                    }
                                })
                                    .then((result) => {
                                        //after anon user click approve or reject, close the window
                                        setTimeout(function () {
                                            location.reload();
                                            //window.open(location, '_self', '');
                                        }, 1000);
                                    });
                            }

                            // Need to get the Status here
                            Syntaq.Form.HasDocuments = false;

                            submission.data.ContributorId = contributor.recordMatterContributor.id;
                            submission.data.ContributorStatus = contributorstatus;
                            
                            Syntaq.Form.SubmitFunction({ "data": submission.data });

                        }).fail(function (data) {
                        });


                    }).fail(function () {
                        toastr.error(Syntaq.Form.SubmittingError);
                    });

                });

            }

            
            function buildform(form, readOnly) {

                setToastrOptions();

                // Supress Warnings
                console.warn = () => { };

                //detect whether the form is saved or submitted;
                var saved = false;

                if (typeof Formio !== "undefined") {

                    var formJSON = JSON.parse(form);

                    if (typeof UpdateSchema === 'function') {
                        formJSON = UpdateSchema(formJSON);
                    }

                    // User accessible schmema modification
                    if (typeof window.setFormLogic === 'function') {
                        formJSON = window.setFormLogic(formJSON);
                    }

                    Formio.icons = 'fontawesome';
                    Formio.createForm(document.getElementById(Syntaq.Form.ElementId), formJSON, { readOnly: readOnly, noAlerts: false })
                        .then(function (form) {

 
                            loadjscssfile(_SyntaqBaseURI + "/assets/formio/app/fontawesome/css/font-awesome.min.css", "css");
                            loadjscssfile(_SyntaqBaseURI + "/lib/toastr/build/toastr.css", "css");
                            loadjscssfile(_SyntaqBaseURI + "/lib/spin.js/spin.css", "css");
                            loadjscssfile(".form-control {border-width: 2px ;}", "cssinline");
                            loadjscssfile(_SyntaqBaseURI + "/TenantCustomization/GetCustomCss", "css");
                            loadjscssfile("#syntaq-content .navbar { padding-left: 0rem!important; }", "cssinline");
                            loadjscssfile("#syntaq-content .dropdown-menu { max-height: 36em; overflow-y: auto; position: relative;box-shadow: 0px 0px 0px 0px white; min-width:0px; padding:0px;margin-bottom:1rem}", "cssinline");
 
                            Form = form;

                            // Payment Form is removed if at the end
                            // and is added back / removed if the form is valid
                            var paymenformpushed = false;
                            var lastformispayment = false;

                            // Array object to track the validity of a form and display
                            // validity in the wizard breadcrumb on render
                            FormPages = {
                                "pages": [
                                ]
                            }
 
                            if (Form.pages) {
                                var lastform = Form.pages[Form.pages.length - 1];
                                lastformispayment = lastform.key.toLowerCase() === 'payment' ? true : false;
                                Form.pages.forEach(function (page, index) {
                                    var item = { "isvalid": false, "key": page.key };
                                    FormPages.pages.push(item)
                                });
                            }

                            // For LC To Flag Invalid Wizard Pages
                            var LCWizardValidation = 0;
                            var LCCustomisationClass = "LCWizardValidation";

                            form.on('initialized', function () {
                                if (typeof InitCustomFormScript === "function") {
                                    InitCustomFormScript();
                                }

                                if (JSON.stringify(formJSON).indexOf('"type":"popupform"')) {

                                    Form.updateValue({
                                        modified: true
                                    });
                                    Form.redraw();
                                }
                                // Record all the important fields 
                                importantComp(form);

                                buildWizardHeader();
                                buildAutoSaveHeader();

                                // Receiving Customization Class From Form Script
                                LCWizardValidation = $('.' + LCCustomisationClass).length;

 
                            });

                            form.on('change', function (event) {
                                //there are changes in the form;
                                saved = false;

                                if (event.changed !== 'undefined') {
                                    onFormChange(event, form);
                                }

                                if (Form.schema.hasOwnProperty("isBuildTicks") && Form.schema.isBuildTicks) {
                                    buildTicks();
                                }

                                showhidePaymentForm();

                                isFormValid = true;
                                checkFormIsvalid(Form);


                                if (FormPages.pages[Form.page] !== undefined) {
                                    FormPages.pages[Form.page].isvalid = isFormValid;
                                }

                                // Add title to every form input fields
                                addTitleToCompInput(Form);

                                //Customisations Required In On Form Change Event

                                /* For hiding/showing important fields via form script */
                                customizedImportantFlag(Form);

                                /* For LC Flagging Invalid Wizard page(s) */
                                if (LCWizardValidation) {
                                    LCTrackInvalidPage();
                                }

                                // Customisation -------- End
                                 
                            });

                            //Warning there are unsaved changes in the form
                            closeWarning(form)

                            function closeWarning(form) {

                                window.onbeforeunload = function (event) {

                                    if (!saved) {
                                        event = event || window.event;
                                        event.returnValue = 'Leave site? Changes that you made may not be saved.';
                                    }
                                }
                            }
                            //warning there are unsaved changes in the form -------END

                            // Call initial Feedback funtion
                            if (Form.schema.hasOwnProperty("feedbackForm") && Form.schema.feedbackForm != null) {
                                loadjscssfile(_SyntaqBaseURI + "/assets/Syntaq/FeedbackForm/feedbackFormStyle.css", "cssinhead");
                                loadjscssfile(_SyntaqBaseURI + "/assets/Syntaq/FeedbackForm/SyntaqFeedback.js", "js");
                                // set time to wait the JS loaded
                                setTimeout(function () { initialFeedbackFunction(Form.schema.feedbackForm); }, 3000);

                            }
                            // Call initial Feedback funtion-----END
                            function showhidePaymentForm() {
                                if (lastformispayment) {

                                    // If second to last page and all ticks not there then hide next
                                    var isvalid = true;
                                    FormPages.pages.forEach(function (page, index) {
                                        if (!page.isvalid && page.key !== 'Payment' || Form.page === Form.pages.length - 2) {
                                            isvalid = false;
                                        }

                                        // ***************** Ticks issue start ********************
                                        // Thi is only checking the from - 21 / Reference
                                        // Manualy tick all the valid forms here 
                                        // TODO : We'll remove or modify if these work works on the dev / production server
                                        //if (Form.page == 21) // Reference
                                        //{
                                        if (page.isvalid) // if it is valid form 
                                            if ($($('.page-link')[index]).children("i").length == 0) // Checking the double tick 
                                                $($('.page-link')[index]).append(Syntaq.Form.ShowProgressElement); // allowed tick
                                        //}

                                        // ***************** Ticks issue end ********************
                                    });

                                    if (Form.page === Form.pages.length - 2) {
                                        isvalid = true;
                                    }


                                    var isSummary = false;
                                    if (Form.page === Form.pages.length - 2) {
                                        isSummary = true;
                                    }

                                    if (isvalid) {
                                        var pageitems = document.querySelectorAll(".page-item");
                                        var pageitem = pageitems[pageitems.length - 1];
                                        pageitem.style.display = "block";

                                        var nextbtns = document.querySelectorAll(".btn-wizard-nav-next");
                                        for (var i = 0; i < nextbtns.length; ++i) {
                                            nextbtns[i].style.display = "block";
                                        }

                                    }
                                    else {
                                        var pageitems = document.querySelectorAll(".page-item");
                                        var pageitem = pageitems[pageitems.length - 1];
                                        pageitem.style.display = "none";

                                        if (isSummary) {
                                            var nextbtns = document.querySelectorAll(".btn-wizard-nav-next");
                                            for (var i = 0; i < nextbtns.length; ++i) {
                                                nextbtns[i].style.display = "none";
                                            }
                                        }

                                    }
                                }
                            }

                            // For LC Validate Wizard Pages And Flag Invaid Ones 
                            function LCTrackInvalidPage() {

                                Form.checkCurrentPageValidity(Form.submission.data, true);
                                FormPages.pages.forEach(function (page, index) {

                                    // Track Form Changes
                                    var link = $('.page-link')[index];
                                    $(link).removeClass(LCCustomisationClass);
                                    if (!page.isvalid) {
                                        $(link).addClass(LCCustomisationClass);
                                        $(".page-link." + LCCustomisationClass).css("color", "red");
                                    } else {
                                        $(link).removeClass(LCCustomisationClass);
                                    }

                                });
                                $(".page-link").css("color", "#006272");
                                $(".page-link." + LCCustomisationClass).css("color", "red");
                            }

                            function buildTicks() {

                                /////////////////////////////
                                // Validate Form
                                /////////////////////////////

                                if (Syntaq.Form.ShowProgressTicks) {
                                    // Track Form Changes
                                    var link = $('.page-link')[Form.page];
                                    var linktext = $(link).text();

                                    $(link).empty();
                                    $(link).append(linktext);
                                    $(link).removeClass(Syntaq.Form.ShowProgressClass);

                                    isFormValid = true;
                                    checkFormIsvalid(Form); // Form.checkCurrentPageValidity(Form.submission.data, true);

                                    if (FormPages.pages[Form.page] !== undefined) {
                                        FormPages.pages[Form.page].isvalid = isFormValid;
                                        if (isFormValid) {
                                            $(link).append(Syntaq.Form.ShowProgressElement);
                                            $(link).addClass(Syntaq.Form.ShowProgressClass);
                                        }
                                    }

                                }

                                FormPages.pages.forEach(function (page, index) {

                                    // Track Form Changes
                                    var link = $('.page-link')[index];
                                    var linktext = $(link).text();

                                    $(link).empty();
                                    $(link).append(linktext);
                                    $(link).removeClass(Syntaq.Form.ShowProgressClass);


                                    if (page.isvalid) {
                                        $(link).append(Syntaq.Form.ShowProgressElement);
                                        $(link).addClass(Syntaq.Form.ShowProgressClass);
                                    }

                                });

                                /////////////////////////////
                                // End Validate Form
                                /////////////////////////////

                            }

                            function buildImportant() {

                                /////////////////////////////
                                // Build Summary if required
                                /////////////////////////////

                                // <label class="text-warning font-weight-bold"> <span class="fa fa-exclamation"></span> Some pages are still missing information recommended for a final contract:</label>
                                // importantview += "<label>This means your contract is not yet in a form suitable for signing, but that’s okay if all you need is a draft.</label>"

                                Form.data.HasImportantFieldsMissing = false;
                                Syntaq.Form.ImportantViews = [];

                                if (Form.pages) {
                                    var importantview = '';
                                    var importantrows = '';
                                    var pages = Form.getPages({
                                        all: true
                                    });
                                    $('.syntaq-component-important').html('<div id="syntaq-component-important-alert" style="background-color: #ffb822c7" class="alert alert-warning  row" role="alert">  <h4 class="alert-heading font-weight-bold col-12"><span class="fa fa-exclamation-triangle"></span> Some pages are still missing information recommended for a final contract.</h4>  <p class="col-12">This means your contract is not yet in a form suitable for signing, but that’s okay if all you need is a draft.</p><div id="important-table" class="w-100"></div></div>');

                                    var i = 0;
                                    pages.forEach(function (page) {
                                        importantview = '';
                                        importantrows = '';

                                        buildImportantSection(page, .75);

                                        var pagelabel = page.label;
                                        var hasview = true;

                                        if (importantrows === '') {
                                            hasview = false;
                                        }
                                        else {
                                            importantview = '<table class=" table table-sm    table-bordered"><tr><td colspan=2 style="font-weight: bold; background-color: #fff " ><h4 style="font-weight: bold;"><span class="fa fa-window-maximize"></span> ' + pagelabel + '</h3></td></tr>' + importantrows + '</table><p></p>';
                                        }

                                        Syntaq.Form.ImportantViews[i] = { key: page.key, important: importantview, hasview: hasview };
                                        i++;
                                    });


                                    Syntaq.Form.ImportantView = ''; Form.data.HasImportantFieldsMissing = false;
                                    Syntaq.Form.ImportantViews.forEach(function (view) {
                                        if (view.hasview) {
                                            $('#important-table').append(view.important);
                                            Syntaq.Form.ImportantView += view.important;
                                            Form.data.HasImportantFieldsMissing = true;
                                        }
                                    });

                                    if (!Form.data.HasImportantFieldsMissing) {
                                        $('#syntaq-component-important-alert').hide();
                                        //let alert = document.createElement();
                                        // $('.syntaq-component-important').prepend('<div class="alert alert-warning row" role="alert">  <h4 class="alert-heading col-12"><span class="fa fa-exclamation-triangle"></span> Some pages are still missing information recommended for a final contract.</h4>  <p class="col-12">This means your contract is not yet in a form suitable for signing, but that’s okay if all you need is a draft.</p></div>');
                                    }

                                    function buildImportantSection(parent, indent) {

                                        indent += .75;
                                        var addedheader = false;
                                        parent.components.forEach(function (comp) {
                                            var type = comp.component.type;
                                            if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {

                                                if (comp._visible) {
                                                    buildImportantSection(comp, indent);
                                                }
                                            }
                                            else {
                                                if (comp.component.important && comp._visible && !comp.hidden) {
                                                    var value = getDataValue(comp, comp.key, false);
                                                    if (value === '' || value === null || value === undefined) {
                                                        var label = comp.label;
                                                        if (comp.parent.parent && !addedheader) {
                                                            importantrows += '<tr><td style="padding-left: ' + (indent - .75) + 'em; font-weight: bold; background-color: #fff " colspan="2"> ' + comp.parent.parent.component.label + '</td></tr>';
                                                            addedheader = true;
                                                        }
                                                        if (!comp.component.importantdisplay) {
                                                            importantrows += '<tr><td class="small"  style="padding-left: ' + indent + 'em; ; background-color: #fff;  font-weight: bold; "><span class="ml-4">' + label + '</span></td></tr>';
                                                        }
                                                    }
                                                }
                                            }
                                        });

                                    }


                                    /////////////////////////////
                                    // End Build Important if required
                                    /////////////////////////////	
                                }

                            }

                            function buildSummary() {

                                /////////////////////////////
                                // Build Summary if required
                                /////////////////////////////

                                if (Form.pages) {
                                    var summaryview = '';
                                    var summaryrows = '';
                                    //var page = Form.pages[Form.page];
                                    //var pagelabel = page.label;

                                    //Form.components.forEach(function (comp) {
                                    //    var type = comp.component.type;
                                    //    if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
                                    //        buildSummarySection(comp, 1.5);
                                    //    }
                                    //    else {
                                    //        buildSummaryComponent(comp, .75);
                                    //    }
                                    //});

                                    //var hasview = true;
                                    //if (summaryview === '') hasview = false;
                                    //summaryview = '<table style="width:100%; border:0px solid #dee2e6 " ><tr><td colspan=2 style="font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6; font-size:1.1em" >' + pagelabel + '</td></tr>' + summaryview + '</table><p></p>';
                                    //Syntaq.Form.SummaryViews[Form.page] = { key: page.key, summary: summaryview, hasview: hasview };

                                    //$('.syntaq-component-summarytable').html('');
                                    //Syntaq.Form.SummaryView = '';
                                    //Syntaq.Form.SummaryViews.forEach(function (view) {
                                    //    if (view.hasview) {
                                    //        $('.syntaq-component-summarytable').append(view.summary);
                                    //        Syntaq.Form.SummaryView += view.summary;
                                    //    }
                                    //});

                                    function buildSummarySection(parent, indent) {
                                        //STQ Modified -- 10357
                                        var dynamicId = 'sectionpanel' + Math.floor(Math.random() * 100);
                                        indent += .75;
                                        parent.components.forEach(function (comp) {
                                            var type = comp.component.type;
                                            if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
                                                if (comp._visible || comp.component.hidden) {
                                                    if (comp.component.showSummary != false) {
                                                        var label = comp.parent.component.label;
                                                        if (type === 'sectionpanel') {
                                                            if (!summaryview.includes(dynamicId)) {
                                                                summaryview += '<tr><td id=' + dynamicId + 'style = "padding-left: ' + indent + 'em; font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6" colspan = "2" > ' + label + '</td ></tr > ';
                                                            }
                                                            else {
                                                                summaryview += '<tr><td class="dividerDiv mt-8" colspan="2" style="background-color: #f2f2f2; height: 0.7rem; border: 1px solid #f2f2f2;"></td></tr>';
                                                            }
                                                        }
                                                        buildSummarySection(comp, indent);
                                                    }
                                                }
                                            }
                                            else {
                                                if (comp._visible || comp.component.hidden) {
                                                    buildSummaryComponent(comp, indent);
                                                }
                                            }
                                        });
                                    }

                                    function buildSummarySection(parent, indent) {
                                        //STQ Modified -- 10357 
                                        var dynamicId = 'sectionpanel' + Math.floor(Math.random() * 100);
                                        indent += .75;
                                        parent.components.forEach(function (comp) {
                                            var type = comp.component.type;
                                            if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
                                                if (comp._visible || comp.component.hidden) {
                                                    if (comp.component.showSummary != false) {
                                                        var label = comp.parent.component.label;
                                                        if (type === 'sectionpanel') {
                                                            if (!summaryview.includes(dynamicId)) {
                                                                summaryview += '<tr><td id=' + dynamicId + 'style = "padding-left: ' + indent + 'em; font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6" colspan = "2" > ' + label + '</td ></tr > ';
                                                            }
                                                            else {
                                                                summaryview += '<tr><td class="dividerDiv mt-8" colspan="2" style="background-color: #f2f2f2; height: 0.5rem; border: 1px solid #f2f2f2;"></td></tr>';
                                                            }
                                                        }
                                                        buildSummarySection(comp, indent);
                                                    }
                                                }
                                            }
                                            else {
                                                if (comp._visible || comp.component.hidden) {
                                                    buildSummaryComponent(comp, indent);
                                                }
                                            }
                                        });
                                    }

                                    function buildSummaryComponent(comp, indent) {
                                        if (comp.component.showSummary) {
                                            var value = getDataValue(comp, comp.key, false);
                                            var label = comp.label;
                                            summaryview += '<tr><td  style="padding-left: ' + indent + 'em; width:50%; ; border:1px solid #dee2e6">' + label + '</td><td style="width:50%; ; border:1px solid #dee2e6">' + value + '</td></tr>';
                                        }
                                    }

                                    var pages = Form.getPages({
                                        all: true
                                    });


                                    var i = 0;
                                    pages.forEach(function (page) {
                                        summaryview = '';
                                        summaryview === buildSummarySection(page, 1.5);

                                        var hasview = true;
                                        var pagelabel = page.label;

                                        if (summaryview === '') {
                                            hasview = false;
                                        }
                                        else {
                                            summaryview = '<table style="width:100%; border:0px solid #dee2e6 " ><tr><td colspan=2 style="font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6; font-size:1.1em" >' + pagelabel + '</td></tr>' + summaryview + '</table><p></p>';
                                        }

                                        Syntaq.Form.SummaryViews[i] = { key: page.key, summary: summaryview, hasview: hasview };
                                        i++;
                                    });


                                    $('.syntaq-component-summarytable').html('');
                                    Syntaq.Form.SummaryView = '';
                                    Syntaq.Form.SummaryViews.forEach(function (view) {
                                        if (view.hasview) {
                                            $('.syntaq-component-summarytable').append(view.summary);
                                            Syntaq.Form.SummaryView += view.summary;
                                        }
                                    });

                                }

                                /////////////////////////////
                                // End Build Summary if required
                                /////////////////////////////	
                            }

                            function buildWizardHeader() {

                                //just add buger menu once
                                //var is_exist = $('#navbarNav1');

                                var is_exist = $('#syntaq-content').find('nav');

                                //buger menu
                                if (Form.schema.hasOwnProperty("display") && Form.schema.display === "wizard" && is_exist.length === 0) {

                                    var container = '<div class="container" style="max-height:5em; over-flow-y:hidden" >'

                                    //var header = $("[aria-label='navigation']");
                                    var header = $('#syntaq-content').find('nav').first();

                                    header.css('margin', '-5px');
                                    //header.wrapAll(container);

                                    $(header).addClass("navbar navbar-expand-lg navbar-light");

                                    var WizardButton = '<button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-target="#navbarNav1"><span class="navbar-toggler-icon"></span></button>';

                                    header.prepend(WizardButton);
                                    var div = '<div class="collapse navbar-collapse" id="navbarNav1"></div>';

                                    //.dropdown-menu
                                    var ul = $('.pagination');
                                    ul.addClass('dropdown-menu');
                                    ul.wrap(div);
                                    ul.addClass('navbar-nav');
                                    // ul.children().wrapAll(container);
                                    // ul.css('--bs-scroll-height', '100px');
                                    var li = document.querySelectorAll('.page-item');
                                    for (var i = 0; i < li.length; ++i) {
                                        $(li).addClass('nav-item');
                                        li[i].display = "";
                                    }
                                }
                            }

                            function buildAutoSaveHeader() {   // Not in awaiting review contributor
                                // auto save only when editable
                                if (!readOnly && Form.schema.hasOwnProperty("autoSaving") && Form.schema.autoSaving && $('#autoSaveSign').length == 0) {
                                    if ($('#contributor-content').length > 0) {  // when in project view
                                        // auto save for wizard and form
                                        var autoSaveStatus = '<span id="autoSaveSign"><span class="ms-3 ml-3 me-1 h6" style="font-size: 0.7em;">AutoSave</span><i class="fas fa-circle ms-1 me-1 text-success" style="font-size: .8em;"></i></span>'
                                        var savingMessage = '<span class="savingMessage h6" style="font-size: 0.7em;"></span>';

                                        $("#project-status-message").append(autoSaveStatus);
                                        $('#autoSaveSign').append(savingMessage);
                                        autoSaveForm(form);
                                    } else {
                                        // auto save for wizard and form
                                        var autoSaveStatus = '<div id="autoSaveSign" class="ms-3 me-2 mt-3 mb-n3 bg-white" style="top: 65px; z-index:3;"><i class="pull-right fas fa-circle ms-1 me-1 text-success" style="font-size: .8em;"></i><span class="pull-right mr-1 h6" style="font-size: 0.7em;">AutoSave</span></div>'
                                        var savingMessage = '<span class="savingMessage pull-right list-inline-item h6 me-4" style="font-size: 0.7em;"></span>';
                                        $(autoSaveStatus).insertBefore($('#syntaq-content'));  // when in form view
                                        $('#autoSaveSign').prepend(savingMessage);
                                        autoSaveForm(form);
                                        if (Form.schema.hasOwnProperty("stickyMenu") && Form.schema.stickyMenu) {
                                            $('#autoSaveSign').addClass("sticky-top");
                                        }
                                    }
                                }
                            }

                            var isAutoSave = false;

                            // Auto save form function.
                            // Auto save form function.
                            function autoSaveForm(form) {
                                var save = false;
                                setInterval(function () {
                                    if ($('div.formio-modified').length > 0 && save && !submitting) {
                                        isAutoSave = true;
                                        $('#autoSaveSign > .savingMessage').text("Saving...");
                                        form.emit('save', form.submission.data);
                                    }
                                }, 10000); //save every 10 seconds 
                                form.on('change', function () {
                                    save = false;
                                    if ($('div.formio-modified').length > 0 && !submitting) {
                                        save = true;
                                    }
                                });
                            }
                            //Auto save form function-------END

                            function buildPostAnonReviewedPage(){
                                $('#syntaq-contributor-content').attr('class', '');
                                $('#syntaq-content').attr('class', 'w-450px bg-body rounded shadow p-lg-15 mx-auto ');
                                $('#syntaq-content div').attr('style', 'display: block; text-align: center;');
                                $('#syntaq-content ul').attr('style', 'display: none ;');
                                $('#syntaq-content').parent().attr('class', 'd-flex flex-center flex-column flex-column-fluid pb-20 mb-20');
                                $('#syntaq-content').parent().parent().css('background-image', 'url(/metronic/assets/media/svg/illustrations/login.png)');
                                $('#syntaq-content').parent().parent().attr('class', 'd-flex flex-column flex-column-fluid bgi-position-y-bottom position-x-center bgi-no-repeat bgi-size-cover bgi-attachment-fixed');
                                $('#syntaq-content').parent().parent().parent().attr('class', 'd-flex flex-column flex-root');
                            }

                            form.on('render', function () {
                                buildSummary();
                                buildImportant();

                                showhidePaymentForm();
                                buildWizardHeader();
                                buildAutoSaveHeader();
                                if(location.href.includes('loadanon') && Syntaq.Form.FormName ==='No_Access_For_Syntaq'){
                                    buildPostAnonReviewedPage();
                                }

                            });

                            // Prevent the submission from going to the form.io server.
                            form.nosubmit = true;

                            // We have an AuthToken
                            // Get the RecordMAtterId
                
                            if (!Syntaq.Form.RecordId) {
                                //check the querystring 
                                var recordId = getUrlParameter('RecordId');
                                Syntaq.Form.RecordId = recordId === '' ? uuidv4() : recordId;
                            }

                            var AuthToken = getParameterByName('AccessToken');
                            // If has Auth Token get the RecordMatterId and RecordMatterItemId
                            // Encoded into token

                            if (!Syntaq.Form.RecordMatterId) {
                                var recordMatterId = getUrlParameter('RecordMatterId');
                                Syntaq.Form.RecordMatterId = recordMatterId === '' ? uuidv4() : recordMatterId;
                            }

                            if (!Syntaq.Form.RecordMatterItemId) {
                                var recordMatterItemId = getUrlParameter('RecordMatterItemId');
                                // Syntaq.Form.RecordMatterItemId = recordMatterItemId === '' ? uuidv4() : recordMatterItemId;
                                if (recordMatterItemId === '') {
                                    Syntaq.Form.RecordMatterItemId = uuidv4();
                                    Syntaq.Form.SubmissionId = uuidv4();
                                } else {

                                    Syntaq.Form.RecordMatterItemId = recordMatterItemId;

                                    var url = _SyntaqBaseURI + "/api/services/app/submissions/getIdByRecordMatterItemId?id=" + Syntaq.Form.RecordMatterItemId + "&AccessToken=" + AuthToken;
                                    if (Syntaq.Form.AnonAuthToken !== null && Syntaq.Form.AnonAuthToken !== '') {
                                        url += '?AccessToken=' + Syntaq.Form.AnonAuthToken;
                                    }

                                    abp.ajax({
                                        type: "GET",
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        contentType: 'application/json',
                                        url: formurl
                                    }).done(function (response) {
                                        Syntaq.Form.SubmissionId = response.id;
                                    });

                                }
                                //fetch or create suibid here
                            }

                            if (!Syntaq.Form.SubmissionId) {
                                Syntaq.Form.SubmissionId = uuidv4();
                            }

                            if (Syntaq.Form.FormType === 'default') {
                                loadRecordData();
                            }
                            else if (Syntaq.Form.FormType === 'popup') {
                                loadFormData();
                            }

                            function loadFormData() {
                                initFormData(JSON.parse(Syntaq.Form.FormData));
                            }

                            function loadRecordData() {

                                var url = _SyntaqBaseURI + "/api/services/app/RecordMatters/GetRecordMatterForEdit?id=" + Syntaq.Form.RecordMatterId + "&RecordMatterItemId=" + Syntaq.Form.RecordMatterItemId + "&FormId=" + Syntaq.Form.FormId; 

                                var recordmode = getParameterByName('load');
                                if (recordmode === 'record' || Syntaq.Form.LoadFromRecord) {
                                    url = _SyntaqBaseURI + "/api/services/app/Records/GetRecordForEdit?id=" + Syntaq.Form.RecordId;
                                }

                                if (AuthToken !== null && AuthToken !== '') {
                                    url += '&AccessToken=' + AuthToken;
                                }

                                if (Syntaq.Form.AnonAuthToken !== null && Syntaq.Form.AnonAuthToken !== '') {
                                    url += '&AccessToken=' + Syntaq.Form.AnonAuthToken;
                                }

                                abp.ajax({
                                    type: "GET",
                                    beforeSend: function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                    },
                                    contentType: 'application/json',
                                    url: url
                                }).done(function (response) {

                                    var data = null;
                                    if (getParameterByName('load') === 'record' || Syntaq.Form.LoadFromRecord) {
                                        var record = response.record;
                                        data = JSON.parse(record.data);

                                        //Syntaq.Form.RecordId = record.id;
                                        Syntaq.Form.AnonAuthToken = record.accessToken;

                                    }
                                    else {
                                        var recordmatter = response.recordMatter;
                                        data = JSON.parse(recordmatter.data);

                                        if (recordmatter.recordId != '00000000-0000-0000-0000-000000000000') {
                                            Syntaq.Form.RecordId = recordmatter.recordId;
                                        }                                        
                                        Syntaq.Form.AnonAuthToken = recordmatter.accessToken;

                                    }

                                    // If you have loaded a recordmatter make sure to ensure the record ID matches
                                    // also load the ANONAccessToken
                                    initFormData(
                                        data
                                    );

                                    Syntaq.Form.LoadFromRecord = false;

                                }).fail(function (data) {
                                    console.write(data);
                                });

                            }

                            function initFormData(data) {
                                // Initialise any functions in custom script
                                onFormChange(null, form);
                                if (typeof window.InitFormData === "function") {
                                    JSONResult = window.InitFormData(data);
                                }
                                JSONResult = LoopThroughQueryString(data);
                                form.submission = {
                                    data: data
                                };
                                form.redraw();
                                if (typeof window.AfterFormDataLoad === "function") {
                                    window.AfterFormDataLoad();
                                }

                                if (Form.pages) {
                                    Form.pages.forEach(function (page, index) {
                                        Syntaq.Form.SummaryViews.push({ key: page.key, summary: '' });
                                    });
                                }

                                var projectNew = false;
                                //check form is in the new status or not
                                if (data.ProjectSteps) {
                                    $.each(data.ProjectSteps, function (key, value) {
                                        if (value['stepId'] === data['CurrentStep']) {
                                            if (value[status] === 'New') {
                                                projectNew = true;
                                            }
                                        }
                                    });
                                }

                                //back to the page where user saved and exited
                                // Commented due to single page issue
                                if (Form.schema.hasOwnProperty("display") && Form.schema.display == "wizard" && data.page !== 'undefined' && projectNew === false) {
                                    
                                    if (Syntaq.Form.ContinueSession) {
                                        setTimeout(function () {
                                            form.setPage(data.page)
                                                .catch(e => {
                                                    setTimeout(function () {
                                                        form.setPage(data.page)
                                                            .catch();
                                                    }, 1000);
                                                }
                                                );
                                        }, 500);
                                    }
                                }
                                // End back to the page where they saved and exited

                            }

                            form.on('draft', function (submission) {

                            });

 

                            form.on('prevPage', function (submission) {
                                firstload = true;
                                window.scrollTo(0, 0);
                            });

                            form.on('nextPage', function (submission) {
                                firstload = true;                                
                                window.scrollTo(0, 0);


                            });

                            closepopup = true;
                            form.on('save', function (submission) {
                                saveForm(submission, false);
                            });

                            function saveForm(submission, saveandclose) {

                                saved = true;

                                if (typeof window.BeforeFormSave === 'function') {
                                    var result = window.BeforeFormSave();
                                    if (result === false) {
                                        return;
                                    }
                                }

                                if (Syntaq.Form.FormType === 'popup') {
                                    if (Syntaq.Form.PopRequired ? checkpopformValid() : true) {
                                        transferData(submission, Syntaq.Form.PopName);
                                    }
                                }
                                else if (Syntaq.Form.FormType === 'default') {

                                    if (Syntaq.Form.AnonAuthToken === null || Syntaq.Form.AnonAuthToken === 'null') {
                                        Syntaq.Form.AnonAuthToken = '';
                                    }
                                    submission.AccessToken = Syntaq.Form.AnonAuthToken;
                                    submission.RecordId = Syntaq.Form.RecordId;
                                    submission.RecordMatterId = Syntaq.Form.RecordMatterId;
                                    submission.RecordMatterItemId = Syntaq.Form.RecordMatterItemId;

                                    if (Syntaq.Form.SubmissionId === '') Syntaq.Form.SubmissionId = uuidv4();
                                    submission.SubmissionId = Syntaq.Form.SubmissionId;

                                    if (Form.schema.hasOwnProperty("display") && Form.schema.display == "wizard") {
                                        submission.page = form.hasOwnProperty("page") ? form.page : 0;
                                    }

                                    var accessToken = getUrlParameter('AccessToken'); // replaces AnonAuthToken
                                    if (accessToken !== null && accessToken !== 'null' && accessToken !== '') {
                                        Syntaq.Form.AnonAuthToken = accessToken;
                                    }
                                    else {
                                        accessToken = Syntaq.Form.AnonAuthToken;
                                    }

                                    if (submission && submission.inputs) {
                                        submission.inputs = [];
                                    }

                                    if (submission && submission.Data) {
                                        submission.Data.inputs = [];
                                    }

                                    if (submission && submission.data) {
                                        submission.data.inputs = [];
                                    }

                                    var Data = JSON.stringify(submission);
                                    var input = JSON.stringify('{"AccessToken": "' + accessToken + '", "AnonAuthToken": "' + Syntaq.Form.AnonAuthToken + '",  "Id": "' + Syntaq.Form.FormId + '", "RecordId":"' + Syntaq.Form.RecordId + '", "RecordMatterId":"' + Syntaq.Form.RecordMatterId + '", "RecordMatterItemId":"' + Syntaq.Form.RecordMatterItemId + '", "Submission": ' + Data + '}');

                                    // Don't trigger another auto save
                                    submitting = true;

                                    abp.ajax({
                                        type: "POST",
                                        contentType: 'application/json',
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        url: _SyntaqBaseURI + "/api/services/app/forms/save",
                                        data: input
                                    }).done(function (data) {
                                        var jsonResult = JSON.parse(data);
                                        Syntaq.Form.AnonAuthToken = jsonResult.AnonAuthToken;
                                        Syntaq.Form.RecordId = jsonResult.RecordId;
                                        Syntaq.Form.RecordMatterId = jsonResult.RecordMatterId;
                                        Syntaq.Form.RecordMatterItemId = jsonResult.RecordMatterItemId;
                                        Syntaq.Form.SubmissionId = jsonResult.SubmissionId;

                                        if (typeof window.AfterFormSave === 'function') {
                                            window.AfterFormSave();
                                        }

                                        if (saveandclose) {
                                            if (ProjectId === "") {
                                                window.location.replace(_SyntaqBaseURI + '/Falcon/Projects/')
                                            }
                                            else {
                                                var openProject = _SyntaqBaseURI + '/Falcon/Projects/ViewProject?Id=' + ProjectId + '&RecordMatterId=' + Syntaq.Form.RecordMatterId;
                                                window.location.replace(openProject)
                                            }
                                        }

                                        // How is this enabled?
                                        // If Enabled then need to know if auto or manual save
  
                                        $('#autoSaveSign > .savingMessage').text(Syntaq.Form.SavedMessage);

                                        if (isAutoSave !== true) {
                                            toastr.success(Syntaq.Form.SavedError);
                                        }

                                        isAutoSave = false;

                                        // MB MODIFIED TODO
                                        submitting = false;
 

                                    }).fail(function () {
                                        submitting = false;
                                        $('#autoSaveSign > .savingMessage').text("Form not saved");
                                    });

                                }
                            }

                            Syntaq.Form.SaveFunction = saveForm;

                            form.on('draft', function (submission) {
                            });

                            // Triggered when they click the submit button.
                            form.on('submit', function (submission) {
                                submitForm(submission);
                            });

                            function submitForm(submission) {

                                try {

                                    toastr.info(Syntaq.Form.SubmittingMessage);
                                    saved = true;

                                    if (typeof window.BeforeFormSubmit === 'function') {
                                        submission = window.BeforeFormSubmit(submission);
                                    }

                                    if (typeof window.ASICSubmit === 'function') {
                                        var result = window.ASICSubmit(submission);
                                        return;
                                    }

                                    //asic submit

                                    var redirectURL;
                                    if (typeof submission.data["sfasystemredirect"] !== "undefined") {
                                        if (submission.data["sfasystemredirect"] !== "") {
                                            redirectURL = submission.data["sfasystemredirect"];
                                        }
                                    }

                                    var formId = JSON.stringify(Syntaq.Form.FormId);

                                    if (Syntaq.Form.AnonAuthToken === null || Syntaq.Form.AnonAuthToken === 'null') {
                                        Syntaq.Form.AnonAuthToken = '';
                                    }

                                    submission.data.RecordId = Syntaq.Form.RecordId;
                                    submission.data.RecordMatterId = Syntaq.Form.RecordMatterId;
                                    submission.data.RecordMatterItemId = Syntaq.Form.RecordMatterItemId;

                                    if (Syntaq.Form.SubmissionId === '') Syntaq.Form.SubmissionId = uuidv4();
                                    submission.data.SubmissionId = Syntaq.Form.SubmissionId;

                                    var summarytable = '';
                                    if (Syntaq.Form.SummaryView === '') { // V1 Summary Control
                                        var temp = {};
                                        temp.summarytable = FormioUtils.generateTable(Form);
                                        summarytable = JSON.stringify(temp.summarytable);
                                    }
                                    else {  // V2 Summary Control
                                        summarytable = JSON.stringify(Syntaq.Form.SummaryView);
                                    }

                                    if (submission && submission.inputs) {
                                        submission.inputs = [];
                                    }

                                    if (submission && submission.Data) {
                                        submission.Data.inputs = [];
                                    }

                                    if (submission && submission.data) {
                                        submission.data.inputs = [];
                                    }

                                    var data = JSON.stringify(submission);
                                    var input = JSON.stringify('{ "AnonAuthToken": "' + Syntaq.Form.AnonAuthToken + '", "id":' + formId + ', "submission":' + data + ',"summarytablehtml":' + summarytable + '}');

                                    if (Syntaq.Form.HasDocuments && Syntaq.Form.ShowSubmissionDialog) {
                                        buildSubmissionDialog();
                                    }

                                    submitting = true;

                                    abp.ajax({
                                        type: "POST",
                                        contentType: 'application/json',
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        url: _SyntaqBaseURI + "/api/services/app/forms/run",
                                        data: input
                                    }).done(function (data) {

                                        toastr.success(Syntaq.Form.SubmittedMessage);

                                        if (typeof window.AfterFormSubmit === 'function') {
                                            window.AfterFormSubmit();
                                        }

                                        if (redirectURL) {
                                            window.location.replace(redirectURL.toString());
                                        }

                                        // START POLLING SUBMISSION
                                        if (Syntaq.Form.HasDocuments && Syntaq.Form.ShowSubmissionDialog) {
                                            poll = setTimeout(pollSubmission, 5000);
                                        }


                                    }).fail(function (data) {
                                        //abp.message.error(data);

                                        $('#submission-dialog-loader').remove();
                                        $('#submission-dialog-title').text('Warning');
                                        $('#submission-dialog-body').html(`<div>${data.message}</div>`);

                                    });

                                }
                                catch (err) {
                                    Form.showErrors(err, true);
                                    //Form.showErrors(err, true);
                                    // MB MODIFIED
                                    $('#submission-dialog-loader').remove();
                                    $('#submission-dialog-title').text('Warning');
                                    $('#submission-dialog-body').html(`<div>${err}</div>`);
                                }

                            };

                            Syntaq.Form.SubmitFunction = submitForm;

                            $('body').append(Syntaq.Form.SubmissionDialogTemplate);
                            var submission_dialog = new Modal('#submission-dialog', {
                                backdrop: true
                            });

                            function buildSubmissionDialog() {

                                $('#submission-dialog-body').html('<div id=\'submission-dialog-loader\' class=\'loader\'></div>');
                                $('#submission-dialog-title').text('Your Form is Submitting');
                                submission_dialog.show();
                            }

                            var submitting = false;
                            var poll = null; 
                            function pollSubmission() {

                                var url = _SyntaqBaseURI + "/api/services/app/submissions/GetSubmissionForView?id=" + Syntaq.Form.SubmissionId;
                                if (Syntaq.Form.AnonAuthToken !== null && Syntaq.Form.AnonAuthToken !== '') {
                                    url += '&AccessToken=' + Syntaq.Form.AnonAuthToken;
                                }

                                submitting = true;

                                abp.ajax({
                                    type: "GET",
                                    contentType: 'application/json',
                                    beforeSend: function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                    },
                                    url: url
                                }).done(function (data) {


                                    if (data !== null) {

                                        if (data.submission.submissionStatus === 'Rejected') {
                                            $('#submission-dialog-loader').remove();
                                            $('#submission-dialog-title').text('Your Submission has been Rejected');

                                            clearTimeout(poll);
                                            return;
                                        }

                                        if (data.submission.submissionStatus === 'Complete') {

                                            $('#submission-dialog-loader').remove();

                                            $('#submission-dialog-title').text('Your Document is Complete');
                                            if (data.recordMatterItems.length > 1 || data.recordMatterItems.length === 1) {
                                                $('#submission-dialog-title').text('Your Documents are Complete');
                                            }

                                            var cnt = 0;
                                            var hasdoc = false;
                                            var isLastIteration = false;
                                            var $submissionDialogBody = $('#submission-dialog-body');
                                            var $tbody = $('<tbody id="submission-dialog-tbody-id"></tbody>');

                                            data.recordMatterItems.forEach(function (item, index) {

                                                cnt === 0 ? $('#submission-dialog-body').append('<table class="border-0"><tbody id="submission-dialog-tbody-id"></tbody></table>') : '';

                                                var authtoken = Syntaq.Form.AnonAuthToken;
                                                if (authtoken == '') {
                                                    authtoken = getParameterByName('AccessToken');
                                                }

                                                var itemurl =
                                                    _SyntaqBaseURI +
                                                    "/Falcon/Documents/GetDocumentForDownload?Id=" +
                                                    item.id +
                                                    "&AccessToken=" +
                                                    authtoken;

                                                var tableRowTemplate = '<tr><td>'

                                                if (item.allowWord) {
                                                    tableRowTemplate += '<div class="p-1"><a href="' + itemurl + '&format=docx" target="_blank"><img style="max-height:32px; max-width:32px" class="stq-primary-icon-sm me-2" title="doc" src="' + _SyntaqBaseURI + '/common/images/primaryicons/doc.svg">' + item.documentName + '</a></div>';
                                                }

                                                if (item.allowPdf) {
                                                    tableRowTemplate += '<div class="p-1"><a href="' + itemurl + '&format=pdf" target="_blank"><img style="max-height:32px; max-width:32px" class="stq-primary-icon-sm me-2" title="doc" src="' + _SyntaqBaseURI + '/common/images/primaryicons/pdf.svg">' + item.documentName + '</a></div>';
                                                }

                                                if (item.allowHTML) {
                                                    tableRowTemplate += '<div class="p-1"><a  href="' + itemurl + '&format=docx" target="_blank"><img style="max-height:32px; max-width:32px" class="stq-primary-icon-sm me-2" title="doc" src="' + _SyntaqBaseURI + '/common/images/primaryicons/html.svg"></a></div>';
                                                }


                                                if (item.allowPdf || item.allowWord || item.allowHTML) {
                                                    hasdoc = true;
                                                    tableRowTemplate += '</td></tr>';
                                                    $('#submission-dialog-tbody-id').append(tableRowTemplate);
                                                }

                                                if (data.hasFiles != null && data.hasFiles == true && cnt == data.recordMatterItems.length - 1) {
                                                    var url = _SyntaqBaseURI + "/api/services/app/Files/GetFiles?RecordId=" + data.submission.recordId + "&RecordMatterId=" + data.submission.recordMatterId + "&RecordMatterItemGroupId=" + item.groupId;
                                                    if (Syntaq.Form.AnonAuthToken !== null && Syntaq.Form.AnonAuthToken !== '') {
                                                        url += '&AccessToken=' + Syntaq.Form.AnonAuthToken;
                                                    }

                                                    abp.ajax({
                                                        type: "GET",
                                                        contentType: 'application/json',
                                                        beforeSend: function (request) {
                                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                                        },
                                                        url: url
                                                    }).done(function (data) {
                                                        data.items.forEach((row) => {
                                                            var tableRowTemplate1 = '<tr><td>'
  
                                                            tableRowTemplate1 = '<p><a class="OnClickLink" href="' + _SyntaqBaseURI + '/Falcon/Files/DownloadFile?RecordId=' + row.file.recordId + '&AccessToken=' + Syntaq.Form.AnonAuthToken + '&RecordMatterId=' + row.file.recordMatterId + '&RecordMatterItemGroupId=' + row.file.recordMatterItemGroupId + '&Filename=' + row.file.fileName + '" name="ExportRuleLink"><img class="stq-primary-icon me-2" title="App" src="' + _SyntaqBaseURI + '/common/images/primaryicons/download.png"> ' + row.file.fileName + '</a></p>';

                                                            tableRowTemplate1 += '</td></tr>';
                                                            $('#submission-dialog-tbody-id').append(tableRowTemplate1);
                                                        });

                                                    });


                                                }

                                                submitting = false;
                                                cnt++ === data.recordMatterItems.length - 1 ? $('#submission-dialog-body').append('</tbody></table>') : '';
                                            });
                                            clearTimeout(poll);

                                            return;

                                        }

                                        poll = setTimeout(pollSubmission, 5000);
                                        return;

                                    }
                                    else {
                                        poll = setTimeout(pollSubmission, 5000);
                                        return;
                                    }

                                }).fail(function () {
                                    //abp.message.error(e.error.message);
                                    submitting = false;
                                });

                            }

                        })
                        .then(() => {

                            // Read paramters
                            var finalise = getUrlParameter('finalised');
                            if (finalise == 1) {
                                setTimeout(function () {
                                    $("#btn-contributor-finalise").trigger('click');
                                }, 1500);
                            }
                            else if (finalise == 2) {
                                $("#btn-contributor-cancel").disabled = false;
                                setTimeout(function () {
                                    $("#btn-contributor-cancel").trigger('click');
                                }, 1500);
                            }

                            var share = getUrlParameter('share');
                            if (share === 'true') {
                                $("#btn-invite").trigger('click');
                            }

                        });
                }
                else {
                    setTimeout(function () {
                        buildform(form, readOnly);
                    }, 500);
                }
            }
        },
        SubmissionDialogTemplate: "<div id=\"submission-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\" style=\"padding:0\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Your Form is Submitting</h5><button type=\"button\" class=\"close btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button></div><div class=\"modal-body\"><div id=\"submission-dialog-body\" style=\"min-height:8em; display: flex; align-items: center; height: 100%;\"></div></div><div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">Close</button></div></div></div></div>",
        ContributorDialogTemplate: "<div id=\"contributor-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\" style=\"padding:0\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Feedback</h5><button type=\"button\" class=\"close btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button></div><div class=\"modal-body\"><div id=\"contributor-dialog-body\" style=\"min-height:8em;display: flex; align-items: center; height: 100%;\"><textarea class=\"form-control\" rows=8 id=\"contributor_comment\" aria-label=\"Contributor Comment\" placeholder=\"Enter your comments here\"></textarea></div></div><div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-default\">Save</button><button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">Cancel</button></div></div></div></div>",
        ContributorCommentDialogTemplate: "<div id=\"contributor-comment-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\" style=\"padding:0\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Comments</h5><button type=\"button\" class=\"close btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button></div><div class=\"modal-body\"><div id=\"contributor-comment-dialog-body\" style=\"min-height:8em;display: flex; align-items: center; height: 100%;\"><textarea class=\"form-control\" id=\"contibutor-dialog-comment\" aria-label=\"Contributor Dialog Comment\" rows=12></textarea></div></div><div class=\"modal-footer\"  ><button id=\"btn-contributor-comment-save\" type=\"button\" class=\"btn btn-primary pull-right\" data-bs-dismiss=\"modal\">Submit</button><button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">Close</button></div></div></div></div>",
    },
    UserProfile: {

        ElementId: 'syntaq-content',

        LoadUI: function () {  // Method which will display type of animal

            setToastrOptions();

            if (Syntaq.GetAuthToken() === null || Syntaq.GetAuthToken() === undefined) {
                Syntaq.Account.Login(this.LoadUI);
                return;
            }

            function setUserTab(tabid) {
                var i;
                var x = document.getElementsByClassName("user-info");
                for (i = 0; i < x.length; i++) {
                    x[i].style.display = "none";
                }
                document.getElementById(tabid).style.display = "block";
            }

            abp.ajax({
                type: "GET",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                contentType: 'application/json',
                url: _SyntaqBaseURI + '/api/services/app/User/GetUserProfileForEdit'
            }).done(function (response) {

                Syntaq.userId = response.user.id;

                var template = Handlebars.compile(userProfiletemplate);
                var userprofilecontent = document.getElementById(Syntaq.UserProfile.ElementId);
                userprofilecontent.innerHTML = template(response.user);

                $("[name=Syntaq-User-Submit]").click(function () {
                    Syntaq.UserProfile.Submit();
                });

                $(".w3-row > a").click(function () {
                    setUserTab($(this).data("tab"));
                    $(".tablink").removeClass("w3-border-red");
                    $(this).find('div').addClass("w3-border-red");
                });

                $('#LogoPictureResize').attr('src', _SyntaqBaseURI + '/Profile/GetLogoPicture?userid=' + Syntaq.userId + '&' + new Date().getTime());
                $('#ChangeLogoPictureForm').attr('action', _SyntaqBaseURI + '/Profile/UploadLogoPicture');

                $('#ChangeLogoPictureForm input[name=LogoPicture]').change(function () {
                    //$('#ChangeLogoPictureForm').submit();
                    uploadlogo();
                });

                function uploadlogo() {

                    var form = document.getElementById('ChangeLogoPictureForm');
                    var data = new FormData(form);
                    var fname = $('#ChangeLogoPictureForm input[name=LogoPicture]').val();

                    var file = document.getElementById('syntaq-logo-upload').files[0];
                    var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
                    if ('|jpg|jpeg|png|gif|'.indexOf(type) === -1) {
                        alert('Invalid File Type');
                        return false;
                    }

                    //File size check
                    if (file.size > 5242880) //5MB
                    {
                        alert('File too large');
                        return false;
                    }

                    var mimeType = file.type;
                    data.append("FileType", mimeType);
                    data.append("FileName", fname);
                    data.append("FileToken", uuidv4());

                    $.ajax({
                        type: "POST",
                        enctype: 'multipart/form-data',
                        url: _SyntaqBaseURI + '/Profile/UploadLogoPicture',
                        data: data,
                        processData: false,
                        contentType: false,
                        cache: false,
                        timeout: 600000,

                        beforeSend: function (request) {
                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                        },
                        success: function (data) {
                            if (data.success) {

                                var $LogoPictureResize = $('#LogoPictureResize');

                                var LogoFilePath = _SyntaqBaseURI + '/Profile/GetLogoPicture?userid=' + Syntaq.userId + '&' + new Date().getTime();
                                uploadedLogoFileToken = data.result.fileToken;

                                $LogoPictureResize.show();
                                $LogoPictureResize.attr('src', LogoFilePath);
                                $LogoPictureResize.attr('originalWidth', data.result.width);
                                $LogoPictureResize.attr('originalHeight', data.result.height);

                                toastr.success(Syntaq.Form.SavedMessage);

                            } else {
                                abp.message.error(response.error.message);
                            }

                        },
                        error: function (e) {

                            $("#result").text(e.responseText);
                            console.log("ERROR : ", e);
                            $("#btnSubmit").prop("disabled", false);

                        }
                    });
                }
            });

        },
        Submit: function () {

            function serializeToJson(serializer) {
                var _string = '{';
                for (var ix in serializer) {
                    var row = serializer[ix];
                    _string += '"' + row.name + '":"' + row.value + '",';
                }
                var end = _string.length - 1;
                _string = _string.substr(0, end);
                _string += '}';
                console.log('_string: ', _string);
                return JSON.parse(_string);
            }

            var xhrUser = new XMLHttpRequest();
            xhrUser.onreadystatechange = function () {

                if (this.readyState === 1 || (this.readyState === 4 && this.status === 200)) {
                    swal({
                        title: "Success",
                        text: 'Profile Updated'
                    });
                }

            };

            xhrUser.open("PUT", _SyntaqBaseURI + "/api/services/app/user/UpdateEmbeddedUser");
            if (Syntaq.GetAuthToken() !== null) {
                xhrUser.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
            }
            xhrUser.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            xhrUser.timeout = 7500;

            //var form = document.querySelector('form');
            //var data = new FormData(form);

            var params = $('#UserInformationsForm input').serializeArray();
            params = serializeToJson(params);
            var input = JSON.stringify(params);
            //var formData = $("#UserInformationsForm").serializeObject();
            xhrUser.send(input);

        }

    },
    Records: {
        FolderId: '00000000-0000-0000-0000-000000000000',
        SkipCount: 0,
        Page: 1,
        PageSize: 25,
        ElementId: 'syntaq-records-content',
        RecordTableTemplate: "<div id=\"syntaq-breadcrumb-content\" class=\"large\"><ul id=\"syntaq-breadcrumb-list\" class=\"breadcrumb h5\"><ul></div><table id=\"syntaq-records\" class=\"display table table-bordered table-hover dt-responsive nowrap\" style=\"width:100% !important; margin-top:0!important\"><thead> <tr><th class=\"h5\" style=\"min-width:25em\">Record</th><th class=\"h5\">Record Matter</th></tr></thead></table>",
        RecordTableForFormTemplate: "<table id=\"syntaq-records\" class=\"display table table-bordered table-hover dt-responsive nowrap\" style=\"width:100% !important; margin-top:0!important\"><thead><tr><th colspan=\"2\"><h2>1. Start a new <button id='btn-create-form' class='btn btn-primary' type='button'></button></h2><hr><h2>2. Or use one of your existing records below.</h2><hr><div id=\"syntaq-breadcrumb-content\" class=\"large\"><ul id=\"syntaq-breadcrumb-list\" class=\"breadcrumb h5\"><ul></div><div class=\"input-group mb-3\"><input id=\"syntaq-search\" type=\"text\" class=\"form-control\" placeholder=\"\" aria-label=\"\" aria-describedby=\"\"><div class=\"input-group-append\"><button id=\"syntaq-search-btn\" class=\"btn btn-outline-secondary\" type=\"button\"><span class=\"fa fa-search\"></span></button></div></div></div></th></tr> <tr><th class=\"h5\" style=\"min-width:25em\">Record</th><th class=\"h5\" style=\"min-width:25em\">Matter</th></tr></thead></table>",
        archiveRecord: function (id) {
            abp.ajax({
                type: "POST",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                contentType: 'application/json',
                url: _SyntaqBaseURI + '/api/services/app/Records/archive?Id=' + id
            }).done(function (response) {
                Syntaq.Records.LoadRecords();
            });
        },
        deleteRecord: function (id) {
            abp.ajax({
                type: "DELETE",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                contentType: 'application/json',
                url: _SyntaqBaseURI + '/api/services/app/Records/Delete?Id=' + id
            }).done(function (response) {
                Syntaq.Records.LoadRecords();
            });
        },
        deleteRecordMatter: function (id) {
            abp.ajax({
                type: "DELETE",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                contentType: 'application/json',
                url: _SyntaqBaseURI + '/api/services/app/RecordMatters/Delete?Id=' + id
            }).done(function (response) {
                Syntaq.Records.LoadRecords();
                toastr.success(Syntaq.Form.DeletedMessage);
            });
        },
        deleteFolder: function (id) {
            abp.ajax({
                type: "DELETE",
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                },
                contentType: 'application/json',
                url: _SyntaqBaseURI + '/api/services/app/Folders/Delete?Id=' + id
            }).done(function (response) {
                if (response.success === true) {
                    Syntaq.Records.LoadRecords();
                }
                toastr.success(Syntaq.Form.DeletedMessage);
            });
        },
        deletConfirm: function (typeid, type) {
            swal({
                title: "Are you sure?",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            })
                .then((confirmed) => {
                    if (confirmed) {
                        switch (type) {
                            case "Record":
                                Syntaq.Records.deleteRecord(typeid);
                                break;
                            case "RecordMatter":
                                Syntaq.Records.deleteRecordMatter(typeid);
                                break;
                            case "Folder":
                                Syntaq.Records.deleteFolder(typeid);
                                break;
                        }
                    }
                });
        },
        downloadFile: function (recordmatteritemid, fname, format) {

            var oFileReq = new XMLHttpRequest();

            oFileReq.responseType = 'blob';
            oFileReq.onload = function (event) {
                var blob = oFileReq.response;
                saveBlob(blob, fname);
            };

            // START THE FORM SCHEMA XHR 
            oFileReq.open("GET", _SyntaqBaseURI + '/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + recordmatteritemid + '&version=1&format=' + format);
            oFileReq.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
            oFileReq.send();

        },
        loadFormslist: function () {
            var oformsList = new XMLHttpRequest();
            oformsList.responseType = 'json';
            oformsList.onload = function () {
                var jsonResponse = oformsList.response;
                if ("result" in jsonResponse) {
                    var selectList = document.getElementById("loadFormslist");
                    jsonResponse["result"].forEach(f => {
                        var option = document.createElement("option");
                        option.value = f.value;
                        option.text = f.label;
                        selectList.appendChild(option);
                    });
                }
            };
            oformsList.open("GET", _SyntaqBaseURI + '/api/services/app/Forms/GetFormsList?Flag=NewFormForRecord');
            oformsList.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
            oformsList.send();

        },
        renderNewFormForRecodeModel: function (action) {
            var popmodel = document.getElementById("newformforrecord");
            if (action === 'load') {
                if (popmodel === null) {
                    var temp = '<div class="modal fade" id="newformforrecord" style="opacity: 1;"></div>';
                    var tempDiv = document.createElement('div');
                    tempDiv.innerHTML = temp.trim();
                    document.body.appendChild(tempDiv.firstChild);
                    popmodel = document.getElementById("newformforrecord");
                }
                popmodel.style.display = 'block';
                var tempBody = '<div class="modal-dialog" style="top: 10%;"><div class="modal-content" style="margin-top: 25vh; resize: both; overflow: auto;"><div class="modal-header"><h4 class="modal-title"><span style="margin-left: 2vh; font-weight: bold;">New Form for Record</span></h4></div><div class="modal-body" style="word-break: break-word; text-align: justify; visibility: visible; position: relative;height: 10vh; padding: 1vh 2vh;"><input type="hidden" id="ridForForm" value=""><select name="Nformslist" id="loadFormslist" class="form-control"></select></div><div class="modal-footer"><a href="javascript:void(0);"  class="btn btn-primary" onclick="Syntaq.Records.newFormOpend()">Opend</a><a href="javascript:void(0)" class="btn btn-primary" onclick="Syntaq.Records.renderNewFormForRecodeModel(\'close\')">Close</a></div></div></div>';
                popmodel.innerHTML = tempBody.trim();
                return document.getElementById("ridForForm");
            }
            else {
                popmodel.style.display = 'none';
                popmodel.innerHTML = '';
                return;
            }
        },
        newFormOpend: function () {
            var recordeid = document.getElementById("ridForForm").value;
            var loadformid = document.getElementById("loadFormslist").value;

            if (recordeid !== null && loadformid !== null) {
                Syntaq.Records.loadForm(loadformid, recordeid, null, null);
            }
            var popmodel = document.getElementById("newformforrecord");
            if (popmodel !== null) {
                popmodel.innerHTML = '';
                popmodel.style.display = 'none';
            }
        },
        loadForm: function (formid, recordid, recordmatterid, recordmatteritemid) {

            jQuery('#' + Syntaq.Records.ElementId).html('');
            jQuery('#' + Syntaq.Account.ElementId).html('');
            jQuery('#syntaq-records-content_wrapper').remove();

            Syntaq.Form.FormId = formid;
            Syntaq.Form.RecordId = recordid;
            Syntaq.Form.RecordMatterId = recordmatterid;
            Syntaq.Form.RecordMatterItemId = recordmatteritemid;
            Syntaq.Form.createForm();

        },
        loadBreadCrumbs: function (jsonCrumbs) {

            var ul = document.getElementById('syntaq-breadcrumb-list');
            ul.innerHTML = '';

            for (i in jsonCrumbs.result) {

                var li = document.createElement('li');
                li.setAttribute('class', 'breadcrumb-item');
                li.setAttribute('data-folderid', jsonCrumbs.result[i].id);

                var a = document.createElement('a');
                a.setAttribute('href', 'javascript:void(0)');
                li.appendChild(a);
                a.innerHTML = jsonCrumbs.result[i].name;

                ul.appendChild(li);

                $(li).on("click", function () {
                    //Syntaq.Busy.setBusy();
                    Syntaq.Records.FolderId = $(this).data('folderid');
                    dataTable.ajax.reload();
                });

            }

        },
        LoadRecordsForForm: function () {

            if (Syntaq.GetAuthToken() === null || Syntaq.GetAuthToken() === undefined) {
                Syntaq.Account.Login(Syntaq.Records.LoadRecordsForForm);
                return;
            }

            jQuery('#' + Syntaq.Form.ElementId).html('');
            jQuery('#' + Syntaq.Account.ElementId).html('');
            jQuery('#syntaq-records-content_wrapper').remove();

            setToastrOptions();
            abp.appPath = _SyntaqBaseURI;

            jQuery('#' + Syntaq.Records.ElementId).html(Syntaq.Records.RecordTableForFormTemplate);
            loadjscssfile(_SyntaqBaseURI + "/Common/Scripts/Datatables/datatables.min.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/metronic/themes/default/css/style.bundle.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/assets/formio/app/fontawesome/css/fontawesome.min.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/assets/sweetalert/sweetalert.min.js", "js");

            $('#btn-create-form').text(Syntaq.Form.FormName);
            $("#btn-create-form").click(function () {
                Syntaq.Form.createForm();
            });

            $("#syntaq-search-btn").click(function () {
                getRecordsForForm();
            });

            $("#syntaq-search").keypress(function (e) {
                if (e.which === 13) {
                    getRecordsForForm();
                }
            });

            var url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize + "&Filter=" + $("#syntaq-search").val();

            var dataTableForForm = jQuery('#' + Syntaq.Records.ElementId).DataTable({
                ajax: {
                    async: true,
                    crossDomain: true,
                    url: url,
                    method: "GET",
                    headers: {
                        Authorization: "bearer " + Syntaq.GetAuthToken(),
                        Accept: "*/*",
                        cache: false
                    },
                    dataFilter: function (data) {
                        var json = jQuery.parseJSON(data);
                        json.recordsTotal = json.result.totalCount;
                        json.recordsFiltered = json.result.totalCount;
                        json.data = json.result.items;
                        return JSON.stringify(json); // return JSON string
                    },
                    dataSrc: 'result.items'

                },
                drawCallback: function () {
                    jQuery('.dataTables_paginate > .pagination').find('li').addClass('page-item');
                    jQuery('.dataTables_paginate > .pagination > li').find('a').addClass('page-link');
                    Syntaq.Busy.clearBusy();
                },
                paging: true,
                serverSide: true,
                bFilter: false,
                bLengthChange: false,
                deferLoading: 0,
                createdRow: function (row, data, dataIndex) {

                    $(row).find("[name='Record']").on("click", function () {
                        Syntaq.Form.RecordId = $(this).data('recordid');
                        jQuery('#' + Syntaq.Records.ElementId).html('');
                        Syntaq.Form.LoadFromRecord = true;
                        Syntaq.Form.createForm();
                    });

                    $(row).find("[name='RecordMatter']").on("click", function () {
                        Syntaq.Form.RecordMatterId = $(this).data('recordmatterid');
                        jQuery('#' + Syntaq.Records.ElementId).html('');
                        Syntaq.Form.LoadFromRecord = false;
                        Syntaq.Form.createForm();
                    });

                    $(row).find("[name='Folder']").on("click", function () {
                        Syntaq.Records.FolderId = $(this).data('folderid');
                        Syntaq.Busy.setBusy();
                        getRecordsForForm();
                    });

                },
                columnDefs: [
                    {
                        targets: 0,
                        width: "100px",
                        data: "name",
                        render: function (data, type, row) {

                            if (row.type === 'Folder') {

                                data = '<a name="Folder"  data-folderid="' + row.id + '" class="mb-2"  href="javascript:void(0);"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/folder.png" width=32  /> <strong class=\'h5\'>' + row.name + '</strong></a>';
                                data += '<span class="pull-right mt-3 mr-5"></span>';

                                return data;
                            } else {

                                data = '<div style="cursor:pointer">';

                                data += '<div name="Record" data-recordid="' + row.id + '">';
                                data += '<span style="width: 110px;"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/binders-folder.png"  /> </span>';

                                var dt = new Date(row.lastModified);
                                var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                var tmoptions = { hour: 'numeric', minute: 'numeric' };
                                dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

                                data += '<span style="width: 350px;"><strong class=h5>' + row.name + '</strong><span class="ml-3"> </span></span>';
                                data += '<br>' + dt + '</span>';
                                data += '</div>';

                                data += '</hr>';

                                data += '</div>';

                            }
                            return data;
                        }
                    },
                    {
                        targets: 1,
                        data: null,
                        render: function (data, type, row) {

                            //alert(row.type + '1');

                            if (row.type === 'Folder') {
                                //data = '<a onclick="Syntaq.Records.FolderId=\'' + row.id + '\'; Syntaq.Records.LoadRecords();"  href=#><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/folder.png" width=32  /> <strong class=\'h5\'>' + row.name + '</strong></a>';
                                //data = '<span class="pull-right mt-3 mr-5"> Folder';
                                //data += '</span>';
                                return '';
                            } else {
                                data = '<div style="cursor:pointer">';

                                if (row.recordMatters !== null) {
                                    var rmc = 0;
                                    row.recordMatters.forEach(function (recordmatter) {

                                        data += '<div name="RecordMatter" data-recordmatterid="' + recordmatter.id + '">';
                                        data += '<span style="width: 110px;"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/moleskine.png"  /> </span>';

                                        var dt = new Date(recordmatter.creationTime);
                                        var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                        dt = dt.toLocaleDateString('en-GB', options);

                                        data += '<span style="width: 350px;"><strong class=h5>' + recordmatter.recordMatterName + '</strong><span class="ml-3"> </span></span>';

                                        data += '<br><span>' + dt + '</span>';

                                        data += '</div>';
                                        rmc = rmc + 1;
                                    });
                                    data += '</hr>';
                                    data += '</div>';
                                }
                            }
                            return data;
                        }
                    }
                ]
            }).on('preXhr.dt', function (e, settings, data) {

                //Syntaq.Busy.setBusy();

                var oBreadcrumbReq = new XMLHttpRequest();
                oBreadcrumbReq.onreadystatechange = function () {
                    if (this.readyState === 4 && this.status === 200) {

                        var jsonCrumbs = JSON.parse(oBreadcrumbReq.response);
                        //Syntaq.Records.loadBreadCrumbs(crumbsJSON);

                        var ul = document.getElementById('syntaq-breadcrumb-list');
                        ul.innerHTML = '';

                        for (i in jsonCrumbs.result) {
                            var li = document.createElement('li');
                            li.setAttribute('class', 'breadcrumb-item');
                            li.setAttribute('data-folderid', jsonCrumbs.result[i].id);

                            var a = document.createElement('a');
                            a.setAttribute('href', 'javascript:void(0)');
                            li.appendChild(a);
                            a.innerHTML = jsonCrumbs.result[i].name;

                            ul.appendChild(li);

                            $(li).on("click", function () {
                                Syntaq.Busy.setBusy();
                                Syntaq.Records.FolderId = $(this).data('folderid');
                                getRecordsForForm();
                            });

                        }

                        Syntaq.Busy.clearBusy();
                    }
                };
                oBreadcrumbReq.open("GET", _SyntaqBaseURI + "/api/services/app/Folders/GetBreadcrumbs?id=" + Syntaq.Records.FolderId + "&type=R");
                oBreadcrumbReq.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                oBreadcrumbReq.send();

                settings.ajax.url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize + "&skipcount=" + settings._iDisplayStart + "&Filter=" + $("#syntaq-search").val();
            });

            function getRecordsForForm() {
                url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&Filter=" + $("#btn-syntaq-search").val() + "&maxResultCount=" + Syntaq.Records.PageSize + "&Filter=" + $("#syntaq-search").val();
                dataTableForForm.ajax.reload();
            }

            Syntaq.Busy.setBusy();
            getRecordsForForm();

        },
        LoadRecords2: function () {

            if (Syntaq.GetAuthToken() === null || Syntaq.GetAuthToken() === undefined) {
                Syntaq.Account.Login(Syntaq.Records.LoadRecords);
                return;
            }

            jQuery('#' + Syntaq.Form.ElementId).html('');
            jQuery('#' + Syntaq.Account.ElementId).html('');
            jQuery('#syntaq-records-content_wrapper').remove();

            setToastrOptions();

            var oRecords = new XMLHttpRequest();
            oRecords.onreadystatechange = function () {
                if (this.readyState === 4 && this.status === 200) {

                    var records = JSON.parse(oRecords.response);

                    var recordselement = document.getElementById(Syntaq.Records.ElementId);
                    //recordselement.innerHTML = '';
                    var html = '<ul id="syntaq-breadcrumb-list" class="breadcrumb" style="width:100%"></ul>'
                    html += '<table style="width:100%">'
                    records.result.items.forEach(function (record, index) {


                        if (record.type == "Folder") {
                            html += '<tr class="syntaq-recordfolder">';
                            html += '<td>';
                            html += '<a name=\'Folder\' onclick=\'Syntaq.Records.FolderId="' + record.id + '"; Syntaq.Busy.setBusy(); Syntaq.Records.LoadRecords2();\' data-folderid=\'' + record.id + '\' href=\'javascript:void(0);\'>';
                            html += '<h4><img src=\'' + _SyntaqBaseURI + '/Common/Images/Entities/folder.png\' width=32  /> ' + record.name + ' </strong>';
                            html += '</a>';

                            html += '</td>';
                            html += '<td>';
                            html += '</td>';
                            html += '</tr>';
                        }

                        if (record.type == "Record") {
                            var rmc = 0;

                            record.recordMatters.forEach(function (recordmatter, index) {

                                html += '<tr class="syntaq-recordmatter-header">';
                                html += '<td colspan=2>';
                                html += '<h4>';
                                if (recordmatter.recordMatterName !== null) {
                                    html += recordmatter.recordMatterName;
                                }
                                html += '<span class="small pull-right">';
                                html += '<a href=\'javascript:void(0);\' data-id="' + recordmatter.id + '"  class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete</a>';                                
                                html += '</span>';
                                html += '</h4>';

                                html += '</td>';
                                html += '</tr>';

                                recordmatter.recordMatterItems.forEach(function (recordmatteritem, index) {

                                    var dt = new Date(recordmatteritem.creationTime);
                                    var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                    dt = dt.toLocaleDateString('en-GB', options);

                                    html += '<tr class="syntaq-recordmatter-item">';
                                    html += '<td>';

                                    if (recordmatteritem.document === true) {
                                        if (recordmatteritem.formId !== '00000000-0000-0000-0000-000000000000') {
                                            if (recordmatteritem.lockOnBuild === true) {
                                                html += '<img src="' + _SyntaqBaseURI + '/Common/Images/Entities/lock.png" height=34/></a>';
                                            }
                                            else {
                                                html += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + record.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="javascript:void(0)"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/form.png" height=34/></a>';
                                            }
                                        }
                                        if (recordmatteritem.allowPdf) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.pdf\', \'pdf\')" href="javascript:void(0);"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/pdf.png" /></a>';
                                        }
                                        if (recordmatteritem.allowWord) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.docx\', \'docx\')" href="javascript:void(0);" ><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/word.png" /></a>';
                                        }
                                        if (recordmatteritem.allowHTML) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.html\', \'html\')" href="javascript:void(0);"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/html-filetype.png"  /><strong>';
                                        }

                                    } else {
                                        html += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + record.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="javascript:void(0);"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/form.png" height=34/></a>';
                                    }

                                    if (recordmatteritem.documentName === "" || recordmatteritem.documentName === null) {
                                        var difference = Math.round((new Date() - new Date(recordmatteritem.creationTime)) / 60000);
                                        html += difference < 5 ? 'Document still assembling' : 'Document Name not set';
                                    } else {
                                        html += recordmatteritem.documentName;
                                    }

                                    html += '</td>';

                                    html += '<td>';
                                    html += '<span class="pull-right">' + dt + '</span>';
                                    html += '</td>';

                                    html += '</tr>';
                                });

                                rmc = rmc + 1;

                            });
                        }

                    });
                    html += '</table>';

                    html += '<nav aria-label="Page navigation example"><ul class="pagination" ><li class="page-item"><a class="page-link" href="javascript:void(0);">Previous</a></li><li class="page-item"><a class="page-link" href="javascript:void(0);">1</a></li><li class="page-item"><a class="page-link" href="javascript:void(0);">2</a></li><li class="page-item"><a class="page-link" href="javascript:void(0);">3</a></li><li class="page-item"><a class="page-link" href="javascript:void(0);">Next</a></li></ul></nav>';

                    jQuery('#' + Syntaq.Records.ElementId).html(html);

                    // Breadcrumbs
                    var oBreadcrumbReq = new XMLHttpRequest();
                    oBreadcrumbReq.onreadystatechange = function () {
                        if (this.readyState === 4 && this.status === 200) {

                            var jsonCrumbs = JSON.parse(oBreadcrumbReq.response);
                            //Syntaq.Records.loadBreadCrumbs(crumbsJSON);

                            var ul = document.getElementById('syntaq-breadcrumb-list');
                            ul.innerHTML = '';

                            for (i in jsonCrumbs.result) {
                                var li = document.createElement('li');
                                li.setAttribute('class', 'breadcrumb-item');
                                li.setAttribute('data-folderid', jsonCrumbs.result[i].id);

                                var a = document.createElement('a');
                                a.setAttribute('href', 'javascript:void(0);');
                                li.appendChild(a);
                                a.innerHTML = jsonCrumbs.result[i].name;

                                ul.appendChild(li);

                                $(li).on("click", function () {
                                    Syntaq.Busy.setBusy();
                                    Syntaq.Records.FolderId = $(this).data('folderid');
                                    Syntaq.Records.LoadRecords2();
                                });

                            }

                            Syntaq.Busy.clearBusy();
                        }
                    };
                    oBreadcrumbReq.open("GET", _SyntaqBaseURI + "/api/services/app/Folders/GetBreadcrumbs?id=" + Syntaq.Records.FolderId + "&type=R");
                    oBreadcrumbReq.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                    oBreadcrumbReq.send();

                    settings.ajax.url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize + "&skipcount=" + settings._iDisplayStart;

                    Syntaq.Busy.clearBusy();
                }
            };

            oRecords.open("GET", _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize);
            oRecords.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
            oRecords.send();

        },
        LoadRecords: function () {

            if (Syntaq.GetAuthToken() === null || Syntaq.GetAuthToken() === undefined) {
                Syntaq.Account.Login(Syntaq.Records.LoadRecords);
                return;
            }

            jQuery('#' + Syntaq.Form.ElementId).html('');
            jQuery('#' + Syntaq.Account.ElementId).html('');
            jQuery('#syntaq-records-content_wrapper').remove();

            setToastrOptions();

            abp.appPath = _SyntaqBaseURI;

            jQuery('#' + Syntaq.Records.ElementId).html(Syntaq.Records.RecordTableTemplate);
            loadjscssfile(_SyntaqBaseURI + "/Common/Scripts/Datatables/datatables.min.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/metronic/themes/default/css/style.bundle.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/assets/formio/app/fontawesome/css/fontawesome.min.css", "css");
            loadjscssfile(_SyntaqBaseURI + "/assets/sweetalert/sweetalert.min.js", "js");

            Syntaq.Busy.setBusy();
            //settings.ajax.url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize + "&skipcount=" + settings._iDisplayStart + "&Filter=" + $("#syntaq-search").val();

            var dataTable = jQuery('#' + Syntaq.Records.ElementId).DataTable({
                ajax: {
                    async: true,
                    crossDomain: true,
                    // URL is overridden in the preXhr.dt event
                    url: _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize,
                    method: "GET",
                    headers: {
                        Authorization: "bearer " + Syntaq.GetAuthToken(),
                        Accept: "*/*",
                        cache: false
                    },
                    dataFilter: function (data) {
                        var json = jQuery.parseJSON(data);
                        json.recordsTotal = json.result.totalCount;
                        json.recordsFiltered = json.result.totalCount;
                        json.data = json.result.items;
                        return JSON.stringify(json); // return JSON string
                    },
                    dataSrc: 'result.items'

                },
                drawCallback: function () {
                    jQuery('.dataTables_paginate > .pagination').find('li').addClass('page-item');
                    jQuery('.dataTables_paginate > .pagination > li').find('a').addClass('page-link');

                    Syntaq.Busy.clearBusy();

                },

                paging: true,
                serverSide: true,
                bFilter: false,
                bLengthChange: false,
                deferLoading: 0,
                createdRow: function (row, data, dataIndex) {


                    $(row).find("[name='loadformforrecordelink']").on("click", function () {
                        var ridHiden = Syntaq.Records.renderNewFormForRecodeModel('load');
                        var recordId = data.id !== null ? data.id : null;
                        if (ridHiden !== null) {
                            ridHiden.value = recordId;
                            Syntaq.Records.loadFormslist(recordId);
                        }
                    });

                    $(row).find("[name='Folder']").on("click", function () {
                        Syntaq.Records.FolderId = $(this).data('folderid');
                        Syntaq.Busy.setBusy();
                        getRecords();
                    });

                    $(row).find("[name='DeleteRecordLink']").on("click", function () {
                        //var recordId = data.id !== null ? data.id : null;

                        var recordId = $(this).data('id');
                        swal({
                            title: "Are you sure?",
                            icon: "warning",
                            buttons: true,
                            dangerMode: true
                        })
                            .then((confirmed) => {
                                if (confirmed) {
                                    abp.ajax({
                                        type: "DELETE",
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        contentType: 'application/json',
                                        url: _SyntaqBaseURI + '/api/services/app/Records/Delete?Id=' + recordId
                                    }).done(function (response) {
                                        Syntaq.Busy.setBusy();
                                        dataTable.ajax.reload();
                                        toastr.success(Syntaq.Form.DeletedMessage);
                                    });
                                }
                            });

                    });

                    $(row).find("[name='ArchiveRecordLink']").on("click", function () {
                   
                        var recordId = $(this).data('id');
                        swal({
                            title: "Are you sure?",
                            icon: "warning",
                            buttons: true,
                            dangerMode: true
                        })
                            .then((confirmed) => {
                                if (confirmed) {
                                    abp.ajax({
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        contentType: 'application/json',
                                        url: _SyntaqBaseURI + '/api/services/app/Records/Archive',
                                        type: "POST",
                                        data: JSON.stringify({ id: recordId }) 
                                    }).done(function (response) {
                                        Syntaq.Busy.setBusy();
                                        dataTable.ajax.reload();
                                        toastr.success(Syntaq.Form.ArchivedMessage);
                                    });
                                }
                            });

                    });

                    $(row).find("[name='DeleteRecordMatterLink']").on("click", function () {

                        var recordMatterId = $(this).data('id');
                        swal({
                            title: "Are you sure?",
                            icon: "warning",
                            buttons: true,
                            dangerMode: true
                        })
                            .then((confirmed) => {
                                if (confirmed) {

                                    abp.ajax({
                                        type: "DELETE",
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        contentType: 'application/json',
                                        url: _SyntaqBaseURI + '/api/services/app/RecordMatters/Delete?Id=' + recordMatterId
                                    }).done(function (response) {
                                        Syntaq.Busy.setBusy();
                                        dataTable.ajax.reload();
                                        toastr.success(Syntaq.Form.DeletedMessage);
                                    });

                                }
                            });

                    });

                    $(row).find("[name='DeleteFolderLink']").on("click", function () {

                        var folderID = $(this).data('id');

                        swal({
                            title: "Are you sure?",
                            icon: "warning",
                            buttons: true,
                            dangerMode: true
                        })
                            .then((confirmed) => {
                                if (confirmed) {

                                    abp.ajax({
                                        type: "DELETE",
                                        beforeSend: function (request) {
                                            request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                        },
                                        contentType: 'application/json',
                                        url: _SyntaqBaseURI + '/api/services/app/Folders/Delete?Id=' + folderID
                                    }).done(function (response) {
                                        if (response.success === true) {
                                            Syntaq.Busy.setBusy();
                                            dataTable.ajax.reload();
                                        }
                                        toastr.success(Syntaq.Form.DeletedMessage);
                                    });

                                }
                            });

                    });

                },
                columnDefs: [
                    {
                        targets: 0,
                        width: "200px",
                        data: "name",
                        render: function (data, type, row) {
                            if (row.type === 'Folder') {
                                data = '<a name="Folder"  data-folderid="' + row.id + '"  href=\'javascript:void(0);\'><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/folder.png" width=32  /> <strong class=\'h5\'>' + row.name + '</strong></a>';
                            } else {
                                var dt = new Date(row.lastModified);
                                var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                var tmoptions = { hour: 'numeric', minute: 'numeric' };
                                dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                                data = '<img src="' + _SyntaqBaseURI + '/Common/Images/Entities/binders-folder.png" width=45 height=45 style="position:relative; top:-5px;"/><strong class=h5>' + row.name + '</strong>';
                                data += '<br>' + dt + '</span>';
                                data += '<div class="ml-1">';
                                data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a data-id="' + row.id + '" class="OnClickLink cursor-pointer me-2 " name="DeleteRecordLink"><i class="fa fa-times text-danger"></i> Delete</a>' : '';
                                data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a data-id="' + row.id + '" class="OnClickLink cursor-pointer" name="ArchiveRecordLink"><i class="fa fa-archive text-warning"></i> Archive</a>' : '';                                
                                data += '</div>';
                            }
                            return data;
                        }
                    },
                    {
                        targets: 1,
                        data: null,
                        render: function (data, type, row) {
                            if (row.type === 'Folder') {
                                data = '<span class="pull-right mt-3 mr-5">';
                                data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a data-id="' + row.id + '" href=\'javascript:void(0);\' class="OnClickLink" name="DeleteFolderLink" ><i class="fa fa-times text-danger"></i> Delete Folder</a>' : '';
                                //data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'#\' onclick="Syntaq.Records.deleteFolder(\'' + row.id + '\')" class="OnClickLink" name="DeleteFolderLink" ><i class="fa fa-times text-danger"></i> Delete Folder</a>' : '';
                                data += '</span>';
                            } else {
                                data = '<div>';

                                if (row.recordMatters !== null) {
                                    var rmc = 0;
                                    row.recordMatters.forEach(function (recordmatter) {
                                        data += '<span style="width: 110px;"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/moleskine.png"  /> </span>';

                                        var dt = new Date(recordmatter.creationTime);
                                        var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                        dt = dt.toLocaleDateString('en-GB', options);

                                        data += '<span style="width: 350px;"><strong class=h5>' + recordmatter.recordMatterName + '</strong><span class="ml-3"> </span></span>';

                                        data += '<span class="pull-right mt-3 mr-5">';
                                        data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'javascript:void(0);\' data-id="' + recordmatter.id + '"  class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete Record Matter</a>' : '';
                                        
                                        data += '</span>';
                                        data += '<table class="ml-3 mb-2 m--bg-fill-light" style="width:95%">';

                                        if (recordmatter.recordMatterItems !== null) {
                                            recordmatter.recordMatterItems.forEach(function (recordmatteritem) {

                                                data += '<tr>';
                                                dt = new Date(recordmatteritem.creationTime);
                                                var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                                var tmoptions = { hour: 'numeric', minute: 'numeric' };
                                                dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);

                                                data += '<td class="ml-3 pt-1 pb-1 pl-1">';

                                                if (recordmatteritem.document === true) {
                                                    if (recordmatteritem.formId !== '00000000-0000-0000-0000-000000000000') {
                                                        if (recordmatteritem.lockOnBuild === true) {
                                                            data += '<img src="' + _SyntaqBaseURI + '/Common/Images/Entities/lock.png" height=34/></a>';
                                                        }
                                                        else {
                                                            data += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + row.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="javascript:void(0);"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/form.png" height=34/></a>';
                                                        }
                                                    }
                                                    if (recordmatteritem.allowPdf) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.pdf\', \'pdf\')" href="javascript:void(0);"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/pdf.png" /></a>';
                                                    }
                                                    if (recordmatteritem.allowWord) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.docx\', \'docx\')" href="javascript:void(0);" ><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/word.png" /></a>';
                                                    }
                                                    if (recordmatteritem.allowHTML) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.html\', \'html\')" href="javascript:void(0);"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '/Common/Images/Entities/html-filetype.png"  /><strong>';
                                                    }

                                                } else {
                                                    data += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + row.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="javascript:void(0);"><img src="' + _SyntaqBaseURI + '/Common/Images/Entities/form.png" height=34/></a>';
                                                }

                                                if (recordmatteritem.documentName === "" || recordmatteritem.documentName === null) {
                                                    var difference = Math.round((new Date() - new Date(recordmatteritem.creationTime)) / 60000);
                                                    data += difference < 5 ? 'Document still assembling' : 'Document Name not set';
                                                } else {
                                                    data += recordmatteritem.documentName;
                                                }

                                                data += '</td>';
                                                data += '<td class="pt-1 pb-1 w-25">' + dt + '</td>';

                                                data += '</tr>';
                                            });
                                        }
                                        data += '</table>';
                                        rmc = rmc + 1;
                                    });
                                    data += '</hr>';
                                    data += '</div>';
                                }
                            }
                            return data;
                        }
                    }
                ]
            }).on('preXhr.dt', function (e, settings, data) {

                var oBreadcrumbReq = new XMLHttpRequest();
                oBreadcrumbReq.onreadystatechange = function () {
                    if (this.readyState === 4 && this.status === 200) {

                        var jsonCrumbs = JSON.parse(oBreadcrumbReq.response);
                        //Syntaq.Records.loadBreadCrumbs(crumbsJSON);

                        var ul = document.getElementById('syntaq-breadcrumb-list');
                        ul.innerHTML = '';

                        for (i in jsonCrumbs.result) {
                            var li = document.createElement('li');
                            li.setAttribute('class', 'breadcrumb-item');
                            li.setAttribute('data-folderid', jsonCrumbs.result[i].id);

                            var a = document.createElement('a');
                            a.setAttribute('href', 'javascript:void(0);');
                            li.appendChild(a);
                            a.innerHTML = jsonCrumbs.result[i].name;

                            ul.appendChild(li);

                            $(li).on("click", function () {
                                Syntaq.Busy.setBusy();
                                Syntaq.Records.FolderId = $(this).data('folderid');
                                getRecords();
                            });

                        }

                        Syntaq.Busy.clearBusy();
                    }
                };
                oBreadcrumbReq.open("GET", _SyntaqBaseURI + "/api/services/app/Folders/GetBreadcrumbs?id=" + Syntaq.Records.FolderId + "&type=R");
                oBreadcrumbReq.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                oBreadcrumbReq.send();

                settings.ajax.url = _SyntaqBaseURI + "/api/services/app/Records/GetAll?id=" + Syntaq.Records.FolderId + "&maxResultCount=" + Syntaq.Records.PageSize + "&skipcount=" + settings._iDisplayStart;


            });

            function getRecords() {
                //Syntaq.Busy.setBusy();
                dataTable.ajax.reload();
            }
            getRecords();
        }
    }
};

function setToastrOptions() {
    toastr.options = {
        "debug": false,
        "positionClass": "toast-bottom-right",
        "onclick": null,
        "fadeIn": 300,
        "fadeOut": 1000,
        "timeOut": 5000,
        "extendedTimeOut": 1000
    };
}
function saveBlob(blob, fileName) {
    var a = document.createElement('a');
    a.href = window.URL.createObjectURL(blob);
    a.download = fileName;
    a.dispatchEvent(new MouseEvent('click'));
}
function loadjscssfile(filename, filetype) {
    var fileref;
    if (filetype === "js") { //if filename is a external JavaScript file
        fileref = document.createElement('script');
        // fileref.async = false;
        fileref.setAttribute("type", "text/javascript");
        fileref.setAttribute("src", filename);
        // fileref.defer = true;
        document.getElementsByTagName('head')[0].appendChild(fileref);
    }
    else if (filetype === "script") { //if filename is an external Script file
        fileref = document.createElement("script");
        fileref.setAttribute("type", "text/javascript");
        fileref.innerHTML = filename;
        document.getElementsByTagName('head')[0].appendChild(fileref);
    }
    else if (filetype === "css") { //if filename is an external CSS file
        fileref = document.createElement("link");
        fileref.setAttribute("rel", "stylesheet");
        fileref.setAttribute("type", "text/css");
        fileref.setAttribute("scoped", "");
        fileref.setAttribute("href", filename);
        document.getElementsByTagName('head')[0].appendChild(fileref);
    }
    else if (filetype === "cssinline") { //if filename is an external CSS file
        fileref = document.createElement("style");
        fileref.setAttribute("scoped", "");
        /*fileref.setAttribute("type", "text/javascript");*/
        fileref.innerHTML = filename;
        document.getElementById("syntaq-content").appendChild(fileref);
    }
    else if (filetype === "cssinhead") { //if filename is an external CSS file and needs to load in head
        fileref = document.createElement("link");
        fileref.setAttribute("rel", "stylesheet");
        fileref.setAttribute("type", "text/css");
        fileref.setAttribute("scoped", "");
        fileref.setAttribute("href", filename);
        document.getElementsByTagName('head')[0].appendChild(fileref);
    }

}
function getComponent(parent, key) {

    var component = null;

    if (typeof parent !== "undefined" && parent !== null) {
        if (parent.components === undefined || parent.components === null) {
            var rptname = '';
            for (var itm in parent) {
                rptname = itm;
            }
            parent = parent[rptname];
        }

        parent.components.forEach(function (comp) {
            if (comp.component.key === key) {
                component = comp;
            }
        });
    }

    return component;
}
function getDataValue(instance, field, tolower = true) {

    result = '';
    if (instance !== null) {

        if (instance.data !== null && instance.data !== undefined && instance.component !== undefined) {
            var data = instance.data;
            if (data[field] !== null && data[field] !== undefined) {
                result = data[field];
                if (isDate(instance, result) && instance.key === field) {
                    if (result !== '') {
                        result = new Date(result);
                        result = result.toDateString();
                    }
                }
                else if (typeof result === 'string' || result instanceof String || typeof result === 'boolean' || result instanceof Boolean) {
                    result = result.toString();
                    if (tolower) result = result.toLowerCase();
                }
                else if (typeof result === 'number' || result instanceof Number) {
                    result = result;
                }

                else {
                    // Field may be a JSON object if a RadioGroup/ Checkboxgroup / YN
                    // Drill down until you get the selected value for the control
                    if (result[field] !== null && result[field] !== undefined) {
                        result = result[field];
                        if (typeof result === 'string' || result instanceof String) {
                            result = result.toString();
                            if (tolower) result = result.toLowerCase();
                        }
                    }
                    else if (result['value'] !== null && result['value'] !== undefined) {
                        result = result['value'];
                        if (typeof result === 'string' || result instanceof String) {
                            result = result.toString();
                            if (tolower) result = result.toLowerCase();
                        }
                    }
                    else {
                        if (instance.component.type === 'person') {
                            var salcho = result['Sal_cho'];
                            result = result['Name_Full_scr'];
                            if (salcho !== '') {
                                result = salcho + ' ' + result;
                            }
                            return result;
                        }
                        else if (instance.component.type === 'addressgroup') {
                            var result = result['Addr_txt'];
                            return result;
                        }

                        result = '';
                    }
                }

            }
            else {

                if (instance.parent !== null && instance.parent !== undefined) {
                    result = getDataValue(instance.parent, field);
                }
                else {
                    result = '';
                }

            }
        }
    }


    // Always return lowercase to make condition checking easy
    return result;

}

function isDate(instance, result) {

    if (instance.component) {
        if (instance.component.type === 'sfadatetime') {

            if (result !== '') {
                result = new Date(result);
                result = result.toDateString();
                if (result === 'Invalid Date') {
                    return false;
                }
            }

            return true;
        }
    }

    return false;
}
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return '';
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function LoopThroughQueryString(JSONData) {

    //loops through queryString
    var queryString = unescape(location.search);

    //if no querystring (homepage etc.) then exit
    if (!queryString) {
        return JSONData;
    }

    //remove the ?
    queryString = queryString.substring(1);

    //split querystring into key/value pairs
    var pairs = queryString.split("&");

    //load the pairs into a collection
    for (var i = 0; i < pairs.length; i++) {
        //split into key/value pair by splitting on =
        var keyValuePair = pairs[i].split("=");

        if (keyValuePair.length > 1) {
            JSONData[keyValuePair[0]] = keyValuePair[1];
        }
    }
    return JSONData;
}
function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};
function UpdateFormLogic(parent, target, type, condition, value) {
    UpdateSchemaLogic(parent, target, type, condition, value);
    return parent;
};
function UpdateSchemaLogic(parent, target, type, condition, value) {

    jQuery(parent.components).each(function (item, component) {
        if (component.key === target) {
            if (type !== "Hidden") {
                component.logic = typeof (component.logic) === 'undefined' ? [] : component.logic;
                var i = (component.logic.length > 0 ? component.logic.length : 0);
                component.logic[i] = {
                    name: 'logic_' + Math.round(new Date().getTime() + (Math.random() * 100)),
                    trigger: {
                        type: 'javascript',
                        javascript: condition
                    },
                    actions: [
                        {
                            name: 'action_' + Math.round(new Date().getTime() + (Math.random() * 100)),
                            type: "property"
                        }
                    ]
                };
            }
            switch (type) {
                case "Hidden":
                    component.customConditional = condition;
                    break;
                case "Required":
                    component.logic[i].actions[0].property = {
                        label: "Required",
                        type: "boolean",
                        value: "validate.required"
                    };
                    component.logic[i].actions[0].state = value;
                    break;
                case "Disabled":
                    component.logic[i].actions[0].property = {
                        label: "Disabled",
                        type: "boolean",
                        value: "disabled"
                    };
                    component.logic[i].actions[0].state = value;
                    break;
                case "Label":
                    component.logic[i].actions[0].property = {
                        label: "Label",
                        type: "string",
                        value: "label"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "Title":
                    component.logic[i].actions[0].property = {
                        label: "Title",
                        type: "string",
                        value: "title"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "Tooltip":
                    component.logic[i].actions[0].property = {
                        label: "Tooltip",
                        type: "string",
                        value: "tooltip"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "Description":
                    component.logic[i].actions[0].property = {
                        label: "Description",
                        type: "string",
                        value: "description"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "Placeholder":
                    component.logic[i].actions[0].property = {
                        label: "Placeholder",
                        type: "string",
                        value: "placeholder"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "CSS Class":
                    component.logic[i].actions[0].property = {
                        label: "CSS Class",
                        type: "string",
                        value: "className"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "Container Custom Class":
                    component.logic[i].actions[0].property = {
                        label: "Container Custom Class",
                        type: "string",
                        value: "customClass"
                    };
                    component.logic[i].actions[0].text = value;
                    break;
                case "SetValue":
                    component.logic[i].actions[0] = {
                        name: 'action_' + Math.round(new Date().getTime() + (Math.random() * 100)),
                        type: "value",
                        value: (component.type === 'sfacheckbox' && value.toLowerCase() === 'false') ? "value = false" : "value = '" + value + "'"
                    };
                    break;
            };

            parent.components[item] = component;
        }

        if (component.type === "section") {

            //path = path == null ? formJSON.components[item]
            jQuery(component.components).each(function (label, object) {

                //path = path == null ? 'components[' + item + ']' : path + '.components[' + item + ']';
                parent.components[item].components[0] = UpdateSchemaLogic(object, target, type, condition, value/*, path*/)
            });
        } else if (component.type === "sfapanel" || component.type === "panel") {

            //path += 
            jQuery(component).each(function (label, object) {

                //path = path == null ? 'components[' + item + ']' : path + '.components[' + item + ']';
                parent.components[item] = UpdateSchemaLogic(object, target, type, condition, value/*, path*/)
            });
        }

    });

    return parent;
}
function setValFrm(parent, field, value) {

    if (parent.components) {

        jQuery.each(parent.components, function (label, object) {

            if (object.component.type === "section") {
                jQuery(object.components).each(function (label, object) {
                    setValFrm(object, field, value);
                });
            } else if (object.component.type === "sfapanel" || object.component.type === "panel") {
                jQuery(object.components).each(function (label, item) {
                    setValFrm(object, field, value);
                });
            }

            if (object.component.key === field) {
                if (object.component.type === "checkboxesgroup" || object.component.type === "sfacheckbox") {
                    var values = value.split("|");
                    value = {};
                    jQuery(values).each(function (label, item) {
                        value['' + item.toLowerCase() + ''] = true;
                    });
                }
                if (object.component.type === "heading" || object.component.type === "label") {
                    object.component.heading = value;
                    object.component.labelvalue = value;
                }
                object.data['' + field + ''] = value;
            }
        });
    }
}
function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
function transferData(submission, PopName) {
    var currentFormKeys = {};
    FormioUtils.eachComponent(Form.components, function (comp) {
        currentFormKeys[comp.key] = true;
    }, true);
    for (var key in currentFormKeys) {
        if (!submission.hasOwnProperty(key)) {
            submission[key] = null;
        }
    }

    opener.postMessage({ command: 'ReturnData', popName: PopName, data: submission }, '*');

    if (closepopup) window.close();
    return false;
}
window.addEventListener("message", function (e) {
    if (e.data.command === 'ReturnData') {
        dataReceiver(e.data.data, e.data.popName);
    }
}, false);
function dataReceiver(result, PopName) {
    var currentFormKeys = {};
    FormioUtils.eachComponent(Form.components, function (comp) {
        currentFormKeys[comp.key] = true;
    }, true);
    for (var key in currentFormKeys) {
        if (result.hasOwnProperty(key)) {
            delete result[key];
        }
    }
    document.getElementsByName(PopName)[0].value = JSON.stringify(result);
    Form.updateValue({
        modified: true
    });
    Form.redraw();
}
function checkpopformValid() {
    return Form.checkValidity(Form.data, true);
}
"use strict";
var Modal = function (el, options) {
    var self = this;

    this.el = document.querySelector(el);
    this.options = options;

    try {
        var list = document.querySelectorAll('#' + this.el.id + ' [data-bs-dismiss="modal"]');
        for (var x = 0; x < list.length; x++) {
            list[x].addEventListener('click', function (e) {
                if (e) e.preventDefault();
                self.hide();
            });
        }
    }
    catch (e) {
        console.log(e);
    }
};
Modal.prototype.show = function () {
    var self = this;
    // adding backdrop (transparent background) behind the modal
    if (self.options.backdrop) {
        var backdrop = document.getElementById('bs.backdrop');
        if (backdrop === null) {
            backdrop = document.createElement('div');
            backdrop.id = "bs.backdrop";
            backdrop.className = "modal-backdrop fade in";
            document.body.appendChild(backdrop);
        }
    }

    // show modal
    this.el.classList.add('in');
    this.el.style.display = 'block';
    document.body.style.paddingRight = '13px';
    document.body.classList.add('modal-open');
};
Modal.prototype.hide = function () {
    var self = this;
    // hide modal
    this.el.classList.remove('in');
    this.el.style.display = 'none';
    //document.body.style = '';
    document.body.classList.remove('modal-open');

    // removing backdrop
    if (self.options.backdrop) {
        var backdrop = document.getElementById('bs.backdrop');
        if (backdrop !== null) document.body.removeChild(backdrop);
    }
};


var userProfiletemplate = `

              <div class="w3-row">
                <a href="javascript:void(0);"  data-tab="user-tab-1">
                  <div class="w3-third tablink w3-bottombar w3-hover-light-grey w3-border-red w3-padding">User Information</div>
                </a>
                <a href="javascript:void(0);"  data-tab="user-tab-2">
                  <div class="w3-third tablink w3-bottombar w3-hover-light-grey w3-padding">Entity Information</div>
                </a>
                 <a href="javascript:void(0);"  data-tab="user-tab-3">
                  <div class="w3-third tablink w3-bottombar w3-hover-light-grey w3-padding">Logo</div>
                </a>
              </div>

                <form id="UserInformationsForm" role="form" novalidate="novalidate" class="form-validation" _lpchecked="1">

                <div id="user-tab-1" class="w3-row pt-4 user-info">

                    <input type="hidden" name="id" value="{{id}}">
                    <input type="hidden" name="userName" value="{{userName}}">
                    <div class="row">

                        <div class="form-group col-md-2">
                            <label>Title</label>
                            <input class="form-control" value="{{title}}" type="text" name="title" autocomplete="off" style="background-image: url(\"{{profilepic}}\"); cursor: auto;">
                        </div>

                        <div class="form-group col-md-4">
                            <label>Name</label>
                            <input class="form-control" value="{{name}}" type="text" name="name" required="" maxlength="64">
                        </div>
                        <div class="form-group col-md-4">
                            <label>Surname</label>
                            <input type="text" name="surname" class="form-control" value="{{surname}}" required="" maxlength="64">
                        </div>

                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Job Title</label>
                            <input type="text" name="jobTitle" class="form-control" value="{{jobTitle}}">
                        </div>

                        <div class="form-group col-md-6">
                            <label>Email address</label>
                            <input type="email" name="emailAddress" class="form-control" value="{{emailAddress}}" required="" maxlength="256">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Mobile Phone number</label>
                            <input type="text" name="phoneNumberMobile" class="form-control" value="{{phoneNumberMobile}}" maxlength="24">
                        </div>

                        <div class="form-group col-md-6">
                            <label>Phone number</label>
                            <input type="text" name="phoneNumber" class="form-control" value="{{phoneNumber}}" maxlength="24">
                        </div>
                    </div>

                    <h5>Address</h5>
                    <hr>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Care Of</label>
                            <input type="text" name="addressCO" class="form-control" value="{{addressCO}}">
                        </div>

                        <div class="form-group col-md-6">
                            <label>Bldg, Floor, Lvl</label>
                            <input type="text" name="addressLine1" class="form-control" value="{{addressLine1}}">
                        </div>
                    </div>
                    <div class="form-group">
                        <label>Street No. and Name</label>
                        <input type="text" name="addressLine2" class="form-control" value="{{addressLine2}}">
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Postcode</label>
                            <input type="text" name="addressPostCode" class="form-control" value="{{addressLinePostCode}}">
                        </div>

                        <div class="form-group col-md-3">
                            <label>State</label>
                            <input type="text" name="addressState" class="form-control" value="{{addressState}}">
                        </div>

                        <div class="form-group col-md-3">
                            <label>Suburb / City</label>
                            <input type="text" name="addressSub" class="form-control" value="{{addressSub}}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Country</label>
                            <input type="text" name="addressCountry" class="form-control" value="{{addressCountry}}">
                        </div>
                    </div>
           
                    <span name='Syntaq-User-Submit' class='btn btn-primary'>Save</span>
                    </div>

                </div>

                <div id="user-tab-2" class="w3-row user-info pt-4" style="display:none">

                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="Entity">Company / Entity Name</label>
                            <input class="form-control" type="text" name="Entity"  value="{{entity}}">
                        </div>
                        <div class="form-group col-md-6">
                            <label for="ABN">Company / Business Number</label>
                            <input class="form-control edited" type="text" name="ABN" pattern="^[0-9]+$" minlength="11" maxlength="11"   value="{{abn}}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="EmailAddressWork">Work Email address</label>
                            <input class="form-control" type="text" name="EmailAddressWork" maxlength="256" value="{{emailAddressWork}}">
                        </div>
                        <div class="form-group col-md-6">
                            <label>Work Phone number </label>
                            <input type="text" name="PhoneNumberWork" class="form-control edited" maxlength="24"  value="{{phoneNumberWork}}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label for="Fax">Fax</label>
                            <input class="form-control" type="text" name="Fax" value="{{fax}}">
                        </div>
                        <div class="form-group col-md-6">
                            <label for="WebsiteURL">URL</label>
                            <input class="form-control edited" type="text" name="WebsiteURL" value="{{websiteURL}}">
                        </div>
                    </div>
                    <h5>Address</h5>
                    <hr>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Care Of</label>
                            <input type="text" name="EntityAddressCO" class="form-control" value="{{entityAddressCO}}">
                        </div>

                        <div class="form-group col-md-6">
                            <label>Bldg, Floor, Lvl</label>
                            <input type="text" name="EntityAddressLine1" class="form-control"  value="{{entityAddressLine1}}">
                        </div>
                    </div>
                    <div class="form-group">
                        <label>Street No. and Name</label>
                        <input type="text" name="EntityAddressLine2" class="form-control"  value="{{entityAddressLine2}}">
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Postcode</label>
                            <input type="text" name="EntityAddressPostCode" class="form-control"  value="{{entityAddressPostCode}}">
                        </div>
                        <div class="form-group col-md-3">
                            <label>State</label>
                            <input type="text" name="EntityAddressState" class="form-control"  value="{{entityAddressState}}">
                        </div>
                        <div class="form-group col-md-3">
                            <label>Suburb / City</label>
                            <input type="text" name="EntityAddressSub" class="form-control"  value="{{entityAddressSub}}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-md-6">
                            <label>Country</label>
                            <input type="text" name="EntityAddressCountry" class="form-control"  value="{{entityAddressCountry}}">
                        </div>
                    </div>

                    <span name='Syntaq-User-Submit' class='btn btn-primary'>Save</span>

                </div>
                </form>

                <div id="user-tab-3" class="w3-row user-info pt-4" style="display:none">
                    <form id="ChangeLogoPictureForm" method="POST" >
                        <input type="hidden" name="id" value="{{id}}">
                        <div class="form-group">
                            <input type="file" name="LogoPicture" id="syntaq-logo-upload" />              
                        </div>
                    </form>
                    <img id="LogoPictureResize" src="" />
                </div>

`;

function addTitleToCompInput(parent) {
    parent.components.forEach(function (comp) {
        var label = "";
        if (comp.error == null && comp.label != undefined && comp.label != null) {
            label += comp.label;
        }
        // 44 - Aria label not descriptive
        //$("input[name*='" + comp.key + "']").attr({ title: label + " " + "Input", 'aria-label': label + " " + "Input" });
        $("input[name*='" + comp.key + "']").attr({ 'label': label });

        if (comp.type === 'addressgroup') {
            $("input[ID*='Care']").attr({ "title": label + " " + "Care", 'aria-label': label + " " + "Care" });
            $("input[ID*='Lvl']").attr({ "title": label + " " + "Lvl", 'aria-label': label + " " + "Lvl" });
            $("input[ID*='City']").attr({ "title": label + " " + "City", 'aria-label': label + " " + "City" });
            $("input[ID*='Street']").attr({ "title": label + " " + "Street", 'aria-label': label + " " + "Street" });
            $("input[ID*='Suburb']").attr({ "title": label + " " + "Suburb", 'aria-label': label + " " + "Suburb" });
            $("input[ID*='State']").attr({ "title": label + " " + "State", 'aria-label': label + " " + "State" });
            $("input[ID*='PostCode']").attr({ "title": label + " " + "PostCode", 'aria-label': label + " " + "PostCode" });
            $("input[ID*='FullAddress']").attr({ "title": label + " " + "FullAddress", 'aria-label': label + " " + "FullAddress" });
            $("select[ID*='Country']").attr({ "title": label + " " + "Country", 'aria-label': label + " " + "Country" });
        }
        if (comp.type === 'person') {
            $("label:contains('Title')").siblings('div').children('select').attr({ "title": label + " " + "Title", 'aria-label': label + " " + "Title" });
            $("label:contains('First Name')").siblings('div').children('input').attr({ "title": label + " " + "First Name", 'aria-label': label + " " + "First Name" });
            $("label:contains('Middle Name')").siblings('div').children('input').attr({ "title": label + " " + "Middle Name", 'aria-label': label + " " + "Middle Name" });
            $("label:contains('Last Name')").siblings('div').children('input').attr({ "title": label + " " + "Last Name", 'aria-label': label + " " + "Last Name" });

        }
        $("textarea[name*='" + comp.key + "']").attr({ "title": label + " " + "Input", 'aria-label': label + " " + "Input" });
        $("input[type=file]").attr({ "title": label + " " + "File Upload", 'aria-label': label + " " + "File Upload" });
        if (comp.type === 'sfanzbn') {
            $(".formio-component-sfanzbn > div > div > div > div").children('input').attr({ "title": label + " " + "Input", 'aria-label': label + " " + "Input" });
        }
    });
}
var importantFields = [];
function importantComp(parent) {
    parent.components.forEach(function (comp) {
        var type = comp.component.type;
        if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
            importantComp(comp);
        } else {
            if (comp.component.important == true) {
                importantFields.push(comp.key);
            }
        }
    });
}

function customizedImportantFlag(parent) {
    parent.components.forEach(function (comp) {
        var type = comp.component.type;
        if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
            customizedImportantFlag(comp);
        } else {

            if (importantFields.includes(comp.key)) {
                if (comp.component.important == true && !$('.formio-component-' + comp.key).is(':visible')) {
                    comp.component.important = false;

                }
                if (comp.component.important == false && $('.formio-component-' + comp.key).is(':visible')) {
                    comp.component.important = true;
                }
            }
        }
    });
}

var isFormValid = true;
function checkFormIsvalid(parent) {
    parent.components.forEach(function (comp) {
        if (comp._visible) {
            var isvalid = checkFieldIsvalid(comp);
            if (!isvalid) {
                //if (comp.error !== null && comp.error !== '') {
                isFormValid = false;
                return;
            }
            if (comp.components !== undefined) {
                checkFormIsvalid(comp);
            }
        }
    });
}

function checkFieldIsvalid(comp) {
    if (comp.type === 'addressgroup') {
        if (comp.error !== null) {
            return false;
        }
        // addressgroup component's build in method.
        return comp.validateRequired();
    }
    if (comp.type === 'person') {
        // if the component is hidden
        if (comp.component.hidden) {
            comp.visible = false;
        }
        // person component's build in method.
        return comp.validateRequired();
    }

    if (comp.component.validate) {
        if (comp.component.validate.required) {
            var val = getDataValue(comp, comp.component.key);
            if (val === '') {
                return false;
            }
        }

        if (comp.component.validate.minlength) {
            var val = getDataValue(comp, comp.component.key);
            if (val.length < comp.component.validate.minlength) {
                return false;
            }
        }

        if (comp.component.validate.maxlength) {
            var val = getDataValue(comp, comp.component.key);
            if (val.length > comp.component.validate.maxlength) {
                return false;
            }
        }

        if (comp.component.validate.pattern) {

            var val = getDataValue(comp, comp.component.key);
            var re = new RegExp(comp.component.validate.pattern);
            if (!re.test(val)) {
                return false;
            }
        }
    }

    return true;

}

function initialFeedbackFunction(feedbackformId) {

    Syntaq.Form.FeedbackFormId = feedbackformId;

    if (feedbackformId !== null && feedbackformId != undefined) {

        var feedbackformurl = _SyntaqBaseURI + "/api/services/app/Forms/GetFormForView?OriginalId=" + feedbackformId + "&version=Live&AccessToken=";

        abp.ajax({
            type: "GET",
            contentType: 'application/json',
            url: feedbackformurl
        }).done(function (response) {

            if (response !== undefined && response !== null) {

                feedbackFormSchema = JSON.parse(response.form.schema);
                if (!feedbackFormSchema.hasOwnProperty("components")) {
                    feedbackFormSchema.components = [];
                }
                // if Anonymly, require input Name and Email.
                if (Syntaq.Form.Anonymous) {
                    //push Email
                    feedbackFormSchema.components.unshift({ "key": "senderEmail", "label": "Email", "showSummary": false, "defaultValue": "", "DoNotLoadFromRecord": false, "widthslider": "8", "offsetslider": "0", "type": "sfaemail", "input": true, "tableView": true, "logic": [], "validate": { "required": true, "customMessage": "" } });
                    //push UserName
                    feedbackFormSchema.components.unshift({ "key": "senderUserName", "label": "Name", "showSummary": false, "DoNotLoadFromRecord": false, "showWordCount": false, "showCharCount": false, "widthslider": "4", "offsetslider": "0", "type": "sfatextfield", "input": true, "tableView": true, "logic": [], "validate": { "required": true, "customMessage": "" }, "inputFormat": "plain" });
                }
                // If doesn't contain "Rating_cho", added this component
                if (!response.form.schema.includes('"key":"Rating_cho"')) {
                    feedbackFormSchema.components.push({ "key": "Rating_cho", "label": "Rating", "showSummary": false, "defaultValue": "5", "DoNotLoadFromRecord": false, "maxValue": 5, "widthslider": "12", "offsetslider": "0", "minValue": 1, "type": "slider", "input": true, "tableView": true, "logic": [] });
                }
                // If doesn't contain "Comment_txt", added this component
                if (!response.form.schema.includes('"key":"Comment_txt"')) {
                    feedbackFormSchema.components.push({ "key": "Comment_txt", "label": "Comment", "showSummary": false, "defaultValue": "", "DoNotLoadFromRecord": false, "showWordCount": false, "showCharCount": false, "widthslider": "12", "offsetslider": "0", "rows": 5, "type": "sfatextarea", "input": true, "tableView": true, "logic": [] });
                }
                var fbMainContainer = document.createElement("div");
                fbMainContainer.id = "feedback-main";
                var fbDiV = '<div id="feedback-div" class="modal-content"><div class="feedback-header"><a id="feedbackClose" class="close feedbackClose">X</a></div><div id="feedback-content"></div></div>';
                fbMainContainer.innerHTML = fbDiV.trim();
                document.body.appendChild(fbMainContainer);

                loadFeedbackButton();
            }
        });
    }
}

/**
With 0 reference handleErrorLinkClick, this function is used to handle page required field error's and this the main code modification of 10728. 
This Function is called directly when the error is being clicked from the top of the form. 
 */
function handleErrorLinkClick() {
    try {
        // Get the target element's ID (assuming it's in the format "id_error")
        const targetId = event.target.id.replace('_error', '');
        let index = -1;
        if (this.Form.pages !== undefined && this.Form.pages.length > 0) {
            // Check if the target field is on a different page
            outerLoop:
            for (let pageIndex = 0; pageIndex < this.Form.pages.length; pageIndex++) {
                const page = this.Form.pages[pageIndex];

                if (page.type === 'sfapanel' || page.type === 'section' || page.type === 'sectionpanel') {
                    for (const component of page.components) {
                        if (component.type === 'sfapanel' || component.type === 'section' || component.type === 'sectionpanel') {
                            for (const comp of component.components) {
                                if (comp.type === 'sfapanel' || comp.type === 'section' || comp.type === 'sectionpanel') {
                                    for (const comp1 of comp.components) {
                                        if (comp1.type === 'sfapanel' || comp1.type === 'section' || comp1.type === 'sectionpanel') {
                                            if (comp1.components.some(c => c.id === targetId)) {
                                                index = pageIndex;
                                                break outerLoop; // Exit all loops
                                            }
                                        } else if (comp1.id === targetId) {
                                            index = pageIndex;
                                            break outerLoop; // Exit all loops
                                        }
                                    }
                                } else if (comp.id === targetId) {
                                    index = pageIndex;
                                    break outerLoop; // Exit all loops
                                }
                            }
                        } else if (component.id === targetId) {
                            index = pageIndex;
                            break outerLoop; // Exit all loops
                        }
                    }
                } else if (page.components.some(component => component.id === targetId)) {
                    index = pageIndex;
                    break; // Only exit the innermost loop
                }
            }

            if (index !== -1) {
                // Switch to the target page if the error is on the current page the scroll and not setpage value else target to the page and scroll to the error element 

                if (index === this.Form.page) {
                    const targetElement = document.getElementById(targetId);
                    // Determine the type of the form element
                    const formElement = targetElement.querySelector('input, select, textarea');
                    if (formElement) {
                        formElement.focus();
                        // Scroll to the form element
                        formElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    }
                }
                else {
                    if (this.Form.data.FormErrors === undefined) {
                        //this.Form.data.FormErrors = this.Form.errors;
                        this.Form.data.FormErrors = this.Form.TotalErrorElements;
                    }
                    else {
                        this.Form.data.FormErrors = this.Form.data.FormErrors.concat(this.Form.errors);
                        this.Form.TotalErrorElements = this.Form.data.FormErrors;
                    }
                    this.Form.data.TargetId = targetId;
                    this.Form.setPage(index);
                }
            }
        }
        else {
            // Find the target element by its ID
            const targetElement = document.getElementById(targetId);
            // Determine the type of the form element
            const formElement = targetElement.querySelector('input, select, textarea');

            if (formElement) {
                formElement.focus();

                // Scroll to the form element
                formElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    }
    catch (e) {
        //ignore
    }
}
