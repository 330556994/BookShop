//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////这里进入后台 js 脚本////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//作者:四人帮   
//时间:2016/12/9
//描述:对后台控制器的大部分 ajax 方法提供支持,生成后台 home 页面的所有datagrid控件并提供数据展现。
//订单状态共有: 1:未处理 2:处理中 3:已发货 4:会员取消订单 5:管理员取消订单 6:已收货 7:已完成 8:订单已取消 9:申请退货中 10:管理员确认退货 11:我方已收货 12:退货完成  

//启动函数
$(function () {
    //先禁用浏览器缓存
    $.ajaxSetup({
        cache: false
    });

    //这里为查询区添加 Tabs（选项卡）
    $('#ttforseach').tabs({
        border: false,
        onSelect: function (title) {
        }
    });

    getMenuCount(); //调用函数 获得菜单栏中对应的项的数量

});

//左侧菜单栏鼠标悬停函数
function mouserOver(obj) {
    $(obj).height(35)
    $(obj).find("a").css("font-size", "25px").css("color","red");
    $(obj).css("background", "#a18b8b");
}

//左侧菜单栏鼠标移开函数   
function mouseOut(obj) {
    $(obj).height(25)
    $(obj).find("a").css("font-size", "20px").css("color", "blue");
    $(obj).css("background", "#A9FACD");
}

//订单详情页面显示图片调用此函数       --》<th data-options="field:'bookImg',formatter:formatPicture">图书图片</th>
function formatPicture(val, row) {
    return "<img  src=\"/Images/BookCovers/" + val + ".jpg\" style=\"height:100px;width:80px;\" />";
}

//订单详情页面显示订单状态调用此函数   --》<th data-options="field:'flag',formatter:formatFlag">订单状态</th>
function formatFlag(val, row) {
    if (val == 1) {
        return '未处理';
    } else if (val == 2) {
        return '处理中';
    } else if (val == 3) {
        return '已发货';
    } else if (val == 4) {
        return '会员取消订单';
    } else if (val == 5) {
        return '管理员取消订单';
    } else if (val == 6) {
        return '已收货';
    } else if (val == 7) {
        return '已完成';
    } else if (val == 8) {
        return '订单已取消';
    } else if (val == 9) {
        return '申请退货中';
    } else if (val == 10) {
        return '管理员确认退货';
    } else if (val == 11) {
        return '我方已收货';
    } else if (val == 12) {
        return '退货完成';
    }
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

//后台管理员登录操作
function adminLogin() {
    $('#loginForm').form('submit', {      //表单数据通过 ajax 请求发送到后台进行判断
        url: '/admin/Home/AdminLogin',
        onSubmit: function () {
        },
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object   
            if (data.success == 0) {    
                location.reload(true);          //这里重新刷新页面 session 中有值则进入后台欢迎界面
                $.messager.alert('成功提示', '登录成功！！！', 'info');
            } else if (data.success == -1) {
                $.messager.alert('错误提示', '登录失败，请检查用户名或密码!', 'error');
            } 
        }
    });
}

//管理员点击用户注册按钮时触发此函数 替换标题内容 按钮上的文字 和按钮所绑定的 onclick 事件 实现注册功能
function addNewAdmin() {
    $("#loginForm h1").html("注册后台管理员");
    $("#LoginName").focus();
    $("#adminLogin").linkbutton({ text: '注册' });
    $('#adminLogin').attr('onclick', 'adminRegist()');
    $("#adminRegister").linkbutton({ text: '返回登陆' });
    $('#adminRegister').attr('onclick', 'returnLogin()');
}

//管理员点击返回登陆时，触发此函数
function returnLogin() {
    $("#loginForm h1").html("后台管理员登录");
    $("#adminLogin").linkbutton({ text: '登录' });
    $('#adminLogin').attr('onclick', 'adminLogin()');
    $("#adminRegister").linkbutton({ text: '用户注册' });
    $('#adminRegister').attr('onclick', 'addNewAdmin()');
}

//管理员注册操作
function adminRegist() {
    $('#loginForm').form('submit', {
        url: '/admin/Home/adminregister',
        onSubmit: function () {
        },
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object   
            if (data.success == 0) {
                $.messager.alert('成功提示', '注册成功,直接点击登录按钮即可登录当前注册成功的账号！！！', 'info');
                returnLogin();
            } else if (data.success == 1) {
                $.messager.alert('错误提示', '注册失败，该注册名称已被占用!', 'error');
            }
            else if (data.success == -1) {
                $.messager.alert('错误提示', '注册失败，请检查用户名或密码!', 'error');
            }
        }
    });
}

//使用 ajax 管理员进行安全退出操作 即销毁当前的session
function btnAdminExit() {
    $.getJSON("/admin/home/AdminExit", function (json) {
        if (json.success == 0) {
            location.reload(true);
            $.messager.alert('成功提示', '账号已安全退出成功！！！', 'info');
        } else if (data.success == -1) {
            $.messager.alert('错误提示', '账号安全退出失败，请联系管理员!', 'error');
        }
    });
}

///////////////////////////// 以下函数对应后台系统菜单 /////////////////////////////

