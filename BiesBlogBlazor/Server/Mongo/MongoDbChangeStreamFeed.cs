using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BiesBlogBlazor.Server.Mongo
{
    namespace Demo.AspNetCore.Changefeed.Services.MongoDB
    {
        public class MongoDbChangeStreamFeed<T>
        {
            private readonly IMongoCollection<T> _collection;
            private readonly TimeSpan _retryIfNoDocumentsFoundDelay;

            public MongoDbChangeStreamFeed(IMongoCollection<T> collection, TimeSpan retryIfNoDocumentsFoundDelay)
            {
                _collection = collection;
                _retryIfNoDocumentsFoundDelay = retryIfNoDocumentsFoundDelay;
            }

            public async IAsyncEnumerable<T> FetchFeed(PipelineDefinition<ChangeStreamDocument<T>, ChangeStreamDocument<T>> filterPipeline = null, [EnumeratorCancellation]CancellationToken cancellationToken = default)
            {
                using var changeStreamFeed = await _collection.WatchAsync(filterPipeline, cancellationToken: cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    while (await changeStreamFeed.MoveNextAsync(cancellationToken))
                    {
                        using var changeStreamFeedCurrentEnumerator = changeStreamFeed.Current.GetEnumerator();

                        while (changeStreamFeedCurrentEnumerator.MoveNext())
                        {
                            yield return changeStreamFeedCurrentEnumerator.Current.FullDocument;
                        }
                    }

                    await Task.Delay(_retryIfNoDocumentsFoundDelay, cancellationToken);
                }
            }
        }
    }
}