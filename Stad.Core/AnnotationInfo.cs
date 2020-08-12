using Stad.Core.Annotation;

namespace Stad.Core
{
    public readonly struct AnnotationInfo
    {
        public AnnotationInfo(StadAnnotation[] annotations)
        {
            Annotations = annotations;
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
                if (annotation is T)
                {
                    return annotation as T;
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