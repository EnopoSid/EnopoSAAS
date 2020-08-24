using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace XcellenceIt.Plugin.Misc.WebApiClient.DataClass
{
    public class UnprocessableEntity : ObjectResult
    {
        /// <summary>
        /// Returns 422 - Unprocessable Entity result from ModelState
        /// </summary>
        /// <param name="modelState"></param>
        public UnprocessableEntity(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 422;
        }
    }
}
