namespace Project.Api;

public class ModuleActionAttribute : Attribute
{
    public ModuleActionAttribute(string actionName)
    {
        ActionName = actionName;
    }
    public string ActionName { get; set; }
}
