using System;
using System.Linq;

namespace DrDocx.Models.Helpers
{
    public static class FieldHelper
    {
        private static readonly string[] ValidFieldTypes = 
        {
            "Date",
            "Text",
            "Paragraph"
        };
        public static bool FieldTypeIsValid(string fieldType) =>
            ValidFieldTypes.Contains(fieldType, StringComparer.OrdinalIgnoreCase);
    }
}