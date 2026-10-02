using System.Threading.Tasks;

namespace SmartBiz.Desktop.Services
{
    public interface IApiService
    {
        string BaseUrl { get; }
        Task<string> GetAsync(string path);
    }
}
