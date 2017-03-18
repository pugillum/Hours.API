using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using HoursApi.Entities;
using HoursApi.Services;
using Microsoft.EntityFrameworkCore;

namespace HoursApi
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));

            var connectionString = Startup.Configuration["connectionStrings:hoursApiDBConnectionString"];
            //var connectionString = @"Server=(localdb)\ProjectsV12;Database=HoursApiDB;Trusted_Connection=True;";
            services.AddDbContext<HoursApiContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IHoursApiRepository, HoursApiRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, HoursApiContext hoursApiContext)
        {
            loggerFactory.AddConsole();

            loggerFactory.AddDebug();

            //loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //ursApiContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Project, Models.ProjectWithoutWorkItemsDto>();
                cfg.CreateMap<Entities.Project, Models.ProjectDto>();
                cfg.CreateMap<Entities.Project, Models.ProjectForUpdateDto>();
                cfg.CreateMap<Models.ProjectForCreationDto, Entities.Project>();
                cfg.CreateMap<Models.ProjectForUpdateDto, Entities.Project>();
                cfg.CreateMap<Entities.WorkItem, Models.WorkItemDto>();
                cfg.CreateMap<Models.WorkItemForCreationDto, Entities.WorkItem>();
                cfg.CreateMap<Models.WorkItemForUpdateDto, Entities.WorkItem>();
                cfg.CreateMap<Entities.WorkItem, Models.WorkItemForUpdateDto>();
            });

            app.UseMvc();
        }
    }
}
