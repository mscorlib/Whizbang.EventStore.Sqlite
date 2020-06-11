-- ----------------------------
-- Table structure for `Events`
-- ----------------------------
DROP TABLE IF EXISTS `Events`;
CREATE TABLE `Events` (
  `SourceId` binary(16) NOT NULL,
  `Version` int(11) NOT NULL,
  `Timestamp` datetime NOT NULL,
  `Type` varchar(1000) NOT NULL,
  `Data` varchar(5000) NOT NULL,
  PRIMARY KEY (`SourceId`,`Version`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for `Snapshots`
-- ----------------------------
DROP TABLE IF EXISTS `Snapshots`;
CREATE TABLE `Snapshots` (
  `SourceId` binary(16) NOT NULL,
  `Version` int(11) NOT NULL,
  `Timestamp` datetime NOT NULL,
  `Type` varchar(1000) NOT NULL,
  `Data` varchar(5000) NOT NULL,
  PRIMARY KEY (`SourceId`,`Version`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
