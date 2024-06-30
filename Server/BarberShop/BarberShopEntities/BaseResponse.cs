using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public BaseResponse(T data,int statusCode,string message)
        {
            Data = data;
            StatusCode = statusCode;
            Message = message;
        }
    }
}
