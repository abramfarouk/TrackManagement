using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrackManagement.API.Response;

namespace TrackManagement.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray());

            var response = new ApiResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}