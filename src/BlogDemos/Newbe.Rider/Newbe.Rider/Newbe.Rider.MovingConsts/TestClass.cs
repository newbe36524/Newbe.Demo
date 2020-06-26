namespace Newbe.Rider.MovingConsts
{
    class A
    {
    }

    class B
    {
        public const string HEHE = "www.newbe.pro";
    }

    class TestClass
    {
        public void Test()
        {
            var wwwNewbePro = B.HEHE;
        }
    }
}