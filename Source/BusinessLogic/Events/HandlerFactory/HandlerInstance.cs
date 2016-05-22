using System.Diagnostics;
using System.Reflection;

namespace BusinessLogic.Events.HandlerFactory
{
    public class HandlerInstance
    {
        private readonly object _instance;

        public HandlerInstance(HandlerMetaInfo handlerMetaInfo, object instance)
        {
            this.HandlerMetaInfo = handlerMetaInfo;
            this._instance = instance;
        }

        public HandlerMetaInfo HandlerMetaInfo { get; set; }

        [DebuggerHidden]
        public object Handle(object message)
        {
            try
            {
                var handleMethod = this.HandlerMetaInfo.HandleMethod;                
                var result = handleMethod.Invoke(this._instance, new[] { message });
                return result;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }

        }

        [DebuggerHidden]
        public object HandleWithParameters(object[] parameters)
        {
            try
            {

                var result = HandlerMetaInfo.HandleMethod.Invoke(_instance, parameters);
                return result;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }

        }
    }
}