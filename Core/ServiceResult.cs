using System;

namespace Core
{
    public class ServiceResult<TResult>
    {
        public TResult Result { get; set; }
        public bool Success { get; set; }
        public string NonSuccessMessage { get; set; }
        public Exception Exception { get; set; }

        public ServiceResult()
        {
        }

        public static ServiceResult<TResult> CreateSuccessResult(TResult result)
        {
            return new ServiceResult<TResult> {Success = true, Result = result};
        }

        public static ServiceResult<TResult> CreateFailure(string nonSuccessMessage)
        {
            return new ServiceResult<TResult> {Success = false, NonSuccessMessage = nonSuccessMessage};
        }

        public static ServiceResult<TResult> CreateFailure(Exception ex)
        {
            return new ServiceResult<TResult>
            {
                Success = false,
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

        public static ServiceResult CreateSuccessResult()
        {
            return new ServiceResult { Success = true};
        }

        public static ServiceResult CreateFailure(string nonSuccessMessage)
        {
            return new ServiceResult { Success = false, NonSuccessMessage = nonSuccessMessage};
        }

        public static ServiceResult CreateFailure(Exception ex)
        {
            return new ServiceResult
            {
                Success = false,
                NonSuccessMessage = String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                Exception = ex
            };
        }
    }
}