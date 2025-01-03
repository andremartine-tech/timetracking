using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters
{
    /// <summary>
    /// Filtro para validar automaticamente os modelos nas requisições.
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Erro(s) de validação encontrados.",
                    Errors = errors
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Não é necessário implementar neste caso.
        }
    }
}
