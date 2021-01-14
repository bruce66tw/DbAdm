using System;
using System.Collections.Generic;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DbName { get; set; }
        public string ProjectSpace { get; set; }
        public string ProjectPath { get; set; }
        public string ConnectStr { get; set; }
        public bool Status { get; set; }
    }
}
