﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class CrudEtable
    {
        public CrudEtable()
        {
            CrudEitem = new HashSet<CrudEitem>();
        }

        public string Id { get; set; }
        public string CrudId { get; set; }
        public string TableId { get; set; }
        public string Kid { get; set; }
        public string MapFid { get; set; }
        public string Col4 { get; set; }
        public string OrderBy { get; set; }
        public int Sort { get; set; }

        public virtual Crud Crud { get; set; }
        public virtual ICollection<CrudEitem> CrudEitem { get; set; }
    }
}
