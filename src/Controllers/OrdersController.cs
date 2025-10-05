using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EshopApi.Models;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IDbConnectionFactory _dbFactory;
    public OrdersController(IDbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] Order order)
    {
        using var db = _dbFactory.CreateConnection();
        var orderId = await db.ExecuteScalarAsync<int>(
            "INSERT INTO orders (customer_name, customer_email) VALUES (@CustomerName, @CustomerEmail) RETURNING id;",
            new { order.CustomerName, order.CustomerEmail });
        foreach (var item in order.Items)
        {
            await db.ExecuteAsync(
                "INSERT INTO order_items (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity);",
                new { OrderId = orderId, ProductId = item.ProductId, Quantity = item.Quantity });
        }
        return Created($"/api/orders/{orderId}", null);
    }
}
