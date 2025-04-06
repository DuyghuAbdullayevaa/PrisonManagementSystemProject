using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PrisonManagementSystem.BL.DTOs.ResponseModel
{
    public class GenericResponseModel<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        [JsonIgnore]
        public int StatusCode { get;  set; } 
        public List<string> Messages { get; set; }

        public static GenericResponseModel<T> SuccessResponse(T data, int statusCode, string message)
        {
            return new GenericResponseModel<T>
            {
                Success = true,
                Data = data,
                StatusCode = statusCode,
                Messages = new List<string> { message }
            };
        }

        public static GenericResponseModel<T> FailureResponse(string error, int statusCode)
        {
            return new GenericResponseModel<T>
            {
                Success = false,
                StatusCode = statusCode,
                Data = default,
                Messages = new List<string> { error }
            };
        }
    }
}
