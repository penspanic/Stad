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
        internal readonly INamedTypeSymbol Task;
        internal readonly INamedTypeSymbol TaskOfT;
        internal readonly INamedTypeSymbol MessagePackObjectAttribute;
        internal readonly INamedTypeSymbol UnionAttribute;
        internal readonly INamedTypeSymbol SerializationConstructorAttribute;
        internal readonly INamedTypeSymbol KeyAttribute;
        internal readonly INamedTypeSymbol IgnoreDataMemberAttribute;
        internal readonly INamedTypeSymbol IMessagePackSerializationCallbackReceiver;
        internal readonly INamedTypeSymbol MessagePackFormatterAttribute;
#pragma warning restore SA1401 // Fields should be private

        public ReferenceSymbols(Compilation compilation, Action<string> logger)
        {
            TaskOfT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            if (TaskOfT == null)
            {
                logger("failed to get metadata of System.Threading.Tasks.Task`1");
            }

            Task = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
            if (Task == null)
            {
                logger("failed to get metadata of System.Threading.Tasks.Task");
            }
        }
    }

    public class TypeCollector
    {
        private const string CodegeneratorOnlyPreprocessorSymbol = "INCLUDE_ONLY_CODE_GENERATION";

        private static readonly SymbolDisplayFormat BinaryWriteFormat = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat ShortTypeNameFormat = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes);

        private readonly bool isForceUseMap;
        private readonly ReferenceSymbols typeReferences;
        private readonly INamedTypeSymbol[] targetTypes;
        private readonly HashSet<string> embeddedTypes = new HashSet<string>(new string[]
        {
            "short",
            "int",
            "long",
            "ushort",
            "uint",
            "ulong",
            "float",
            "double",
            "bool",
            "byte",
            "sbyte",
            "decimal",
            "char",
            "string",
            "object",
            "System.Guid",
            "System.TimeSpan",
            "System.DateTime",
            "System.DateTimeOffset",

            "MessagePack.Nil",

            // and arrays
            "short[]",
            "int[]",
            "long[]",
            "ushort[]",
            "uint[]",
            "ulong[]",
            "float[]",
            "double[]",
            "bool[]",
            "byte[]",
            "sbyte[]",
            "decimal[]",
            "char[]",
            "string[]",
            "System.DateTime[]",
            "System.ArraySegment<byte>",
            "System.ArraySegment<byte>?",

            // extensions
            "UnityEngine.Vector2",
            "UnityEngine.Vector3",
            "UnityEngine.Vector4",
            "UnityEngine.Quaternion",
            "UnityEngine.Color",
            "UnityEngine.Bounds",
            "UnityEngine.Rect",
            "UnityEngine.AnimationCurve",
            "UnityEngine.RectOffset",
            "UnityEngine.Gradient",
            "UnityEngine.WrapMode",
            "UnityEngine.GradientMode",
            "UnityEngine.Keyframe",
            "UnityEngine.Matrix4x4",
            "UnityEngine.GradientColorKey",
            "UnityEngine.GradientAlphaKey",
            "UnityEngine.Color32",
            "UnityEngine.LayerMask",
            "UnityEngine.Vector2Int",
            "UnityEngine.Vector3Int",
            "UnityEngine.RangeInt",
            "UnityEngine.RectInt",
            "UnityEngine.BoundsInt",

            "System.Reactive.Unit",
        });

        private readonly Dictionary<string, string> knownGenericTypes = new Dictionary<string, string>
        {
#pragma warning disable SA1509 // Opening braces should not be preceded by blank line
            { "System.Collections.Generic.List<>", "global::MessagePack.Formatters.ListFormatter<TREPLACE>" },
            { "System.Collections.Generic.LinkedList<>", "global::MessagePack.Formatters.LinkedListFormatter<TREPLACE>" },
            { "System.Collections.Generic.Queue<>", "global::MessagePack.Formatters.QueueFormatter<TREPLACE>" },
            { "System.Collections.Generic.Stack<>", "global::MessagePack.Formatters.StackFormatter<TREPLACE>" },
            { "System.Collections.Generic.HashSet<>", "global::MessagePack.Formatters.HashSetFormatter<TREPLACE>" },
            { "System.Collections.ObjectModel.ReadOnlyCollection<>", "global::MessagePack.Formatters.ReadOnlyCollectionFormatter<TREPLACE>" },
            { "System.Collections.Generic.IList<>", "global::MessagePack.Formatters.InterfaceListFormatter2<TREPLACE>" },
            { "System.Collections.Generic.ICollection<>", "global::MessagePack.Formatters.InterfaceCollectionFormatter2<TREPLACE>" },
            { "System.Collections.Generic.IEnumerable<>", "global::MessagePack.Formatters.InterfaceEnumerableFormatter<TREPLACE>" },
            { "System.Collections.Generic.Dictionary<,>", "global::MessagePack.Formatters.DictionaryFormatter<TREPLACE>" },
            { "System.Collections.Generic.IDictionary<,>", "global::MessagePack.Formatters.InterfaceDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Generic.SortedDictionary<,>", "global::MessagePack.Formatters.SortedDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Generic.SortedList<,>", "global::MessagePack.Formatters.SortedListFormatter<TREPLACE>" },
            { "System.Linq.ILookup<,>", "global::MessagePack.Formatters.InterfaceLookupFormatter<TREPLACE>" },
            { "System.Linq.IGrouping<,>", "global::MessagePack.Formatters.InterfaceGroupingFormatter<TREPLACE>" },
            { "System.Collections.ObjectModel.ObservableCollection<>", "global::MessagePack.Formatters.ObservableCollectionFormatter<TREPLACE>" },
            { "System.Collections.ObjectModel.ReadOnlyObservableCollection<>", "global::MessagePack.Formatters.ReadOnlyObservableCollectionFormatter<TREPLACE>" },
            { "System.Collections.Generic.IReadOnlyList<>", "global::MessagePack.Formatters.InterfaceReadOnlyListFormatter<TREPLACE>" },
            { "System.Collections.Generic.IReadOnlyCollection<>", "global::MessagePack.Formatters.InterfaceReadOnlyCollectionFormatter<TREPLACE>" },
            { "System.Collections.Generic.ISet<>", "global::MessagePack.Formatters.InterfaceSetFormatter<TREPLACE>" },
            { "System.Collections.Concurrent.ConcurrentBag<>", "global::MessagePack.Formatters.ConcurrentBagFormatter<TREPLACE>" },
            { "System.Collections.Concurrent.ConcurrentQueue<>", "global::MessagePack.Formatters.ConcurrentQueueFormatter<TREPLACE>" },
            { "System.Collections.Concurrent.ConcurrentStack<>", "global::MessagePack.Formatters.ConcurrentStackFormatter<TREPLACE>" },
            { "System.Collections.ObjectModel.ReadOnlyDictionary<,>", "global::MessagePack.Formatters.ReadOnlyDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Generic.IReadOnlyDictionary<,>", "global::MessagePack.Formatters.InterfaceReadOnlyDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Concurrent.ConcurrentDictionary<,>", "global::MessagePack.Formatters.ConcurrentDictionaryFormatter<TREPLACE>" },
            { "System.Lazy<>", "global::MessagePack.Formatters.LazyFormatter<TREPLACE>" },
            { "System.Threading.Tasks<>", "global::MessagePack.Formatters.TaskValueFormatter<TREPLACE>" },

            { "System.Tuple<>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,,,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,,,,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },
            { "System.Tuple<,,,,,,,>", "global::MessagePack.Formatters.TupleFormatter<TREPLACE>" },

            { "System.ValueTuple<>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,,,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,,,,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },
            { "System.ValueTuple<,,,,,,,>", "global::MessagePack.Formatters.ValueTupleFormatter<TREPLACE>" },

            { "System.Collections.Generic.KeyValuePair<,>", "global::MessagePack.Formatters.KeyValuePairFormatter<TREPLACE>" },
            { "System.Threading.Tasks.ValueTask<>", "global::MessagePack.Formatters.KeyValuePairFormatter<TREPLACE>" },
            { "System.ArraySegment<>", "global::MessagePack.Formatters.ArraySegmentFormatter<TREPLACE>" },

            // extensions
            { "System.Collections.Immutable.ImmutableArray<>", "global::MessagePack.ImmutableCollection.ImmutableArrayFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableList<>", "global::MessagePack.ImmutableCollection.ImmutableListFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableDictionary<,>", "global::MessagePack.ImmutableCollection.ImmutableDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableHashSet<>", "global::MessagePack.ImmutableCollection.ImmutableHashSetFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableSortedDictionary<,>", "global::MessagePack.ImmutableCollection.ImmutableSortedDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableSortedSet<>", "global::MessagePack.ImmutableCollection.ImmutableSortedSetFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableQueue<>", "global::MessagePack.ImmutableCollection.ImmutableQueueFormatter<TREPLACE>" },
            { "System.Collections.Immutable.ImmutableStack<>", "global::MessagePack.ImmutableCollection.ImmutableStackFormatter<TREPLACE>" },
            { "System.Collections.Immutable.IImmutableList<>", "global::MessagePack.ImmutableCollection.InterfaceImmutableListFormatter<TREPLACE>" },
            { "System.Collections.Immutable.IImmutableDictionary<,>", "global::MessagePack.ImmutableCollection.InterfaceImmutableDictionaryFormatter<TREPLACE>" },
            { "System.Collections.Immutable.IImmutableQueue<>", "global::MessagePack.ImmutableCollection.InterfaceImmutableQueueFormatter<TREPLACE>" },
            { "System.Collections.Immutable.IImmutableSet<>", "global::MessagePack.ImmutableCollection.InterfaceImmutableSetFormatter<TREPLACE>" },
            { "System.Collections.Immutable.IImmutableStack<>", "global::MessagePack.ImmutableCollection.InterfaceImmutableStackFormatter<TREPLACE>" },

            { "Reactive.Bindings.ReactiveProperty<>", "global::MessagePack.ReactivePropertyExtension.ReactivePropertyFormatter<TREPLACE>" },
            { "Reactive.Bindings.IReactiveProperty<>", "global::MessagePack.ReactivePropertyExtension.InterfaceReactivePropertyFormatter<TREPLACE>" },
            { "Reactive.Bindings.IReadOnlyReactiveProperty<>", "global::MessagePack.ReactivePropertyExtension.InterfaceReadOnlyReactivePropertyFormatter<TREPLACE>" },
            { "Reactive.Bindings.ReactiveCollection<>", "global::MessagePack.ReactivePropertyExtension.ReactiveCollectionFormatter<TREPLACE>" },
#pragma warning restore SA1509 // Opening braces should not be preceded by blank line
        };

        private readonly Action<string> logger;

        private readonly bool disallowInternal;

        // visitor workspace:
        private HashSet<ITypeSymbol> alreadyCollected;
        private List<ObjectSerializationInfo> collectedObjectInfo;
        private List<EnumSerializationInfo> collectedEnumInfo;
        private List<ObjectSerializationInfo> collectedClosedTypeGenericInfo;

        public TypeCollector(Compilation compilation, bool disallowInternal, bool isForceUseMap, Action<string> logger)
        {
            this.logger = logger;
            this.typeReferences = new ReferenceSymbols(compilation, logger);
            this.disallowInternal = disallowInternal;
            this.isForceUseMap = isForceUseMap;

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
                    //   ((x.TypeKind == TypeKind.Interface) && x.GetAttributes().Any(x2 => x2.AttributeClass.ApproximatelyEqual(typeReferences.UnionAttribute)))
                    //|| ((x.TypeKind == TypeKind.Class && x.IsAbstract) && x.GetAttributes().Any(x2 => x2.AttributeClass.ApproximatelyEqual(typeReferences.UnionAttribute)))
                    //|| ((x.TypeKind == TypeKind.Class) && x.GetAttributes().Any(x2 => x2.AttributeClass.ApproximatelyEqual(typeReferences.MessagePackObjectAttribute)))
                    //|| ((x.TypeKind == TypeKind.Struct) && x.GetAttributes().Any(x2 => x2.AttributeClass.ApproximatelyEqual(typeReferences.MessagePackObjectAttribute))))
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

            if (this.embeddedTypes.Contains(typeSymbol.ToString()))
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
            if (this.isForceUseMap/* || (bool)contractAttr.ConstructorArguments[0].Value*/)
            {
                // All public members are serialize target except [Ignore] member.


                foreach (IPropertySymbol item in RoslynExtensions.GetAllMembers(type).OfType<IPropertySymbol>().Where(x => !x.IsOverride))
                {
                    // TODO: ignore 검사
                    var member = new MemberSerializationInfo
                    {
                        IsReadable = (item.GetMethod != null) && item.GetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                        IsWritable = (item.SetMethod != null) && item.SetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
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

                    members.Add(member);
                    this.CollectCore(item.Type); // recursive collect
                }

                foreach (IFieldSymbol item in RoslynExtensions.GetAllMembers(type).OfType<IFieldSymbol>())
                {
                    // TODO: ignore 검사

                    if (item.IsImplicitlyDeclared)
                    {
                        continue;
                    }

                    var customFormatterAttr = item.GetAttributes().FirstOrDefault(x => x.AttributeClass.ApproximatelyEqual(this.typeReferences.MessagePackFormatterAttribute))?.ConstructorArguments[0].Value as INamedTypeSymbol;

                    var member = new MemberSerializationInfo
                    {
                        IsReadable = item.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                        IsWritable = item.DeclaredAccessibility == Accessibility.Public && !item.IsReadOnly && !item.IsStatic,
                        IsProperty = false,
                        IsField = true,
                        Name = item.Name,
                        Type = item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        ShortTypeName = item.Type.ToDisplayString(BinaryWriteFormat),
                    };
                    if (!member.IsReadable && !member.IsWritable)
                    {
                        continue;
                    }

                    members.Add(member);
                    this.CollectCore(item.Type); // recursive collect
                }
            }
            else
            {
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
            }

            // GetConstructor
            IEnumerator<IMethodSymbol> ctorEnumerator = null;
            IMethodSymbol ctor = type.Constructors.Where(x => x.DeclaredAccessibility == Accessibility.Public).SingleOrDefault(x => x.GetAttributes().Any(y => y.AttributeClass.ApproximatelyEqual(this.typeReferences.SerializationConstructorAttribute)));
            if (ctor == null)
            {
                ctorEnumerator = type.Constructors.Where(x => x.DeclaredAccessibility == Accessibility.Public).OrderByDescending(x => x.Parameters.Length).GetEnumerator();

                if (ctorEnumerator.MoveNext())
                {
                    ctor = ctorEnumerator.Current;
                }
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

        private static bool TryGetNextConstructor(IEnumerator<IMethodSymbol> ctorEnumerator, ref IMethodSymbol ctor)
        {
            if (ctorEnumerator == null || ctor != null)
            {
                return false;
            }

            if (ctorEnumerator.MoveNext())
            {
                ctor = ctorEnumerator.Current;
                return true;
            }
            else
            {
                ctor = null;
                return false;
            }
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
