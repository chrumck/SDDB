using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

using SDDB.Domain.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDDB.Domain.Infrastructure
{
    //DbHelpers----------------------------------------------------------------------------------------------------------------//
    public static class DbHelpers
    {
        //attempt to save changes to DBContext, retry if exception on deadlock thrown
        public static async Task<string> SaveChangesAsync(DbContext dbContext)
        {
            var errorMessage = "";
            for (int i = 1; i <= 10; i++)
            {
                try
                {
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    break;
                }
                catch (Exception e)
                {
                    var message = e.GetBaseException().Message;
                    if (i == 10 || !message.Contains("Deadlock found when trying to get lock"))
                    {
                        errorMessage += String.Format("Error saving records: {0}\n", message);
                        break;
                    }
                }
                await Task.Delay(200).ConfigureAwait(false);
            }
            return errorMessage;
        }
    }

    //IDbEntityExtensions------------------------------------------------------------------------------------------------------//
    public static class IDbEntityExtensions
    {
        //checks if prop name is in  ModifiedPropeties array
        public static bool PropIsModified<TIn, TOut>(this TIn instance, Expression<Func<TIn, TOut>> lambda) where TIn : IDbEntity
        {
            var body = (MemberExpression)lambda.Body;
            if (body == null) {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", lambda.ToString()));
            }
            string propName = body.Member.Name;

            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Contains(propName);
        }

        //checks if prop name is in  ModifiedPropeties array - overload to get string propName
        public static bool PropIsModified<TIn>(this TIn instance, string propName) where TIn : IDbEntity
        {
            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Contains(propName);
        }

        //fill dbEntry with empty related entities to avoid having null objects at 1st nesting level
        public static void FillRelatedIfNull<T>(this T instance) where T : IDbEntity
        {
            var excludedProperties = new string[] { "Id", "TSP" };
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            foreach (var property in properties)
            {
                if (!property.GetMethod.IsVirtual) continue;
                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                if (excludedProperties.Contains(property.Name)) continue;

                if (property.GetValue(instance) == null) property.SetValue(instance, Activator.CreateInstance(property.PropertyType));
            }
        }

        //copy modified properties from record to dbEntry (props listed in record's 'ModifiedProperties')
        public static void CopyModifiedProps<T>(this T instance, T record) where T : IDbEntity
        {
            var excludedProperties = new string[] { "Id", "TSP" };
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            foreach (var property in properties)
            {
                if (property.GetMethod.IsVirtual) continue;
                if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) continue;
                if (excludedProperties.Contains(property.Name)) continue;

                if (record.PropIsModified(property.Name)) property.SetValue(instance, property.GetValue(record));
            }
        }

        //check is properites got modified and belong to logged properties
        public static bool LoggedPropsModified<T>(this T instance, string[] loggedProperties) where T : IDbEntity
        {
            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Any(x => loggedProperties.Contains(x));
        }

    }


}
