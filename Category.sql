USE master;  
GO 

CREATE PROCEDURE dbo.Category  
AS   

DELETE cs FROM dbo.CategoriesStage AS cs
INNER JOIN dbo.Categories AS c
ON c.ID=cs.ID
AND c.CategoryName=cs.CategoryName
AND c.Parent=cs.Parent
AND c.CategoryImage=cs.CategoryImage

IF OBJECT_ID('tempdb..#c') IS NOT NULL DROP TABLE #c

SELECT DISTINCT ID, CategoryName,Parent,CategoryImage
INTO #c
FROM dbo.CategoriesStage

DELETE FROM #c
WHERE ID IN (SELECT ID FROM #c GROUP BY ID HAVING COUNT(ID)>1) 

UPDATE c
SET CategoryName=cs.CategoryName
,Parent=cs.Parent
,CategoryImage=cs.CategoryImage
FROM dbo.Categories AS c
INNER JOIN #c AS cs
ON c.ID=cs.ID

DELETE cs FROM #c AS cs
INNER JOIN dbo.Categories AS c
ON c.ID=cs.ID

INSERT INTO dbo.Categories (ID,CategoryName,Parent,CategoryImage)
SELECT ID,CategoryName,Parent,CategoryImage FROM #c

DELETE cs FROM dbo.CategoriesStage AS cs
INNER JOIN dbo.Categories AS c
ON c.ID=cs.ID
AND c.CategoryName=cs.CategoryName
AND c.Parent=cs.Parent
AND c.CategoryImage=cs.CategoryImage

DROP TABLE #c

GO  