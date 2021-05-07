using ESCS.Domain;
using ESCS.Manager;
using ESCS.Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Component
{
    public class SalesOrderDetailsComponent : BaseComponent<SalesOrderDetails, SalesOrderDetailsManager>, ISalesOrderDetailsService
    {
    }
}
