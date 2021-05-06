using System;

namespace Core
{
    public class ServiceResult<TResult>
    {
        public TResult Result { get; set; }
        public bool Success { get; set; }
        public string NonSuccessMessage { get; set; }
        public Exception Exception { get; set; }

        public int Status { get; set; }

        public ServiceResult()
        {
        }

        public static ServiceResult<TResult> CreateSuccessResult(TResult result, int status = 200)
        {
            return new ServiceResult<TResult> {Success = true, Result = result, Status = status};
        }

        public static ServiceResult<TResult> CreateFailure(string nonSuccessMessage, int status = 500)
        {
            return new ServiceResult<TResult> {Success = false, NonSuccessMessage = nonSuccessMessage, Status = status};
        }

        public static ServiceResult<TResult> CreateFailure(Exception ex, int status = 500)
        {
            return new ServiceResult<TResult>
            {
                Success = false,
                Status = status,
                NonSuccessMessage = String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                Exception = ex
            };
        }
    }
    
    public class ServiceResult
    {
        public ServiceResult ()
        {
        }

        public bool Success { get; set; }
        public string NonSuccessMessage { get; set; }
        public Exception Exception { get; set; }
        
        public int Status { get; set; }

        public static ServiceResult CreateSuccessResult(int status = 200)
        {
            return new ServiceResult { Success = true, Status = status };
        }

        public static ServiceResult CreateFailure(string nonSuccessMessage, int status = 500)
        {
            return new ServiceResult { Success = false, NonSuccessMessage = nonSuccessMessage, Status = status};
        }

        public static ServiceResult CreateFailure(Exception ex, int status = 500)
        {
            return new ServiceResult
            {
                Status = status,
                Success = false,
                NonSuccessMessage = String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                Exception = ex
            };
        }
    }
}