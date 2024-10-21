
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net;

namespace pinewoodApp.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient = new HttpClient();
    public Customer[] allCustomers;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task OnGetAsync()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("User-Agent"); //cors issue
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var response = await _httpClient.GetAsync("https://localhost:7166/customer/all");
            var data = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(data))
            {
                var jsonString = await reader.ReadToEndAsync();
                TempData["allCustomers"] = jsonString;
                allCustomers = JsonSerializer.Deserialize<Customer[]>(jsonString);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
        }
    }

    public async Task OnPostCreate()
    {
        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string emailAddress = Request.Form["emailAddress"];

        if (firstName == "" || lastName == "" || emailAddress == "")
        {
            //show a error message
            await OnGetAsync();
            return;
        }

        //not done validation, running out of time

        var newCustomer = new Customer
        {
            customerId = 0,
            firstName = firstName,
            lastName = lastName,
            email = emailAddress,
            creationDate = DateTime.UtcNow
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7166/customer/create", newCustomer);

        if (response.IsSuccessStatusCode)
        {
            await OnGetAsync();
        }
    }
    public async Task OnPostDelete(int id)
    {
        var url = $"https://localhost:7166/customer/delete?customerId={id}";
        var response = await _httpClient.PostAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            await OnGetAsync();
        }
    }
}
