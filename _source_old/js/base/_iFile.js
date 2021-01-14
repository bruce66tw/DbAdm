
//檔案上傳欄位(單筆) & 檔案處理
var _iFile = {

    //初始化, 在一開始載入畫面資料時呼叫
    init: function(fid, path, form) {
        //_obj.getF(_iFile.fileF(id), form).val('');
        var file = _obj.getF(_iFile.fileF(fid), form);
        file.val('');
        file.data('fun', '');
        _iFile.setPath(fid, path, form);
        /*
        //file element 要 reset
        var file = _obj.getF(_iFile.fileF(id), form);
        //var $el = $('#example-file');
        file.wrap('<form>').closest('form').get(0).reset();
        file.unwrap();
        */
    },

    //get file filter string
    fileF: function (fid) {
        return '[name=' + fid + ']';
    },

    //get label filter of file
    labelF: function(fid) {
        return '.xd-' + fid + '-url';
    },

    //顯示路徑(檔名only)
    setPath: function (fid, path, form) {
        var label = _obj.getF(_iFile.labelF(fid), form);
        var link = label.find('a');
        var fileName = _iFile.getFileName(path);
        link.text(fileName);    //儲存路徑的地方
        link.attr('href', path);

        //顯示label和刪除link
        if (_str.isEmpty(fileName))
            label.hide();
        else
            label.show();
    },

    //private: 讀取某個字串後面的文字
    _getFileAfter: function (path, sep) {
        //var name = path;
        var pos = path.lastIndexOf(sep);
        if (pos > 0)
            path = path.substring(pos + 1);
        return path;
    },

    //讀取檔名
    getFileName: function (path) {
        return _iFile._getFileAfter(path, '/');
    },

    //讀取副檔名
    getFileExt: function (path) {
        return _iFile._getFileAfter(path, '.');
    },

    //private: get file object
    _getObject: function (fid, form) {
        return _obj.getF(_iFile.fileF(fid), form);
    },

    //增加一個路徑
    addPath: function (fid, path, form) {
        var obj = _iFile._getObject(fid, form);
        _iFile._setEdit(obj, _fun.ModeC);
        _iFile.setPath(fid, path, form);
    },

    //刪除路徑(設定 data-deleted 屬性)
    deletePath: function (fid, form) {
        var obj = _iFile._getObject(fid, form);
        _iFile._setEdit(obj, 'D');
        _iFile.setPath(fid, '', form);
    },

    //private
    //set data-fun
    _setEdit: function(obj, status) {
        obj.data('fun', status);
    },

    //路徑是否刪除
    isDeleted: function (fid, form) {
        var obj = _iFile._getObject(fid, form);
        return (obj.data('fun') == 'D');
    },

    //是否有 "異動(新增/修改)" 的上傳檔案, 如果沒有則傳回null
    getFile: function (fid, form) {
        var obj = _iFile._getObject(fid, form);
        var files = obj.get(0).files;
        return (files.length > 0) ? files[0] : null;
    },

    //是否上傳檔案, 包含原本存在和後來選取的檔案(判斷link內容是否空白)
    //page沒有foucs時, visible會傳回false, 所以在這裡判斷 href
    hasFile: function (fid, form) {
        //return _obj.getF(_iFile.labelF(fid), form).find('a').is(':visible');
        return !_str.isEmpty(_obj.getF(_iFile.labelF(fid), form).find('a').attr('href'));
    },

    //row add file for upload & save row
    rowAddFile: function (data, row, fid, serverId, form) {
        var file = _iFile.getFile(fid, form);
        if (file != null)
            data.append(serverId, file);
        else if (_iFile.isDeleted(fid, form))
            row['_' + fid] = 1;
    },

    /**
     * formData add file for upload
     * param {formData} data
     * param {string} fid: file field id
     * param {string} serverId: server side variable name
     * param {object} form: jquery form
     * return {boolean} has file or not
     */ 
    dataAddFile: function (data, fid, serverId, form) {
        var file = _iFile.getFile(fid, form);
        var hasFile = (file != null);
        if (hasFile)
            data.append(serverId, file);
        return hasFile;
    },

    //傳回所有新增的資料列中, 第一筆含有空白檔案的index
    //jquery .each break: return n(continue), return false(break)
    //return: 空白檔案的資料序號(base 0), -1表示無
    getEmptyFileForNew: function (form) {
        var found = -1;
        form.find('tr').each(function (index, item) {
            var tr = $(item);
            if (tr.data('fun') === _fun.ModeC) {
                var obj = tr.find(':file');
                if (obj.length === 0 || obj[0].files.length == 0) {
                    found = index;
                    return false;
                }
            }
        });
        return found;
    },

    //=== event start ===
    onChangeFile: function (me) {
        //case of 無檔案
        var me2 = $(me);
        var value = me.value;
        var fid = me2.data('id');
        if (_str.isEmpty(value)) {
            _iFile.deletePath(fid);
            return;
        }

        //檢查副檔名
        var exts = me2.data('exts').toLowerCase();
        if (!_str.isEmpty(exts)) {
            var ext = _iFile.getFileExt(value).toLowerCase();
            exts = ',' + exts + ',';
            if (exts.indexOf(',' + ext + ',') < 0){
                _tool.msg(RB.UploadFileNotMatch);
                me.value = '';
                return;
            }
        }

        //檢查檔案大小
        var max = me2.data('max');
        if (me.files[0].size > max * 1024 * 1024) {
            _tool.msg(_str.format(RB.UploadFileNotBig, max));
            me.value = '';
            return;
        }

        //case ok
        _iFile.addPath(fid, value);
    },
    //=== event end ===

}; //class