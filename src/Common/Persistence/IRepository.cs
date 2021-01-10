using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Persistence
{
    public interface IRepository<T>
    {
        Task<T> Load(Guid id);
        Task Save(T entry);
    }
}
