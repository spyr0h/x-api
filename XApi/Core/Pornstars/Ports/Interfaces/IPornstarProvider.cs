﻿using XApi.Core.Pornstars.Models;

namespace XApi.Core.Pornstars.Ports.Interfaces;

public interface IPornstarProvider
{
    Task<IList<Pornstar>> ProvideAllPornstars();
    Task<IList<Pornstar>> ProvidePornstarsForIds(int[] ids);
}
