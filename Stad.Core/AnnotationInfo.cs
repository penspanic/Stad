using Stad.Annotation;

namespace Stad.Core
{
    public readonly struct AnnotationInfo
    {
        public AnnotationInfo(StadAnnotation[] annotations)
        {
            Annotations = annotations;
        }

        public static AnnotationInfo CreateFrom(Stad.Model.AnnotationInfoProto proto)
        {
            var result = new AnnotationInfo();
            // TODO: implement
            return result;
        }

        public readonly StadAnnotation[] Annotations;

        public T Get<T>() where T : StadAnnotation
        {
            if (Annotations == null || Annotations.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < Annotations.Length; ++i)
            {
                var annotation = Annotations[i];
                if (annotation is T found)
                {
                    return found;
                }
            }

            return null;
        }

        public bool TryGet<T>(out T returnValue) where T : StadAnnotation
        {
            returnValue = Get<T>();
            if (returnValue != null)
            {
                return true;
            }

            return false;
        }
    }
}