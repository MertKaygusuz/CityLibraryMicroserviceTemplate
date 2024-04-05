using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibrary.Shared.DbBase.SQL
{
    public interface ISoftDeletable
    {
        public DateTime? DeletedAt { get; set; }

        public string DeletedBy { get; set; }
    }
}
