using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Whizbang.Core.Data.FluentSqlClient;
using Whizbang.Core.EventSource.Storage;
using Whizbang.Core.Exceptions;


namespace Whizbang.EventStore.Sqlite
{
    public class EventStorageProvider : EventStorageBase
    {
        private readonly string _connString;
        private readonly IExecutor _transExecutor;

        public EventStorageProvider()
        {
            ConnectionStringSettings connStr = ConfigurationManager.ConnectionStrings["EventStore"];

            if (null == connStr || string.IsNullOrWhiteSpace(connStr.ConnectionString))
            {
                throw new NotFoundException("未配置连接字符串：EventStore！");
            }

            _connString = connStr.ConnectionString;

            _transExecutor = EventStore.Sqlite.Sqlite.With(connStr.ConnectionString);
        }


        protected override IEnumerable<EventObject> LoadEventObjects(Guid sourceId, int skipVersion)
        {
            var eventObjects = new List<EventObject>();

            EventStore.Sqlite.Sqlite.With(_connString)
                .Reader(Queries.GetEventsSql,
                    new Dictionary<string, object>
                    {
                        {"SourceId", sourceId},
                        {"SkipVersion", skipVersion},
                    },
                    rdr =>
                    {
                        /*
public const string GetEventsSql = @"
SELECT [SourceId]
      ,[Version]
      ,[Timestamp]
      ,[Type]
      ,[Data]
  FROM [EventStore].[dbo].[Events]
  WHERE [SourceId] = @SourceId 
  AND [Version] > @SkipVersion 
";
                         */

                        var obj = new EventObject
                        {
                            SourceId = rdr.GetGuid(0),
                            Version = rdr.GetInt32(1),
                            Timestamp = rdr.GetDateTime(2),
                            Type = rdr.GetString(3),
                            Data = rdr.GetString(4),
                        };

                        eventObjects.Add(obj);
                    })
                .Execute();

            return eventObjects;
        }

        protected override void PersistEvent(IEnumerable<EventObject> eventObjects, Guid aggregateId,
            int expectedVersion)
        {
            foreach (var snapshotObject in eventObjects)
            {
                _transExecutor.NonQuery(Queries.InsertEventSql,
                    new Dictionary<string, object>
                    {
                        {"SourceId", snapshotObject.SourceId},
                        {"Version", snapshotObject.Version},
                        {"Timestamp", snapshotObject.Timestamp},
                        {"Type", snapshotObject.Type},
                        {"Data", snapshotObject.Data},
                    },
                    rowsAffected =>
                    {
                        const int expectedRows = 1;

                        if (expectedRows != rowsAffected)
                            throw new AffectedRowsException(rowsAffected, expectedRows, "事件源");
                    });
            }

            _transExecutor.Execute();
        }

        protected override SnapshotObject LoadSnapshotObject(Guid sourceId)
        {
            SnapshotObject snapshot = null;

            EventStore.Sqlite.Sqlite.With(_connString)
                .Reader(
                    Queries.GetSnapshotSql,
                    new Dictionary<string, object>
                    {
                        {"SourceId", sourceId}
                    },
                    rdr =>
                    {
                        //if(!rdr.Read())return;

                        snapshot = new SnapshotObject
                        {
                            SourceId = rdr.GetGuid(0),
                            Version = rdr.GetInt32(1),
                            Timestamp = rdr.GetDateTime(2),
                            Type = rdr.GetString(3),
                            Data = rdr.GetString(4),
                        };
                    })
                .Execute();

            return snapshot;
        }

        protected override void PersistSnapshot(SnapshotObject snapshotObject)
        {
            _transExecutor
                .NonQuery(
                    Queries.InsertSnapshotSql,
                    new Dictionary<string, object>
                    {
                        {"SourceId", snapshotObject.SourceId},
                        {"Version", snapshotObject.Version},
                        {"Timestamp", snapshotObject.Timestamp},
                        {"Type", snapshotObject.Type},
                        {"Data", snapshotObject.Data},
                    },
                    rowsAffected =>
                    {
                        const int expectedRows = 1;

                        if (expectedRows != rowsAffected)
                            throw new AffectedRowsException(rowsAffected, expectedRows, "快照源");
                    });

            //trans with persist events
        }
    }
}