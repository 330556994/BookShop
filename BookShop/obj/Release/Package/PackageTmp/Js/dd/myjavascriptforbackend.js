//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////这里进入后台 js 脚本////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//作者:龚涛    
//时间:2016/12/9
//描述:对后台控制器的所有 ajax 方法提供支持,生成后台 index 页面的所有datagrid控件并提供数据展现。

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
})

//图书管理操作
function btnBookManager(title,author,bookid) {
    //使用Javascript方式创建DataGrid控件  并请求相应的控制器返回json数据以显示在 DataGrid 中
    $('#dg').datagrid({
        url: '/admin/Home/listforbook',
        queryParams: {
            'title': title,
            'author': author,       
            'bookid':bookid
        },
        style: 'width:100%',
        fitColumns: true,
        singleSelect: true,
        autoRowHeight: true,
        sortName: "Id",
        sortOrder: "desc",
        pagination: true,
        loadMsg: '正在拼命加载中,请稍候...',
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
            { field: 'Title', title: '书名', width:200  },
            { field: 'Author', title: '作者',  align: 'center' },
            { field: 'Pname', title: '出版社名称',  align: 'center' },
            { field: 'Cname', title: '类别名称', align: 'center' },
            { field: 'PublishDate', title: '出版日期',  align: 'center' },
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
                        return '清仓商品';
                    }
                }
            },
            {
                field: 'isSeckillGoogs', title: '是否为秒杀商品', align: 'center', formatter: function (value, row, index) {
                    if (value == 0) {
                        return '非秒杀商品';
                    } if (value == 1) {
                        return '秒杀商品';
                    }
                }
            }, {
                field: 'isIntegralGoods', title: '是否为积分兑换商品', align: 'center', formatter: function (value, row, index) {
                    if (value == 0) {
                        return '非积分商品';
                    } if (value == 1) {
                        return '积分商品';
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
                $("#windowforeditbook").window('open');
            }
        }, '-', {
            iconCls: 'icon-cancel', text: '删除图书',
            handler: function () {
                //这是调用方法，名字叫 getSelected，该方法无参数
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除书名为《" + obj.Title + "》的图书吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletebookbyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
                    $("#windowforeditbook").window('open');
                }
            }
        }, '-', {
            iconCls: 'icon-add', text: '添加图书至清仓活动',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
            iconCls: 'icon-add', text: '添加图书至秒杀活动',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的图书信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                } else {
                    $('#saveorupdateseckillgoods').linkbutton({ text: '添加' });  //再将按钮文字改为添加
                    //为表单赋值
                    $("#sysmanidofseckill").val(1); //管理员赋值
                    $("#bookIdforseckill").val(obj.Id);
                    $("#bookTitleforseckill").val(obj.Title);
                    $("#windowforaddgoodstoceckillgoods").window('open');

                }
            }
        }, '-', {
            iconCls: 'icon-add', text: '添加图书至积分商品',
            handler: function () {
                //在调用模态窗口之前  因为是修改操作因此要将所选的图书信息显示值模态窗口的 form 表单中
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
                $('#allPage').layout('expand', 'east');            }
        }, '-', {
            iconCls: 'icon-add', text: '关闭查询区',
            handler: function () {
                $('#allPage').layout('collapse', 'east');
                    
            }
        }, '-', {
            iconCls: 'icon-more', text: '全屏显示',
            handler: function () {
                $('#allPage').layout('collapse', 'east');
                $('#allPage').layout('collapse', 'west');
            }
        }, '-', {
            iconCls: 'icon-add', text: '取消全屏',
            handler: function () {
                $('#allPage').layout('expand', 'west');
            }
        }]
    });
}

//图书类别管理操作
function btnCategoryManager() {
    $('#dg').datagrid({
        url: '/admin/Home/listforcategory',
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
            { field: 'Id', title: '类别编号', align: 'center' },
            { field: 'Name', title: '类别名称', align: 'center' }
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除名称为 " + obj.Name + " 的类别吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletecategorybyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                } else {
                    $("#CategoryId").val(obj.Id);
                    $("#CategoryName").textbox('setValue', obj.Name);
                    $('#saveorupdatecategory').linkbutton({ text: '修改' });
                    $("#windowforeditcategory").window('open');
                }
            }
        }]
    });
}

