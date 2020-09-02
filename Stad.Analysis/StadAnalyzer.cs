using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stad.Annotation;
using Stad.Container;
using Stad.Core;
using Stad.Core.Model;
using Stad.Core.Source.Assembly;

namespace Stad.Analysis
{
    public static class StadAnalyzer
    {
        private class AnalyzeContext
        {
            public AnalyzeContext(TypeCollectorResult collectResult)
            {
                CollectResult = collectResult;
            }

            public readonly TypeCollectorResult CollectResult;
            public List<StadModel> AllModels = new List<StadModel>();
        }

        public static async Task<StadRegistry> MakeRegistryFromSource(string rootPath)
        {
            var compilation = await CreateFromDirectoryAsync(rootPath, null, CancellationToken.None);
            TypeCollectorForCompilation typeCollectorForCompilation = new TypeCollectorForCompilation(compilation, true, (str) => Console.WriteLine(str));
            var result = typeCollectorForCompilation.Collect();
            return MakeRegistry(result);
        }

        public static async Task<StadRegistry> MakeRegistryFromAssembly(string[] dllPaths)
        {
            foreach (string path in dllPaths)
            {
                Assembly assembly = Assembly.LoadFile(path);
                assembly.GetTypes()
            }
        }

        private static Task<CSharpCompilation> CreateFromProjectAsync(string[] csprojs, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromProjectAsync(csprojs, preprocessorSymbols, cancellationToken);
        }

        private static Task<CSharpCompilation> CreateFromDirectoryAsync(string directoryRoot, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromDirectoryAsync(directoryRoot, preprocessorSymbols, cancellationToken);
        }

        private static ObjectSerializationInfo GetObjectInfo(TypeCollectorResult collectorResult, string type)
        {
            var memberObjectInfo = collectorResult.CollectedObjectInfo.FirstOrDefault(i => i.FullName == type);
            if (memberObjectInfo == null)
            {
                memberObjectInfo = collectorResult.CollectedClosedTypeGenericInfo.FirstOrDefault(i => i.FullName == type);
            }

            return memberObjectInfo;
        }

        private static StadRegistry MakeRegistry(TypeCollectorResult result)
        {
            AnalyzeContext context = new AnalyzeContext(result);
            List<DataSetModel> dataSetModels = new List<DataSetModel>();

            foreach (ObjectSerializationInfo info in result.CollectedObjectInfo)
            {
                var dataSetDefinitionAttribute = info.Attributes.FirstOrDefault(a =>
                    a.AttributeClass?.Name.Contains(nameof(Stad.Annotation.DataSetDefinition)) ?? false);
                if (dataSetDefinitionAttribute != null)
                {
                    var dataSetModel = MakeDataSetModel(context, info);
                    if (dataSetModel != null)
                    {
                        dataSetModels.Add(dataSetModel);
                    }
                }
            }

            return StadRegistry.Create(new ReadOnlyCollection<DataSetModel>(dataSetModels));
        }

        private static DataSetModel MakeDataSetModel(AnalyzeContext context, ObjectSerializationInfo info)
        {
            List<StadModel> listModels = new List<StadModel>();
            List<StadModel> singleModels = new List<StadModel>();

            foreach (MemberSerializationInfo member in info.Members)
            {
                var memberObjectInfo = GetObjectInfo(context.CollectResult, member.Type);
                if (memberObjectInfo == null)
                {
                    throw new Exception($"Member not found, {member.Type}");
                }

                if (memberObjectInfo.Attributes.FirstOrDefault(a =>
                    a.AttributeClass?.Name.Contains(nameof(Stad.Annotation.IgnoreMemberAttribute)) ?? false) != null)
                {
                    continue;
                }

                StadModel memberModel = MakeStadModel(context, memberObjectInfo);
                if (memberModel == null)
                {
                    throw new Exception($"Member create failed, {memberObjectInfo}");
                }

                if (memberModel.Type.Contains("StadKeyValueCollection") == true)
                {
                    listModels.Add(memberModel);
                }
                else
                {
                    singleModels.Add(memberModel);
                }
            }

            foreach (StadModel model in context.AllModels)
            {
                foreach (MemberDefinition memberDefinition in model.Members)
                {
                    var memberModel = context.AllModels.Find(m => m.Type == memberDefinition.Type);
                    memberDefinition.SetModel(memberModel);
                }
            }

            return new DataSetModel(info.FullName, new ReadOnlyCollection<StadModel>(listModels), new ReadOnlyCollection<StadModel>(singleModels));
        }

        // TODO: renaming
        private static StadModel MakeStadModel(in AnalyzeContext context, ObjectSerializationInfo objectInfo)
        {
            List<MemberDefinition> memberDefinitions = new List<MemberDefinition>();
            foreach (MemberSerializationInfo eachMember in objectInfo.Members)
            {
                var memberDefinition = new MemberDefinition(eachMember.Type, eachMember.Name, eachMember.IsField ? MemberKind.Field : MemberKind.Property,
                    MakeMemberAnnotationInfo(eachMember));
                memberDefinitions.Add(memberDefinition);
                if (context.AllModels.Exists(m => m.Type == eachMember.Type) == false)
                {
                    var memberObjectInfo = GetObjectInfo(context.CollectResult, memberDefinition.Type);
                    if (memberObjectInfo != null)
                    {
                        MakeStadModel(context, memberObjectInfo);
                    }
                }
            }

            var model = new StadModel(objectInfo.FullName, MakeObjectAnnotationInfo(objectInfo),
                new ReadOnlyCollection<MemberDefinition>(memberDefinitions));
            context.AllModels.Add(model);

            return model;
        }

        private static AnnotationInfo MakeObjectAnnotationInfo(ObjectSerializationInfo objectInfo)
        {
            return MakeAnnotationInfo(objectInfo.Attributes);
        }

        private static AnnotationInfo MakeMemberAnnotationInfo(MemberSerializationInfo memberInfo)
        {
            return MakeAnnotationInfo(memberInfo.Attributes);
        }

        private static AnnotationInfo MakeAnnotationInfo(ImmutableArray<AttributeData> attributeDatas)
        {
            if (attributeDatas.IsDefaultOrEmpty)
            {
                return new AnnotationInfo();
            }

            var list = new List<Annotation.StadAnnotation>();
            foreach (AttributeData data in attributeDatas)
            {
                if (data.AttributeClass == null)
                {
                    continue;
                }

                // TODO: implement
                // TODO: client side Annotation과 Stad Model에서의 Annotation은 다른 것을 사용해야 하는가?

                switch (data.AttributeClass.Name)
                {
                    case nameof(Annotation.IdForAttribute):
                        list.Add(new IdForAttribute(null));
                        break;
                }
            }

            return new AnnotationInfo(list.ToArray());
        }
    }
}