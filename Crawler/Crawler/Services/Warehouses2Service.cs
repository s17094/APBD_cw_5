using System.Data;
using System.Data.SqlClient;
using Crawler.Models;

namespace Crawler.Services;

public class Warehouses2Service : IWarehouses2Service
{

    private readonly IConfiguration _configuration;

    public Warehouses2Service(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<WarehouseProductResponse> RegisterProductAsync(WarehouseProduct warehouseProduct)
    {
        await using var connection =
            new SqlConnection(_configuration.GetConnectionString("ProductionDb"));
        await using var command = new SqlCommand("AddProductToWarehouse", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@IdProduct", warehouseProduct.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", warehouseProduct.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", warehouseProduct.Amount);
        command.Parameters.AddWithValue("@CreatedAt", warehouseProduct.CreatedAt);

        connection.Open();
        var commandResult = await command.ExecuteScalarAsync();
        return new WarehouseProductResponse(Convert.ToInt32(commandResult));
    }
}