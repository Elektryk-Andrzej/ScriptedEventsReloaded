// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    // Required by the compiler for C# 11 'required' keyword
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class RequiredMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
    {
        public string FeatureName { get; } = featureName;

        public const string RefStructs = "RefStructs";
        public const string RequiredMembers = "RequiredMembers";
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class SetsRequiredMembersAttribute : Attribute { }
}