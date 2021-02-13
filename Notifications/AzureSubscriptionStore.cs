using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace vaxalert.Stores
{
    public class AzureSubscriptionStore : ISubscriptionStore
    {
        private readonly Lazy<CosmosClient> _cosmosClient;
        private readonly IOptions<CosmosOptions> _options;

        public AzureSubscriptionStore(IOptions<CosmosOptions> options)
        {
            _cosmosClient = new Lazy<CosmosClient>(GetCosmosClient);
            _options = options;
        }

        private CosmosClient GetCosmosClient() => new CosmosClient(_options.Value.ConnectionString);

        private async Task<Container> GetContainerAsync(string container, string partitionKey)
        {
            var client = GetCosmosClient();
            var database = client.GetDatabase("VaxAlert");
            await database.CreateContainerIfNotExistsAsync(container, partitionKey);
            return database.GetContainer(container);
        }

        private Task<Container> GetSubscriptionsContainerAsync() => GetContainerAsync("Subscriptions", "/PartitionKey");

        public async Task AddSubscriptionAsync(string eventKey, Subscriber subscriber)
        {
            var container = await GetSubscriptionsContainerAsync();
            if (subscriber.Email != null)
            {
                await container.CreateItemAsync(new Subscription
                {
                    Id = $"{eventKey}:{subscriber.Email}",
                    EventKey = eventKey,
                    Email = subscriber.Email
                });
            }

            if (subscriber.Phone != null)
            {
                await container.CreateItemAsync(new Subscription
                {
                    Id = $"{eventKey}:{subscriber.Phone}",
                    EventKey = eventKey,
                    Phone = subscriber.Phone
                });
            }
        }

        public async Task<IEnumerable<Subscriber>> GetSubscribersAsync(string eventKey)
        {
            var subscriptions = await GetSubscriptionsAsync($"SELECT * FROM s WHERE s.EventKey = '{eventKey}'");
            return subscriptions.Select(x => new Subscriber { Email = x.Email, Phone = x.Phone}).ToList();
        }

        public Task<IEnumerable<Subscription>> GetSubscriptionsAsync()
        {
            return GetSubscriptionsAsync("SELECT * FROM s");
        }

        private async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(string query)
        {
            var container = await GetSubscriptionsContainerAsync();
            var subscriptions = new List<Subscription>();
            var items = container.GetItemQueryIterator<Subscription>(new QueryDefinition(query));
            while (items.HasMoreResults)
            {
                subscriptions.AddRange(await items.ReadNextAsync());
            }

            return subscriptions;
        }
    }
}