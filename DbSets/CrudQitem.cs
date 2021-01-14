﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class CrudQitem
    {
        public string Id { get; set; }
        public string CrudId { get; set; }
        public string ColumnId { get; set; }
        public string TableAs { get; set; }
        public string InputType { get; set; }
        public string InputData { get; set; }
        public string Op { get; set; }
        public bool IsRange { get; set; }
        public bool IsFind2 { get; set; }
        public string PosGroup { get; set; }
        public string LayoutCols { get; set; }
        public string ExtInfo { get; set; }
        public int Sort { get; set; }

        public virtual Crud Crud { get; set; }
    }
}
