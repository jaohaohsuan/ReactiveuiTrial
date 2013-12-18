using System.Collections.Generic;
using Autofac;

namespace RoutingSample
{
    public class ComponentsRegistedEvent
    {
        static void Register(ContainerBuilder builder, ComponentRegistrationValue registration)
        {
            if (registration.Key != null)
                builder.RegisterType(registration.ConcreteType).Named(registration.Key, registration.InterfaceType);
            else
                builder.RegisterType(registration.ConcreteType).As(registration.InterfaceType);
        }

        private readonly ContainerBuilder _builder;

        public ComponentsRegistedEvent(IEnumerable<ComponentRegistrationValue> values)
        {
            _builder = new ContainerBuilder();

            foreach (var o in values)
                Register(_builder, o);
        }

        public ComponentsRegistedEvent(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void Update(IContainer container)
        {
            _builder.Update(container);
        }
    }
}