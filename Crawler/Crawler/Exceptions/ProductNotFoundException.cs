using Crawler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public class ProductNotFoundException : WarehouseException
{
    private readonly int? _idProduct;

    public ProductNotFoundException(int? idProduct)
    {
        _idProduct = idProduct;
    }

    protected internal override IActionResult GetResponse()
    {
        var message = new ErrorMessage("Not found product with id = " + _idProduct);
        return new ObjectResult(message)
        {
            StatusCode = 404
        };
    }
}