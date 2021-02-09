﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class CrudRitem
    {
        public string Id { get; set; }
        public string CrudId { get; set; }
        public string ColumnCode { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public string RitemType { get; set; }
        public string ExtInfo { get; set; }
        public int Sort { get; set; }

        public virtual Crud Crud { get; set; }
    }
}
