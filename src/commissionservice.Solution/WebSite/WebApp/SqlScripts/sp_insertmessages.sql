开启监听数据修改功能
ALTER DATABASE [aspnet-Demo] SET NEW_BROKER
ALTER DATABASE [aspnet-Demo] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE

GRANT SUBSCRIBE QUERY NOTIFICATIONS TO "DESKTOP-RH4DFBA\office"
ALTER AUTHORIZATION ON DATABASE::[aspnet-Demo] TO "DESKTOP-RH4DFBA\office";
select * from sys.transmission_queue
select * from sys.dm_qn_subscriptions

----------------------------------------------------------------
--新增序列表
----------------------------------------------------------------
USE [webappv4]
GO

/****** Object:  Table [dbo].[Sequence]    Script Date: 2020/7/29 16:48:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sequence](
	[prefix] [nvarchar](10) NOT NULL,
	[seed] [int] NULL,
 CONSTRAINT [PK_Sequence] PRIMARY KEY CLUSTERED 
(
	[prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [DF_Sequence_seed]  DEFAULT ((0)) FOR [seed]
GO
---------------------------------------------------------------------
--新增存储过过程
---------------------------------------------------------------------
USE [webappv4]
GO

/****** Object:  StoredProcedure [dbo].[SP_NextVal]    Script Date: 2020/7/29 16:49:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_NextVal](@prefix nvarchar(10))
as
begin  
declare @val int=1;
IF NOT EXISTS (SELECT * FROM [dbo].[Sequence] WHERE prefix = @prefix)
 begin
  begin try
      INSERT INTO [dbo].[Sequence]
        (prefix, seed)
      VALUES (@prefix, @val);
      select @val
  end try
  begin catch
    update [dbo].[Sequence] set seed=seed+1,@val=seed+1 where prefix = @prefix
    select @val
  end catch
 end
else
 begin
 --WAITFOR DELAY '00:00:10.000';
 update [dbo].[Sequence] set seed=seed+1,@val=seed+1 where prefix = @prefix
 select @val
 end
end 
GO
---------------------------------------------------
--[SP_InsertCovids19]
---------------------------------------------------
USE [webappv4]
GO

/****** Object:  StoredProcedure [dbo].[SP_InsertCovids19]    Script Date: 2020/7/29 16:51:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertCovids19] 
	-- Add the parameters for the stored procedure here
	 @country nvarchar(100),
	 @province nvarchar(100),
	 @latitude decimal(18,5),
	 @longitude decimal(18,5),
	 @date date,
	 @confirmed int,
	 @deaths int,
	 @recovered int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [id] FROM [dbo].[Covid19] WHERE [country] = @country and [province] =@province and [date]=@date ) 
	INSERT INTO [dbo].[Covid19]
           ([country]
		   ,[province]
		   ,[latitude]
		   ,[longitude]
           ,[date]
           ,[confirmed]
           ,[deaths]
           ,[recovered]
           ,[TenantId]
		   ,[CreatedBy]
		   ,[CreatedDate])
     VALUES
           (@country
		   ,@province
		   ,@latitude
		   ,@longitude
           ,@date
           ,@confirmed
           ,@deaths
           ,@recovered
		   ,1
		   ,'Hangfire'
		   ,GETDATE()
           )

END
GO



