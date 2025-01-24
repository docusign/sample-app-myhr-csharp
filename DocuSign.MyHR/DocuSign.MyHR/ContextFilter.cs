using Microsoft.AspNetCore.Mvc.Filters;

namespace DocuSign.MyHR
{
    public class ContextFilter : IActionFilter
    {
        private readonly Context _context;

        public ContextFilter(Context context)
        {
            _context = context;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                _context.Init(httpContext.User);
            }
        }
    }
}