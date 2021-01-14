
//small public components
var _tool = {

    init: function () {
        //alert
        _tool.xgAlert = $('#xgAlert');

        //msg
        _tool.xgMsg = $('#xgMsg');
        _tool.xgMsg.find('.modal-footer button').text(RB.BtnYes);

        //ans
        _tool.xgAns = $('#xgAns');
        _tool.xgAns.find('.xd-yes').text(RB.BtnYes);    //set button text
        _tool.xgAns.find('.xd-no').text(RB.BtnCancel);

        //edit area
        _tool.xgArea = $('#xgArea');
        _tool.xgArea.find('.xd-yes').text(RB.BtnYes);
        _tool.xgArea.find('.xd-no').text(RB.BtnCancel);
    },

    /**
     * show message box
     * param msg {string} html or string
     * param fnOk {function} callback function
     */
    msg: function (msg, fnOk) {
        var box = _tool.xgMsg;
        box.find('.xd-msg').html(msg);
        _modal.showO(box);

        //set callback
        _tool._fnOnMsgOk = fnOk;
    },

    /**
     * show confirmation 
     */
    ans: function (msg, fnYes, fnCancel) {
        var box = _tool.xgAns;
        box.find('.xd-msg').html(msg);
        _modal.showO(box);

        //set callback
        _tool._fnOnAnsYes = fnYes;
        _tool._fnOnAnsNo = (fnCancel === undefined) ? null : fnCancel;
    },

    /**
     * show alert(auto close), use bootstrap alert
     * param {string} msg
     * param {string} color: default blue, R(red)
     */
    alert: function (msg, color) {
        var box = _tool.xgAlert;
        box.find('.xd-msg').text(msg)
        box.fadeIn(500, function () {
            box.show();
            setTimeout(function () {
                _tool.onAlertClose();
            }, 5000);   //show 5 seconds
        });
    },

    //show waiting
    showWait: function () {
        //$('body').addClass('xg-show-loading');
        $('#xgWait').show();
    },
    hideWait: function () {
        //$('body').removeClass('xg-show-loading');
        $('#xgWait').hide();
    },

    //show waiting
    showArea: function (title, value, fnOk) {
        var box = _tool.xgArea;
        box.find('.modal-title').text(title);
        box.find('textarea').val(value);
        _tool._fnOnAreaYes = fnOk;
        _modal.showO(box);
    },

    onAreaYes: function () {
        var box = _tool.xgArea;
        if (_tool._fnOnAreaYes) {
            _modal.hideO(box);
            var value = box.find('textarea').val();
            _tool._fnOnAreaYes(value);
        }
    },

    /**
     * onclick alert close button
     */
    onAlertClose: function () {
        var box = _tool.xgAlert;
        box.fadeOut(500, function () {
            box.hide();
        });
    },

    /**
     * triggered when user click confirmation yes button
     * called by XgAnsHelper
     */
    onAnsYes: function () {
        if (_tool._fnOnAnsYes) {
            _modal.hideO(_tool.xgAns);
            _tool._fnOnAnsYes();
        }
    },
    onAnsNo: function () {
        if (_tool._fnOnAnsNo)
            _tool._fnOnAnsNo();
        _modal.hideO(_tool.xgAns);
    },
    onMsgOk: function () {
        if (_tool._fnOnMsgOk)
            _tool._fnOnMsgOk();
        _modal.hideO(_tool.xgMsg);
    },

}; //class