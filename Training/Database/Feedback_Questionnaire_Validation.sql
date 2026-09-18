/*
    Feedback questionnaire validation
    ---------------------------------
    A trainee must not be able to submit batch feedback when no active
    questionnaire question has been assigned to that training.

    This is a server-side DB safety net in addition to the page-level check.
*/

IF OBJECT_ID('dbo.TR_Feedback_RequireQuestionnaire', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER dbo.TR_Feedback_RequireQuestionnaire;
END
GO

CREATE TRIGGER dbo.TR_Feedback_RequireQuestionnaire
ON dbo.Feedback
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted I
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM TrainingFeedbackCategory TFC
            INNER JOIN FeedbackQuestionMaster FQM
                ON FQM.CategoryID = TFC.CategoryID
            WHERE TFC.TrainingID = I.TrainingID
              AND FQM.Active = 1
        )
    )
    BEGIN
        RAISERROR
        (
            'Feedback cannot be submitted because no active questionnaire is assigned to this training.',
            16,
            1
        );

        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO
