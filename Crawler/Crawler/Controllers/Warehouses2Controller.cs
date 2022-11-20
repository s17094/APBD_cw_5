using Crawler.Models;
using Crawler.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Controllers
{

    [Route("api/warehouses2")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {

        private readonly IWarehouses2Service _warehouses2Service;

        public Warehouses2Controller(IWarehouses2Service warehouses2Service)
        {
            _warehouses2Service = warehouses2Service;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct(WarehouseProduct warehouseProduct)
        {
            var idProductWarehouse = await _warehouses2Service.RegisterProductAsync(warehouseProduct);
            return Ok(idProductWarehouse);
        }

    }
}
