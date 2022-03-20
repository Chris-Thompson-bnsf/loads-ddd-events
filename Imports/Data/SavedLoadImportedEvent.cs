using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.EfCore;

namespace Imports.Data;

[Table("LoadImportedEvent")]
public class SavedLoadImportedEvent : EntityBase
{
    [StringLength(60)]
    public string BolNumber { get; init; }

    [StringLength(60)]
    public string CustomerCode { get; init; }

    public string DetailsJson { get; init; }
}