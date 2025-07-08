namespace pizdec
{
    internal static class Program
    {


        [STAThread]

        static void Main()

        {
            //przekazuje i startuje nie form, tylko kastomowy application context, aby stworzyc apke nie form based a notifyicon based
            ApplicationConfiguration.Initialize();
            ApplicationContext applicationContext = new CustomApplicationContext();
            Application.Run(applicationContext);
            
        }
    }
}