using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Text;

namespace ESCS.Web.Apps
{
    /// <summary>
    /// 分页控件的HTML呈现
    /// </summary>
    public class RenderPager
    {
        /// <summary>    
        /// 当前页面的ViewContext    
        /// </summary>    
        private ViewContext viewContext;
        /// <summary>    
        /// 当前页码    
        /// </summary>    
        private int currentPage;
        /// <summary>    
        /// 页面要显示的数据条数    
        /// </summary>    
        private int pageSize;
        /// <summary>    
        /// 总的记录数    
        /// </summary>    
        private int totalCount;
        /// <summary>    
        /// Pager Helper 要显示的页数    
        /// </summary>    
        private int toDisplayCount;

        private string pagelink;

        private string formId;

        public void RenderPagerMethod(ViewContext viewContext, int currentPage, int pageSize, int totalCount, int toDisplayCount)
        {
            this.viewContext = viewContext;
            this.currentPage = currentPage;
            this.pageSize = pageSize;
            this.totalCount = totalCount;
            this.toDisplayCount = toDisplayCount;
            string reqUrl = viewContext.RequestContext.HttpContext.Request.RawUrl;
            string link = "";

            Regex re = new Regex(@"pageIndex=(\d+)|pageIndex=", RegexOptions.IgnoreCase);

            MatchCollection results = re.Matches(reqUrl);

            if (results.Count > 0)
            {
                link = reqUrl.Replace(results[0].ToString(), "pageIndex=[%page%]");
            }
            else if (reqUrl.IndexOf("?") < 0)
            {
                link = reqUrl + "?pageIndex=[%page%]";
            }
            else
            {
                link = reqUrl + "&pageIndex=[%page%]";
            }
            this.pagelink = link;
        }

        public RenderPager(ViewContext viewContext, int currentPage, int pageSize, int totalCount, int toDisplayCount)
        {
            RenderPagerMethod(viewContext, currentPage, pageSize, totalCount, toDisplayCount);
        }

        public RenderPager(ViewContext viewContext, int currentPage, int pageSize, int totalCount, int toDisplayCount, string formId)
        {
            this.formId = formId;
            RenderPagerMethod(viewContext, currentPage, pageSize, totalCount, toDisplayCount);
        }

        public string RenderHtml()
        {
            if (totalCount <= pageSize)
                return string.Empty;

            //总页数    
            int pageCount = (int)Math.Ceiling(this.totalCount / (double)this.pageSize);

            //起始页    
            int start = 1;

            //结束页    
            int end = toDisplayCount;
            if (pageCount < toDisplayCount) end = pageCount;

            //中间值    
            int centerNumber = toDisplayCount / 2;

            if (pageCount > toDisplayCount)
            {

                //显示的第一位    
                int topNumber = currentPage - centerNumber;

                if (topNumber > 1)
                {
                    start = topNumber;
                }

                if (topNumber > pageCount - toDisplayCount)
                {
                    start = pageCount - toDisplayCount + 1;
                }

                //显示的最后一位    
                int endNumber = currentPage + centerNumber;

                if (currentPage >= pageCount - centerNumber)
                {
                    end = pageCount;
                }
                else
                {
                    if (endNumber > toDisplayCount)
                    {
                        end = endNumber;
                    }
                }

            }

            StringBuilder sb = new StringBuilder();

            //Previous    
            if (this.currentPage > 1)
            {
                sb.Append(GeneratePageLink("<前一页", this.currentPage - 1));
            }

            if (start > 1)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append("...");
            }

            //end Previous    

            for (int i = start; i <= end; i++)
            {
                if (i == this.currentPage)
                {
                    sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i));
                }
            }

            //Next    
            if (end < pageCount)
            {
                sb.Append("...");
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }

            if (this.currentPage < pageCount)
            {
                sb.Append(GeneratePageLink("后一页>", this.currentPage + 1));
            }
            //end Next    

            sb.Append(" <span><input type=\"text\" class=\"pagerInput\" id=\"pagerInput\" maxlength=\"4\" onkeypress=\"return event.keyCode>=48&&event.keyCode<=57\"/></span> ");
            if (!string.IsNullOrEmpty(this.formId))
            {
                sb.Append(" <span><input type=\"button\" value=\"跳转\" class=\"btn3\" onclick=\"SubmitForm(" + this.formId + ",pagerInput.value)\"/></span>");
            }
            else
            {
                sb.Append(" <span><input type=\"button\" value=\"跳转\" class=\"btn3\" onclick=\"if (!isNaN(parseInt(pagerInput.value))) window.location='" + this.pagelink.Replace("[%page%]", "") + "'+pagerInput.value;\"/></span>");
            }
            if (!string.IsNullOrEmpty(this.formId))
            {
                sb.Append("<script language=\"javascript\" type=\"text/javascript\">function SubmitForm(id, pageIndex) {"
                    + "if ($('#form1').find(\"input[id='pageIndex']\").length < 1) { $('#form1').append('<input name=\"pageIndex\" type=\"hidden\" id=\"pageIndex\"/>');"
                    + "}$(\"#pageIndex\").val(pageIndex);$('#form1').submit();}</script>");
            }
            return sb.ToString();
        }

        /// <summary>    
        /// 生成Page的链接    
        /// </summary>    
        /// <param name="linkText">文字</param>    
        /// <param name="PageNumber">页码</param>    
        /// <returns></returns>    
        private string GeneratePageLink(string linkText, int PageNumber)
        {

            string linkFormat = string.Empty;
            if (!string.IsNullOrEmpty(this.formId))
            {
                linkFormat = string.Format(" <a href=\"#\" onclick=\"SubmitForm(" + this.formId + ",{0})\">{1}</a> ", PageNumber, linkText);
            }
            else
            {
                string link =this.pagelink.Replace("[%page%]", PageNumber.ToString());
                linkFormat = string.Format(" <a href=\"{0}\">{1}</a> ", link, linkText);
            }
            return linkFormat;
        }
    }
}