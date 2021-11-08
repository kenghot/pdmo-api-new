using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using mof.DataModels.Models;
using IdentityServer4.AccessTokenValidation;
using mof.IServices;
using mof.Services;
using mof.ServiceModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Localization.SqlLocalizer.DbStringLocalizer;
using System.Reflection;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mof.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IHostingEnvironment env)
        {
            Configuration = configuration;
            if (env.IsDevelopment())
            {
                _createNewRecordWhenLocalisedStringDoesNotExist = true;
            }
        }
        private bool _createNewRecordWhenLocalisedStringDoesNotExist = false;
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           

           services.AddMvcCore()
                 .AddAuthorization()
                 .AddJsonFormatters();
            services.AddSingleton<IConfiguration>(Configuration);
            // services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
               // c.CustomSchemaIds(x => x.FullName);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "mof.ServiceModels.xml"));
                c.OperationFilter<MyHeaderFilter>();
                

            });
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var connectionLocalize = Configuration.GetConnectionString("LocalizeConnection");

            services.AddSingleton<IEmailSender, Classes.EmailSender>();
  
            services.AddDbContext<MOFContext>(options => options.UseSqlServer(connectionString));
            //localization
            #region Localization
        
            services.AddDbContext<LocalizationModelContext>(options =>
                options.UseSqlServer(
                         connectionLocalize 
                ),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton
            );
            var useTypeFullNames = false;
            var useOnlyPropertyNames = false;
            var returnOnlyKeyIfNotFound = false;
          
            // Requires that LocalizationModelContext is defined
            // _createNewRecordWhenLocalisedStringDoesNotExist read from the dev env. 
            services.AddSqlLocalization(options => options.UseSettings(
                useTypeFullNames,
                useOnlyPropertyNames,
                returnOnlyKeyIfNotFound,
                _createNewRecordWhenLocalisedStringDoesNotExist));

            // services.AddSqlLocalization(options => options.ReturnOnlyKeyIfNotFound = true);
            // services.AddLocalization(options => options.ResourcesPath = "Resources");

            //services.AddScoped<LanguageActionFilter>();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                           // new CultureInfo("en-EN"),
                            new CultureInfo("en-US"),
                            new CultureInfo("th-TH")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "th-TH");
                    options.SupportedCultures = supportedCultures;
                    //  options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders.Clear();
                    options.RequestCultureProviders.Add(new MyCultureProvider());
                    //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                    //{

                    //    //...
                         
                    //   var userLangs = context.Request.Headers["Accept-Language"].ToString();
                    //    string firstLang = "";
                    //    string culture = "th-TH";
                    
                    //    if (string.IsNullOrEmpty(userLangs))
                    //    {
                    //        firstLang = userLangs.Split(',').FirstOrDefault();
                    //        if (string.IsNullOrEmpty(firstLang))
                    //        {
                    //            if (firstLang == "en")
                    //            {
                    //                firstLang = "en-US";
                    //            }
                    //        }
                    //    }
                         
                         
                    // return Task.FromResult(new ProviderCultureResult("th-TH"));
                    //}));
                });
            #endregion
            // identity
            var idsConn = Configuration.GetSection("IdentityServerUrl").Value;
            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = idsConn;
                options.RequireHttpsMetadata = false;

                options.Audience = "api1";

            });


    //        services.AddAuthentication("token")
    //.AddOAuth2Introspection("token", options =>
    //{
    //    options.Authority = idsConn;

    //    // this maps to the API resource name and secret
    //    options.ClientId = "js";
    //    options.ClientSecret = "secret";
    //});
            services.AddDbContext<ServiceModels.Identity.ApplicationDbContext>(options => options.UseSqlServer(connectionString));
         

            services.AddIdentity<ServiceModels.Identity.ApplicationUser, ServiceModels.Identity.ApplicationRole>()
                .AddEntityFrameworkStores<ServiceModels.Identity.ApplicationDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });


            // mof services
            services.AddHttpContextAccessor();
            services.AddTransient<IIdentityRepository, IdentityRepository>();
            services.AddTransient<IOrganization, OrganizationRepository>();
            services.AddTransient<IPlan, PlanRepository>();
            services.AddTransient<IIIPM, IIPMRepository>();
            services.AddTransient<ISystemHelper, SystemHelper>();
            services.AddTransient<IProject, ProjectRepository>();
            //services.AddTransient<IFinancialReport, FinancialReportRepository>();
            services.AddTransient<IAgreement, AgreementRepository>();
            services.AddTransient<IReport, ReportRepository>();
            //services.AddTransient<IMonthlyReport, MonthlyReportRepository>();
            services.AddTransient<ICommon, CommonRepository>();
            services.AddTransient<IIIPMSync, IIPMSyncRepository>();
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = idsConn;
            //        options.RequireHttpsMetadata = true;
            //        options.ApiName = "api1";
            //    });

            services.AddCors(options =>
            {
                //options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                options.AddPolicy("AllowMyOrigin",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                   .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                     opts => { opts.ResourcesPath = "Resources"; })
                  .AddDataAnnotationsLocalization();
                //.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                //     opts => { opts.ResourcesPath = "Resources"; })
                //  .AddDataAnnotationsLocalization();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseFastReport();
            app.UseStaticFiles();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
          
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.InjectJavascript("https://ajax.googleapis.com/ajax/libs/jquery/3.4.0/jquery.min.js");
                c.InjectJavascript("/js/InjectSwagger.js");
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("mof.api.Resources.Swagger_Custom.html");
            });
            
            // XML Documentation
            

            app.UseCors("AllowMyOrigin");
            //var supportedCultures = new[]
            //{
            //     new CultureInfo("en-US"),
            //     new CultureInfo("th-TH"),
            //};
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture("th-TH"),
            //    // Formatting numbers, dates, etc.
            //    SupportedCultures = supportedCultures,
            //    // UI strings that we have localized.
            //    SupportedUICultures = supportedCultures
            //});
   
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    
    }
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class MyHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Content-Type",
                In = "header",
                Type = "string",
                Default = "application/json",
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Type = "string",
                Required = false // set to false if this is optional
            });
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Accept-Language",
                In = "header",
                Type = "string",
                Default = "th-TH",
                Required = false // set to false if this is optional
            });
            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = "api-version",
            //    In = "header",
            //    Type = "string",
            //    Default = "1.0",
            //    Required = true // set to false if this is optional
            //});
        }
    }
    public class MyCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
   
            var userLangs = httpContext.Request.Headers["Accept-Language"].ToString();
            string firstLang = "";
            string culture = "th-TH";
          
          
            if (!string.IsNullOrEmpty(userLangs))
            {
                firstLang = userLangs.Split(',').FirstOrDefault();
                if (!string.IsNullOrEmpty(firstLang))
                {
                    if (firstLang.Substring(0, 2) == "en")
                    {
                        culture = firstLang;
                    }
                }
            }
 
            return await Task.FromResult(new ProviderCultureResult(culture));
 
        }
    }
    //internal class ValidateModelAttribute : ActionFilterAttribute
    //{
        
    //    public override void OnActionExecuting(ActionExecutingContext actionContext)
    //    {
    //        if (actionContext.ModelState.IsValid == false)
    //        {
    //            actionContext.Result = new BadRequestObjectResult(
    //                actionContext.ModelState.Values
    //                    .SelectMany(e => e.Errors)
    //                    .Select(e => e.ErrorMessage));
    //        }
    //    }
    //}
}
