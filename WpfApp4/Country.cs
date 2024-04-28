using System;
using System.Collections.Generic;

namespace WpfApp4;

public partial class Country
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public decimal SurfaceArea { get; set; }

    public int Population { get; set; }

    public int? Capital { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
