/*
    
    作者:龚涛  2016-12-8

    这是 User 控制器下的 大部分页面中  实际调用的 js 函数 大部分 都在这里

*/


$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });

    //调用 滚动广告函数
    changeImg();

    //调用滚动 秒杀信息
    movedome();

    //打开页面时关闭 模态的 签到窗口
    $('#w').window('close');


    //这个是为 我的积分按钮 添加 下拉选项
    var p = $('#tt').tabs().tabs('tabs')[2];
    var mb = p.panel('options').tab.find('a.tabs-inner');
    mb.menubutton({
        menu: '#mmforscore'
    }).click(function () {
        $('#tt').tabs('select', 2);
    });

     

    //签到页面显示 星期几
    var week = new Array("日", "一", "二", "三", "四", "五", "六");
    $("#week").html("星期" + week[new Date().getDay()]);

    //获取签到天数
    getRegistCount();

    //显示当前系统时间
    $("#time").html("今天是" + new Date().getFullYear() + "年" + new Date().getMonth() + "月" + new Date().getDate() + "日,连续签到可以获得大量积分  ");

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
    $("#hehe").html("说点什么吧，每日签到可获得积分哦~~~");
    //用户在签到窗口点击取消按钮
    $("#idEditorCancel").click(function () {
        $('#w').window('close');
    });

    //用户在签到点击发表 使用  ajax 方式  这里添加一个条件即在发表签到之前 发送一个  ajax 请求获得数据库最近一次签到的时间 如果
    // 和当前时间相差小于 24 小时  则直接不用请求 签到的那个action 直接给出一句话  一天之内不能连续签到，明天再来吧
    $("#idEditorPublishBtn").click(function () {
        var datetime=new Date();        
        $.get('/user/GetRegistrationCount', {'year': datetime.getFullYear(),'month':datetime.getMonth(),'day':datetime.getDate()}, function (json) {
            var flag;   //定义天数
            if (json.success == 0) {
                flag = 0;                      //从第一天开始
               
            } else {
                flag = json.lianxuCount;        //获取连续签到天数
            }
            $.get('/user/GetTheNewResiRegistrationTime', function (json) {
                if (json.success == -1) {
                    var value = $("#hehe").html();
                    var userid = 3;
                    var url = "/user/ResiRegistration";
                    var count = flag + 1;
                    var data = { "content": value, "flag": count }; //这里传入 两个参数  签到内容   和    连续签到天数
                    $.get(url, data, function (json) {
                        var score = (count - 1) * 2 + 1;
                        //json {"success":0 } 0代表成功，1代表失败
                        if (json.success == 0) {
                            $('#w').window('close');    //关闭模态窗口
                            $.get('/user/AddScore', { "score": score }, function (json) {
                                if (json.success == 0) {
                                    $.get('/user/AddScoreHistory', { 'flag': 1, 'singleScore': score, 'descriptions': '这是你于' + datetime.getFullYear() + '年' + (datetime.getMonth() + 1) + '月' + datetime.getDate() +'日'+ datetime.getHours() +'时'+ datetime.getMinutes() +'分'+ datetime.getSeconds() + '秒执行的签到 并且连续第' + count + '天签到，积分加了' + score + "分" }, function (json) {
                                        if (json.success == 0) {
                                            $.messager.alert('成功提示', '签到成功!!! 你是连续第' + count + '天签到，积分加' + score + '！', 'info');
                                            //再次请求获取签到天数显示在页面上
                                            getRegistCount();
                                        } else {
                                            $.messager.alert('错误提示', '积分历史记录添加异常，请联系管理员!', 'error');
                                        }
                                    }, 'json');
                                    
                                } else {
                                    $.messager.alert('错误提示', '积分添加异常，请联系管理员!', 'error');
                                }
                            }, "json");
                        }
                        else {
                            $('#w').window('close');
                            $.messager.alert('错误提示', '签到失败，请联系管理员!', 'error');
                        }
                    }, "json");
                    return;
                }
                
                //获取当前系统时间
                var date = new Date();
                //将服务端传回的 年月日 数据与当前系统时间进行比较     这里操蛋的是 javascript date 对象的 月份从0开始的 切记
                if (date.getDate() == json.date && date.getMonth() + 1 == json.month && date.getFullYear() == json.year) {
                    $('#w').window('close');    //关闭模态窗口
                    $.messager.alert('错误提示', '一天内只能签到一次!明天再来吧！！！', 'error');
                } else {
                    var value = $("#hehe").html();
                    var url = "/user/ResiRegistration";
                    var count = flag + 1;
                    var data = { "content": value, "flag": count };
                    $.get(url, data, function (json) {
                        //json {"success":0 } 0代表成功，1代表失败
                        if (json.success == 0) {
                            $('#w').window('close');    //关闭模态窗口
                            var score = (count - 1) * 2 + 1;
                            $.get('/user/AddScore', { "score": score }, function (json) {
                                if (json.success == 0) {
                                    $.messager.alert('成功提示', '签到成功!!! 你是连续第' + count + '天签到，积分加' + score + '！', 'info');
                                    //再次请求获取签到天数显示在页面上
                                    getRegistCount();
                                } else {
                                    $.messager.alert('错误提示', '签到失败，请联系管路远!', 'error');
                                }
                            }, "json");
                        }
                        else {
                            $('#w').window('close');
                            $.messager.alert('错误提示', '签到失败!', 'error');
                        }
                    }, "json");
                }
            }, "json");
        } ,'json');
    });



