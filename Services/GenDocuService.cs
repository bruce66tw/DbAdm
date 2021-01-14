using Base.Services;
using BaseWeb.Services;
using DbAdm.Models;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbAdm.Services
{
    public class GenDocuService
    {
        /// <summary>
        /// 在畫面上直接產生word檔 for db文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="tableIds"></param>
        public void Run(string projectId, string[] tableIds = null)
        {
            #region check template file
            var error = "";
            //var locale = _Fun.GetLocale();
            var tplPath = _Fun.DirRoot + "_template\\Table.docx";
            if (!File.Exists(tplPath))
            {
                error = "no file " + tplPath;
                goto lab_error;
            }
            #endregion

            #region column rows & check empty
            var args = new List<object>();
            var sql = @"
select 
    p.Name as ProjectName,
	t.Name as TableName, t.Cname as TableCname,
	c.Name, c.Cname, c.DataType,
	c.Nullable, c.DefaultValue, c.Note,
	c.Sort
from dbo.[Column] c
inner join dbo.[Table] t on c.TableId = t.Id
inner join dbo.[Project] p on t.ProjectId = p.Id
where 1=1
";
            #endregion

            #region sql 加上 projectId & tableIds 條件
            var hasCond = false;
            if (!_Str.IsEmpty(projectId))
            {
                sql += "and t.ProjectId=@_ProjectId ";
                args.Add("_ProjectId");
                args.Add(projectId);
                hasCond = true;
            }
            if (tableIds != null && tableIds.Count() > 0)
            {
                var cond = _Sql.SqlAddIn("TableId", tableIds, ref args);
                sql += "and t.Id in (" + cond + ")";
                hasCond = true;
            }
            if (!hasCond)
            {
                error = "ProjectId & TableIds are need.";
                goto lab_error;
            }
            #endregion

            #region 讀取db
            var cols = _Db.GetModels<ColumnDto>(sql, args);
            if (cols == null)
            {
                error = "No col rows found";
                goto lab_error;
            }

            //has name, cname
            var tables = cols
                .GroupBy(a => new { a.ProjectName, a.TableName, a.TableCname })
                .OrderBy(a => a.Key.TableName)
                .Select(a => new { a.Key.ProjectName, a.Key.TableName, a.Key.TableCname })
                .ToList();

            var tableLen = tables.Count;
            if (tableLen == 0)
            {
                error = "No table found";
                goto lab_error;
            }
            #endregion

            #region prepare stream
            var ms = new MemoryStream();
            var tplBytes = System.IO.File.ReadAllBytes(tplPath);
            ms.Write(tplBytes, 0, (int)tplBytes.Length);

            //see: _Word.cs DataIntoStream()
            //stream -> docx
            //using (var docx = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            using (var docx = WordprocessingDocument.Open(ms, true))
            {
                //call delegate if need
                var mainPart = docx.MainDocumentPart;

                //=== 2.do single row start ===
                //read template file
                var fileTpl = "";
                using (var sr = new StreamReader(mainPart.GetStream()))
                {
                    fileTpl = sr.ReadToEnd();
                }

                //set project name
                fileTpl = fileTpl.Replace("[ProjectName]", tables[0].ProjectName);

                //get word body start/end pos
                const string bodyStartTag = "<w:body>";
                const string bodyEndTag = "</w:body>";
                var bodyStartTagLen = bodyStartTag.Length;
                var bodyEndTagLen = bodyEndTag.Length;
                int bodyStart = 0, bodyEnd = 0;
                var bodyTpl = _Word.GetRangeStr(ref bodyStart, ref bodyEnd, fileTpl, "", "", bodyStartTag, bodyEndTag);

                //adjust bodyTpl, remove start/end tag
                bodyTpl = bodyTpl.Substring(bodyStartTagLen, bodyTpl.Length - bodyStartTagLen - bodyEndTagLen);
                //bodyStart += bodyStartTagLen;
                //bodyEnd -= bodyEndTagLen;

                //get rows template string
                const string midTag = "[!]";
                int rowStart = 0, rowEnd = 0;
                var rowTpl = _Word.GetRangeStr(ref rowStart, ref rowEnd, bodyTpl, midTag).Replace(midTag, "");
                if (rowTpl == "")
                {
                    error = "can not find rowTpl String.";
                    goto lab_error;
                }

                //columns loop
                //var oldTable = "";
                var rowLeft = bodyTpl.Substring(0, rowStart);
                var fileStr = "";
                //var tableLen = tables.Count();
                for (var i = 0; i < tableLen; i++)
                {
                    //new page: 
                    //do multiple rows first
                    var tableName = tables[i].TableName;
                    var tableCols = cols
                        .Where(a => a.TableName == tableName)
                        .OrderBy(a => a.Sort)
                        .ToList();
                    var rowStr = "";
                    foreach (var col in tableCols)
                    {
                        //[S] for Sort, make it shorter for fit word cell
                        rowStr += rowTpl
                            .Replace("[S]", col.Sort.ToString())
                            .Replace("[Name]", col.Name)
                            .Replace("[Cname]", col.Cname)
                            .Replace("[DataType]", col.DataType)
                            .Replace("[Nullable]", col.Nullable ? "Y" : "")
                            .Replace("[DefaultValue]", col.DefaultValue)
                            .Replace("[Note]", col.Note);
                    }

                    //fileStr += tplStr.Replace("[Table]", tableName + "(" + tables[i].TableCname + ")");
                    /*
                    */
                    //add into fileStr
                    fileStr += rowLeft.Replace("[Table]", tableName + "(" + tables[i].TableCname + ")") +
                        rowStr +
                        bodyTpl.Substring(rowEnd + 6);

                    //add page break if need
                    if (i < tableLen - 1)
                        fileStr += _Word.PageBreak;
                }

                fileStr = fileTpl.Substring(0, bodyStart) +
                    bodyStartTag +
                    fileStr +
                    bodyEndTag +
                    fileTpl.Substring(bodyEnd + bodyEndTagLen);
                //string to docx stream, must use FileMode.Create, or target file can not open !!
                using (var sw = new StreamWriter(mainPart.GetStream(FileMode.Create)))
                {
                    sw.Write(fileStr);
                    //sw.Write(tplStr);
                }

                //Debug.Assert(IsDocxValid(doc), "Invalid File!");
                //no save, but can debug !!
                //mainPart.Document.Save();
                //return;
            }
            #endregion

            //echo stream to file
            _WebWord.EchoStream(ms, "Table.docx");
            return;

            lab_error:
            _Log.Error("TableService.cs GenWord() failed: " + error);
            return;
        }

    }//class
}
