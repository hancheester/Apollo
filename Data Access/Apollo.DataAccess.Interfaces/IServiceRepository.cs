
namespace Apollo.DataAccess.Interfaces
{
    public interface IServiceRepository<T1, T2, T3>
        where T1 : struct
        where T2 : class, new() 
        where T3 : class, new()
    {
        int Create(T1 action, T2 request);
        T3 Return(T1 action, T2 request);
        void Update(T1 action, T2 request);
        void Delete(T1 action, T2 request);
        T3 Command(T1 action, T2 request);
    }
}
