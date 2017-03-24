declare @a int --变量定义
set @a=10 --赋值
declare @bookid int
declare @title varchar(50)
declare @author varchar(50)
set @bookid=4958
--也可以直接从数据表中赋值
print @a --显示
select @title=title,@author=author from Books where id=@bookid
select @title,@author --显示
go  --代表一个批处理结束，之前定义的局部变量销毁
--下面演示if语句
declare @year int
--调用内部函数,getdate当前日期，datepart获得日期的一部分
--得到当前年份
set @year= DATEPART(year, GETDATE())
if (@year % 4=0  and @year % 100!=0 ) or @year %400=0
   begin
     --cast函数是类型转换 
		print cast(@year as varchar(50))+'是闰年'
   end
else
 begin
    print cast(@year as varchar(50))+'不是闰年'
 end
go
----------------------------------------
--循环
declare @i int 
declare @sum int 
set @i=1
set @sum=0
while @i<=100
begin
	set @sum=@sum+@i
	set @i=@i+1
end
print '1+2+3+4+...100='+cast(@sum as varchar(50))
----------------------------------------
--游标的使用
--游标是一种操作数据表的对象，非常像ado.net datareader对象
--datareader对象的使用
-- 打开链接，定义cmd，设置select语句，cmd.ExecuteDatareader
-- while(dr.Read()){
    -- name=dr["name"].ToString()
--}
--dr.close,  conn.close()
declare @cid int 
declare @title varchar(50) 
declare @unitprice numeric(10,2)
set @cid=25
--定义游标变量，名字叫cur，
declare  cur cursor  for
 select title,unitprice from Books where CategoryId=@cid
--打开游标
open cur --打开游标 刚打开时，指向第一条的上一条
--读取下一条记录
fetch next from cur into @title,@unitprice
--@@fetch_status 这是全局变量，这个变量指示了游标读取状态
-- 0代表成功
--如果读取成功，则循环
while @@FETCH_STATUS=0
begin
  --处理数据，这里只是简单打印
	print @title+cast(@unitprice as varchar(50))
	fetch next from cur into @title,@unitprice --继续读取下一条
end
close cur --关闭游标，如果关闭后，可以再调用open打开
deallocate cur--释放游标,释放后游标真的没有了，不能再open
go
=============================================























print 10%3