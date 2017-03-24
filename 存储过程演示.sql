
-- books ,categories,publishers
--分页的思想，显示多少取多少，目的提高性能
-- 第10条到第30条记录
-- 函数 ROW_NUMBER() over(order by a.id)
select * from (
select ROW_NUMBER() over(order by a.id) as rowid,
 a.*,b.Name as  Cname,b.SortNum,
c.Name as Pname
 from books a 
  inner join Categories b
  on a.CategoryId=b.Id
  inner join Publishers c
  on a.PublisherId=c.Id
  where CategoryId=25 and Title like '%.NET%') tablea
    where rowid between 10 and 30
   ===========================
  存储过程？ 是一个独立的功能，直接调用
  好处是提高性能
  create proc proc_demo1(
		@a int, --参数
		@b varchar(50),
		@c varchar(50) output --输出参数	
  )
  as
		declare @d varchar(50) --定义变量
		set @d='abcd'
		set @c=@b+@d 
		return 0 --存储过程返回值只能返回int 
  go
-------------调用上面的存储过程-----
  declare @aa int
  declare @bb varchar(50)
  declare @cc varchar(50)
  declare @ret int
  set @aa=10
  set @bb='aaa'
  exec @ret=proc_demo1 @aa,@bb,@cc output 
  select @ret,@cc
  go
  
  =========================
  create proc  proc_getbooks(
    @start int ,--从第几条取
    @end int ,--取到第几条
    @sort varchar(50) ,--排序条件如 a.id desc 
     @categoryid  varchar(50) --图书类别编号  
  )
  as
  select * from (
select ROW_NUMBER() over(order by @sort) as rowid,
 a.*,b.Name as  Cname,b.SortNum,
c.Name as Pname
 from books a 
  inner join Categories b
  on a.CategoryId=b.Id
  inner join Publishers c
  on a.PublisherId=c.Id
  where CategoryId=@categoryid ) tablea
    where rowid between @start and @end
   go 
  ============
  exec proc_getbooks 10,30,'a.id desc',25
=========================================


alter proc proc_page(
  --通用版本分页存储过程
	@tablename varchar(50) ,--表名
	@fields varchar(100)='*',--要显示的列名，默认*，如 * 或  id,name,age 
	@start varchar(10) ='1',--默认是1 起始索引
	@end varchar(10) ='10', --默认是10 结束索引
	@sort varchar(50)='id asc' ,--默认 排序表达式
	@where varchar(100) --where条件 注意必须带where关键字
)
as
--动态sql
declare @sql varchar(1000) --定义sql临时变量
set @sql='
select * from (
select ROW_NUMBER() over(order by id) as rowid,'
+@fields+'
 from '+@tablename +'
   '+@where +') tablea
    where rowid between '+@start+ ' and '+@end
   --print @sql 
  exec(@sql) --动态sql
go
==================
exec proc_page 'categories','*','10','20',
'id desc',''
----------------------------
USE [BookShopPlus]
GO
/****** Object:  StoredProcedure [dbo].[proc_margeOrder]    Script Date: 11/11/2016 09:07:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[proc_margeOrder]
(
--合并订单，只处理同一用户，都是未处理
--将源订单合并到目标订单中
--合并后，源订单状态改成8
@sourceId int, --要合并的源订单
@targetId int --合并到目标订单
)
as
  declare @userid1 int --源订单会员编号
  declare @userid2 int --目标订单会员编号
  declare @flag1 int --源订单状态
  declare @flag2 int --目标订单状态
  --定义游标获得源订单明细 
  declare  cur_1 cursor for select BookID,Quantity,unitprice from OrderBook where OrderID=@sourceId 
  declare @bookid int--图书编号
  declare @quantity int--图书数量
  declare @unitprice numeric(8,2)--单价
  declare @count int 
  declare @totalprice numeric(8,2) --合并后订单总价
begin
  select @userid1=userid,@flag1=flag from Orders where ID=@sourceId
  select @userid2=userid,@flag2=flag from Orders where ID=@targetid
  if @userid1!=@userid2 
      return -1 --表示不是同一用户
  if @flag1!=1 and @flag2!=1 
	return -2 --订单状态不为1
--条件满足开始处理合并
--	
 open cur_1 --打开游标
fetch next from cur_1 into @bookid,@quantity,@unitprice
begin try
while @@FETCH_STATUS=0 
begin
    --取出目标订单明细中，该图书的数量
	select @count=COUNT(*) from OrderBook where BookID=@bookid and OrderID=@targetid
	if @count=0
	   --不存在，则插入
	    insert into OrderBook values(@targetid,@bookid,@quantity,@unitprice)
	 else
	    --存在则更新数量
	    update OrderBook set Quantity=Quantity+@quantity where OrderID=@targetid and BookID=@bookid
	 fetch next from cur_1 into @bookid,@quantity,@unitprice
end 
close cur_1
deallocate cur_1
	  update Orders set flag=8 where ID=@sourceid --更新源订单状态
     select @totalprice=SUM(quantity*unitprice) from OrderBook
     where OrderID=@targetid --求出合并后订单明细总价 
     --更新目标订单总价
      update Orders set TotalPrice=	@totalprice where ID=@targetid 
     commit tran --提交事务
     return 0 --正常    
end try
begin catch
	rollback tran--回滚
	return -3  --处理错误
end catch
end

