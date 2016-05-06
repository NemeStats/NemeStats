using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLogic.Events.HandlerFactory
{
    public class HandlerMetaInfo
    {
        private readonly Dictionary<Type, List<object>> _customAttributes = new Dictionary<Type, List<object>>();

        public HandlerMetaInfo(Type handlerType, MethodInfo handleMethod)
        {
            this.Type = handlerType;
            this.HandleMethod = handleMethod;

            ScanAttributes(handlerType.GetCustomAttributes(true));
            ScanAttributes(this.HandleMethod.GetCustomAttributes(true));

        }

        private void ScanAttributes(IEnumerable<object> attributes)
        {
            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();
                if (!this._customAttributes.ContainsKey(attributeType))
                {
                    this._customAttributes.Add(attributeType, new List<object> { attribute });
                }
                else
                {
                    this._customAttributes[attributeType].Add(attributeType);
                }
            }
        }

        public Type Type { get; set; }

        public MethodInfo HandleMethod { get; set; }

        public int? ExpireSeconds { get; set; }

        public TA GetCustomAttribute<TA>() where TA : Attribute
        {
            List<object> attributes;
            if (this._customAttributes.TryGetValue(typeof(TA), out attributes))
            {
                return attributes.FirstOrDefault() as TA;
            }
            return null;
        }

        public bool HasCustomAttribute(Type attributeType)
        {
            return this._customAttributes.ContainsKey(attributeType);
        }

        public bool HasCustomAttribute<T>()
        {
            return HasCustomAttribute(typeof(T));
        }
    }


}