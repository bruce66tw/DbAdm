//多筆維護
//處理沒有 child 的資料
//called by _crud.js
/*
 一個 crud json 包含3個欄位: _rows, _deletes, _childs
   _rows: 多筆資料, 包含資料異動和 "檔案上傳"
   _deletes[]: 要刪除的Id array 字串(後端才能decode!!), 只有一個 id欄位(必須使用json格式才能傳到後端 !!)
   _childs  
 處理編輯畫面的多筆資料, 
   1.產生的資料(變數)包含3個欄位:
     form: form container
     rows[]: 表示要異動的資料 array, 欄位為欄位名稱, 其中 :
       //_new : 1/0
       //_key : 主key值, from data-key, for刪除only
       _fileNo : 欄位對應到要上傳的檔案序號(>=0), -1表示無檔案, -2表示要刪除檔案
 注意:
   //1.tr要放一個checkbox欄位 for 刪除資料
   //2.tr最多只能有一個上傳檔案欄位(也可以沒有)
   3.系統自動填入tr 2個屬性 :
     //(a).data-new: 1/0
     //(b).data-key: 單欄位主key
   4.異動欄位設定屬性 : name='xxx', xxx為欄位名稱
   //5.刪除按鈕固定呼叫 : this.onClickDeleteRows(_me.divXXX, _me.XXX)
   ??5.刪除按鈕固定呼叫 : this.onClickDeleteRows(_me.multiXXX)
   ??6.檔案欄位固定內容 : <input type='file' onchange='this.onChangeFile(this)'>
   ??7.在後端要自行處理上傳檔名的問題
   ??8.刪除多table多筆資料時, 分隔符號為: table(:), row(;), col(,), 後端必須同時配合!!
   ??9.多筆資料的上傳檔案暫時呼叫 _xp.tdFile(url)

 * 注意:
 *   保留的自訂函數:
 *     //void ufAfterLoadJson(rowJson)
 *     //bool ufWhenSave():
 *     //void ufAfterSave(): 在 _crud.js呼叫

   //如果有child, 則新增時要設定 id=new index
   儲存前要設定 data fkeyFid(base 0, for 新增)
 */

/**
 * initial
 * kid {string} key field id(single key)
 * formId {string} (optional) form row container id
 *   如果有值, 則系統會自動載入UI & prepare save rows
 *   其中, rows的container tag 固定捉 tbody
 * tplRowId {string} row template id, 不可空白
 *   可以產生 instance varibles for 自定函數 :
 *     fidTypes & 呼叫 method
 * tplIsMany {bool} (default true) template 是否為多筆
 *   單筆/多筆的取值方式不同
 * //mapFid {string} nested child 必須傳入這個值
 * //extInfo {json} (optional) 額外資訊, 包含以下欄位:
 *   fnGetUpdRows {function} getUpdRows()
 *   extFids {string[]} (optional) 額外要更新的欄位清單, extFids fields id array for send to server side when update
 */
