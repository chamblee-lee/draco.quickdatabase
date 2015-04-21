CREATE TABLE [dbo].[User](
	[UID] [varchar](128) NOT NULL PRIMARY KEY,
	[Name] [varchar](128) NULL,
	[Alias] [varchar](128) NULL,
	[Gender] [varchar](50) NULL,
	[Age] [int] NULL DEFAULT ((0)),
	[Birthday] [datetime] NULL,
	[Stature] [float] NULL,
	[Address] [varchar](512) NULL,
	[Photo] [image] NULL,
	[Introduction] [text] NULL,
	[IsLocked] [bit] NULL DEFAULT ((0)),
	[CreateDate] [datetime] NULL
)

