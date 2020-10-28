using System;

namespace Newbe.ExpressionsTests
{
    public class MissingObjectException : Exception
    {
        public Type ResolvingType { get; }

        public MissingObjectException(Type resolvingType)
            : this($"unable to resolve {resolvingType}", resolvingType)
        {
            ResolvingType = resolvingType;
        }

        public MissingObjectException(string? message, Type resolvingType) : base(message)
        {
            ResolvingType = resolvingType;
        }

        public MissingObjectException(string? message, Exception? innerException, Type resolvingType) : base(message,
            innerException)
        {
            ResolvingType = resolvingType;
        }
    }
}