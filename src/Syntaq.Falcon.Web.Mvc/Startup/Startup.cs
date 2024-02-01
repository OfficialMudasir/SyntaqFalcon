using System;
using System.IO;
using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Hangfire;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.Configure;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Identity;
using Syntaq.Falcon.Schemas;
using Syntaq.Falcon.Web.Chat.SignalR;
using Syntaq.Falcon.Web.Common;
using Syntaq.Falcon.Web.Resources;
using Swashbuckle.AspNetCore.Swagger;
using Syntaq.Falcon.Web.IdentityServer;
using Syntaq.Falcon.Web.Swagger;
using Stripe;
using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.Mvc.Caching;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.IdentityServer4vNext;
using HealthChecks.UI;
using HealthChecks.UI.Client;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Syntaq.Falcon.Web.HealthCheck;
using Owl.reCAPTCHA;
using HealthChecksUISettings = HealthChecks.UI.Configuration.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Syntaq.Falcon.Web.Extensions;
using SecurityStampValidatorCallback = Syntaq.Falcon.Identity.SecurityStampValidatorCallback;
using Microsoft.AspNetCore.Authorization;
using Syntaq.Falcon.Authorization.Handlers;
using Syntaq.Falcon.Authorization.Requirements;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Linq;
using Abp.Extensions;
using Syntaq.Falcon.Settings.Dtos;
using Abp.Timing;

using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using Syntaq.Falcon.Web.Hangfire;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Syntaq.Falcon.Filters;

//REFACTOR FOR MERGE
namespace Syntaq.Falcon.Web.Startup
{
    public class Startup
    {
        private const string AllowAllOriginsPolicy = "AllowAll";
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _appConfiguration = env.GetAppConfiguration();
            //STQ MODIFIED
            Clock.Provider = ClockProviders.Utc;
            _hostingEnvironment = env;

            Configuration = configuration;

        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            // MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());

