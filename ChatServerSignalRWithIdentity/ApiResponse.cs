using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Details { get; set; }

        public ApiResponse()
        {
            Status = HttpStatusCode.OK;
        }

        public ApiResponse(T data)
        {
            Data = data;
            Status = HttpStatusCode.OK;
        }

        public ApiResponse(HttpStatusCode httpStatusCode, string details)
        {
            Status = httpStatusCode;
            Details = details;
        }
    }
}
