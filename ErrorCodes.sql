USE master;  
GO 

CREATE PROCEDURE dbo.ErrorCodes   
AS   

DELETE es FROM dbo.ErrorStage AS es
INNER JOIN dbo.Error AS e
ON e.ErrorCode=es.ErrorCode
AND e.ErrorMessage=es.ErrorMessage

IF OBJECT_ID('tempdb..#e') IS NOT NULL DROP TABLE #e

SELECT DISTINCT ErrorCode, ErrorMessage
INTO #e
FROM dbo.ErrorStage

DELETE FROM #e
WHERE ErrorCode IN (SELECT ErrorCode FROM #e GROUP BY ErrorCode HAVING COUNT(ErrorCode)>1) 

UPDATE e
SET ErrorMessage=es.ErrorMessage
FROM dbo.Error AS e
INNER JOIN #e es
ON e.ErrorCode=es.ErrorCode

DELETE es FROM #e AS es
INNER JOIN dbo.Error AS e
ON e.ErrorCode=es.ErrorCode

INSERT INTO dbo.Error (ErrorCode, ErrorMessage)
SELECT ErrorCode, ErrorMessage FROM #e

DELETE es FROM dbo.ErrorStage AS es
INNER JOIN dbo.Error AS e
ON e.ErrorCode=es.ErrorCode
AND e.ErrorMessage=es.ErrorMessage

DROP TABLE #e

GO  