/////////////////////////////// 从这里开始  对左导航中的 li 添加他们的事件   即实现点击相应 的 li 触发 click 时间   使 tabs 中的内容切换至 li 标题相对应的位置/////////////////////  

    //    tabs 的顺序 是从 0 开始的 
    $("#subnav .black").find("li").eq(0).click(function () { $tabs.tabs('select', 0); return false; });
    $("#subnav .black").find("li").eq(1).click(function () { $tabs.tabs('select', 1); return false; });
    $("#subnav .black").find("li").eq(2).click(function () { $tabs.tabs('select', 2); return false; });
    $("#subnav .black").find("li").eq(3).click(function () { $tabs.tabs('select', 3); return false; });
    $("#subnav .black").find("li").eq(4).click(function () { $tabs.tabs('select', 4); return false; });
    $("#subnav .black").find("li").eq(5).click(function () { $tabs.tabs('select', 5); return false; });


/////////////////////////////////////////////////////////////////


    //开始为   订单的下拉选项添加事件   现获取这些下拉选项的包装集   再添加事件
    $("#cancelOrder").click(function () { $('#dg').datagrid('load', {flag:1}); });

    $("#removeOrder").click(function () { $('#dg').datagrid('load', { flag: 2 }); });

    $("#okOrder").click(function () { $('#dg').datagrid('load', { flag: 3 }); });

    $("#returnmoneyOrder").click(function () { $('#dg').datagrid('load', { flag: 4 }); });

    


    //用户点击我的签到  获得签到信息
    $('#myregistration').datagrid({
        url: '/user/ResiRegistration_list',
        fitColumns: true,//列宽自适应
        columns: [[
            { field: 'Id', title: '编号',  align: 'center' },
            { field: 'Content', title: '签到内容', },
            {
                field: 'RegistrationTime', title: '签到时间',  formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }

        ]]
    });

    

    //用户点击我的评论  获得评论信息
    $('#mycomment').datagrid({
        url: '/user/GetComment',
        fitColumns: true,//列宽自适应
        columns: [[
            { field: 'ISBN', title: '评论图书图片',  align: 'center', formatter: function (value, row, index) {
                return "<a href=\"javascript:void(0)\" onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\"  /></a>";
            }
            },
            { field: 'Comment', title: '评论内容' },
            { field: 'Flag', title: '是否评为星级',width:'180', formatter: function (value, row, index) {
                //这里进行判断  如果value值为 0 则说明 当前用户的这条 评论没有被后台管理员评为星级   数据库中  flag 的定义为
                //  0为普通评论 1,2,3分别对应被网站设置为（*1,**5,***10）   下面根据 得到的值 在页面上显示 相应的数据
                //  这里的  星级 图片 同一格式如  0xing.png   1xing.png    可拼接为    value+'xing.png'
                return '<img src=\"Images/' + value + 'xing.png\" style=\"height:30px;width:auto;\" />';
                }
            },
            { field: 'Date', title: '评论时间', formatter: function (value, row, index) {
                var d = eval("new " + value.replace(/\//g, ""));
                return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]]
    });

    //用户点击 我的收藏夹时 显示 当前用户所收藏的内容并按照 倒序的方式排序 使用 ajax 方式获取数据 
    $("#mycollection").datagrid({
        url: '/user/GetCollection',
        singleSelect: true,
        height:500,
        fitColumns: true,//列宽自适应
        columns: [[
            { field: 'Id', title: '编号', align: 'center', align: 'center' },
            { field: 'ISBN', title: '图书图片',width:'180', align: 'center', formatter: function (value, row, index) {
                return "<a href=\"javascript:void(0)\"  onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" onmouseover=\"$(function () {$(&apos;." + value + "&apos;).height(150).width(120);})\" onmouseout=\"$(function () {$(&apos;." + value + "&apos;).height(100).width(77);})\" /></a>";
            }
            },
            { field: 'booktitle', title: '书名'},
            {
                field: 'CollectTime', title: '收藏时间',  formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]]
    });
    
    //用户点击 积分历史 显示 当前用户所收藏的内容并按照 倒序的方式排序 使用 ajax 方式获取数据 
    $("#myscorehistory").datagrid({
        url: '/user/ShowScoreHistory',
        singleSelect: true,
        fitColumns: true,//列宽自适应
        sortName: "Id", //默认排序列
        sortOrder: "asc", //排序顺序，asc 或者desc
        columns: [[
            { field: 'Id', title: '编号',align:'center', width: 40},
            {
                field: 'Descriptions', title: '详情', width: 330,height:200,align: 'center' , formatter: function (value, row, index) {
                    
                    return "<textarea style=\"width:300px;readonly:'readonly';\">"+value+"</textarea>";
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
                field: 'CreateTime', title: '创建时间', width:60, formatter: function (value, row, index) {
                    var d = eval("new " + value.replace(/\//g, ""));
                    return "" + d.getFullYear() + "年" + (d.getMonth() + 1) + "月" + d.getDate() + "日";
                }
            }
        ]]
    });
    // 发送请求获取 积分数
    $.get('/user/GetCurrentAndTotalScore', function (json) { $("#currentScore").html("您的可用积分为：" + json.currentScore); $("#totalScore").html("您的总积分为：" + json.totalScore); }, 'json');

    ////展开行以显示该行的详细信息
    //$('#dg').datagrid({
    //    view: detailview,
    //    fitColumns: true,//列宽自适应
    //    detailFormatter: function (index, row) {
    //        return '<div class="ddv" style="height:300px;"></div>';
    //    },
    //    onExpandRow: function (index, row) {
    //        var ddv = $(this).datagrid('getRowDetail', index).find('div.ddv');
    //        ddv.propertygrid({    
    //            url: '/user/GetOrderDetail',
    //            queryParams: {
    //                orderid: row.Id
    //            },
    //            showGroup: true,    
    //            scrollbarSize: 0    
    //        });  
    //        $('#dg').datagrid('fixDetailRowHeight', index);
    //    }
    //});


    //用户点击取消订单按钮对  订单进行操作是 触发
    $("#btnCancelOrders").click(function () {
        //这是调用方法，名字叫getSelected，该方法无参数
        var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
        if (obj == null) {
            $.messager.alert('操作提示', '请您选择一条订单信息！', 'info');
        }
        $.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm("操作提示", "您确定要取消订单号为"+obj.Id+"的订单吗？", function (data) {
            if (data) {
                alert("是");
            }
        });
    });
    
    //用户点击确认收获时 即 确认订单时 触发此函数
    $("#btnOkOrder").click(function () {
        //这是调用方法，名字叫getSelected，该方法无参数
        var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
        if (obj == null) {
            $.messager.alert('操作提示', '请您选择一条订单信息！', 'info');
        }
        $.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm("操作提示", "你确定完成订单号" + obj.Id + "的订单吗？确认后支付金额将打入商家账户,请确认收货后点击确认订单！！！", function (data) {
            if (data) {
                alert("是");
            }
        });
    });
    
    //用户点击退单按钮对  订单进行操作时 触发
    $("#btnRemoveOrder").click(function () {
        //这是调用方法，名字叫getSelected，该方法无参数
        var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
        if (obj == null) {
            $.messager.alert('操作提示', '请您选择一条订单信息！', 'info');
        }
        $.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm("操作提示", "您确定要退掉订单号为" + obj.Id + "的订单吗？", function (data) {
            if (data) {
                alert("是");
            }
        });
    });
    
    //用户点击删除评论时  触发该函数
    $("#btnDeleteComment").click(function () {
        //这是调用方法，名字叫getSelected，该方法无参数
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
    });
    //用户点击取消收藏时  触发该函数
    $("#btnDeleteCollections").click(function () {
        //这是调用方法，名字叫getSelected，该方法无参数
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
    });
    
    
    var $tabs = $('#tt').tabs(); // 第一个标签被选中
    $('#btnSearchOrder').click(function () { // 绑定单击事件
        //$("#dg").attr("data-options", "queryParams: {title:'" + $("#searchBook").val() + "' }");      想要改变 附加的url参数 这也是一个可行的办法   但是实现起来过于麻烦  不如使用  load 访法简单
        //alert($("#dg").attr("data-options"));

        //这里使用 的是 datagrid 的 load 方法    title 为传给后台的 参数即文本框的 val() 值  可以通过此方法实现查询功能  
        $('#dg').datagrid('load', {
            title:$("#searchBook").val()
        });


        
        return false;
    });

    btnGetMyOrder();

});


//用户点击我的订单  获得订单信息
function btnGetMyOrder() {
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
        ]],
        toolbar: [{
            iconCls: 'icon-edit', text: '处理订单',
            handler: function () {
                var obj = $("#dgforuserorder").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $("#OrderId").textbox('setValue', obj.Id);
                $("#OrderDate").textbox('setValue', obj.OrderDate);
                $("#loginId").textbox('setValue', obj.loginId);
                $("#TotalPrice").textbox('setValue', obj.TotalPrice);
                $("#OrderPersonName").textbox('setValue', obj.personName);
                $("#UserPhone").textbox('setValue', obj.phone);
                $("#OrderAddress").textbox('setValue', obj.address);
                $("#OrderRemark").textbox('setValue', obj.OrderRemark);
                $("#OrderFlag").textbox('setValue', obj.flag);
                $('#dgfororder').datagrid({
                    queryParams: {
                        orderId: $("#OrderId").val(),
                    }
                });
                $("#windowforOrderManager").window('open');
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '管理员取消订单',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dgforuserorder").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $("#OrderId").textbox('setValue', obj.Id);
                $("#OrderDate").textbox('setValue', obj.OrderDate);
                $("#loginId").textbox('setValue', obj.loginId);
                $("#TotalPrice").textbox('setValue', obj.TotalPrice);
                $("#OrderPersonName").textbox('setValue', obj.personName);
                $("#UserPhone").textbox('setValue', obj.phone);
                $("#OrderAddress").textbox('setValue', obj.address);
                $("#OrderRemark").textbox('setValue', obj.OrderRemark);
                $("#OrderFlag").textbox('setValue', obj.flag);
                $('#dgfororder').datagrid({
                    queryParams: {
                        orderId: $("#OrderId").val(),
                    }
                });
                $('#dgfororder').datagrid('reload');
                $('#orderButton').linkbutton({ text: '取消订单' });  //再将按钮文字改为修改
                $('#orderButton').attr('onclick', 'adminCancelOrderByAjax()');  //再更改和按钮绑定的函数 让他执行取消订单操作
                $("#windowforOrderManager").window('open');
            }
        }]
    });
}

//获取用户的 签到天数
function getRegistCount() {
    var datetime = new Date();
    $.get("/user/GetRegistrationCount",{'year': datetime.getFullYear(),'month':datetime.getMonth(),'day':datetime.getDate()}, function (json) {
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

//这是  datagrid 中查看订单中 订单编号的 a 标签中的onclick() 时间  btnShowBookDetail(isbn) 传入的值为 图书的isbn号 该函数的目的是通过该 isbn 号码
// 获取图书的 id  从而转到  指定图书的详情页面
function btnShowBookDetail(value) {
    //发送 ajax 请求 后台 通过  orderid 获取 bookid
    $.get('/user/GetBookIdByOrderId', { 'isbn': value }, function (json) {
        if (json.bookid == 0) {
            alert("图书找不到");
        } else {
            location.href = '/books/detail?id=' + json.bookid;
        }
    }, 'json');
}








//验证方法
function validate($dom) {
    var v = $dom.val();
    var id = $dom.attr("id");
    var flag = true;
    switch (id) {
        case "email":
            $("#email_prompt").html("");
            var reg = /^\w+@\w+(\.[a-zA-Z]{2,3}){1,2}$/;
            if (v == "") {
                $("#email_prompt").removeClass().addClass("register_prompt_error").html("电子邮件是必填项，请输入您的Email地址");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else if (reg.test(v) == false) {
                $("#email_prompt").removeClass().addClass("register_prompt_error").html("电子邮件格式不正确，请重新输入");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else {
                $("#email_prompt").removeClass().addClass("register_prompt_ok");
                $dom.removeClass().addClass("register_input");
            }
            break;
        case "Title":
            $("#Title_prompt").html("");
            var reg = /^[a-zA-Z0-9]{4,20}$/;
            if (v == "") {
                $("#Title_prompt").removeClass().addClass("register_prompt_error").html("昵称为必填项，请输入您的昵称");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else if (reg.test(v) == false) {
                $("#Title_prompt").removeClass().addClass("register_prompt_error").html("昵称格式错误，请用大小写英文字母、数字，长度4-20个字符");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else {
                $("#Title_prompt").removeClass().addClass("register_prompt_ok");
                $dom.removeClass().addClass("register_input");
            }
            break;
        case "pwd":
            $("#pwd_prompt").html("");
            var reg = /^[a-zA-Z0-9]{6,20}$/;
            if (v == "") {
                $("#pwd_prompt").removeClass().addClass("register_prompt_error").html("密码为必填项，请设置您的密码");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else if (reg.test(v) == false) {
                $("#pwd_prompt").removeClass().addClass("register_prompt_error").html("密码格式错误，请用大小写英文字母、数字，长度6-20个字符");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else {
                $("#pwd_prompt").removeClass().addClass("register_prompt_ok");
                $dom.removeClass().addClass("register_input");
            }
            break;
        case "repwd":
            $("#repwd_prompt").html("");
            if (v == "") {
                $("#repwd_prompt").removeClass().addClass("register_prompt_error").html("请再次输入您的密码");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else if ($("#pwd").val() != v) {
                $("#repwd_prompt").removeClass().addClass("register_prompt_error").html("两次输入密码不一致，请重新输入");
                $dom.removeClass().addClass("register_input register_input_Blur");
                flag = false;
            } else {
                $("#repwd_prompt").removeClass().addClass("register_prompt_ok");
                $dom.removeClass().addClass("register_input");
            }
            break;
        default:
            break;
    }
    return flag;
}





