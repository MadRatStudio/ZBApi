using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Model.Common
{
    public class ApiResponse
    {
        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("response")]
        public object Response { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

    }

    public class ApiResponse<T>
    {
        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("response")]
        public T Response { get; set; }

        public static implicit operator ApiResponse<T>(ApiResponse a)
        {
            return new ApiResponse<T>
            {
                IsSuccess = a.IsSuccess,
                Message = a.Message,
                Response = (T)a.Response
            };
        }

    }
}
