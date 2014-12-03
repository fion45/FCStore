/* ��������� SQL����             */
CREATE TABLE dbo.ELMAH_Error
(
    ErrorId     UNIQUEIDENTIFIER NOT NULL,
    Application NVARCHAR(60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Host        NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Type        NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Source      NVARCHAR(60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Message     NVARCHAR(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [User]      NVARCHAR(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    StatusCode  INT NOT NULL,
    TimeUtc     DATETIME NOT NULL,
    Sequence    INT IDENTITY (1, 1) NOT NULL,
    AllXml      NTEXT COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.ELMAH_Error WITH NOCHECK ADD 
    CONSTRAINT PK_ELMAH_Error PRIMARY KEY NONCLUSTERED
    (
        ErrorId
    )  ON [PRIMARY] 
GO

ALTER TABLE dbo.ELMAH_Error ADD 
    CONSTRAINT DF_ELMAH_Error_ErrorId DEFAULT (newid()) FOR [ErrorId]
GO

CREATE NONCLUSTERED INDEX IX_ELMAH_Error_App_Time_Seq ON dbo.ELMAH_Error
(
    [Application] ASC,
    [TimeUtc] DESC,
    [Sequence] DESC
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.ELMAH_GetErrorXml
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

SET NOCOUNT ON

SELECT 
    AllXml
FROM 
    ELMAH_Error
WHERE
    ErrorId = @ErrorId
AND
    Application = @Application



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.ELMAH_GetErrorsXml
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

SET NOCOUNT ON

DECLARE @FirstTimeUTC DateTime
DECLARE @FirstSequence int
DECLARE @StartRow int
DECLARE @StartRowIndex int

-- Get the ID of the first error for the requested page

SET @StartRowIndex = @PageIndex * @PageSize + 1
SET ROWCOUNT @StartRowIndex

SELECT  
    @FirstTimeUTC = TimeUTC,
    @FirstSequence = Sequence
FROM 
    ELMAH_Error
WHERE   
    Application = @Application
ORDER BY 
    TimeUTC DESC, 
    Sequence DESC

-- Now set the row count to the requested page size and get
-- all records below it for the pertaining application.

SET ROWCOUNT @PageSize

SELECT 
    @TotalCount = COUNT(1) 
FROM 
    ELMAH_Error
WHERE 
    Application = @Application

SELECT 
    errorId, 
    application,
    host, 
    type,
    source,
    message,
    [user],
    statusCode, 
    CONVERT(VARCHAR(50), TimeUtc, 126) + 'Z' time
FROM 
    ELMAH_Error error
WHERE
    Application = @Application
AND 
    TimeUTC <= @FirstTimeUTC
AND 
    Sequence <= @FirstSequence
ORDER BY
    TimeUTC DESC, 
    Sequence DESC
FOR
    XML AUTO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE dbo.ELMAH_LogError
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

SET NOCOUNT ON

INSERT
INTO
    ELMAH_Error
    (
        ErrorId,
        Application,
        Host,
        Type,
        Source,
        Message,
        [User],
        AllXml,
        StatusCode,
        TimeUtc
    )
VALUES
    (
        @ErrorId,
        @Application,
        @Host,
        @Type,
        @Source,
        @Message,
        @User,
        @AllXml,
        @StatusCode,
        @TimeUtc
    )

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO