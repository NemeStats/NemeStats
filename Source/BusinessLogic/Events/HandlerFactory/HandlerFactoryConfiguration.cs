using System;
using System.Collections.Generic;
using System.Reflection;

namespace BusinessLogic.Events.HandlerFactory
{
    public class HandlerFactoryConfiguration
    {
        private readonly List<Type> _orderedHandlers = new List<Type>();

        private readonly List<Assembly> _handlerAssembliesToScan = new List<Assembly>();

        private readonly List<Assembly> _messageAssembliesToScan = new List<Assembly>();

        public List<Type> OrderedHandlers => this._orderedHandlers;

        public List<Assembly> HandlerAssembliesToScan => this._handlerAssembliesToScan;

        public List<Assembly> MessageAssembliesToScan => this._messageAssembliesToScan;

        public HandlerFactoryConfiguration AddHandler(Type handlerType, params Type[] requiredHandlers)
        {
            foreach (var requiredHandler in requiredHandlers)
            {
                SafeAddHandler(requiredHandler);
            }

            SafeAddHandler(handlerType);

            return this;
        }

        public HandlerFactoryConfiguration AddHandler<T>(params Type[] requiredHandlers)
        {
            AddHandler(typeof(T), requiredHandlers);
            return this;
        }

        public HandlerFactoryConfiguration AddHandlerAssembly(Assembly assembly)
        {
            this._handlerAssembliesToScan.Add(assembly);

            return this;
        }

        public HandlerFactoryConfiguration AddMessageAssembly(Assembly assembly)
        {
            this._messageAssembliesToScan.Add(assembly);

            return this;
        }

        public HandlerFactoryConfiguration AddMessageAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                this._messageAssembliesToScan.Add(assembly);
            }


            return this;
        }

        private void SafeAddHandler(Type handler)
        {
            if (!this.OrderedHandlers.Contains(handler))
            {
                this._orderedHandlers.Add(handler);
            }
        }
    }
}