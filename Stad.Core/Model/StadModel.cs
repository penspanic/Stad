using System.Collections.ObjectModel;

namespace Stad.Core.Model
{
    public class StadModel : IStadModel
    {
        public override string ToString()
        {
            return Type;
        }

        public StadModel(string type, AnnotationInfo annotationInfo, ReadOnlyCollection<MemberDefinition> members)
        {
            Type = type;
            TypeCode = Utility.ToTypeCode(type);
            AnnotationInfo = annotationInfo;
            Members = members;
        }

        public string Type { get; }
        public int TypeCode { get; }
        public AnnotationInfo AnnotationInfo { get; }
        public ReadOnlyCollection<MemberDefinition> Members { get; }
    }
}