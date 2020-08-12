using System;
using System.Reflection;
using Stad.DataSet;

namespace Stad
{
    public class ReflectionUtility
    {
        public static MemberInfo FindMember(object target, DataSetItem item)
        {
            Type targetType = target.GetType();
            FieldInfo fieldInfo = targetType.GetField(item.Name);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }

            PropertyInfo propertyInfo = targetType.GetProperty(item.Name);
            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            return null;
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