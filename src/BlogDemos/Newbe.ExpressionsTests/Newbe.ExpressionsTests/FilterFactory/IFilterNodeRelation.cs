namespace Newbe.ExpressionsTests.FilterFactory
{
    public interface IFilterNodeRelation<T> : IFilterNode<T>
    {
        IFilterNode<T> Left { get; }
        IFilterNode<T> Right { get; }
        public string Relation { get; set; }
    }
}