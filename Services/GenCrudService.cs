﻿using Base.Enums;
using Base.Services;
using DbAdm.Enums;
using DbAdm.Models;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DbAdm.Services
{
    //generate CRUD, use Handlebars.net
    public class GenCrudService
    {
        //constant
        const string ColSep = ", ";             //item column seperator
        const string CrudProg = "[prog]";     //key word inside file name
        const string PosGroup0 = "-abc99";      //impossible for initial

        //rows for curdId list
        private List<CrudDto> _cruds = null;        
        private List<CrudQitemDto> _qitems = null;
        private List<CrudRitemDto> _ritems = null;
        private List<CrudEtableDto> _etables = null;
        private List<CrudEitemDto> _eitems = null;

        //template folder
        private string _tplDir = _Fun.DirRoot + "_template/";
        //generated 6 files, 1(template),2(target folder),3(target file)
        private string[] _crudFiles = new string[] {
            "Controller.txt", "Controllers", "[prog]Controller.cs",
            "ReadService.txt", "Services", "[prog]Read.cs",
            "EditService.txt", "Services", "[prog]Edit.cs",
            "ReadView.txt", "Views/[prog]", "Read.cshtml",
            "EditView.txt", "Views/[prog]", "Edit.cshtml",
            "JS.txt", "wwwroot/js/view", "[prog].js",
        };
        private int _crudFileLen;

        //constructor
        public GenCrudService()
        {
            _crudFileLen = _crudFiles.Length;
        }

        //async for file io
        public async Task<bool> RunAsync(string crudIdList2)
        {
            //only alpha, num and ','
            if (!_Str.IsAlphaNum(crudIdList2, "GenCrudService Run()"))
            {
                _Log.Error("GenCrudService.cs Run() only accept alphabet and numeric: (" + crudIdList2 + ")");
                return false;
            }

            var crudIds = crudIdList2.Split(',');
            var crudIdList = _Str.ListAddQuote(crudIdList2);

            //1.get _cruds(Crud rows)
            var db = _Xp.GetDb();
            _cruds = (from c in db.Crud
                      join p in db.Project on c.ProjectId equals p.Id
                      //join t in db.Table on a.TableId equals t.Id
                      where crudIds.Contains(c.Id)
                      select new CrudDto()
                      {
                          Id = c.Id,
                          Project = p.Code,
                          ProjectPath = p.ProjectPath,
                          ProgName = c.ProgName,
                          ProgCode = c.ProgCode,
                          TableAs = c.TableAs,
                          LabelHori = c.LabelHori,
                          ReadSql = c.ReadSql,
                          HasCreate = c.HasCreate,
                          HasUpdate = c.HasUpdate,
                          HasDelete = c.HasDelete,
                          HasView = c.HasView,
                          HasExport = c.HasExport,
                          HasReset = c.HasReset,
                          AuthType0 = (c.AuthType == 0),
                          AuthType1 = (c.AuthType == 1),
                          AuthType2 = (c.AuthType == 2),
                      })
                      .ToList();

            _qitems = (from q in db.CrudQitem
                       join c in db.Column on q.ColumnId equals c.Id
                       where crudIds.Contains(q.CrudId)
                       orderby q.CrudId, q.Sort
                       select new CrudQitemDto()
                       {
                           CrudId = q.CrudId,
                           Fid = c.Code,
                           Name = c.Name,
                           DataType = c.DataType,
                           PosGroup = q.PosGroup,
                           LayoutCols = q.LayoutCols,
                           PlaceHolder = "",
                           IsFind2 = q.IsFind2,
                           Op = q.Op,
                           InputType = q.InputType,
                           InputData = q.InputData,
                       })
                       .ToList();

            //2.get _crudRitems(CrudRitem rows)
            _ritems = (from r in db.CrudRitem
                       where crudIds.Contains(r.CrudId)
                       orderby r.CrudId, r.Sort
                       select new CrudRitemDto()
                       {
                           CrudId = r.CrudId,
                           RitemType = r.RitemType,
                           ExtInfo = r.ExtInfo,
                           Name = r.Name,
                           Width = r.Width,
                           ColumnCode = r.ColumnCode,
                           //Column = c.Name,
                       })
                       .ToList();

            //3.get _crudEtable(CrudEtable rows)
            _etables = (from e in db.CrudEtable
                        join t in db.Table on e.TableId equals t.Id
                        where crudIds.Contains(e.CrudId)
                        select new CrudEtableDto()
                        {
                            Id = e.Id,
                            CrudId = e.CrudId,
                            TableCode = t.Code,
                            TableName = t.Name,
                            PkeyFid = e.Kid,
                            FkeyFid = e.MapFid,
                            HasCol4 = (e.Col4 == "1"),
                            HalfWidth = e.HalfWidth,
                            OrderBy = e.OrderBy,
                        })
                        .ToList();

            //4.get _crudEitem(CrudEitem rows)
            _eitems = (from e in db.CrudEitem
                       join t in db.CrudEtable on e.EtableId equals t.Id
                       join c in db.Column on e.ColumnId equals c.Id
                       where crudIds.Contains(t.CrudId)
                       select new CrudEitemDto()
                       {
                           EtableId = t.Id,
                           Fid = c.Code,
                           Name = c.Name,
                           DataType = c.DataType,
                           Required = e.Required,
                           HasCreate = e.HasCreate,
                           HasUpdate = e.HasUpdate,
                           CheckType = e.CheckType,
                           CheckData = e.CheckData,
                           InputType = e.InputType,
                           InputData = e.InputData,
                           PosGroup = e.PosGroup,
                           LayoutCols = e.LayoutCols,
                           PlaceHolder = e.PlaceHolder,
                           Sort = e.Sort,
                           Width = e.Width,
                       })
                       //.OrderBy(a => new { a.EtableId, a.Sort })  //.net core not support !!
                       .OrderBy(a => a.EtableId).ThenBy(a => a.Sort)
                       .ToList();

            db.Dispose();

            //loop產生檔案
            foreach (var crudId in crudIds)
            {
                if (!await GenByCrudIdAsync(crudId))
                    return false;
            }

            //case of ok
            return true;
        }

        /// <summary>
        /// generate crud files by crudId
        /// </summary>
        /// <param name="crudId"></param>
        /// <returns></returns>
        private async Task<bool> GenByCrudIdAsync(string crudId)
        {
            //get crud
            var error = "";
            var crud = _cruds.FirstOrDefault(a => a.Id == crudId);
            if (crud == null)
            {
                error = "no CrudId: " + crudId;
                goto lab_error;
            }

            //rows for one crudId
            var fitems = _qitems.Where(a => a.CrudId == crudId).ToList();
            var ritems = _ritems.Where(a => a.CrudId == crudId).ToList();
            var etables = _etables.Where(a => a.CrudId == crudId).ToList();

            #region set crud.RsItemStrs && IsGroup, IsGroupStart, IsGroupEnd
            var fitemLen = (fitems == null) ? 0 : fitems.Count;
            int i;
            if (fitemLen > 0)
            {
                var rSitemStrs = new List<String>();
                var posGroup = PosGroup0;   //initial value, get one impossible value for avoid repeat
                for (i=0; i<fitemLen; i++)
                {
                    //set rSitemStrs
                    var fitem = fitems[i];
                    rSitemStrs.Add(GetRServiceItemStr(fitem));

                    //set IsGroup, IsGroupStart, IsGroupEnd
                    if (!string.IsNullOrEmpty(fitem.PosGroup))
                    {
                        fitem.IsGroupStart = (fitem.PosGroup != posGroup);
                        fitem.IsGroup = fitem.IsGroupStart || (fitem.PosGroup == posGroup);
                        fitem.IsGroupEnd = !fitem.IsGroup ? false :
                            (i + 1 == fitemLen) ? true :
                            (fitems[i + 1].PosGroup != posGroup);

                        //for next loop
                        posGroup = fitem.PosGroup;
                    }

                    //fitem.RvStr = GetViewItemStr(fitem);  //set after XgFindTbar
                }
                crud.RsItemStrs = rSitemStrs;
                crud.HasFitemCols = fitems.Any(a => _Str.IsEmpty(a.LayoutCols));

                //set ReadSelectCols
                crud.ReadSelectCols = fitems
                    .Where(a => a.InputType == InputTypeEstr.Select)
                    .Select(a => a.InputData)
                    .Distinct()
                    .ToList();

                //set Fitems, F2items
                var f2Pos = fitems.FindIndex(a => a.IsFind2);
                var hasFind2 = (f2Pos > 0);    //must > 0
                crud.HasFindForm = true;
                crud.HasFind2Form = hasFind2;

                //split fitems to f2items
                if (hasFind2)
                {
                    var f2items = new List<CrudQitemDto>();
                    var f2Len = fitemLen - f2Pos;
                    f2items.AddRange(fitems.GetRange(f2Pos, f2Len));
                    fitems.RemoveRange(f2Pos, f2Len);
                    crud.F2items = f2items;
                    fitemLen -= f2Pos;    //adjust

                    //set RvStr
                    foreach (var fitem in f2items)
                        fitem.RvStr = GetViewItemStr(etables[0], fitem);
                }

                //adjust: fitems row 1 add toolbar buttons
                posGroup = fitems[0].PosGroup;
                //find group 1 last row for write toolbar
                int pos;
                if (string.IsNullOrEmpty(posGroup))
                {
                    pos = 0;
                    fitems[pos].IsGroup = true;
                    fitems[pos].IsGroupStart = true;
                    fitems[pos].IsGroupEnd = false;
                }
                else
                {
                    pos = fitems.FindIndex(a => a.PosGroup != posGroup);
                    pos = (pos < 0) ? fitemLen - 1 : pos - 1;
                    fitems[pos].IsGroup = true;
                    fitems[pos].IsGroupStart = false;
                    fitems[pos].IsGroupEnd = false;
                }
                //set RvStr first
                foreach (var fitem in fitems)
                    fitem.RvStr = GetViewItemStr(etables[0], fitem);

                fitems.Insert(pos + 1, new CrudQitemDto()
                {
                    //@await Component.InvokeAsync("XgFindTbar", new { isHorizontal = true, hasReset = true, hasFind2 = true })
                    RvStr = GetCompStart("XgFindTbar") +
                        GetCols(
                            ViewBool("isHorizontal", crud.LabelHori, false),
                            ViewBool("hasReset", crud.HasReset, true),
                            ViewBool("hasFind2", hasFind2, true)) +
                        GetCompEnd(),

                    IsGroupStart = false,
                    IsGroupEnd = true,
                    IsGroup = true,
                });
                crud.Fitems = fitems;
            }
            #endregion

            #region set crud.Ritems, crud.JsColDefStrs
            //set ritems(result items)
            var hasRitem = (ritems != null && ritems.Count > 0);
            if (hasRitem)
            {
                //array len are different, seperate to 2 arrays
                var jsStrs = new List<string>();  //part fields for js
                var ritemLen = ritems.Count;
                for (i = 0; i < ritemLen; i++)
                {
                    var ritem = ritems[i];
                    ritem.ViewStr = GetRViewHeadStr(ritem);

                    var jsStr = GetJsColDefStr(crud, ritem, i);
                    if (jsStr != "")
                        jsStrs.Add(jsStr);
                }

                /*
                //add crudFun if need
                if (crud.HasCreate || crud.HasUpdate || crud.HasDelete)
                {
                    var ritem = new CrudRitemDto()
                    {
                        RitemType = RitemTypeEstr.CrudFun,
                        Width = 100,
                        Name = "維護",
                    };
                    ritems.Add(new CrudRitemDto()
                    {
                        ViewStr = GetRViewHeadStr(ritem)
                    });

                    jsStrs.Add(GetJsColDefStr(crud, ritem, ritemLen));
                }
                */

                crud.Ritems = ritems;
                crud.JsColDefStrs = jsStrs;
            }
            #endregion

            //set EditSelectCols(ReadSelectCols already done)
            crud.EditSelectCols = _eitems
                .Where(a => a.InputType == InputTypeEstr.Select &&
                    !crud.ReadSelectCols.Contains(a.InputData))
                .Select(a => a.InputData)
                .Distinct()
                .ToList();

            #region set crud.MainTable, crud.ChildTables
            //set etable.Eitems
            var etableLen = etables.Count;
            for (i = 0; i < etableLen; i++)
            {
                var etable = etables[i];
                var eitems = _eitems.Where(a => a.EtableId == etable.Id).ToList();
                etable.Eitems = eitems;

                var posGroup = PosGroup0;   //set impossible init value
                var eitemLen = eitems.Count;
                var isTable = (i > 0);
                for (var j=0; j< eitemLen; j++)
                {
                    var eitem = eitems[j];
                    eitem.ServiceStr = GetEServiceItemStr(eitem);
                    eitem.ViewStr = GetViewItemStr(etable, eitem, isTable);

                    //add hide field string for modal input(textArea)
                    if (eitem.InputType == InputTypeEstr.Modal)
                        AddHideStr(etable, eitem);

                    //child table set table header
                    if (i > 0)
                        eitem.HeadStr = GetEViewHeadStr(eitem);

                    //set posGroup related
                    if (string.IsNullOrEmpty(eitem.PosGroup))
                        continue;

                    eitem.IsGroupStart = (eitem.PosGroup != posGroup);
                    eitem.IsGroup = eitem.IsGroupStart || (eitem.PosGroup == posGroup);
                    eitem.IsGroupEnd = !eitem.IsGroup ? false :
                        (j + 1 == eitemLen) ? true :
                        (eitems[j + 1].PosGroup != eitem.PosGroup);
                    posGroup = eitem.PosGroup;
                }
            }
            crud.MainTable = etables[0];

            //set ChildTables & ManyTables
            if (etableLen > 1)
            {
                //set ChildTables
                crud.ChildTables = new List<CrudEtableDto>();
                var childTables = crud.ChildTables;
                for (i = 1; i < etableLen; i++)
                {
                    //at end, add row function component(delete/deleteUpDown)
                    var table = etables[i];
                    table.SortFid = table.Eitems
                        .Where(a => a.InputType == InputTypeEstr.Sort)
                        .Select(a => a.Fid)
                        .FirstOrDefault();
                    table.Eitems.Add(new CrudEitemDto()
                    {
                        HeadStr = "<th></th>",
                        //@await Component.InvokeAsync("XgDeleteRow", "_me.mCol.onDeleteRow(this)")
                        ViewStr = string.IsNullOrEmpty(table.SortFid)
                            ? "<td width='60px' class='text-center'>@await Component.InvokeAsync(\"XgDeleteRow\", \"_me.m" + table.TableCode + ".onDeleteRow(this)\")</td>"
                            : "<td width='100px' class='text-center'>@await Component.InvokeAsync(\"XgDeleteUpDown\", new { mName = \"_me.m" + table.TableCode + "\" })</td>",
                    });
                    childTables.Add(table);
                }

                //ManyTables
                crud.ManyTables = string.Join(", ", crud.ChildTables
                    .Select(a => "_me.m" + a.TableCode)
                    .ToList());

                //set crud.HasFile
                crud.HasFile = etables.SelectMany(a => a.Eitems).Any(b => b.InputType == InputTypeEstr.File);
            }
            #endregion

            #region generate crud files
            var multiEdit = (etables.Count > 1);
            //var tplTail = multiEdit ? "2" : "1";
            var projectPath = _Str.AddAntiSlash(crud.ProjectPath);
            for (i = 0; i < _crudFileLen; i = i + 3)
            {
                //read template file to string
                //var tplFile = _tplDir + _crudFiles[i].Replace(CrudTplTail, tplTail);
                var tplFile = _tplDir + _crudFiles[i];
                var tplStr = await _File.ToStrAsync(tplFile);
                if (tplStr == null)
                {
                    _Log.Error("no template file: " + tplFile + "," + "??");
                    goto lab_error;
                }

                //mustache replace
                var mustache = Handlebars.Compile(tplStr);
                var result = HttpUtility.HtmlDecode(mustache(crud));

                //if file existed, return false
                var tableCode = crud.ProgCode;
                var toDir = projectPath + _Str.AddAntiSlash(_crudFiles[i + 1]).Replace(CrudProg, tableCode);
                var toFile = toDir + _crudFiles[i + 2].Replace(CrudProg, tableCode);
                //var toFile = _File.GetNextFileName(toDir + _crudFiles[i + 2].Replace(CrudTable, tableName), true);
                if (File.Exists(toFile))
                    File.Copy(toFile, _File.GetNextFileName(toFile, true));

                //create folder
                _File.MakeDir(toDir);

                //save file
                await _File.StrToFileAsync(result, toFile);
            }//for
            #endregion

            //case of ok
            return true;

        lab_error:
            _Log.Error("GenCrudService.cs GenByCrudIdAsync() error: " + error);
            return false;
        }

        #region get item string
        /// <summary>
        /// get js column define string (ritem)
        /// </summary>
        /// <returns></returns>
        private string GetJsColDefStr(CrudDto crud, CrudRitemDto ritem, int i)
        {
            var str = "";
            switch (ritem.RitemType)
            {
                case RitemTypeEstr.Normal:
                    return "";

                case RitemTypeEstr.CrudFun:   //escape {
                    var ext = ritem.ExtInfo;
                    //align column
                    str = string.Format(@"{{ targets: [{0}], render: function (data, type, full, meta) {{
                    return _crud.dtCrudFun(full.Id, full.Name, {1}, {2}, {3});
                }}}},", i, BoolToStr(crud.HasUpdate), BoolToStr(crud.HasDelete), BoolToStr(crud.HasView));
                    //", i, SubToBool(ext, 0), SubToBool(ext, 1), SubToBool(ext, 2));
                    break;

                case RitemTypeEstr.SetStatus:
                    str = string.Format(@"{{ targets: [{0}], render: function (data, type, full, meta) {{
                    return _crud.dtSetStatus(full.Id, data);
                }}}},", i);
                    break;

                case RitemTypeEstr.UserDefined:
                    str = string.Format(@"{{ targets: [{0}], render: function (data, type, full, meta) {{
                    //TODO: add your code
                    return '';
                }}}},", i);
                    break;

                    //default:
                    //    continue;
            }
            return str;
        }

        /// <summary>
        /// get ritem header string for view
        /// </summary>
        /// <returns></returns>
        private string GetRViewHeadStr(CrudRitemDto item)
        {
            return "<th" + (item.Width == 0 ? ">" : " width='" + item.Width + "px'>") +
                item.Name + 
                "</th>";
        }

        /// <summary>
        /// get edit service item string, skip empty item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetEServiceItemStr(CrudEitemDto item)
        {
            if (string.IsNullOrEmpty(item.Fid))
                return "";

            return "new EitemDto { " +
                KeyValue("Fid", item.Fid, true) +
                ServiceFid("Required", item.Required ? "true" : "") +
                ServiceFid("Create", !item.HasCreate ? "false" : "") +
                ServiceFid("Update", !item.HasUpdate ? "false" : "") +
                ServiceFid("CheckType", GetCheckTypeName(item.CheckType)) +
                ServiceFid("CheckData", item.CheckData, true) +
                " },";
        }

        /// <summary>
        /// get read service item string
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetRServiceItemStr(CrudQitemDto item)
        {
            return "new QitemDto { " +
                KeyValue("Fid", item.Fid, true) +
                ServiceFid("Op", GetQitemOpName(item.Op)) +
                " },";
        }

        /// <summary>
        /// add hidden field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="item"></param>
        private void AddHideStr(CrudEtableDto table, CrudEitem0Dto item)
        {
            if (table.HideViewStrs == null)
                table.HideViewStrs = new List<string>();
            table.HideViewStrs.Add(ViewHide(item.Fid));
        }

        /// <summary>
        /// get hide item string for edit view
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        private string ViewHide(string fid)
        {
            return GetCompStart("XiHide") +
                KeyValue("fid", fid, true) +
                GetCompEnd();
        }

        /// <summary>
        /// get view item(eitem/qitem) string
        /// </summary>
        /// <param name="table"></param>
        /// <param name="item"></param>
        /// <param name="isMany"></param>
        /// <returns></returns>
        private string GetViewItemStr(CrudEtableDto table, CrudEitem0Dto item, bool isMany = false)
        {
            var center = false;
            var name = isMany ? "" : item.Name;
            var str = "";
            switch (item.InputType)
            {
                case InputTypeEstr.Hide:
                case InputTypeEstr.Sort:
                    //no need consider in table or not
                    AddHideStr(table, item);
                    return "";

                case InputTypeEstr.Text:
                case InputTypeEstr.TextArea:
                case InputTypeEstr.Password:
                    var compType = (item.InputType == InputTypeEstr.TextArea) 
                        ? "XiTextArea" : "XiText";
                    str = GetCompStart(compType) + 
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            (item.InputType == InputTypeEstr.Password) ? ViewType("password") : "",
                            ViewMaxLen(item.DataType),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                case InputTypeEstr.Numeric:
                    str = GetCompStart("XiNum") +
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            //ViewMaxLen(item.DataType),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                case InputTypeEstr.CheckBox:
                    center = true;
                    str = GetCompStart("XiCheck") + 
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            //KeyValue("value", "1", true),
                            ViewInRow(item.IsGroup),
                            ViewLabel(item.InputData)) +
                        GetCompEnd();
                    break;

                case "D":   //date
                    str = GetCompStart("XiDate") +
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                case "DT":   //datetime
                    str = GetCompStart("XiDt") +
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                /* TODO: other edit item                  
                case "R":   //radio
                    break;
                */

                case InputTypeEstr.Select:
                    str = GetCompStart("XiSelect") + 
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            ViewSelectRows(item),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                case InputTypeEstr.File:
                    str = GetCompStart("XiFile") + 
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            ViewRequired(item.Required),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                case InputTypeEstr.Modal:
                    item.Width = 85;
                    center = true;
                    //item.Name for modal title
                    str = GetCompStart("XgOpenModal") +
                        GetCols(
                            ViewTitle(item.Name),
                            ViewFid(item.Fid),
                            ViewMaxLen(item.DataType),
                            ViewRequired(item.Required)) +
                        GetCompEnd();
                    break;

                case InputTypeEstr.ReadOnly:
                    str = GetCompStart("XiRead") +
                        GetCols(
                            ViewTitle(name),
                            ViewFid(item.Fid),
                            ViewInRow(item.IsGroup),
                            ViewLayout(isMany, item.LayoutCols)) +
                        GetCompEnd();
                    break;

                default:
                    str = "??";
                    break;
            }

            //add width & center for table
            if (isMany)
            {
                var attr = (item.Width > 0) ? " width='" + item.Width + "px'" : "";
                if (center)
                    attr += " class='text-center'";
                str = "<td" + attr + ">" + str + "</td>";
            }
            return str;
        }

        /// <summary>
        /// get component first string
        /// </summary>
        /// <param name="type">component type, ex: XiHide</param>
        /// <returns>part component string</returns>
        private string GetCompStart(string type)
        {
            //must escape '{'
            return $"@await Component.InvokeAsync(\"{type}\", new {{ ";
        }

        /// <summary>
        /// get view item end string
        /// </summary>
        /// <returns></returns>
        private string GetCompEnd()
        {
            return " })";
        }

        /// <summary>
        /// get edit view header(th) string
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetEViewHeadStr(CrudEitemDto item)
        {
            //skip some types
            if (item.InputType == InputTypeEstr.Hide || item.InputType == InputTypeEstr.Sort)
                return "";

            //@await Component.InvokeAsync("XgTh", new { title = "XXX", required = true })
            return (item.Required || !string.IsNullOrEmpty(item.PlaceHolder))
                ? GetCompStart("XgTh") +
                    GetCols(
                        ViewTitle(item.Name),
                        ViewPlaceHolder(item.PlaceHolder),
                        ViewRequired(item.Required)) +
                    GetCompEnd()
                : string.Format("<th{0}>{1}</th>", 
                    (string.IsNullOrEmpty(item.LayoutCols) || item.LayoutCols=="0" ? "" : " width='" + item.LayoutCols + "'"), 
                    item.Name);
        }
        #endregion

        #region get view(edit/read) column content
        //get view field id
        private string ViewTitle(string title)
        {
            return KeyValue("title", title, true);
        }
        private string ViewFid(string fid)
        {
            return KeyValue("fid", fid, true);
        }
        private string ViewType(string type)
        {
            return KeyValue("type", type, true);
        }
        /*
        //get view input data
        private string ViewInputData(CrudEitem0Dto item)
        {
            return KeyValue(item.InputData, true);//??
        }
        */
        private string ViewLabel(string label)
        {
            return string.IsNullOrEmpty(label)
                ? ""
                : KeyValue("label", label, true);
        }
        private string ViewRequired(bool required)
        {
            return required
                ? KeyValue("required", "true")
                : "";
        }
        private string ViewPlaceHolder(string value)
        {
            return string.IsNullOrEmpty(value)
                ? ""
                : KeyValue("placeHolder", value, true);
        }
        private string ViewInRow(bool isGroup)
        {
            return isGroup
                ? KeyValue("inRow", BoolToStr(isGroup))
                : "";
        }
        /// <summary>
        /// get bool key-value for view
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="value"></param>
        /// <param name="writeStatus">get only when status</param>
        /// <returns></returns>
        private string ViewBool(string fid, bool value, bool writeStatus = true)
        {
            return (value == writeStatus)
                ? KeyValue(fid, BoolToStr(value))
                : "";
        }

        private string ViewSelectRows(CrudEitem0Dto item)
        {
            var value = string.IsNullOrEmpty(item.InputData)
                ? "??" : item.InputData;
            return KeyValue("rows", "(List<IdStrDto>)ViewBag." + value);
        }
        //get view of column length
        private string ViewMaxLen(string dataType)
        {
            //get column length
            var lenStr = _Str.GetMid(dataType, "(", ")");
            var len = 0;
            if (lenStr.Length == 0)
                len = 10;
            else if (int.TryParse(lenStr, out int len2))
                len = len2;

            return (len == 0)
                ? ""
                : KeyValue("maxLen", len);
        }
        //get view LayoutCols, return "cols" if empty
        private string ViewLayout(bool isMany, string layoutCols)
        {
            /*
            var value = (string.IsNullOrEmpty(layoutCols))
                ? (isGroup ? "new int[]{0, 2, 3}" : "cols")
                : "new int[]{" + (isGroup ? "0," : "1,") + layoutCols + "}";
            return ViewColStr(value);
            */
            return isMany
                ? ""
                : KeyValue("cols", layoutCols, true);
        }
        //get view column string
        private string KeyValue(string fid, object value, bool quote = false)
        {
            if (_Str.IsEmpty(value))
                return "";

            var value2 = value.ToString();
            if (quote)
                value2 = "\"" + value2 + "\"";
            return fid + " = " + value2;
        }
        /*
        private string ViewColStr_old(object val0, bool quote = false)
        {
            var value = _Str.IsEmpty(val0) ? "" : val0.ToString();
            if (quote)
                value = "\"" + value + "\"";
            return value;
        }
        */
        #endregion

        /// <summary>
        /// boolean to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string BoolToStr(bool value)
        {
            return value ? "true" : "false";
        }

        /// <summary>
        /// get column parameters list
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetCols(params string[] args)
        {
            var data = "";
            for (var i = 0; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(args[i]))
                    data += args[i] + ColSep;
            }

            if (data != "")
                data = data.Substring(0, data.Length - ColSep.Length);
            return data;
        }

        /*
        // for html help(version of asp.net)
        private string GetCols_old(params string[] args)
        {
            var data = "";
            var skip = true;
            //get from tail
            var skips = new List<string>() { "", "\"\"", "0", "false", "cols" };
            for (int i = args.Length - 1; i >= 0; i--)
            {
                if (skip && skips.Contains(args[i]))
                    continue;

                skip = false;
                data = Sep + args[i] + data;
            }
            //if (data != "")
            //    data = data.Substring(0, data.Length - Sep.Length);
            return data;
        }
        */

        /*
        private int GetColLen(string dataType)
        {
            var num = _Str.GetMid(dataType, "(", ")");
            return (num.Length == 0) ? 10 : Convert.ToInt32(num);
        }
        */

        /// <summary>
        /// get service(edit/read) fid string
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="value"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        private string ServiceFid(string fid, string value, bool quote = false)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (quote)
                value = "\"" + value + "\"";
            return ColSep + fid + " = " + value;
        }

        #region get mapping name
        /// <summary>
        /// get query item operator name by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetQitemOpName(string type)
        {
            switch (type)
            {
                case ItemOpEstr.Equal:
                    return "";  //not show
                case ItemOpEstr.Like:
                    return "ItemOpEstr.Like";
                case ItemOpEstr.NotLike:
                    return "ItemOpEstr.NotLike";
                case ItemOpEstr.In:
                    return "ItemOpEstr.In";
                case ItemOpEstr.Like2:
                    return "ItemOpEstr.Like2";
                case ItemOpEstr.LikeList:
                    return "ItemOpEstr.LikeList";
                case ItemOpEstr.LikeCols:
                    return "ItemOpEstr.LikeCols";
                case ItemOpEstr.Like2Cols:
                    return "ItemOpEstr.Like2Cols";
                case ItemOpEstr.IsNull:
                    return "ItemOpEstr.IsNull";
                case ItemOpEstr.NotNull:
                    return "ItemOpEstr.NotNull";
                case ItemOpEstr.InRange:
                    return "ItemOpEstr.InRange";
                case ItemOpEstr.UserDefined:
                    return "ItemOpEstr.UserDefined";
                default:
                    return "??";
            }
        }

        /// <summary>
        /// get check type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetCheckTypeName(string type)
        {
            switch (type)
            {
                case CheckTypeEstr.None:
                    return "";
                case CheckTypeEstr.Email:
                    return "CheckTypeEstr.Email";
                case CheckTypeEstr.Url:
                    return "CheckTypeEstr.Url";
                case CheckTypeEstr.Min:
                    return "CheckTypeEstr.Min";
                case CheckTypeEstr.Max:
                    return "CheckTypeEstr.Max";
                case CheckTypeEstr.Range:
                    return "CheckTypeEstr.Range";
                default:
                    return "??";
            }
        }
        #endregion

        /// <summary>
        /// substring to boolean
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        /*
        private string SubStrToBool(string str, int pos)
        {
            return (_Str.IsEmpty(str)) ? "false" :
                (str.Length <= pos) ? "false" :
                (str.Substring(pos, 1) == "1") ? "true" :
                "false";
        }
        */

    }//class
}