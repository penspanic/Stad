// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Stad.Analysis
{
    public class ObjectSerializationInfo
    {
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

        public bool HasIMessagePackSerializationCallbackReceiver { get; set; }

        public bool NeedsCastOnBefore { get; set; }

        public bool NeedsCastOnAfter { get; set; }

        public MemberSerializationInfo GetMember(int index)
        {
            return this.Members[index];
        }

        public ImmutableArray<AttributeData> Attributes { get; set; }
    }

    public class MemberSerializationInfo
    {
        public bool IsProperty { get; set; }

        public bool IsField { get; set; }

        public bool IsWritable { get; set; }

        public bool IsReadable { get; set; }
        
        public string Type { get; set; }

        public string Name { get; set; }

        public string ShortTypeName { get; set; }

        public string CustomFormatterTypeName { get; set; }

        public ImmutableArray<AttributeData> Attributes { get; set; }
    }

    public class EnumSerializationInfo
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string UnderlyingType { get; set; }

        public string FormatterName => (this.Namespace == null ? this.Name : this.Namespace + "." + this.Name) + "Formatter";

        public ImmutableArray<AttributeData> Attributes { get; set; }
    }
}
