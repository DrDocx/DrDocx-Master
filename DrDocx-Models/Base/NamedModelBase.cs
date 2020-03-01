namespace DrDocx.Models
{
    /// <summary>
    /// Inherits from DatabaseModelBase and adds a Name property so that all models with names can inherit from it.
    /// </summary>
    public abstract class NamedModelBase : DatabaseModelBase
    {
        public string Name { get; set; }
    }
}