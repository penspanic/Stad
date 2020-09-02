// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// MessagePack.GeneratorCore.CodeAnalysis.TypeCollector 개조해서 씀.
#pragma warning disable SA1649 // File name should match first type name

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Stad.Analysis
{
    public class TypeCollectorResult
    {
        public ObjectSerializationInfo[] CollectedObjectInfo;
        public EnumSerializationInfo[] collectedEnumInfo;
        public ObjectSerializationInfo[] CollectedClosedTypeGenericInfo;
    }

    public class MessagePackGeneratorResolveFailedException : Exception
    {
        public MessagePackGeneratorResolveFailedException(string message)
            : base(message)
        {
        }
    }

    internal class ReferenceSymbols
    {
#pragma warning disable SA1401 // Fields should be private
        internal readonly INamedTypeSymbol MessagePackObjectAttribute;
        internal readonly INamedTypeSymbol UnionAttribute;
        internal readonly INamedTypeSymbol KeyAttribute;
        internal readonly INamedTypeSymbol IgnoreDataMemberAttribute;
#pragma warning restore SA1401 // Fields should be private

        public ReferenceSymbols(Compilation compilation, Action<string> logger)
        {
        }
    }

    public class TypeCollectorForCompilation
    {
        private static readonly SymbolDisplayFormat BinaryWriteFormat = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat ShortTypeNameFormat = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes);

        private readonly ReferenceSymbols typeReferences;
        private readonly INamedTypeSymbol[] targetTypes;

        private readonly Action<string> logger;

        private readonly bool disallowInternal;

        // visitor workspace:
        private HashSet<ITypeSymbol> alreadyCollected;
        private List<ObjectSerializationInfo> collectedObjectInfo;
        private List<EnumSerializationInfo> collectedEnumInfo;
        private List<ObjectSerializationInfo> collectedClosedTypeGenericInfo;

        public TypeCollectorForCompilation(Compilation compilation, bool disallowInternal, Action<string> logger)
        {
            this.logger = logger;
            this.typeReferences = new ReferenceSymbols(compilation, logger);
            this.disallowInternal = disallowInternal;

            targetTypes = RoslynExtensions.GetNamedTypeSymbols(compilation)
                .Where(x =>
                {
                    if (x.DeclaredAccessibility == Accessibility.Public)
                    {
                        return true;
                    }

                    if (!disallowInternal)
                    {
                        return x.DeclaredAccessibility == Accessibility.Friend;
                    }

                    return false;
                })
                .Where(x => true)
                .ToArray();
        }

        private void ResetWorkspace()
        {
            this.alreadyCollected = new HashSet<ITypeSymbol>();
            this.collectedObjectInfo = new List<ObjectSerializationInfo>();
            this.collectedEnumInfo = new List<EnumSerializationInfo>();
            this.collectedClosedTypeGenericInfo = new List<ObjectSerializationInfo>();
        }

        // EntryPoint
        public TypeCollectorResult Collect()
        {
            this.ResetWorkspace();

            foreach (INamedTypeSymbol item in this.targetTypes)
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
        private void CollectCore(ITypeSymbol typeSymbol)
        {
            if (!this.alreadyCollected.Add(typeSymbol))
            {
                return;
            }

            if (!this.IsAllowAccessibility(typeSymbol))
            {
                return;
            }

            if (!(typeSymbol is INamedTypeSymbol type))
            {
                return;
            }

            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                this.CollectEnum(type);
                return;
            }

            if (type.IsGenericType)
            {
                this.CollectGeneric(type);
                return;
            }

            if (type.TupleUnderlyingType != null)
            {
                CollectGeneric(type.TupleUnderlyingType);
                return;
            }

            if (type.Locations[0].IsInMetadata)
            {
                return;
            }

            this.CollectObject(type);
            return;
        }

        private void CollectEnum(INamedTypeSymbol type)
        {
            var info = new EnumSerializationInfo
            {
                Name = type.ToDisplayString(ShortTypeNameFormat).Replace(".", "_"),
                Namespace = type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString(),
                FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                UnderlyingType = type.EnumUnderlyingType.ToDisplayString(BinaryWriteFormat),
                Attributes = type.GetAttributes()
            };

            this.collectedEnumInfo.Add(info);
        }

        private void CollectGeneric(INamedTypeSymbol type)
        {
            INamedTypeSymbol genericType = type.ConstructUnboundGenericType();
            var genericTypeString = genericType.ToDisplayString();
            var fullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // special case
            if (fullName == "global::System.ArraySegment<byte>" || fullName == "global::System.ArraySegment<byte>?")
            {
                return;
            }


            // Collect substituted types for the type parameters (e.g. Bar in Foo<Bar>)
            foreach (var item in type.TypeArguments)
            {
                this.CollectCore(item);
            }

            // Collect only closed generic types (e.g. Foo<string>).
            var unboundGenericInfo = GetObjectInfo(type);
            collectedClosedTypeGenericInfo.Add(unboundGenericInfo);
        }

        private void CollectObject(INamedTypeSymbol type)
        {
            ObjectSerializationInfo info = GetObjectInfo(type);
            collectedObjectInfo.Add(info);
        }

        private ObjectSerializationInfo GetObjectInfo(INamedTypeSymbol type)
        {
            var isClass = !type.IsValueType;
            var members = new List<MemberSerializationInfo>();
            // Only KeyAttribute members
            var searchFirst = true;
            var hiddenIntKey = 0;

            foreach (IPropertySymbol item in RoslynExtensions.GetAllMembers(type).OfType<IPropertySymbol>())
            {
                if (item.IsIndexer)
                {
                    continue; // .tt files don't generate good code for this yet: https://github.com/neuecc/MessagePack-CSharp/issues/390
                }

                // TODO: Ignore Member 처리

                var member = new MemberSerializationInfo
                {
                    IsReadable = (item.GetMethod != null) && item.GetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    IsWritable = (item.SetMethod != null) && item.SetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    IsProperty = true,
                    IsField = false,
                    Name = item.Name,
                    Type = item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ShortTypeName = item.Type.ToDisplayString(BinaryWriteFormat),
                    Attributes = item.Type.GetAttributes()
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
                this.CollectCore(item.Type); // recursive collect
            }

            foreach (IFieldSymbol item in RoslynExtensions.GetAllMembers(type).OfType<IFieldSymbol>())
            {
                if (item.IsImplicitlyDeclared)
                {
                    continue;
                }

                // TODO: ignore 검사

                var member = new MemberSerializationInfo
                {
                    IsReadable = item.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    IsWritable = item.DeclaredAccessibility == Accessibility.Public && !item.IsReadOnly && !item.IsStatic,
                    IsProperty = true,
                    IsField = false,
                    Name = item.Name,
                    Type = item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ShortTypeName = item.Type.ToDisplayString(BinaryWriteFormat),
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
                this.CollectCore(item.Type); // recursive collect
            }

            var info = new ObjectSerializationInfo
            {
                IsClass = isClass,
                ConstructorParameters = null,
                Members = members.ToArray(),
                Name = GetMinimallyQualifiedClassName(type),
                FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Namespace = type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString(),
                Attributes = type.GetAttributes()
            };

            return info;
        }

        private static string GetMinimallyQualifiedClassName(INamedTypeSymbol type)
        {
            var name = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            name = name.Replace(".", "_");
            name = name.Replace("<", "_");
            name = name.Replace(">", "_");
            name = Regex.Replace(name, @"\[([,])*\]", match => $"Array{match.Length - 1}");
            return name;
        }

        private bool IsAllowAccessibility(ITypeSymbol symbol)
        {
            do
            {
                if (symbol.DeclaredAccessibility != Accessibility.Public)
                {
                    if (this.disallowInternal)
                    {
                        return false;
                    }

                    if (symbol.DeclaredAccessibility != Accessibility.Internal)
                    {
                        return true;
                    }
                }

                symbol = symbol.ContainingType;
            }
            while (symbol != null);

            return true;
        }
    }
}
