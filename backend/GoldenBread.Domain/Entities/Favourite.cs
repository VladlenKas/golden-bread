using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public partial class Favourite
{
    public int FavouriteId { get; set; }

    public int AccountId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
