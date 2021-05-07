using ESCS.Domain;
using ESCS.Manager;
using ESCS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Component
{
    public class PurchaseOrderComponent : BaseComponent<PurchaseOrder, PurchaseOrderManager>, IPurchaseOrderService
    {
    }
}
