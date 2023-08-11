using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using FundaReport.Settings;
using FundaReport.Services;

namespace FundaReport
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        private ILogger<Startup> logger { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            CurrentEnvironment = env;
            BuildConfiguration();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete("ConfigureServices is obsolete")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            //ConfigureMVC(services, CurrentEnvironment);
            RegisterServices(services);

            // make sure this happens AFTER the Register Services method.
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(x =>
                //{
                //    x.SwaggerEndpoint("/swagger/v1/swagger.json", "FundaReport 1.0");
                //});
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(CurrentEnvironment.ContentRootPath);

            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (CurrentEnvironment.IsDevelopment())
            {
                builder.AddJsonFile($"appsettings.{CurrentEnvironment.EnvironmentName}.json", optional: true);
            }

            Configuration = builder.Build();
        }

        protected void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ReportService>();
            services.AddHttpClient<FundaHttpService>(client =>
            {
                var fundaApiSettings = Configuration.GetSection("FundaApi").Get<FundaApiSettings>();
                client.BaseAddress = new Uri(fundaApiSettings.BaseUrl);
            });

            //.ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            //{
            //    var pipInternalAPISettings = Configuration.GetSection("PIPInternalAPI").Get<PIPInternalAPISettings>();
            //    return serviceProvider.GetService<IHttpMessageHandlerBuilder>()
            //        .BuildPrimary(pipInternalAPISettings.InternalAPIUrl);
            //});

            //services.AddTransient<IHttpMessageHandlerBuilder, HttpMessageHandlerBuilder>();

            CheckConfiguration(services);
        }

        private void CheckConfiguration(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            this.logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Startup>>();
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();

            var missingConfiguration = appSettings.Value.CollectMissingConfiguration();

            if (missingConfiguration.Any())
            {
                throw new Exception(missingConfiguration);
            }
        }
    }
}
