using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.Exceptions;

namespace WebAPI.Filters
{
    /// <summary>
    /// Filtro global para captura e tratamento de exceções.
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // Tratamento específico para exceções de domínio
            if (exception is DomainException domainException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = domainException.Message
                });

                context.ExceptionHandled = true;
                return;
            }

            // Tratamento específico para exceções de validação
            if (exception is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = validationException.Message
                });

                context.ExceptionHandled = true;
                return;
            }

            // Tratamento genérico para outras exceções
            context.Result = new ObjectResult(new
            {
                Message = "Ocorreu um erro interno no servidor.",
                Details = exception.Message
            })
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
