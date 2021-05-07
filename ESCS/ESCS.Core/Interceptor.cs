using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Transactions;

namespace ESCS.Core
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public class Interceptor : IInterceptor
    {
        public void PreProceed()
        {
            //调用方法前的代码
        }

        /// <summary>
        /// 判断是否需要执行事务操作
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private bool GetNeedTransaction(IInvocation invocation)
        {
            string methodName = invocation.Method.Name.ToUpper();//获取当前调用的方法名称
            if (methodName.Contains("GET") || methodName.Contains("FIND"))//如果是“查询”方法就不需要开启事务
            {
                return false; //不需要开启事务管理
            }
            return true;
        }

        /// <summary>
        /// 拦截器的核心处理，处理事务，日志等信息
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            DoMethod(invocation);
            return;

            bool isNeedTransaction = GetNeedTransaction(invocation);//默认需要开启事务，此变量用于确定是否需要开启事务
            if (isNeedTransaction)
            {                
                //注意：必须开启系统的“Distributed Transaction Coordinator”服务
                using (TransactionScope tsCope = new TransactionScope(TransactionScopeOption.RequiresNew))//实例化一个事务
                {
                    try
                    {
                        DoMethod(invocation);
                        tsCope.Complete();//提交事务
                    }
                    catch (Exception ex)
                    {
                        WriteErrorLog(ex);
                    }
                    finally
                    {
                        tsCope.Dispose();
                    }
                }
            }
            else
            {
                DoMethod(invocation);
            }
        }

        private void DoMethod(IInvocation invocation)
        {
            PreProceed();//调用Create/User方法前要做事，比如验证权限
            invocation.Proceed();//调用Create方法
            PostProceed();//调用Create/User方法后要做的事，比如记录日志
        }

        private void WriteErrorLog(Exception ex)
        { 
            
        }

        public void PostProceed()
        {
            //调用方法顺利完成后的代码
        }
    }
}
