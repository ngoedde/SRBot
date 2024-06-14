namespace SRGame.Client.Entity;

[System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]  // Multiuse attribute.
public class TranslationAttribute(string fieldName) : System.Attribute
{
    public string FieldName { get; } = fieldName;
    public string Value { get; }
}