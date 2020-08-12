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
        public MemberDefinition(string name, MemberKind kind, AnnotationInfo annotationInfo, StadModel model)
        {
            Name = name;
            Kind = kind;
            AnnotationInfo = annotationInfo;
            Model = model;
        }

        public readonly string Name;
        public readonly MemberKind Kind;
        public readonly AnnotationInfo AnnotationInfo;
        public readonly StadModel Model;
    }
}