                //STQ MODIFIED
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.DateParseHandling = DateParseHandling.None;
                });


            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.DateParseHandling = DateParseHandling.None;
            });


            if (bool.Parse(_appConfiguration["KestrelServer:IsEnabled"]))
            {
                ConfigureKestrel(services);
            }

            IdentityRegistrar.Register(services);

            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration, options =>
                    options.UserInteraction = new UserInteractionOptions()
                    {
                        LoginUrl = "/Account/Login",
                        LogoutUrl = "/Account/LogOut",
                        ErrorUrl = "/Error"
                    });
            }
            else
            {
                services.Configure<SecurityStampValidatorOptions>(opts =>
                {
                    opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
                });
            }

            AuthConfigurer.Configure(services, _appConfiguration);

            // STQ MODIFIED
            if (_appConfiguration.GetValue<bool>("SwaggerUi:Enabled"))
            {
                //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Falcon API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.ParameterFilter<SwaggerEnumParameterFilter>();
                    options.SchemaFilter<SwaggerEnumSchemaFilter>();
                    options.OperationFilter<SwaggerOperationIdFilter>();
                    options.OperationFilter<SwaggerOperationFilter>();
                    options.CustomDefaultSchemaIdSelector();
                }); //.AddSwaggerGenNewtonsoftSupport();
            }

            //Recaptcha
            services.AddreCAPTCHAV3(x =>
            {
                x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
                x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
            });

            if (WebConsts.HangfireDashboardEnabled)
            {
                // Hangfire (Enable to use Hangfire instead of default job manager)
                services.AddHangfire(config =>
                {
                    //only retry once
                    config.UseFilter(new AutomaticRetryAttribute { Attempts = 2 });

                    //hangfire log will keep 7 days
                    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"))
                          .WithJobExpirationTimeout(TimeSpan.FromDays(7));

                    HangfireService.InitializeJobs();
                });

                services.AddHangfireServer();
            }

            services.AddScoped<IWebResourceManager, WebResourceManager>();

            services.AddSignalR();

            if (WebConsts.GraphQL.Enabled)
            {
                services.AddAndConfigureGraphQL();
            }

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            {
                services.AddAbpZeroHealthCheck();

                var healthCheckUISection = _appConfiguration.GetSection("HealthChecks")?.GetSection("HealthChecksUI");

                if (bool.Parse(healthCheckUISection["HealthChecksUIEnabled"]))
                {
                    services.Configure<HealthChecksUISettings>(settings =>
                    {
                        healthCheckUISection.Bind(settings, c => c.BindNonPublicProperties = true);
                    });

                    services.AddHealthChecksUI()
                        .AddInMemoryStorage();
                }
            }

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new RazorViewLocationExpander());
            });

            ////
            /// CUSTOM SECTION
            /// 

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // Add CORS policy

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsPolicy,
                builder =>
                {

                    var allowAnyOrigin = _appConfiguration.GetValue<bool>("Cors:AllowAnyOrigin");
                    var allowedOrigins = _appConfiguration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                    if (allowAnyOrigin)
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(allowedOrigins);
                    }

                    //builder.AllowAnyOrigin()
                    builder.AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("woff2")
                            .WithExposedHeaders("woff")
                            .WithExposedHeaders("ttf");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewById", policy => policy.Requirements.Add(new ACLRoleRequirement("View", "Id")));
                options.AddPolicy("EditById", policy => policy.Requirements.Add(new ACLRoleRequirement("Edit", "Id")));
                options.AddPolicy("DeleteById", policy => policy.Requirements.Add(new ACLRoleRequirement("Delete", "Id")));
                options.AddPolicy("ShareById", policy => policy.Requirements.Add(new ACLRoleRequirement("Share", "Id")));

                options.AddPolicy("ViewByOriginalId", policy => policy.Requirements.Add(new ACLRoleRequirement("View", "OriginalId")));
                options.AddPolicy("EditByOriginalId", policy => policy.Requirements.Add(new ACLRoleRequirement("Edit", "OriginalId")));
                options.AddPolicy("DeleteByOriginalId", policy => policy.Requirements.Add(new ACLRoleRequirement("Delete", "OriginalId")));
                options.AddPolicy("ShareByOriginalId", policy => policy.Requirements.Add(new ACLRoleRequirement("Share", "OriginalId")));

                options.AddPolicy("ViewByRecordMatterId", policy => policy.Requirements.Add(new ACLRoleRequirement("View", "RecordMatterId")));
                options.AddPolicy("EditByRecordMatterId", policy => policy.Requirements.Add(new ACLRoleRequirement("Edit", "RecordMatterId")));
                options.AddPolicy("DeleteByRecordMatterId", policy => policy.Requirements.Add(new ACLRoleRequirement("Delete", "RecordMatterId")));
                options.AddPolicy("ShareByRecordMatterId", policy => policy.Requirements.Add(new ACLRoleRequirement("Share", "RecordMatterId")));
            });

            services.AddSingleton<IAuthorizationHandler, ACLRoleHandler>();

            if (string.Equals(
                 Environment.GetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED"),
                 "true", StringComparison.OrdinalIgnoreCase))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    // Only loopback proxies are allowed by default.
                    // Clear that restriction because forwarders are enabled by explicit 
                    // configuration.
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });
            }

            var metadataURL = _appConfiguration["MetaDataServiceUrl"];

            services.Configure<STQSaml2Configuration>(Configuration.GetSection("Saml2"));
            services.Configure<STQSaml2Configuration>(saml2Configuration =>
            {

                saml2Configuration.Enabled = Convert.ToBoolean(_appConfiguration["Saml2:Enabled"]);
                saml2Configuration.LoggedOutPage = _appConfiguration["Saml2:LoggedOutPage"];
                saml2Configuration.AllowedAudienceUris.Add("Syntaq.Falcon");            

                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(metadataURL));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }
            });

            services.AddSaml2();

            services.AddControllers(config =>
            {
                config.Filters.Add(new DateTimeZoneNormailser());
            });

            //App Settings
            services.Configure<AppConfig>(_appConfiguration.GetSection("App"));

            //Get Edge
            services.Configure<GetEdgeConfig>(_appConfiguration.GetSection("GetEdge"));

            //AssemblyFunction Settings
            services.Configure<AssemblyFunctionConnection>(_appConfiguration.GetSection("AssemblyFunctionConnection"));

            //Storage Settings
            services.Configure<StorageConnection>(_appConfiguration.GetSection("StorageConnection"));

            //FileValidationService Settings
            services.Configure<NZBNConnection>(_appConfiguration.GetSection("NZBNConnection"));

            //FileValidationService Settings
            services.Configure<FileValidationService>(_appConfiguration.GetSection("FileValidationService"));

            //Syntaq Payments Stripe Key
            services.Configure<StripeConnection>(_appConfiguration.GetSection("StripeConfiguration"));

            //Get Edge
            services.Configure<StGeorgeConfig>(_appConfiguration.GetSection("StGeorge"));

            //Get Edge
            services.Configure<JSONWebToken>(_appConfiguration.GetSection("JSONWebToken"));

            // Add SAML SSO services.
            var samlsection = _appConfiguration.GetSection("SAML");
            services.AddSaml(samlsection);

            //Configure Abp and Dependency Injection
            return services.AddAbp<FalconWebMvcModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config")
                );

                options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"),
                    SearchOption.AllDirectories);
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            // Security - Registered before static files to always set header
            //app.UseHsts(hsts => hsts.MaxAge(365));
            //app.UseXContentTypeOptions();
            //app.UseReferrerPolicy(opts => opts.NoReferrer());

            //app.UseStaticFiles();

            //////////////////////////////////////////////////////////////////
            // Start STQ Modifications
            /////////////////////////////////////////////////////////////////
            //app.UseCookiePolicy(
            //    new CookiePolicyOptions
            //    {
            //        Secure = CookieSecurePolicy.Always,
            //        HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            //    });

            //////////////////////////////////////////////////////////////////
            // END STQ Modifications
            /////////////////////////////////////////////////////////////////

            // Security - Registered after static files, for dynamic content.
            //app.UseXfo(xfo => xfo.Deny());
            //app.UseRedirectValidation();
            //app.UseXXssProtection(options => options.EnabledWithBlockMode());


            // Forward Headers to provide support for Linux hosted WebApp
            if (string.Equals(
    Environment.GetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED"),
    "true", StringComparison.OrdinalIgnoreCase))
            {
                app.UseForwardedHeaders();
            }

            //Initializes ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseFalconForwardedHeaders();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
                app.UseExceptionHandler("/Error");
                app.UseFalconForwardedHeaders();
            }

            app.UseHttpsRedirection();



            //STQ MODIFIED
            app.UseSaml2();
            app.UseCors(AllowAllOriginsPolicy);


            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Method == "OPTIONS")
            //    {
            //        //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //        //context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            //        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            //        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, pragma, expires, woff2, wott, woff, ttf");
            //        context.Response.Headers.Add("Access-Control-Expose-Headers", "woff2, wott, woff, ttf"); // Add this line
            //        context.Response.Headers.Add("Access-Control-Max-Age", "86400"); // 1 day
            //        context.Response.StatusCode = StatusCodes.Status204NoContent;
            //    }
            //    else
            //    {
            //        //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            //        await next();
            //    }
            //});

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();

            if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware();
            }

            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
            }

            app.UseAuthorization();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<DatabaseCheckHelper>()
                    .Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    app.UseAbpRequestLocalization();
                }
            }

            if (WebConsts.HangfireDashboardEnabled)
            {
                //Hangfire dashboard & server (Enable to use Hangfire instead of default job manager)
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[]
                        {new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard)}
                });
            }

            if (bool.Parse(_appConfiguration["Payment:Stripe:IsActive"]))
            {
                StripeConfiguration.ApiKey = _appConfiguration["Payment:Stripe:SecretKey"];
            }

            if (WebConsts.GraphQL.Enabled)
            {
                app.UseGraphQL<MainSchema>();
                if (WebConsts.GraphQL.PlaygroundEnabled)
                {
                    app.UseGraphQLPlayground(
                        new GraphQLPlaygroundOptions()); //to explorer API navigate https://*DOMAIN*/ui/playground
                }
            }

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHub<AbpCommonHub>("/signalr");
                //endpoints.MapHub<ChatHub>("/signalr-chat");

                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                }

                app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>().EndpointConfiguration
                    .ConfigureAllEndpoints(endpoints);
            });

            if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            {
                if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksUI:HealthChecksUIEnabled"]))
                {
                    app.UseHealthChecksUI();
                }
            }

            if (WebConsts.SwaggerUiEnabled)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();
                //Enable middleware to serve swagger - ui assets(HTML, JS, CSS etc.)
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(_appConfiguration["App:SwaggerEndPoint"], "Falcon API V1");
                    options.IndexStream = () => Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Syntaq.Falcon.Web.wwwroot.swagger.ui.index.html");
                    options.InjectBaseUrl(_appConfiguration["App:WebSiteRootAddress"]);
                }); //URL: /swagger
            }
        }

        private void ConfigureKestrel(IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            {
                options.Listen(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 443),
                    listenOptions =>
                    {
                        var certPassword = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Password");
                        var certPath = _appConfiguration.GetValue<string>("Kestrel:Certificates:Default:Path");
                        var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath,
                            certPassword);
                        listenOptions.UseHttps(new HttpsConnectionAdapterOptions()
                        {
                            ServerCertificate = cert
                        });
                    });
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Falcon API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.ParameterFilter<SwaggerEnumParameterFilter>();
                options.SchemaFilter<SwaggerEnumSchemaFilter>();
                options.OperationFilter<SwaggerOperationIdFilter>();
                options.OperationFilter<SwaggerOperationFilter>();
                options.CustomDefaultSchemaIdSelector();

                // Add summaries to swagger
                var canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                if (!canShowSummaries)
                {
                    return;
                }

                var mvcXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var mvcXmlPath = Path.Combine(AppContext.BaseDirectory, mvcXmlFile);
                options.IncludeXmlComments(mvcXmlPath);

                var applicationXml = $"Syntaq.Falcon.Application.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                options.IncludeXmlComments(applicationXmlPath);

                var webCoreXmlFile = $"Syntaq.Falcon.Web.Core.xml";
                var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                options.IncludeXmlComments(webCoreXmlPath);
            }); //.AddSwaggerGenNewtonsoftSupport(); // STQ MODIFIED FIX SWAGGER GENERATION ERROR
        }
    }

    //STQ MODIFIED
    public class STQSaml2Configuration : Saml2Configuration
    {
        public Boolean Enabled { get; set; }
        public String LoggedOutPage { get; set; }
    }

}
