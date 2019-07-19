using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Configuration;

namespace Custom.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AzureADOptions options = new AzureADOptions();

            //"AzureAd" is the name of section in AppSettings.Config
            Configuration.Bind("AzureAd", options);

            // Make sure the Name "AzureAdAuthenticationScheme" is same in Ocelot.json
            services.AddAuthentication()
               .AddJwtBearer("AzureAdAuthenticationScheme", x =>
               {
                   x.Authority = $"{options.Instance}/{options.TenantId}";
                   x.RequireHttpsMetadata = false;
                   x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                   {
                       //keep on adding the valid client ids of backend apis here. 
                       //If gateway has to support new services in future, add the client id of each backend api
                       ValidAudiences = new[] { options.ClientId}
                   };
               });

            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseOcelot();
        }
    }
}