//图书管理操作
function btnBookManager(obj, title, author, bookid,categoryid,publisherid) {
    $('#allPage').layout('collapse', 'east');       //不管查询区是否打开 在加载 datagrid 时都执行关闭操作
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->基础数据管理  -->图书管理')  //将操作区标题改为对应的系统菜单值
    $(obj).parent().parent().parent().find("a").removeClass("current");     //加载时首先将所有 传入对象的父节点的父节点下的所有 a 标签 移除 .current 样式
    $(obj).addClass("current"); //添加样式 区别于没有点击的菜单项
    $('#IsHaveMessage').hide(); //隐藏 "当前无数据" 的提示
    //使用Javascript方式创建DataGrid控件  并请求相应的控制器返回json数据以显示在 DataGrid 中
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view, //由于共用了一个 datagrid 所以一加载就设置 view 为默认样式
        url: '/admin/Home/listforbook',    //请求的 url
        queryParams: {
            'Title': title,                //请求参数
            'Author': author,       
            'Id': bookid,
            'CategoryId': categoryid,
            'PublisherId':publisherid
        },
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        sortName: "Id",
        sortOrder: "desc",
        pagination: true,
        loadMsg: '正在玩命加载中,请稍候...',
        pageNumber: 1,
        pageSize: 5,
        pageList: [5, 10, 15, 20],
        pagination:'true',
        columns: [[
            {
                field: 'Id', title: '图书编号',align: 'center', formatter: function (value, row, index) {
                    return "<a  href=\"/books/detail?id=" + value + "\" target=\"_blank\" style=\"color:red;text-decoration:underline;\" >" + value + "</a>";
                }
            },
            {
                field: 'ISBN', title: '评论图书图片', align: 'center', formatter: function (value, row, index) {
                    return "<img  src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" />";
                   // return "<a href=\"javascript:void(0)\"  onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" onclick=\"$(function () {alert(0)})\" onmouseout=\"$(function () {$(&apos;." + value + "&apos;).height(100).width(77);})\" /></a>";

                }
            },
            { field: 'Title', title: '书名', align:'center'  },
            { field: 'Author', title: '作者',  align: 'center' },
            { field: 'Pname', title: '出版社名称',  align: 'center' },
            { field: 'Cname', title: '类别名称', align: 'center' },
            {
                field: 'PublishDate', title: '出版日期', align: 'center', formatter: function (value, row, index) {
                return value.substr(6, 4) + '年' + value.substr(0, 2) + "月" + value.substr(3, 2) + "日";
                }
            },
            {
                field: 'MarketPrice', title: '市场价', align: 'center', formatter: function (value, row, index) {
                    return "￥" + value;
                }
            },
            {
                field: 'UnitPrice', title: '会员价', align: 'center', formatter: function (value, row, index) {
                    return "￥" + value;
                }
            },
            { field: 'Clicks', title: '点击率',align: 'center' },
            {
                field: 'Flag', title: '是否为清仓商品', align: 'center', formatter: function (value, row, index) {
                    //--为了实现清仓功能，在books表中增加 flag 列：0 表示 非清仓商品；  1 表示 清仓商品
                    if (value == 0) {
                        return '非清仓商品';
                    } if (value == 1) {
                        return '<span style="color:red;">清仓商品</span>';
                    }
                }
            },
            {
                field: 'isSeckillGoogs', title: '是否为秒杀商品', align: 'center', formatter: function (value, row, index) {
                    if (value == 0) {
                        return '非秒杀商品';
                    } if (value == 1) {
                        return '<span style="color:red;">秒杀商品</span>';
                    }
                }
            }, {
                field: 'isIntegralGoods', title: '是否为积分兑换商品', align: 'center', formatter: function (value, row, index) {
                    if (value == 0) {
                        return '非积分商品';
                    } if (value == 1) {
                        return '<span style="color:red;">积分商品</span>';
                    }
                }
            }
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-add', text: '添加图书',
            handler: function () {
                //添加图书时  将图书的id的input标签的值置为空
                $("#BookId").val("");
                $('#saveorupdatebook').linkbutton({ text: '保存' });  
                $("#Title").textbox('setValue', '');
                $("#Author").textbox('setValue', '');
                $("#TOC").textbox('setValue', '');
                $("#Pname").textbox('setValue', '');
                $("#PublishDate").textbox('setValue', '');
                $("#ISBN").textbox('setValue', '');
                $("#MarketPrice").textbox('setValue', '');
                $("#UnitPrice").textbox('setValue', '');
                $("#Cname").textbox('setValue', '');
                $("#Clicks").textbox('setValue', '');
                $("#Flag").textbox('setValue', '');
                $("#ClearPrice").textbox('setValue', '');
                $("#ContentDescription").textbox('setValue', '');
                $('#windowforeditbook').window('setTitle', '添加图书');//为打开的模态窗口标题赋值
                $("#windowforeditbook").window('open');
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '删除图书',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除书名为《" + obj.Title + "》的图书吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletebookbyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '修改图书',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的图书信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    //为表单赋值
                    $("#BookId").val(obj.Id);   //这里的 bookid 隐藏域并没有使用 easyui 的控件，赋值还是使用 .val() 方法
                    $('#saveorupdatebook').linkbutton({ text: '修改' });  //再将按钮文字改为修改
                    $("#Title").textbox('setValue', obj.Title);
                    $("#Author").textbox('setValue', obj.Author);
                    $("#TOC").textbox('setValue', obj.TOC);
                    $("#Pname").textbox('setValue', obj.Pname);
                    $("#PublishDate").textbox('setValue',obj.PublishDate);
                    $("#ISBN").textbox('setValue', obj.ISBN);
                    $("#MarketPrice").textbox('setValue', obj.MarketPrice);
                    $("#UnitPrice").textbox('setValue', obj.UnitPrice);
                    $("#Cname").textbox('setValue', obj.Cname);
                    $("#Clicks").textbox('setValue', obj.Clicks);
                    $("#Flag").textbox('setValue', obj.Flag);
                    $("#ClearPrice").textbox('setValue', obj.ClearPrice);
                    $("#ContentDescription").textbox('setValue', obj.ContentDescription);
                    $('#windowforeditbook').window('setTitle', '修改图书');//为打开的模态窗口标题赋值
                    $("#windowforeditbook").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-addcleargoods', text: '添加图书至清仓活动',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $('#saveorupdatecleargoods').linkbutton({ text: '添加' });  //再将按钮文字改为添加
                    //这里为 窗口的表单赋值时并没有为隐藏域 seckillId 赋值 而在修改操作的这个位置则肤乐至 以区别于修改操作
                    $("#bookId").val(obj.Id);
                    $("#bookcleargoodsflag").val(obj.Flag);
                    $("#bookTitle").val(obj.Title);
                    $("#windowforaddgoodstocleargoods").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-addseckillgoods', text: '添加图书至秒杀活动',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的图书信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $('#saveorupdateseckillgoods').linkbutton({ text: '添加' });  //再将按钮文字改为添加
                    //为表单赋值
                    $("#sysmanidofseckill").val(1); //管理员赋值
                    $("#bookIdforseckill").val(obj.Id);
                    $("#bookTitleforseckill").val(obj.Title);
                    $('#windowforaddgoodstoceckillgoods').window('setTitle', '添加图书至秒杀活动');//为打开的模态窗口标题赋值
                    $("#windowforaddgoodstoceckillgoods").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-addscoregoods', text: '添加图书至积分商品',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的图书信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $('#saveorupdatescoregoods').linkbutton({ text: '添加' });  //再将按钮文字改为添加
                    $("#bookidforintegralgoods").val(obj.Id);
                    $("#booktitleforintegralgoods").val(obj.Title);
                    $("#windowforaddintegralgoods").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-add', text: '打开查询区',
            handler: function () {
                $('#allPage').layout('expand', 'east'); //打开查询区                $('#ttforseach').tabs('select', 0);     //打开查询区时 选中 查询图书的选项卡            }
        }], onLoadSuccess: function (data) {        // datagrid 加载完成时执行
            if (data.total == 0) {                  // 如果 datagrid 中没有数据的话
                $('#IsHaveMessage').show();         // 将在 datagrid空间下方的 <div id="IsHaveMessage" style="display:none"><h1 style="text-align:center">当前没有数据</h1></div> 元素显示
            }
        }
    });
}

