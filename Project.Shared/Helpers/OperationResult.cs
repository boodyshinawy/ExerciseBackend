namespace Project.Shared
{
    public class OperationResult<T>
    {
        public OperationResult(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }

        public static OperationResult<T> Succeeded(string message = "Operation completed successfully.", T data = default)
        {
            return new OperationResult<T>(true, message, data);
        }

        public static OperationResult<T> NotFound(string message = "Item not found.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }

        public static OperationResult<T> Failed(string message = "Operation failed.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }

        public static OperationResult<T> Existed(string message = "Item already exists.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }

        public static OperationResult<T> Unauthorized(string message = "Unauthorized access.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }

        public static OperationResult<T> BadRequest(string message = "Bad request.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }

        public static OperationResult<T> InternalServerError(string message = "Internal server error.", T data = default)
        {
            return new OperationResult<T>(false, message, data);
        }
    }
}
