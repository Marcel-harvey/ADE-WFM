namespace ADE_WFM.Models.DTOs
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failure(string message, IEnumerable<string>? errors = null)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Message = message,
                Errors = errors ?? Array.Empty<string>()
            };
        }
    }
}
