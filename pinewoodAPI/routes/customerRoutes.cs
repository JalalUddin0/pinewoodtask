using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
public static class CustomerRoutes
{
    public static void SetCustomerRoutes(this WebApplication app)
    {

        app.MapPost("/customer/create", async (HttpContext httpContext, Customer customer) =>
        {
            DateTime now = DateTime.UtcNow;
            customer.creationDate = now;

            (Customer?, string?) res = await CustomerImplementation.CreateCustomer(customer);
            if (res.Item2 != null)
            {
                Console.WriteLine("error: " + res.Item2);
                return Results.Problem(res.Item2);
            }

            return Results.Ok(res.Item1);
        });

        app.MapGet("/customer/all", async () =>
        {
            Customer?[] res = await CustomerImplementation.GetAllCustomers();
            return res;
        });

        app.MapGet("/customer/{id}", async (HttpContext httpContext) =>
        {
            long CustomerId = long.Parse(httpContext.Request.RouteValues["id"].ToString());
            if (CustomerId < 0)
            {
                return Results.BadRequest();
            }

            Customer? res = await CustomerImplementation.GetCustomerById(CustomerId);
            if (res != null)
            {
                return Results.Ok(res);
            }

            return Results.NotFound();
        });

        app.MapPost("/customer/update", async (HttpContext httpContext, Customer[] customer) =>
        {
            (List<Customer>?, string?) res = await CustomerImplementation.UpdateCustomers(customer);
            if (res.Item2 != null)
            {
                Console.WriteLine("error: " + res.Item2);
                return Results.Problem(res.Item2);
            }

            return Results.Ok(res.Item1);
        });

        app.MapPost("/customer/delete", async (HttpContext httpContext, long customerId) =>
        {
            bool res = await CustomerImplementation.DeleteCustomer(customerId);
            if (res == false)
            {
                return Results.Problem("unable to remove");
            }

            return Results.Ok("removed");
        });
    }
}