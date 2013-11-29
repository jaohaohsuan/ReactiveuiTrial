using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using Autofac;
using Reactive.EventAggregator;

namespace RoutingSample
{

    public class BatchComponentsRegistration : IObserver<IList<ComponentsRegistration>>
    {
        private readonly EventAggregator _eventAggregator;

        public BatchComponentsRegistration(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnNext(IList<ComponentsRegistration> value)
        {
            var builder = new ContainerBuilder();

            foreach (var o in value)
                Register(builder, o);

            _eventAggregator.Publish(builder);
        }

        private void Register(ContainerBuilder builder, ComponentsRegistration registration)
        {
            if (registration.Key != null)
                builder.RegisterType(registration.ConcreteType).Named(registration.Key, registration.InterfaceType);
            else
                builder.RegisterType(registration.ConcreteType).As(registration.InterfaceType);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}