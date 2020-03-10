namespace DrDocx.Models
{
    // TODO: Build this out including a symbolic formula parser to convert to percentile.
    // Not a v0.2-alpha thing but very useful.
    public class ScoreType : NamedModelBase
    {
        public string Formula { get; set; }
        public string Description { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}