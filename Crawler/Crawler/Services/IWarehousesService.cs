using Crawler.Models;

namespace Crawler.Services;

public interface IWarehousesService
{
    Task<WarehouseProductResponse> RegisterProductAsync(WarehouseProduct warehouseProduct);
}