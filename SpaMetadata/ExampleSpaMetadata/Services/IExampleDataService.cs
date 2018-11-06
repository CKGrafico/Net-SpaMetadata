using ExampleSpaMetadata.Models;
using System.Threading.Tasks;

namespace ExampleSpaMetadata.Services
{
    public interface IExampleDataService
    {
        Task<Result> Get(string path);
    }
}
