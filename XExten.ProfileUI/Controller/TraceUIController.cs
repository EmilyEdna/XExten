using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XExten.ProfileUI.DbFactory;
using XExten.XCore;

namespace XExten.ProfileUI.Controller
{
    [Controller(BaseUrl = "/Trace")]
    public class TraceUIController
    {
        [Post]
        [JsonDataConvert]
        public Object GetTraceData(int PageIndex, int PageSize)
        {
            using MemoryDb db = new MemoryDb();
            var data = db.Traces
                .Where(t => t.CreateTime > Convert.ToDateTime(DateTime.Now.AddDays(-1).ToShortDateString()))
                .Where(t => t.CreateTime <= Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString()))
                .OrderByDescending(t => t.CreateTime).Select(t => t.Result.ToModel<JObject>()).ToPage(PageIndex, PageSize);
            return data;
        }

        [Get]
        public void ClearMemory()
        {
            using MemoryDb db = new MemoryDb();
            var Trace = db.Traces.ToList();
            if (Trace.Count > 500)
            {
                db.Traces.RemoveRange(Trace);
                db.SaveChanges();
            }

        }
    }
}
