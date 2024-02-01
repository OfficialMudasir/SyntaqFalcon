/// <reference path="../../../../../assets/formio/app/jquery/jquery.min.js" />
(function ($) {
    app.modals.CreateEmbedModal = function () {
        var _modalManager;

        var EmbedType = 'Forms';
        var AccessType = 'Annon';
        var VersionType = '';
        var Version = '';
        var AuthType = '';
        var AuthToken = '';

        function initWizard() {

            // Initialize form wizard
            wizard = new KTWizard('kt_wizard_v2', {
                startStep: 1, // initial active step number
                clickableSteps: true  // allow step clicking
            });

            $(document).ready(function () {
                if (JsType === "Form") {
                    $("#EmbedTypeString").text("Forms");
                    EmbedType = "Forms";
                    $("#VersionTypeDiv").show();
                    $("#LiveForm").prop("checked", true);
                    VersionType = "Live";
                    $("#VersionDiv").hide();
                    $("#AuthTypeDiv").hide();
                    $("#AuthTokenDiv").hide();
                } else {
                    $("#EmbedTypeString").text("Records");
                    EmbedType = "Records";
                    $("#VersionTypeDiv").hide();
                    VersionType = "";
                    $("#VersionDiv").hide();
                    Version = '';
                    $("#LiveForm").prop("checked", false);
                    $("#AnonAccess").hide();
                    $("#AccessAuth").prop("checked", true);
                    $("#AuthTypeDiv").show();
                    AccessType = "Auth";
                    $("#AuthTypeDiv").show();
                    $("#AuthTokenDiv").show();
                    $("[name='AuthType']").prop('checked', true);
                    AuthType = "Token";
                }

                var Clipboard = new ClipboardJS("[name='CopyGeneratedCodeLink']"); 
				// Change event
                wizard.on("change", function (wizard) {
                    if ((wizard.getStep() === 3 && JsType === "Form") || (wizard.getStep() === 2 && JsType === "Record")) {
                        var Subdomain = window.location.hostname.split('.');                        
                        var OutputCode = '<link href="' + window.location.origin + '/assets/syntaq/syntaq.bootstrap.min.css" type="text/css" rel="stylesheet">\n';
                        OutputCode += '<link href="' + window.location.origin + '/assets/formio/app/fontawesome/css/all.min.css" type="text/css" rel="stylesheet">\n';
                        OutputCode += '<script src="' + window.location.origin + '/assets/formio/app/jquery/jquery.min.js"></script>\n';
                        OutputCode += '<script src="' + window.location.origin + '/assets/moment/moment.min.js"></script>\n';
                        OutputCode += '<script id="syntaq-script" src="' + window.location.origin + '/assets/Syntaq/Syntaq.js"></script>\n';
						OutputCode += '<script>\n';
						OutputCode += '\twindow.onload = function (e) {\n';
						//OutputCode += '\t\tSyntaq.BaseUrl = \'https://' + Subdomain[0] + '.syntaq.com\';\n';
                        OutputCode += '\t\tSyntaq.TenantName = \'' + TenancyName + '\';\n';
						//OutputCode += '\t\tSyntaq.Dependancies.Load();\n';
						if (AccessType === "Auth" && AuthType === "Token") {
							if (!$.isEmptyObject(AuthToken)) {
								OutputCode += '\t\tSyntaq.AuthToken = \'' + AuthToken + '\';\n';
							} else {
								OutputCode += '\t\tWARNING: Authentication Token hasn\'t been supplied!\n';
							}
						}

                        if (AccessType === "Annon") {
                            OutputCode += '\t\tSyntaq.Form.Anonymous = true;\n';
                        }
                        if (EmbedType === "Forms") {
                            OutputCode += '\t\tSyntaq.Form.FormId = \'' + JsFormId + '\';\n';
                            if (VersionType === "Live") {
                                OutputCode += '\t\tSyntaq.Form.Version = \'Live\';\n';
                            } else if (VersionType === "Specific") {
                                OutputCode += '\t\tSyntaq.Form.Version = \'' + Version + '\';\n';
                            }                            
                            OutputCode += '\t\tSyntaq.Form.ShowProgressTicks = true;\n';
                            OutputCode += '\t\tSyntaq.Form.createForm();\n';
                        } else {
                            OutputCode += '\t\tSyntaq.Records.LoadRecords();\n';
                        }
                        OutputCode += '\t}\n';
                        if (AccessType === "Auth" && AuthType === "Login") {
                            OutputCode += '\tfunction onLoginComplete() {\n';
                            if (EmbedType === "Forms") {
                                OutputCode += '\t\tSyntaq.Form.createForm();\n';
                            } else {
                                OutputCode += '\t\tSyntaq.Records.LoadRecords();\n';
                            }
                            OutputCode += '\t}\n';
                        }
                        OutputCode += '</script>\n';

                        if (EmbedType === "Forms") {
                            OutputCode += '<div id="syntaq-content" class="p-3"></div>\n';
                        } else {
                            OutputCode += '<table id="syntaq-records-content" class="p-3"></table>\n';
                        }

                        $("#GeneratedCode").text(OutputCode.trim());
                        $("[name='DownloadGeneratedCodeLink']").attr("data-content", "" + OutputCode.trim() + "");
                        Prism.highlightElement($("#GeneratedCode")[0]);
                    } else {
                        $("#GeneratedCode").text("");
                        $("[name='DownloadGeneratedCodeLink']").attr("href", "");
                    }

                    KTUtil.scrollTop();
                });
            });
        };

        this.init = async function (modalManager) {
            _modalManager = modalManager;
            _$modalDialog = _modalManager.getModal().find('.modal-dialog');
            _$modalDialog.css("max-width", "90em");

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });
            initWizard();
        };

        $("[name='EmbedType']").on("change", function () {
            if (this.value === "Forms") {
                $("#EmbedTypeString").text("Forms");
                EmbedType = "Forms";
                $("#VersionTypeDiv").show();
                $("#LiveForm").prop("checked", true);
                VersionType = "Live";
                $("#AnonAccess").show();
                $("#AccessAuth").prop("checked", false);
                $("#AccessAnon").prop("checked", true);
                $("#AuthTypeDiv").hide();
                $("#AuthTokenDiv").hide();
                $("[name='AuthType']").prop('checked', false);
            } else {
                $("#EmbedTypeString").text("Records");
                EmbedType = "Records";
                $("#VersionTypeDiv").hide();
                VersionType = "";
                $("#VersionDiv").hide();
                Version = '';
                $("#LiveForm").prop("checked", false);
                $("#AnonAccess").hide();
                $("#AccessAuth").prop("checked", true);
                $("#AuthTypeDiv").show();
                AccessType = "Auth";
            }
        });

        $("[name='VersionType']").on("change", function () {
            if (this.value === "Specific") {
                VersionType = "Specific";
                $("#VersionDiv").show();
                $("[name='FormVersionsDropdown']").change();
            } else {
                VersionType = "Live";
                $("#VersionDiv").hide();
                Version = '';
            }
        });

        $("[name='FormVersionsDropdown']").on("change", function () {
            Version = this.value;
        });

        $("[name='AccessType']").on("change", function () {
            if (this.value === "Annon") {
                $("#AuthTypeDiv").hide();
                $("#AuthTokenDiv").hide();
                $("[name='AuthType']").prop('checked', false);
                AccessType = "Annon";
            } else {
                $("#AuthTypeDiv").show();
                AccessType = "Auth";
            }
        });

        $("[name='AuthType']").on("change", function () {
            if (this.value === "Token") {
                $("#AuthTokenDiv").show();
                AuthType = "Token";
            } else {
                $("#AuthTokenDiv").hide();
                AuthType = "Login";
            }
        });

        $("#AuthToken").on("change", function () {
            AuthToken = this.value;
        });

        $("[name='DownloadGeneratedCodeLink']").on("click", function () {
            var OutputBlob = new Blob([this.attributes[2].nodeValue.toString()], { type: 'text/html' });
            url = window.URL.createObjectURL(OutputBlob);
            this.href = url;
            this.target = '_blank';
            this.download = 'EmbedCode.html';
        });
    };
})(jQuery);