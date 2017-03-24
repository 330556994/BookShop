/*
* 美化表格 nicetable  1.0版本
*/
$(function () {
    //包装集上面的each方法,就是遍历每个内部元素
    //参数index 代表索引，tab代表的是当前dom元素
    $(".nicetable").each(function (index, tab) {
        $(tab).find("tr:first").addClass("title");
        $(tab).find("tr:gt(0):even").addClass("one");
        $(tab).find("tr:gt(0):odd").addClass("two");
        $(tab).find("tr:gt(0)").mouseover(function () {
            //移上去触发
            //this 代表当前对象，这是DOM元素
            //将这个DOM元素转成包装集，这样就可以使用包装集的方法
            $(this).addClass("highlight");
        }).mouseout(function () {
            //移开
            $(this).removeClass("highlight");
        }).click(function () {
            //选中效果
            $(this).toggleClass("select");
        });
    })

//    $(".nicetable tr:first").addClass("title");
  //  $(".nicetable tr:gt(0):even").addClass("one");
    //$(".nicetable tr:gt(0):odd").addClass("two");
    //jquery很多包装集方法的返回值就是jquery，就是包装集
 /*   $(".nicetable tr:gt(0)").mouseover(function () {
        //移上去触发
        //this 代表当前对象，这是DOM元素
        //将这个DOM元素转成包装集，这样就可以使用包装集的方法
        $(this).addClass("highlight");
    }).mouseout(function () {
        //移开
        $(this).removeClass("highlight");
    }).click(function () {
        //选中效果
        $(this).toggleClass("select");
    });*/
})