using System;
using System.Collections.Generic;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class Column
    {
        public string Id { get; set; }
        public string TableId { get; set; }
        public string Name { get; set; }
        public string Cname { get; set; }
        public string DataType { get; set; }
        public bool Nullable { get; set; }
        public string DefaultValue { get; set; }
        public int Sort { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
}
