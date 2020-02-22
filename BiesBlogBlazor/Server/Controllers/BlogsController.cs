using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiesBlogBlazor.Server.Mongo;
using BiesBlogBlazor.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BiesBlogBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        public IMongoCollection<Blog> BlogCollection { get; set; }

        public BlogsController(MongoOptions mongoOptions)
        {
            var client = new MongoClient(mongoOptions.ConnectionString);
            BlogCollection = client.GetDatabase(mongoOptions.Database).GetCollection<Blog>(nameof(Blog));
        }

        [HttpGet]
        public async Task<IEnumerable<Blog>> Get()
        {
            return (await BlogCollection.FindAsync((blog) => true)).ToList();
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<Blog> Get(string id)
        {
            return (await BlogCollection.FindAsync((blog) => blog.Id == id)).SingleOrDefault();
        }
    }
}
