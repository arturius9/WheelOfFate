using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SupportWheelOfFateWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupportWheelOfFateWebApi.Business_Logic;

namespace SupportWheelOfFate
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
            services.AddDbContext<WheelOfFateContext>(options => options.UseInMemoryDatabase("WheelOfFate"));
            //services.AddDbContext<WheelOfFateContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            services.AddMvc();

            //Regiseter Application Services
            services.AddScoped<IBusinessService, BusinessService>();
            services.AddScoped<IWheelOfFateContext, WheelOfFateContext>();

        }

        // This method gets     called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Web API not used");
            });
        }
    }
}
