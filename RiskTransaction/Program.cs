using Newtonsoft.Json;

class Program
{
    public static async Task Main()
    {
        var result = await GetRiskTransaction("853793d552635f533aa982b92b35b00e63a1c1add062c099da2450a15119bcb2");
        Console.WriteLine(result);
    }
    private static async Task<bool> GetRiskTransaction(string transactionHash)
    {
        HttpClient client = new HttpClient();

        string url = $"https://apilist.tronscanapi.com/api/transaction-info?hash={transactionHash}";

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            dynamic transaction = JsonConvert.DeserializeObject(json);

            bool isHighRisk = Convert.ToBoolean(transaction.riskTransaction) || CheckNormalAddresses(transaction.normalAddressInfo);

            return isHighRisk;
        }
        else
        {
            throw new Exception($"Failed to get transaction details: {response.StatusCode}");
        }
    }

    private static bool CheckNormalAddresses(dynamic normalAddressInfo)
    {
        foreach (var addressInfo in normalAddressInfo)
        {
            if ((bool)addressInfo.Value.risk)
            {
                return true;
            }
        }
        return false;
    }
}