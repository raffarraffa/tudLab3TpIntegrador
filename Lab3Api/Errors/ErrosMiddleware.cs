// using Microsoft.AspNetCore.Http;
// using System;
// using System.Threading.Tasks;
using System.Text.Json;

namespace Lab3Api.Errors
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;


        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);


                if (context.Response.StatusCode == 404)
                {
                    await RecursoNoEncontrado(context);
                }
            }
            catch (Exception ex)
            {
                await ExcepcionServidor(context, ex);
            }
        }

        private Task RecursoNoEncontrado(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 404,
                message = "El recurso no fue encontrado."
            }));
        }

        private Task ExcepcionServidor(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 500,
                message = "Ha ocurrido un error en el servidor.",
                detail = exception.Message
            }));
        }
    }
}
