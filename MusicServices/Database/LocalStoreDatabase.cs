﻿using System;
using LiteDB;

namespace MusicServices.Database
{
    public partial class LocalStoreDatabase : IDisposable
    {
        private readonly LiteDatabase myStoreDatabase;

        internal readonly ILiteCollection<StoredLyric> myStoredLyrics;

        public LocalStoreDatabase()
        {
            var connectionType = ConnectionType.Direct;

            myStoreDatabase = new LiteDatabase(new ConnectionString { Filename = $"./lyric-store.db", Connection = connectionType });

            myStoreDatabase.Mapper.EmptyStringToNull = false;

            myStoredLyrics = myStoreDatabase.GetCollection<StoredLyric>("avatars");
        }

        public void Dispose()
        {
            myStoreDatabase.Dispose();
        }
    }
}
