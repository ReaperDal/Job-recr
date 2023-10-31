using System.Net.Http;
using System.Threading.Tasks;
using Job_Recruitment.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class Privat24Controller : Controller
{
    private readonly HttpClient _client;

    public Privat24Controller()
    {
        _client = new HttpClient();
    }

    public async Task<IActionResult> GetExchangeRates()
    {
        var response = await _client.GetAsync("https://api.privatbank.ua/p24api/exchange_rates?date=01.12.2014");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error", "Failed to fetch exchange rates."); 
        }

        var ratesJson = await response.Content.ReadAsStringAsync();
        var rates = JsonConvert.DeserializeObject<ExchangeRate>(ratesJson);

        return View("ExchangeRates", rates); 
    }
}
