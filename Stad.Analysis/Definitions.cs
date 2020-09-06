// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Stad.Analysis
{
    public class ObjectSerializationInfo
    {
        public override string ToString()
        {
            return FullName;
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Namespace { get; set; }

        public bool IsClass { get; set; }

        public bool IsStruct
        {
            get { return !this.IsClass; }
        }

        public MemberSerializationInfo[] ConstructorParameters { get; set; }

        public MemberSerializationInfo[] Members { get; set; }

        public MemberSerializationInfo GetMember(int index)
        {
            return this.Members[index];
        }

        public ImmutableArray<NeutralAttributeData> Attributes { get; set; }
    }

    public class MemberSerializationInfo
    {
        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public bool IsProperty { get; set; }

        public bool IsField { get; set; }

        public bool IsWritable { get; set; }

        public bool IsReadable { get; set; }
        
        public string Type { get; set; }

        public string Name { get; set; }

        public string ShortTypeName { get; set; }

        public ImmutableArray<NeutralAttributeData> Attributes { get; set; }
    }

    public class EnumSerializationInfo
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string UnderlyingType { get; set; }

        public ImmutableArray<NeutralAttributeData> Attributes { get; set; }
    }

    public class NeutralAttributeData
    {
        public string Type { get; }
        public ImmutableArray<object> Arguments { get; }

        public NeutralAttributeData(string type, ImmutableArray<object> arguments)
        {
            Type = type;
            Arguments = arguments;
        }

        public static ImmutableArray<NeutralAttributeData> FromAttributeDataArray(
            ImmutableArray<Microsoft.CodeAnalysis.AttributeData> array)
        {
            if (array.IsDefaultOrEmpty)
                return ImmutableArray<NeutralAttributeData>.Empty;

            return ImmutableArray.CreateRange(array.Select(FromAttributeData));
        }

        public static ImmutableArray<NeutralAttributeData> FromCustomAttributeArray(
            IEnumerable<object> array)
        {
            if (array.Any() == false)
                return ImmutableArray<NeutralAttributeData>.Empty;

            return ImmutableArray.CreateRange(array.Select(FromCustomAttribute));
        }

        public static NeutralAttributeData FromAttributeData(Microsoft.CodeAnalysis.AttributeData attributeData)
        {
            ImmutableArray<object> arguments = attributeData.ConstructorArguments.IsDefaultOrEmpty
                ? ImmutableArray<object>.Empty
                : ImmutableArray.CreateRange(attributeData.ConstructorArguments.Select(a => a.Value));

            string type = attributeData.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "";
            return new NeutralAttributeData(type, arguments);
        }

        public static NeutralAttributeData FromCustomAttribute(object attribute)
        {
            // TODO: 이쁜 방법이 없을까.. 너무 암묵적인 룰인데
            Type type = attribute.GetType();
            var members = type.GetMembers();
            List<object> arguments = new List<object>();
            foreach (MemberInfo member in members)
            {
                if (member is FieldInfo fieldInfo)
                {
                    arguments.Add(fieldInfo.GetValue(attribute));
                }

                if (member is PropertyInfo propertyInfo)
                {
                    arguments.Add(propertyInfo.GetValue(attribute));
                }
            }

            return new NeutralAttributeData(type.FullName, ImmutableArray.CreateRange(arguments));
        }
    }
}
