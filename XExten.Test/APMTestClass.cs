using System;
using System.Collections.Generic;
using System.Text;
using XExten.APM;
using XExten.XCore;
using Xunit;

namespace XExten.Test
{
    public class APMTestClass
    {
        private void BuildTestData(DataOperation dataOperation)
        {
            dataOperation.Set("内存", 4567, dateTime: DateTimeOffset.Now.AddDays(-1));//上一天的数据
            dataOperation.Set("内存", 6789, dateTime: DateTimeOffset.Now.AddMinutes(-2));

            dataOperation.Set("CPU", .65, dateTime: DateTimeOffset.Now.AddMinutes(-2));
            dataOperation.Set("CPU", .78, dateTime: DateTimeOffset.Now.AddMinutes(-2));
            dataOperation.Set("CPU", .75, dateTime: DateTimeOffset.Now.AddMinutes(-2));
            dataOperation.Set("CPU", .92, dateTime: DateTimeOffset.Now.AddMinutes(-1));
            dataOperation.Set("CPU", .48, dateTime: DateTimeOffset.Now.AddMinutes(-1));

            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-3));
            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-3));
            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-2));
            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-2));
            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-1));
            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now.AddMinutes(-1));

            dataOperation.Set("访问量", 1, dateTime: DateTimeOffset.Now);//当前分钟，将不被收集


        }
        [Fact]
        public void CpuTest() {
          var data =  DataHelper.GetCPUCounter();
        }
        [Fact]
        public void SetAndGetTest() {
            DataOperation dataOperation = new DataOperation("SetAndGetTest");
            BuildTestData(dataOperation);

            var memoryData = dataOperation.GetDataItemList("内存");
            Assert.Equal(2, memoryData.Count);

            var cpuData = dataOperation.GetDataItemList("CPU");
            Assert.Equal(5, cpuData.Count);

            var viewData = dataOperation.GetDataItemList("访问量");
            Assert.Equal(7, viewData.Count);
        }

        [Fact]
        public void ReadAndCleanDataItemsTest()
        {
            DataOperation dataOperation = new DataOperation("ReadAndCleanDataItemsTest");
            BuildTestData(dataOperation);
            var result = dataOperation.ReadAndCleanDataItems(true, false);//清除所有当前分钟前的过期数据

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);//内存、CPU、访问量3个分类
            Console.WriteLine(result.ToJson());
            Console.WriteLine("===============");

            //立即获取，检查是否已经清空当前分钟之前的数据
            var memoryData = dataOperation.GetDataItemList("内存");
            Assert.Empty(memoryData);

            var cpuData = dataOperation.GetDataItemList("CPU");
            Assert.Empty(cpuData);

            var viewData = dataOperation.GetDataItemList("访问量");
            Assert.Single(viewData);//当前分钟的缓存不会被清除

            //模拟当前时间

        }

        [Fact]
        public void ReadAndCleanDataItems_KeepTodayDataTest()
        {
            DataOperation dataOperation = new DataOperation("ReadAndCleanDataItems_KeepTodayDataTest");
            BuildTestData(dataOperation);
            var result = dataOperation.ReadAndCleanDataItems(true, true);//只清除今天之前的记录

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);//内存、CPU、访问量3个分类
            Console.WriteLine(result.ToJson());
            Console.WriteLine("===============");

            //立即获取，检查是否已经清空当前分钟之前的数据
            var memoryData = dataOperation.GetDataItemList("内存");
            Assert.Single(memoryData);//删除1条昨天的数据

            var cpuData = dataOperation.GetDataItemList("CPU");
            Assert.Equal(5, cpuData.Count);//当天数据全部保留

            var viewData = dataOperation.GetDataItemList("访问量");
            Assert.Equal(7, viewData.Count);//当天数据全部保留

            //模拟当前时间

        }
    }
}
