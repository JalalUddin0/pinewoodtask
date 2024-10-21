
using System.Text.Json;
public class CustomerImplementation
{

    public static async Task<(Customer?, string?)> CreateCustomer(Customer customer)
    {
        //first check for dups
        Customer c = await GetCustomerByEmail(customer.email);
        if (c != null)
        {
            return (null, "Customer email already exists");
        }

        string filePath = "data/customerdata.json";
        string jsonString = await File.ReadAllTextAsync(filePath);
        List<Customer> customersList = JsonSerializer.Deserialize<List<Customer>>(jsonString);

        //get the highest customer id and increment it
        long lastCustomerId = 0;
        if (customersList != null)
        {
            for (int i = 0; i < customersList.Count; i++)
            {
                if (customersList[i].customerId > lastCustomerId)
                {
                    lastCustomerId = customersList[i].customerId;
                }
            }
        }

        customer.customerId = lastCustomerId + 1;
        customersList.Add(customer);

        var newCustomerJson = JsonSerializer.Serialize(customersList);

        File.WriteAllText("data/customerdata.json", newCustomerJson);

        Customer? newCustomer = await GetCustomerById(lastCustomerId + 1);
        if (newCustomer == null || newCustomer.email != customer.email)
        {
            return (null, "Unable to add new customer");
        }
        return (newCustomer, null);
    }
    public static async Task<Customer?[]> GetAllCustomers()
    {
        try
        {
            string filePath = "data/customerdata.json";
            string jsonString = await File.ReadAllTextAsync(filePath);
            var customers = JsonSerializer.Deserialize<Customer[]>(jsonString);
            return customers;
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
            return null;
        }
    }
    public static async Task<Customer?> GetCustomerById(long CustomerId)
    {
        try
        {
            string filePath = "data/customerdata.json";
            string jsonString = await File.ReadAllTextAsync(filePath);
            var customers = JsonSerializer.Deserialize<Customer[]>(jsonString);

            var foundCustomer = customers.FirstOrDefault(c => c.customerId == CustomerId);

            if (foundCustomer != null)
            {
                return foundCustomer;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
            return null;
        }
    }

    public static async Task<Customer?> GetCustomerByEmail(string Email)
    {
        try
        {
            string filePath = "data/customerdata.json";
            string jsonString = await File.ReadAllTextAsync(filePath);
            var customers = JsonSerializer.Deserialize<Customer[]>(jsonString);

            var foundCustomer = customers.FirstOrDefault(c => c.email == Email);

            if (foundCustomer != null)
            {
                return foundCustomer;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
            return null;
        }
    }

    public static async Task<(List<Customer>?, string?)> UpdateCustomers(Customer[] customer)
    {
        string filePath = "data/customerdata.json";
        string jsonString = await File.ReadAllTextAsync(filePath);
        List<Customer> customersList = JsonSerializer.Deserialize<List<Customer>>(jsonString);
        List<Customer> addedCustomers = new List<Customer>();

        if (customersList == null || customersList.Count == 0)
        {
            return (null, "Customer not found");
        }

        for (int i = 0; i < customer.Length; i++)
        {
            int index = customersList.FindIndex(c => c.customerId == customer[i].customerId);
            if (index == -1)
            {
                return (null, "Customer not found");
            }
            customersList[i].firstName = customer[i].firstName;
            customersList[i].lastName = customer[i].lastName;
            customersList[i].email = customer[i].email;
            addedCustomers.Add(customersList[i]);
        }

        var newCustomerJson = JsonSerializer.Serialize(customersList);

        File.WriteAllText("data/customerdata.json", newCustomerJson);

        return (addedCustomers, null);
    }

    public static async Task<bool> DeleteCustomer(long customerId)
    {
        try
        {
            string filePath = "data/customerdata.json";
            string jsonString = await File.ReadAllTextAsync(filePath);
            List<Customer> customersList = JsonSerializer.Deserialize<List<Customer>>(jsonString);

            if (customersList == null || customersList.Count == 0)
            {
                return false;
            }

            int index = customersList.FindIndex(c => c.customerId == customerId);
            if (index == -1)
            {
                return false;
            }

            customersList.RemoveAt(index);
            var newCustomerJson = JsonSerializer.Serialize(customersList);

            File.WriteAllText("data/customerdata.json", newCustomerJson);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e);
            return false;
        }
    }
}