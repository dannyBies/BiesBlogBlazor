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
    public class BlogFeedHostedService : BackgroundService
    {
        private readonly IHubContext<BlogHub, IBlogFeed> _blogHub;
        private readonly MongoOptions _mongoOptions;

        public BlogFeedHostedService(IHubContext<BlogHub, IBlogFeed> blogHub, MongoOptions mongoOptions)
        {
            _blogHub = blogHub;
            _mongoOptions = mongoOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = new MongoClient(_mongoOptions.ConnectionString);
            var database = client.GetDatabase(_mongoOptions.Database);
            var collection = database.GetCollection<Blog>(nameof(Blog));
            
            var insertOperationsOnlyFilter = new EmptyPipelineDefinition<ChangeStreamDocument<Blog>>().Match(x => x.OperationType == ChangeStreamOperationType.Insert);
            var blogFeed = new MongoDbChangeStreamFeed<Blog>(collection, TimeSpan.FromSeconds(5));

            try
            {
                await foreach (Blog blog in blogFeed.FetchFeed(insertOperationsOnlyFilter, cancellationToken))
                {
                    await _blogHub.Clients.All.BlogCreated(blog);
                }
            }
            catch (OperationCanceledException)
            { }
        }
    }
}
