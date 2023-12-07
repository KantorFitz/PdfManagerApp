using System.ComponentModel;

namespace PdfManagerApp.Data;

public enum SearchFinishReason
{
    [Description("Nieznany problem")]
    Unknown,

    [Description("Wyszukiwanie zakończone")]
    FinishedNormally,

    [Description("Anulowano")]
    OperationCancelled,

    [Description("Wystąpił błąd")]
    ExceptionOccured,
}
