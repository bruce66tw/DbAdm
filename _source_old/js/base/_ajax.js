//jquery ajax call
var _ajax = {

    /** 
     * ajax call
     * url {string} action url
     * data {json} property should be string !!
     * fnOk {function} success callback function
     * fnError {function} failed callback function
     * return {json} 執行結果
     */
    getJson: function (url, data, fnOk, fnError) {
        var json = {
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',   //backend return json(JsonResult)
            //processData: false
        };
        _ajax._call(json, fnOk, fnError);
    },

    /**
     * if upload file, must use FormData
     */
    getJsonByFormData: function (url, data, fnOk, fnError) {
        var json = {
            url: url,
            type: 'POST',
            cache: false,
            data: data,
            contentType: false,     //false!! 傳入參數編碼方式, default為 "application/x-www-form-urlencoded"
            dataType: 'json',       //todo: testing
            processData: false,     //false!! if true it will convert input data to string, then get error !!
        };
        _ajax._call(json, fnOk, fnError);
    },

    /**
     * backend return string
     */ 
    getStr: function (url, data, fnOk, fnError) {
        var json = {
            url: url,
            type: 'POST',
            data: data,
            dataType: 'text',   //backend return text(ContentResult with text)
        };
        _ajax._call(json, fnOk, fnError);
    },

    /**
     * backend return partial view
     * return html string
     */
    getView: function (url, data, fnOk, fnError) {
        var json = {
            url: url,
            type: 'POST',
            data: data,
            dataType: 'html',
        };
        _ajax._call(json, fnOk, fnError);
    },

    /**
     * upload file
     * param url {string}
     * param serverId {} server side id
     * param fileObj {file} file object
     * param fnOk {function}
     */
    file: function (url, serverId, fileObj, fnOk, fnError) {
        var data = new FormData();  //for upload files if need 
        data.append(serverId, fileObj);
        var json = {
            url: url,
            type: 'POST',
            data: data,
            dataType: 'text',       //server return text
            cache: false,
            contentType: false,     //false!! 傳入參數編碼方式, default為 "application/x-www-form-urlencoded"
            processData: false,     //false!! if true it will convert input data to string, then get error !!
        };
        _ajax._call(json, fnOk, fnError);
    },

    /**
     * 包裝jQuery ajax(), 只傳回成功的狀態(包含自訂錯誤訊息)
     * param json {json} ajax json
     * param fnOk {function} callback function
     * return {json} ResultDto
     */
    _call: function (json, fnOk, fnError) {
        var config = {
            //contentType: 'application/json; charset=utf-8',
            //traditional: true,
            //async: false,
            success: function (data) {
                //data 參數對應後端 ResultDto
                if (data && data.ErrorMsg) {
                    if (fnError == null)
                        _tool.msg(data.ErrorMsg);
                    else
                        fnError(data);
                } else if (fnOk) {
                    fnOk(data);
                }
            },

            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr != null) {
                    console.log("status" + xhr.status);
                    console.log(thrownError);
                }
            },
            beforeSend: function () {
                _tool.showWait();
            },
            complete: function () {
                _tool.hideWait();
            },
        };

        $.ajax(_json.copy(json, config));
    },

};//class