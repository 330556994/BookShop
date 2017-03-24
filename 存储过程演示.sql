
-- books ,categories,publishers
--��ҳ��˼�룬��ʾ����ȡ���٣�Ŀ���������
-- ��10������30����¼
-- ���� ROW_NUMBER() over(order by a.id)
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
  �洢���̣� ��һ�������Ĺ��ܣ�ֱ�ӵ���
  �ô����������
  create proc proc_demo1(
		@a int, --����
		@b varchar(50),
		@c varchar(50) output --�������	
  )
  as
		declare @d varchar(50) --�������
		set @d='abcd'
		set @c=@b+@d 
		return 0 --�洢���̷���ֵֻ�ܷ���int 
  go
-------------��������Ĵ洢����-----
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
    @start int ,--�ӵڼ���ȡ
    @end int ,--ȡ���ڼ���
    @sort varchar(50) ,--���������� a.id desc 
     @categoryid  varchar(50) --ͼ�������  
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
  --ͨ�ð汾��ҳ�洢����
	@tablename varchar(50) ,--����
	@fields varchar(100)='*',--Ҫ��ʾ��������Ĭ��*���� * ��  id,name,age 
	@start varchar(10) ='1',--Ĭ����1 ��ʼ����
	@end varchar(10) ='10', --Ĭ����10 ��������
	@sort varchar(50)='id asc' ,--Ĭ�� ������ʽ
	@where varchar(100) --where���� ע������where�ؼ���
)
as
--��̬sql
declare @sql varchar(1000) --����sql��ʱ����
set @sql='
select * from (
select ROW_NUMBER() over(order by id) as rowid,'
+@fields+'
 from '+@tablename +'
   '+@where +') tablea
    where rowid between '+@start+ ' and '+@end
   --print @sql 
  exec(@sql) --��̬sql
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
--�ϲ�������ֻ����ͬһ�û�������δ����
--��Դ�����ϲ���Ŀ�궩����
--�ϲ���Դ����״̬�ĳ�8
@sourceId int, --Ҫ�ϲ���Դ����
@targetId int --�ϲ���Ŀ�궩��
)
as
  declare @userid1 int --Դ������Ա���
  declare @userid2 int --Ŀ�궩����Ա���
  declare @flag1 int --Դ����״̬
  declare @flag2 int --Ŀ�궩��״̬
  --�����α���Դ������ϸ 
  declare  cur_1 cursor for select BookID,Quantity,unitprice from OrderBook where OrderID=@sourceId 
  declare @bookid int--ͼ����
  declare @quantity int--ͼ������
  declare @unitprice numeric(8,2)--����
  declare @count int 
  declare @totalprice numeric(8,2) --�ϲ��󶩵��ܼ�
begin
  select @userid1=userid,@flag1=flag from Orders where ID=@sourceId
  select @userid2=userid,@flag2=flag from Orders where ID=@targetid
  if @userid1!=@userid2 
      return -1 --��ʾ����ͬһ�û�
  if @flag1!=1 and @flag2!=1 
	return -2 --����״̬��Ϊ1
--�������㿪ʼ����ϲ�
--	
 open cur_1 --���α�
fetch next from cur_1 into @bookid,@quantity,@unitprice
begin try
while @@FETCH_STATUS=0 
begin
    --ȡ��Ŀ�궩����ϸ�У���ͼ�������
	select @count=COUNT(*) from OrderBook where BookID=@bookid and OrderID=@targetid
	if @count=0
	   --�����ڣ������
	    insert into OrderBook values(@targetid,@bookid,@quantity,@unitprice)
	 else
	    --�������������
	    update OrderBook set Quantity=Quantity+@quantity where OrderID=@targetid and BookID=@bookid
	 fetch next from cur_1 into @bookid,@quantity,@unitprice
end 
close cur_1
deallocate cur_1
	  update Orders set flag=8 where ID=@sourceid --����Դ����״̬
     select @totalprice=SUM(quantity*unitprice) from OrderBook
     where OrderID=@targetid --����ϲ��󶩵���ϸ�ܼ� 
     --����Ŀ�궩���ܼ�
      update Orders set TotalPrice=	@totalprice where ID=@targetid 
     commit tran --�ύ����
     return 0 --����    
end try
begin catch
	rollback tran--�ع�
	return -3  --�������
end catch
end

