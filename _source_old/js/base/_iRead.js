
//label
var _iRead = {

    //value by id
    get: function (id, form) {
        return _iRead.getO(_obj.getD(id, form));   //use data-id
    },
    //value by filter
    getF: function (filter, form) {
        return _iRead.getO(_obj.getF(filter, form));
    },
    //value by object
    getO: function (obj) {
        return obj.text();
    },

    set: function (id, value, form) {
        _iRead.setO(_obj.getD(id, form), value);   //use data-id
    },
    setF: function (filter, value, form) {
        _iRead.setO(_obj.getF(filter, form), value)
    },
    setO: function (obj, value) {
        obj.text(value);
    },

};