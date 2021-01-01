using System.Collections.ObjectModel;
using System.Linq;

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

        public static StadModel CreateFrom(Stad.Model.StadModelProto proto)
        {
            return new StadModel(
                proto.Type,
                AnnotationInfo.CreateFrom(proto.Annotationinfo),
                new ReadOnlyCollection<MemberDefinition>(
                    proto.Members.Select(m => MemberDefinition.CreateFrom(m)).ToList())
                );
        }

        public string Type { get; }
        public int TypeCode { get; }
        public AnnotationInfo AnnotationInfo { get; }
        public ReadOnlyCollection<MemberDefinition> Members { get; }
    }
}