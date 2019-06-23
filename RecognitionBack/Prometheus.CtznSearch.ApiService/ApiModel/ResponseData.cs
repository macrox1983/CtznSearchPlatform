using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.CtznSearch.ApiService.ApiModel
{
    public class ResponseData
    {
        public ResponseData()
        {
            Error = 0;
        }

        public ResponseData(int error, string message)
        {
            Error = error;
            Message = message;
        }        

        public int Error { get; set; }   
        
        public string Message { get; set; } 
    }

    public class ResponseData<T>:ResponseData
    {
        public ResponseData(T data):base(0, null)
        {
            Object = data;
        }

        public T Object { get; set; }
    }
}
