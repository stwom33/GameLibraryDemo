using Newtonsoft.Json.Linq;

namespace GameLibrary.APIs;

public interface IPriceChartingAPIClient
{
    public Task<JObject> GetPriceChartingProductByUPC(string upc);
}