//出版社管理操作
function btnPublisherManager() {
    $('#dg').datagrid({
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
        pageSize: 5,
        pageList: [5, 10, 15, 20],
        columns: [[
            { field: 'Id', title: '出版社编号', align: 'center' },
            { field: 'Name', title: '出版社名称', align: 'center' }
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除名称为 " + obj.Name + " 的出版社吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get('/admin/home/deletepublisherbyid', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                } else {
                    $("#publisherId").val(obj.Id);
                    $("#publisherName").textbox('setValue', obj.Name);
                    $("#windowforeditpublisher").window('open');
                    $('#saveorupdatepublisher').linkbutton({ text: '修改' });
                }
            }
        }]
    });
}

//会员账号信息管理操作
function btnUserManager() {
    $('#dg').datagrid({
        url: '/admin/Home/ListForUsers',
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//会员评论管理操作
function btnReaderCommentManager() {
    $('#dg').datagrid({
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
            { field: 'Title', title: '评论标题' },
            { field: 'Comment', title: '评论内容' },
            { field: 'Btitle', title: '图书名称' },
            { field: 'Uname', title: '评论会员' },
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要删除标题为 " + obj.Title + " 的评论吗？", function (data) {
                    if (data) {
                        //如果 用户点击确定后  执行一个  ajax 请求  执行删除相应编号的  评论
                        $.get("/user/DeleteComment", { 'commentid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '删除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//秒杀活动管理操作
function btnSeckillActiveManager() {
    $('#dg').datagrid({
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从秒杀活动商品中移除名称为 " + obj.booktitle + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromSeckillGoods', { 'seckillid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
                    $("#windowforaddgoodstoceckillgoods").window('open');   //窗口打开后 选中行的数据就进入了表单中
                }
            }
        }]
    });
}

//清仓活动管理操作
function btnClearBooksManager() {
    $('#dg').datagrid({
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
                    return "<a href=\"javascript:void(0)\" onclick=\"btnShowBookDetail(" + value + ")\"><img class=\"" + value + "\" src=\"/Images/BookCovers/" + value + ".jpg\" style=\"height:100px;width:80px;\"  /></a>";
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从清仓活动商品中移除名称为 " + obj.Title + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromClearGoods', { 'bookid': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//积分商品管理操作
function btnIntegralGoodsManager() {
    $('#dg').datagrid({
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要从积分商品中移除名称为 " + obj.booktitle + " 的图书吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/RemoveBookFromIntegralGoods', { 'id': obj.Id }, function (json) {
                            if (json.success == 0) {   //说名删除操作成功
                                $.messager.alert('成功提示', '移除成功！！！', 'info');
                                $('#dg').datagrid('reload');    //删除之后马上刷新一下 使数据与数据库中一致
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//未处理状态订单管理操作
function btnOrderManager(flag,orderid) {
    GetOrderMessageByFlag(flag,orderid);      
    $('#dg').datagrid({
        toolbar: [{
            iconCls: 'icon-edit', text: '处理订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
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
                var obj = $("#dg").datagrid("getSelected");//得到当前选中的第一行. 返回的json对象
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
        }, '-', {
            iconCls: 'icon-add', text: '打开查询区',
            handler: function () {
                
                $('#allPage').layout('expand', 'east');
                $('#ttforseach').tabs('select', 1);        //这里打开查询区时 让选项卡选中查询订单的内容页面
            }
        }, '-', {
            iconCls: 'icon-add', text: '关闭查询区',
            handler: function () {
                $('#allPage').layout('collapse', 'east');
            }
        }, '-', {
            iconCls: 'icon-more', text: '全屏显示',
            handler: function () {
                $('#allPage').layout('collapse', 'east');
                $('#allPage').layout('collapse', 'west');
            }
        }, '-', {
            iconCls: 'icon-add', text: '取消全屏',
            handler: function () {
                $('#allPage').layout('expand', 'west');
            }
        }]
    });
}

//处理中状态订单管理操作
function btnProcessingOrderManager() {
    GetOrderMessageByFlag(2,0);     //此处调用函数 传入参数 2 表示返回的是 处理中状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于处理中状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-edit', text: '发货',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
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
                $('#orderButton').linkbutton({ text: '发货' });  //再将按钮文字改为修改
                $('#orderButton').attr('onclick', 'outGoodsOrderByAjax()');  //再更改和按钮绑定的函数 让他执行取消订单操作
                $("#windowforOrderManager").window('open');
            }
        }]
    });
}

//发货中状态订单管理操作
function btnDeliveryOrderManager() {
    GetOrderMessageByFlag(3);     //此处调用函数 传入参数 3 表示返回的是 发货中状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于发货中状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要冻结编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/FrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {
                                $.messager.alert('成功提示', '会员编号为 ' + obj.Id + ' 的账号已冻结！！！', 'info');
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//会员已取消状态订单管理操作
function btnUserCancelOrderManager() {
    GetOrderMessageByFlag(4,0);     //此处调用函数 传入参数 4 表示返回的是 会员已取消状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于会员已取消状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-edit', text: '处理用户取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
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
                $('#orderButton').linkbutton({ text: '同意取消' });  //再将按钮文字改为修改
                $('#orderButton').attr('onclick', 'agreeUserCancelOrderByAjax()');  //再更改和按钮绑定的函数 让他执行同意订单取消操作
                $("#windowforOrderManager").window('open');
            }
        }, '-', {
            iconCls: 'icon-edit', text: '解封账号',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//管理员已取消状态订单管理操作
function btnAdminCancelOrderManager() {
    GetOrderMessageByFlag(5);     //此处调用函数 传入参数 5 表示返回的是 管理员已取消状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于管理员已取消状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要冻结编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/FrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {
                                $.messager.alert('成功提示', '会员编号为 ' + obj.Id + ' 的账号已冻结！！！', 'info');
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//会员已收货状态订单管理操作
function btnUserConfirmReceivedOrderManager() {
    GetOrderMessageByFlag(6);     //此处调用函数 传入参数 6 表示返回的是 会员已收货状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于会员已收货状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要冻结编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/FrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {
                                $.messager.alert('成功提示', '会员编号为 ' + obj.Id + ' 的账号已冻结！！！', 'info');
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//已完成状态订单管理操作
function btnUserConfirmReceivedOrderManager() {
    GetOrderMessageByFlag(7);     //此处调用函数 传入参数 7 表示返回的是 已完成状态的订单列表
    $('#dg').datagrid({
        //调用函数后列表已经有了 但是工具栏是根据不同状态的订单而变化的 这里添加针对于已完成状态订单的工具栏
        //添加工具栏
        toolbar: [{
            iconCls: 'icon-cancel', text: '取消订单',
            handler: function () {
                var obj = $("#dg").datagrid("getSelected");
                if (obj == null) {
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
                }
                $.messager.defaults = { ok: "是", cancel: "否" };
                $.messager.confirm("操作提示", "您确定要冻结编号为 " + obj.Id + " 的账号吗？", function (data) {
                    if (data) {
                        $.get('/admin/home/FrozenUserAccount', { 'userid': obj.Id }, function (json) {
                            if (json.success == 0) {
                                $.messager.alert('成功提示', '会员编号为 ' + obj.Id + ' 的账号已冻结！！！', 'info');
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
                    $.messager.alert('操作提示', '执行操作之前请选择一条记录！', 'info');
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
        }]
    });
}

//使用 ajax 获取订单信息 参数值 flag 决定订单列表中订单的状态 ：   订单状态共有-->     1.未处理  只有这个状态会员可以取消  2.处理中  3.已发货 4.会员已取消 5.管理员取消 6.已收货  7.已完成 8.订单已退   此处 flag 就可以传入这八种状态对应的数字实现不同类型订单的显示
function GetOrderMessageByFlag(flag,orderid) {
    $('#dg').datagrid({
        url: '/admin/Home/ListForOrderByFlag',
        queryParams: {
            'flag': flag,      // 调用该函数传入的参数 flag 作为 ajax 请求参数进入后台方法  返回传入参数类型的订单信息 json 数据 ，最后完成展现
            'orderid':orderid
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
    });
}

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

//使用 ajax 添加图书到清仓列表中
function addGoodsToClearGoodsByAjax(){
    $.getJSON('/admin/home/AddBooksToClearGoods', { 'bookid': $('#bookId').val(), 'flag': $('#bookcleargoodsflag').val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '图书已添加到清仓商品中！！！', 'info');
            $("#windowforaddgoodstocleargoods").window('close');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
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
    btnBookManager($("#inputBookTitle").val(), $("#inputBookTitle").val(), $("#inputBookTitle").val())
}

//使用 ajax 修改订单为处理中状态
function doOrderByAjax() {
    $('#windowforOrderManager').window('close');
    $.getJSON('/admin/home/UpdateOrderFlagToDoing', { 'orderid': $("#OrderId").val() }, function (json) {     //直接发请求修改
        if (json.success == 0) {   //说名修改操作成功
            $.messager.alert('成功提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已修改为处理中状态！！！', 'info');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
        } else if (json.success == 1) {   //说名该订单状态不是处理中状态
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不是未处理状态,提交失败！！！', 'error');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
        }
        else if (json.success == 2) {   //说名该订单状态已经是处理中状态
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单已经处于处理中状态！！！', 'error');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
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
        } else if (json.success == 1) {  
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为未处理和处理中状态，无法进行取消订单操作！！！', 'error');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
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
        } else if (json.success == 1) {   
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为处理中状态，无法进行已发货状态操作！！！', 'error');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
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
        } else if (json.success == 1) {   
            $.messager.alert('错误提示', '订单号为 ' + $("#OrderId").val() + ' 的订单不为会员已取消状态，无法进行订单已取消状态操作！！！', 'error');
            $('#dg').datagrid('reload');    //修改之后马上刷新一下 使表格数据与数据库中一致
        } else if (json.success == -1) {
            $.messager.alert('错误提示', '发货失败，请联系管理员!', 'error');
        }
    });

}

//使用 ajax 根据 订单状态查询订单
function btnSerchOrder() {
    $('#allPage').layout('collapse', 'east');       //用户点击查询之后立即关闭查询区使操作区域大一些
    if ($('#inputOrderId').val() == '' && $('#chooseorderflag').combobox('getValue') == 0)
    {
        $.messager.alert('操作提示', '请输入订单号或选择订单状态！', 'info');
        return false;   //取消按钮的默认操作
    }
    btnOrderManager($('#chooseorderflag').combobox('getValue'), $('#inputOrderId').val() == '' ? 0 : $('#inputOrderId').val());       //调用函数返回传入状态订单视图
}

var interval = 1000;
function ShowCountDown(year, month, day) {
    var now = new Date();
    var endDate = new Date(year, month - 1, day);
    var leftTime = endDate.getTime() - now.getTime();
    var leftsecond = parseInt(leftTime / 1000);
    //var day1=parseInt(leftsecond/(24*60*60*6)); 
    var day1 = Math.floor(leftsecond / (60 * 60 * 24));
    var hour = Math.floor((leftsecond - day1 * 24 * 60 * 60) / 3600);
    var minute = Math.floor((leftsecond - day1 * 24 * 60 * 60 - hour * 3600) / 60);
    var second = Math.floor(leftsecond - day1 * 24 * 60 * 60 - hour * 3600 - minute * 60);
    $(".timeout").html( day1 + "天" + hour + "小时" + minute + "分" + second + "秒");
}

$(function () {
    setInterval("ShowCountDown(" + new Date().getFullYear() + "," + (new Date().getMonth() + 1) + ", " + 12+")", 1000);
});







