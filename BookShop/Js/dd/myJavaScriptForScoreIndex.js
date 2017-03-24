

//作者:李天页
//功能描述:钱币直接兑换积分页面需要用到的jquery函数




$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });
    window.onload = changeNum();
})

//当要兑换的积分，输入框发生改变时，自动算出需要多少钱
function changeNum() {
    var num = $("#num").val();
    if (num < 0) {
        alert("请输入大于等于0的整数");
        $("#num").val(0);
        window.location.reload();
    } else {
        var url = "/ExchangeIntegral/GetNeedMoney";
        var data = { "num": num };
        $.ajax({
            url: url,
            data: data,
            dataType: "html",//希望服务端返回的数据类型json,text,html...
            cache: false,//异步加载                
            success: function (data, textStatus) {
                // data 可能是 xmlDoc, jsonObj, html, text, 等等...
                //this; // 调用本次AJAX请求时传递的options参数
                //status=200，请求成功触发
                $("#money").html(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // 通常 textStatus 和 errorThrown 之中
                // 只有一个会包含信息
                //this; // 调用本次AJAX请求时传递的options参数
                //请求失败，如404,500错误
                $.messager.alert('错误提示', '请求发生了错误' + textStatus, 'error');
            }
        });
    }
}


//点击确认修改事件
function btnBuyIntegral() {
    var num = $("#num").val();
    if (num <= 0) {
        alert("请输入大于0的整数");
    } else {
        var url = "/ExchangeIntegral/AddInte";
        var data = { "num": num };
        $.ajax({
            url: url,
            data: data,
            dataType: "json",//希望服务端返回的数据类型json,text,html...
            cache: false,//异步加载                
            success: function (data, textStatus) {
                // data 可能是 xmlDoc, jsonObj, html, text, 等等...
                //this; // 调用本次AJAX请求时传递的options参数
                //status=200，请求成功触发
                if (data.success == 0) {
                    alert("兑换成功!");
                } else {
                    alert("兑换失败!");
                }
                $("#num").val(0);
                window.location.reload();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // 通常 textStatus 和 errorThrown 之中
                // 只有一个会包含信息
                //this; // 调用本次AJAX请求时传递的options参数
                //请求失败，如404,500错误
                $.messager.alert('错误提示', '请求发生了错误' + textStatus, 'error');
            }
        });
    }
}