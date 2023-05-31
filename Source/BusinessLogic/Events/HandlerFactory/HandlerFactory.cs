using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLogic.Events.HandlerFactory
{
    public abstract class HandlerFactory
    {
        private readonly Type _handlerInterfaceGenericType;

        private readonly Type _messageInterfaceType;

        private readonly IList<Assembly> _messageAssemblies = new List<Assembly>();

        protected HandlerFactory(HandlerFactoryConfiguration factoryConfiguration, Type handlerInterfaceGenericType, Type messageInterfaceType)
        {
            this._handlerInterfaceGenericType = handlerInterfaceGenericType;
            this._messageInterfaceType = messageInterfaceType;


            foreach (var assembly in factoryConfiguration.MessageAssembliesToScan)
            {
                RegisterMessageAssembly(assembly);
            }

            foreach (var type in factoryConfiguration.OrderedHandlers)
            {
                RegisterHandler(type);
            }

            foreach (var assembly in factoryConfiguration.HandlerAssembliesToScan)
            {
                RegisterAssemblyHandlers(assembly);
            }
        }

        public Dictionary<Type, IList<HandlerMetaInfo>> MessageHandlers { get; } = new Dictionary<Type, IList<HandlerMetaInfo>>();

        private void RegisterAssemblyHandlers(Assembly assembly)
        {
            var types = new List<Type>();

            types.AddRange(assembly.GetTypes().Where(IsMessageHandler));

            types.ForEach(RegisterHandler);
        }

        private void RegisterMessageAssembly(Assembly assembly)
        {
            this._messageAssemblies.Add(assembly);
        }

        /// <summary>
        ///   Returns true if the given type is a message handler.
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        private bool IsMessageHandler(Type t)
        {
            if (t.IsValueType)
            {
                return false;
            }

            if (t.IsAbstract)
            {
                return false;
            }

            return t.GetInterfaces().Select(GetMessageTypeFromMessageHandler).Any(messageType => messageType != null);
        }

        /// <summary>
        ///   Returns the message type handled by the given message handler type.
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        private Type GetMessageTypeFromMessageHandler(Type t)
        {
            if (t.IsGenericType)
            {
                Type[] args =
                    t.GetGenericArguments()
                        .Where(tp => this._messageInterfaceType.IsAssignableFrom(tp)).ToArray();

                if (args.Length != 1)
                {
                    return null;
                }

                Type handlerType = this._handlerInterfaceGenericType.MakeGenericType(args[0]);
                if (handlerType.IsAssignableFrom(t))
                {
                    return args[0];
                }
            }

            return null;
        }

        private void RegisterHandler(Type handlerType)
        {
            var messageTypes = GetMessageTypesIfIsMessageHandler(handlerType).ToList();

            var interfaces = messageTypes.Where(t => t.IsInterface || t.IsAbstract).ToList();
            foreach (var i in interfaces)
            {
                if (!this._messageAssemblies.Contains(i.Assembly))
                {
                    this._messageAssemblies.Add(i.Assembly);
                }

                foreach (var assembly in this._messageAssemblies)
                {
                    messageTypes.AddRange(assembly.GetTypes().Where(p => i.IsAssignableFrom(p)));
                }
            }

            foreach (var messageType in messageTypes.Where(t => !t.IsInterface))
            {
                if (!this.MessageHandlers.ContainsKey(messageType))
                {
                    this.MessageHandlers[messageType] = new List<HandlerMetaInfo>();
                }

                if (this.MessageHandlers[messageType].All(h => h.Type != handlerType))
                {
                    this.MessageHandlers[messageType].Add(new HandlerMetaInfo(handlerType, GetMessageHandlerHandleMethod(handlerType, messageType)));
                }
            }
        }

        /// <summary>
        ///   If the type is a message handler, returns all the message types that it handles
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        private IEnumerable<Type> GetMessageTypesIfIsMessageHandler(Type type)
        {
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var t in type.GetInterfaces().Where(t => t.IsGenericType))
            {
                var potentialMessageType = t.GetGenericArguments().FirstOrDefault();

                if (potentialMessageType == null)
                {
                    continue;
                }

                if (
                    this._handlerInterfaceGenericType.MakeGenericType(potentialMessageType).IsAssignableFrom(t))
                {
                    yield return potentialMessageType;
                }
            }
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        public int HandlersWithAttributeCount<TA>(Type messageType) where TA : Attribute
        {
            IList<HandlerMetaInfo> handlerTypes;

            if (this.MessageHandlers.TryGetValue(messageType, out handlerTypes))
            {
                return handlerTypes.Count(c => c.HasCustomAttribute<TA>());
            }

            return 0;
        }

        public int HandlersCount(Type messageType)
        {
            IList<HandlerMetaInfo> handlerTypes;

            if (this.MessageHandlers.TryGetValue(messageType, out handlerTypes))
            {
                return handlerTypes.Count;
            }

            return 0;
        }

        public IList<HandlerInstance> GetHandlers(Type messageType, Type withAttribute = null)
        {
            var filteredHandlers = GetHandlerMetaInfos(messageType, withAttribute);

            var result = new List<HandlerInstance>();

            try
            {
                foreach (var filteredHandler in filteredHandlers)
                {
                    result.Add(new HandlerInstance(filteredHandler, EventHandlerObjectFactory.Container.GetInstance(filteredHandler.Type)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception encountered with filteredHandlers", ex);
            }

            return result;

        }

        public List<HandlerMetaInfo> GetHandlerMetaInfos(Type messageType, Type withAttribute)
        {
            IList<HandlerMetaInfo> handlerTypes;

            var filteredHandlers = new List<HandlerMetaInfo>();
            if (this.MessageHandlers.TryGetValue(messageType, out handlerTypes))
            {
                filteredHandlers = handlerTypes.Where(h => withAttribute == null || h.HasCustomAttribute(withAttribute)).ToList();
            }
            return filteredHandlers;
        }

        private MethodInfo GetMessageHandlerHandleMethod(Type targetType, Type messageType)
        {
            MethodInfo method = targetType.GetMethod("Handle", new[] { messageType });
            if (method != null)
            {
                return method;
            }

            var handlerType = this._handlerInterfaceGenericType.MakeGenericType(messageType);
            return targetType.GetInterfaceMap(handlerType)
                .TargetMethods
                .FirstOrDefault();
        }
    }
}
