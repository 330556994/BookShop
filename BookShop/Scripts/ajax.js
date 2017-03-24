
//实例化，xmlhttprequest对象
function createXmlHttpRequest() {
    var xmlHttpRequest;//定义核心变量
    if (window.XMLHttpRequest) {
        //Mozilla浏览器
        xmlHttpRequest = new XMLHttpRequest();
    } else
        if (window.ActiveXObject) {
            //IE浏览器
            try {
                xmlHttpRequest = new ActiveXObject("msxml2.XMLHTTP");
            } catch (e) {
                try {
                    xmlHttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
                }
                catch (e) {
                    }
            }
        }
    return xmlHttpRequest;
}

//这个函数功能，是将一个字符串转换成json对象
function stringToJSON(str) {
    var a;
    eval('a=' + str + ';');
    return a;
}