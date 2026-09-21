IF COL_LENGTH('dbo.TrainingDetails','CertificateEligibilityMode') IS NULL
BEGIN
    ALTER TABLE dbo.TrainingDetails ADD CertificateEligibilityMode NVARCHAR(10) NOT NULL CONSTRAINT DF_TrainingDetails_CertificateEligibilityMode DEFAULT('ALL');
END
GO

UPDATE dbo.TrainingDetails
SET CertificateEligibilityMode='ALL'
WHERE CertificateEligibilityMode IS NULL OR CertificateEligibilityMode NOT IN ('ALL','PASS','FAIL');
GO