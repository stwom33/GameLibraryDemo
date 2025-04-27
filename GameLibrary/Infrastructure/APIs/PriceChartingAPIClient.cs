
using GameLibrary.Infrastructure;
using Newtonsoft.Json.Linq;

namespace GameLibrary.APIs;

public class PriceChartingAPIClient : IPriceChartingAPIClient
{

    private static readonly string _token = "c0b53bce27c1bdab90b1605249e600dc43dfd1d5";

    private static HttpClient apiClient = new()
    {
        BaseAddress = new Uri("https://www.pricecharting.com/api/"),
    };

    public PriceChartingAPIClient() {}

    public async Task<JObject> GetPriceChartingProductByUPC(string upc)
    {
        // {
        //     "status": "success",              // response status
        //     "cib-price": 42995,               // $429.95
        //     "console-name": "Super Nintendo",
        //     "id": "6910",                     // unique PriceCharting product ID
        //     "loose-price": 17244,             // $172.44
        //     "new-price": 53000,               // $530.00
        //     "product-name": "EarthBound",
        //     "release-date": "1995-06-05"      // 5 June 1995
        // }

        try
        {
            using HttpResponseMessage response = await apiClient.GetAsync($"product?t={_token}&upc={upc}");
                
            response.EnsureSuccessStatusCode();
        
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");

            return new JObject(jsonResponse);
        }
        catch(Exception ex)
        {
            //log the ex
            return new JObject("{\"status\": \"error\"}");
        }

    }
}