//图书类别管理操作
function btnCategoryManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->基础数据管理  -->图书类别管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: detailview,
        url: '/admin/Home/listforcategory',
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
        pageNumber: 1,
        pageSize: 20,
        pageList: [5, 10, 15, 20],
        columns: [[
            { field: 'Id', title: '类别编号', width: '100', align: 'center' },
            { field: 'SortNum', title: 'SORTNUM', width: '100', align: 'center' },
            { field: 'categoryBookCount', title: '所属类别下图书数量', width: '250', align: 'center' },
            { field: 'Name', title: '类别名称', width: '250', align: 'center' }
            
        ]],            
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-add', text: '添加类别',
            handler: function () {
                $("#CategoryId").val("");
                $("#CategoryName").textbox('setValue', '');
                $('#saveorupdatecategory').linkbutton({ text: '保存' });
                $("#windowforeditcategory").window('open');
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '删除类别',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除名称为 " + obj.Name + " 的类别吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletecategorybyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '修改类别',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的类别信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $("#CategoryId").val(obj.Id);
                    $("#CategoryName").textbox('setValue', obj.Name);
                    $('#saveorupdatecategory').linkbutton({ text: '修改' });
                    $("#windowforeditcategory").window('open');
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }, detailFormatter: function (index, row) {
            return '<div style="padding:2px"><table id="ddv-' + index + '"></table></div>';
        }, onExpandRow: function (index, row) {
            $('#ddv-' + index).datagrid({
                url: '/admin/Home/ListForBook',
                queryParams: {
                    categoryid: row.Id,                       //此时传入 的 row.Id 相当于 图书分类编号  datagrid 显示相应订单号的 详细信息
                },
                style: 'width:90%',
                fitColumns: true,
                singleSelect: true,
                autoRowHeight: true,
                pagination: true,
                loadMsg: '正在玩命加载中,请稍候...',
                pageNumber: 1,
                pageSize: 5,
                pageList: [5, 10, 15, 20],
                pagination: 'true',
                columns: [[
                    {
                        field: 'Id', title: '图书编号', align: 'center', formatter: function (value, row, index) {
                            return "<a  href=\"/books/detail?id=" + value + "\" target=\"_blank\" style=\"color:red;text-decoration:underline;\" >" + value + "</a>";
                        }
                    },
                    {
                        field: 'ISBN', title: '评论图书图片', align: 'center', formatter: function (value, row, index) {
                            return "<img  src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" />";
                        }
                    },
                    { field: 'Title', title: '书名', align: 'center' },
                    { field: 'Author', title: '作者', align: 'center' },
                    { field: 'Pname', title: '出版社名称', align: 'center' },
                    { field: 'Cname', title: '类别名称', align: 'center' },
                    {
                        field: 'PublishDate', title: '出版日期', align: 'center', formatter: function (value, row, index) {
                            return value.substr(6, 4) + '年' + value.substr(0, 2) + "月" + value.substr(3, 2) + "日";
                        }
                    },
                    {
                        field: 'MarketPrice', title: '市场价', align: 'center', formatter: function (value, row, index) {
                            return "￥" + value;
                        }
                    },
                    {
                        field: 'UnitPrice', title: '会员价', align: 'center', formatter: function (value, row, index) {
                            return "￥" + value;
                        }
                    },
                    { field: 'Clicks', title: '点击率', align: 'center' },
                    {
                        field: 'Flag', title: '是否为清仓商品', align: 'center', formatter: function (value, row, index) {
                            //--为了实现清仓功能，在books表中增加 flag 列：0 表示 非清仓商品；  1 表示 清仓商品
                            if (value == 0) {
                                return '非清仓商品';
                            } if (value == 1) {
                                return '<span style="color:red;">清仓商品</span>';
                            }
                        }
                    },
                    {
                        field: 'isSeckillGoogs', title: '是否为秒杀商品', align: 'center', formatter: function (value, row, index) {
                            if (value == 0) {
                                return '非秒杀商品';
                            } if (value == 1) {
                                return '<span style="color:red;">秒杀商品</span>';
                            }
                        }
                    }, {
                        field: 'isIntegralGoods', title: '是否为积分兑换商品', align: 'center', formatter: function (value, row, index) {
                            if (value == 0) {
                                return '非积分商品';
                            } if (value == 1) {
                                return '<span style="color:red;">积分商品</span>';
                            }
                        }
                    }
                ]],
                onResize: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        }
    });
}

