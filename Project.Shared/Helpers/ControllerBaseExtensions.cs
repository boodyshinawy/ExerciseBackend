using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Project.Shared
{
    public static class ControllerBaseExtensions
    {
        public async static Task<IActionResult> GetPagedDataAsync<T>(this ControllerBase controller, IQueryable<T> data, int currentPage = 1, int pageSize = 10, dynamic info = null) where T : class
        {
            if (pageSize > 100)
            {
                pageSize = 100;
            }
            currentPage--;
            currentPage = Math.Max(currentPage, 0);
            var startIndex = pageSize * currentPage;
            var pagedData = await data.Skip(startIndex).Take(pageSize).ToListAsync();

            if (currentPage == 0)
            {
                var itemsCount = await data.CountAsync();
                var pagesCount = Math.Ceiling(itemsCount / (double)pageSize);
                return controller.Ok(new
                {
                    message = "Paged data retrieved successfully",
                    data = new
                    {
                        Items = pagedData,
                        ItemsCount = itemsCount,
                        PagesCount = pagesCount,
                        Info = info
                    }
                });
            }

            return controller.Ok(OperationResult<IEnumerable<T>>.Succeeded("Paged data retrieved successfully", pagedData));
        }


        public static IActionResult AppNotFound<T>(this ControllerBase controller,string message)
        {
            return controller.NotFound(OperationResult<T>.NotFound(message));
        }

        public static IActionResult AppSuccess<T>(this ControllerBase controller,string message = "Operation completed successfully.", T data =default)
        {
            return controller.Ok(OperationResult<T>.Succeeded(message,data));
        }

        public static IActionResult AppSuccess<T>(this ControllerBase controller)
        {
            return controller.Ok(OperationResult<T>.Succeeded());
        }
        public static IActionResult AppSuccess<T>(this ControllerBase controller, OperationResult<T> result)
        {
            return controller.Ok(result);
        }

        public static IActionResult AppFailed<T>(this ControllerBase controller, string message = "[ERROR] We have an error, please try again!")
        {
            return controller.BadRequest(OperationResult<T>.Failed(message));
        }

        public static IActionResult AppUnauthorized<T>(this ControllerBase controller, string message = "Unauthorized access!")
        {
            return controller.Unauthorized(OperationResult<T>.Unauthorized(message));
        }



        public static IActionResult AppInternalServerError<T>(this ControllerBase controller, string message = "Internal server error!")
        {
            return controller.StatusCode(500, OperationResult<T>.InternalServerError(message));
        }
    }
}
