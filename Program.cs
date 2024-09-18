namespace PortSniffer;

class Program
{
    static void StartOver(string[] args, OutPutModel outputModel)
    {
        Console.Write("Want to check another domain/IP [Y/N]: ");
        string? response = Console.ReadLine();
        if (response.ToUpper().Equals("Y"))
        {
            Finish(outputModel);
            Main(args);
        }
    }

    static void Finish(OutPutModel outputModel)
    {
        outputModel.DeleteResultsFile();
    }

    async static Task Main(string[] args)
    {
        InputModel inputModel = new InputModel();
        inputModel.Input();
        PortSnifferModel portSniffer = new PortSnifferModel(inputModel.GetIPList());
        inputModel.InitByFlags(ref portSniffer, ref portSniffer.OutputModel);
        OutPutModel outputModel = portSniffer.OutputModel;
        await portSniffer.CheckAll();
        outputModel.OpenResultsFile();
        StartOver(args, outputModel);
        Finish(outputModel);
    }
}
