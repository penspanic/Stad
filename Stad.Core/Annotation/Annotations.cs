using System;
using System.Runtime.InteropServices;

namespace Stad.Core.Annotation
{
    public abstract class StadAnnotation : Attribute
    {
        
    }

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct)]
    public class RegistryDefinitionAttribute : StadAnnotation
    {
        
    }

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct)]
    public class StadModelAttribute : StadAnnotation
    {
    }

    [StructLayout(LayoutKind.Explicit)]
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class RangeAttribute : StadAnnotation
    {
        [FieldOffset(0)]
        public readonly int IntStart;
        [FieldOffset(4)]
        public readonly int IntEnd;
        [FieldOffset(0)]
        public readonly float FloatStart;
        [FieldOffset(4)]
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
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class NameAttribute : StadAnnotation
    {
        public readonly string Name;

        public NameAttribute(string name)
        {
            Name = name;
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
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class IdForAttribute : StadAnnotation
    {
        public IdForAttribute(Type type)
        {
            
        }
    }

    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HiddenAttribute : StadAnnotation
    {
        
    }
}