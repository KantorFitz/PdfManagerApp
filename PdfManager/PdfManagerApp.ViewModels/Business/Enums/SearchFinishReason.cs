using System.ComponentModel;

namespace PdfManagerApp.ViewModels.Business.Enums;

public enum SearchFinishReason
{
    [Description("Nieznany problem")] 
    Unknown,

    [Description("Wyszukiwanie zakończone")]
    FinishedNormally,

    [Description("Anulowano")] 
    OperationCancelled,

    [Description("Wystąpił błąd")] 
    ExceptionOccured
}