//出版社管理操作
function btnPublisherManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->基础数据管理  -->出版社管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: detailview,
        url: '/admin/Home/listforpublisher',
        style: 'width:90%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
        sortName: "Id", 
        sortOrder: "desc", 
        pageNumber: 1,
        pageSize: 20,
        pageList: [5, 10, 15, 20],
        columns: [[
            { field: 'Id', title: '出版社编号', align: 'center' },
            { field: 'publisherBookCount', title: '所属图书分类下的图书数量', align: 'center' },
            { field: 'Name', title: '出版社名称', align: 'center' ,width:'300'}
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-add', text: '添加出版社',
            handler: function () {
                $("#publisherId").val("");
                $("#publisherName").textbox('setValue', '');
                $('#saveorupdatepublisher').linkbutton({ text: '添加' });
                $("#windowforeditpublisher").window('open');
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '删除出版社',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除名称为 " + obj.Name + " 的出版社吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletepublisherbyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '修改出版社信息',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的类别信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $("#publisherId").val(obj.Id);
                    $("#publisherName").textbox('setValue', obj.Name);
                    $("#windowforeditpublisher").window('open');
                    $('#saveorupdatepublisher').linkbutton({ text: '修改' });
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }, detailFormatter: function (index, row) {
            return '<div style="padding:2px"><table id="ddv-' + index + '"></table></div>';
        }, onExpandRow: function (index, row) {
            $('#ddv-' + index).datagrid({
                url: '/admin/Home/ListForBook',
                queryParams: {
                    publisherid: row.Id,                       //此时传入 的 row.Id 相当于 图书出版社编号  datagrid 显示相应订单号的 详细信息
                },
                style: 'width:90%',
                fitColumns: true,
                singleSelect: true,
                autoRowHeight: true,
                pagination: true,
                loadMsg: '正在玩命加载中,请稍候...',
                pageNumber: 1,
                pageSize: 5,
                pageList: [5, 10, 15, 20],
                pagination: 'true',
                columns: [[
                    {
                        field: 'Id', title: '图书编号', align: 'center', formatter: function (value, row, index) {
                            return "<a  href=\"/books/detail?id=" + value + "\" target=\"_blank\" style=\"color:red;text-decoration:underline;\" >" + value + "</a>";
                        }
                    },
                    {
                        field: 'ISBN', title: '评论图书图片', align: 'center', formatter: function (value, row, index) {
                            return "<img  src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" />";
                        }
                    },
                    { field: 'Title', title: '书名', align: 'center' },
                    { field: 'Author', title: '作者', align: 'center' },
                    { field: 'Pname', title: '出版社名称', align: 'center' },
                    { field: 'Cname', title: '类别名称', align: 'center' },
                    {
                        field: 'PublishDate', title: '出版日期', align: 'center', formatter: function (value, row, index) {
                            return value.substr(6, 4) + '年' + value.substr(0, 2) + "月" + value.substr(3, 2) + "日";
                        }
                    },
                    {
                        field: 'MarketPrice', title: '市场价', align: 'center', formatter: function (value, row, index) {
                            return "￥" + value;
                        }
                    },
                    {
                        field: 'UnitPrice', title: '会员价', align: 'center', formatter: function (value, row, index) {
                            return "￥" + value;
                        }
                    },
                    { field: 'Clicks', title: '点击率', align: 'center' },
                    {
                        field: 'Flag', title: '是否为清仓商品', align: 'center', formatter: function (value, row, index) {
                            //--为了实现清仓功能，在books表中增加 flag 列：0 表示 非清仓商品；  1 表示 清仓商品
                            if (value == 0) {
                                return '非清仓商品';
                            } if (value == 1) {
                                return '<span style="color:red;">清仓商品</span>';
                            }
                        }
                    },
                    {
                        field: 'isSeckillGoogs', title: '是否为秒杀商品', align: 'center', formatter: function (value, row, index) {
                            if (value == 0) {
                                return '非秒杀商品';
                            } if (value == 1) {
                                return '<span style="color:red;">秒杀商品</span>';
                            }
                        }
                    }, {
                        field: 'isIntegralGoods', title: '是否为积分兑换商品', align: 'center', formatter: function (value, row, index) {
                            if (value == 0) {
                                return '非积分商品';
                            } if (value == 1) {
                                return '<span style="color:red;">积分商品</span>';
                            }
                        }
                    }
                ]],
                onResize: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        }
    });
}

//积分商品管理操作
function btnIntegralGoodsManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->基础数据管理  -->积分商品管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view,
        url: '/admin/Home/ListForIntegralGoods',
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
            { field: 'Id', title: '积分商品编号', align: 'center' },
            { field: 'BookId', title: '图书编号', align: 'center' },
            {
                field: 'bookimg', title: '积分图书图片', align: 'center', formatter: function (value, row, index) {
                    return "<img  src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" />";
                }
            },
            { field: 'booktitle', title: '图书名字', align: 'center' },
            { field: 'PaymentIntegral', title: '兑换积分', align: 'center' },
            { field: 'sectionName', title: '积分段', align: 'center' },
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '从积分商品中移除图书',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从积分商品中移除名称为 " + obj.booktitle + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromIntegralGoods', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '移除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '更改积分商品信息',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                else {
                    $('#saveorupdatescoregoods').linkbutton({ text: '修改' });  //再将按钮文字改为修改
                    $("#IntegralGoodsId").val(obj.Id);
                    $("#booktitleforintegralgoods").val(obj.booktitle);
                    $("#bookidforintegralgoods").val(obj.BookId);
                    //$("#IntegralSectionId").textbox('setValue', obj.sectionName);
                    $("#IntegralSectionId").combobox('setValue', obj.sectionName);  //两种赋值方式都可以
                    $("#PaymentIntegral").textbox('setValue', obj.PaymentIntegral);

                    $("#windowforaddintegralgoods").window('open');
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//会员账号信息管理操作
function btnUserManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->前台会员和评论  -->会员详情管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view,
        url: '/admin/Home/ListForUsers',
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
        pageNumber: 1,
        pageSize: 20,
        pageList: [5, 10, 15, 20],
        columns: [[
            { field: 'Id', title: '编号', align: 'center' },
            { field: 'LoginId', title: '账号', align: 'center'},
            {
                field: 'LoginPwd', title: '密码', align: 'center', formatter: function (value, row, index) {
                    //--为了实现后台管理员冻结账号功能，添加此字段  规则是 0：正常  1：已冻结
                    return "******";
                }
            },
            { field: 'Mail', title: '邮箱', align: 'center' },
            { field: 'Birthday', title: '生日', align: 'center' },
            { field: 'Address', title: '地址', align: 'center' },
            { field: 'UserRoleId', title: '会员等级', align: 'center' },
            { field: 'Name', title: '注册姓名', align: 'center' },
            { field: 'Phone', title: '电话号码', align: 'center' },
            { field: 'RegisterTime', title: '注册时间', align: 'center' },
            { field: 'scoreCurrent', title: '当前积分', align: 'center' },
            { field: 'scoreTotal', title: '历史积分', align: 'center' },
            { field: 'commentCount', title: '会员评论数', align: 'center' },
            { field: 'orderCount', title: '会员订单数', align: 'center' },
            { field: 'collectionsCount', title: '会员收藏数', align: 'center' },
            { field: 'IsFrozen', title: '账号状态', align: 'center', formatter: function (value, row, index) {
                //--为了实现后台管理员冻结账号功能，添加此字段  规则是 0：正常  1：已冻结
                if (value == 0) {
                    return '未冻结';
                } if (value == 1) {
                    return '已冻结';
                }
            }}
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '冻结帐号',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要冻结编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/FrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {   
                                $.messager.alert('成功提示', '会员编号为 '+obj.Id+' 的账号已冻结！！！', 'info');
                                $('#dg').datagrid('reload');    
                            } else if (json.success == 1) {
                                $.messager.alert('错误提示', '该用户账号已经是冻结状态!', 'error');
                            } else {
                                $.messager.alert('错误提示', '冻结账号失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '解封账号',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要解封编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/CancelFrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {   
                                $.messager.alert('成功提示', '解封账号成功！！！', 'info');
                                $('#dg').datagrid('reload');    
                            } else if (json.success == 1) {
                                $.messager.alert('错误提示', '该用户账号并没有被冻结!', 'error');
                            } else {
                                $.messager.alert('错误提示', '解封账号失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '导出Excel表格',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                else {
                    $.getJSON("/admin/home/ExportUserMessageByExcel", function (json) {
                        if (json.success == 0) {
                            alert("导出成功");
                        } else {
                            alert("导出失败");
                        }
                    });
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//会员评论管理操作
function btnReaderCommentManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->前台会员和评论  -->会员评论管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view,
        url: '/admin/Home/listforreadercomment',
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
            { field: 'Id', title: '评论编号', align: 'center' },
            { field: 'Title', title: '评论标题', align: 'center' },
            { field: 'Comment', title: '评论内容', align: 'center' },
            { field: 'Btitle', title: '图书名称', align: 'center' },
            { field: 'Uname', title: '评论会员', align: 'center' },
            {
                field: 'Flag', title: '评论星级', width: 300, formatter: function (value, row, index) {
                    return '<img src=\"/Images/' + value + 'xing.png\" style=\"height:30px;width:auto;\" />';
                }
            },
            { field: 'Date', title: '评论时间' },
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '删除该评论',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除标题为 " + obj.Title + " 的评论吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get("/user/DeleteComment", { 'commentid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '删除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '修改星级',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    //给文本框进行赋值操作
                    $("#commentId").val(obj.Id);
                    $("#UnameforComment").val(obj.Uname);
                    //给下拉框赋上当前选中行的对应值
                    //$("#commentFlag").find("option").eq(obj.Flag).prop("selected","selected");
                    //后来改成使用 easyui 得而下拉框 必须要这样赋值
                    $('#commentFlag').combobox('setValue', obj.Flag);
                    $("#windowforeditusercommentstar").window('open');
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//订单管理操作        
function btnOrderManager(obj, flag, orderid) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->订单管理  -->订单处理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    GetOrderMessageByFlag(flag, orderid);
    $('#dg').datagrid({
        toolbar: [{
            iconCls: 'icon-edit', text: '处理订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 1) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行处理订单操作，只有订单状态为未处理的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行处理订单操作';
                    prepareForDetailOrderBooksListMessage(obj, '处理完成', 'doOrderByAjax()');
                }
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '管理员取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 1) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行管理员取消订单操作，只有订单状态为未处理的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行管理员取消订单操作';
                    prepareForDetailOrderBooksListMessage(obj, '取消订单', 'adminCancelOrderByAjax()', windowTitle);
                }
            }
        }, '-', {
            iconCls: 'icon-edit', text: '发货',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 2) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行发货操作，只有订单状态为处理中的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行发货操作';
                    prepareForDetailOrderBooksListMessage(obj, '发货', 'outGoodsOrderByAjax()', windowTitle);
                }
            }
        }, '-', {
            iconCls: 'icon-edit', text: '同意会员取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 4) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行同意会员取消订单操作，只有订单状态为会员取消订单状态的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + '的订单进行同意会员取消订单操作';
                    prepareForDetailOrderBooksListMessage(obj, '同意取消', 'agreeUserCancelOrderByAjax()');
                }
            }
        }, '-', {
            iconCls: 'icon-edit', text: '同意会员退货请求',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 9) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行管理员同意会员退货请求操作，只有订单状态为会员申请退货中状态的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行同意会员退货请求操作';
                    prepareForDetailOrderBooksListMessage(obj, '同意退货', 'agreeUserReturnGoodsOrderByAjax()', windowTitle);
                }
            }
        }, '-', {
            iconCls: 'icon-edit', text: '退货商品收到',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 10) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行退货商品收到操作，只有订单状态为管理员同意退货状态的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行退货商品收到操作';
                    prepareForDetailOrderBooksListMessage(obj, '确定收到', 'getReturnGoodsOrderByAjax()', windowTitle);
                }
            }
        }, '-', {
            iconCls: 'icon-edit', text: '退货完成',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } if (obj.flag != 11) {
                    $.messager.alert('操作提示', '您当前选中操作的订单无法进行退货完成操作，只有订单状态为退货商品已收到状态的订单才可以执行该操作！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + '的订单进行退货完成操作';
                    prepareForDetailOrderBooksListMessage(obj, '退货完成', 'okReturnGoodsOrderByAjax()', windowTitle);
                }
            }
        }, '-', {
            iconCls: 'icon-add', text: '打开查询区',
            handler: function () {
                $('#allPage').layout('expand', 'east');
                $('#ttforseach').tabs('select', 1);
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//订单配货管理操作
function btnProcessingOrderManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->订单管理  -->订单配货');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    GetOrderMessageByFlag(2, 0);     //此处调用函数 传入参数 2 表示返回的是 处理中状态的订单列表
    $('#dg').datagrid({
        toolbar: [{
            iconCls: 'icon-edit', text: '发货',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行发货操作';
                    prepareForDetailOrderBooksListMessage(obj, '发货', 'outGoodsOrderByAjax()', windowTitle);
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//会员已取消状态订单管理操作
function btnUserCancelOrderManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->订单管理  -->退单处理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    GetOrderMessageByFlag(4, 0);     //此处调用函数 传入参数 4 表示返回的是 会员已取消状态的订单列表
    $('#dg').datagrid({
        toolbar: [{
            iconCls: 'icon-edit', text: '处理用户取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行处理用户取消订单操作';
                    prepareForDetailOrderBooksListMessage(obj, '同意取消', 'agreeUserCancelOrderByAjax()', windowTitle);
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//会员请求退货订单管理操作
function btnUserReturnGoodsOrderManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->订单管理  -->退货处理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    GetOrderMessageByFlag(9, 0);     //此处调用函数 传入参数 9 表示返回的是 会员发出退货请求的订单列表
    $('#dg').datagrid({
        toolbar: [{
            iconCls: 'icon-edit', text: '处理用户退货订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    var windowTitle = '您当前正在对订单号为 ' + obj.Id + ' 的订单进行处理用户退货订单操作';
                    prepareForDetailOrderBooksListMessage(obj, '同意退货', 'agreeUserReturnGoodsOrderByAjax()', windowTitle);
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//秒杀活动管理操作
function btnSeckillActiveManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->销售活动  -->秒杀活动管理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view,
        url: '/admin/Home/listforseckillbooks',
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
            { field: 'Id', title: '活动编号', align: 'center' },
            {
                field: 'BookId', title: '图书编号', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"/books/detail?id=" + value + "\" target=\"_blank\" style=\"color:red;text-decoration:underline;\" >" + value + "</a>";
                }
            },
            { field: 'bookimg', title: '图书图片', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"javascript:void(0)\" onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\"  /></a>";
                }
            },
            { field: 'booktitle', title: '图书标题', align: 'center' },
            {
                field: 'Flag', title: '活动状态', align: 'center', formatter: function (value, row, index) {
                    if (value == 0) {
                        return '未开始';
                    } if (value == 1) {
                        return '已开始';
                    } if (value == 2) {
                        return '已结束';
                    }
                }
            },
            { field: 'BeginTime', title: '活动开始时间', align: 'center', formatter: function (value, row, index) {
                    return value.substr(6,4)+'年'+value.substr(0, 2) + "月" + value.substr(3,2)+"日"+value.substr(11,2)+":"+value.substr(14,2)+":"+value.substr(17,2)
                }
            },
            { field: 'EndTime', title: '活动结束时间', align: 'center', formatter: function (value, row, index) {
                return  value.substr(6, 4) + '年' + value.substr(0, 2) + "月" + value.substr(3, 2) + "日" + value.substr(11, 2) + ":" + value.substr(14, 2) + ":" + value.substr(17, 2)
                }
            },
            { field: 'SeckillPrice', title: '秒杀价', align: 'center' },
            { field: 'UnitPrice', title: '会员价', align: 'center' },
            { field: 'GoodsQty', title: '库存', align: 'center' },

            //{ field: 'ddd', title: '倒计时时间', align: 'center' ,width:300 , formatter: function (value, row, index) {
            //    return "<div class=\"timeout\" style=\"color:red;font-size:20px\"></div>"
            //}}
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '从秒杀活动中移除图书',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从秒杀活动商品中移除名称为 " + obj.booktitle + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromSeckillGoods', { 'seckillid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount();
                            } else {
                                $.messager.alert('错误提示', '移除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '修改秒杀图书信息',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                } else {
                    $('#saveorupdateseckillgoods').linkbutton({ text: '修改' });  //再将按钮文字改为修改
                    //这里为 seckillId 赋值 区别保存操作
                    $("#seckillId").val(obj.Id);
                    $("#begintimeofseckill").datebox('setValue', obj.BeginTime);
                    $("#endtimeofseckill").datebox('setValue', obj.EndTime);
                    $("#bookIdforseckill").val(obj.BookId);
                    $("#bookTitleforseckill").val(obj.booktitle);
                    $("#seckillofflag option[value='" + obj.Flag + "']").attr("selected", "selected");  // 是下拉框中显示管理员选中的活动状态
                    $("#seckillofprice").val(obj.SeckillPrice);
                    $("#seckillofqty").val(obj.GoodsQty);
                    $('#windowforaddgoodstoceckillgoods').window('setTitle', '修改秒杀活动图书信息');//为打开的模态窗口标题赋值
                    $("#windowforaddgoodstoceckillgoods").window('open');   //窗口打开后 选中行的数据就进入了表单中
                }
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

//清仓活动管理操作
function btnClearBooksManager(obj) {
    $('#allPage').layout('collapse', 'east');
    $("#allPage").layout('panel', 'center').panel('setTitle', '操作区  -->您当前位于  -->销售活动  -->清仓处理');
    $(obj).parent().parent().parent().find("a").removeClass("current");
    $(obj).addClass("current");
    $('#IsHaveMessage').hide(); //隐藏 提示无数据
    $('#dg').datagrid({
        view: $.fn.datagrid.defaults.view,
        url: '/admin/Home/ListForClearBooks',
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
            { field: 'Id', title: '图书编号', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"/books/detail?id=" + value + "\" target=\"_blank\" style=\"color:red;text-decoration:underline;\" >" + value + "</a>";
                }
            },
            {
                field: 'ISBN', title: '图书图片', align: 'center', formatter: function (value, row, index) {
                    return "<a href=\"javascript:void(0)\"><img class=\"" + value + "\" src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\"  /></a>";
                }
            },
            { field: 'Title', title: '图书标题', align: 'center' },
            { field: 'ClearPrice', title: '清仓价', align: 'center' },
            { field: 'Author', title: '作者', align: 'center' },
            { field: 'UnitPrice', title: '会员价', align: 'center' },
        ]],
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '从清仓活动中移除图书',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从清仓活动商品中移除名称为 " + obj.Title + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromClearGoods', { 'bookid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
                                getMenuCount()
                            } else {
                                $.messager.alert('错误提示', '移除失败，请联系管理员!', 'error');
                            }
                        }, 'json');
                    }
                });
            }
        }, '-', {
            iconCls: 'icon-edit', text: '更改清仓价',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'warning');
                }
                else {
                    $('#saveorupdatecleargoods').linkbutton({ text: '修改' });  //再将按钮文字改为修改
                    $("#bookIdforeditclearprice").val(obj.Id);
                    $("#bookTitleforeditclearprice").val(obj.Title);
                    $("#beforeclearprice").val("￥"+obj.ClearPrice);
                    $("#windowforeditclearprice").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-add', text: '添加清仓图书(转至图书管理界面)',
            handler: function () {
                btnBookManager();
            }
        }], onLoadSuccess: function (data) {
            if (data.total == 0) {
                $('#IsHaveMessage').show();
            }
        }
    });
}

