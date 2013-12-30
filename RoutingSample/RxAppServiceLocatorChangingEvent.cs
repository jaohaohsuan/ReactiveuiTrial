using System.Collections.Generic;
using Autofac;

namespace RoutingSample
{
    public class RxAppServiceLocatorChangingEvent
    {
        static void Register(ContainerBuilder builder, ComponentRegistrationValue registration)
        {
            if (registration.Key != null)
                builder.RegisterType(registration.ConcreteType).Named(registration.Key, registration.InterfaceType);
            else
                builder.RegisterType(registration.ConcreteType).As(registration.InterfaceType);
        }

        private readonly ContainerBuilder _builder;

        public bool RaiseContainerEUpdateEvent { get; private set; }

        public RxAppServiceLocatorChangingEvent(IEnumerable<ComponentRegistrationValue> values)
        {
            _builder = new ContainerBuilder();

            foreach (var o in values)
                Register(_builder, o);
        }

        //public RxAppServiceLocatorChangingEvent(ContainerBuilder builder)
        //{
        //    _builder = builder;
        //    RaiseContainerEUpdateEvent = true;
        //}

        public void Update(IContainer container)
        {
            _builder.Update(container);
        }
    }
}