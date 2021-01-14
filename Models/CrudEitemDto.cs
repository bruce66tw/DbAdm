namespace DbAdm.Models
{
    //for Gen Crud
    public class CrudEitemDto : CrudEitem0Dto
    {
        //base
        public string CrudId { get; set; }
        public string EtableId { get; set; }
        //public string Fid { get; set; }
        //public string Cname { get; set; }
        //public string DataType { get; set; }

        //public bool Required { get; set; }
        public bool HasCreate { get; set; }
        public bool HasUpdate { get; set; }
        //public string CheckType { get; set; }
        //public string CheckData { get; set; }

        //public string InputType { get; set; }
        //public string InputData { get; set; }
        //public string PosGroup { get; set; }
        //public string LayoutCols { get; set; }
        //public string PlaceHolder { get; set; }

        //extend
        public string ServiceStr { get; set; }
        public string ViewStr { get; set; }
        public string HeadStr { get; set; }     //for child table, header string

        //public bool IsGroupStart { get; set; }  //同一列開始
        //public bool IsGroupEnd { get; set; }    //同一列結束
        //public bool IsGroup { get; set; }       //同一列

    }
}