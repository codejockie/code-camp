using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCodeCamp.Data;
using Newtonsoft.Json;

namespace MyCodeCamp
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }

    IConfiguration _config;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton(_config);
      services.AddDbContext<CampContext>(ServiceLifetime.Scoped);
      services.AddScoped<ICampRepository, CampRepository>();
      services.AddTransient<CampDbInitializer>();

      services.AddMvc()
              .AddJsonOptions(options => {
                options.SerializerSettings
                       .ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app,
                          IHostingEnvironment env,
                          ILoggerFactory loggerFactory,
                          CampDbInitializer seeder)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      loggerFactory.AddConsole(_config.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMvc(config =>
      {
        //config.MapRoute("default", "ap1/{controller}/{action}");
      });

      seeder.Seed().Wait();
    }
  }
}
