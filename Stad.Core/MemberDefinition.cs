using Stad.Core.Model;

namespace Stad.Core
{
    public enum MemberKind
    {
        Undefined = 0,
        Field,
        Property,
    }

    public class MemberDefinition
    {
        public MemberDefinition(string type, string name, MemberKind kind, AnnotationInfo annotationInfo)
        {
            Type = type;
            Name = name;
            Kind = kind;
            AnnotationInfo = annotationInfo;
        }

        // TODO: Caching 해둬도 될만한 내용인 듯
        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public readonly string Type;
        public readonly string Name;
        public readonly MemberKind Kind;
        public readonly AnnotationInfo AnnotationInfo;
        public StadModel Model { get; private set; }

        public void SetModel(StadModel model)
        {
            Model = model;
        }
    }
}