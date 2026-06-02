using System;

namespace Frontend
{
	public sealed class Singleton
	{
        private static Singleton instance = null;
        public string name;
        public string Lname;
        public string email;
        public string id;

        private Singleton()
        {
            name = "";
            Lname = "";
            email = "";
            id = "";
        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}