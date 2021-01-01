using Stad.Core.Model;

namespace Stad.Core
{
    public class MemberDefinition
    {
        public MemberDefinition(string type, string name, Stad.Model.MemberKind kind, AnnotationInfo annotationInfo)
        {
            Type = type;
            Name = name;
            Kind = kind;
            AnnotationInfo = annotationInfo;
        }

        public static MemberDefinition CreateFrom(Stad.Model.MemberDefinitionProto proto)
        {
            var result = new MemberDefinition(proto.Type, proto.Name, proto.Kind,       AnnotationInfo.CreateFrom(proto.AnnotationInfo));
            return result;
        }

        // TODO: Caching 해둬도 될만한 내용인 듯
        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public readonly string Type;
        public readonly string Name;
        public readonly Stad.Model.MemberKind Kind;
        public readonly AnnotationInfo AnnotationInfo;
        public StadModel Model { get; private set; } // TODO: Immutable 하도록 수정

        public void SetModel(StadModel model)
        {
            Model = model;
        }
    }
}