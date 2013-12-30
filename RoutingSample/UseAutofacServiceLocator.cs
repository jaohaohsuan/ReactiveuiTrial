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
    public class UseAutofacServiceLocator
    {
        private readonly Subject<ComponentRegistrationValue> _registerTypesObserver = new Subject<ComponentRegistrationValue>();
        
        private ILifetimeScope _current;

        private readonly IObservable<IList<ComponentRegistrationValue>> _onRegisted;

        public UseAutofacServiceLocator()
        {
            _onRegisted = _registerTypesObserver.Buffer(TimeSpan.FromMilliseconds(5)).Where(buffer => buffer.Any());
        }

        public void Upate(ILifetimeScope value)
        {
            _current = value;
            RxApp.ConfigureServiceLocator(OnGetService, OnGetAllServices, OnRegister); 
        }

        public IObservable<IList<ComponentRegistrationValue>> OnRegisted { get { return _onRegisted; } }


        private void OnRegister(Type concreteType, Type interfaceType, string key)
        {
            _registerTypesObserver.OnNext(new ComponentRegistrationValue(concreteType, interfaceType, key));
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
    }
}