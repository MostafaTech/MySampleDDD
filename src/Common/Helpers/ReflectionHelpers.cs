using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Helpers
{
    public static class ReflectionHelpers
    {

        public static Type[] GetMessageTypesHandledByMessageHandler(Type handlerType)
        {
            return handlerType.GetMember("Handle")
                .Where(m => m.MemberType == MemberTypes.Method).Cast<MethodInfo>()
                .Select(m => m.GetParameters()[0].ParameterType).ToArray();
        }

        public static MethodInfo GetMessageHandlerHandleMethod(Type handlerType, Type messageType)
        {
            return handlerType.GetMember("Handle")
                .Where(m => m.MemberType == MemberTypes.Method).Cast<MethodInfo>()
                .FirstOrDefault(m => m.GetParameters()[0].ParameterType == messageType);
        }

        public static Dictionary<Type, MethodInfo> GetAllHandleMethods(Type ofType, string methodName)
        {
            return ofType.GetMember(methodName)
                .Where(m => m.MemberType == MemberTypes.Method).Cast<MethodInfo>()
                .ToDictionary(k => k.GetParameters().First().ParameterType, v => v);
        }

        public static Task InvokeMethodAsync(MethodInfo method, object instance, params object[] parameters)
        {
            return (Task)method.Invoke(instance, parameters);
        }

    }
}
