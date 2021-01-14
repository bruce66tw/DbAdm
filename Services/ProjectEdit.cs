using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DbAdm.Services
{
    public class ProjectEdit
    {
        private EditDto GetDto()
        {
            return new EditDto
            {
                Table = "dbo.Project",
                PkeyFid = "Id",   //primary key 欄位id
                Col4 = null,
                Items = new[] {
                    new EitemDto { Fid = "Id" },
                    new EitemDto { Fid = "Name", Required = true },
                    new EitemDto { Fid = "DbName", Required = true },
                    new EitemDto { Fid = "ProjectSpace", Required = true },
                    new EitemDto { Fid = "ProjectPath", Required = true },
                    //new EitemModel { Fid = "ServiceSpace", Required = true },
                    //new EitemModel { Fid = "ServicePath", Required = true },
                    new EitemDto { Fid = "ConnectStr", Required = true },
                    new EitemDto { Fid = "Status", Required = true },
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

        //key為空白表示新增資料
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

        /*
        public ErrorDto Delete(string key)
        {
            var sql = @"
delete c
from dbo.[Column] c 
inner join dbo.[Table] t on c.TableId=t.Id
where t.ProjectId=@Id;

delete dbo.[Table]
where ProjectId=@Id;

delete dbo.[Project]
where Id=@Id;
";
            return _Db.Update(sql, new List<object>() { "Id", key })
                ? null
                : new ErrorDto() { Msg = "Delete Failed." };
            //return new CrudEdit(_crud).DeleteRow(key);
        }
        */

    } //class
}
