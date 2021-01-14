using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace DbAdm.Services
{
    public class MyCrudEdit
    {
        private EditDto GetDto() {
            //���@4��tables
            return new EditDto
            {
                Table = "dbo.Crud",
                PkeyFid = "Id",
                Col4 = new string[] { null, "Created", null, "Revised"},
                Items = new[] {
                    new EitemDto { Fid = "Id" },
                    new EitemDto { Fid = "ProjectId", Required = true },
                    new EitemDto { Fid = "TableId", Required = true },
                    new EitemDto { Fid = "FunName", Required = true },
                    new EitemDto { Fid = "LabelHori", Required = true },
                    new EitemDto { Fid = "ReadSql" },
                    new EitemDto { Fid = "TableAs" },
                    new EitemDto { Fid = "HasCreate" },
                    new EitemDto { Fid = "HasUpdate" },
                    new EitemDto { Fid = "HasDelete" },
                    new EitemDto { Fid = "HasView" },
                    new EitemDto { Fid = "HasExport" },
                    new EitemDto { Fid = "HasReset" },
                    new EitemDto { Fid = "AuthType", Required = true },
                    new EitemDto { Fid = "Status", Required = true },
                },
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        //����child readSql���ϥ� in �y�k
                        ReadSql = @"
select a.*,
	c.Name, c.Cname, c.DataType
from dbo.CrudQitem a
join dbo.[Column] c on a.ColumnId=c.Id
where a.CrudId in ({0})
order by a.Sort
",
                        Table = "dbo.CrudQitem",                        
                        PkeyFid = "Id",
                        FkeyFid = "CrudId",
                        OrderBy = "Sort",
                        Col4 = null,
                        Items = new [] {
                            new EitemDto { Fid = "Id" },
                            new EitemDto { Fid = "CrudId" },
                            new EitemDto { Fid = "ColumnId", Required = true },
                            new EitemDto { Fid = "TableAs" },
                            new EitemDto { Fid = "InputType", Required = true },
                            new EitemDto { Fid = "InputData" },
                            new EitemDto { Fid = "Op", Required = true },
                            new EitemDto { Fid = "IsRange" },
                            new EitemDto { Fid = "IsFind2" },
                            new EitemDto { Fid = "PosGroup" },
                            new EitemDto { Fid = "LayoutCols" },
                            new EitemDto { Fid = "ExtInfo" },
                            new EitemDto { Fid = "Sort", Required = true },
                        },
                    },
                    new EditDto
                    {
                        //����child readSql���ϥ� in �y�k
                        ReadSql = @"
select a.*
from dbo.CrudRitem a
where a.CrudId in ({0})
order by a.Sort
",
                        Table = "dbo.CrudRitem",
                        PkeyFid = "Id",
                        FkeyFid = "CrudId",
                        OrderBy = "Sort",
                        Col4 = null,
                        Items = new [] {
                            new EitemDto { Fid = "Id" },
                            new EitemDto { Fid = "CrudId" },
                            new EitemDto { Fid = "ColumnName", Required = true },
                            new EitemDto { Fid = "Cname", Required = true },
                            new EitemDto { Fid = "Width", /*Type = ItemTypeEnum.Num*/ },
                            new EitemDto { Fid = "RitemType", Required = true },
                            new EitemDto { Fid = "ExtInfo" },
                            new EitemDto { Fid = "Sort", Required = true },
                        },
                    },
                    new EditDto
                    {
                        Table = "dbo.CrudEtable",
                        PkeyFid = "Id",
                        FkeyFid = "CrudId",
                        OrderBy = "Sort",
                        Col4 = null,
                        Items = new [] {
                            new EitemDto { Fid = "Id" },
                            new EitemDto { Fid = "CrudId" },
                            new EitemDto { Fid = "TableId", Required = true },
                            new EitemDto { Fid = "Kid", Required = true },
                            new EitemDto { Fid = "MapFid" },
                            new EitemDto { Fid = "Col4" },
                            new EitemDto { Fid = "OrderBy" },
                            new EitemDto { Fid = "Sort", Required = true },
                        },
                        Childs = new []
                        {
                            new EditDto
                            {
                                ReadSql = @"
select a.*,
	c.Name, c.Cname, c.DataType
from dbo.CrudEitem a
join dbo.[Column] c on a.ColumnId=c.Id
where a.EtableId in ({0})
order by a.Sort
",
                                Table = "dbo.CrudEitem",
                                PkeyFid = "Id",
                                FkeyFid = "EtableId",
                                OrderBy = "Sort",
                                Col4 = null,
                                Items = new [] {
                                    new EitemDto { Fid = "Id" },
                                    new EitemDto { Fid = "EtableId" },
                                    new EitemDto { Fid = "ColumnId", Required = true },
                                    new EitemDto { Fid = "InputType", Required = true },
                                    new EitemDto { Fid = "InputData" },
                                    new EitemDto { Fid = "Required" },
                                    new EitemDto { Fid = "HasCreate" },
                                    new EitemDto { Fid = "HasUpdate" },
                                    new EitemDto { Fid = "PlaceHolder" },                                    
                                    new EitemDto { Fid = "DefaultValue" },
                                    new EitemDto { Fid = "PosGroup" },
                                    new EitemDto { Fid = "LayoutCols" },
                                    new EitemDto { Fid = "Width" },
                                    new EitemDto { Fid = "CheckType" },
                                    new EitemDto { Fid = "CheckData" },
                                    new EitemDto { Fid = "Sort" },    //��ݳ]�w
                                },
                            },
                        },
                    },
                },
            };      
        }

        private CrudEdit Service()
        {
            return new CrudEdit(GetDto());
        }

        public JObject GetJson(string key)
        {
            return Service().GetJson(key);
        }

        public ResultDto SaveCreate(JObject json)
        {
            #region check input
            //Etable���i����
            #endregion

            return Service().SaveCreate(json);
        }

        public ResultDto SaveUpdate(string key, JObject json)
        {
            return Service().SaveUpdate(key, json);
        }

        /*
        //return error msg if any
        private string WhenSave(bool isNew, JObject row)
        {
            //case of ok
            return "";
        }
        */

        public ResultDto Delete(string key)
        {
            return Service().Delete(key);
        }

    } //class
}
