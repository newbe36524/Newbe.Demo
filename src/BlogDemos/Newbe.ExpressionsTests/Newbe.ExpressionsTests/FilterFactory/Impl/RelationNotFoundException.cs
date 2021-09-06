using System;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class RelationNotFoundException : Exception
    {
        public string Relation { get; }

        public RelationNotFoundException(string relation)
            : this($"{relation} is not supported", relation)
        {
            Relation = relation;
        }

        public RelationNotFoundException(string? message, string relation) : base(message)
        {
            Relation = relation;
        }

        public RelationNotFoundException(string? message, Exception? innerException, string relation) : base(message,
            innerException)
        {
            Relation = relation;
        }
    }
}