using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Persistence
{
    public abstract class DomainRepository<TDomainModel, TEntity> : IRepository<TDomainModel>
        where TDomainModel : Domain.AggregateRoot<TEntity>
    {
        public abstract Task<TDomainModel> Load(Guid id);

        public virtual async Task Save(TDomainModel entry)
        {
            var eventHandlerMethods = Helpers.ReflectionHelpers.GetAllHandleMethods(this.GetType(), "Apply");
            foreach (var evt in entry.Events)
            {
                if (eventHandlerMethods.ContainsKey(evt.GetType()))
                {
                    var method = eventHandlerMethods[evt.GetType()];
                    await Helpers.ReflectionHelpers.InvokeMethodAsync(method, this, new object[] { evt, entry });
                }
            }
        }
    }
}
