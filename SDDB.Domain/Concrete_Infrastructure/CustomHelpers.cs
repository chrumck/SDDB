using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using SDDB.Domain.Abstract;

namespace SDDB.Domain.Infrastructure
{
    public static class CustomHelpers
    {

        //checks if prop name is in  ModifiedPropeties array
        public static bool PropIsModified<TIn, TOut>(this TIn instance, Expression<Func<TIn, TOut>> lambda) where TIn : IDbEntity
        {
            var body = (MemberExpression)lambda.Body;

            if (body == null) throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", lambda.ToString()));

            string propName = body.Member.Name;

            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Contains(propName);
        }

        //checks if prop name is in  ModifiedPropeties array - overload
        public static bool PropIsModified<TIn>(this TIn instance, string propName) where TIn : IDbEntity
        {
            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Contains(propName);
        }

    }
}
