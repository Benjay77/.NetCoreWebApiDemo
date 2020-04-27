using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using IdentityServer4.AccessTokenValidation;
using IdentityServer.Extentions;
using Microsoft.AspNetCore.Http;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //IdentityServer
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                //            sql => sql.MigrationsAssembly(migrationsAssembly));
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                //            sql => sql.MigrationsAssembly(migrationsAssembly));

                //    options.EnableTokenCleanup = false;//�Ƿ�����ݿ�����������ݣ�Ĭ��Ϊfalse
                //    options.TokenCleanupInterval = 300;//���ƹ���ʱ�䣬Ĭ��Ϊ3600�룬һ��Сʱ
                //})
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            services.AddMvc(MvcOptions => 
            { 
                MvcOptions.EnableEndpointRouting = false; 
            });

            //�û�У��
            services.AddAuthentication()
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = "http://localhost:5000"; // IdentityServer��������ַ
                  options.ApiName = "IdentityServerApi"; // ������Խ��������֤��API��Դ������
                  options.RequireHttpsMetadata = false; // ָ���Ƿ�ΪHTTPS
              });

            services.ConfigureSwagger();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //IdentityServer
            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.UseAuthentication();

            //����
            app.UseCors("any");

            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(m =>
            {
                m.SwaggerEndpoint("/swagger/V1.0.0.0/swagger.json", "IdentityServerApi-V1.0.0.0");
                //m.InjectJavascript("/Scripts/Swagger.js");
                m.OAuthClientId("IdentityServerApi");//�ͻ�������
                m.OAuthAppName("IdentityServerApi��Ȩ"); // ����
            });

            //InitializeDatabase(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// ��ʼ�����ݿ�
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

    }
}
