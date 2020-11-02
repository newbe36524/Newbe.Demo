namespace Newbe.ExpressionsTests
{
    public interface IUserDal
    {
        bool Exists(string phone, string password);
    }
}