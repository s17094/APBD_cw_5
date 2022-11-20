using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class WarehouseNotFoundException : WarehouseException
{
    private readonly int? _idWarehouse;

    public WarehouseNotFoundException(int? idWarehouse)
    {
        _idWarehouse = idWarehouse;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Not found warehouse with id = " + _idWarehouse);
        return new ObjectResult(message)
        {
            StatusCode = 404
        };
    }
}