using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
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
        public static async Task<StadRegistry> MakeRegistry(string rootPath)
        {
            var compilation = await CreateFromDirectoryAsync(rootPath, null, CancellationToken.None);
            TypeCollector typeCollector = new TypeCollector(compilation, true, false, (str) => Console.WriteLine(str));
            var result = typeCollector.Collect();
            return MakeRegistry(result);
        }

        private static Task<CSharpCompilation> CreateFromProjectAsync(string[] csprojs, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromProjectAsync(csprojs, preprocessorSymbols, cancellationToken);
        }

        private static Task<CSharpCompilation> CreateFromDirectoryAsync(string directoryRoot, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromDirectoryAsync(directoryRoot, preprocessorSymbols, cancellationToken);
        }

        private static StadRegistry MakeRegistry(TypeCollectorResult result)
        {
            List<DataSetModel> dataSetModels = new List<DataSetModel>();

            foreach (ObjectSerializationInfo info in result.CollectedObjectInfo)
            {
                var dataSetDefinitionAttribute = info.Attributes.FirstOrDefault(a =>
                    a.AttributeClass?.Name.Contains(nameof(Stad.Annotation.DataSetDefinition)) ?? false);
                if (dataSetDefinitionAttribute != null)
                {
                    var dataSetModel = MakeDataSetModel(result, info);
                    if (dataSetModel != null)
                    {
                        dataSetModels.Add(dataSetModel);
                    }
                }
            }

            return StadRegistry.Create(new ReadOnlyCollection<DataSetModel>(dataSetModels));
        }

        private static DataSetModel MakeDataSetModel(TypeCollectorResult collectorResult, ObjectSerializationInfo info)
        {
            List<StadModel> allModels = new List<StadModel>();
            List<StadModel> listModels = new List<StadModel>();
            List<StadModel> singleModels = new List<StadModel>();

            foreach (MemberSerializationInfo member in info.Members)
            {
                var memberObjectInfo = collectorResult.CollectedObjectInfo.FirstOrDefault(i => i.FullName == member.Type);
                if (memberObjectInfo == null)
                {
                    memberObjectInfo = collectorResult.CollectedClosedTypeGenericInfo.FirstOrDefault(i => i.FullName == member.Type);
                    if (memberObjectInfo == null)
                    {
                        throw new Exception($"Member not found, {member.Type}");
                    }
                }

                if (memberObjectInfo.Attributes.FirstOrDefault(a =>
                    a.AttributeClass?.Name.Contains(nameof(Stad.Annotation.IgnoreMemberAttribute)) ?? false) != null)
                {
                    continue;
                }

                StadModel memberModel = MakeStadModel(collectorResult,member, memberObjectInfo);
                if (memberModel == null)
                {
                    throw new Exception($"Member create failed, {memberObjectInfo}");
                }

                allModels.Add(memberModel);
                if (memberModel.Type.Contains("StadKeyValueCollection") == true)
                {
                    listModels.Add(memberModel);
                }
                else
                {
                    singleModels.Add(memberModel);
                }
            }

            foreach (StadModel model in allModels)
            {
                foreach (MemberDefinition memberDefinition in model.Members)
                {
                    var memberModel = allModels.Find(m => m.Type == memberDefinition.Type);
                    if (memberModel != null)
                    {
                        memberDefinition.SetModel(memberModel);
                    }
                }
            }

            return new DataSetModel(new ReadOnlyCollection<StadModel>(listModels), new ReadOnlyCollection<StadModel>(singleModels));
        }

        private static StadModel MakeStadModel(in TypeCollectorResult collectorResult, MemberSerializationInfo memberInfo, ObjectSerializationInfo objectInfo)
        {
            string type = objectInfo.Name;
            List<MemberDefinition> memberDefinitions = new List<MemberDefinition>();
            foreach (MemberSerializationInfo eachMember in objectInfo.Members)
            {
                var memberDefinition = new MemberDefinition(eachMember.Type, eachMember.Name, eachMember.IsField ? MemberKind.Field : MemberKind.Property,
                    MakeMemberAnnotationInfo(eachMember));
                memberDefinitions.Add(memberDefinition);
            }

            return new StadModel(type, MakeMemberAnnotationInfo(memberInfo), new ReadOnlyCollection<MemberDefinition>(memberDefinitions));
        }

        private static AnnotationInfo MakeMemberAnnotationInfo(MemberSerializationInfo memberInfo)
        {
            var attributes = memberInfo.Attributes;
            if (attributes.IsDefaultOrEmpty)
            {
                return new AnnotationInfo();
            }

            var list = new List<Annotation.StadAnnotation>();
            foreach (AttributeData data in attributes)
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