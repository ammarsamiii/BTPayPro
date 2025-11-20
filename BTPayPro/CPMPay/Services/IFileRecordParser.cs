namespace BTPayPro.CPMPay.Services
{
    public interface IFileRecordParser<T>
    {
        T Parse(string line);
    }
}
