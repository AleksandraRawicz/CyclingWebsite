using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CyclingWebsite.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CyclingWebsite.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (ForbidException ex)
            {
                context.Response.StatusCode = 403;
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
            }
            throw new NotImplementedException();
        }
    }

    
}