function EditMany(kid, formId, tplRowId, sortFid) {

    /*
    if (tplIsMany === undefined)
        tplIsMany = true;
    this.tplIsMany = tplIsMany;
    */

    //分隔符號, 前後端必須一致
    //tableSep: ':',  //table 分隔符號
    //rowSep: ';',    //row 分隔符號
    //colSep: ',',    //column 分隔符號
    /*
     * rows
     * deletes
     * tplRowId
     * 
     */
    //debugger;
    //=== constant start ===
    //this.DataKey = '_key';      //row key for multi rows only(for delete function) !!
    //this.DataIsNew = '_new';    //1(created), 0(updated)

    //新增row時, 同時設定以下2個row box 欄位, save時, 會傳入後端
    //1.row index, 
    //this.DataIndex = '_index';
    //2.對應上層資料的key值, 如上層為新增, 則設定為 DataIndex的值(前面加負號, 做為區別)
    //this.DataMapFid = '_mapfid';

    this.init = function () {
        //上層為修改, 這一層為新增時, 後端會用到這個欄位
        //getUpdRows()設定
        this.DataFkeyFid = '_fkeyfid';

        //this.DataOld = '_old';      //    //舊資料存在 data 屬性, 內容必須與 _editMany.DataOld 相同
        //this.dataFun = '_fun';     //??data-fun
        //this.DataKey = 'key';     //data-key
        //=== constant end ===

        //=== property start ===
        //debugger;
        /*
        extInfo = extInfo || {};
        var extFids = extInfo.extFids;
        this.extInfo = extInfo;
        this.extFids = extFids;
        this.extFidLen = (extFids === undefined) ? 0 : extFids.length;
        */

        //this._childs = childs;
        this.kid = kid;
        this.isMulti = true;
        //this.fidTypes = null;       //name 欄位id/type pair string array
        this.fileIds = [];      //上傳檔案欄位id array

        var rowObj = $('#' + tplRowId);
        this.tplRow = rowObj.html();
        this.fidTypes = _edit.getFidTypes($(this.tplRow));
        this.fidTypeLen = this.fidTypes.length;

        //var fileIds = this.fileIds;     //在each裡面不可使用外面的this
        rowObj.find(':file').each(function (index) {
            //var obj = $(this);
            this.fileIds[index] = $(this).data('id');    //??
        });
        this.hasFile = this.fileIds.length > 0; //是否有檔案欄位

        this.hasForm = !_str.isEmpty(formId);
        if (this.hasForm) {
            this.form = $('#' + formId);     //multiple rows container object
            this.rowsBox = this.form.find('tbody'); //rows container固定捉取tbody        
        }

        this.sortFid = sortFid;
        //this.hasSort = !_str.isEmpty(sortFid);

        //是否有 form, 
        //如果沒有, 則必須自行處理 row render & getUpdRows()
        //this.hasFormAndTpl = (this.hasForm && this.hasTpl);
        //this.ftypes = null;     //field type, 對應 fidTypes


        //this.oldRows = [];      //current rows
        //this.oldRows = [];    //original rows
        this.deletedRows = [];  //deleted key array
        this.newIndex = 0;      //new row serial no
        //=== property end ===
    };

    /*
    this.getFidTypes = function (box) {
        var fidTypes = [];
        box.find('[data-id]').each(function (i, item) {
            var obj = $(item);
            var j = i * 2;
            fidTypes[j] = obj.data('id');
            fidTypes[j + 1] = _input.getType(obj);
        });
        return fidTypes;
    },
    */

    /**
     * is a new row or not
     * row {json} 
     * return {bool}
     */
    this.isNewRow = function (row) {
        //return _str.isEmpty(row[this.kid]);
        return this.isNewKey(row[this.kid]);
    };

    /**
     * reset edit form
     */
    this.reset = function () {
        //this.loadRows();
        //if (!this.hasForm)
        //    return;

        //rowsBox.empty();   //empty rows ui first
        this.newIndex = 0;
        this.deletedRows = [];
    };

    /**
     * load this json rows into UI
     * param {jarray} rows
     */
    this.loadJson = function (json) {
        //reset first
        this.reset();

        if (json == null || json[_crud.Rows] == null)
            return;

        if (this.hasForm)
            this.loadRows(this.rowsBox, json[_crud.Rows]);
        else
            this.ufLoadJson(json);
    };

    /**
     * load this json rows into UI
     * param {jarray} rows
     */
    /*
    this.loadRows = function (rows) {
        //reset first
        this.reset();

        if (this.hasForm)
            this.loadRows(this.rowsBox, rows);
        else
            this.ufLoadJson(rows);
    };
    */

    //load row by row box(container)
    this.loadRow = function (box, row, index) {
        var form = $(Mustache.render(this.tplRow, { Index: index }));
        _form.loadRow(form, row);   //使用 name

        //set old value for each field
        //var fidLen = fidTypes.length;
        for (var i = 0; i < this.fidTypeLen; i = i + 2) {
            fid = this.fidTypes[i];
            var obj = _obj.get(fid, form);
            obj.data(_edit.DataOld, row[fid]);
        }

        box.append(form);
    };

    //load rows by rows box(container)
    this.loadRows = function (rowsBox, rows) {
        //reset first
        rowsBox.empty();

        //var rows = json._rows;
        var rowLen = (rows == null) ? 0 : rows.length;
        if (rowLen === 0)
            return;

        //render this rows
        //var fidTypeLen = fidTypes.length;
        for (var i = 0; i < rowLen; i++) {
            var row = rows[i];
            var obj = $(Mustache.render(this.tplRow, row));
            //obj.data(this.DataIndex, i);    //set row index

            //set old value for each field
            for (var j = 0; j < this.fidTypeLen; j = j + 2) {
                fid = this.fidTypes[j];
                var obj2 = _obj.get(fid, obj);
                _edit.setOld(obj2, row[fid]);
            }

            _form.loadRow(obj, row);
            /*
            //設定 checkbox, radio status(後端無法設定) !!
            //要加入 checkbox 欄位, 只會讀取有name的欄位值
            obj.find(':checkbox').each(function () {
                var item = $(this);
                var id = item.data('id');
                if (id !== undefined && id.indexOf('-') < 0)
                    _iCheck.setO(item, row[id]);
            });

            //要加入 radio 欄位, 只會讀取有name的欄位值
            obj.find(':radio').each(function () {
                var item = $(this);
                var id = item.data('id');
                if (id !== undefined)
                    _iRadio.setO(item, row[id]);
            });
            */

            obj.appendTo(rowsBox);
        }        
    };

    this.valid = function () {
        return (this.hasForm)
            ? this.form.valid()
            : this.ufValid();
    };

    /*
    //是否有檔案欄位
    this.hasFile = function () {
        return this._hasFile;
    };
    */

    /*
    //get row index for new row, 取負數, 避免跟現存資料重覆
    this.getNewId = function () {
        this.newIndex++;
        return this.newIndex;
    };

    //get row by key
    this.getOldRow = function (key) {
        var rows = this.oldRows;
        for (var i = 0; i < rows.length; i++) {
            if (rows[i][this.kid] == key)
                return rows[i];
        }

        //not found
        return null;
    };

    //get current rows
    this.getOldRows = function () {
        return this.oldRows;
    };

    //set this.fidTypes[] 只寫一次
    this._setFidTypes = function (trObj) {
        if (this.fidTypes !== null)
            return;

        if (trObj === undefined)
            trObj = this.rowsBox.find('tr:first');
        this.fidTypes = _editMany.getFidTypes(trObj);
    };

    //tr: tr object
    this._getRowByTr = function (trObj) {
        var row = {};
        for (var i = 0; i < this.fidLen; i++) {
            fid = this.fidTypes[i];
            obj = trObj.find('[data-id=' + fid + ']');
            row[fid] = _input.getByType(obj, me.ftypes[i], trObj);
        }
        return row;
    };
    */

    /**
      * get row(json) by tr object
      * trObj {object} tr object
      * fidTypes {string array} field info array
      * return {json} one row
      */
    this.getRow = function (trObj) {
        //var fidTypes = this.fidTypes;
        var row = {};
        for (var i = 0; i < this.fidTypeLen; i = i + 2) {
            fid = this.fidTypes[i];
            obj = _obj.get(fid, trObj);
            row[fid] = _input.getByType(obj, this.fidTypes[i + 1], trObj);
        }
        return row;
    };

    /**
     * get updated json
     * param {upKey}
     * return {json} different column only
     */
    this.getUpdJson = function (upKey) {
        if (!this.hasForm)
            return this.ufGetUpdJson();

        var json = {};
        json[_crud.Rows] = this.getUpdRowsByArg(upKey, this.rowsBox);
        json[_crud.Deletes] = this.getDeletedRows();
        return json;
    };

    /**
     * get updated rows
     * param {upKey}
     * return {json} different column only
     */
    /*
    this.getUpdRows = function (upKey) {
        return this.hasForm
            ? this.getUpdRowsByArg(upKey, this.rowsBox)
            : this.ufGetUpdRows();
    };
    */

    //是否為new key, parseInt(英數字) 會傳回int, 不可使用!1
    this.isNewKey = function (key) {
        return (key.length <= 5);
    };

    /**
     * get updated rows(not include _childs, _deletes)
     * will also set fkeyFid
     * param {object} rowsBox rows container
     * param {string} tr filter(optional) default to 'tr'
     * return null if empty
     */ 
    this.getUpdRowsByArg = function (upKey, rowsBox, trFilter) {
        //rowsBox = rowsBox || this.rowsBox;
        trFilter = trFilter || 'tr';

        //set sort field
        this.setSort();

        //debugger;
        var rows = [];  //return rows        
        var me = this;  //先用變數接起來, 否則在 each() 裡面不能用 this
        rowsBox.find(trFilter).each(function (idx, item) {
            //add new row if empty key
            var tr = $(item);
            var key = _input.get(me.kid, tr);
            if (me.isNewKey(key)) {
                var row2 = me.getRow(tr);
                row2[me.DataFkeyFid] = upKey;   //無條件寫入這個欄位!!
                rows.push(row2);
                return;     //continue;
            }

            //add modified fields
            //var key = tr.data(_fun.DataKey);
            //var oldRow = me.getOldRow(key);
            var diffRow = {};
            var diff = false;
            var fid, ftype, value, obj;
            for (var j = 0; j < me.fidTypes.length; j = j + 2) {
                //label 不取值
                ftype = me.fidTypes[j + 1];
                if (ftype === 'label')
                    continue;

                fid = me.fidTypes[j];
                obj = _obj.get(fid, tr);
                value = _input.getByType(obj, ftype, tr);
                //如果使用完全比對, 字串和數字會不相等!!
                if (value != _edit.getOld(obj)) {
                    diffRow[fid] = value;
                    diff = true;
                }
            }

            if (diff) {
                /* ??
                //diffRow[me.DataIsNew] = 0;
                //diffRow[_fun.DataKey] = key;
                for (var j = 0; j < me.extFidLen; j++) {
                    diffRow[me.extFids[j]] = oldRow[me.extFids[j]];
                }
                */
                diffRow[kid] = key;    //set key value
                //diffRow[me.DataFkeyFid] = upKey;   //無條件寫入這個欄位!!
                rows.push(diffRow);
            }
        });
        return (rows.length === 0) ? null : rows;
    };

    /** 
     * get deleted rows(key array "string" !!)
     * return empty if empty.
     */ 
    this.getDeletedRows = function () {
        return (this.deletedRows.length === 0)
            ? null : this.deletedRows.join();
    };    

    //onclick addRow button
    this.onAddRow = function () {
        this.addRow();
    };

    /**
     * 增加一筆資料
     * param {object} row(optional)
     * return {object} row jquery object(with UI)
     */
    //this.addRow = function (upKey, row) {
    this.addRow = function (row) {
        if (!this.hasForm) {
            _log.error('EditMany.js addRow() failed, hasForm is false.');
            return null;
        }

        row = row || {};
        //row[this.DataIsNew] = isNew ? 1 : 0;
        //var isNew = this.isNewRow(row);
        //if (this.isNewRow(row))
        //    row[this.kid] = ;
        //if (this.oldRows == null)
        //    this.oldRows = [];
        //this.oldRows[this.oldRows.length] = row;

        var obj = this.renderRow(row);
        this.boxSetNewId(obj);
        return obj;
    };

    //user click deleteRow
    this.onDeleteRow = function (btn) {        
        var trObj = $(btn).closest('tr');
        this.deleteRow(_iText.get(this.kid, trObj), trObj);
    };

    /**
     * add deleted row & remove UI row
     * param {string} key: row key
     * param {object} (optional)trObj tr object
     */ 
    this.deleteRow = function (key, trObj) {
        var rows = this.deletedRows;
        var found = false;
        var rowLen = rows.length;
        for (var i = 0; i < rowLen; i++) {
            //do nothing if existed
            if (rows[i][this.kid] === key) {
                found = true;
                break;
            }
        }

        //add deleted[]
        if (!found)
            rows[rowLen] = key;

        //remove row
        //if (this.hasForm && trObj)
            //this.rowsBox.remove(trObj);
        trObj.remove();
    };

    /*
    //update row property
    this.setRow = function (key, row) {
        //find key & update
        for (var i = 0; i < this.oldRows.length; i++) {
            var oldRow = this.getOldRow(key);
            if (oldRow == null) {
                _error.log('EditMany.js setRow() failed: no row with key=' + key);
                continue;
            }

            //copy to oldRow
            _json.copy(row, oldRow);
            break;
        }
    };
    */

    /**
     * render row by UI template
     * return jquery object of row
     */ 
    this.renderRow = function (row) {
        if (!this.hasForm)
            return null;

        var obj = $(Mustache.render(this.tplRow, row));
        //obj.data(this.kid, row[this.kid]);
        obj.appendTo(this.rowsBox);
        return obj;
    };

    /**
     * formData add files
     * param {string} levelStr
     * param {FormData} data
     * return void
     */ 
    this.dataAddFiles = function (levelStr, data) {
        if (this.fileLen === 0)
            return;

        var map = {};   //fileId 和筆數的對應
        this.rowsBox.find('tr').each(function (index, item) {
            var tr = $(item);
            for (var i = 0; i < this.fileLen; i++) {
                var fid = this.fileIds[i];
                var serverId = _edit.getFileServerId(levelStr, fid);
                if (_iFile.dataAddFile(data, fid, serverId, this.form)) {
                    map[fid] = (map[fid] == null) ? 0 : map[fid] + 1;
                    //增加一個變數, 表示檔案對應的Id(負值表示new)
                    data.append("_" + serverId + map[fid], tr.data(_key));  //row Id與上傳檔案對應
                }
            }
        });
    };

    //=== 以下待修正 ===
    /**
     @description 2個功能: 
       1.FormDate 增加上傳檔案
       2.累加多筆資料
     如果多筆資料有上傳檔案, 而且是多主key, 則要在後端自行處理上傳檔案名稱的問題 !!
     注意: radio 有2種情形(true/false):
         
     @param {object} data FormData(在外面宣告), 把上傳檔案加到這個變數裡面
     @param {array} toRows 來源多筆資料, [0]為單筆(已存在), [1]以後為多筆(開始寫入)
     @param src: container
     @param oneRadio: 
       1.false: row有自己的 radio group(default): (此時id & name不同):
         用id 找name, 取name有checked的項目取值, 再寫回 id欄位
       2.true: rows共用一個 radio group: (此時id & name相同):
     //kid: key id欄位名稱, 把key值寫到這個欄位, 如果沒有上傳檔案, 則不需要
     //setRowsFiles: function (data, src, src, kid) {
     //@return 資料筆數
    */
    //addFilesAndRows: function (data, toRows, src) {
    //??
    /*
    this.dataAddRows = function (data, toRows, src) {
        //if (oneRadio === undefined)
        //    oneRadio = false;
        var fileLen = data.getAll('files').length;    //目前上傳檔案數量
        var rows = [];      //要異動的多筆資料
        var fields = [];    //obj. id, type欄位
        src.box.find('tr').each(function (index, item) {
            //寫入欄位資訊 fields[id,type] (只寫第一次)
            var tr = $(item);
            if (fields.length === 0) {
                //尋找所有 data-id 的欄位
                tr.find("[data-id]").each(function (i2, item2) {
                    var obj2 = $(item2);
                    fields[i2] = {
                        //obj: obj2,
                        id: obj2.data('id'),
                        type: _input.getType(obj2),
                    };
                });
            }

            //檔案加入 formData, 欄位名稱(後端變數名稱)為 files
            var fileNo = -1;    //初始化, -1表示無檔案
            var files = tr.find(':file');
            if (files.length > 0) {
                files = files[0].files;
                if (files.length > 0) {
                    data.append('files', files[0]);
                    fileNo = fileLen;
                    fileLen++;
                }
            }

            //寫入異動資料
            //row為多筆的一筆資料, 保留欄位的名稱加底線
            var row = { _fileNo: fileNo };  //對應要上傳的檔案位置序號, -1表示無檔案
            //if (kid !== undefined && kid != '')
            //    row[kid] = tr.data('key');      //寫入key值
            row._fun = tr.data(this.dataFun);          //row fun??
            row._key = tr.data(this.DataKey);          //row key
            for (var i = 0; i < fields.length; i++) {
                var field = fields[i];
                var value = '';
                var obj = tr.find('[data-id=' + field.id + ']');
                //考慮多筆的 radio 欄位是否為共用!!
                row[field.id] = (field.type == 'radio')
                    ? (src.oneRadio)
                        ? _iRadio.getO(obj, src.box)
                        : _iRadio.get(obj.attr('name'), src.box)
                    : _input.getByType(obj, field.type, tr);
            }
            rows.push(row);
        });

        //陣列加一
        toRows[toRows.length] = rows;   //寫入外部 rows
        src.rows = rows;    //同時寫入自己的rows !!
        //return rows;
    };

    //write row key info from jquery object to model
    this.keyObjToModel = function (obj, model) {
        model[this.DataIsNew] = obj.data(this.DataIsNew);
        model[this.DataKey] = obj.data(this.DataKey);
    };
    //write row key info from model to jquery object
    this.keyModelToObj = function (model, obj) {
        obj.data(this.DataIsNew, model[this.DataIsNew]);
        obj.data(this.DataKey, model[this.DataKey]);
    };
    this.keyModelToModel = function (from, to) {
        if (!_str.isEmpty(from[this.DataIsNew]))
            to[this.DataIsNew] = from[this.DataIsNew];
        if (!_str.isEmpty(from[this.DataKey]))
            to[this.DataKey] = from[this.DataKey];
    };
    this.keyValuesToObj = function (isNew, key, obj) {
        obj.data(this.DataIsNew, isNew);
        obj.data(this.DataKey, key);
    };
    this.keyValuesToModel = function (isNew, key, model) {
        model[this.DataIsNew] = isNew;
        model[this.DataKey] = key;
    };
    */

    /**
    * @param {string} rows checkbox data-id value
    */
    this.zz_boxLoadData = function (rows) {
        var len = (rows == null) ? 0 : rows.length;

        //empty rows ui first
        this.rowsBox.empty();

        //render rows
        for (var i = 0; i < len; i++)
            this.rowsBox.append(Mustache.render(this.tplRow, rows[i]));

        //keep old column values

        //reset
        //this.oldRows = rows;
        this.newIndex = 0;
        this.deletedRows = [];
    };    

    /**
     * todo
     * @description 修改fun為'U' if need
     */
    this.onChangeFile = function (me) {
        //檢查檔案大小 50M
        if (me.files[0].size > _fun.maxFileSize) {
            _tool.msg('上傳檔案不可大於50M !');
            me.value = '';
            return;
        }

        var tr = $(me).closest('tr');
        if (tr.data('fun') === '')
            tr.data('fun', _fun.ModeU);
    };

    this.rowSetMapId = function (row, fkeyFid) {
        if (row != null && this.isNewRow(row))
            row[this.DataFkeyFid] = fkeyFid;
    };

    this.rowsSetMapId = function (rows, fkeyFid) {
        if (rows != null) {
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                if (row != null && this.isNewRow(row))
                    row[this.DataFkeyFid] = fkeyFid;
            }
        }
    };

    /*
    this.boxSetMapId = function (box, fkeyFid) {
        box.data(this.DataFkeyFid, fkeyFid);
    };
    */

    //set new id for box
    this.boxSetNewId = function (box) {
        this.newIndex++;
        _iText.set(this.kid, this.newIndex, box);
    };

    //set sort field
    this.setSort = function () {
        var sortFid = this.sortFid;
        if (!_str.isEmpty(sortFid)) {
            this.rowsBox.find('tr').each(function (i, item) {
                //this did not work in this loop !!
                _iText.set(sortFid, i, $(item));
            });
        }
    };


    //=== 最後呼叫 ===
    this.init();

    /*
    //??
    //src: 來源資料
    //return: true/false
    this.onClickDeleteRows = function (src) {
        var find = false;
        if (src.deletedRows == null)
            src.deletedRows = [];
        src.box.find('[data-id=' + src.checkFid + ']:checked').each(function (index, item) {
            find = true;
            var check = $(item);
            var tr = check.closest('tr');
            var key = tr.data('key');
            if (key !== '')
                src.deletedRows[src.deletedRows.length] = key;
            //刪除資料
            tr.remove();
        });
        return find;
        //_tool.msg('請先選取資料。')
    };

    //選取所有checkbox
    //onClickCheckAll: function (tableId, dataFid, status) {
    onClickCheckAll: function (me, dataFid) {
        dataFid = dataFid || '_check0';
        var status = me.checked;
        $(me).closest('table').find('[data-id=' + dataFid + ']:not(:disabled)').prop('checked', status);
    };

    //??
    //get field by rowNo and dataId ??
    this.getField = function (tbody, rowNo, dataId) {
        return tbody.find('tr').eq(rowNo).find('[data-id=' + dataId + ']');
    };

    //?? -> _crud.js
    //keys is two dimension
    this.keysToStr = function (keys) {
        var strs = [];
        for (var i = 0; i < keys.length; i++) {
            strs[i] = (keys[i].length == 0)
                ? ''
                : keys[i].join(_fun.RowSep);
        }
        return strs.join(_fun.TableSep);
    };
    */

} //class