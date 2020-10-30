namespace Newbe.ExpressionsTests
{
    public static class ObjectFactoryExtensions
    {
        public static T Resolve<T>(this IObjectFactory factory)
        {
            var obj = factory.Resolve(typeof(T));
            return (T) obj;
        }
    }
}