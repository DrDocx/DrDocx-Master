using System.Linq;

namespace DrDocx.API.Helpers
{
    public static class RecordExists
    {
        public static bool FieldExists(DatabaseContext context, int id) => context.Fields.Any(e => e.Id == id);
        public static bool FieldGroupExists(DatabaseContext context, int id) => context.FieldGroups.Any(e => e.Id == id);
        public static bool PatientExists(DatabaseContext context, int id) => context.Patients.Any(e => e.Id == id);
        
    }
}