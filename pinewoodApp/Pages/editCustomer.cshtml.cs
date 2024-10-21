
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net;

namespace pinewoodApp.Pages;

public class EditCustomerModel : PageModel
{
    private readonly HttpClient _httpClient = new HttpClient();
    public Customer customer = new Customer();
    public Customer updatedCustomer;
    private readonly ILogger<IndexModel> _logger;

    public EditCustomerModel(ILogger<IndexModel> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task OnGetAsync(long id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("User-Agent");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var response = await _httpClient.GetAsync($"https://localhost:7166/customer/{id}");
            var data = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(data))
            {
                var jsonString = await reader.ReadToEndAsync();
                TempData["updateCustomer"] = jsonString;
                customer = JsonSerializer.Deserialize<Customer>(jsonString);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
        }
    }
    public async Task OnPostUpdate()
    {
        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string emailAddress = Request.Form["emailAddress"];

        //not done validation, running out of time

        customer = JsonSerializer.Deserialize<Customer>(TempData["updateCustomer"].ToString());

        var newCustomer = new Customer
        {
            customerId = customer.customerId,
            firstName = firstName != "" ? firstName : customer.firstName,
            lastName = lastName != "" ? lastName : customer.lastName,
            email = emailAddress != "" ? emailAddress : customer.email,
            creationDate = customer.creationDate
        };

        Customer[] updateArray = { newCustomer };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7166/customer/update", updateArray);

        RedirectToRoute("Index");
    }
}
