
/**
 * 注意:
 *   保留的自訂函數: 
 *     void ufAfterLoadJson(rowJson)
 *     error ufWhenSave():
 *     void ufAfterSave(): 在 _crud.js呼叫
 * 
 */

/**
 * 單筆維護, 包含以下保留欄位:
 *   _edit:
 *   _childs:
 * called by _crud.js
 * kid {string} (optional 'Id') key field id
 * formId {string} (optional 'formEdit')
 * return {EditOne}
 */ 
function EditOne(kid, formId) {

    //constant 
    //this.DataOld = '_old';      //    //舊資料存在 data 屬性, 內容必須與 _editMany.DataOld 相同

    this.init = function () {
        //this._childs = childs;
        this.kid = kid || 'Id';
        this.form = $('#' + (formId || 'formEdit'));     //multiple rows container object
        this.isMulti = false;
        //this.oldRow = null;
        //this.fids = null;       //id 欄位清單

        this.fidTypes = _edit.getFidTypes(this.form);
        this.fidTypeLen = this.fidTypes.length;

        //上傳檔案欄位id array
        this.fileIds = [];
        //var fileIds = this.fileIds;     //在each裡面不可使用this
        this.form.find(':file').each(function (index) {
            //var obj = $(this);
            this.fileIds[index] = $(this).attr('id');
        });
        this.fileLen = this.fileIds.length;
        this.hasFile = this.fileLen > 0; //是否有檔案欄位

        /*
        this.toJson = function () {
            return _form.toJson(this.form);
        };
        */
    };

    /*
    this.getFidTypes = function (box) {
        var fidTypes = [];
        box.find('[id]').each(function (i, item) {
            var obj = $(item);
            var j = i * 2;
            fidTypes[j] = obj.attr('id');
            fidTypes[j + 1] = _input.getType(obj);
        });
        return fidTypes;
    };
    */

    /*
    this.toJsonStr = function () {
        return _form.toJsonStr(this.form);
    };
    */

    /**
     * is a new row or not
     * return {bool}
     */
    this.getKey = function () {
        return _input.get(this.kid, this.form);
    };

    /**
     * is a new row or not
     * return {bool}
     */
    this.isNewRow = function () {
        return _str.isEmpty(this.getKey());
    };

    this.loadRow = function (row) {
        _form.loadRow(this.form, row);

        //set old value for each field
        for (var i = 0; i < this.fidTypeLen; i = i + 2) {
            fid = this.fidTypes[i];
            var obj = _obj.get(fid, this.form);
            obj.data(_edit.DataOld, row[fid]);
        }
    };

    /**
     * 讀取畫面上的資料
     * return {json}
     */
    /*
    this.getRow = function () {
        return _form.toJson(this.form);
    };
    */

    /**
     * get updated row, 包含 _childs
     * return {json} different column only
     */
    this.getUpdRow = function () {
        return _edit.getUpdRow(this.kid, this.fidTypes, this.form);
    };

    this.reset = function () {
        _form.reset(this.form);
    };

    /**
     * set form to editable or not
     * status {bool} editable or not
     */
    this.setEdit = function (status) {
        _form.setEdit(this.form, status);
    };

    /**
     * formData add files, 每個欄位只能上傳一個檔案
     * 後端參數名稱固定為t(table) + levelStr + "_" + field Id, ex: t01_File1
     * param {string} levelStr
     * param {FormData} data
     * return void
     */
    this.dataAddFiles = function (levelStr, data) {
        for (var i = 0; i < this.fileLen; i++)
            _iFile.dataAddFile(data, this.fileIds[i], _edit.getFileServerId(levelStr, this.fileIds[i]), this.form);
    };

    //=== file event start ===
    //file field be triggered
    this.onFile = function (me) {        
    };

    this.onOpenFile = function (me) {
    };

    this.onViewFile = function (me) {
    };

    this.onDeleteFile = function (me) {
    };
    //=== file event end ===

    //最後呼叫
    this.init();
}//class