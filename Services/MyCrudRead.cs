using Base.Enums;
using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace DbAdm.Services
{
    public class MyCrudRead
    {
        //設定查詢 sql 和查詢欄位
        private ReadDto model = new ReadDto()
        {
            ReadSql = @"
select 
    a.Id, a.Status, 
    t.Name as TableName,
    p.Name as ProjectName,
    a.Created
from dbo.Crud a
join dbo.Project p on p.Id = a.ProjectId
join dbo.[Table] t on t.Id = a.TableId
order by p.Id, a.Id desc
",
            TableAs = "a",
            Items = new [] {
                new QitemDto { Fid = "ProjectId" },
                new QitemDto { Fid = "Name", Col = "t.Name", Op = ItemOpEstr.Like },
                new QitemDto { Fid = "Status" },
            },
        };

        public JObject GetPage(DtDto dt)
        {
            return new CrudRead().GetPage(model, dt);
        }

    } //class
}
