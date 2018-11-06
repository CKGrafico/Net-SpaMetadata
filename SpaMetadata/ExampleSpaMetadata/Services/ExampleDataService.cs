using ExampleSpaMetadata.Models;
using System.Threading.Tasks;

namespace ExampleSpaMetadata.Services
{
    public class ExampleDataService : IExampleDataService
    {
        public Task<Result> Get(string path)
        {
            // Async for example purposes
            return Task.FromResult(new Result
            {
                Title = $"Title for page {path}",
                Image = $"/assets/images/custom.png",
                Description = $"Description for page {path}",
            });
        }
    }
}
