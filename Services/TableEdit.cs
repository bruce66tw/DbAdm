using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace DbAdm.Services
{
    public class TableEdit
    {
        private EditDto GetDto()
        {
            return new EditDto
            {
                Table = "dbo.[Table]",
                PkeyFid = "Id",
                Col4 = null,
                Items = new[] {
                    new EitemDto { Fid = "Id" },
                    new EitemDto { Fid = "ProjectId" },
                    new EitemDto { Fid = "Name" },
                    new EitemDto { Fid = "Cname" },
                    new EitemDto { Fid = "Note" },
                    new EitemDto { Fid = "Status" },
                },
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        Table = "dbo.[Column]",
                        PkeyFid = "Id",
                        FkeyFid = "TableId",
                        OrderBy = "Sort",
                        Col4 = null,
                        Items = new [] {
                            new EitemDto { Fid = "Id" },
                            new EitemDto { Fid = "TableId" },
                            new EitemDto { Fid = "Name" },
                            new EitemDto { Fid = "Cname" },
                            new EitemDto { Fid = "DataType" },
                            new EitemDto { Fid = "Nullable" },
                            new EitemDto { Fid = "DefaultValue" },
                            new EitemDto { Fid = "Sort" },
                            new EitemDto { Fid = "Note" },
                            new EitemDto { Fid = "Status" },
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
            return Service().SaveCreate(json);
        }

        public ResultDto SaveUpdate(string key, JObject json)
        {
            return Service().SaveUpdate(key, json);
        }

        public ResultDto Delete(string key)
        {
            return Service().Delete(key);
        }

    } //class
}
