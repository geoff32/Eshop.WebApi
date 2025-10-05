using Dapper;
using Microsoft.AspNetCore.Mvc;
using EshopApi.Models;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IDbConnectionFactory _dbFactory;
    public ProductsController(IDbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        using var db = _dbFactory.CreateConnection();
        var products = await db.QueryAsync<Product>("SELECT * FROM products");
        return Ok(products);
    }
}
