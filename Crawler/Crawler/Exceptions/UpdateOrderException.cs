using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class UpdateOrderException : WarehouseException
{
    private readonly int? _idOrder;

    public UpdateOrderException(int? idOrder)
    {
        _idOrder = idOrder;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Unexpected error occured while updating order with id = " + _idOrder);
        return new ObjectResult(message)
        {
            StatusCode = 500
        };
    }
}