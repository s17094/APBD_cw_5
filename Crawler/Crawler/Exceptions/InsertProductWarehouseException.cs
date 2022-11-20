using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class InsertProductWarehouseException : WarehouseException
{
    private readonly int? _idProduct;
    private readonly int? _idWarehouse;

    public InsertProductWarehouseException(int? idProduct, int? idWarehouse)
    {
        _idProduct = idProduct;
        _idWarehouse = idWarehouse;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Unexpected error occured while inserting product with id = " + _idProduct
            + " to warehouse with id = " + _idWarehouse);
        return new ObjectResult(message)
        {
            StatusCode = 500
        };
    }
}