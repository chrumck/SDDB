﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SDDB.UnitTests
{
    public class MockDependencyResolver : IDependencyResolver
    {
        private readonly Dictionary<Type, object> collection;

        public MockDependencyResolver()
        {
            collection = new Dictionary<Type, object>();
        }

        public void Freeze<T>(object mockedObject)
        {
            collection.Add(typeof(T), mockedObject);
        }

        public object GetService(Type serviceType)
        {
            if (collection.ContainsKey(serviceType))
            {
                return collection[serviceType];
            }
            var message = String.Format("No object is registered for type {0}", serviceType.Name);
            throw new ArgumentException(message);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}