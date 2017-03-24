
//作者:李天页
//功能描述:后台管理系统中，系统设置需要用到的js


//启动事件
$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });
})

//打开系统设置模态窗体
function btnSystemSettingManager(obj) {

    $('#allPage').layout('collapse', 'east');       //不管查询区是否打开 在加载 datagrid 时都执行关闭操作
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->系统管理  -->系统设置')  //将操作区标题改为对应的系统菜单值
    $(obj).parent().parent().parent().find("a").removeClass("current");     //加载时首先将所有 传入对象的父节点的父节点下的所有 a 标签 移除 .current 样式
    $(obj).addClass("current"); //添加样式 区别于没有点击的菜单项
    $('#IsHaveMessage').hide(); //隐藏 "当前无数据" 的提示
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view, //由于共用了一个 datagrid 所以一加载就设置 view 为默认样式 
    });
    $('#windowforbtnSystemSettingManager').window('open');  
}

//星级评论共用一个分布视图，ajax异步调用
function starComment(several) {
    var url = "/admin/HomeSystemSetting/starComment";
    var data = { "several": several };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//星级评论中，点击确认修改按钮，验证数据是否通过
function btnStarValue() {
    var num = $("#statNum").val();
    var several = $("#several").val();
    var url = "/admin/HomeSystemSetting/editStarComment";
    var data = { "num": num, "several": several };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            //0:成功，1:失败,2:输入数据小于0
            if (data.success == 0) {
                $.messager.alert('提示', '修改成功');
            }
            if (data.success == 1) {
                $.messager.alert('提示', '修改失败');
            }
            if (data.success == 2) {
                $.messager.alert('提示', '输入数据必须大于0');
            }
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

//签到设置，共用一个分布视图，ajax异步调用,flag代表是单次还是连续,0:单次,1:连续
function SignSetting(flag) {
    var url = "/admin/HomeSystemSetting/SignView";
    var data = { "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//签到设置，点击确认修改按钮，验证数据是否通过
function btnSignValue() {
    var num = $("#statNum").val();
    var flag = $("#flag").val();
    var url = "/admin/HomeSystemSetting/EditSign";
    var data = { "num": num, "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            //0:成功，1:失败,2:输入数据小于0
            if (data.success == 0) {
                $.messager.alert('提示', '修改成功');
            }
            if (data.success == 1) {
                $.messager.alert('提示', '修改失败');
            }
            if (data.success == 2) {
                $.messager.alert('提示', '输入数据必须大于0');
            }
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

//确认收货和确认订单的分布视图，ajax调用,flag代表是确认收货还是确认订单,0:确认收货,1:确认订单
function confirmSetting(flag) {
    var url = "/admin/HomeSystemSetting/ConfirmView";
    var data = { "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//确认时间设置，点击确认修改按钮，验证数据是否通过
function btnAutoValue() {
    var num = $("#statNum").val();
    var flag = $("#flag").val();
    var url = "/admin/HomeSystemSetting/EditAutoConfirmTime";
    var data = { "num": num, "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            //0:成功，1:失败,2:输入数据小于0
            if (data.success == 0) {
                $.messager.alert('提示', '修改成功');
            }
            if (data.success == 1) {
                $.messager.alert('提示', '修改失败');
            }
            if (data.success == 2) {
                $.messager.alert('提示', '输入数据必须大于0');
            }
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

//分页尺寸的分页视图
function pageSizeSetting() {
    var url = "/admin/HomeSystemSetting/pageSizeSetting";
    $.ajax({
        url: url,//请求那个网址               
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//分页尺寸数值修改
function btnPageSizeValue() {
    var num = $("#statNum").val();
    var url = "/admin/HomeSystemSetting/EditPageSize";
    var data = { "num": num };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            //0:成功，1:失败,2:输入数据小于0
            if (data.success == 0) {
                $.messager.alert('提示', '修改成功');
            }
            if (data.success == 1) {
                $.messager.alert('提示', '修改失败');
            }
            if (data.success == 2) {
                $.messager.alert('提示', '输入数据必须大于等于0');
            }
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

//系统是否关闭，分页视图，ajax调用
function systemOnOrOff() {
    var url = "/admin/HomeSystemSetting/SystemOpenClose";
    $.ajax({
        url: url,//请求那个网址               
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//系统开启or关闭设置,ajax调用
function btnOpenClose(flag) {
    var url = "/admin/HomeSystemSetting/OpenOrClose";
    var data = { "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            //0:成功，1:失败,2:输入数据小于0
            if (data.success == 0) {
                if (data.flag == 0) {
                    $.messager.alert('提示', '系统成功启动');
                }
                if (data.flag == 1) {
                    $.messager.alert('提示', '系统成功关闭');
                }
            }
            if (data.success == 1) {
                if (data.flag == 0) {
                    $.messager.alert('提示', '系统当前已经开启，请不要重复开启');
                }
                if (data.flag == 1) {
                    $.messager.alert('提示', '系统当前已经关闭，请不要重复关闭');
                }
            }

            //this.systemOnOrOff()这个方法无法调用，只能将代码直接写在这里 
            var url = "/admin/HomeSystemSetting/SystemOpenClose";
            $.ajax({
                url: url,//请求那个网址               
                dataType: "html",//希望服务端返回的数据类型json,text,html...
                cache: false,//异步加载                
                success: function (data, textStatus) {
                    // data 可能是 xmlDoc, jsonObj, html, text, 等等...
                    //this; // 调用本次AJAX请求时传递的options参数
                    //status=200，请求成功触发
                    $("#SystemSettingManagerCenter").html(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // 通常 textStatus 和 errorThrown 之中
                    // 只有一个会包含信息
                    //this; // 调用本次AJAX请求时传递的options参数
                    //请求失败，如404,500错误
                    $.messager.alert('错误提示', '请求发生了错误' + textStatus, 'error');
                }
            });

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // 通常 textStatus 和 errorThrown 之中
            // 只有一个会包含信息
            //this; // 调用本次AJAX请求时传递的options参数
            //请求失败，如404,500错误
            $.messager.alert('错误提示', '请求发生了错误' + textStatus);
        }
    });

}





//兑换积分页面，ajax调用呈现,flag,0:钱直接换积分,1:订单价值增加积分
function moneyToIntegral(flag) {
    var url = "/admin/HomeSystemSetting/MoneyToInteView";
    var data = { "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "html",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            $("#SystemSettingManagerCenter").html(data);
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

//兑换积分页面,点击确定按钮事件，ajax调用呈现,flag,0:钱直接换积分,1:订单价值增加积分
function btnInteValue() {
    var value = $("#value").val();
    var flag = $("#flag").val();
    var url = "/admin/HomeSystemSetting/MoneyToInteHandle";
    var data = { "value": value, "flag": flag };
    $.ajax({
        url: url,//请求那个网址
        data: data,//传递什么数据到服务端
        dataType: "json",//希望服务端返回的数据类型json,text,html...
        cache: false,//异步加载                
        success: function (data, textStatus) {
            // data 可能是 xmlDoc, jsonObj, html, text, 等等...
            //this; // 调用本次AJAX请求时传递的options参数
            //status=200，请求成功触发
            if (data.success == 0) {
                $.messager.alert('提示', '修改成功');
            }
            if (data.success == 1) {
                $.messager.alert('提示', '修改失败');
            }
            if (data.success == 2) {
                $.messager.alert('提示', '输入数据必须大于等于1');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // 通常 textStatus 和 errorThrown 之中
            // 只有一个会包含信息
            //this; // 调用本次AJAX请求时传递的options参数
            //请求失败，如404,500错误
            $.messager.alert('错误提示', '请求发生了错误' + textStatus);
        }
    });
}