using System.ComponentModel;
namespace ExchangeModel.Enum
{
    public enum BaudRate : int
    {
        [Description("9600")]
        BDR_9600 =9600,
        [Description("14400")]
        BDR_14400 = 14400,
        [Description("19200")]
        BDR_19200 = 19200,
        [Description("38400")]
        BDR_38400 = 38400,
        [Description("56000")]
        BDR_56000 = 56000,
        [Description("57600")]
        BDR_57600 = 57600,
        [Description("115200")]
        BDR_115200 = 115200,
        [Description("128000")]
        BDR_128000 = 128000,
        [Description("256000")]
        BDR_256000 = 256000,
    }
}