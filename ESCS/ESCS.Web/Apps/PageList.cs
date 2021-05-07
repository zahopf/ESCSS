using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESCS.Web.Apps
{
    public class PageList<T> : List<T>
    {
        /// <summary>    
        /// 当前索引页   
        /// </summary>    
        public int PageIndex { get; private set; }
        /// <summary>    
        /// 每页显示数据条目    
        /// </summary>    
        public int PageSize { get; private set; }
        /// <summary>    
        /// 总页数    
        /// </summary>    
        public int TotalCount { get; private set; }

        public PageList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)//构造函数
        {
            PageIndex = pageIndex;//为本地变量赋值
            PageSize = pageSize;//为本地变量赋值
            TotalCount = totalCount;//为本地变量赋值
            AddRange(source);
        }
    }

}