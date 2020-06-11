-- ----------------------------
-- Table structure for ]Events]
-- ----------------------------
CREATE TABLE [Events] (
  [SourceId] uniqueidentifier NOT NULL,
  [Version] int NOT NULL,
  [Timestamp] datetime NOT NULL,
  [Type] varchar(1000) NOT NULL,
  [Data] varchar(5000) NOT NULL,
  CONSTRAINT [sqlite_autoindex_Events_1] PRIMARY KEY ([SourceId], [Version])
) 

-- ----------------------------
-- Table structure for ]Snapshots]
-- ----------------------------
CREATE TABLE [Snapshots] (
  [SourceId] uniqueidentifier NOT NULL,
  [Version] int NOT NULL,
  [Timestamp] datetime NOT NULL,
  [Type] varchar(1000) NOT NULL,
  [Data] varchar(5000) NOT NULL,
  CONSTRAINT [sqlite_autoindex_Snapshots_1] PRIMARY KEY ([SourceId], [Version])
)
