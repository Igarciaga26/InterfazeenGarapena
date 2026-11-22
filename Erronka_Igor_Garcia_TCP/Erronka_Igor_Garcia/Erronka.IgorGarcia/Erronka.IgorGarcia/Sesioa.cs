namespace Erronka.IgorGarcia
{
    public static class Sesioa
    {
        public static Erabiltzailea? Unekoa { get; set; }

        public static bool Admin
        {
            get { return Unekoa != null && Unekoa.Rola == "admin"; }
        }

        public static bool BiltegiaIkusi
        {
            get { return Unekoa != null && (Unekoa.Rola == "admin" || Unekoa.Rola == "langilea"); }
        }
    }
}