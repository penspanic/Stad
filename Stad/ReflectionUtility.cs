using System;
using System.Reflection;
using Stad.DataSet;

namespace Stad
{
    public class ReflectionUtility
    {
        public static (MemberInfo, Type) FindMember(object target, DataSetItem item)
        {
            Type targetType = target.GetType();
            FieldInfo fieldInfo = targetType.GetField(item.Name);
            if (fieldInfo != null)
            {
                return (fieldInfo, fieldInfo.FieldType);
            }

            PropertyInfo propertyInfo = targetType.GetProperty(item.Name);
            if (propertyInfo != null)
            {
                return (propertyInfo, propertyInfo.PropertyType);
            }

            return (null, null);
        }

        public static bool SetValue(object target, DataSetItem item, object value)
        {
            Type targetType = target.GetType();
            FieldInfo fieldInfo = targetType.GetField(item.Name);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(target, value);
                return true;
            }

            PropertyInfo propertyInfo = targetType.GetProperty(item.Name);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, value);
                return true;
            }

            return false;
        }
    }
}