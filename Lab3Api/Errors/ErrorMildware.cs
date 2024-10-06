using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Lab3Api.Middlewares
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
            await _next(context);


            var statusCode = context.Response.StatusCode;
            string message;

            switch (statusCode)
            {
                case 404:
                    message = "El recurso que buscas no se ha encontrado.";
                    break;
                case 403:
                    message = "No tienes permiso para acceder a este recurso.";
                    break;
                case 500:
                    message = "Se ha producido un error en el servidor.";
                    break;
                default:
                    message = "Ha ocurrido un error inesperado.";
                    break;
            }


            if (statusCode >= 400)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = message
                }));
            }
        }
    }
}
