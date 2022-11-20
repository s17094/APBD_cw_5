using Microsoft.AspNetCore.Mvc;

namespace Crawler.Exceptions;

public abstract class WarehouseException : Exception
{
    protected internal abstract IActionResult GetResponse();
}