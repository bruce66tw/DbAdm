
//program, 包含 crud功能
var _prog = {
    //filter: '.xg-prog-path',
    me: null,
    oriPath: '',    //original path

    init: function () {
        _prog.me = $('.xg-prog-path');
        _prog.oriPath = _prog.me.text();
    },

    //reset path to initial
    resetPath: function () {
        _prog.me.text(_prog.oriPath);
    },

    //for crud, set path for create
    setPathCreate: function () {
        _prog.me.text(_prog.oriPath + '-' + RB.Create);
    },
    //for crud, set path for update
    setPathUpdate: function () {
        _prog.me.text(_prog.oriPath + '-' + RB.Update);
    },
};