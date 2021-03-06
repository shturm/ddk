﻿using System.ComponentModel.DataAnnotations;

namespace Ddk.Web.Data.Entities
{
    public enum OrderStatus
    {
        [Display(Name = "Изчакваща")]
        Pending = 10,
        [Display(Name = "Изпратена")]
        Sent = 20,
        [Display(Name = "Затворена")]
        Cancelled = 30,
        [Display(Name = "Върната")]
        Returned = 40
    }
}
