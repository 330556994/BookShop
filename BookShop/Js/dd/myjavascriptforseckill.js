//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////这里进入秒杀页面 js 脚本////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//作者:四人帮   
//时间:2016/12/26
//描述:对会员控制器的大部分 ajax 方法提供支持,生成会员页面 index 页面的所有datagrid控件并提供数据展现。


var limit_time;
if (window.name == "") {//查看是否已有记录的时间  
    limit_time = 6000;//设置结束时间，单位秒  
} else {
    limit_time = window.name;
}
function timeDown() {
    if (limit_time >= 0) {
        var day = Math.floor(limit_time / 60 / 60 / 24); /*划分出时分秒*/
        var hour = Math.floor(limit_time / 60 / 60 % 24);
        var minutes = Math.floor(limit_time / 60 % 60);
        var seconds = Math.floor(limit_time % 60);
        $("#t_day").html(day); /*写到页面中*/
        $("#t_hour").html(hour);
        $("#t_minute").html(minutes);
        $("#t_second").html(seconds);
        if (limit_time == 10) {
            alert("提醒，还有10秒结束");
        }
        --limit_time;
        window.name = limit_time;//记录当前时间。  
    }
    else {
        clearInterval(timer);
        alert("时间到！活动结束");
    }
}
timer = setInterval("timeDown()", 1000);//设置定时器，1秒钟执行一次timeDown()函数；  

function add(bookid, seckillprice) {
    var url = "/order/specialorder?bookid=" + bookid + "&price=" + seckillprice + "&flag=1" + "&qty=1";
    location.href = url;
};

$(function () {
    changeImg();
    $("#miaoshao").mouseover(function () {
        $(this).css("background-color", "red").css("color", "yellow");
    }).mouseout(function () {
        $(this).css("background-color", "#00ff90").css("color", "#fff");;
    });

    $(document).snowfall('clear');
    document.body.className = "darkBg";
    $(document).snowfall({ round: true, minSize: 5, maxSize: 8, flakeCount: 200 });

    $('.contentdiv').mouseover(function () { $(this).css('border', '10px solid #ffd800'); }).mouseout(function () { $(this).css('border', '10px solid #303030'); });
    $('.bookimage').mouseover(function () { $(this).height(260).width(180).css('border', '5px solid pink'); }).mouseout(function () { $(this).height(240).width(160).css('border', '5px solid white'); });
});

function mouserOver(obj) {
    $(obj).find('img').height(70).width(80);
}
function mouseOut(obj) {
    $(obj).find('img').height(60).width(70);
}
//轮换图片
function changeImg() {
    var index = 0;
    var stop = false;
    var $li = $("#content").find("#scroll_img").children("li");
    var $page = $("#content").find("#scroll_number").children("li");
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