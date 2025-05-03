SELECT * FROM sys.columns
WHERE object_id = object_id('Article')

SELECT
    OBJECT_NAME(referenced_object_id) as 'Referenced Object',
    OBJECT_NAME(parent_object_id) as 'Referencing Object',
    COL_NAME(parent_object_id, parent_column_id) as 'Referencing Column Name',
    OBJECT_NAME(constraint_object_id) 'Constraint Name'
FROM sys.foreign_key_columns
WHERE OBJECT_NAME(referenced_object_id) = 'Article'


SELECT
	parent_object_id,
	parent_column_id AS column_id,
	COL_NAME(parent_object_id, parent_column_id) as 'PK',
	OBJECT_NAME(referenced_object_id) as 'Referenced Object'

FROM sys.foreign_key_columns
WHERE parent_object_id = object_id('Article')


