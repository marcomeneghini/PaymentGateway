USE [PaymentGateway.Api]
GO

/****** Object:  Table [dbo].[Merchants]    Script Date: 05/07/2020 13:10:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Merchants](
	[Id] [uniqueidentifier] NOT NULL,
	[Denomination] [nvarchar](max) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[SortCode] [nvarchar](max) NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_Merchants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [PaymentGateway.Api]
GO

/****** Object:  Table [dbo].[PaymentStatuses]    Script Date: 05/07/2020 13:10:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PaymentStatuses](
	[PaymentId] [uniqueidentifier] NOT NULL,
	[RequestId] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_PaymentStatuses] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


