declare @a int --��������
set @a=10 --��ֵ
declare @bookid int
declare @title varchar(50)
declare @author varchar(50)
set @bookid=4958
--Ҳ����ֱ�Ӵ����ݱ��и�ֵ
print @a --��ʾ
select @title=title,@author=author from Books where id=@bookid
select @title,@author --��ʾ
go  --����һ�������������֮ǰ����ľֲ���������
--������ʾif���
declare @year int
--�����ڲ�����,getdate��ǰ���ڣ�datepart������ڵ�һ����
--�õ���ǰ���
set @year= DATEPART(year, GETDATE())
if (@year % 4=0  and @year % 100!=0 ) or @year %400=0
   begin
     --cast����������ת�� 
		print cast(@year as varchar(50))+'������'
   end
else
 begin
    print cast(@year as varchar(50))+'��������'
 end
go
----------------------------------------
--ѭ��
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
--�α��ʹ��
--�α���һ�ֲ������ݱ�Ķ��󣬷ǳ���ado.net datareader����
--datareader�����ʹ��
-- �����ӣ�����cmd������select��䣬cmd.ExecuteDatareader
-- while(dr.Read()){
    -- name=dr["name"].ToString()
--}
--dr.close,  conn.close()
declare @cid int 
declare @title varchar(50) 
declare @unitprice numeric(10,2)
set @cid=25
--�����α���������ֽ�cur��
declare  cur cursor  for
 select title,unitprice from Books where CategoryId=@cid
--���α�
open cur --���α� �մ�ʱ��ָ���һ������һ��
--��ȡ��һ����¼
fetch next from cur into @title,@unitprice
--@@fetch_status ����ȫ�ֱ������������ָʾ���α��ȡ״̬
-- 0����ɹ�
--�����ȡ�ɹ�����ѭ��
while @@FETCH_STATUS=0
begin
  --�������ݣ�����ֻ�Ǽ򵥴�ӡ
	print @title+cast(@unitprice as varchar(50))
	fetch next from cur into @title,@unitprice --������ȡ��һ��
end
close cur --�ر��α꣬����رպ󣬿����ٵ���open��
deallocate cur--�ͷ��α�,�ͷź��α����û���ˣ�������open
go
=============================================























print 10%3