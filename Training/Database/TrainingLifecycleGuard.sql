/*
    Training lifecycle guard
    - Before final completion: Admin may add trainees/sessions.
    - After final completion: trainee/session/attendance changes are blocked.
    - If a new trainee or session is added after attendance was completed,
      attendance is reopened so the new requirement can be completed.
*/

IF OBJECT_ID('dbo.trg_TrainingAssignment_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_TrainingAssignment_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_TrainingAssignment_LifecycleGuard
ON dbo.TrainingAssignment
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted I
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = I.TrainingID
        WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. New trainee cannot be added.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    /* A newly added trainee creates a new attendance requirement. */
    UPDATE SM
       SET SM.AttendanceStatus = NULL,
           SM.AttendanceCompletedOn = NULL,
           SM.AttendanceCompletedBy = NULL
    FROM dbo.SessionMaster SM
    INNER JOIN inserted I
        ON I.TrainingID = SM.TrainingID;

    UPDATE TD
       SET TD.WorkflowStatus = 'E',
           TD.TrainingStatus = 'InProgress',
           TD.UpdatedOn = GETDATE(),
           TD.UpdatedBy = 'System'
    FROM dbo.TrainingDetails TD
    INNER JOIN inserted I
        ON I.TrainingID = TD.TrainingID;
END
GO

IF OBJECT_ID('dbo.trg_SessionMaster_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_SessionMaster_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_SessionMaster_LifecycleGuard
ON dbo.SessionMaster
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted I
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = I.TrainingID
        WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. New session cannot be added.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    /* A newly added session creates a new attendance requirement. */
    UPDATE TD
       SET TD.WorkflowStatus = 'E',
           TD.TrainingStatus = 'InProgress',
           TD.UpdatedOn = GETDATE(),
           TD.UpdatedBy = 'System'
    FROM dbo.TrainingDetails TD
    INNER JOIN inserted I
        ON I.TrainingID = TD.TrainingID;
END
GO

IF OBJECT_ID('dbo.trg_SessionAttendance_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_SessionAttendance_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_SessionAttendance_LifecycleGuard
ON dbo.SessionAttendance
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted I
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = I.TrainingID
        WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. Attendance cannot be changed.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;
END
GO
