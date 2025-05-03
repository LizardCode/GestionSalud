 select 
        replace(col.name, ' ', '_') ColumnName,
        case typ.name 
            when 'bigint' then 'long'
            when 'binary' then 'byte[]'
            when 'bit' then 'bool'
            when 'char' then 'String'
            when 'date' then 'DateTime'
            when 'datetime' then 'DateTime'
            when 'datetime2' then 'DateTime'
            when 'datetimeoffset' then 'DateTimeOffset'
            when 'decimal' then 'decimal'
            when 'float' then 'float'
            when 'image' then 'byte[]'
            when 'int' then 'int'
            when 'money' then 'decimal'
            when 'nchar' then 'char'
            when 'ntext' then 'string'
            when 'numeric' then 'decimal'
            when 'nvarchar' then 'string'
            when 'real' then 'double'
            when 'smalldatetime' then 'DateTime'
            when 'smallint' then 'short'
            when 'smallmoney' then 'decimal'
            when 'text' then 'string'
            when 'time' then 'TimeSpan'
            when 'timestamp' then 'DateTime'
            when 'tinyint' then 'byte'
            when 'uniqueidentifier' then 'Guid'
            when 'varbinary' then 'byte[]'
            when 'varchar' then 'string'
            else 'UNKNOWN_' + typ.name
        END + CASE WHEN col.is_nullable=1 AND typ.name NOT IN ('binary', 'varbinary', 'image', 'text', 'ntext', 'varchar', 'nvarchar', 'char', 'nchar') THEN '?' ELSE '' END ColumnTypeNet , 
		CASE WHEN col.is_nullable=1 AND typ.name NOT IN ('binary', 'varbinary', 'image', 'text', 'ntext', 'varchar', 'nvarchar', 'char', 'nchar') THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END IsNullable,
		CASE WHEN typ.name IN ('text', 'ntext', 'varchar', 'nvarchar', 'char', 'nchar') THEN col.max_length ELSE NULL END  AS MaxLengthString,
		col.is_identity AS IsIdentity,
		CASE WHEN fk.fk_column IS NOT NULL THEN CONVERT(BIT, 1) ELSE CONVERT(BIT, 0) END  AS IsForeignKey,
		fk.ReferencedTable
    from sys.columns col
        join sys.types typ ON  col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id  
		LEFT JOIN (
				SELECT
					parent_object_id,
					parent_column_id AS column_id,
					COL_NAME(parent_object_id, parent_column_id) as fk_column,	
					OBJECT_NAME(referenced_object_id) as ReferencedTable
			FROM sys.foreign_key_columns

		) fk ON col.object_id = fk.parent_object_id AND col.column_id = fk.column_id
		
    where col.object_id = object_id('Article')
	ORDER BY col.column_id