///////////////////////////// 以上函数对应后台系统菜单 /////////////////////////////

//使用 ajax 添加或修改图书信息
function addNewBookMessageOrUpdateBookMessageByAjax() {
    $('#bookForm').form('submit', {
        url: '/admin/Home/AddNewBookMessageOrUpdateBookMessageByAjax',
        onSubmit: function () {
        },
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object   
            if (data.success == 0) {   //说名添加操作成功
                $.messager.alert('成功提示', '添加成功！！！', 'info');
                $("#windowforeditbook").window('close');
                $('#dg').datagrid('reload');    //添加之后马上刷新一下 使数据与数据库中一致
                getMenuCount();
            } else if (data.success == -1) {
                $("#windowforeditbook").window('close');
                $.messager.alert('错误提示', '添加失败，请联系管理员!', 'error');
            } else if (data.success == 2) {
                $.messager.alert('成功提示', '修改成功！！！', 'info');
                $("#windowforeditbook").window('close');
                $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            } else if (data.success == 3) {
                $("#windowforeditbook").window('close');
                $.messager.alert('错误提示', '修改失败，请联系管理员!', 'error');
            }
        }
    });
}

//使用 ajax 添加图书分类或修改图书分类信息
function addNewCategoryMessageOrUpdateCategoryMessageByAjax() {
    $('#categoryForm').form('submit', {
        url: '/admin/Home/AddNewCategoryMessageOrUpdateCategoryMessageByAjax',
        onSubmit: function () {
        },
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object   
            if (data.success == 0) {   //说名添加操作成功
                $.messager.alert('成功提示', '添加成功！！！', 'info');
                $("#windowforeditcategory").window('close');
                $('#dg').datagrid('reload');    //添加之后马上刷新一下 使数据与数据库中一致
                getMenuCount();
            } else if (data.success == -1) {
                $("#windowforeditcategory").window('close');
                $.messager.alert('错误提示', '添加失败，请联系管理员!', 'error');
            } else if (data.success == 2) {
                $.messager.alert('成功提示', '修改成功！！！', 'info');
                $("#windowforeditcategory").window('close');
                $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            } else if (data.success == 3) {
                $("#windowforeditcategory").window('close');
                $.messager.alert('错误提示', '修改失败，请联系管理员!', 'error');
            }
        }
    });
}

