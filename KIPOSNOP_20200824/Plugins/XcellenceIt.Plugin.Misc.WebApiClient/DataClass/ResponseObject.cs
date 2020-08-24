using Microsoft.AspNetCore.Mvc;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class ResponseObject 
    {
        private string _message { get; set; }

        public ResponseObject(string message)
        {
            _message = message;
        }

        public NotFoundObjectResult NotFound()
        {

            return new NotFoundObjectResult(new ErrorObject
            {
                Status = 404,
                ErrorMessage = _message
            });
        }

        public BadRequestObjectResult BadRequest()
        {
            return new BadRequestObjectResult(new ErrorObject
            {
                Status = 400,
                ErrorMessage = _message
            });
        }
    }
}
