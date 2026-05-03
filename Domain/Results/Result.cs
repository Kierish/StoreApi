namespace Domain.Results
{
    public class Result<T> 
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public Error Error { get; }

        private Result(bool isSuccess, T? data, Error error)
        {
            if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error state.", nameof(error));
            }

            IsSuccess = isSuccess;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T? data)
            => new(true, data, Error.None);

        public static Result<T> Failure(Error error)
            => new(false, default, error);
    }
}
