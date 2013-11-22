using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Autofac;
using Autofac.Core.Lifetime;
using ReactiveUI;

namespace RoutingSample
{
    public class LifetimeScopeContainer : IObserver<EventPattern<LifetimeScopeBeginningEventArgs>>
    {
        private readonly Subject<Tuple<Type, Type, string>> _registerTypesObserver;
        
        private ILifetimeScope _current;

        public LifetimeScopeContainer(Subject<Tuple<Type, Type, string>> registerTypesObserver)
        {

            _registerTypesObserver = registerTypesObserver;

            //_registerTypesObserver = new Subject<Tuple<Type, Type, string>>();

          

            //_registerTypesObserver.Buffer(TimeSpan.FromMilliseconds(5)).Where(b => b.Any())
            //                      .Subscribe(new BatchComponentsRegistration(container));

            RxApp.ConfigureServiceLocator(OnGetService, OnGetAllServices, OnRegister);
        }

        private void OnRegister(Type concreteType, Type interfaceType, string key)
        {
            _registerTypesObserver.OnNext(Tuple.Create(concreteType, interfaceType, key));
        }

        private object OnGetService(Type interfaceType, string key)
        {
            if (key != null) return _current.ResolveNamed(key, interfaceType);
            return _current.Resolve(interfaceType);
        }

        private IEnumerable<object> OnGetAllServices(Type interfaceType, string key)
        {
            var constructed = typeof(IEnumerable<>).MakeGenericType(new[] { interfaceType });

            if (key != null)
                return _current.ResolveNamed(key, constructed) as IEnumerable<object>;
            return _current.Resolve(constructed) as IEnumerable<object>;
        }

        public void OnNext(EventPattern<LifetimeScopeBeginningEventArgs> value)
        {
            _current = value.EventArgs.LifetimeScope;
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