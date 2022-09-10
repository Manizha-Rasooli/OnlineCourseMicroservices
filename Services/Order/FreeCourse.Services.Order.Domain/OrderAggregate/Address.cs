using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    public class Address:ValueObject
    {
        #region Properties
        public string Province { get;private set; }
        public string District { get; private set; }
        public string Street{ get; private set; }
        public string ZipCode{ get; private set; }
        public string Line{ get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructure method set thw address properties values
        /// </summary>
        /// <param name="province"></param>
        /// <param name="district"></param>
        /// <param name="street"></param>
        /// <param name="zipCode"></param>
        /// <param name="line"></param>
        public Address(string province, string district, string street, string zipCode, string line)
        {
            Province = province;
            District = district;
            Street = street;
            ZipCode = zipCode;
            Line = line;
        }
        #endregion

        /// <summary>
        ///This Method can set the values that we want to set for zipcode
        /// </summary>
        /// <param name="zipCode"></param>
        public void SetZipCode(string zipCode)
        {
            // if we have any business roles; write here business code
            ZipCode = zipCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Province;
            yield return District;
            yield return Street;
            yield return ZipCode;
            yield return Line;
        }
    }
}
