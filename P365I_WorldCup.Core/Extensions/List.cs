﻿using System.Collections.Generic;
using System.Linq;

namespace P365I_WorldCup.Core.Extensions
{
    public static class List
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            //Usage: List<List<Entity>> splittedLists = updateEntities.ChunkBy<Entity>(50);
            return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
        }
    }
}
