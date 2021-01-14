using Base.Enums;
using Base.Models;
using Base.Services;
using BaseWeb.Services;
using Newtonsoft.Json.Linq;

namespace DbAdm.Services
{
    public class TableRead
    {
        //�]�w�d�� sql �M�d�����
        private ReadDto dto = new ReadDto()
        {
            ReadSql = @"
select 
    a.Id, a.Name, a.Cname, a.Status, 
    p.Name as ProjectName, p.DbName
from dbo.[Table] a
inner join dbo.Project p on p.Id = a.ProjectId
order by p.Id, a.Name
",
            ExportSql = @"
select 
    a.Name, a.Cname,  
    p.Name as ProjectName, p.DbName
from dbo.[Table] a
inner join dbo.Project p on p.Id = a.ProjectId
order by p.Id, a.Name
",
            TableAs = "a",
            Items = new [] {
                new QitemDto { Fid = "ProjectId" },
                new QitemDto { Fid = "Name", Op = ItemOpEstr.Like },
                new QitemDto { Fid = "Cname", Op = ItemOpEstr.Like2 },
                new QitemDto { Fid = "Status" },
            },
        };

        //�Ǧ^�@����ƦC
        public JObject GetPage(DtDto dt)
        {
            //�I�s�������O CrudRead
            return new CrudRead().GetPage(dto, dt);
        }

        public void Export(JObject cond)
        {
            _WebExcel.ScreenByCrud(dto, cond, "Table", _Locale.GetFilePath("Table.xlsx"), 1);
        }

    } //class
}