//使用 ajax 添加图书或修改图书出版社信息
function addNewPublisherMessageOrUpdatePublisherMessageByAjax() {
    $('#publishForm').form('submit', {
        url: '/admin/Home/AddNewPublisherMessageOrUpdatePublisherMessageByAjax',
        onSubmit: function () {
        },
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object    
            if (data.success == 0) {   //说名添加操作成功
                $.messager.alert('成功提示', '添加成功！！！', 'info');
                $("#windowforeditpublisher").window('close');
                $('#dg').datagrid('reload');    //添加之后马上刷新一下 使数据与数据库中一致
                getMenuCount();
            }
            else if (data.success == -1) {
                $("#windowforeditpublisher").window('close');
                $.messager.alert('错误提示', '添加失败，请联系管理员!', 'error');
            } else if (data.success == 2) {
                $.messager.alert('成功提示', '修改成功！！！', 'info');
                $("#windowforeditpublisher").window('close');
                $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            } else if (data.success == 3) {
                $("#windowforeditpublisher").window('close');
                $.messager.alert('错误提示', '修改失败，请联系管理员!', 'error');
            }
        }
    });
}

//使用 ajax 添加图书到清仓列表中
function addGoodsToClearGoodsByAjax(){
    $.getJSON('/admin/home/AddBooksToClearGoods', { 'bookid': $('#bookId').val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '图书已添加到清仓商品中！！！', 'info');
            $("#windowforaddgoodstocleargoods").window('close');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == -2) {
            $.messager.alert('操作提示', '请不要尝试重复添加图书到清仓商品中！！！', 'error');
            $("#windowforaddgoodstocleargoods").window('close');
        } else {
            $("#windowforaddgoodstocleargoods").window('close');
            $.messager.alert('错误提示', '添加到清仓商品失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 添加图书到秒杀活动列表中和修改秒杀活动信息
function addBooksToSeckillGoodsOrUpdateSeckillMessageByAjax() {
    //先判断是添加还是修改
    if ($("#seckillId").val() == '') {   //新增
        $.getJSON('/admin/home/AddBooksToSeckillGoodsOrUpdateSeckillMessageByAjax', { 'bookid': $('#bookIdforseckill').val(), 'begintime': $('#begintimeofseckill').datebox('getValue'), 'endtime':$('#endtimeofseckill').datebox('getValue'), 'seckillprice': $('#seckillofprice').val(), 'sysmanid': 1, 'flag': $('#seckillofflag').val(), 'goodsqty': $('#seckillofqty').val() }, function (json) {     //直接发请求修改
            if (json.success == 0) {   //说名新增操作成功
                $.messager.alert('成功提示', '图书编号为' + $('#bookIdforseckill').val() + '的图书已添加到秒杀活动商品中！！！', 'info');
                $("#windowforaddgoodstoceckillgoods").window('close');
                $('#dg').datagrid('reload');    //新增之后马上刷新一下 使表格数据与数据库中一致
                getMenuCount();
            } else if (json.success == -2) {
                $.messager.alert('操作提示', '请不要尝试重复添加图书到秒杀活动商品中！！！', 'error');
                $("#windowforaddgoodstoceckillgoods").window('close');
            } else {
                $("#windowforaddgoodstoceckillgoods").window('close');
                $.messager.alert('错误提示', '添加到秒杀活动商品失败，请联系管理员!', 'error');
            }
        });
    }
    else {        // 将 seckillId（秒杀信息编号） 加入url参数
        $.getJSON('/admin/home/AddBooksToSeckillGoodsOrUpdateSeckillMessageByAjax', { 'bookid': $('#bookIdforseckill').val(), 'begintime': $('#begintimeofseckill').datebox('getValue'), 'endtime': $('#endtimeofseckill').datebox('getValue'), 'seckillprice': $('#seckillofprice').val(), 'sysmanid': 1, 'flag': $('#seckillofflag').val(), 'goodsqty': $('#seckillofqty').val(), 'id': $("#seckillId").val() }, function (json) {     //直接发请求修改
            if (json.success == 0) {   //说名修改操作成功
                $.messager.alert('成功提示', '秒杀活动编号为' + $("#seckillId").val() + '的信息已更新成功！！！', 'info');
                $("#windowforaddgoodstoceckillgoods").window('close');
                $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            } else {
                $("#windowforaddgoodstoceckillgoods").window('close');
                $.messager.alert('错误提示', '添加到秒杀活动商品失败，请联系管理员!', 'error');
            }
        });
    }
}

//使用 ajax 添加图书到积分商品列表中和修改积分列表信息
function addNewIntegralGoodsMessaegeOrUpdateIntegralGoodsMessaegeByAjax() {
    $('#integralForm').form('submit', {
        url: '/admin/Home/AddNewIntegralGoodsMessaegeOrUpdateIntegralGoodsMessaegeByAjax',
        success: function (data) {
            var data = eval('(' + data + ')');  // change the JSON string to javascript object   
            if (data.success == 0) {   //说名添加操作成功
                $.messager.alert('成功提示', '添加成功！！！', 'info');
                $("#windowforaddintegralgoods").window('close');
                $('#dg').datagrid('reload');    //添加之后马上刷新一下 使数据与数据库中一致
                getMenuCount();
            } else if (data.success == -1) {
                $("#windowforaddintegralgoods").window('close');
                $.messager.alert('错误提示', '添加失败，请联系管理员!', 'error');
            } else if (data.success == 2) {
                $.messager.alert('成功提示', '修改成功！！！', 'info');
                $("#windowforaddintegralgoods").window('close');
                $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            } else if (data.success == 3) {
                $("#windowforaddintegralgoods").window('close');
                $.messager.alert('错误提示', '修改失败，请联系管理员!', 'error');
            }
        }
    });
}

//使用 ajax 修改会员评论星级
function updateCommentStarByAjax() {
    $.get('/admin/home/UpdateUserCommentStarByAjax', { 'commentid': $('#commentId').val(), 'flag': $('#commentFlag').combobox('getValue') }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '修改成功！！！', 'info');
            $("#windowforeditusercommentstar").window('close');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
        } else {
            $("#windowforeditusercommentstar").window('close');
            $.messager.alert('错误提示', '修改失败，请联系管理员!', 'error');
        }
    }, 'json');
}

