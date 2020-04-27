using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer.Controllers;
using IdentityServer4.EntityFramework.DbContexts;

namespace IdentityServer.Controllers
{
    /// <summary>
    /// 产品API
    /// </summary>
    public class ClientController : BaseController
    {
        private readonly ConfigurationDbContext _dataContext;

        public ClientController(ConfigurationDbContext datacontext)
        {
            _dataContext = datacontext;
        }

        // GET: api/<controller>

        /// <summary>
        /// 获取授权的Clients
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetClientsAsync()
        {
            var clients = await _dataContext.Clients.ToListAsync();
            return new JsonResult(clients);
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        
        /// <summary>
        /// 添加授权Client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpPost]
        public  IActionResult Post([FromBody]Client client)
        {
            var res =  _dataContext.Clients.Add(client);
            if (_dataContext.SaveChanges() > 0)
                return Ok(true);
            else
                return Ok(false);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
