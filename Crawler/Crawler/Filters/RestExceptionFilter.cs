using System.Data.SqlClient;
using Crawler.Exceptions;
using Crawler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Crawler.Filters;

public class RestExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is WarehouseException warehouseException)
        {
            context.Result = warehouseException.GetResponse();
        }

        if (context.Exception is SqlException sqlException)
        {
            var message = new ErrorMessage(sqlException.Message);
            if (sqlException.Class == 18)
            {
                context.Result = new ObjectResult(message)
                {
                    StatusCode = 404
                };
            }
            else
            {
                context.Result = new ObjectResult(message)
                {
                    StatusCode = 500
                };
            }
        }
    }
}