IF COL_LENGTH('dbo.TrainingDetails','PreTestCertificateRule') IS NULL
BEGIN
    ALTER TABLE dbo.TrainingDetails ADD PreTestCertificateRule NVARCHAR(10) NULL;
END
GO

IF COL_LENGTH('dbo.TrainingDetails','PostTestCertificateRule') IS NULL
BEGIN
    ALTER TABLE dbo.TrainingDetails ADD PostTestCertificateRule NVARCHAR(10) NULL;
END
GO

UPDATE dbo.TrainingDetails
SET PreTestCertificateRule=NULL
WHERE PreTestCertificateRule IS NOT NULL AND PreTestCertificateRule NOT IN ('ALL','PASS');
GO

UPDATE dbo.TrainingDetails
SET PostTestCertificateRule=NULL
WHERE PostTestCertificateRule IS NOT NULL AND PostTestCertificateRule NOT IN ('ALL','PASS');
GO
