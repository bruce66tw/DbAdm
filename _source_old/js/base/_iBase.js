
//base class of all input field
//must loaded first, or will got error !!
var _iBase = {

    //get value by id
    get: function (id, box) {
        return _iBase.getO(_obj.get(id, box));
    },
    //get value by filter
    getF: function (ft, box) {
        return _iBase.getO(_obj.getF(ft, box));
    },
    /*
    //get value by name
    getN: function (id, box) {
        return _iBase.getO(_obj.getN(id, box));
    },
    */
    //get value by object
    getO: function (obj) {
        return obj.val();
    },

    //set value
    set: function (id, value, box) {
        _iBase.setO(_obj.get(id, box), value)
    },
    setF: function (ft, value, box) {
        _iBase.setO(_obj.getF(ft, box), value)
    },
    /*
    setN: function (id, value, box) {
        _iBase.setO(_obj.getN(id, box), value)
    },
    */
    setO: function (obj, value) {
        obj.val(value);
    },

    //set edit status
    setEdit: function (id, status, box) {
        _iBase.setEditO(_obj.get(id, box), status);
    },
    setEditF: function (ft, status, box) {
        _iBase.setEditO(_obj.getF(ft, box), status);
    },
    /*
    setEditN: function (id, status, box) {
        _iBase.setEditO(_obj.getN(id, box), status);
    },
    */
    setEditO: function (obj, status) {
        obj.prop('readonly', !status);
    },

};//class