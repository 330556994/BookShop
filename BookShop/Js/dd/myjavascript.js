//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////这里进入前台会员 js 脚本////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//作者:四人帮   
//时间:2016/12/7
//描述:对会员控制器的大部分 ajax 方法提供支持,生成会员页面 index 页面的所有datagrid控件并提供数据展现。

$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });

    //调用 滚动广告函数
    changeImg();

    //调用滚动 秒杀信息
    movedome();

    //获取签到天数
    getRegistCount();

    //签到页面显示 星期几
    var week = new Array("日", "一", "二", "三", "四", "五", "六");
    $("#week").html("星期" + week[new Date().getDay()]);

    //显示当前系统时间
    $("#time").html("今天是" + new Date().getFullYear() + "年" + new Date().getMonth() + "月" + new Date().getDate() + "日,连续签到可以获得大量积分  ");

    //这个是为我的订单按钮添加下拉选项 可选择不同状态的订单
    var p = $('#tt').tabs().tabs('tabs')[0];
    var mb = p.panel('options').tab.find('a.tabs-inner');
    mb.menubutton({
        menu: '#mmfororder'
    }).click(function () {
        $('#tt').tabs('select', 0);
    });

    //显示订单信息
    selectMyOrders();

    /////////////////////////////// 从这里开始  对左导航中的 li 添加他们的事件   即实现点击相应 的 li 触发 click 时间   使 tabs 中的内容切换至 li 标题相对应的位置/////////////////////  

    //    tabs 的顺序 是从 0 开始的 
    $("#subnav .black").find("li").eq(0).click(function () { $('#tt').tabs('select', 0); return false; });
    $("#subnav .black").find("li").eq(1).click(function () { $('#tt').tabs('select', 1); return false; });
    $("#subnav .black").find("li").eq(2).click(function () { $('#tt').tabs('select', 2); return false; });
    $("#subnav .black").find("li").eq(3).click(function () { $('#tt').tabs('select', 3); return false; });
    $("#subnav .black").find("li").eq(4).click(function () { $('#tt').tabs('select', 4); return false; });
    $("#subnav .black").find("li").eq(5).click(function () { $('#tt').tabs('select', 5); return false; });

    /////////////////////////////////////////////////////////////////

    //开始为   订单的下拉选项添加事件   现获取这些下拉选项的包装集   再添加事件
    // load方法： 加载和显示第一页的所有行。如果指定了'param'，它将取代'queryParams'属性。通常可以通过传递一些参数执行一次查询，通过调用这个方法从服务器加载新数据。
    // 此处传入相应的 flag 获取对应状态的订单信息
    $("#allOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 0 }); });   //会员所有订单

    $("#cancelOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 4 }); });   //会员申请取消订单
    
    $("#cancelOkOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 8 }); });   //管理员同意用户取消的订单

    $("#outGoodsOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 3 }); }); //已发货订单

    $("#getGoodsOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 6}); });  //已收货的订单

    $("#okOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 7 }); });       //已完成订单

    $("#returnGoodsOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 9 }); });  //申请退货订单
    
    $("#agreeReturnGoodsOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 10 }); });  //管理员同意退货

    $("#successReturnGoodsOrder").click(function () { $('#dgforuserorder').datagrid('load', { flag: 12 }); });  //退货已完成订单

    $('#tt').tabs({
        onSelect: function (title) {        // onSelect事件在用户在选择一个选项卡面板的时候触发。参数 title 为选项卡标题
            if (title == "我的订单") {              //用户选择一个选项卡面板的时候根据 选项卡标题 判断 去调用相应的函数实现数据展现
                selectMyOrders();
            } else if (title == "我的评论") {
                selectMyComments();
            } else if (title == "我的积分") {
                selectMyScores();
            } else if (title == "我的收藏") {
                selectMyColletions();
            } else if (title == "我的签到") {
                selectMyRegistration();
            } else if (title == "积分历史") {
                selectMyScoreHistories();
            }
        }
    });
});

//显示当前用户的所有订单  对应菜单 我的订单
function selectMyOrders() {
    $('#dgforuserorder').datagrid({
        url: '/user/GetOrder',
        queryParams: {
            'orderid': 0
        },
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
        pageNumber: 1,
        pageSize: 5,
        pageList: [5, 10, 15, 20],
        columns: [[
            { field: 'Id', title: '订单编号', align: 'center' },
            {
                field: 'OrderDate', title: '订单日期', align: 'center', formatter: function (value, row, index) {
                    return value.substr(6, 4) + '年' + value.substr(0, 2) + "月" + value.substr(3, 2) + "日" + value.substr(11, 2) + ":" + value.substr(14, 2) + ":" + value.substr(17, 2)
                }
            },
            { field: 'loginId', title: '用户登录名', align: 'center' },
            { field: 'TotalPrice', title: '订单总金额', align: 'center' },
            { field: 'personName', title: '收件人姓名', align: 'center' },
            { field: 'phone', title: '联系电话', align: 'center' },
            { field: 'address', title: '收货地址', align: 'center' },
            { field: 'OrderRemark', title: '订单备注', align: 'center' },
            {
                field: 'flag', title: '订单状态', align: 'center', formatter: function (value, row, index) {
                    if (value == 1) {
                        return '未处理';
                    } else if (value == 2) {
                        return '处理中';
                    } else if (value == 3) {
                        return '已发货';
                    } else if (value == 4) {
                        return '会员取消订单';
                    } else if (value == 5) {
                        return '管理员取消订单';
                    } else if (value == 6) {
                        return '已收货';
                    } else if (value == 7) {
                        return '已完成';
                    } else if (value == 8) {
                        return '订单已取消';
                    } else if (value == 9) {
                        return '申请退货中';
                    } else if (value == 10) {
                        return '管理员确认退货';
                    } else if (value == 11) {
                        return '我方已收货';
                    } else if (value == 12) {
                        return '退货完成';
                    }
                }
            }
        ]],toolbar: [{
            iconCls: 'icon-cancel', text: '取消订单',
            handler: function () {
                detailOrderByAjax(4);
            }
        }, '-', {
            iconCls: 'icon-edit', text: '确认收货',
            handler: function () {
                detailOrderByAjax(6);
            }
        }, '-', {
            iconCls: 'icon-ok', text: '完成订单',
            handler: function () {
                detailOrderByAjax(7);
            }
        }, '-', {
            iconCls: 'icon-edit', text: '退货',
            handler: function () {
                detailOrderByAjax(9);
            }
        }]
    });
}

//显示当前用户的所有评论  对应菜单 我的评论
function selectMyComments() {
    //用户点击我的评论  获得评论信息
    $('#mycomment').datagrid({
        url: '/user/GetComment',
        fitColumns: true,//列宽自适应
        columns: [[
            {
                field: 'ISBN', title: '评论图书图片', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"javascript:void(0)\" onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\"  /></a>";
                }
            },
            { field: 'Comment', title: '评论内容' },
            {
                field: 'Flag', title: '是否评为星级', width: '180', formatter: function (value, row, index) {
                    //这里进行判断  如果value值为 0 则说明 当前用户的这条 评论没有被后台管理员评为星级   数据库中  flag 的定义为
                    //  0为普通评论 1,2,3分别对应被网站设置为（*1,**5,***10）   下面根据 得到的值 在页面上显示 相应的数据
                    //  这里的  星级 图片 同一格式如  0xing.png   1xing.png    可拼接为    value+'xing.png'
                    return '<img src=\"Images/' + value + 'xing.png\" style=\"height:30px;width:auto;\" />';
                }
            },
            {
                field: 'Date', title: '评论时间', formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]],toolbar: [{
            iconCls: 'icon-cancel', text: '删除评论',
            handler: function () {
                btnDeleteComment();
            }
        }]
    });
}

//显示当前用户的积分信息  对应菜单 我的积分
function selectMyScores() {
    // 发送请求获取 积分数
    $.get('/user/GetCurrentAndTotalScore', function (json) { $("#currentScore").html("您的可用积分为：" + json.currentScore); $("#totalScore").html("您的总积分为：" + json.totalScore); }, 'json');
}

//显示当前用户的所有收藏  对应菜单 我的收藏
function selectMyColletions() {
    //用户点击 我的收藏夹时 显示 当前用户所收藏的内容并按照 倒序的方式排序 使用 ajax 方式获取数据 
    $("#mycollection").datagrid({
        url: '/user/GetCollection',
        singleSelect: true,
        height: 500,
        fitColumns: true,//列宽自适应
        columns: [[
            { field: 'Id', title: '编号', align: 'center', align: 'center' },
            {
                field: 'ISBN', title: '图书图片', width: '180', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"javascript:void(0)\"  onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" onmouseover=\"$(function () {$(&apos;." + value + "&apos;).height(150).width(120);})\" onmouseout=\"$(function () {$(&apos;." + value + "&apos;).height(100).width(77);})\" /></a>";
                }
            },
            { field: 'booktitle', title: '书名' },
            {
                field: 'CollectTime', title: '收藏时间', formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]], toolbar: [{
            iconCls: 'icon-cancel', text: '取消收藏',
            handler: function () {
                btnDeleteCollections();
            }
        }]
    });
}

//显示当前用户的所有签到  对应菜单 我的签到
function selectMyRegistration() {
    //用户点击我的签到  获得签到信息
    $('#myregistration').datagrid({
        url: '/user/ResiRegistration_list',
        fitColumns: true,//列宽自适应
        columns: [[
            { field: 'Id', title: '编号', align: 'center' },
            { field: 'Content', title: '签到内容', },
            {
                field: 'RegistrationTime', title: '签到时间', formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }

        ]]
    });
}

//显示当前用户的积分历史  对应菜单 积分历史
function selectMyScoreHistories() {
    //用户点击 积分历史 显示 当前用户所收藏的内容并按照 倒序的方式排序 使用 ajax 方式获取数据 
    $("#myscorehistory").datagrid({
        url: '/user/ShowScoreHistory',
        singleSelect: true,
        fitColumns: true,//列宽自适应
        sortName: "Id", //默认排序列
        sortOrder: "asc", //排序顺序，asc 或者desc
        columns: [[
            { field: 'Id', title: '编号', align: 'center', width: 40 },
            {
                field: 'Descriptions', title: '详情', width: 330, height: 200, align: 'center', formatter: function (value, row, index) {

                    return "<textarea style=\"width:300px;readonly:'readonly';\">" + value + "</textarea>";
                }
            },
            {
                field: 'SingleScore', align: 'center', title: '积分变化', width: 60, formatter: function (value, row, index) {
                    if (value > 0) {
                        return "+" + value + "分"
                    } else {
                        return value + "分"
                    }
                }
            },
            {
                field: 'CreateTime', title: '创建时间', width: 60, formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]]
    });
}

//用户取消收藏操作
function btnDeleteCollections() {
    var obj = $("#mycollection").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
    if (obj == null) {
        $.messager.alert('操作提示', '请您选择您的一条收藏！', 'info');
    }
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm("操作提示", "您确定要取消书名为《" + obj.booktitle + "》的收藏吗？", function (data) {
        if (data) {
            //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
            $.get("/user/DeleteCollections", { 'collectionsid': obj.Id }, function (json) {
                if (json.success == 0) {   //说名删除操作成功
                    $('#mycollection').datagrid('reload');
                } else {
                    $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                }
            }, 'json');
        }
    });
}

//用户删除评论操作
function btnDeleteComment() {
    var obj = $("#mycomment").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
    if (obj == null) {
        $.messager.alert('操作提示', '请您选择您的一条评论！', 'info');
    }
    $.messager.defaults = { ok: "是", cancel: "否" };
    $.messager.confirm("操作提示", "您确定要删除评论标题为《" + obj.Title + "》的评论吗？", function (data) {
        if (data) {
            //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
            $.get("/user/DeleteComment", { 'commentid': obj.Id }, function (json) {
                if (json.success == 0) {   //说名删除操作成功
                    $('#mycomment').datagrid('reload');
                } else {
                    $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                }
            }, 'json');
        }
    });
}

//用户点击签到按钮时触发此函数
function btnUserRegistration(){
    $(".txt").text("发表");
    $("#hehe").html("说点什么吧，每日签到可获得积分哦~~~");
    //鼠标获取焦点是 文本框为空  失去焦点是 若 文本框中没有用户输入的签到内容 则恢复原来的文字
    $("#hehe").focus(function () {
        if ($(this).html() == "说点什么吧，每日签到可获得积分哦~~~") {
            $(this).html("");
        }
    }).blur(function () {
        if ($(this).html() == "") {
            $(this).html("说点什么吧，每日签到可获得积分哦~~~");
        }
    });
    $('#w').window({
        title: '发表动态',
        minimizable: false,     //将一些不要的按钮统统不显示
        maximizable: false,
        closable: false,
        collapsible: false,
    });
    $("#commentorregistration").attr("onclick", "btnUserOkRegistration()");
    $('#w').window('open');
}

//用户点击 发表签到内容时 触发此函数
function btnUserOkRegistration() {
    $.getJSON('/user/UserRegistration', { 'content': $('#hehe').html() }, function (json) {
        if (json.success==0) {
            $('#w').window('close');
            $.messager.alert('成功提示', '签到成功!!! ！', 'info');
            getRegistCount();   //再次请求获取签到天数显示在页面上
        } if (json.success == 1) {
            $('#w').window('close');
            $.messager.alert('成功提示', '签到失败!!! 您今天已经签到过了，明天再来吧 ！', 'error');
            getRegistCount();   //再次请求获取签到天数显示在页面上
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '签到失败，请联系管路远!', 'error');
        }
    });
}

//用户点击 发表对订单中图书评论内容时 触发此函数
function btnUserOkComments() {
    var obj = $("#dgfororder").datagrid("getSelected");
    $.getJSON('/user/UserComment', { 'bookid': obj.BookID, 'content': $("#hehe").html() }, function (json) {
        if (json.success == 0) {
            $('#w').window('close');
            $.messager.alert('成功提示', '评论发表成功！', 'info');
        } else {
            $('#w').window('close');
            $.messager.alert('错误提示', '评论发表失败，请联系管理员!', 'error');
        }
    });
}

//修改会员信息方法
function btnUpdateUserMessage() {
    $('#myform').form('submit', {
        url: '/user/UpdateUserMessage',
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object    
            if (data.success==0) {
                $.messager.alert('成功提示', '会员信息修改成功!!! ！', 'ok');
                location.href = '/users';
            }
        }
    });
}

//获取用户的 签到天数
function getRegistCount() {
    var datetime = new Date();
    $.get("/user/GetRegistrationCount", { 'year': datetime.getFullYear(), 'month': datetime.getMonth(), 'day': datetime.getDate() }, function (json) {
        var date = json.lianxuCount
        $("#qiandaocount").html(date);
    }, "json");
}

//轮换图片
function changeImg() {
    var index = 0;
    var stop = false;
    var $li = $("#content1").find("#scroll_img").children("li");
    var $page = $("#content1").find("#scroll_number").children("li");
    $page.eq(index).addClass("scroll_number_over").stop(true, true).siblings().removeClass("scroll_number_over");
    $page.mouseover(function () {
        stop = true;
        index = $page.index($(this));
        $li.eq(index).stop(true, true).fadeIn().siblings().fadeOut();
        $(this).addClass("scroll_number_over").stop(true, true).siblings().removeClass("scroll_number_over");
    }).mouseout(function () {
        stop = false;
    });
    setInterval(function () {
        if (stop) return;
        index++;
        if (index >= $li.length) {
            index = 0;
        }
        $li.eq(index).stop(true, true).fadeIn().siblings().fadeOut();
        $page.eq(index).addClass("scroll_number_over").stop(true, true).siblings().removeClass("scroll_number_over");
    }, 3000);
}

//秒杀信息循环垂直向上滚动
function movedome() {
    var marginTop = 0;
    var stop = false;
    var interval = setInterval(function () {
        if (stop) return;
        $("#express").children("li").first().animate({ "margin-top": marginTop-- }, 0, function () {
            var $first = $(this);
            if (!$first.is(":animated")) {
                if ((-marginTop) > $first.height()) {
                    $first.css({ "margin-top": 0 }).appendTo($("#express"));
                    marginTop = 0;
                }
            }
        });
    }, 50);
    $("#express").mouseover(function () {
        stop = true;
    }).mouseout(function () {
        stop = false;
    });
}

//获取订单详情方法
function detailOrderByAjax(flag) {
    var obj = $("#dgforuserorder").datagrid("getSelected");
    if (obj == null) {
        $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
    } else {
        if(flag==4 && obj.flag!=1){
            $.messager.alert('操作提示', '您当前选中操作的订单无法进行取消订单操作，只有订单状态为未处理的订单才可以执行该操作！', 'warning');
        } else if (flag == 6 && obj.flag != 3) {
            $.messager.alert('操作提示', '您当前选中操作的订单无法进行确认收货操作，只有订单状态为已发货的订单才可以执行该操作！', 'warning');
        } else if (flag == 7 && obj.flag != 6) {
            $.messager.alert('操作提示', '您当前选中操作的订单无法进行已完成订单操作，只有订单状态为已收货的订单才可以执行该操作！', 'warning');
        } else if (flag == 9 && obj.flag != 7) {
            $.messager.alert('操作提示', '您当前选中操作的订单无法进行退货操作，只有订单状态为已完成的订单才可以执行该操作！', 'warning');
        } else {
            $("#OrderId").textbox('setValue', obj.Id);
            $('#dgfororder').datagrid({
                queryParams: {
                    orderId: $("#OrderId").val(),
                }
            });
            if (flag == 4) {
                $("#btnConfirmOrder").linkbutton({ text: '取消订单' });
            }
            if (flag == 9) {
                $("#btnConfirmOrder").linkbutton({ text: '确认退货' });
            }
            if (flag == 6) {
                $("#btnConfirmOrder").linkbutton({ text: '确认收货' });
            }
            if (flag == 7) {
                $("#btnConfirmOrder").linkbutton({ text: '完成订单' });
                $('#dgfororder').datagrid({
                    toolbar: [{
                        iconCls: 'icon-edit', text: '评论商品',
                        handler: function () {
                            var obj = $("#dgfororder").datagrid("getSelected");
                            if (obj == null) {
                                $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                            } else {
                                //$('#w').window('title','您正在对《' + obj.title + '》进行评论');
                                $(".txt").text("评论");
                                $("#hehe").html("评论就有机会获得积分哦~~~");
                                //鼠标获取焦点是 文本框为空  失去焦点是 若 文本框中没有用户输入的签到内容 则恢复原来的文字
                                $("#hehe").focus(function () {
                                    if ($(this).html() == "评论就有机会获得积分哦~~~") {
                                        $(this).html("");
                                    }
                                }).blur(function () {
                                    if ($(this).html() == "") {
                                        $(this).html("评论就有机会获得积分哦~~~");
                                    }
                                });
                                $('#w').window({
                                    title: '您正在对《' + obj.title + '》进行评论',
                                    minimizable: false,     //将一些不要的按钮统统不显示
                                    maximizable: false,
                                    closable: false,
                                    collapsible: false,
                                });
                                $("#commentorregistration").attr("onclick", "btnUserOkComments()");
                                $('#w').window('open');
                            }
                        }
                    }]
                });
            }
            $("#btnConfirmOrder").attr("onclick", "changeOrderFlagByAjax(" + flag + ")");
            $("#windowforOrderManager").window('open');
        }
    }
    
}

//改变订单状态方法
function changeOrderFlagByAjax(flag) {
    var obj = $("#dgforuserorder").datagrid("getSelected");
    $.messager.defaults = { ok: "是", cancel: "否" };
    var content = "";
    if (flag == 4) {
        content = "您确定要取消订单号为 " + obj.Id + " 的订单吗？";
    }
    if (flag == 9) {
        content = "您确定要对订单号为 " + obj.Id + " 的订单进行退货操作吗？";
    }
    if (flag == 6) {
        content = "您确定要对 " + obj.Id + " 的订单进行已收货操作吗？ 一旦确认您支付的金额将打入商家账户,请谨慎操作,以免造成人财两空的后果！！！";
    }
    if (flag == 7) {
        content = "您确定要对 " + obj.Id + " 的订单进行已完成操作吗？";
    }
    $.messager.confirm("操作提示", content, function (data) {
        if (data) {
            $.get('/user/UserUpdateOrderFlag', { 'orderid': obj.Id,'flag':flag }, function (json) {
                if (flag == 4) {
                    if (json.success == 0) {   
                        $.messager.alert('成功提示', '订单号为 ' + obj.Id + ' 的订单取消订单请求已提交，需等待管理员同意请求才可以取消订单！！！', 'info');
                        $('#dgforuserorder').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 1) {
                        $.messager.alert('错误提示', '您当前操作的订单已经被管理员处理，无法申请取消订单!', 'error');
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 0) {
                        $.messager.alert('错误提示', '取消订单失败，请联系管理员!', 'error');
                        $("#windowforOrderManager").window('close');
                    }
                }
                if (flag == 9) {    ///此时的状况为 退货处理
                    if (json.success == 0) {   
                        $.messager.alert('成功提示', '订单号为 '+obj.Id+' 的订单退货请求已提交，需等待管理员同意请求才可以处理退货操作！！！', 'info');
                        $('#dgforuserorder').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 1) {
                        $.messager.alert('错误提示', '订单号为 ' + obj.Id + ' 的订单并未处于已完成状态，无法申请退货处理!', 'error');
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 0) {
                        $.messager.alert('错误提示', '退货操作请求失败，请联系管理员!', 'error');
                        $("#windowforOrderManager").window('close');
                    }
                }
                if (flag == 6) {    ///此时的状况为 一手货处理
                    if (json.success == 0) {
                        $.messager.alert('成功提示', '订单号为 ' + obj.Id + ' 的订单已收货操作完成！！！', 'info');
                        $('#dgforuserorder').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 1) {
                        $.messager.alert('错误提示', '订单号为 ' + obj.Id + ' 的订单并未处于已发货状态，无法进行已收货操作!', 'error');
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 0) {
                        $.messager.alert('错误提示', '已收货操作请求失败，请联系管理员!', 'error');
                        $("#windowforOrderManager").window('close');
                    }
                }
                if (flag == 7) {    ///此时的状况为 订单已完成处理
                    if (json.success == 0) {
                        $.messager.alert('成功提示', '订单号为 ' + obj.Id + ' 的订单已完成！！！', 'info');
                        $('#dgforuserorder').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 1) {
                        $.messager.alert('错误提示', '订单号为 ' + obj.Id + ' 的订单并未处于已收货状态，无法进行已完成操作!', 'error');
                        $("#windowforOrderManager").window('close');
                    } else if (json.success == 0) {
                        $.messager.alert('错误提示', '订单已完成操作请求失败，请联系管理员!', 'error');
                        $("#windowforOrderManager").window('close');
                    }
                }
            }, 'json');
        }
    });
}

//订单详情页面显示订单状态调用此函数   --》 <th data-options="field:'flag',formatter:formatFlag">订单状态</th>
function formatFlag(value, row) {
    if (value == 1) {
        return '<span style="color:red">未处理</span>';
    } else if (value == 2) {
        return '<span style="color:#ff6a00">处理中</span>';
    } else if (value == 3) {
        return '<span style="color:#00ff21">已发货</span>';
    } else if (value == 4) {
        return '<span style="color:#0094ff">会员取消订单</span>';
    } else if (value == 5) {
        return '<span style="color:#fb6262">管理员取消订单</span>';
    } else if (value == 6) {
        return '<span style="color:#ff6a00">已收货</span>';
    } else if (value == 7) {
        return '<span style="color:green">已完成</span>';
    } else if (value == 8) {
        return '<span style="color:#ff006e">订单已取消</span>';
    } else if (value == 9) {
        return '<span style="color:#cf3e3e">申请退货中</span>';
    } else if (value == 10) {
        return '<span style="color:#4800ff">管理员确认退货</span>';
    } else if (value == 11) {
        return '<span style="color:#342727">我方已收货</span>';
    } else if (value == 12) {
        return '<span style="color:#b200ff">退货完成</span>';
    }
}

//订单详情页面显示图片调用此函数       --》<th data-options="field:'bookImg',formatter:formatPicture">图书图片</th>
function formatPicture(val, row) {
    return "<img  src=\"/Images/BookCovers/" + val + ".jpg\" style=\"height:100px;width:80px;\" />";
}

//订单详情页面显示购买方式调用此函数   --》<th data-options="field:'flag',formatter:formatFlagForBuyWay">购买方式</th>
function formatFlagForBuyWay(val, row) {
    if (val == 0) {
        return '<span style="color:blue">普通订单</span>';
    } else if (val == 1) {
        return '<span style="color:#ff6a00">秒杀订单</span>';
    } else if (val == 2) {
        return '<span style="color:#00ff21">清仓订单</span>';
    }
}
