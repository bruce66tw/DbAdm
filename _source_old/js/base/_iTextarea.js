
//textarea欄位, 如果為 html 內容, 則必須再呼叫 _iHtml.js 功能 !!
var _iTextarea = $.extend({}, _iBase, {

    getO: function (obj) {
        return obj.html();
        //return obj.val();
    },

    setO: function (obj, value) {
        obj.html(value);
        //obj.val(value);
    },

}); //class