namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class Error
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }
    }
}