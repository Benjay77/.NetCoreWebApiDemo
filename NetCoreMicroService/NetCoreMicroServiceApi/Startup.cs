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

            //�������ע��
            services.ConfigureCors();

            //�û�У��
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = "http://localhost:5000"; // IdentityServer��������ַ
                  options.ApiName = "NetCoreMicroServiceApi"; // ������Խ��������֤��API��Դ������
                  options.RequireHttpsMetadata = false; // ָ���Ƿ�ΪHTTPS
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
                m.OAuthClientId("NetCoreMicroServiceApi");//�ͷ�������
                m.OAuthAppName("NetCoreMicroServiceApi��Ȩ"); // ����
            });

            //����
            app.UseCors("any");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
