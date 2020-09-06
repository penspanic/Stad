using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Stad.Annotation
{
    public abstract class StadAnnotation : Attribute
    {
        
    }

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct)]
    public class DataSetDefinition : StadAnnotation
    {
        
    }

    //[StructLayout(LayoutKind.Explicit)]
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class RangeAttribute : StadAnnotation
    {
        //[FieldOffset(0)]
        public readonly int IntStart;
        //[FieldOffset(4)]
        public readonly int IntEnd;
        //[FieldOffset(0)]
        public readonly float FloatStart;
        //[FieldOffset(4)]
        public readonly float FloatEnd;

        public RangeAttribute(int start, int end)
        {
            IntStart = start;
            IntEnd = end;
        }

        public RangeAttribute(float start, float end)
        {
            FloatStart = start;
            FloatEnd = end;
        }

        public static RangeAttribute FromArgument(ImmutableArray<object> arguments)
        {
            return new RangeAttribute((int)arguments[0], (int)arguments[1]);
        }
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class NameAttribute : StadAnnotation
    {
        public readonly string Name;

        public NameAttribute(string name)
        {
            Name = name;
        }

        public static NameAttribute FromArgument(ImmutableArray<object> arguments)
        {
            return new NameAttribute((string) arguments[0]);
        }
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class DescAttribute : StadAnnotation
    {
        public readonly string[] DescLines;
        public DescAttribute(params string[] lines)
        {
            DescLines = lines;
            // 한줄로 합쳐진 것도 필요할지도?
        }

        public static DescAttribute FromArgument(ImmutableArray<object> arguments)
        {
            return new DescAttribute((string[]) arguments[0]);
        }
    }

    public class ReferenceAttribute : StadAnnotation
    {
        // TODO: 단순 type이 아닌 특정 DataSet 지정할 수 있게? 하나의 타입으로 여러 데이터가 구성될 수 있으니.
        public readonly string TypeFullName;
        
        public ReferenceAttribute(Type type)
        {
            TypeFullName = type.FullName;
        }

        private ReferenceAttribute(string typeFullName)
        {
            TypeFullName = typeFullName;
        }

        public static ReferenceAttribute FromArgument(ImmutableArray<object> arguments)
        {
            return new ReferenceAttribute((string) arguments[0]);
        }
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class IgnoreMemberAttribute : StadAnnotation
    {
        
    }
}