namespace TrackManagement.API.Response
{

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public static ApiResponse<T> Ok(T? data, string message = "")
        {
            return new ApiResponse<T>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = message,
                Data = data

            };
        }
        public static ApiResponse<T> Fail(string message , int statusCode = StatusCodes.Status400BadRequest)
        {
            return new ApiResponse<T>()
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            
            };
        }

    }
}


