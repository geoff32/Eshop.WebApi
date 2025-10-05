using Dapper;
using Microsoft.AspNetCore.Mvc;
using EshopApi.Models;

[ApiController]
[Route("api/classes")]
public class ClassesController : ControllerBase
{
    private readonly IDbConnectionFactory _dbFactory;
    public ClassesController(IDbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Classe>>> GetClasses()
    {
        using var db = _dbFactory.CreateConnection();
        var classes = await db.QueryAsync<Classe>("SELECT * FROM classes");
        return Ok(classes);
    }
}
