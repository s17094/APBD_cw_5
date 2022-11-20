using Crawler.Models;
using Crawler.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {

        private readonly IWarehousesService _warehousesService;

        public WarehousesController(IWarehousesService warehousesService)
        {
            _warehousesService = warehousesService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct(WarehouseProduct warehouseProduct)
        {
            var idProductWarehouse = await _warehousesService.RegisterProductAsync(warehouseProduct);
            return Ok(idProductWarehouse);
        }

    }
}
