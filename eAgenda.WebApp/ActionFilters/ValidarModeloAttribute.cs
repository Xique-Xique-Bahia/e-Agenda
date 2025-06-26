using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eAgenda.WebApp.ActionFilters;

public class ValidarModeloAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Lógica ANTES da execução do método de ação
        var modelState = context.ModelState;

        if (!modelState.IsValid)
        {
            var controller = (Controller)context.Controller;

            var viewModel = context.ActionArguments
                .Values
                .FirstOrDefault(x => x.GetType().Name.EndsWith("ViewModel"));

            context.Result = controller.View(viewModel);
        }
    }
}