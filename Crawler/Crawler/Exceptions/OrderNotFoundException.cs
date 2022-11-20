using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class OrderNotFoundException : WarehouseException
{
    private readonly int? _idProduct;
    private readonly int? _amount;
    private readonly DateTime? _createdAt;

    public OrderNotFoundException(int? idProduct, int? amount, DateTime? createdAt)
    {
        _idProduct = idProduct;
        _amount = amount;
        _createdAt = createdAt;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Not found order with idProduct = " + _idProduct
            + ", amount = " + _amount + ", and created time less than " + _createdAt);
        return new ObjectResult(message)
        {
            StatusCode = 404
        };
    }
}