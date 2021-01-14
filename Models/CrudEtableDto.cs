using System.Collections.Generic;

namespace DbAdm.Models
{
    //for Gen Crud
    public class CrudEtableDto
    {
        //base
        public string Id { get; set; }
        public string CrudId { get; set; }
        public string Table { get; set; }       //table name
        public string TableCname { get; set; }  //table cname
        public string PkeyFid { get; set; }
        public string FkeyFid { get; set; }
        public string OrderBy { get; set; }
        public bool HasCol4 { get; set; }

        //extend
        public string SortFid { get; set; }     //sort field id
        public List<CrudEitemDto> Eitems { get; set; }
        //hide item string for edit view
        public List<string> HideViewStrs { get; set; }
    }
}