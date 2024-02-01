// JavaScript source code

var parser = document.createElement('a');
parser.href = document.getElementById('syntaq-script').src;
var _SyntaqBaseURI = parser.protocol + "//" + parser.host;

loadjscssfile(_SyntaqBaseURI + "/assets/jquery-form/jquery.form.js", "js");
loadjscssfile("https://js.stripe.com/v3/", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/handlebars/handlebars.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/formio/dist/formio.full.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/toastr/build/toastr.min.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/spin.js/spin.js", "js");
loadjscssfile(_SyntaqBaseURI + "/AbpServiceProxies/GetAll?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/abp-web-resources/Abp/Framework/scripts/abp.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/lib/abp-web-resources/Abp/Framework/scripts/libs/abp.jquery.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/AbpScripts/GetScripts?v=636936179744097465?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI + "/view-resources/Areas/Falcon/Views/_Bundles/datatables-all.min.js?v=" + Math.floor(Math.random() * 10) + "", "js");
loadjscssfile(_SyntaqBaseURI +"/lib/sweetalert/dist/sweetalert.min.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/formio/dist/formio.full.css", "cssinhead");
loadjscssfile(_SyntaqBaseURI + "/assets/sweetalert/sweetalert.min.js", "js");
loadjscssfile(_SyntaqBaseURI + "/assets/w3/w3.css", "cssinhead");

var Form = { data: {} }; // main form object
var FormPages = {}; // array to track the validity of pages across a wizard

var body = document.getElementById('syntaq-content');

//--------------find user location
window.regionSyntaq = "Unknown";

function getUserPlace() {
    fetch("https://extreme-ip-lookup.com/json/")
        .then((res) => res.json())
        .then((response) => {
            window.regionSyntaq = response.countryCode;
            console.log(window.regionSyntaq);
        })
        .catch((data, status) => {
            console.log("Request failed");
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
                url: Syntaq.BaseUrl + '/Account/Register?view=/Views/Account/Register.min.cshtml',
                success: function (data) {
                    var formcontent = document.getElementById(Syntaq.Account.RegistrationElementId);
                    formcontent.innerHTML = data;
                }
            });
            jQuery.ajax({
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
                    alert(errorThrown + error);
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
        parseJwt: function(token) {
            var base64Url = token.split('.')[1];
            var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        }
    },
    Form: {
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
        createForm: function () {  // Method which will display type of animal

            //abp.appPath = _SyntaqBaseURI;
            var anonAuthToken = getUrlParameter('AccessToken');
            Syntaq.Form.AnonAuthToken = anonAuthToken;
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
            var formurl = _SyntaqBaseURI + "/api/services/app/Forms/GetFormForView?OriginalId=" + Syntaq.Form.FormId + "&version=" + Syntaq.Form.Version + "&AccessToken=" + AccessToken + "&RecordMatterId=" + Syntaq.Form.RecordMatterId;

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
                contentType: 'application/json',
                url: formurl
            }).done(function (response) {

                var formOBJ = response.form;
                $('#FormName').text(formOBJ.name);

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

                var btncontent = '<div id="contributor-content" class="list-inline kt-container kt-container--fluid pl-0 ml-0 mt-2" style="clear:both;"><div id="contributor-btn-content"></div></div>';

                var btndraft = '<div class="list-inline-item pull-right"><button id="btn-contributor-draft" data-title="Draft" data-status="3" class="btn btn-primary btn-wizard-nav-cancel btn-draft">' + app.localize('Draft') + '</button></div>';
                var btndone = '<div class="list-inline-item pull-right"><button id="btn-done" data-title="Done" data-status="3" class="btn btn-success   btn-wizard-nav-cancel btn-done">' + app.localize('Done') + '</button></div>';

                var btnsubmit = '<div class="list-inline-item pull-right mr-2"><button id="btn-contributor-submit" data-title="" data-status="" class="btn btn-primary btn-wizard-nav-cancel btn-submit">' + app.localize('Submit') + '</button></div>';
                var btnsaveandclose = '<div class="list-inline-item pull-right mr-2"><button id="btn-contributor-saveclose" data-title="" data-status="" class="btn btn-primary btn-wizard-nav-cancel btn-saveclose">' + app.localize('Save and Close') + '</button></div>';

                var btnpublish = '<div class="list-inline-item pull-right"><button id="btn-contributor-publish" data-title="Publish" data-status="3" class="btn btn-success  btn-wizard-nav-cancel btn-publish">' + app.localize('Publish') + '</button></div>';
                var btnfinalise = '<div class="list-inline-item pull-right "><button id="btn-contributor-finalise" data-title="Finalise" data-status="3" class="btn btn-success  btn-wizard-nav-cancel btn-finalise">' + app.localize('Finalise') + '</button></div>';

                var btnendorse = '<div class="list-inline-item pull-right  ml-2"><button id="btn-contributor-endorse" data-title="Submit Review" data-status="3" class="btn btn-success  btn-wizard-nav-cancel btn-contributor">' + app.localize('Submit Review') + '</button></div>';
                var btnapprove = '<div class="list-inline-item pull-right  ml-2"><button id="btn-contributor-approve" data-title="Approve" data-status="3" class="btn btn-success   btn-wizard-nav-cancel btn-contributor">' + app.localize('Approve') + '</button></div>';
                var btnreject = '<div class="list-inline-item pull-right"><button id="btn-contributor-reject" data-title="Decline" data-status="1" class="btn btn-danger  btn-wizard-nav-cancel btn-contributor">' + app.localize('Decline') + '</button></div>';
                var btncancel = '<div class="list-inline-item pull-right"><button id="btn-contributor-cancel" data-title="Cancel Approval"  data-status="2" class="btn btn-danger btn-wizard-nav-cancel btn-contributor">' + app.localize('Cancel') + '</button></div>';
                var btncomments = '<div class="list-inline-item pull-right"><button id="btn-contributor-comments" data-title="Comments" class="btn btn-primary btn-wizard-nav-comments btn-contributor-comments">' + app.localize('Comments') + '</button></div>';

                var btncomplete = '<div class="list-inline-item pull-right ml-2"><button id="btn-contributor-complete" data-title="Complete" data-status="3" class="btn btn-success btn-wizard-nav-cancel btn-contributor">' + app.localize('Complete') + '</button></div>';

                var spanmessage = '<div class="list-inline-item h5"><span class="label kt-badge kt-badge--info kt-badge--inline pull-right"> ' + contributor.message + '</span></div>';

                var btninvite = '<button id="btn-invite" type="button" class="mr-2 btn btn-primary  pull-right"  data-status="2">' + app.localize('Share') + '</button>';

                $('#syntaq-contributor-content').append(btncontent);

                if (contributor.recordMatterContributor.stepRole === 0) {
                    $('#contributor-btn-content').append(btnendorse);
                    $('#contributor-btn-content').append(btnreject);
                }

                if (contributor.recordMatterContributor.stepRole === 1) {
                    $('#contributor-btn-content').append(btnapprove);
                    $('#contributor-btn-content').append(btnreject);
                }

                if (contributor.recordMatterContributor.stepRole === 2) {
                    $('#contributor-btn-content').append(btncomplete);
                    $('#contributor-btn-content').append(btnreject);
                }

                if (contributor.recordMatterContributor.stepRole === 3) {

                    if (contributor.requireApproval) {
                        $('#contributor-btn-content').append(btnfinalise);
                        $('#contributor-btn-content').append(btnpublish);
                        $('#contributor-btn-content').append(btninvite);
                    }
                    else {
                        $('#contributor-btn-content').append(btndone);
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

                $('#contributor-btn-content').append(btnsubmit);
                $('#contributor-btn-content').append(btnsaveandclose);

                $('#contributor-content').append(spanmessage);

                var contributordocumentpanel = '<span id="contributor-document-panel" class=pull-right></span> ';
                $('#contributor-content').append(contributordocumentpanel);

                if (documents !== undefined && documents !== null) {
                    documents.forEach(function (document, index) {
                        var documentelement = '  <a class="mr-2" href="' + _SyntaqBaseURI + '/Falcon/RecordMatterItems/GetDocumentForDownload?Id=' + document.id + '&version=1&format=' + document.format + '"><img height=32 src="' + _SyntaqBaseURI + '/Common/Images/Entities/' + document.format + '.png"/> ' + document.name + '</a> ';
                        $('#contributor-document-panel').append(documentelement);
                    });
                }

                if (contributor.recordMatterContributor.stepRole === 0 || contributor.recordMatterContributor.stepRole === 1 || contributor.recordMatterContributor.stepRole === 3) {
                    $('#contributor-content').append("<hr/>");
                }

                $('body').append(Syntaq.Form.ContributorDialogTemplate);
                contributor_dialog = new Modal('#contributor-dialog', {
                    backdrop: true
                });

                var contributorstatus = null; var contributortype = null;
                var contributormessage = '';
                $(".btn-contributor").click(function () {

                    contributorstatus = $(this).data('status');
                    contributortype = $(this).data('title');
                    $('#btn-contributor-comment-save').text(contributortype);

                    switch (contributortype) {
                        case 'Complete':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success");
                            contributormessage = 'You successfully completed editing the document.';
                            break;
                        case 'Publish':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success");
                            contributormessage = 'You successfully published the document.';
                            break;
                        case 'Submit Review':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-success");
                            contributormessage = 'You successfully reviewed the document.';
                            break;
                        case 'Decline':
                            $('#btn-contributor-comment-save').attr("class", "btn btn-danger");
                            contributormessage = 'Review Declined.';
                            break;
                    }

                    contributor_dialog.show();
                });

                $(".btn-done").click(function () {

                    isFormValid = true;

                    checkFormIsvalid(Form);

                    FormPages.pages.forEach(function (page, index) {
                        if (!page.isvalid) {
                            isFormValid = false;
                        }
                    });

                    if (isFormValid) {
                        finaliseStep();
                    }
                    else {
                        swal({
                            title: "Error",
                            text: "Form cannot be published with missing required fields"
                        });
                    }
                });

                $(".btn-submit").click(function () {
                    var submission = { "data": Form.data };
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


                        swal({
                            title: "Success",
                            text: "Step Reviewed",
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
                                // location.reload();
                            });

                        Syntaq.Form.SubmitFunction({ "data": Form.data });


                    }).fail(function (data) {
                    });

                }

                $("#btn-contributor-publish").click(function () {
                    isFormValid = true;
                    checkFormIsvalid(Form);

                    FormPages.pages.forEach(function (page, index) {
                        if (!page.isvalid) {
                            isFormValid = false;
                        }
                    });

                    if (isFormValid) {
                        var datatitle = $(this).data('title');
                        var title = 'Confirm ' + datatitle;
                        var status = $(this).data('status');

                        var submission = Form.data;
                        Syntaq.Form.SaveFunction(submission, false);

                        swal({
                            title: app.localize("Publish"),
                            text: app.localize('Do you want to invite a Reviewer?'),
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
                                    _createOrEditModal.open({ id: null, recordmatterId: Syntaq.Form.RecordMatterId, formID: Syntaq.Form.FormId, role: 0 });
                                } else {

                                    // FINALISE?
                                    publishStep2();

                                    swal({
                                        title: "Are you sure?",
                                        text: app.localize('Do you want to Finalise?'),
                                        // icon: "warning",
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
                                        .then((result2) => {
                                            if (result2) {

                                                //publishStep2();
                                                finaliseStep();
                                                //swal({
                                                //    title: "Are you sure?",
                                                //    text: app.localize('Do you want to invite an Approver?'),
                                                //   // icon: "warning",
                                                //    buttons:
                                                //    {
                                                //        confirm: {
                                                //            text: "Yes",
                                                //            value: true,
                                                //            visible: true,
                                                //            className: "",
                                                //            closeModal: true
                                                //        },
                                                //        cancel: {
                                                //            text: "No",
                                                //            value: false,
                                                //            visible: true,
                                                //            className: "",
                                                //            closeModal: true,
                                                //        }
                                                //    },
                                                //    dangerMode: false,
                                                //})
                                                //    .then((result3) => {
                                                //        if (result3) {
                                                //            _createOrEditModal.open({ id: null, recordmatterId: Syntaq.Form.RecordMatterId, formID: Syntaq.Form.FormId, role: 1 });
                                                //        } else {
                                                //            finaliseStep();
                                                //        }
                                                //    });	                                       
                                            } else {
                                                // Exit
                                            }
                                        });


                                }
                            });

                    }
                    else {
                        swal({
                            title: "Missing Required Field",
                            text: "You cannot send your form for review with required field missing."
                        });
                    }

                });

                function finaliseStep() {

                    isFormValid = true;
                    checkFormIsvalid(Form);

                    if (isFormValid) {
                        var data = JSON.stringify({ Id: Syntaq.Form.RecordMatterId, Finalise: true });
                        var url = _SyntaqBaseURI + '/api/services/app/Projects/FinaliseStep';

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

                            swal({
                                title: "Success",
                                text: "Step Finalised",
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
                                    var openProject = location.protocol + "//" + window.location.hostname + (window.location.port !== null ? ":" + window.location.port : "") + '/Falcon/Projects/ViewProject/' + ProjectId;
                                    window.open(openProject);

                                });


                            Syntaq.Form.SubmitFunction({ "data": Form.data });
                            //this.Form.submit();

                        }).fail(function (data) {

                        });
                    }


                }

                $("#btn-contributor-finalise").click(function () {
                    isFormValid = true;
                    checkFormIsvalid(Form);

                    FormPages.pages.forEach(function (page, index) {
                        if (!page.isvalid) {
                            isFormValid = false;
                        }
                    });

                    if (isFormValid) {
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

                                    finaliseStep(title);

                                    //swal({
                                    //    title: "Are you sure?",
                                    //    text: app.localize('Do you want to invite an Approver?'),
                                    //   // icon: "info",
                                    //    buttons:
                                    //    {
                                    //        confirm: {
                                    //            text: "Yes",
                                    //            value: true,
                                    //            visible: true,
                                    //            className: "",
                                    //            closeModal: true
                                    //        },
                                    //        cancel: {
                                    //            text: "No",
                                    //            value: false,
                                    //            visible: true,
                                    //            className: "",
                                    //            closeModal: true,
                                    //        }
                                    //    }//,
                                    //    //dangerMode: true,
                                    //})
                                    //.then((result) => {
                                    //    if (result) {
                                    //        _createOrEditModal.open({ id: null, recordmatterId: Syntaq.Form.RecordMatterId, formID: Syntaq.Form.FormId, role: 1 });
                                    //    } else {
                                    //        finaliseStep(title);
                                    //    }
                                    //});	
                                }
                            });

                    }
                    else {
                        swal({
                            title: "Error",
                            text: "Form cannot be published with missing required fields"
                        });
                    }
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

                    var AuthToken = getParameterByName('AccessToken');
                    var data = { 'AccessToken': AuthToken, 'Comments': $('#contibutor-dialog-comment').val() };
                    var input = JSON.stringify('{"AccessToken": "' + AuthToken + '", "Comments": "' + $('#contibutor-dialog-comment').val() + '"}');

                    toastr.info('Submitting');

                    abp.ajax({
                        type: "PUT",
                        contentType: 'application/json',
                        url: _SyntaqBaseURI + "/api/services/app/RecordMatterContributors/UpdateComments",
                        data: JSON.stringify(data)
                    }).done(function (data) {

                        var AccessToken = getParameterByName('AccessToken');
                        data = JSON.stringify({ AccessToken: AccessToken, Status: contributorstatus });
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
                            $("#contributor-content button").attr("disabled", true);


                            Syntaq.Form.SubmitFunction({ "data": Form.data });

                            //this.Form.submit();
                            //swal({
                            //    title: "Success",
                            //    text: contributormessage,
                            //    buttons:
                            //    {
                            //        confirm: {
                            //            text: "Ok",
                            //            value: true,
                            //            visible: true,
                            //            className: "",
                            //            closeModal: true
                            //        }
                            //    }
                            //})
                            //.then((result) => {                                
                            //   // location.reload();
                            //});

                        }).fail(function (data) {
                        });


                    }).fail(function () {
                        toastr.error('Comments not updated');
                    });

                });

            }

            function buildform(form, readOnly) {

                setToastrOptions();

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
                            // Load css into scoped syntaq-content after the form is built
                            if (Syntaq.Form.FormType === 'popup') {
                                loadjscssfile(_SyntaqBaseURI + "/assets/formio/app/bootstrap/css/bootstrap.min.css", "cssinhead"); //already allow in the security content
                                loadjscssfile(_SyntaqBaseURI + "/assets/syntaq/syntaq.bootstrap.min.css", "cssinhead");
                                loadjscssfile(_SyntaqBaseURI + "/assets/formio/dist/formio.full.css", "cssinhead");
                             }
                            loadjscssfile(_SyntaqBaseURI + "/assets/formio/app/fontawesome/css/font-awesome.min.css", "css");
                            loadjscssfile(_SyntaqBaseURI + "/lib/toastr/build/toastr.css", "css");
                            loadjscssfile(_SyntaqBaseURI + "/lib/spin.js/spin.css", "css");
                            loadjscssfile(".form-control {border-width: 2px ;}", "cssinline");
                            loadjscssfile(_SyntaqBaseURI + "/TenantCustomization/GetCustomCss", "css");
                            loadjscssfile("#syntaq-content .navbar { padding-left: 0rem!important; }", "cssinline");
                            loadjscssfile(".btn-xxs, .btn-group-xxs > .btn, .component-btn-group .component-settings-button { padding: 2px 2px; font-size: 12px; line-height: 1.2em; border-radius: 0; width: 18px; height: 18px; }", "cssinline");

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


                            //console.log(form);
                            //console.log(Form);
                            //change wizard nar bar to buger menu
                            // if (From.wizard.display ==="wizard") {

                            //FormPages.pages.forEach(function (page, index) {

                            //    // Track Form Changes
                            //    var li = $('.page-item')[index];

                            //    if (page.isvalid) {

                            //        $(link).addClass("nav-item");
                            //    }

                            //});


                            //}

                            form.on('initialized', function () {
                                console.log('init');
                                if (typeof InitCustomFormScript === "function") {
                                    InitCustomFormScript();
                                }

                                if (JSON.stringify(formJSON).indexOf('"type":"popupform"')) {
                                    
                                    Form.updateValue({
                                        modified: true
                                    });
                                    Form.redraw();
                                }



                            });

                            // console.log(Form);

                            form.on('change', function (event) {

                                //there are changes in the form;
                                saved = false;

                                buildSummary();
                                buildImportant();

                                buildWizardHeader();
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

                            });


                            if (Form.schema.hasOwnProperty("autoSaving") && Form.schema.autoSaving) {
                                autoSaveForm(form);
                            }
                            // Auto save form function. 
                            function autoSaveForm(form) {
                                var save = false;
                                toastr.info('Form will be automatically saved...');
                                setInterval(function () {
                                    if ($('div.formio-modified').length > 0 && save) {
                                        //toastr.info('Form auto saving...');
                                        form.emit('save', form.submission.data);
                                        //Set a time out for temp stoping auto saving
                                        setTimeout(function () {
                                            save = false;
                                            toastr.info('Stop auto saving...');
                                        }, 30000)
                                    }
                                }, 30000); //30 seconds
                                form.on('change', function () {
                                    save = true;
                                });
                            }
                            //Auto save form function-------END

                            //warning there are unsaved changes in the form
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

                                if (Form.pages) {
                                    var importantview = '';
                                    var importantrows = '';
                                    var page = Form.pages[Form.page];
                                    var pagelabel = page.label;

                                    buildImportantSection(Form, .75);

                                    var hasview = true;

                                    if (importantrows === '') {
                                        hasview = false;
                                    }
                                    else {
                                        importantview = '<table class=" table table-sm    table-bordered"><tr><td colspan=2 style="font-weight: bold; background-color: #fff " ><h4 style="font-weight: bold;"><span class="fa fa-window-maximize"></span> ' + pagelabel + '</h3></td></tr>' + importantrows + '</table><p></p>';
                                    }

                                    $('.syntaq-component-important').html('<div id="syntaq-component-important-alert" style="background-color: #ffb822c7" class="alert alert-warning  row" role="alert">  <h4 class="alert-heading font-weight-bold col-12"><span class="fa fa-exclamation-triangle"></span> Some pages are still missing information recommended for a final contract.</h4>  <p class="col-12">This means your contract is not yet in a form suitable for signing, but that’s okay if all you need is a draft.</p><div id="important-table" class="w-100"></div></div>');

                                    Syntaq.Form.ImportantViews[Form.page] = { key: page.key, important: importantview, hasview: hasview };                          
                                    Syntaq.Form.ImportantView = ''; Form.data.HasImportantFieldsMissing = false;
                                    Syntaq.Form.ImportantViews.forEach(function (view) {
                                        if (view.hasview) {
                                            $('#important-table').append(view.important);
                                            Syntaq.Form.ImportantView += view.important;
                                            Form.data.HasImportantFieldsMissing = true;
                                        }
                                    });

                                    if (! Form.data.HasImportantFieldsMissing) {
                                        $('#syntaq-component-important-alert').hide();
                                        //let alert = document.createElement();
                                       // $('.syntaq-component-important').prepend('<div class="alert alert-warning row" role="alert">  <h4 class="alert-heading col-12"><span class="fa fa-exclamation-triangle"></span> Some pages are still missing information recommended for a final contract.</h4>  <p class="col-12">This means your contract is not yet in a form suitable for signing, but that’s okay if all you need is a draft.</p></div>');
                                    }
                                    
                                    function buildImportantSection(parent, indent) {

                                        indent += .75;    
                                        var addedheader = false;
                                        parent.components.forEach(function (comp) {
                                            var type = comp.component.type;
                                            if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel' )) {                                                    
                                                if (comp._visible) { 
                                                    buildImportantSection(comp, indent);                                                                                                           
                                                }                                               
                                            }
                                            else {                                               
                                                if (comp.component.important && comp._visible) {
                                                    var value = getDataValue(comp, comp.key, false);
                                                    if (value === '' || value === null || value === undefined) {
                                                        var label = comp.label;                                                    
                                                        if (comp.parent.parent && !addedheader) {
                                                            importantrows += '<tr><td style="padding-left: ' + (indent - .75) + 'em; font-weight: bold; background-color: #fff " colspan="2"><span class="fa fa-share fa-rotate-0 fa-flip-vertical"></span> ' + comp.parent.parent.component.label + '</td></tr>';
                                                            addedheader = true;
                                                        }
                                                        importantrows += '<tr><td class="small"  style="padding-left: ' + indent + 'em; ; background-color: #fff;  font-weight: bold; "><span class="ml-4">' + label + '</span></td></tr>';
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
                                    var page = Form.pages[Form.page];
                                    var pagelabel = page.label;

                                    Form.components.forEach(function (comp) {
                                        var type = comp.component.type;
                                        if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
                                            buildSummarySection(comp, 1.5);
                                        }
                                        else {
                                            buildSummaryComponent(comp, .75);
                                        }
                                    });

                                    var hasview = true;
                                    if (summaryview === '') hasview = false;
                                    summaryview = '<table style="width:100%; border:0px solid #dee2e6 " ><tr><td colspan=2 style="font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6; font-size:1.1em" >' + pagelabel + '</td></tr>' + summaryview + '</table><p></p>';
                                    Syntaq.Form.SummaryViews[Form.page] = { key: page.key, summary: summaryview, hasview: hasview };

                                    $('.syntaq-component-summarytable').html('');
                                    Syntaq.Form.SummaryView = '';
                                    Syntaq.Form.SummaryViews.forEach(function (view) {
                                        if (view.hasview) {
                                            $('.syntaq-component-summarytable').append(view.summary);
                                            Syntaq.Form.SummaryView += view.summary;
                                        }
                                    });

                                    function buildSummarySection(parent, indent) {

                                        indent += .75;
                                        parent.components.forEach(function (comp) {
                                            var type = comp.component.type;
                                            if ((type === 'datagrid' || type === 'panel' || type === 'sfapanel' || type === 'section' || type === 'sectionpanel')) {
                                                if (comp._visible) {
                                                    var label = comp.parent.component.label;
                                                    if (type === 'sectionpanel') {
                                                        summaryview += '<tr><td style="padding-left: ' + indent + 'em; font-weight: bold;background-color: #f2f2f2; border:1px solid #dee2e6" colspan="2">' + label + '</td></tr>';
                                                    }
                                                    buildSummarySection(comp, indent);
                                                }
                                            }
                                            else {
                                                if (comp._visible) {
                                                    buildSummaryComponent(comp, indent);
                                                }
                                            }
                                        });
                                    }

                                    function buildSummaryComponent(comp, indent) {
                                        if (comp.component.showSummary && comp._visible) {
                                            var value = getDataValue(comp, comp.key, false);
                                            var label = comp.label;
                                            summaryview += '<tr><td  style="padding-left: ' + indent + 'em; width:50%; ; border:1px solid #dee2e6">' + label + '</td><td style="width:50%; ; border:1px solid #dee2e6">' + value + '</td></tr>';
                                        }
                                    }
                                    /////////////////////////////
                                    // End Build Summary if required
                                    /////////////////////////////	
                                }

                            }

                            function buildWizardHeader() {

                                //just add buger menu once
                                var is_exist = $('#navbarNav1');
                                
                                //buger menu
                                if (Form.schema.hasOwnProperty("display") && Form.schema.display === "wizard" && is_exist.length === 0) {

                                    //console.log("wizard");

                                    var header = $("[aria-label='navigation']");
                                    $(header).addClass("navbar navbar-expand-lg navbar-light");
                                    var WizardButton = '<button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav1"><span class="navbar-toggler-icon"></span></button>';

                                    header.prepend(WizardButton);
                                    var div = '<div class="collapse navbar-collapse" id="navbarNav1"></div>';

                                    var ul = $('.pagination');
                                    ul.wrap(div);
                                    ul.addClass('navbar-nav');
                                    var li = document.querySelectorAll('.page-item');
                                    for (var i = 0; i < li.length; ++i) {
                                        $(li).addClass('nav-item');
                                        li[i].display = "";
                                    }
                                    // console.log(form);
                                }
                            }

                            form.on('render', function () {
                                buildSummary();
                                showhidePaymentForm();
                                buildWizardHeader();
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

                                var url = _SyntaqBaseURI + "/api/services/app/RecordMatters/GetRecordMatterForEdit?id=" + Syntaq.Form.RecordMatterId + "&RecordMatterItemId=" + Syntaq.Form.RecordMatterItemId + "&AccessToken=" + AuthToken;

                                var recordmode = getParameterByName('load');
                                if (recordmode === 'record' || Syntaq.Form.LoadFromRecord) {
                                    url = _SyntaqBaseURI + "/api/services/app/Records/GetRecordForEdit?id=" + Syntaq.Form.RecordId;
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

                                        Syntaq.Form.RecordId = recordmatter.recordId;
                                        Syntaq.Form.AnonAuthToken = recordmatter.accessToken;

                                    }

                                    // If you have loaded a recordmatter make sure to ensure the record ID matches
                                    // also load the ANONAccessToken
                                    initFormData(
                                        data
                                    );

                                    Syntaq.Form.LoadFromRecord = false;

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

                                // Commented due to single page issue
                                if (Form.schema.hasOwnProperty("display") && Form.schema.display == "wizard" && data.page !== 'undefined') {
                                    form.setPage(data.page);
                                }

                            }

                            form.on('draft', function (submission) {

                            });

                            form.on('prevPage', function (submission) {
                                firstload = true;
                            });

                            form.on('nextPage', function (submission) {
                                firstload = true;
                            });

                            form.on('save', function (submission) {
                                saveForm(submission, false);
                            });

                            function saveForm(submission, saveandclose) {

                                toastr.info('Form saving...');
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

                                    submission.AccessToken = Syntaq.Form.AnonAuthToken;
                                    submission.RecordId = Syntaq.Form.RecordId;
                                    submission.RecordMatterId = Syntaq.Form.RecordMatterId;
                                    submission.RecordMatterItemId = Syntaq.Form.RecordMatterItemId;
                                    submission.SubmissionId = Syntaq.Form.SubmissionId;

                                    var accessToken = getUrlParameter('AccessToken'); // replaces AnonAuthToken

                                    if (accessToken === null || accessToken === 'null') accesstoekn = '';


                                    if (Form.schema.hasOwnProperty("display") && Form.schema.display == "wizard") {
                                        submission.page = form.hasOwnProperty("page") ? form.page : 0;
                                    }

                                    var Data = JSON.stringify(submission);
                                    var input = JSON.stringify('{"AccessToken": "' + accessToken + '", "AnonAuthToken": "' + Syntaq.Form.AnonAuthToken + '",  "Id": "' + Syntaq.Form.FormId + '", "RecordId":"' + Syntaq.Form.RecordId + '", "RecordMatterId":"' + Syntaq.Form.RecordMatterId + '", "RecordMatterItemId":"' + Syntaq.Form.RecordMatterItemId + '", "Submission": ' + Data + '}');

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
                                            var openProject = _SyntaqBaseURI + '/Falcon/Projects/ViewProject/' + ProjectId;
                                            window.location.replace(openProject)
                                        }

                                        toastr.success('Form saved');
                                    }).fail(function () {
                                        toastr.error('Form not saved');
                                    });

                                }
                            }

                            Syntaq.Form.SaveFunction = saveForm;

                            form.on('draft', function (submission) {
                                console.log(submission);
                                //submitForm(submission);
                            });

                            // Triggered when they click the submit button.
                            form.on('submit', function (submission) {
                                submitForm(submission);
                            });

                            function submitForm(submission) {
                                toastr.info('Form submitting...');
                                saved = true;


                                if (typeof window.BeforeFormSubmit === 'function') {
                                    submission = window.BeforeFormSubmit(submission);
                                }

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

                                var data = JSON.stringify(submission);
                                var input = JSON.stringify('{ "AnonAuthToken": "' + Syntaq.Form.AnonAuthToken + '", "id":' + formId + ', "submission":' + data + ',"summarytablehtml":' + summarytable + '}');

                                if (Syntaq.Form.HasDocuments && Syntaq.Form.ShowSubmissionDialog) {
                                    buildSubmissionDialog();
                                }

                                abp.ajax({
                                    type: "POST",
                                    contentType: 'application/json',
                                    beforeSend: function (request) {
                                        request.setRequestHeader("Authorization", "Bearer " + Syntaq.GetAuthToken());
                                    },
                                    url: _SyntaqBaseURI + "/api/services/app/forms/run",
                                    data: input
                                }).done(function (data) {
                                    toastr.success('Form submitted');
                                    if (typeof window.AfterFormSubmit === 'function') {
                                        window.AfterFormSubmit();
                                    }

                                    if (redirectURL) {
                                        window.location.replace(redirectURL.toString());
                                    }
                                }).fail(function () {
                                    abp.message.error(e.error.message);
                                });

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

                                poll = setTimeout(pollSubmission, 3000);

                            }

                            var poll = null;
                            function pollSubmission() {

                                var url = _SyntaqBaseURI + "/api/services/app/submissions/GetSubmissionForView?id=" + Syntaq.Form.SubmissionId;
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
                                            if (data.recordMatterItems.length > 1) {
                                                $('#submission-dialog-title').text('Your Documents are Complete');
                                            }

                                            var cnt = 0; var hasdoc = false;
                                            data.recordMatterItems.forEach(function (item) {

                                                cnt === 0 ? $('#submission-dialog-body').append('<table class="mb-2"><tbody id="submission-dialog-tbody-id"></tbody></table>') : '';

                                                var itemurl =
                                                    _SyntaqBaseURI +
                                                    "/Falcon/Documents/GetDocumentForDownload?Id=" +
                                                    item.id +
                                                    "&AccessToken=" +
                                                    Syntaq.Form.AnonAuthToken;

                                                var tableRowTemplate = '<tr><td>'

                                                if (item.allowWord) {
                                                    tableRowTemplate += '<p><a href="' + itemurl + '&format=docx"><img style="height: 28px;" src="' + _SyntaqBaseURI + '/Common/Images/Entities/word.png" />' + item.documentName + '</a></p>';
                                                }

                                                if (item.allowPdf) {
                                                    tableRowTemplate += '<p><a href="' + itemurl + '&format=pdf"><img style="height: 28px;" src="' + _SyntaqBaseURI + '/Common/Images/Entities/pdf.png" />' + item.documentName + '</a></p>';
                                                }

                                                if (item.allowHTML) {
                                                    tableRowTemplate += '<p><a href="' + itemurl + '&format=docx"><img style="height: 28px;" src="' + _SyntaqBaseURI + '/Common/Images/Entities/html-filetype.png" /></a></p>';
                                                }

                                                if (item.allowPdf || item.allowWord || item.allowHTML) {
                                                    hasdoc = true;
                                                    tableRowTemplate += '</td></tr>';
                                                    $('#submission-dialog-tbody-id').append(tableRowTemplate);
                                                }

                                                cnt++ === data.recordMatterItems.length - 1 ? $('#submission-dialog-body').append('</tbody></table>') : '';

                                            });

                                            clearTimeout(poll);
                                            return;

                                        }

                                        poll = setTimeout(pollSubmission, 3000);
                                        return;

                                    }
                                    else {
                                        poll = setTimeout(pollSubmission, 3000);
                                        return;
                                    }

                                }).fail(function () {
                                    //abp.message.error(e.error.message);
                                });

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
        SubmissionDialogTemplate: "<div id=\"submission-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Your Form is Submitting</h5><button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button></div><div class=\"modal-body\"><div id=\"submission-dialog-body\" style=\"min-height:8em\"></div></div><div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Close</button></div></div></div></div>",
        ContributorDialogTemplate: "<div id=\"contributor-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Feedback</h5><button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button></div><div class=\"modal-body\"><div id=\"contributor-dialog-body\" style=\"min-height:8em\"><textarea class=\"form-control\" rows=8 id=\"contributor_comment\" placeholder=\"Enter your comments here\"></textarea></div></div><div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-default\" onclick=\"alert('d');\">Save</button><button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Cancel</button></div></div></div></div>",
        ContributorCommentDialogTemplate: "<div id=\"contributor-comment-dialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class= \"modal-dialog\" role=\"document\" ><div class=\"modal-content\"><div class=\"modal-header bg-light\"><h5 id=\"submission-dialog-title\" class=\"modal-title\">Comments</h5><button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button></div><div class=\"modal-body\"><div id=\"contributor-comment-dialog-body\" style=\"min-height:8em\"><textarea class=\"form-control\" id=\"contibutor-dialog-comment\" rows=12></textarea></div></div><div class=\"modal-footer\"><button id=\"btn-contributor-comment-save\" type=\"button\" class=\"btn btn-primary\" data-dismiss=\"modal\">Submit</button><button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Close</button></div></div></div></div>",
    },
    UserProfile: {

        ElementId: 'syntaq-content',

        LoadUI: function () {  // Method which will display type of animal

            setToastrOptions();

            var url = _SyntaqBaseURI + '/api/services/app/User/GetUserForEdit';
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
                url: _SyntaqBaseURI + '/api/services/app/User/GetUserForEdit'
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

                                toastr.success('Logo Updated');

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

                if (this.readyState === 4 && this.status === 200) {
                    toastr.success('Profile Updated');
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
                toastr.info('Record Deleted.');
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
                toastr.info('Folder Deleted.');
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
                var tempBody = '<div class="modal-dialog" style="top: 10%;"><div class="modal-content" style="margin-top: 25vh; resize: both; overflow: auto;"><div class="modal-header"><h4 class="modal-title"><span style="margin-left: 2vh; font-weight: bold;">New Form for Record</span></h4></div><div class="modal-body" style="word-break: break-word; text-align: justify; visibility: visible; position: relative;height: 10vh; padding: 1vh 2vh;"><input type="hidden" id="ridForForm" value=""><select name="Nformslist" id="loadFormslist" class="form-control"></select></div><div class="modal-footer"><a href="#?load=record"  class="btn btn-primary" onclick="Syntaq.Records.newFormOpend()">Opend</a><a href="#" class="btn btn-primary" onclick="Syntaq.Records.renderNewFormForRecodeModel(\'close\')">Close</a></div></div></div>';
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
                a.setAttribute('href', 'javascript:;');
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

                                data = '<a name="Folder"  data-folderid="' + row.id + '" class="mb-2"  href=#><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/folder.png" width=32  /> <strong class=\'h5\'>' + row.name + '</strong></a>';
                                data += '<span class="pull-right mt-3 mr-5"></span>';

                                return data;
                            } else {

                                data = '<div style="cursor:pointer">';

                                data += '<div name="Record" data-recordid="' + row.id + '">';
                                data += '<span style="width: 110px;"><img src=' + _SyntaqBaseURI + + '"/Common/Images/Entities/binders-folder.png"  /> </span>';

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
                                        data += '<span style="width: 110px;"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/moleskine.png"  /> </span>';

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
                            a.setAttribute('href', '#');
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

            //Syntaq.Busy.setBusy();

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
                            html += '<a name=\'Folder\' onclick=\'Syntaq.Records.FolderId="' + record.id + '"; Syntaq.Busy.setBusy(); Syntaq.Records.LoadRecords2();\' data-folderid=\'' + record.id + '\' href=\'#\'>';
                            html += '<h4><img src=\'' + _SyntaqBaseURI + '"/Common/Images/Entities/folder.png\' width=32  /> ' + record.name + ' </strong>';
                            html += '</a>';

                            html += '</td>';
                            html += '<td>';
                            //html += '<span class="pull-right mt-3 mr-5">';
                            //html += record.userACLPermission === "E" || record.userACLPermission === "O" ? '<a data-id="' + record.id + '" href=\'#\' class="OnClickLink" name="DeleteFolderLink" ><i class="fa fa-times text-danger"></i> Delete Folder</a>' : '';
                            //html += '</span">';
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
                                html += '<a href=\'#\' data-id="' + recordmatter.id + '"  class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete</a>';
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
                                                html += '<img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/lock.png" height=34/></a>';
                                            }
                                            else {
                                                html += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + record.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="#"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/form.png" height=34/></a>';
                                            }
                                        }
                                        if (recordmatteritem.allowPdf) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.pdf\', \'pdf\')" href="#"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/pdf.png" /></a>';
                                        }
                                        if (recordmatteritem.allowWord) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.docx\', \'docx\')" href="#" ><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/word.png" /></a>';
                                        }
                                        if (recordmatteritem.allowHTML) {
                                            html += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.html\', \'html\')" href="#"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/html-filetype.png"  /><strong>';
                                        }

                                    } else {
                                        html += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + record.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="#"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/form.png" height=34/></a>';
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

                    html += '<nav aria-label="Page navigation example"><ul class="pagination" ><li class="page-item"><a class="page-link" href="#">Previous</a></li><li class="page-item"><a class="page-link" href="#">1</a></li><li class="page-item"><a class="page-link" href="#">2</a></li><li class="page-item"><a class="page-link" href="#">3</a></li><li class="page-item"><a class="page-link" href="#">Next</a></li></ul></nav>';

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
                                a.setAttribute('href', '#');
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
                                        toastr.info('Record Deleted.');
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
                                        toastr.info('Record Matter Deleted.');
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
                                        toastr.info('Folder Deleted.');
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
                                data = '<a name="Folder"  data-folderid="' + row.id + '"  href=#><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/folder.png" width=32  /> <strong class=\'h5\'>' + row.name + '</strong></a>';
                            } else {
                                var dt = new Date(row.lastModified);
                                var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                var tmoptions = { hour: 'numeric', minute: 'numeric' };
                                dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                                data = '<img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/binders-folder.png" width=45 height=45 style="position:relative; top:-5px;"/><strong class=h5>' + row.name + '</strong>';
                                data += '<br>' + dt + '</span>';
                                data += '<div class="ml-1">';
                                //data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a class="OnClickLink" name="loadformforrecordelink"><i class="fas fa-plus"></i> New Form </a>' : '';
                                data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a data-id="' + row.id + '" class="OnClickLink" name="DeleteRecordLink"><i class="fa fa-times text-danger"></i> Delete Record</a>' : '';
                                //data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'#\'  onclick="Syntaq.Records.deleteRecord(\'' + row.id + '\')" class="OnClickLink" name="DeleteRecordLink"><i class="fa fa-times text-danger"></i> Delete Record</a>' : '';
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
                                data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a data-id="' + row.id + '" href=\'#\' class="OnClickLink" name="DeleteFolderLink" ><i class="fa fa-times text-danger"></i> Delete Folder</a>' : '';
                                //data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'#\' onclick="Syntaq.Records.deleteFolder(\'' + row.id + '\')" class="OnClickLink" name="DeleteFolderLink" ><i class="fa fa-times text-danger"></i> Delete Folder</a>' : '';
                                data += '</span>';
                            } else {
                                data = '<div>';

                                if (row.recordMatters !== null) {
                                    var rmc = 0;
                                    row.recordMatters.forEach(function (recordmatter) {
                                        data += '<span style="width: 110px;"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/moleskine.png"  /> </span>';

                                        var dt = new Date(recordmatter.creationTime);
                                        var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                                        dt = dt.toLocaleDateString('en-GB', options);

                                        data += '<span style="width: 350px;"><strong class=h5>' + recordmatter.recordMatterName + '</strong><span class="ml-3"> </span></span>';

                                        data += '<span class="pull-right mt-3 mr-5">';
                                        data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'#\' data-id="' + recordmatter.id + '"  class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete Record Matter</a>' : '';
                                        //data += row.userACLPermission === "E" || row.userACLPermission === "O" ? '<a href=\'#\' onclick="Syntaq.Records.deleteRecordMatter(\'' + recordmatter.id + '\')" class="OnClickLink" name="DeleteRecordMatterLink" data-identifier="' + rmc + '"><i class="fa fa-times text-danger"></i> Delete Record Matter</a>' : '';
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
                                                            data += '<img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/lock.png" height=34/></a>';
                                                        }
                                                        else {
                                                            data += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + row.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="#"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/form.png" height=34/></a>';
                                                        }
                                                    }
                                                    if (recordmatteritem.allowPdf) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.pdf\', \'pdf\')" href="#"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/pdf.png" /></a>';
                                                    }
                                                    if (recordmatteritem.allowWord) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.docx\', \'docx\')" href="#" ><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/word.png" /></a>';
                                                    }
                                                    if (recordmatteritem.allowHTML) {
                                                        data += '<a onclick="Syntaq.Records.downloadFile(\'' + recordmatteritem.id + '\', \'' + recordmatteritem.documentName + '.html\', \'html\')" href="#"><img style="height:38; width:38px" src="' + _SyntaqBaseURI + '"/Common/Images/Entities/html-filetype.png"  /><strong>';
                                                    }

                                                } else {
                                                    data += '<a class="OnClickLink" onclick="Syntaq.Records.loadForm(\'' + recordmatteritem.formId + '\', \'' + row.id + '\', \'' + recordmatter.id + '\', \'' + recordmatteritem.id + '\')" href="#"><img src="' + _SyntaqBaseURI + '"/Common/Images/Entities/form.png" height=34/></a>';
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
                            a.setAttribute('href', '#');
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

        if (instance.data !== null && instance.data !== undefined) {
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

    window.close();
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
        var list = document.querySelectorAll('#' + this.el.id + ' [data-dismiss="modal"]');
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
    document.body.style = '';
    document.body.classList.remove('modal-open');

    // removing backdrop
    if (self.options.backdrop) {
        var backdrop = document.getElementById('bs.backdrop');
        if (backdrop !== null) document.body.removeChild(backdrop);
    }
};


var userProfiletemplate = `

              <div class="w3-row">
                <a href="javascript:void(0)"  data-tab="user-tab-1">
                  <div class="w3-third tablink w3-bottombar w3-hover-light-grey w3-border-red w3-padding">User Information</div>
                </a>
                <a href="javascript:void(0)"  data-tab="user-tab-2">
                  <div class="w3-third tablink w3-bottombar w3-hover-light-grey w3-padding">Entity Information</div>
                </a>
                 <a href="javascript:void(0)"  data-tab="user-tab-3">
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