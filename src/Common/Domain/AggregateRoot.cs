using System;
using System.Collections.Generic;

namespace Common.Domain
{
    public abstract class AggregateRoot<TEntity>
    {
        public TEntity State { get; protected set; }
        public int Version { get; protected set; }

        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IReadOnlyList<IDomainEvent> Events { get => _events; }

        public AggregateRoot() { }
        public AggregateRoot(TEntity state)
        {
            State = state;
        }

        protected void Publish(IDomainEvent @event)
        {
            _events.Add(@event);

            // invoke apply method
            var applyMethods = Helpers.ReflectionHelpers.GetAllHandleMethods(this.GetType(), "Apply");
            if (applyMethods.ContainsKey(@event.GetType()))
            {
                var method = applyMethods[@event.GetType()];
                method.Invoke(this, new object[] { @event });
            }

            Version++;
        }
    }
}
