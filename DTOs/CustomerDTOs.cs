namespace SAFuneralSuppliesAPI.DTOs;

public class CreateCustomerRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Status { get; set; } = "pending";
}

public class UpdateCustomerStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class UpdateCustomerRequest
{
    public string? Email { get; set; }
    public string? CompanyName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty; // "admin" or "customer"
}

public class BulkUpdateRequest
{
    public List<int> CustomerIds { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}

public class BulkDeleteRequest
{
    public List<int> CustomerIds { get; set; } = new();
}

public class CustomerResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CustomersListResponse
{
    public List<CustomerResponse> Customers { get; set; } = new();
}
