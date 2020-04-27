using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreMicroServiceApi.Data;
using Microsoft.EntityFrameworkCore;
using NetCoreMicroServiceApi.Extentions;
using IdentityServer4.AccessTokenValidation;

namespace NetCoreShopMicroService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SqlHelper.ConStr = Configuration["ConStr"];

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //跨域服务注册
            services.ConfigureCors();

            //用户校验
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = "http://localhost:5000"; // IdentityServer服务器地址
                  options.ApiName = "NetCoreMicroServiceApi"; // 用于针对进行身份验证的API资源的名称
                  options.RequireHttpsMetadata = false; // 指定是否为HTTPS
              });

            services.ConfigureSwagger();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(m => 
            {
                m.SwaggerEndpoint("/swagger/V1.0.0.0/swagger.json", "NetCoreMicroServiceApi-V1.0.0.0");
                m.InjectJavascript("/Scripts/Swagger.js");
                m.OAuthClientId("NetCoreMicroServiceApi");//客服端名称
                m.OAuthAppName("NetCoreMicroServiceApi授权"); // 描述
            });

            //跨域
            app.UseCors("any");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
