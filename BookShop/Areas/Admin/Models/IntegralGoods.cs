//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookShop.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class IntegralGoods
    {
        public IntegralGoods()
        {
            this.OrderIntegral = new HashSet<OrderIntegral>();
        }
    
        public int Id { get; set; }
        public int BookId { get; set; }
        public int PaymentIntegral { get; set; }
        public int IntegralSectionId { get; set; }
    
        public virtual Books Books { get; set; }
        public virtual ICollection<OrderIntegral> OrderIntegral { get; set; }
        public virtual IntegralSection IntegralSection { get; set; }
    }
}
