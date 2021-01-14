using Base.Enums;
using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace DbAdm.Services
{
    public class ColumnRead
    {
        private ReadDto model = new ReadDto()
        {
            ReadSql = @"
Select 
    p.Name as ProjectName, t.Name as TableName,
    c.Id, c.Name, c.Cname, 
    c.Status, c.DataType
From dbo.[Column] c
inner join dbo.[Table] t on t.Id=c.TableId
inner join dbo.Project p on p.Id=t.ProjectId
Order by p.Id, t.Id, c.Sort
",
            TableAs = "c",
            Items = new [] {
                new QitemDto { Fid = "ProjectId", Col = "t.ProjectId" },
                new QitemDto { Fid = "TableName", Col = "t.Name", Op = ItemOpEstr.Like },
                new QitemDto { Fid = "Name", Op = ItemOpEstr.Like },
                new QitemDto { Fid = "Status" },
            },
        };

        //�Ǧ^�@����ƦC
        public JObject GetPage(DtDto dt)
        {
            //�I�s�������O CrudRead
            return new CrudRead().GetPage(model, dt);
        }        

    } //class
}
