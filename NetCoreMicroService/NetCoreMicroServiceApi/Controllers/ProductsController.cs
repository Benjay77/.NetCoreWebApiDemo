using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreMicroServiceApi.Data;
using NetCoreMicroServiceApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreMicroServiceApi.Controllers
{
    /// <summary>
    /// 产品API
    /// </summary>
    public class ProductsController : BaseController
    {
        private readonly DataContext _dataContext;

        public ProductsController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        // GET: api/<controller>

        /// <summary>
        /// 获取产品数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = await _dataContext.Products.ToListAsync();//Products.ListAll();
            return new JsonResult(products);
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
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
