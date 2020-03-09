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
    public class BlogPostsController : ControllerBase
    {
        public IMongoCollection<BlogPost> BlogCollection { get; set; }

        public BlogPostsController(MongoOptions mongoOptions)
        {
            var client = new MongoClient(mongoOptions.ConnectionString);
            BlogCollection = client.GetDatabase(mongoOptions.Database).GetCollection<BlogPost>(nameof(BlogPost));
        }

        [HttpGet]
        public async Task<IEnumerable<BlogPost>> Get()
        {
            return (await BlogCollection.FindAsync((blog) => true)).ToList();
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<BlogPost> Get(string id)
        {
            return (await BlogCollection.FindAsync((blog) => blog.Id == id)).SingleOrDefault();
        }
    }
}
