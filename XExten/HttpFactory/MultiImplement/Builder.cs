using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class Builder : IBuilder
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public IBuilder Build()
        {


            return this;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public List<string> Run()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> RunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
