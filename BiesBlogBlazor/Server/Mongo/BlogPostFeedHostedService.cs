using BiesBlogBlazor.Server.Hubs;
using BiesBlogBlazor.Server.Mongo.Demo.AspNetCore.Changefeed.Services.MongoDB;
using BiesBlogBlazor.Shared.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BiesBlogBlazor.Server.Mongo
{
    public class BlogPostFeedHostedService : BackgroundService
    {
        private readonly IHubContext<BlogHub, IBlogFeed> _blogHub;
        private readonly MongoOptions _mongoOptions;

        public BlogPostFeedHostedService(MongoOptions mongoOptions, IHubContext<BlogHub, IBlogFeed> blogHub)
        {
            _blogHub = blogHub;
            _mongoOptions = mongoOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoOptions.ConnectionString);
            var database = client.GetDatabase(_mongoOptions.Database);
            var collection = database.GetCollection<BlogPost>(nameof(BlogPost));
            
            var insertOperationsOnlyFilter = new EmptyPipelineDefinition<ChangeStreamDocument<BlogPost>>().Match(x => x.OperationType == ChangeStreamOperationType.Insert);
            var blogPostFeed = new MongoDbChangeStreamFeed<BlogPost>(collection, TimeSpan.FromSeconds(5));

            try
            {
                await foreach (var blog in blogPostFeed.FetchFeed(insertOperationsOnlyFilter, cancellationToken))
                {
                    await _blogHub.Clients.All.BlogPostCreated(blog);
                }
            }
            catch (OperationCanceledException)
            { }
        }
    }
}
