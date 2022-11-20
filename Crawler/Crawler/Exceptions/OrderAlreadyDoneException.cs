using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class OrderAlreadyDoneException : WarehouseException
{
    private readonly int? _idOrder;

    public OrderAlreadyDoneException(int? idOrder)
    {
        _idOrder = idOrder;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Order with id = " + _idOrder + " is already done");
        return new ObjectResult(message)
        {
            StatusCode = 409
        };
    }
}