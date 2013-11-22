using System;
using System.Collections.Generic;
using Autofac;

namespace RoutingSample
{
    public class BatchComponentsRegistration : IObserver<IList<Tuple<Type, Type, string>>>
    {
        private readonly IContainer _container;

        public BatchComponentsRegistration(IContainer container)
        {
            _container = container;
        }

        public void OnNext(IList<Tuple<Type, Type, string>> value)
        {
            var builder = new ContainerBuilder();

            foreach (var tuple in value)
            {
                var key = tuple.Item3;
                var concreteType = tuple.Item1;
                var interfaceType = tuple.Item2;

                if (key != null)
                    builder.RegisterType(concreteType).Named(key, interfaceType);
                else
                    builder.RegisterType(concreteType).As(interfaceType);
            }

            builder.Update(_container);
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