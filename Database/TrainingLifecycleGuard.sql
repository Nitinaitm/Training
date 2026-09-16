/*
    Training lifecycle guard
    - Before final completion: Admin may add trainees/sessions.
    - After final completion: trainee/session/attendance/requirement changes are blocked.
    - If a new trainee or session is added after attendance was completed,
      attendance is reopened so the new requirement can be completed.
*/

IF OBJECT_ID('dbo.trg_TrainingDetails_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_TrainingDetails_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_TrainingDetails_LifecycleGuard
ON dbo.TrainingDetails
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    /* The transition INTO Completed is allowed. Any later update is blocked. */
    IF EXISTS
    (
        SELECT 1
        FROM deleted D
        WHERE ISNULL(D.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(D.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. Training details cannot be changed.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;
END
GO

IF OBJECT_ID('dbo.trg_TrainingAssignment_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_TrainingAssignment_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_TrainingAssignment_LifecycleGuard
ON dbo.TrainingAssignment
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM
        (
            SELECT TrainingID FROM inserted
            UNION
            SELECT TrainingID FROM deleted
        ) X
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = X.TrainingID
        WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. Trainee assignment cannot be changed.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    /* Only a new trainee assignment reopens attendance. */
    IF EXISTS (SELECT 1 FROM inserted)
       AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
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
END
GO

IF OBJECT_ID('dbo.trg_SessionMaster_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_SessionMaster_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_SessionMaster_LifecycleGuard
ON dbo.SessionMaster
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM
        (
            SELECT TrainingID FROM inserted
            UNION
            SELECT TrainingID FROM deleted
        ) X
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = X.TrainingID
        WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
           OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    BEGIN
        RAISERROR('Training is already completed. Session cannot be changed.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    /* A newly added session creates a new attendance requirement. */
    IF EXISTS (SELECT 1 FROM inserted)
       AND NOT EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE TD
           SET TD.WorkflowStatus = 'E',
               TD.TrainingStatus = 'InProgress',
               TD.UpdatedOn = GETDATE(),
               TD.UpdatedBy = 'System'
        FROM dbo.TrainingDetails TD
        INNER JOIN inserted I
            ON I.TrainingID = TD.TrainingID;
    END
END
GO

IF OBJECT_ID('dbo.trg_SessionAttendance_LifecycleGuard','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_SessionAttendance_LifecycleGuard;
GO

CREATE TRIGGER dbo.trg_SessionAttendance_LifecycleGuard
ON dbo.SessionAttendance
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM
        (
            SELECT TrainingID FROM inserted
            UNION
            SELECT TrainingID FROM deleted
        ) X
        INNER JOIN dbo.TrainingDetails TD
            ON TD.TrainingID = X.TrainingID
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
