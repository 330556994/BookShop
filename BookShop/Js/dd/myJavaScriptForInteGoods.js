
//作者:李天页
//功能描述:在积分换购商品页面的js功能




$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });

});

//页面初始化的时候，ajax加载全部积分商品
$(document).ready(this.Integral(0));

        
//ajax调用，异步加载中央内容
function Integral(sectionId) {
    var url = "/IntegralGoods/IntegralSection_ajax";            
    var data = { "sectionId": sectionId}
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载
               
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#center").html(data);
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


//点击换购事件
function buy(id, PaymentIntegral) {
    var score = $("#score").val();
    if(score<PaymentIntegral){
        alert('对不起，您的可用积分不足!')
    }else{
        url = "/order/add?goodsId=" + id;
        location.href = url;
    }
}

//不需要积分换购
$(function(){    
    $("#btn").bind('click',function(){   
        var url='/order/add';             
        location.href=url;
    });           
});  
