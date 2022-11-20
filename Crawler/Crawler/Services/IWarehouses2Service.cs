using Crawler.Models;

namespace Crawler.Services;

public interface IWarehouses2Service
{
    Task<WarehouseProductResponse> RegisterProductAsync(WarehouseProduct warehouseProduct);
}