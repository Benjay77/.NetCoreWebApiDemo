using IdentityServer.IdentityServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdentityServer.Extentions
{
    /// <summary>
    /// 服务扩展方法
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                //CorsPolicy
                options.AddPolicy("any", builder => builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }


        /// <summary>
        /// Swagger
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(m =>
            {
                m.SwaggerDoc("V1.0.0.0", new OpenApiInfo 
                {
                    Title = "IdentityServerApi", Version = "V1.0.0.0" ,
                    Description = "IdentityServer4Api",
                    Contact = new OpenApiContact
                    {
                        Name = "Benjay",
                        Email = "benjay7@163.com",
                        Url = new Uri ("https://github.com/Benjay77")
                    }
                });
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                m.IncludeXmlComments(xmlPath);
                //向生成的Swagger添加一个或多个“securityDefinitions”，用于API的登录校验

                //.NetCore2.x+Swagger4版本
                //m.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Flow = "implicit", // 只需通过浏览器获取令牌（适用于swagger）
                //    AuthorizationUrl = "http://localhost:5000/connect/authorize",//获取登录授权接口
                //    Scopes = new Dictionary<string, string> {
                //        { "demo_api", "Demo API - full access" }//指定客户端请求的api作用域。 如果为空，则客户端无法访问
                //    }
                //});

                //.NetCore3.x+Swagger5版本
                m.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        // 只需通过浏览器获取令牌（适用于swagger）
                        Implicit = new OpenApiOAuthFlow
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                ["IdentityServerApi"] = "API - full access" // //指定客户端请求的api作用域。 如果为空，则客户端无法访问
                            },
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize")//"/oauth2/authorize"获取登录授权接口
                        }
                    }
                });
                m.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new string[] {}
                    }
                });


                m.OperationFilter<AuthorizeCheckOperationFilter>(); // 添加IdentityServer4认证过滤
                m.OperationFilter<AddResponseHeadersFilter>();
                m.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // 解决相同类名会报错的问题
                m.CustomSchemaIds(type => type.FullName);
            });
        }
    }
}
