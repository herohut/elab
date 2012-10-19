using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbTest
{
    public partial class PersonData
    {
        public int? PersonId
        {
            get
            {
                if (this.C__Id_User != null)
                    return this.C__Id_User;
                if (this.C__IdClient != null)
                    return this.C__IdClient;
                if (this.C__IdClientUser != null)
                    return this.C__IdClientUser;
                if (this.C__IdGenericPerson != null)
                    return this.C__IdGenericPerson;
                throw new Exception("Data like shit");
            }
        }

        public int? GetPersonId()
        {
            if (this.C__Id_User != null)
                return this.C__Id_User;
            if (this.C__IdClient != null)
                return this.C__IdClient;
            if (this.C__IdClientUser != null)
                return this.C__IdClientUser;
            if (this.C__IdGenericPerson != null)
                return this.C__IdGenericPerson;
            throw new Exception("Data like shit");
        }
    }

    public partial class DataField
    {
        public int? GetPersonId()
        {
            if (this.C__Id_User != null)
                return this.C__Id_User;
            if (this.C__IdClient != null)
                return this.C__IdClient;
            if (this.C__IdClientUser != null)
                return this.C__IdClientUser;
            if (this.C__IdGenericPerson != null)
                return this.C__IdGenericPerson;
            throw new Exception("Data like shit");
        }
        public int? PersonId
        {
            get
            {
                if (this.C__Id_User != null)
                    return this.C__Id_User;
                if (this.C__IdClient != null)
                    return this.C__IdClient;
                if (this.C__IdClientUser != null)
                    return this.C__IdClientUser;
                if (this.C__IdGenericPerson != null)
                    return this.C__IdGenericPerson;
                throw new Exception("Data like shit");
            }
        }

        public object Value
        {
            get
            {
                if (this.CompostiteGenericDatas.Count != 0)
                    return this.DateTimeGenericDatas.Single();
                if (this.StringGenericDatas.Count != 0)
                    return this.StringGenericDatas.Single();
                if (this.TreeListGenericDatas.Count != 0)
                    return this.TreeListGenericDatas.Single();
                if (this.DateTimeGenericDatas.Count != 0)
                    return this.DateTimeGenericDatas.Single();

                throw new Exception("Data like shit");
            }
        }
    }
}
