using System;

namespace DrDocx.Models
{
    public abstract class DatabaseModelBase
    {
        public int Id { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    }
}