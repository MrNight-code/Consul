namespace Consulcon.Domain.Common
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string TraceId { get; set; }

        public ErrorResponse(string errorCode, string message, string traceId)
        {
            ErrorCode = errorCode;
            Message = message;
            TraceId = traceId;
        }
    }
}
