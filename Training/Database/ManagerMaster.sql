IF OBJECT_ID('dbo.ManagerMaster','U') IS NOT NULL
BEGIN
IF COL_LENGTH('dbo.ManagerMaster','ManagerID') IS NULL
ALTER TABLE dbo.ManagerMaster ADD ManagerID NVARCHAR(20) NULL;
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name='UQ_ManagerMaster_EmpID' AND parent_object_id=OBJECT_ID('dbo.ManagerMaster'))
ALTER TABLE dbo.ManagerMaster DROP CONSTRAINT UQ_ManagerMaster_EmpID;
IF COL_LENGTH('dbo.ManagerMaster','EmpName') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN EmpName;
IF COL_LENGTH('dbo.ManagerMaster','DOB') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN DOB;
IF COL_LENGTH('dbo.ManagerMaster','DOJ') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN DOJ;
IF COL_LENGTH('dbo.ManagerMaster','MobileNo') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN MobileNo;
IF COL_LENGTH('dbo.ManagerMaster','EmailID') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN EmailID;
IF COL_LENGTH('dbo.ManagerMaster','PlaceOfPosting') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN PlaceOfPosting;
IF COL_LENGTH('dbo.ManagerMaster','Designation') IS NOT NULL
ALTER TABLE dbo.ManagerMaster DROP COLUMN Designation;
UPDATE ManagerMaster SET ManagerID='MGR'+RIGHT('0000'+CAST(ID AS VARCHAR(20)),4) WHERE ISNULL(ManagerID,'')='';
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UQ_ManagerMaster_ManagerID' AND object_id=OBJECT_ID('dbo.ManagerMaster'))
CREATE UNIQUE INDEX UQ_ManagerMaster_ManagerID ON dbo.ManagerMaster(ManagerID);
ALTER TABLE dbo.ManagerMaster ALTER COLUMN ManagerID NVARCHAR(20) NOT NULL;
END
ELSE
BEGIN
CREATE TABLE dbo.ManagerMaster (
    ID INT IDENTITY(1,1) NOT NULL,
    ManagerID NVARCHAR(20) NOT NULL,
    EmpID NVARCHAR(150) NOT NULL,
    MapForLocation NVARCHAR(150) NULL,
    TrainingLocationID NVARCHAR(100) NOT NULL,
    CreatedOn DATETIME NOT NULL CONSTRAINT DF_ManagerMaster_CreatedOn DEFAULT (GETDATE()),
    CreatedBy NVARCHAR(20) NULL,
    ActiveStatus NVARCHAR(20) NULL,
    CONSTRAINT PK_ManagerMaster PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT UQ_ManagerMaster_ManagerID UNIQUE (ManagerID)
);
END