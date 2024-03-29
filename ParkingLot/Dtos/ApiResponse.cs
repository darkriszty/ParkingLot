﻿namespace ParkingLot.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public T Response { get; set; }

        public static ApiResponse<T> FailResponse(string message)
        {
            return new ApiResponse<T>
            {
                ErrorMessage = message,
                Success = false
            };
        }

        public static ApiResponse<T> SuccessResult(T response)
        {
            return new ApiResponse<T> {Success = true, Response = response};
        }
    }
}
