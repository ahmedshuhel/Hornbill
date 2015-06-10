namespace Hornbill.Common
{
    public interface IQuery<out T>
    {
        T Execute();
    }
}