using System.Data.SqlClient;
using Crawler.Exceptions;
using Crawler.Models;

namespace Crawler.Services;

public class WarehousesService : IWarehousesService
{
    private readonly IConfiguration _configuration;

    public WarehousesService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<WarehouseProductResponse> RegisterProductAsync(WarehouseProduct warehouseProduct)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("ProductionDb"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction) transaction;
        try
        {
            var price = await GetProductPriceAsync(command, warehouseProduct.IdProduct);
            await VerifyWarehouseAsync(command, warehouseProduct.IdWarehouse);
            var idOrder = await GetValidOrderIdAsync(command, warehouseProduct.IdProduct,
                warehouseProduct.Amount,
                warehouseProduct.CreatedAt);
            await UpdateOrderFullFilledAtFieldAsync(command, idOrder);
            var idProductWarehouse =
                await InsertProductWarehouseAsync(command, warehouseProduct, idOrder, price);
            await transaction.CommitAsync();
            return new WarehouseProductResponse(idProductWarehouse);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<double> GetProductPriceAsync(SqlCommand command, int? idProduct)
    {
        command.CommandText = "select Price from Product where IdProduct = @idProduct";
        command.Parameters.AddWithValue("@idProduct", idProduct);

        double price;

        await using var sqlDataReader = await command.ExecuteReaderAsync();
        if (await sqlDataReader.ReadAsync())
        {
            price = double.Parse(sqlDataReader["Price"].ToString());
        }
        else
        {
            throw new ProductNotFoundException(idProduct);
        }
        command.Parameters.Clear();

        return price;
    }
    
    private async Task VerifyWarehouseAsync(SqlCommand command, int? idWarehouse)
    {
        command.CommandText = "select IdWarehouse from Warehouse where IdWarehouse = @idWarehouse";
        command.Parameters.AddWithValue("@idWarehouse", idWarehouse);
        await using var sqlDataReader = await command.ExecuteReaderAsync();
        if (!sqlDataReader.HasRows)
        {
            throw new WarehouseNotFoundException(idWarehouse);
        }
        command.Parameters.Clear();
    }
    
    private async Task<int?> GetValidOrderIdAsync(SqlCommand command, int? idProduct,
        int? amount, DateTime? createdAt)
    {
        var idOrder = await GetOrderIdAsync(command, idProduct, amount, createdAt);
        if (idOrder == null)
        {
            throw new OrderNotFoundException(idProduct, amount, createdAt);
        }

        if (await IsOrderAlreadyDoneAsync(command, idOrder))
        {
            throw new OrderAlreadyDoneException(idOrder);
        }

        return idOrder;
    }

    private async Task<int?> GetOrderIdAsync(SqlCommand command, int? idProduct, int? amount, DateTime? createdAt)
    {
        command.CommandText = "select * from [Order] " +
                              "where IdProduct = @idProduct " +
                              "and Amount = @amount " +
                              "and CreatedAt < @createdAt";
        command.Parameters.AddWithValue("@idProduct", idProduct);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@createdAt", createdAt);

        await using var sqlDataReader = await command.ExecuteReaderAsync();
        
        command.Parameters.Clear();
        
        if (await sqlDataReader.ReadAsync())
        {
            return int.Parse(sqlDataReader["IdOrder"].ToString());
        }

        return null;
    }

    private async Task<bool> IsOrderAlreadyDoneAsync(SqlCommand command, int? idOrder)
    {
        command.CommandText = "select * from Product_Warehouse where IdOrder = @idOrder";
        command.Parameters.AddWithValue("@idOrder", idOrder);
        await using var sqlDataReader = await command.ExecuteReaderAsync();

        command.Parameters.Clear();

        if (sqlDataReader.HasRows)
        {
            return true;
        }

        return false;
    }

    private async Task UpdateOrderFullFilledAtFieldAsync(SqlCommand command, int? idOrder)
    {
        command.CommandText = "update [Order] set FulfilledAt = getdate() where IdOrder = @idOrder";
        command.Parameters.AddWithValue("@idOrder", idOrder);
        if (await command.ExecuteNonQueryAsync() == 0)
        {
            throw new UpdateOrderException(idOrder);
        }
        command.Parameters.Clear();
    }

    private async Task<int> InsertProductWarehouseAsync(SqlCommand command, WarehouseProduct warehouseProduct, int? idOrder, double price)
    {
        command.CommandText =
            "insert into Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
            "output inserted.IdProductWarehouse " +
            "values (@idWarehouse, @idProduct, @idOrder, @amount, @price, getdate())";
        command.Parameters.AddWithValue("@idWarehouse", warehouseProduct.IdWarehouse);
        command.Parameters.AddWithValue("@idProduct", warehouseProduct.IdProduct);
        command.Parameters.AddWithValue("@idOrder", idOrder);
        command.Parameters.AddWithValue("@amount", warehouseProduct.Amount);
        command.Parameters.AddWithValue("@price", warehouseProduct.Amount * price);

        var commandResult = await command.ExecuteScalarAsync();
        commandResult = (commandResult == DBNull.Value) ? null : commandResult;
        if (commandResult == null)
        {
            throw new InsertProductWarehouseException(warehouseProduct.IdProduct, warehouseProduct.IdWarehouse);
        }

        return Convert.ToInt32(commandResult);
    }
}