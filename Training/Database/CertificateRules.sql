IF COL_LENGTH('dbo.SessionMaster','PreTestCertificateRule') IS NULL
BEGIN
    ALTER TABLE dbo.SessionMaster ADD PreTestCertificateRule NVARCHAR(10) NULL;
END
GO

IF COL_LENGTH('dbo.SessionMaster','PostTestCertificateRule') IS NULL
BEGIN
    ALTER TABLE dbo.SessionMaster ADD PostTestCertificateRule NVARCHAR(10) NULL;
END
GO

IF COL_LENGTH('dbo.TrainingDetails','MinimumAttendancePercentage') IS NULL
BEGIN
    ALTER TABLE dbo.TrainingDetails ADD MinimumAttendancePercentage DECIMAL(5,2) NULL;
END
GO

UPDATE dbo.SessionMaster
SET PreTestCertificateRule=NULL
WHERE PreTestCertificateRule IS NOT NULL AND PreTestCertificateRule NOT IN ('ALL','PASS');
GO

UPDATE dbo.SessionMaster
SET PostTestCertificateRule=NULL
WHERE PostTestCertificateRule IS NOT NULL AND PostTestCertificateRule NOT IN ('ALL','PASS');
GO

UPDATE dbo.TrainingDetails
SET MinimumAttendancePercentage=NULL
WHERE MinimumAttendancePercentage IS NOT NULL AND (MinimumAttendancePercentage < 0 OR MinimumAttendancePercentage > 100);
GO