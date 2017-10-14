using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data.Entities;

namespace MyCodeCamp.Data
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json");

      _config = builder.Build();
    }

    IConfigurationRoot _config;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton(_config);

      // When used with ASP.net core, add these lines to Startup.cs
      var connectionString = _config.GetConnectionString("Data:ConnectionString");
      services.AddEntityFrameworkNpgsql()
              .AddDbContext<CampContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Scoped);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
    {
      app.UseAuthentication();
    }
  }
}
