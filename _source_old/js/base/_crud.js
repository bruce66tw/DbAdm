//crud static 公用類別
//只處理master table跟第一層child table
var _crud = {

    /**
     * constant
     */
    Rows: '_rows',
    Childs: '_childs',
    Deletes: '_deletes',

    Create: 'C',
    Update: 'U',
    View: 'V',

    /**
     * save middle variables
     */
    temp: {},

    //=== jQuery datatables start ===
    /**
     * datatable layout
     */
    dtDom: '<"toolbar">t<li>p',

    /**
     * datatable column config
     */ 
    dtColConfig: {
        className: 'xg-center',
        orderable: false,
        targets: '_all',
    },

    /**
     * dt欄位: checkbox欄位 for 選取多筆
     * param {string} value checkbox value
     * param {bool} editable (optional true) 是否可選取
     * param {string} fid (optional '_check1') data-id value
     */
    dtCheck0: function (value, editable, fid) {
        if (editable === undefined)
            editable = true;
        fid = fid || _iCheck.check0Id;
        return _iCheck.render2(0, fid, value, false, '', editable);
    },

    //??
    dtRadio1: function (value, editable) {
        if (editable === undefined)
            editable = true;
        return _iRadio.render(_iCheck.check0Id, '', false, value, editable);
    },

    /**
     * dt欄位: set Status(checkbox)
     * value {string} checkbox value, will translate to bool
     * onClickFn {string} 自定義function, 如果空白則使用default(_crud.onSetStatus)
     */ 
    dtSetStatus: function (key, value, onClickFn) {
        //todo: 
        return '';

        //debugger;
        var checked = _str.toBool(value);
        if (_str.isEmpty(onClickFn)) {
            onClickFn = _str.format("_crud.onSetStatus(this,\'{0}\')", key);
        }
        return _iCheck.render2(0, '', 1, checked, '', true, '', "onclick=" + onClickFn);
    },

    /**
     * crud funs: 修改,刪除,檢視
     * key {} row key
     * rowName {} 刪除時提示框顯示的欄位值, 表示要刪除的資料列
     * hasUpdate {bool} update 功能
     * hasDelete {bool} delete 功能
     * hasView {bool} view 功能
     */
    dtCrudFun: function (key, rowName, hasUpdate, hasDelete, hasView) {
        var funs = '';
        if (hasUpdate)
            funs += _str.format('<a onclick="_crud.onUpdate(\'{0}\');"><i class="icon-pen" title="{1}"></i></a>', key, RB.TipUpdate);
        if (hasDelete)
            funs += _str.format('<a onclick="_crud.onDelete(\'{0}\',\'{1}\');"><i class="icon-times" title="{2}"></i></a>', key, rowName, RB.TipDelete);
        if (hasView)
            funs += _str.format('<a onclick="_crud.onView(\'{0}\');"><i class="icon-eye" title="{1}"></i></a>', key, RB.TipView);

        return funs;
    },
    //=== jQuery datatables end ===    

    /**
     * initial, 傳入簡化的edits[]
     * dtConfig {Object} datatables config
     * edits {Array} 維護畫面設定{object},
     *   1.null: 表示單一table, 固定捉取formEdit
     *   2.多個edit object, 如果第一個陣列為null, 則使用new EditOne()
     */
    init: function (dtConfig, edits) {
        //set _me.edits[]
        var Childs = _crud.Childs;  //constant
        var edit = null;  //具有結構的edit設定
        if (edits == null) {
            edit = new EditOne();
            //_me.hasChild = false;
        } else {
            edit = (edits[0] === null) ? new EditOne() : edits[0];
            //_me.hasChild = edits.length > 1;
            if (edits.length > 1) {
                edit[Childs] = [];
                //var childs = _me.edits._childs;
                for (var i = 1; i < edits.length; i++)
                    edit[Childs][i - 1] = edits[i];
            }
        }

        //set variables
        //_me.row = null; //edit時從後端傳回
        _me.nowFun = '';    //now fun of edit form: C(create),U(update),V(view)
        _me.divRead = $('#divRead');
        //_me.divReadTool = $('#divReadTool');
        _me.divEdit = $('#divEdit');
        _me.formFind = $('#formFind');
        _me.formFind2 = $('#formFind2');
        if (_me.formFind2.length === 0)
            _me.formFind2 = null;
        //_me.formEdit = $('#formEdit');

        _me.edit = edit;
        _me.hasChild = (_fun.notNull(_me.edit[Childs]) && _me.edit[Childs].length > 0);
        //_me.editLen = _me.edits.length;

        //initial forms(recursive)
        _crud.initForm(_me.edit);

        //for xgOpenModal
        _me.modal = null;

        //table div id固定為 table1, 後端固定呼叫 GetPage
        _me.dt = new Datatable('#table1', 'GetPage', dtConfig);
        _prog.init();   //prog path
    },

    //initial forms(recursive)
    initForm: function (edit) {
        if (edit.form == null)
            return;

        _iDate.init('', edit.form);  //init 日期欄位(所有欄位)
        _valid.init(edit.form);
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++)
            _crud.initForm(_crud.getEditChild(edit, i));
    },

    /**
     * get master edit form
     */
    getForm0: function () {
        return _me.edit.form;
    },

    //=== event start ===
    /**
     * 展開查詢畫面的額外欄位
     */ 
    onFind2: function () {
        //$('.xg-find-form').slideToggle();
        var find2 = _me.formFind2;
        if (find2 == null)
            return;
        else if (_obj.isShow(find2))
            _form.hideShow([find2]);
        else
            _form.hideShow(null, [find2]);

    },

    /**
     * get Find condition
     */
    getFindCond: function () {
        var row = _form.toJson(_me.formFind);
        var find2 = _me.formFind2;
        if (find2 !== null && _obj.isShow(find2))
            _json.copy(_form.toJson(find2), row);
        return row;
    },

    /**
     * click Find(查詢資料)
     */
    onFind: function () {
        var cond = _crud.getFindCond();
        _me.dt.find(cond);
    },

    /**
     * click Export(匯出excel)
     */
    onExport: function () {
        var cond = _crud.getFindCond();
        window.location = 'Export?cond=' + _json.toStr(cond);
    },

    /**
     * 回到列表頁
     */
    onToRead: function () {
        _crud.toReadMode();
    },

    /**
     * on click Create(新增資料)
     */
    onCreate: function () {
        _me.key = '';
        _crud.setEditStatus(_crud.Create);
        _prog.setPathCreate();
        _crud.resetForm(_me.edit);
        _form.swap(_me.divEdit);
    },

    /**
     * on click Update(修改資料)
     * key {string} row key
     */
    onUpdate: function (key) {
        _crud.getJsonAndSetMode(key, _crud.Update);
    },

    onView: function (key) {
        _crud.getJsonAndSetMode(key, _crud.View);
    },

    getJsonAndSetMode: function (key, fun) {
        if (_str.isEmpty(key)) {
            _log.error('error: key is empty !');
            return;
        }

        //_crud.toUpdateMode(key);
        _ajax.getJson('GetJson', { key: key }, function (data) {
            //_me.nowRow = data;
            _crud.loadJson(data);

            //to edit(U/V) mode
            _me.key = key;
            _prog.setPathUpdate();  //todo
            _crud.setEditStatus(fun);
            _form.swap(_me.divEdit);

            /*
            //trigger custom function if any
            if ($.isFunction(_me._ueOnUpdate)){
                _me._ueOnUpdate(data);
            }
            */
        });
    },

    /**
     * to edit(U/V) mode
     */
    /*
    setModeAndShow: function (mode, key) {
        _me.key = key;
        _crud.setEditStatus(mode);
        //_me.isNew = false;
        //_me.divReadTool.hide();
        _prog.setPathUpdate();  //todo
        _form.swap(_me.divEdit);
    },
    */

    //set edit form status
    //fun: C,U,V
    setEditStatus: function (fun) {
        var isView = (fun == _crud.View);
        var run = (_me.nowFun == _crud.View && !isView) ? true :
            (_me.nowFun != _crud.View && isView) ? true :
                false;

        //set variables
        _me.nowFun = fun;

        if (run) {
            var div = _me.divEdit;
            div.find('input, textarea, select, button').prop('disabled', isView);
            if (isView)
                div.find('#btnToRead').prop('disabled', false);
            //div.find('input', '').prop('disabled', isView); //for selected
        }
    },

    /**
     * click setStatus, 固定呼叫後端 SetStatus action
     * me {string} checkbox element
     * key {string} row key
     */
    onSetStatus: function (me, key) {
        var status = _iCheck.checkedO($(me));
        _ajax.getStr('SetStatus', { key: key, status: status }, function (msg) {
            _tool.alert(RB.UpdateOk);
        });
    },

    /**
     * load row(include childs) into UI
     * will call ufAfterLoadJson() 
     */
    loadJson: function (json) {
        //load master(single) row
        var edit = _me.edit;
        edit.loadRow(json);

        //load childs rows(只需載入第一層)
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            var edit2 = _crud.getEditChild(edit, i);
            //_crud.loadChildJson(edit2, _crud.getChildJsonByUp(rowJson, i));
            edit2.loadJson(_crud.getChildJson(json, i));
        }

        //call ufAfterLoadJson() if existed
        if (_fun.notNull(edit.ufAfterLoadJson))
            edit.ufAfterLoadJson(json);
    },

    /**
     * load childs rows into UI(recursive)
     * will call ufLoadJson()
     */
    /*
    loadChild: function (edit, rows) {
        //check rows
        //var rows = _crud.getJsonRows(rowJson);  //??
        if (rows == null)
            return;

        //load this
        edit.loadRows(rows);

        //load childs
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            var edit2 = _crud.getEditChild(edit, i);
            _crud.loadChild(edit2, _crud.getChildJsonByUp(rowJson, i));
        }
    },
    */

    /**
     * get updated data for save(has _childs, _deletes) for EditOne.js
     */ 
    getUpdJson: function (formData) {
        //load master(single) row
        var edit = _me.edit;
        var row = edit.getUpdRow();
        var key = edit.getKey();

        //file
        var levelStr = '0'; //string
        if (edit.hasFile)
            edit.dataAddFiles(levelStr, formData); //加入上傳檔案(多個)

        //load child(multiple) rows
        var hasChild = false;
        var childs = [];
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            var edit2 = _crud.getEditChild(edit, i);
            var childJson = _crud.getChildUpdJson(key, levelStr + i, formData, edit2);
            if (childJson == null)
                continue;

            //has child rows
            hasChild = true;
            childs[i] = childJson; 
        }

        var data = {};
        var hasData = false;
        if (row != null) {
            hasData = true;
            data[_crud.Rows] = [row];
        }
        if (hasChild) {
            hasData = true;
            data[_crud.Childs] = childs;
        }
        if (hasData) {
            _crud._removeNull(0, data);
            return data;
        } else {
            return null;
        }
    },

    //get updated json(has _rows, _childs, _deletes) for EditMany.js
    //只讀取第一層child資料, 不是 recursive
    getChildUpdJson: function (upKey, levelStr, formData, edit) {

        //get this
        //var rows = edit.getUpdRows(upKey);      //get this rows        
        //var deletes = edit.getDeletedRows();    //get _deletes

        var json = edit.getUpdJson(upKey);

        //file ??
        if (edit.hasFile)
            edit.dataAddFiles(levelStr, formData); //加入上傳檔案(多個)

        return json;
        /* skip recursive
        //get childs
        var hasChild = false;
        var childs = [];
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            var edit2 = _crud.getEditChild(edit, i);
            var json2 = _crud.getChildUpdJson(levelStr, formData, edit2);
            if (json2 == null)
                continue;

            //has child rows
            hasChild = true;
            childs[i] = json2;
        }
        */

        /*
        var json = {};
        if (rows != null && rows.length > 0)
            json[_crud.Rows] = rows;
        if (deletes != null && deletes.length > 0)
            json[_crud.Deletes] = deletes;
        //if (hasChild)
        //    row._childs = childs;

        return _json.isEmpty(json) ? null : json;
        */
    },

    //傳入edit
    getEditChildLen: function (edit) {
        var fid = _crud.Childs;
        return (edit[fid] == null) ? 0 : edit[fid].length; 
    },
    //傳入edit
    getEditChild: function (edit, childIdx) {
        return edit[_crud.Childs][childIdx];
    },

    //get json rows
    getJsonRows: function (json) {
        return (json == null || json[_crud.Rows] == null)
            ? null
            : json[_crud.Rows];
    },

    //get child json
    getChildJson: function (upJson, childIdx) {
        var fid = _crud.Childs;
        return (upJson[fid] == null || upJson[fid].length <= childIdx)
            ? null
            : upJson[fid][childIdx];
    },

    //get child json rows
    getChildRows: function (upJson, childIdx) {
        var child = _crud.getChildJson(upJson, childIdx);
        return _crud.getJsonRows(child);
    },

    /**
     * 設定 child rows
     * return child object
     */ 
    setChildRows: function (upRow, childIdx, rows) {
        var fid = _crud.Childs;
        if (upRow == null)
            upRow = {};
        if (upRow[fid] == null)
            upRow[fid] = [];
        if (upRow[fid].length <= childIdx)
            upRow[fid][childIdx] = {};
        var child = upRow[fid][childIdx];
        child[_crud.Rows] = rows;
        return child;
    },

    /**
     * forms validate check
     * return {bool}
     */ 
    validAll: function () {
        var edit = _me.edit;
        if (!edit.form.valid())
            return false;

        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            var edit2 = _crud.getEditChild(edit, i);
            if (!edit2.valid())
                return false;
        }

        //case of ok
        return true;
    },

    /**
     * (recursive) childs validate check
     * return {bool}
     */
    /*
    validChild: function (edit) {
        //valid this
        if (!edit.valid())
            return false;

        //valid childs
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            if (!_crud.validChild(_crud.getEditChild(edit, i)))
                return false;
        }

        //case of ok
        return true;
    },
    */

    /**
     * get saving row, 只讀取master table跟第一層child table
     * formData {FormData} (optional) 有上傳檔案時必須在外面宣告此變數, 再傳入
     * return {json}
     */
    /*
    getSaveJson: function (formData) {
        //var isNew = (_str.isEmpty(_me.key));
        var edit = _me.edit;
        var isNew = edit.isNewRow();
        var row = isNew ? edit.getRow() : edit.getUpdRow();   //saving row
        //var deletes = [];
        //var formData = new FormData();  //for upload files if need   
        //todo: _me.editLen
        var childs = [];
        var childLen = _crud.getEditChildLen(edit);
        var hasChild = false;
        for (var i = 0; i < childLen; i++) {
            //updated rows & files
            var edit2 = _crud.getEditChild(edit, i);
            //todo: file
            if (edit2.hasFile)
                edit2.dataAddFiles(i, formData); //加入上傳檔案(多個)

            var child = {
                _rows: edit2.getUpdRows(),
                _deletes: edit2.getDeletedRows(),
            };
            if (child._rows != null || child._deletes != null) {
                hasChild = true;
                childs[i] = child;
            }
        }
        if (hasChild) {
            if (row == null)
                row = {};
            row._childs = childs;
        }
        return row;
    },
    */

    /**
     * on click save, 有上傳檔案時, 後端參數名稱固定為T(n)+FieldName
     * 傳送到後端的資料包含以下欄位:  
     *   key, row(包含_childs, _deletes, _fileNo), files
     */
    onSave: function () {
        //check input
        if (!_crud.validAll()) {
            _tool.alert(RB.InputWrong);
            return;
        }

        //call ufWhenSave if need
        if (_fun.notNull(_me.edit.ufWhenSave)) {
            var error = _me.edit.ufWhenSave();
            if (error != '') {
                _tool.msg(error);
                return;
            }
        }

        //debugger;
        //get saving row
        var formData = new FormData();  //for upload files if need
        var row = _crud.getUpdJson(formData);

        //temp add
        //return;

        //save, 固定呼叫 Save action
        var data = null;
        if (_crud.hasFile) {
            //has files
            data = formData;
            data.append('json', _json.toStr(row));

            _ajax.getJsonByFormData('Save', data, function (result) {
                _crud.afterSave(result);
            });
        } else {
            //no files
            data = { json: _json.toStr(row) };
            _ajax.getJson('Save', data, function (result) {
                _crud.afterSave(result);
            });
        }
    },

    //recursive remove null
    //level: for debug
    _removeNull: function (level, obj) {
        //debugger;
        $.each(obj, function (key, value) {
            if (value === null) {
                //null才需要清除, 空白不必 !!
                delete obj[key];
            } else if (_json.isKeyValue(value)) {
                _crud._removeNull(level+1, value);
            } else if ($.isArray(value)) {
                $.each(value, function (k, v) {
                    _crud._removeNull(level + 1, v);

                    if (_json.isEmpty(v))
                        v = null;
                });
                var isEmpty = true;
                var len = value.length;
                //從陣列後面開始處理
                for (var i=len - 1; i>=0; i--) {
                    if (!_json.isEmpty(value[i])) {
                        isEmpty = false;
                    } else if (isEmpty) {
                        //刪除陣列元素
                        delete value[i];
                    } else {
                        value[i] = null;
                    }
                }
                if (isEmpty)
                    delete obj[key];
            /*
            } else {
                if (key.substr(0, 2) === '__')
                    delete obj[key];
            */
            }
        });
    },

    /**
     * 展開查詢畫面的額外欄位
     * data: ResultDto
     */
    afterSave: function (data) {
        debugger;
        //call ufWhenSave if need
        if (_fun.notNull(_me.edit.ufAfterSave))
            _me.edit.ufAfterSave();

        //save no rows
        if (data.Value === '0') {
            _tool.msg(RB.SaveNone);
            return;
        }

        //case of ok
        var start = _me.dt.dt.page.info().start;
        _tool.alert(RB.SaveOk + '(' + data.Value + ')');
        _me.dt.reload();
        _crud.toReadMode();
    },

    /**
     * todo
     * click check all, 選取/取消 box內的所有checkbox
     * me {string} row key
     * box {string} row key
     * dataFid {string} row key
     */
    onCheckAll: function (me, box, dataFid) {
        _iCheck.setF('[data-id=' + dataFid + ']:not(:disabled)', _iCheck.checkedO($(me)), box);
    },

    /**
     * click Delete, 刪除一筆資料, 後端固定呼叫 Delete()
     * key {string} row key
     * rowName {string} 資料列名稱
     */
    onDelete: function (key, rowName) {
        _crud.temp.data = { key: key };
        _tool.ans(RB.SureDeleteRow + ' (' + rowName + ')', function () {
            _ajax.getStr('Delete', { key: key }, function (msg) {
                _tool.alert(RB.DeleteOk);
                _me.dt.reload();
            });
        });
    },

    /**
     * todo
     * 刪除選取的多筆資料, 後端固定呼叫 DeleteByKeys()
     * box {string} row key
     * dataFid {string} row key
     */
    onDeleteRows: function (box, dataFid) {
        //get selected keys
        var keys = _iCheck.getCheckedValues(box, dataFid);
        if (keys.length === 0) {
            _tool.msg(RB.PlsSelectDeleted);
            return;
        }

        //刪除多筆資料, 後端固定呼叫 DeleteByKeys()
        _crud.temp.data = { keys: keys };
        _tool.ans(RB.SureDeleteSelected, function () {
            _ajax.getStr('DeleteByKeys', _crud.temp.data, function (msg) {
                _tool.alert(RB.DeleteOk);
                _me.dt.reload();
            });
        });
    },

    /**
     * table onclick openModal button(link)
     * param btn {button} 
     * param title {string} modal title
     * param col {string} input field name
     * param required {boolean}
     * param maxLen {int} 
     */ 
    onOpenModal: function (btn, title, col, required, maxLen) {
        //get tr object
        var tr = $(btn).closest('tr');
        var value = _iText.get(col, tr);
        _tool.showArea(title, value, function (result) {
            _iText.set(col, result, tr);
        });
    },
    //=== event end ===

    /**
     * ??
     * key array to string(加上table & row分隔符號, two dimension)
     * keys {string[]} key list
     * return {string} 字串加上table & row分隔符號
     */
    _keysToStr: function (keys) {
        var strs = [];
        for (var i = 0; i < keys.length; i++) {
            strs[i] = (keys[i].length === 0)
                ? ''
                : keys[i].join(_fun.RowSep);
        }
        return strs.join(_fun.TableSep);
    },
  

    //=== set mode start ===
    //reset form(recursive)
    resetForm: function (edit) {
        var childLen = _crud.getEditChildLen(edit);
        for (var i = 0; i < childLen; i++) {
            _crud.resetForm(_crud.getEditChild(edit, i));
        }
    },

    /**
     * 回到新增模式
     */
    /*
    toCreateMode: function () {
        //_me.isNew = true;
        _me.key = '';
        _crud.setEditStatus(_crud.Create);
        //_me.divReadTool.hide();
        _prog.setPathCreate();
        _crud.resetForm(_me.edit);
        _form.swap(_me.divEdit);
    },
    */

    /**
     * 回到修改模式
     * key {string} row key
     */
    /*
    toUpdateMode: function (key) {
        _crud.setModeAndShow(_crud.Update, key);
    },
    */

    /**
     * to view mode
     */
    /*
    toViewMode: function (key) {
        _crud.setModeAndShow(_crud.View, key);
    },
    */

    /**
     * 回到列表模式
     */
    toReadMode: function () {
        //_me.divReadTool.show();
        _prog.resetPath();
        _form.swap(_me.divRead);
    },
    //=== set mode end ===    

};//class