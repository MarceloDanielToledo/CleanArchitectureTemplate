using System.Runtime.Serialization;

namespace Application.Exceptions
{
    public class ApplicationException : Exception
    {
        public int StatusCode { get; }
        public List<string> Errors { get; private set; } = new();
        public ApplicationException(int statuscode, List<string> errors) : base()
        {
            StatusCode = statuscode;
            Errors = errors;
        }
        public ApplicationException(int statuscode, string error) : base()
        {
            StatusCode = statuscode;
            Errors.Add(error);
        }
        public ApplicationException(string error) : base()
        {
            Errors.Add(error);
        }
        public ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
