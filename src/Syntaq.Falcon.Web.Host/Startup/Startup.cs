using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Abp.Hangfire;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Identity;
using Syntaq.Falcon.Web.Chat.SignalR;
using Syntaq.Falcon.Web.Common;
using Swashbuckle.AspNetCore.Swagger;
using Syntaq.Falcon.Web.IdentityServer;
using Syntaq.Falcon.Web.Swagger;
using Stripe;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using HealthChecks.UI.Client;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Syntaq.Falcon.Configure;
using Syntaq.Falcon.Schemas;
using Syntaq.Falcon.Web.HealthCheck;
using Newtonsoft.Json.Serialization;
using Owl.reCAPTCHA;
using HealthChecksUISettings = HealthChecks.UI.Configuration.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Https;
//STQ MODIFIED
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http;
using Abp.Timing;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Syntaq.Falcon.Filters;

namespace Syntaq.Falcon.Web.Startup
{
    public class Startup
    {
        private const string AllowAllOriginsPolicy = "AllowAll";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            //STQ MODIFIED
            Clock.Provider = ClockProviders.Utc;
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            // Configure HSTS
            // https://aka.ms/aspnetcore-hsts
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security
            //services.AddHsts(options =>
            //{
            //    options.MaxAge = TimeSpan.FromDays(90);
            //    options.IncludeSubDomains = true;
            //    options.Preload = true;
            //});

            //// Configure HTTPS redirection
            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            //    options.HttpsPort = 443;
            //});


            // Forward Headers to provide support for Linux hosted WebApp
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

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //MVC
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());

                // STQ MODIFIED
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

            }).AddNewtonsoftJson(options => {
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

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsPolicy,
                builder =>
                {
                    //builder.AllowAnyOrigin()
                    builder.WithOrigins("https://procurement.govt.nz", "https://nzgp-prod.sites.silverstripe.com", "https://prod.procurement.govt.nz", "https://nzgp-uat.sites.silverstripe.com", "https://uat.procurement.govt.nz", "https://nzgp2-prod.sites.silverstripe.com", "https://nzgp2-uat.sites.silverstripe.com", "http://local.procurement.govt.nz", "http://local.nzgp.govt.nz", "http://procurement.test", "http://nzgp.test")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("woff2")
                            .WithExposedHeaders("woff")
                            .WithExposedHeaders("ttf");
                });
            });

            services.AddScoped<Syntaq.Falcon.Filters.DateTimeZoneNormailser>();

            services.AddSignalR();
            if (bool.Parse(_appConfiguration["KestrelServer:IsEnabled"]))
            {
                ConfigureKestrel(services);
            }

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration, options =>
                    options.UserInteraction = new UserInteractionOptions()
                    {
                        LoginUrl = "/UI/Login",
                        LogoutUrl = "/UI/LogOut",
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

            if (_appConfiguration.GetValue<bool>("SwaggerUi:Enabled"))
            {
                //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
                ConfigureSwagger(services);
            }

            //Recaptcha
            services.AddreCAPTCHAV3(x =>
            {
                x.SiteKey = _appConfiguration["Recaptcha:SiteKey"];
                x.SiteSecret = _appConfiguration["Recaptcha:SecretKey"];
            });

            if (WebConsts.HangfireDashboardEnabled)
            {
                //Hangfire(Enable to use Hangfire instead of default job manager)
                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
                });

                services.AddHangfireServer();
            }

            if (WebConsts.GraphQL.Enabled)
            {
                services.AddAndConfigureGraphQL();
            }

            // This section is throwing an error Todo
            //if (bool.Parse(_appConfiguration["HealthChecks:HealthChecksEnabled"]))
            //{
            //    services.AddAbpZeroHealthCheck();

            //    var healthCheckUISection = _appConfiguration.GetSection("HealthChecks")?.GetSection("HealthChecksUI");

            //    if (bool.Parse(healthCheckUISection["HealthChecksUIEnabled"]))
            //    {
            //        services.Configure<HealthChecksUISettings>(settings =>
            //        {
            //            healthCheckUISection.Bind(settings, c => c.BindNonPublicProperties = true);
            //        });
            //        services.AddHealthChecksUI()
            //            .AddInMemoryStorage();
            //    }
            //}

            //Configure Abp and Dependency Injection
            return services.AddAbp<FalconWebHostModule>(options =>
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

            //////////////////////////////////////////////////////////////////
            // Start STQ Modifications
            /////////////////////////////////////////////////////////////////
            //app.UseCookiePolicy(
            //    new CookiePolicyOptions
            //    {
            //        Secure = CookieSecurePolicy.Always,
            //        HttpOnly =  Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            //    });

            //////////////////////////////////////////////////////////////////
            // END STQ Modifications
            /////////////////////////////////////////////////////////////////


            // Security - Registered before static files to always set header

            if (string.Equals(
    Environment.GetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED"),
    "true", StringComparison.OrdinalIgnoreCase))
            {
                app.UseForwardedHeaders();
            }

            app.UseHsts(hsts => hsts.MaxAge(365));
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());


            app.UseStaticFiles();

            // Security - Registered after static files, for dynamic content.
            app.UseXfo(xfo => xfo.Deny());
            app.UseRedirectValidation();
            app.UseXXssProtection(options => options.EnabledWithBlockMode());

            //Initializes ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
                app.UseExceptionHandler("/Error");
            }


            //STQ MODIFIED
            app.UseCors(AllowAllOriginsPolicy);

            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.Method == "OPTIONS")
            //    {
            //        //context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // SET IN WEB.CONFIG
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
            app.UseJwtTokenMiddleware();

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
                //Hangfire dashboard &server(Enable to use Hangfire instead of default job manager)
                app.UseHangfireDashboard(WebConsts.HangfireDashboardEndPoint, new DashboardOptions
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
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapHub<ChatHub>("/signalr-chat");

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

                app.ApplicationServices.GetRequiredService<IAbpAspNetCoreConfiguration>().EndpointConfiguration.ConfigureAllEndpoints(endpoints);
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
                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(_appConfiguration["App:SwaggerEndPoint"], "Falcon API V1");
                    options.IndexStream = () => Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Syntaq.Falcon.Web.wwwroot.swagger.ui.index.html");
                    options.InjectBaseUrl(_appConfiguration["App:ServerRootAddress"]);
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

                //add summaries to swagger
                bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                if (canShowSummaries)
                {
                    var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                    options.IncludeXmlComments(hostXmlPath);

                    var applicationXml = $"Syntaq.Falcon.Application.xml";
                    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                    options.IncludeXmlComments(applicationXmlPath);

                    var webCoreXmlFile = $"Syntaq.Falcon.Web.Core.xml";
                    var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                    options.IncludeXmlComments(webCoreXmlPath);
                }
            }); //.AddSwaggerGenNewtonsoftSupport();  // STQ MODIFIED FIX SWAGGER GENERATION ERROR
        }
    }

}