//使用 ajax 修改清仓活动中图书的价格
function editclearpricebyajax() {
    $.getJSON('/admin/home/EditClearPriceByAjax', { 'bookid': $('#bookIdforeditclearprice').val(), 'newclearprice': $('#afterclearprice').val()}, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '图书名为' + $("#bookTitleforeditclearprice").val() + '的图书的清仓价已成功修改为：￥' + $("#afterclearprice").val() + '！！！', 'info');
            $("#windowforeditclearprice").window('close');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
        } else {
            $("#windowforeditclearprice").window('close');
            $.messager.alert('错误提示', '图书清仓价修改失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 查询图书
function btnSerchBooks() {
    $('#allPage').layout('collapse', 'east');
    btnBookManager(null, $("#inputBookTitle").val(), $("#inputBookAuthor").val(), $("#inputBookId").val(),$('#chooseCategories').combobox('getValue'),$('#chossePublishers').combobox('getValue')); //调用函数 传参实现查询
}

//使用 ajax 根据 订单状态查询和订单号查询
function btnSerchOrder() {
    $('#allPage').layout('collapse', 'east');       //用户点击查询之后立即关闭查询区使操作区域大一些
    if ($('#inputOrderId').val() == '' && $('#chooseorderflag').combobox('getValue') == 0)  //判断用户输入的查询条件，如果订单编号和 下拉框都没有输入或选择则提示相应信息
    {
        $.messager.alert('操作提示', '请输入订单号或选择订单状态！', 'info');
        return false;   //取消按钮的默认操作
    }
    btnOrderManager(null, $('#chooseorderflag').combobox('getValue'), $('#inputOrderId').val() == '' ? 0 : $('#inputOrderId').val());       //调用函数返回传入状态订单视图  此处如果订单状态输入框用户没有输入则传值0
}

//使用 ajax 获取订单信息 参数值 flag 决定订单列表中订单的状态 ： 订单状态共有-->  1:未处理 2:处理中 3:已发货 4:会员取消订单 5:管理员取消订单 6:已收货 7:已完成 8:订单已取消 9:申请退货中 10:管理员确认退货 11:我方已收货 12:退货完成  
function GetOrderMessageByFlag(flag, orderid) {
    $('#dg').datagrid({
        view: detailview,
        url: '/admin/Home/ListForOrderByFlag',
        queryParams: {
            'flag': flag,      // 调用该函数传入的参数 flag 作为 ajax 请求参数进入后台方法  返回传入参数类型的订单信息 json 数据 ，最后完成展现
            'orderid': orderid
        },
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
        pageNumber: 1,
        pageSize: 20,
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
            }
        ]],detailFormatter: function (index, row) {     
            return '<div style="padding:2px"><table id="ddv-' + index + '"></table></div>';
        }, onExpandRow: function (index, row) {  
            $('#ddv-' + index).datagrid({
                url: '/admin/Home/ListForOrderBooksByOrderId',
                queryParams: {
                    orderId: row.Id,                       //此时传入 的 row.Id 相当于 orderid  datagrid 显示相应订单号的 详细信息
                },
                style: 'width:90%',
                fitColumns: true,
                singleSelect: true,
                autoRowHeight: true,
                pagination: true,
                loadMsg: '正在玩命加载中,请稍候...',
                columns: [[
                    { field: 'Id', title: '订单明细', align: 'center' },
                    { field: 'OrderID', title: '订单编号', align: 'center' },
                    { field: 'BookID', title: '图书编号', align: 'center' },
                    {
                        field: 'bookImg', title: '图书图片', align: 'center', formatter: function (value, row, index) {
                            return "<img  src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\" />";
                        }
                    },
                    { field: 'title', title: '图书名字', align: 'center' },
                    { field: 'Quantity', title: '数量', align: 'center' },
                    { field: 'marketPrice', title: '市场价', align: 'center' },
                    { field: 'UnitPrice', title: '会员价', align: 'center' },
                    {
                        field: 'flag', title: '购买方式', align: 'center', formatter: function (value, row, index) {
                            if (value == 0) {
                                return '<span style="color:blue">普通订单</span>';
                            } else if (value == 1) {
                                return '<span style="color:#ff6a00">秒杀订单</span>';
                            } else if (value == 2) {
                                return '<span style="color:#00ff21">清仓订单</span>';
                            }
                        }},
                    {
                        field: 'flagfororders', title: '订单状态', align: 'center', formatter: function (value, row, index) {
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
                    }
                ]],
                onResize: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg').datagrid('fixDetailRowHeight', index);
                    }, 0);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        }
    });
}

