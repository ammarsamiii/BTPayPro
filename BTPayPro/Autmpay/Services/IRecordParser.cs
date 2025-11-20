namespace BTPayPro.Autmpay.Services
{
    public interface IRecordParser<T>
    {
        T Parse(string line);
    }
}
