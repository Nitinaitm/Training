/* Training requirement skip controls
   Run once on the Training database. */

IF COL_LENGTH('dbo.SessionMaster','AttendanceSkipped') IS NULL
    ALTER TABLE dbo.SessionMaster ADD AttendanceSkipped bit NOT NULL CONSTRAINT DF_SessionMaster_AttendanceSkipped DEFAULT(0);

IF COL_LENGTH('dbo.SessionMaster','AttendanceSkipReason') IS NULL
    ALTER TABLE dbo.SessionMaster ADD AttendanceSkipReason nvarchar(500) NULL;

IF COL_LENGTH('dbo.SessionMaster','AttendanceSkipBy') IS NULL
    ALTER TABLE dbo.SessionMaster ADD AttendanceSkipBy nvarchar(100) NULL;

IF COL_LENGTH('dbo.SessionMaster','AttendanceSkipOn') IS NULL
    ALTER TABLE dbo.SessionMaster ADD AttendanceSkipOn datetime NULL;

IF COL_LENGTH('dbo.SessionMaster','PreAssessmentSkipped') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PreAssessmentSkipped bit NOT NULL CONSTRAINT DF_SessionMaster_PreAssessmentSkipped DEFAULT(0);

IF COL_LENGTH('dbo.SessionMaster','PreAssessmentSkipReason') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PreAssessmentSkipReason nvarchar(500) NULL;

IF COL_LENGTH('dbo.SessionMaster','PreAssessmentSkipBy') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PreAssessmentSkipBy nvarchar(100) NULL;

IF COL_LENGTH('dbo.SessionMaster','PreAssessmentSkipOn') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PreAssessmentSkipOn datetime NULL;

IF COL_LENGTH('dbo.SessionMaster','PostAssessmentSkipped') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PostAssessmentSkipped bit NOT NULL CONSTRAINT DF_SessionMaster_PostAssessmentSkipped DEFAULT(0);

IF COL_LENGTH('dbo.SessionMaster','PostAssessmentSkipReason') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PostAssessmentSkipReason nvarchar(500) NULL;

IF COL_LENGTH('dbo.SessionMaster','PostAssessmentSkipBy') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PostAssessmentSkipBy nvarchar(100) NULL;

IF COL_LENGTH('dbo.SessionMaster','PostAssessmentSkipOn') IS NULL
    ALTER TABLE dbo.SessionMaster ADD PostAssessmentSkipOn datetime NULL;

IF COL_LENGTH('dbo.TrainingDetails','FeedbackSkipped') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD FeedbackSkipped bit NOT NULL CONSTRAINT DF_TrainingDetails_FeedbackSkipped DEFAULT(0);

IF COL_LENGTH('dbo.TrainingDetails','FeedbackSkipReason') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD FeedbackSkipReason nvarchar(500) NULL;

IF COL_LENGTH('dbo.TrainingDetails','FeedbackSkipBy') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD FeedbackSkipBy nvarchar(100) NULL;

IF COL_LENGTH('dbo.TrainingDetails','FeedbackSkipOn') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD FeedbackSkipOn datetime NULL;

IF COL_LENGTH('dbo.TrainingDetails','CertificateSkipped') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD CertificateSkipped bit NOT NULL CONSTRAINT DF_TrainingDetails_CertificateSkipped DEFAULT(0);

IF COL_LENGTH('dbo.TrainingDetails','CertificateSkipReason') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD CertificateSkipReason nvarchar(500) NULL;

IF COL_LENGTH('dbo.TrainingDetails','CertificateSkipBy') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD CertificateSkipBy nvarchar(100) NULL;

IF COL_LENGTH('dbo.TrainingDetails','CertificateSkipOn') IS NULL
    ALTER TABLE dbo.TrainingDetails ADD CertificateSkipOn datetime NULL;
GO

/* Database-level safety: when attendance is required and a session is not skipped,
   a trainee cannot start a Pre/Post test unless attendance is Present. */
IF OBJECT_ID('dbo.trg_TestAttempt_AttendanceGate','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_TestAttempt_AttendanceGate;
GO

CREATE TRIGGER dbo.trg_TestAttempt_AttendanceGate
ON dbo.TestAttempt
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted I
        INNER JOIN dbo.TestMaster TM ON TM.TestID = I.TestID
        INNER JOIN dbo.SessionMaster SM ON SM.SessionID = TM.SessionID
        INNER JOIN dbo.TrainingDetails TD ON TD.TrainingID = SM.TrainingID
        WHERE TM.TestType IN ('Pre','Post')
          AND ISNULL(TD.AttendanceRequired,0) = 1
          AND ISNULL(SM.AttendanceSkipped,0) = 0
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.SessionAttendance SA
              WHERE SA.SessionID = SM.SessionID
                AND SA.EmpID = I.EmpID
                AND SA.AttendanceStatus = 'Present'
          )
    )
    BEGIN
        RAISERROR('Trainee cannot take Pre/Post Test because attendance is not Present for this session.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO