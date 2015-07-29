using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using SDDB.Domain.Abstract;

namespace SDDB.Domain.Infrastructure
{
    //IDbEntityExtensions------------------------------------------------------------------------------------------------------//
    public static class IDbEntityExtensions
    {
        //checks if prop name is in  ModifiedPropeties array 
        public static bool PropIsModified<T>(this T instance, string propName) where T: IDbEntity
        {
            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Contains(propName);
        }

        //checks if prop name is in  ModifiedPropeties array - overload accepting lambda
        public static bool PropIsModified<T, TOut>(this T instance, Expression<Func<T, TOut>> lambda) where T: IDbEntity
        {
            var body = (MemberExpression)lambda.Body;
            if (body == null) 
            {
                throw new ArgumentException(
                    string.Format("Expression '{0}' refers to a method, not a property.", lambda.ToString()));
            }
            return instance.PropIsModified(body.Member.Name);
        }

        //fill dbEntry with empty related entities to avoid having null objects at 1st nesting level
        public static void FillRelatedIfNull<T>(this T instance) where T: IDbEntity
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

        //fill dbEntry with empty related entities to avoid having null objects at 1st nesting level - overload for list<IDbEntity>
        public static void FillRelatedIfNull<T> (this List<T> instance) where T: IDbEntity
        {
            foreach (var record in instance)
            {
                FillRelatedIfNull(record);
            }
        }

        //copy modified properties from record to dbEntry (props listed in record's 'ModifiedProperties')
        public static void CopyModifiedProps<T> (this T instance, T record) where T: IDbEntity
        {
            foreach (var property in instance.getEntityProps())
            {
                if (record.PropIsModified(property.Name)) { copyPropertyHelper(instance, record, property); }
            }
        }

        //check is properties got modified and belong to logged properties
        public static bool LoggedPropsModified<T>(this T instance, string[] loggedProperties) where T : IDbEntity
        {
            return instance.ModifiedProperties == null ? false : instance.ModifiedProperties.Any(x => loggedProperties.Contains(x));
        }


        //Helpers--------------------------------------------------------------------------------------------------------------//
        #region Helpers

        //copy property from instance to record, exclude virtual, not mapped and excluded properties
        private static void copyPropertyHelper<T>(T instance, T record, PropertyInfo property) where T: IDbEntity
        {
            var excludedProperties = new string[] { "Id" };
            if (property.GetMethod.IsVirtual && typeof(IDbEntity).GetProperty(property.Name) == null) return;
            if (property.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() != null) return;
            if (excludedProperties.Contains(property.Name)) return;
            property.SetValue(instance, property.GetValue(record));
        }

        //get EF entity properties
        private static PropertyInfo[] getEntityProps<T>(this T instance)
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
        }



        #endregion

    }


}
