//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webshop
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderTable()
        {
            this.OrderLine = new HashSet<OrderLine>();
        }
    
        public int Order_ID { get; set; }
        public int Customer_ID { get; set; }
        public decimal PriceTotal { get; set; }
        public System.DateTime DateOrdered { get; set; }
        public string Street { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeliveryFirstName { get; set; }
        public string DeliveryLastName { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryStreet { get; set; }
        public string DeliveryZip { get; set; }
        public string Payment { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderLine> OrderLine { get; set; }
    }
}
