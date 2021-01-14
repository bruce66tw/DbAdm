
//for EditOne.js, EditMany.js
var _edit = {

    //data property name for keep old value
    DataOld: '_old',

    /**
     * get old value 
     * param obj {object} input jquery object
     * return {string}
     */
    getOld: function (obj) {
        return obj.data(_edit.DataOld);
    },

    /**
     * set old value
     * param obj {object} input jquery object
     * param value {int/string}
     */
    setOld: function (obj, value) {
        obj.data(_edit.DataOld, value);
    },

    /*
    loadRowByArg: function (box, row, fidTypes) {
        _form.loadRow(box, row);

        //set old value for each field
        //var fidLen = fidTypes.length;
        for (var i = 0; i < fidTypes.length; i = i + 2) {
            fid = fidTypes[i];
            var obj = _obj.get(fid, box);
            obj.data(_edit.DataOld, row[fid]);
        }
    },
    */

    //called by: EditOne.js, EditMany.js
    //不設定 mapId
    getUpdRow: function (kid, fidTypes, box) {
        //如果key value為空白, 則傳回整列資料
        var key = _input.get(kid, box);
        if (_str.isEmpty(key))
            return _form.toJson(box);

        var diff = false;
        var row = {};
        var fid, ftype, value, obj;
        for (var j = 0; j < fidTypes.length; j = j + 2) {
            //skip label type
            ftype = fidTypes[j + 1];
            if (ftype === 'label')
                continue;

            fid = fidTypes[j];
            obj = _obj.get(fid, box);
            value = _input.getByType(obj, ftype, box);
            //如果使用完全比對, 字串和數字會不相等!!
            if (value != obj.data(_edit.DataOld)) {
                row[fid] = value;
                diff = true;
            }
        }
        if (!diff)
            return null;

        row[kid] = key;
        return row;
    },

    /**
     * get fid-type array
     * param box {object} container
     * return {string array} 
     */
    getFidTypes: function (box) {
        var fidTypes = [];
        box.find('[name]').each(function (i, item) {
            var obj = $(item);
            var j = i * 2;
            //fidTypes[j] = obj.data('id');
            fidTypes[j] = obj.attr('name');
            fidTypes[j + 1] = _input.getType(obj);
        });
        return fidTypes;
    },

    /**
     * get field info array by box object & row filter
     * box {object} form/div container
     * trFilter {string} (optional 'tr')
     * return json array
     */
    /*
    getFidTypesByDid: function (box, trFilter) {
        //trFilter = trFilter || 'tr';
        //var trObj = box.find(trFilter + ':first');

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

    /*
    //for EditMany.js
    getFidTypesById: function (box) {
        //return _edit._getFidTypes(box, '[id]');
        var fidTypes = [];
        box.find('[id]').each(function (i, item) {
            var obj = $(item);
            var j = i * 2;
            fidTypes[j] = obj.attr('id');
            fidTypes[j + 1] = _input.getType(obj);
        });
        return fidTypes;
    },
    */

    /**
     * get field info array: 0:id, 1:type
     * param {object} trObj
     * return {string array}
     */
    /*
    _getFidTypes: function (box, filter) {
        var fidTypes = [];
        box.find(filter).each(function (i, item) {
            var obj = $(item);
            var j = i * 2;
            fidTypes[j] = obj.data('id');
            fidTypes[j + 1] = _input.getType(obj);
        });
        return fidTypes;
    },
    */

    /**
     * get server side var name for file field
     * param {string} levelStr
     * param {string} fid: ui file id
     * return {string} format: t(table) + levelStr + '_' + field Id
     */
    getFileServerId: function (levelStr, fid) {
        return 't' + levelStr + '_' + fid;
    }
};