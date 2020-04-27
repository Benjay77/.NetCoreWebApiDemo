using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreMicroServiceApi.IdentityServer
{
    /// <summary>
    /// IdentityServer4认证过滤器
    /// </summary>
    public class AuthorizeCheckOperationFilter: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //获取是否添加登录特性
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
             .Union(context.MethodInfo.GetCustomAttributes(true))
             .OfType<AuthorizeAttribute>().Any();

            if (authAttributes)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "暂无访问权限" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "禁止访问" });
                //给api添加锁的标注

                //.NetCore2.x+Swagger4版本
                //operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                //{
                //    new Dictionary<string, IEnumerable<string>> {{"oauth2", new[] {"demo_api"}}}
                //};
            }
        }
    }
}
