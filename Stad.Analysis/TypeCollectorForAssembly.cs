// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// MessagePack.GeneratorCore.CodeAnalysis.TypeCollector 개조해서 씀.
#pragma warning disable SA1649 // File name should match first type name

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Stad.Analysis
{
    internal class ReferenceSymbols2
    {
#pragma warning disable SA1401 // Fields should be private
        internal readonly INamedTypeSymbol MessagePackObjectAttribute;
        internal readonly INamedTypeSymbol UnionAttribute;
        internal readonly INamedTypeSymbol KeyAttribute;
        internal readonly INamedTypeSymbol IgnoreDataMemberAttribute;
#pragma warning restore SA1401 // Fields should be private

        public ReferenceSymbols2(CollectContext context, Action<string> logger)
        {
        }
    }

    internal class CollectContext
    {
        public CollectContext(Assembly[] assemblies)
        {
            AllTypes = assemblies.SelectMany(a => a.GetTypes()).ToArray();
        }

        public readonly IReadOnlyList<Type> AllTypes;
    }

    public class TypeCollectorForAssembly
    {
        private CollectContext _context;
        private readonly ReferenceSymbols2 typeReferences;
        private readonly Type[] targetTypes;

        private readonly Action<string> logger;

        private readonly bool disallowInternal;

        // visitor workspace:
        private HashSet<Type> alreadyCollected;
        private List<ObjectSerializationInfo> collectedObjectInfo;
        private List<EnumSerializationInfo> collectedEnumInfo;
        private List<ObjectSerializationInfo> collectedClosedTypeGenericInfo;

        public TypeCollectorForAssembly(Assembly[] assemblies, Action<string> logger)
        {
            _context = new CollectContext(assemblies);
            this.logger = logger;
            
            this.typeReferences = new ReferenceSymbols2(_context, logger);
            this.disallowInternal = disallowInternal;

            targetTypes = _context.AllTypes
                .ToArray();
        }

        private void ResetWorkspace()
        {
            this.alreadyCollected = new HashSet<Type>();
            this.collectedObjectInfo = new List<ObjectSerializationInfo>();
            this.collectedEnumInfo = new List<EnumSerializationInfo>();
            this.collectedClosedTypeGenericInfo = new List<ObjectSerializationInfo>();
        }

        // EntryPoint
        public TypeCollectorResult Collect()
        {
            this.ResetWorkspace();

            foreach (Type item in this.targetTypes)
            {
                this.CollectCore(item);
            }

            return new TypeCollectorResult()
            {
                CollectedObjectInfo = this.collectedObjectInfo.OrderBy(x => x.FullName).ToArray(),
                collectedEnumInfo = this.collectedEnumInfo.OrderBy(x => x.FullName).ToArray(),
                CollectedClosedTypeGenericInfo =
                    this.collectedClosedTypeGenericInfo.OrderBy(x => x.FullName).ToArray()
            };
        }

        // Gate of recursive collect
        private void CollectCore(Type type)
        {
            if (!this.alreadyCollected.Add(type))
            {
                return;
            }

            if (type.IsEnum)
            {
                this.CollectEnum(type);
                return;
            }

            if (type.IsGenericType)
            {
                this.CollectGeneric(type);
                return;
            }

            this.CollectObject(type);
            return;
        }

        private void CollectEnum(Type type)
        {
            var info = new EnumSerializationInfo
            {
                Name = type.Name,
                Namespace = type.Namespace,
                FullName = type.FullName,
                UnderlyingType = type.GetEnumUnderlyingType().FullName,
                Attributes = NeutralAttributeData.FromCustomAttributeArray(type.GetCustomAttributes())
            };

            this.collectedEnumInfo.Add(info);
        }

        private void CollectGeneric(Type type)
        {
            var fullName = type.FullName;

            // special case
            if (fullName == "global::System.ArraySegment<byte>" || fullName == "global::System.ArraySegment<byte>?")
            {
                return;
            }


            // Collect substituted types for the type parameters (e.g. Bar in Foo<Bar>)
            foreach (var item in type.GenericTypeArguments)
            {
                this.CollectCore(item);
            }

            // Collect only closed generic types (e.g. Foo<string>).
            var unboundGenericInfo = GetObjectInfo(type);
            collectedClosedTypeGenericInfo.Add(unboundGenericInfo);
        }

        private void CollectObject(Type type)
        {
            ObjectSerializationInfo info = GetObjectInfo(type);
            collectedObjectInfo.Add(info);
        }

        private ObjectSerializationInfo GetObjectInfo(Type type)
        {
            var isClass = !type.IsValueType;
            var members = new List<MemberSerializationInfo>();
            // Only KeyAttribute members
            var searchFirst = true;
            var hiddenIntKey = 0;

            foreach (PropertyInfo item in type.GetProperties())
            {
                var member = new MemberSerializationInfo
                {
                    IsReadable = (item.GetMethod != null) && item.GetMethod.IsPublic,
                    IsWritable = (item.SetMethod != null) && item.SetMethod.IsPublic,
                    IsProperty = true,
                    IsField = false,
                    Name = item.Name,
                    Type = item.PropertyType.FullName,
                    ShortTypeName = item.PropertyType.Name,
                    Attributes = NeutralAttributeData.FromCustomAttributeArray(item.GetCustomAttributes())
                };
                if (!member.IsReadable && !member.IsWritable)
                {
                    continue;
                }

                if (searchFirst)
                {
                    searchFirst = false;
                }

                members.Add(member);
                this.CollectCore(item.PropertyType); // recursive collect
            }

            foreach (FieldInfo item in type.GetFields())
            {
                var member = new MemberSerializationInfo
                {
                    IsReadable = item.IsPublic && !item.IsStatic,
                    IsWritable = item.IsPublic && !item.IsInitOnly && !item.IsStatic,
                    IsProperty = true,
                    IsField = false,
                    Name = item.Name,
                    Type = item.FieldType.FullName,
                    ShortTypeName = item.FieldType.Name,
                };
                if (!member.IsReadable && !member.IsWritable)
                {
                    continue;
                }

                if (searchFirst)
                {
                    searchFirst = false;
                }

                members.Add(member);
                this.CollectCore(item.FieldType); // recursive collect
            }

            var info = new ObjectSerializationInfo
            {
                IsClass = isClass,
                ConstructorParameters = null,
                Members = members.ToArray(),
                Name = GetMinimallyQualifiedClassName(type),
                FullName = type.FullName,
                Namespace = type.Namespace,
                Attributes = NeutralAttributeData.FromCustomAttributeArray(type.GetCustomAttributes())
            };

            return info;
        }

        private static string GetMinimallyQualifiedClassName(Type type)
        {
            var name = type.FullName;
            if (name == null)
                return string.Empty;

            name = name.Replace(".", "_");
            name = name.Replace("<", "_");
            name = name.Replace(">", "_");
            name = Regex.Replace(name, @"\[([,])*\]", match => $"Array{match.Length - 1}");
            return name;
        }
    }
}
