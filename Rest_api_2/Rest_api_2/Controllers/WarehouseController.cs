using Microsoft.AspNetCore.Mvc;
using Rest_api_2.Models;
using Rest_api_2.Services;

namespace Rest_api_2.Controllers;

[Route("/api")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("/warehouses")]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductDTO product)
    {
        try
        {
            var addProduct = await _warehouseService.AddProduct(product);
            return Ok(addProduct);
        }catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        

     
    }
}