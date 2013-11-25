﻿using System;

namespace RoutingSample
{
    public class ComponentsRegistration
    {
        private readonly Type _concreteType;
        private readonly Type _interfaceType;
        private readonly string _key;

        public ComponentsRegistration(Type concreteType, Type interfaceType, string key)
        {
            _concreteType = concreteType;
            _interfaceType = interfaceType;
            _key = key;
        }

        public Type ConcreteType { get { return _concreteType; } }

        public Type InterfaceType { get { return _interfaceType; } }

        public string Key { get { return _key; } }
    }
}