//使用 ajax 修改订单为处理中状态
function doOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToDoing', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为处理中状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == 1) {   //说名该订单状态不是处理中状态
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不是未处理状态,提交失败！！！', 'error');
        } else if (json.success == 2) {   //说名该订单状态已经是处理中状态
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已经处于处理中状态！！！', 'error');
        } else if(json.success==-1) {
            $.messager.alert('错误提示', '订单状态修改失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 修改订单为管理员已取消状态
function adminCancelOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToAdminCancel', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为管理员取消订单状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == 1) {  
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为未处理和处理中状态，无法进行取消订单操作！！！', 'error');
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '取消订单失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 修改订单为已发货状态
function outGoodsOrderByAjax(){
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToOutGoods', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为已发货订单状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == 1) {   
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为处理中状态，无法进行已发货状态操作！！！', 'error');
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '发货失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 管理员同意用户取消订单
function agreeUserCancelOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToCanceledOrder', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为订单已取消状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == 1) {   
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为会员已取消状态，无法进行订单已取消状态操作！！！', 'error');
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '修改订单状态失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 管理员同意用户退货请求的订单处理
function agreeUserReturnGoodsOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToReturnGoodsOrder', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为管理员确认退货状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
            getMenuCount();
        } else if (json.success == 1) {
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为会员已收货或已完成状态,无法进行同意会员退货操作！！！', 'error');
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '修改订单状态失败，请联系管理员!', 'error');
        }
    });
}

//使用 ajax 管理员收到用户退货商品
function getReturnGoodsOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.get('/admin/home/UpdateOrderFlagToGetReturnGoodsOrder', { 'orderId': $("#OrderId").val() }, function (json) {
        if (json.success == 0) {
            $.messager.alert('成功提示', '订单状态修改为我方已收货！！！', 'info');
            $('#dg').datagrid('reload');
            getMenuCount();
        } else if (json.success == 1) {
            $.messager.alert('错误提示', '订单状态不为管理员确认退货状态，操作失败!', 'error');
        } else {
            $.messager.alert('错误提示', '操作失败，请联系管理员!', 'error');
        }
    }, 'json');
     
}

//使用 ajax 管理员执行退货订单已完成操作
function okReturnGoodsOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.get('/admin/home/UpdateOrderFlagToSuccessReturnGoodsOrder', { 'orderId': $("#OrderId").val() }, function (json) {
        if (json.success == 0) {
            $.messager.alert('成功提示', '订单状态已修改为退货完成！！！', 'info');
            $('#dg').datagrid('reload');
            getMenuCount();
        } else if (json.success == 1) {
            $.messager.alert('错误提示', '订单状态不为我方已收货状态，操作失败!', 'error');
        } else {
            $.messager.alert('错误提示', '操作失败，请联系管理员!', 'error');
        }
    }, 'json');

}

//使用 ajax 获取系统菜单中各项对应的记录数   此函数在 启动函数中调用 并且在系统菜单中各个操作中若对数据进行了增删改操作 则会调用该函数 覆盖原先的 系统菜单中各项的记录数
function getMenuCount() {
    $.getJSON('/admin/home/GetAllMenuCount', function (json) {
        $('#btnBookManager').html("(" + json.bookCount + ")");
        $('#btnCategoryManager').html("(" + json.categoryCount + ")");
        $('#btnPublisherManager').html("(" + json.publishCount + ")");
        $('#btnIntegralGoodsManager').html("(" + json.integralGoodCount + ")");
        $('#btnUserManager').html("(" + json.userCount + ")");
        $('#btnReaderCommentManager').html("(" + json.readerComment + ")");
        $('#btnOrderManagercount').html("(" + json.ordersCount + ")");
        $('#btnProcessingOrderManager').html("(" + json.outOrdersCount + ")");
        $('#btnUserCancelOrderManager').html("(" + json.cancelOrdersCount + ")");
        $('#btnUserReturnGoodsOrderManager').html("(" + json.returnGoodsOrdersCount + ")");
        $('#btnSeckillActiveManager').html("(" + json.seckillOrdersCount + ")");
        $('#btnClearBooksManager').html("(" + json.clearOrdersCount + ")");
    });
}

//此方法打开订单详情窗口   根据传入的参数 methodName 替换按钮的事件   参数 executeName 表示替换按钮上的文字  参数 obj 为选中行的 对象   参数 windowTitle 表示 替换即将打开窗口的标题
function prepareForDetailOrderBooksListMessage(obj, executeName, methodName, windowTitle) {
    $("#OrderId").textbox('setValue', obj.Id);
    $("#OrderDate").textbox('setValue', obj.OrderDate);
    $("#loginId").textbox('setValue', obj.loginId);
    $("#TotalPrice").textbox('setValue', obj.TotalPrice);
    $("#OrderPersonName").textbox('setValue', obj.personName);
    $("#UserPhone").textbox('setValue', obj.phone);
    $("#OrderAddress").textbox('setValue', obj.address);
    $("#OrderRemark").textbox('setValue', obj.OrderRemark);
    $("#OrderFlag").textbox('setValue', formatFlag(obj.flag));  //为文本框进行赋值
    $('#dgfororder').datagrid({
        queryParams: {
            orderId: $("#OrderId").val(),                       //传入 orderid 此时 datagrid 显示相应订单号的 详细信息
        }
    });
    $('#orderButton').linkbutton({ text: executeName });        //替换按钮文字
    $('#windowforOrderManager').window('setTitle', windowTitle);//为打开的模态窗口标题赋值
    $('#orderButton').attr('onclick', methodName);              //替换按钮绑定的函数 实现不同的功能
    $("#windowforOrderManager").window('open');
}








