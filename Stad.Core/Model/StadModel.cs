using System.Collections.ObjectModel;

namespace Stad.Core.Model
{
    public class StadModel : IStadModel
    {
        public StadModel(AnnotationInfo annotationInfo, ReadOnlyCollection<MemberDefinition> members)
        {
            AnnotationInfo = annotationInfo;
            Members = members;
        }

        public readonly AnnotationInfo AnnotationInfo;
        public readonly ReadOnlyCollection<MemberDefinition> Members;
    }
}